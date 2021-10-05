/******************************************************************************
**
** @file speedway.cs
**
** Last update: Jan 24, 2008
**
** This file provides speedway reader implementation.
** 
******************************************************************************/



/*
 *****************************************************************************

 * IMPINJ EXPRESSLY DISCLAIMS ANY AND ALL WARRANTIES CONCERNING THIS SOFTWARE AND 
 * DOCUMENTATION, INCLUDING ANY WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR 
 * ANY PARTICULAR PURPOSE, AND WARRANTIES OF PERFORMANCE, AND ANY WARRANTY THAT 
 * MIGHT OTHERWISE ARISE FROM COURSE OF DEALING OR USAGE OF TRADE. NO WARRANTY IS 
 * EITHER EXPRESS OR IMPLIED WITH RESPECT TO THE USE OF THE SOFTWARE OR DOCUMENTATION. 
 * Under no circumstances shall Impinj be liable for incidental, special, indirect, 
 * direct or consequential damages or loss of profits, interruption of business, 
 * or related expenses which may arise from use of software or documentation, 
 * including but not limited to those resulting from defects in software and/or 
 * documentation, or loss or inaccuracy of data of any kind.
 * 
 * (c) Copyright Impinj, Inc. 2007. All rights reserved.  

 *****************************************************************************/


using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Runtime.Remoting;
using System.Collections;
using System.Xml;
using System.Xml.Serialization;
using System.Data;

namespace CSL.Mach1
{
    /// <summary>
    /// Pre-defined delegates that are used for provent event-based notification
    /// </summary>
    /// <param name="bmn"></param>
    #region Delegates 
    public delegate void delegateBootModemNtf(MANAGEMENT_NTF.BOOT_MODEM_NTF bmn);
    public delegate void delegateGPIAlertNtf(MANAGEMENT_NTF.GPI_ALERT_NTF gan);
    public delegate void delegateSocketConnectionStatusNtf(MANAGEMENT_NTF.SOCKET_CONNECTION_STATUS_NTF scsn);
    public delegate void delegateStatusReportNtf(MANAGEMENT_NTF.STATUS_REPORT_NTF srn);
    public delegate void delegateSystemErrorNtf(MANAGEMENT_NTF.SYSTEM_ERROR_NTF sen);
    public delegate void delegateTemperatureAlarmNtf(MANAGEMENT_NTF.TEMPERETURE_ALARM_NTF tan);
    public delegate void delegateInventoryNtf(OPERATION_NTF.INVENTORY_NTF inv);
    public delegate void delegateAntennaAlertNtf(OPERATION_NTF.ANTENNA_ALERT_NTF aan);
    public delegate void delegateInventoryStatusNtf(OPERATION_NTF.INVENTORY_STATUS_NTF isn);
    public delegate void delegateRfSurveyNtf(OPERATION_NTF.RF_SURVEY_NTF rsn);
    public delegate void delegateModemStoppedNtf(OPERATION_NTF.MODEM_STOPPED_NTF msn);
    public delegate void delegateAccumulateStatusNtf(OPERATION_NTF.ACCUMULATION_STATUS_NTF asn);
    public delegate void delegateProcessMach1Message(MACH1_FRAME mf);

    
    public delegate void delegateTagRead(Tag tag);
    #endregion 

    /// <summary>
    /// Command return results
    /// </summary>
    public enum CMD_RETURN
    {
        COMMAND_SUCESS = 0,
        CMD_RESPONSE_TIME_OUT,
        COMMAND_FAILED,
        RESPONSE_CAN_NOT_RECOGNIZED,
        INVALID_COMMAND
    }

    [Serializable]
    public class SpeedwayManualResetEvent
    {
        public ManualResetEvent evt;
        public byte[] data;

        public SpeedwayManualResetEvent(bool status)
        {
            evt = new ManualResetEvent(status);
        }
    }

    [Serializable]
    public class TIME_STAMP
    {
        public UInt32 seconds;
        public UInt32 u_seconds;
    }

    [Serializable]
    [XmlInclude(typeof(Tag))]
    public class Tag
    {
        [XmlElement("EPC")]
        public string epc;

        [XmlElement("Reader")]
        public string reader_name;

        [XmlElement("Antenna")]
        public int antenna;

        [XmlElement("Rssi")]
        public int rssi;

        [XmlElement("PhaseI")]
        public int phaseI;
        [XmlElement("PhaseQ")]
        public int phaseQ;

        [XmlElement("Frequency")]
        public float frequency;

        [XmlElement("TimeStamp")]
        public TIME_STAMP timeStamp = new TIME_STAMP();

        public override string ToString()
        {
            string s;
            s = string.Format("<Tag><ID>{0}</ID><Reader>{1}</Reader><Antenna>{2}</Antenna><RSSI>{3}</RSSI><TimeStamp><Second>{4}</Second><u_Second>{5}</u_Second></TimeStamp></Tag>", epc, reader_name, antenna, rssi, timeStamp.seconds, timeStamp.u_seconds);
            return s;
        }
    }

    /// <summary>
    /// Speedway Reader Parameters
    /// </summary>
    [Serializable]
    public class CSLReaderSettings : MarshalByRefObject
    {

        [Serializable]
        public class Gen2Params
        {
            public int inventory_mode = 0;
            public int session = 0;
            public int auto_set_mode = 2;
            public int mode_id = 1;
            public int population = 32;
        }

        [Serializable]
        public class ReaderInformation
        {
            public string reader_name = "default";
            public int region = 0;
            public int frequency_mode;
            public ushort[] frequencies;
            public ushort[] reduced_power_frequencies;

            //Bug
            //Bug reported by Xiaoyong Su
            //Bug Fixed by Xiaoyong Su, 01/09/2008
            public ushort frequency;
            
            /// <summary>
            /// 0 - auto select, 1 - 4 seconds
            /// </summary>
            public int lbt_time;    

            public string software_ver = string.Empty;
            public string firmware_ver = string.Empty;
            public string fpga_ver = string.Empty;
        }

        [Serializable]
        public class Antenna
        {
            public bool enabled = false;
            public float power = 30;
            public short rssi = -60;
        }

        public Gen2Params gen2_params;
        public ReaderInformation reader_information;
        public Antenna[] antennas = new Antenna[4];
        public bool maximum_sensitivity = true;
        public bool enable_buffering = false;
       

        public CSLReaderSettings()
        {
            gen2_params = new Gen2Params();
            reader_information = new ReaderInformation();
            for (int i = 0; i < 4; i++) antennas[i] = new Antenna();
        }

        public CSLReaderSettings(string xmlstring)
        {
            byte[] data = System.Text.Encoding.ASCII.GetBytes(xmlstring);
            
            System.IO.MemoryStream ms = new System.IO.MemoryStream();   
            ms.Write(data, 0, data.Length);
            ms.Position = 0; 
            XmlSerializer serializer = new XmlSerializer(typeof(CSLReaderSettings));
            CSLReaderSettings settings = (CSLReaderSettings)serializer.Deserialize(ms);

            gen2_params = settings.gen2_params;
            reader_information = settings.reader_information;
            antennas = settings.antennas;

            //Bug
            //Reported by Xiaoyong Su, 01/09/2008
            //Fixed by Xiaoyong Su, 01/09/2008
            maximum_sensitivity = settings.maximum_sensitivity;
            enable_buffering = settings.enable_buffering;
        }

        public override string ToString()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(CSLReaderSettings));
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            XmlTextWriter xtw = new XmlTextWriter(ms, Encoding.ASCII);
            serializer.Serialize(xtw, this);

            byte[] data = ms.GetBuffer();
            return System.Text.Encoding.ASCII.GetString(data);
        }
    }

    /// <summary>
    /// Use to contain asyn. read data
    /// </summary>
    /// 
    [Serializable]
    public class AsynReadState
    {
        public byte[] data;
        public AsynReadState(int buffer_size)
        {
            data = new byte[buffer_size];
        }
    }

    /// <summary>
    /// Speedway reader implementation class
    /// </summary>
    /// 
    [Serializable]
    public class CS461Reader : MarshalByRefObject, IDisposable 
    {
        #region Reader control parameters
        //will be defined later
        #endregion

        #region Adjustable Parameters
        public int MODEM_INIT_TIME_OUT = 8000;          //Modem initialization time out
        public int CMD_RESPONSE_TIME_OUT = 500;         //General command response time out
        public int MODEM_STOP_TIME_OUT = 2000;          //Modem stop time out
        public int NOTIFICTION_TIME_OUT = 500;          //Notification time out
        #endregion

        #region Members
        public const UInt16 buffer_size = 2048;         //used to store data read from socket
        public bool include_timestamp = false;          //By default, all command send to reader do not include timestamp        
        public CSLReaderSettings reader_setting = new CSLReaderSettings();

        #region Private Members
        private const int port = 49380;                 //Speedway default port to accept message is 49380
        private bool connected = false;                 //connnect status
        private TcpClient client;                       //tcp client for commnunicate with reader
        private NetworkStream ns;                       
        private AsynReadState read_state;
        private InvalidCommandNtf lastError;            //The last command error
        private byte[] reserved_data = null;            //used to store un-complete data from last read
        private ArrayList inventoryBuffer;              //Buffer for containing inventory data
        private TIME_STAMP inventory_start_time = new TIME_STAMP();
        private TIME_STAMP inventory_stop_time = new TIME_STAMP();
        #endregion

        #endregion

        #region Events
        public event delegateBootModemNtf onBootModemNtfReceived;
        public event delegateGPIAlertNtf onGPIAlertNtfReceived;
        public event delegateSocketConnectionStatusNtf onSocketConnectionStatusNtfReceived;
        public event delegateStatusReportNtf onStatusReportNtfReceived;
        public event delegateSystemErrorNtf onSystemErrorNtfReceived;
        public event delegateTemperatureAlarmNtf onTemperatureAlarmNtfReceived;
        public event delegateInventoryNtf onInventoryNtfReceived;
        public event delegateAntennaAlertNtf onAntennaAlertNtfReceived;
        public event delegateInventoryStatusNtf onInventoryStatusNtfReceived;
        public event delegateRfSurveyNtf onRFSurveyNtfReceived;
        public event delegateModemStoppedNtf onModemStopNtfReceived;
        public event delegateAccumulateStatusNtf onAccumulationStatusNtfReceived;
        public event delegateTagRead onTagRead;

        //public event
        #endregion

        #region reader events
        SpeedwayManualResetEvent bootModemNtfEvent;
        SpeedwayManualResetEvent bootModemRspEvent;
        SpeedwayManualResetEvent getMCSVersionRspEvent;
        SpeedwayManualResetEvent getReaderInfoRspEvent;
        SpeedwayManualResetEvent getStateRspEvent;
        SpeedwayManualResetEvent shutdownModemRspEvent;
        SpeedwayManualResetEvent getTemperatureAlarmRspEvent;
        SpeedwayManualResetEvent setTemperatureAlarmRspEvent;
        SpeedwayManualResetEvent setGPORspEvent;
        SpeedwayManualResetEvent setGPIRspEvent;
        SpeedwayManualResetEvent getGPIRspEvent;
        SpeedwayManualResetEvent setStatusReportRspEvent;
        SpeedwayManualResetEvent setTcpConnectionOptionsRspEvent;
        SpeedwayManualResetEvent getTcpConnectionOptionsRspEvent;
        SpeedwayManualResetEvent getOCSVersionRspEvent;
        SpeedwayManualResetEvent loadFromProfileRspEvent;
        SpeedwayManualResetEvent storeToProfileRspEvent;
        SpeedwayManualResetEvent setTxPowerRspEvent;
        SpeedwayManualResetEvent getTxPowerRspEvent;
        SpeedwayManualResetEvent getSupportedGen2ParamsRspEvent;
        SpeedwayManualResetEvent setAntennaRspEvent;
        SpeedwayManualResetEvent getAntennaRspEvent;
        SpeedwayManualResetEvent setTxFrequencyRspEvent;
        SpeedwayManualResetEvent getTxFrequencyRspEvent;
        SpeedwayManualResetEvent setGen2ParamsRspEvent;
        SpeedwayManualResetEvent getGen2ParamsRspEvent;
        SpeedwayManualResetEvent checkAntennaRspEvent;
        SpeedwayManualResetEvent checkAntennaNtfEvent;
        SpeedwayManualResetEvent setInventoryReportRspEvent;
        SpeedwayManualResetEvent setLBTParamsRspEvent;
        SpeedwayManualResetEvent getLBTParamRspEvent;
        SpeedwayManualResetEvent inventoryContinueRspEvent;
        SpeedwayManualResetEvent inventoryRspEvent;
        SpeedwayManualResetEvent modemStopRspEvent;
        SpeedwayManualResetEvent rfSurveyRspEvent;
        SpeedwayManualResetEvent tagReadRspEvent;
        SpeedwayManualResetEvent tagWriteRspEvent;
        SpeedwayManualResetEvent tagLockRspEvent;
        SpeedwayManualResetEvent tagKillRspEvent;
        SpeedwayManualResetEvent tagCustomRspEvent;
        SpeedwayManualResetEvent tagReadNtfEvent;
        SpeedwayManualResetEvent tagWriteNtfEvent;
        SpeedwayManualResetEvent tagLockNtfEvent;
        SpeedwayManualResetEvent tagKillNtfEvent;
        SpeedwayManualResetEvent invalidCommandNtfEvent;
        SpeedwayManualResetEvent setRxConfigRspEvent;
        SpeedwayManualResetEvent getRxConfigRspEvent;
        SpeedwayManualResetEvent setRegionNtfEvent;
        SpeedwayManualResetEvent setRegionRspEvent;
        SpeedwayManualResetEvent reportInventoryRspEvent;
        SpeedwayManualResetEvent getCapabilitiesRspEvent;
        SpeedwayManualResetEvent testWriteRspEvent;
        SpeedwayManualResetEvent testWriteNtfEvent;
        SpeedwayManualResetEvent setProfileSequenceRspEvent;

        #endregion

        #region Trigger Asyn Function Call
        
        #region Operation NTF
        private void TriggerAsynInventory(OPERATION_NTF.INVENTORY_NTF inv)
        {
            if (onInventoryNtfReceived != null) onInventoryNtfReceived(inv);
        }
        private void TriggerAsynAntennaAlert(OPERATION_NTF.ANTENNA_ALERT_NTF aan)
        {
            if(onAntennaAlertNtfReceived!=null)onAntennaAlertNtfReceived(aan);
        }
        private void TriggerAsynRfSurvey(OPERATION_NTF.RF_SURVEY_NTF rsn)
        {
            if (onRFSurveyNtfReceived != null) onRFSurveyNtfReceived(rsn);
        }
        private void TriggerAsynModemStopped(OPERATION_NTF.MODEM_STOPPED_NTF msn)
        {
            if (onModemStopNtfReceived != null) onModemStopNtfReceived(msn);
        }
        private void TriggerAsynInventoryStatus(OPERATION_NTF.INVENTORY_STATUS_NTF isn)
        {
            if (onInventoryStatusNtfReceived != null) onInventoryStatusNtfReceived(isn);
        }
        private void TriggerAsynAccumulationStatus(OPERATION_NTF.ACCUMULATION_STATUS_NTF asn)
        {
            if (onAccumulationStatusNtfReceived != null) onAccumulationStatusNtfReceived(asn);
        }

        private void TriggerAsynTagRead(Tag tag)
        {
            if (onTagRead != null) onTagRead(tag);
        }
        #endregion 

        #region Management NTF
        private void TriggerAsynTemperatureAlarm(MANAGEMENT_NTF.TEMPERETURE_ALARM_NTF tan)
        {
            if (onTemperatureAlarmNtfReceived != null) onTemperatureAlarmNtfReceived(tan);
        }
        private void TriggerAsynSystemError(MANAGEMENT_NTF.SYSTEM_ERROR_NTF sen)
        {
            if (onSystemErrorNtfReceived != null) onSystemErrorNtfReceived(sen);
        }
        private void TriggerAsynStatusReport(MANAGEMENT_NTF.STATUS_REPORT_NTF srn)
        {
            if (onStatusReportNtfReceived != null) onStatusReportNtfReceived(srn);
        }
        private void TriggerAsynGPIAlert(MANAGEMENT_NTF.GPI_ALERT_NTF gan)
        {
            if (onGPIAlertNtfReceived != null) onGPIAlertNtfReceived(gan);
        }
        private void TriggerAsynModemBootStatus(MANAGEMENT_NTF.BOOT_MODEM_NTF bmn)
        {
            if (onBootModemNtfReceived != null) onBootModemNtfReceived(bmn);
        }
        private void TriggerSocketConnectionStatus(MANAGEMENT_NTF.SOCKET_CONNECTION_STATUS_NTF ssn)
        {
            if (onSocketConnectionStatusNtfReceived != null) onSocketConnectionStatusNtfReceived(ssn);
        }
        #endregion 

        #endregion

        #region Properties

        /// <summary>
        /// Check if the reader is connected.
        /// </summary>
        public bool Connected
        {
            get { return connected; }
        }

        /// <summary>
        /// Return the speedway reader settings.
        /// </summary>
        public CSLReaderSettings Settings
        {
            get { return reader_setting; }
        }

        /// <summary>
        /// Return inventory start time
        /// </summary>
        public TIME_STAMP InventoryStartTime
        {
            get { return inventory_start_time; }
        }

        /// <summary>
        /// Return invenotry stop time
        /// </summary>
        public TIME_STAMP InventoryStopTime
        {
            get { return inventory_stop_time; }
        }


        #endregion

        /// <summary>
        /// Remoting lease time is disabled
        /// </summary>
        /// <returns></returns>
        public override object InitializeLifetimeService()
        {
            return null;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public CS461Reader()
        {
            inventoryBuffer = new ArrayList();
        }


        private void FlushInventoryBuffer()
        {
            inventoryBuffer = new ArrayList();
        }

        /// <summary>
        /// Return buffered inventory data
        /// </summary>
        /// <returns></returns>
        public DataSet GetBufferedInventoryData()
        {
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            dt.Columns.Add("EPC");
            dt.Columns.Add("Antenna");
            dt.Columns.Add("Rssi");
            dt.Columns.Add("TimeStamp_Sec");
            dt.Columns.Add("TimeStamp_uSec");

            ds.Tables.Add(dt);
            
            foreach (Tag tag in inventoryBuffer)
            {
                ds.Tables[0].Rows.Add(new string[]{tag.epc, tag.antenna.ToString(), tag.rssi.ToString(), tag.timeStamp.seconds.ToString(), tag.timeStamp.u_seconds.ToString()});
            }

            return ds;
        }

        /// <summary>
        /// Apply reader setting to connected reader.
        /// </summary>
        /// <param name="setting"></param>
        /// <returns></returns>
        public bool ApplyReaderSetting(CSLReaderSettings setting)
        {
            if (!connected)
            {
                if (!Initialize(setting.reader_information.reader_name, (REGULATORY_REGION)setting.reader_information.region))             
                    return false;
            }

            CMD_RETURN ret;

            //Set Antenna
            byte antennas = 0;
            for(int i=0; i<4; i++)antennas |= (byte)(setting.antennas[i].enabled? (1<<i):0);
            OPERATION_CMD.SET_ANTENNA_RESULT sar;
            
            ret = SetAntenna(antennas, out sar);
            if (ret != CMD_RETURN.COMMAND_SUCESS)return false;

            //Set gen2 parameters
            OPERATION_CMD.SET_GEN2_PARAMS_RESULT spr;
            OPERATION_CMD.GEN2_PARAM gen2Param = new OPERATION_CMD.GEN2_PARAM();
            gen2Param.auto_set_mode = (OPERATION_CMD.GEN2_PARAM.GEN2_LINK_MODE)setting.gen2_params.auto_set_mode;
            gen2Param.inv_search_mode = (OPERATION_CMD.GEN2_PARAM.INVENTORY_SEARCH_MODE)(setting.gen2_params.inventory_mode);
            gen2Param.mode_id = (OPERATION_CMD.GEN2_PARAM.MODE_ID)setting.gen2_params.mode_id;
            gen2Param.session = (OPERATION_CMD.GEN2_PARAM.SESSION)setting.gen2_params.session;

            ret = SetGen2Params(gen2Param, out spr);
            if (ret != CMD_RETURN.COMMAND_SUCESS)return false;

            //Set Sensitivity
            /*if(!setting.maximum_sensitivity)
            {
                short[] sens = new short[4] { setting.antennas[0].rssi, setting.antennas[1].rssi, setting.antennas[2].rssi, setting.antennas[3].rssi};
                ret = SetRxConfig(OPERATION_CMD.SET_RX_SENSITIVITY_MODE.FIXED_PER_ANTENNA, sens, out err);

                if (ret != CMD_RETURN.COMMAND_SUCESS)return false;
            }*/


            //Set Tx Power
            OPERATION_CMD.SET_TX_POWER_RESULT stpr;
            float[] power = new float[]{setting.antennas[0].power,  
                setting.antennas[0].power,
                setting.antennas[1].power,
                setting.antennas[2].power,
                setting.antennas[3].power};

            ret = SetTxPower(power, out stpr);
            if (ret != CMD_RETURN.COMMAND_SUCESS) return false;

            //Set Tx Frequency
            OPERATION_CMD.SET_FREQUENCY_RESULT sfr;
            OPERATION_CMD.FREQUENCY_SET_MODE mode = (OPERATION_CMD.FREQUENCY_SET_MODE)setting.reader_information.frequency_mode;
            switch (mode)
            {
                case OPERATION_CMD.FREQUENCY_SET_MODE.CENTER_FREQUENCY:
                    ret = SetTxFrequency(OPERATION_CMD.FREQUENCY_SET_MODE.CENTER_FREQUENCY, setting.reader_information.frequency, null, out sfr);
                    break;
                case OPERATION_CMD.FREQUENCY_SET_MODE.CHOOSE_FROM_LIST:
                    ret = SetTxFrequency(OPERATION_CMD.FREQUENCY_SET_MODE.CHOOSE_FROM_LIST, 0, setting.reader_information.frequencies, out sfr);
                    break;
                case OPERATION_CMD.FREQUENCY_SET_MODE.REDUCED_POWER_FREQUENCY_LIST:

                    break;
                case OPERATION_CMD.FREQUENCY_SET_MODE.CHOOSE_FROM_REGULATORY:
                default:
                    ret = SetTxFrequency(OPERATION_CMD.FREQUENCY_SET_MODE.CHOOSE_FROM_REGULATORY, 0, null, out sfr);
                    break;
            }
            if (ret != CMD_RETURN.COMMAND_SUCESS)return false;


            reader_setting = setting;
            return true;
        }

        /// <summary>
        /// Get reader settings
        /// </summary>
        /// <param name="setting"></param>
        /// <returns></returns>
        public void GetReaderSettings(out CSLReaderSettings setting)
        {
            CMD_RETURN  ret;

            setting = new CSLReaderSettings();
            setting.reader_information.reader_name = this.reader_setting.reader_information.reader_name;
            setting.reader_information.region = this.reader_setting.reader_information.region;

            //Get antenna information
            bool[] ant_port_status = new bool[]{false, false, false, false};
            ret = GetAntenna(out ant_port_status);
            if (ret == CMD_RETURN.COMMAND_SUCESS) for (int i = 0; i < 4; i++) setting.antennas[i].enabled = ant_port_status[i];
            
            //Get gen2 parameters
            OPERATION_CMD.GEN2_PARAM gen2_params;
            ret = GetGen2Params(true, out gen2_params);
            if (ret == CMD_RETURN.COMMAND_SUCESS)
            {
                setting.gen2_params.session = (int)gen2_params.session;
                setting.gen2_params.mode_id = (int)gen2_params.mode_id;
                setting.gen2_params.inventory_mode = (int)gen2_params.inv_search_mode;
                setting.gen2_params.auto_set_mode = (int)gen2_params.auto_set_mode;
            }

            //Get reader software information
            MANAGEMENT_CMD.READER_INFO_RSP ris;
            ret = GetReaderInfo(out ris);
            if (ret == CMD_RETURN.COMMAND_SUCESS)
            {
                setting.reader_information.software_ver = ris.software_version;
                setting.reader_information.firmware_ver = ris.firmware_verison;
                setting.reader_information.fpga_ver = ris.FPGA_version;
            }

            //Get Tx Power
            float[] powers = new float[4];
            ret = GetTxPower(out powers);
            if (ret == CMD_RETURN.COMMAND_SUCESS) 
                for (int i = 0; i < 4; i++)
                {
                    if (powers.Length == 4)
                        setting.antennas[i].power = powers[i];
                    else
                        setting.antennas[i].power = powers[0];
                };

            //Get Tx Frequency
            OPERATION_CMD.FREQUENCY_SET_MODE freq_mode;
            ret = GetTxFrequency(out freq_mode,
                out setting.reader_information.frequency, 
                out setting.reader_information.frequencies, out setting.reader_information.reduced_power_frequencies);

            setting.reader_information.frequency_mode = (int)freq_mode;
        }

 
        /// <summary>
        /// Initialize Speedway reader
        /// </summary>
        /// <param name="deviceName">Device name or IP address</param>
        /// <param name="region">Regulatory region</param>
        /// <returns></returns>
        public bool Initialize(string deviceName, REGULATORY_REGION region)
        {
            try
            {
                client = new TcpClient(deviceName, port);

                ns = client.GetStream();
                if (ns == null) return false;
            }
            catch
            {
                return false;
            }

            read_state = new AsynReadState(buffer_size);
            ns.BeginRead(read_state.data, 0, read_state.data.Length, new AsyncCallback(OnDataRead), read_state);

            if (BootModem() == CMD_RETURN.COMMAND_SUCESS)
            {
                if (SetRegulatoryRegion(region) == CMD_RETURN.COMMAND_SUCESS) return true;
                else
                    return false;
            }
            else
                return false;
        }

        /// <summary>
        /// Disconnect reader
        /// </summary>
        /// <returns></returns>
        public bool Disconnect()
        {
            try
            {
                try
                {
                    ModemStop();
                }
                catch { }

                connected = false;
                client.Close();

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Implement disposable interface
        /// </summary>
        public void Dispose()
        {
            Disconnect();
        }

        /// <summary>
        /// Return last invalid command notification
        /// </summary>
        public InvalidCommandNtf GetLastInvalidCommandNotification
        {
            get { try { return lastError; } catch { return null; } }
        }
        
        private void ProcessRSPMach1Frame(MACH1_FRAME mf)
        {
            switch (mf.header.category)
            {
                #region Management RSP
                case CATEGORY.MODEM_MANAGEMENT:
                    switch (mf.header.message_id)
                    {
                        case MANAGEMENT_CMD.BOOT_MODEM:
                            if (bootModemRspEvent != null) bootModemRspEvent.evt.Set();
                            break;
                        case MANAGEMENT_CMD.GET_MCS_VERSION:
                            getMCSVersionRspEvent.data = new byte[mf.payload_len];
                            if(mf.PAYLOAD!=null)Array.Copy(mf.PAYLOAD, getMCSVersionRspEvent.data, mf.payload_len);
                            getMCSVersionRspEvent.evt.Set();
                            break;
                        case MANAGEMENT_CMD.SHUTDOWN_MODEM:
                            shutdownModemRspEvent.evt.Set();
                            break;
                        case MANAGEMENT_CMD.SET_TEMPERATURE_ALART:
                            setTemperatureAlarmRspEvent.evt.Set();
                            break;
                        case MANAGEMENT_CMD.GET_READER_INFO:
                            getReaderInfoRspEvent.data = new byte[mf.payload_len];
                            if(mf.PAYLOAD!=null)Array.Copy(mf.PAYLOAD, getReaderInfoRspEvent.data, mf.payload_len);
                            getReaderInfoRspEvent.evt.Set();
                            break;
                        case MANAGEMENT_CMD.GET_STATE:
                            getStateRspEvent.data = new byte[mf.payload_len];
                            if(mf.PAYLOAD!=null)Array.Copy(mf.PAYLOAD, getStateRspEvent.data, mf.payload_len);
                            getStateRspEvent.evt.Set();
                            break;
                        case MANAGEMENT_CMD.GET_TEMPERATURE_ALART:
                            getTemperatureAlarmRspEvent.data = new byte[mf.payload_len];
                            if(mf.PAYLOAD!=null)Array.Copy(mf.PAYLOAD, getTemperatureAlarmRspEvent.data, mf.payload_len);
                            getTemperatureAlarmRspEvent.evt.Set();
                            break;
                        case MANAGEMENT_CMD.SET_GPO:
                            setGPORspEvent.data = new byte[mf.payload_len];
                            if(mf.PAYLOAD!=null)Array.Copy(mf.PAYLOAD, setGPORspEvent.data, mf.payload_len);
                            setGPORspEvent.evt.Set();
                            break;
                        case MANAGEMENT_CMD.SET_GPI:
                            setGPIRspEvent.data = new byte[mf.payload_len];
                            if(mf.PAYLOAD!=null)Array.Copy(mf.PAYLOAD, setGPIRspEvent.data, mf.payload_len);
                            setGPIRspEvent.evt.Set();
                            break;
                        case MANAGEMENT_CMD.GET_GPI:
                            getGPIRspEvent.data = new byte[mf.payload_len];
                            if(mf.PAYLOAD!=null)Array.Copy(mf.PAYLOAD, getGPIRspEvent.data, mf.payload_len);
                            getGPIRspEvent.evt.Set();
                            break;
                        case MANAGEMENT_CMD.SET_STATUS_REPORT:
                            setStatusReportRspEvent.evt.Set();
                            break;
                        case MANAGEMENT_CMD.SET_TCP_CONNECTION_OPTIONS:
                            setTcpConnectionOptionsRspEvent.evt.Set();
                            break;
                        case MANAGEMENT_CMD.GET_TCP_CONNECTION_OPTIONS:
                            getTcpConnectionOptionsRspEvent.data = new byte[mf.packet_len];
                            if (mf.PAYLOAD != null) Array.Copy(mf.PAYLOAD, getTcpConnectionOptionsRspEvent.data, mf.payload_len);
                            getTcpConnectionOptionsRspEvent.evt.Set();
                            break;
                    }
                    break;
                #endregion 

                #region Operation RSP
                case CATEGORY.OPERATION_MODEL:
                    switch (mf.header.message_id)
                    {
                        case OPERATION_CMD.SET_REGULATORY_REGION:
                            if (setRegionRspEvent != null) setRegionRspEvent.evt.Set();
                            break;
                        case OPERATION_CMD.INVENTORY:
                            inventoryRspEvent.data = new byte[mf.payload_len];
                            if(mf.PAYLOAD!=null)Array.Copy(mf.PAYLOAD, inventoryRspEvent.data, mf.payload_len);
                            this.inventory_start_time.seconds = mf.timestamp_second;
                            this.inventory_start_time.u_seconds = mf.timestamp_us;
                            inventoryRspEvent.evt.Set();
                            break;
                        case OPERATION_CMD.INVENTORY_CONTINUE:
                            inventoryContinueRspEvent.data = new byte[mf.payload_len];
                            if(mf.PAYLOAD!=null)Array.Copy(mf.PAYLOAD, inventoryContinueRspEvent.data, mf.payload_len);
                            inventoryContinueRspEvent.evt.Set();
                            break;
                        case OPERATION_CMD.GET_OCS_VERSION:
                            getOCSVersionRspEvent.data = new byte[mf.payload_len];
                            if(mf.PAYLOAD!=null)Array.Copy(mf.PAYLOAD, getOCSVersionRspEvent.data, mf.payload_len);
                            getOCSVersionRspEvent.evt.Set();
                            break;
                        case OPERATION_CMD.LOAD_FROM_PROFILE:
                            loadFromProfileRspEvent.evt.Set();
                            break;
                        case OPERATION_CMD.STORE_TO_PROFILE:
                            storeToProfileRspEvent.evt.Set();
                            break;
                        case OPERATION_CMD.GET_CAPABILITY:
                            getCapabilitiesRspEvent.data = new byte[mf.payload_len];
                            if (mf.PAYLOAD != null) Array.Copy(mf.PAYLOAD, getCapabilitiesRspEvent.data, mf.packet_len);
                            getCapabilitiesRspEvent.evt.Set();
                            break;
                        case OPERATION_CMD.SET_TX_POWER:
                            setTxPowerRspEvent.data = new byte[mf.payload_len];
                            if(mf.PAYLOAD!=null)Array.Copy(mf.PAYLOAD, setTxPowerRspEvent.data, mf.payload_len);
                            setTxPowerRspEvent.evt.Set();
                            break;
                        case OPERATION_CMD.GET_TX_POWER:
                            getTxPowerRspEvent.data = new byte[mf.payload_len];
                            if(mf.PAYLOAD!=null)Array.Copy(mf.PAYLOAD, getTxPowerRspEvent.data, mf.payload_len);
                            getTxPowerRspEvent.evt.Set();
                            break;
                        case OPERATION_CMD.SET_ANTENNA:
                            setAntennaRspEvent.data = new byte[mf.payload_len];
                            if(mf.PAYLOAD!=null)Array.Copy(mf.PAYLOAD, setAntennaRspEvent.data, mf.payload_len);
                            setAntennaRspEvent.evt.Set();
                            break;
                        case OPERATION_CMD.GET_ANTENNA:
                            getAntennaRspEvent.data = new byte[mf.payload_len];
                            if(mf.PAYLOAD!=null)Array.Copy(mf.PAYLOAD, getAntennaRspEvent.data, mf.payload_len);
                            getAntennaRspEvent.evt.Set();
                            break;
                        case OPERATION_CMD.SET_TX_FREQUENCY:
                            setTxFrequencyRspEvent.data = new byte[mf.payload_len];
                            if(mf.PAYLOAD!=null)Array.Copy(mf.PAYLOAD, setTxFrequencyRspEvent.data, mf.payload_len);
                            setTxFrequencyRspEvent.evt.Set();
                            break;
                        case OPERATION_CMD.GET_TX_FREQUENCY:
                            getTxFrequencyRspEvent.data = new byte[mf.payload_len];
                            if(mf.PAYLOAD!=null)Array.Copy(mf.PAYLOAD, getTxFrequencyRspEvent.data, mf.payload_len);
                            getTxFrequencyRspEvent.evt.Set();
                            break;
                        case OPERATION_CMD.SET_GEN2_PARAMS:
                            setGen2ParamsRspEvent.data = new byte[mf.payload_len];
                            if(mf.PAYLOAD!=null)Array.Copy(mf.PAYLOAD, setGen2ParamsRspEvent.data, mf.payload_len);
                            setGen2ParamsRspEvent.evt.Set();
                            break;
                        case OPERATION_CMD.GET_GEN2_PARAMS:
                            getGen2ParamsRspEvent.data = new byte[mf.payload_len];
                            if(mf.PAYLOAD!=null)Array.Copy(mf.PAYLOAD, getGen2ParamsRspEvent.data, mf.payload_len);
                            getGen2ParamsRspEvent.evt.Set();
                            break;
                        case OPERATION_CMD.CHECK_ANTENNA:
                            checkAntennaRspEvent.evt.Set();
                            break;
                        case OPERATION_CMD.SET_INVENTORY_REPORT:
                            setInventoryReportRspEvent.data = new byte[mf.payload_len];
                            if(mf.PAYLOAD!=null)Array.Copy(mf.PAYLOAD, setInventoryReportRspEvent.data, mf.payload_len);
                            setInventoryReportRspEvent.evt.Set();
                            break;
                        case OPERATION_CMD.SET_LBT_PARAMS:
                            setLBTParamsRspEvent.data = new byte[mf.payload_len];
                            if(mf.PAYLOAD!=null)Array.Copy(mf.PAYLOAD, setLBTParamsRspEvent.data, mf.payload_len);
                            setLBTParamsRspEvent.evt.Set();
                            break;
                        case OPERATION_CMD.GET_LBT_PARAMS:
                            getLBTParamRspEvent.data = new byte[mf.payload_len];
                            if(mf.PAYLOAD!=null)Array.Copy(mf.PAYLOAD, getLBTParamRspEvent.data, mf.payload_len);
                            getLBTParamRspEvent.evt.Set(); 
                            break;
                        case OPERATION_CMD.REPORT_INVENTORY:
                            reportInventoryRspEvent.data = new byte[mf.payload_len];
                            if (mf.PAYLOAD != null) Array.Copy(mf.PAYLOAD, reportInventoryRspEvent.data, mf.payload_len);
                            reportInventoryRspEvent.evt.Set();
                            break;
                        case OPERATION_CMD.MODEM_STOP:
                            modemStopRspEvent.evt.Set();
                            this.inventory_stop_time.seconds = mf.timestamp_second;
                            this.inventory_stop_time.u_seconds = mf.timestamp_us;
                            break;
                        case OPERATION_CMD.RF_SURVEY:
                            rfSurveyRspEvent.data = new byte[mf.payload_len];
                            if(mf.PAYLOAD!=null)Array.Copy(mf.PAYLOAD, rfSurveyRspEvent.data, mf.payload_len);
                            rfSurveyRspEvent.evt.Set();
                            break;
                        case OPERATION_CMD.TAG_READ:
                            tagReadRspEvent.evt.Set();
                            break;
                        case OPERATION_CMD.TAG_CUSTOM:
                            tagCustomRspEvent.data = new byte[mf.payload_len];
                            if(mf.PAYLOAD!=null)Array.Copy(mf.PAYLOAD, tagCustomRspEvent.data, mf.payload_len);
                            tagCustomRspEvent.evt.Set();
                            break;
                        case OPERATION_CMD.TAG_KILL:
                            tagKillRspEvent.data = new byte[mf.payload_len];
                            if(mf.PAYLOAD!=null)Array.Copy(mf.PAYLOAD, tagKillRspEvent.data, mf.payload_len);
                            tagKillRspEvent.evt.Set();
                            break;
                        case OPERATION_CMD.TAG_LOCK:
                            tagLockRspEvent.data = new byte[mf.payload_len];
                            if(mf.PAYLOAD!=null)Array.Copy(mf.PAYLOAD, tagLockRspEvent.data, mf.payload_len);
                            tagLockRspEvent.evt.Set();
                            break;
                        case OPERATION_CMD.TAG_WRITE:
                            tagWriteRspEvent.data = new byte[mf.payload_len];
                            if(mf.PAYLOAD!=null)Array.Copy(mf.PAYLOAD, tagWriteRspEvent.data, mf.payload_len);
                            tagWriteRspEvent.evt.Set();
                            break;
                        case OPERATION_CMD.SET_RX_CONFIG:
                            setRxConfigRspEvent.data = new byte[mf.payload_len];
                            if(mf.PAYLOAD!=null)Array.Copy(mf.PAYLOAD, setRxConfigRspEvent.data, mf.payload_len);
                            setRxConfigRspEvent.evt.Set();
                            break;
                        case OPERATION_CMD.GET_RX_CONFIG:
                            getRxConfigRspEvent.data = new byte[mf.payload_len];
                            if(mf.PAYLOAD!=null)Array.Copy(mf.PAYLOAD, getRxConfigRspEvent.data, mf.payload_len);
                            getRxConfigRspEvent.evt.Set();
                            break;
                        case OPERATION_CMD.GET_SUPPORTED_GEN2_PARAMS:
                            getSupportedGen2ParamsRspEvent.data = new byte[mf.payload_len];
                            if(mf.PAYLOAD!=null)Array.Copy(mf.PAYLOAD, getSupportedGen2ParamsRspEvent.data, mf.payload_len);
                            getSupportedGen2ParamsRspEvent.evt.Set();
                            break;
                        case OPERATION_CMD.SET_PROFILE_SEQUENCE:
                            setProfileSequenceRspEvent.data = new byte[mf.payload_len];
                            if (mf.PAYLOAD != null) Array.Copy(mf.PAYLOAD, setProfileSequenceRspEvent.data, mf.payload_len);
                            setProfileSequenceRspEvent.evt.Set();
                            break;
                    }
                    break;

                #endregion 

                #region Test RSP
                case CATEGORY.TEST:
                    switch (mf.header.message_id)
                    {
                        case TEST_CMD_SET.TEST_WRITE:
                            testWriteRspEvent.data = new byte[mf.payload_len];
                            if (mf.PAYLOAD != null) Array.Copy(mf.PAYLOAD, testWriteRspEvent.data, mf.payload_len);
                            testWriteRspEvent.evt.Set();
                            break;
                    }
                    break;
                #endregion

                case CATEGORY.MACH1_PROTOCOL_ERROR:
                    break;
            }
        }
        private void ProcessNTFMach1Frame(MACH1_FRAME mf)
        {
            switch (mf.header.category)
            {
                #region Management NTF
                case CATEGORY.MODEM_MANAGEMENT:
                    switch (mf.header.message_id)
                    {
                        case MANAGEMENT_NTF.BOOT_MODEM:
                            try
                            {
                                MANAGEMENT_NTF.BOOT_MODEM_NTF bmn = new MANAGEMENT_NTF.BOOT_MODEM_NTF(mf.PAYLOAD);
                                if (bootModemNtfEvent != null && bmn.boot_result_code == 0) bootModemNtfEvent.evt.Set();
                                if (bmn != null && onBootModemNtfReceived != null)
                                {
                                    bmn.timestamp_us = mf.timestamp_us;
                                    bmn.timestamp_second = mf.timestamp_second;
                                    bmn.reader_name = this.reader_setting.reader_information.reader_name;
                                    delegateBootModemNtf dbn = new delegateBootModemNtf(TriggerAsynModemBootStatus);                                
                                    dbn.BeginInvoke(bmn, null, null);
                                }

                            }
                            catch { }
                            break;
                        case MANAGEMENT_NTF.SOCKET_CONNECTION_STATUS:
                            try
                            {
                                MANAGEMENT_NTF.SOCKET_CONNECTION_STATUS_NTF ssn = new MANAGEMENT_NTF.SOCKET_CONNECTION_STATUS_NTF(mf.PAYLOAD);

                                if (ssn.socket_status == SOCKET_STATUS.CONNECTION_SUCCESS) connected = true;
                                if (ssn != null && onSocketConnectionStatusNtfReceived != null)
                                {
                                    ssn.timestamp_us = mf.timestamp_us;
                                    ssn.timestamp_second = mf.timestamp_second;
                                    ssn.reader_name = this.reader_setting.reader_information.reader_name;
                                    delegateSocketConnectionStatusNtf dss = new delegateSocketConnectionStatusNtf(TriggerSocketConnectionStatus);
                                    dss.BeginInvoke(ssn, null, null);
                                    
                                }
                                
                            }
                            catch { }
                            break;
                        case MANAGEMENT_NTF.TEMPERATURE_ALART:
                            try
                            {
                                MANAGEMENT_NTF.TEMPERETURE_ALARM_NTF tan = new MANAGEMENT_NTF.TEMPERETURE_ALARM_NTF(mf.PAYLOAD);
                                if (tan != null && onTemperatureAlarmNtfReceived != null)
                                {
                                    tan.timestamp_us = mf.timestamp_us;
                                    tan.timestamp_second = mf.timestamp_second;
                                    tan.reader_name = this.reader_setting.reader_information.reader_name;
                                    delegateTemperatureAlarmNtf tanNtf = new delegateTemperatureAlarmNtf(TriggerAsynTemperatureAlarm);
                                    tanNtf.BeginInvoke(tan, null, null);
                                }
                            }
                            catch { }
                            break;
                        case MANAGEMENT_NTF.GPI_ALERT:
                            try
                            {
                                MANAGEMENT_NTF.GPI_ALERT_NTF gan = new MANAGEMENT_NTF.GPI_ALERT_NTF(mf.PAYLOAD);
                                if (gan != null && onGPIAlertNtfReceived != null)
                                {
                                    gan.timestamp_us = mf.timestamp_us;
                                    gan.timestamp_second = mf.timestamp_second;
                                    gan.reader_name = this.reader_setting.reader_information.reader_name;
                                    delegateGPIAlertNtf dgn = new delegateGPIAlertNtf(TriggerAsynGPIAlert);
                                    dgn.BeginInvoke(gan, null, null);
                                }
                            }
                            catch { }
                            break;
                        case MANAGEMENT_NTF.SYSTEM_ERROR:
                            try
                            {
                                MANAGEMENT_NTF.SYSTEM_ERROR_NTF sen = new MANAGEMENT_NTF.SYSTEM_ERROR_NTF(mf.PAYLOAD);
                                if (sen != null && onSystemErrorNtfReceived != null)
                                {
                                    sen.timestamp_us = mf.timestamp_us;
                                    sen.timestamp_second = mf.timestamp_second;
                                    sen.reader_name = this.reader_setting.reader_information.reader_name;
                                    delegateSystemErrorNtf dsn = new delegateSystemErrorNtf(TriggerAsynSystemError);
                                    dsn.BeginInvoke(sen, null, null);
                                }
                            }
                            catch { }
                            break;
                        case MANAGEMENT_NTF.STATUS_REPORT:
                            try
                            {
                                MANAGEMENT_NTF.STATUS_REPORT_NTF srn = new MANAGEMENT_NTF.STATUS_REPORT_NTF(mf.PAYLOAD);
                                if (srn != null && onStatusReportNtfReceived != null)
                                {
                                    srn.timestamp_us = mf.timestamp_us;
                                    srn.timestamp_second = mf.timestamp_second;
                                    srn.reader_name = this.reader_setting.reader_information.reader_name;
                                    delegateStatusReportNtf drn = new delegateStatusReportNtf(TriggerAsynStatusReport);
                                    drn.BeginInvoke(srn, null, null);
                                }
                            }
                            catch { }
                            break;
                    }
                    break;
                #endregion 

                #region Operation NTF
                case CATEGORY.OPERATION_MODEL:
                    switch (mf.header.message_id)
                    {
                        case OPERATION_NTF.SET_REGULATORY_REGION:
                            if (setRegionNtfEvent != null) setRegionNtfEvent.evt.Set();
                            break;
                        case OPERATION_NTF.INVENTORY:
                            OPERATION_NTF.INVENTORY_NTF inv_ntf = new OPERATION_NTF.INVENTORY_NTF(mf.PAYLOAD);

                            Tag tag= new Tag();
                            switch (inv_ntf.antenna)
                            {
                                case OPERATION_NTF.INVENTORY_NTF.ANTENNA.ANTENNA1:
                                    tag.antenna = 1;
                                    break;
                                case OPERATION_NTF.INVENTORY_NTF.ANTENNA.ANTENNA2:
                                    tag.antenna = 2;
                                    break;
                                case OPERATION_NTF.INVENTORY_NTF.ANTENNA.ANTENNA3:
                                    tag.antenna = 3;
                                    break;
                                case OPERATION_NTF.INVENTORY_NTF.ANTENNA.ANTENNA4:
                                    tag.antenna = 4;
                                    break;
                                default:
                                    tag.antenna = 1;
                                    break;
                            }
                            
                            tag.epc = inv_ntf.EPC;
                            tag.rssi = inv_ntf.rssi;
                            tag.phaseI = inv_ntf.phaseI;
                            tag.phaseQ = inv_ntf.phaseQ;
                            
                            tag.reader_name = this.Settings.reader_information.reader_name;
                            try
                            {
                                if (tag.reader_name == null || tag.reader_name.Length == 0) tag.reader_name = "default";
                            }
                            catch
                            {
                                tag.reader_name = "default";
                            }
                            tag.timeStamp.seconds = mf.timestamp_second;
                            tag.timeStamp.u_seconds = mf.timestamp_us;

                            if (onTagRead != null)
                            {
                                try
                                {
                                    delegateTagRead dtr = new delegateTagRead(TriggerAsynTagRead);
                                    dtr.BeginInvoke(tag, null, null);
                                }
                                catch { }
                            }


                            if (reader_setting.enable_buffering)
                            {
                                bool existed = false;
                                foreach (Tag t in inventoryBuffer)
                                {
                                    if (t.epc == tag.epc)
                                    {
                                        existed = true;
                                        break;
                                    }
                                }

                                if (!existed) inventoryBuffer.Add(tag);
                            }

                            if (inv_ntf != null && onInventoryNtfReceived!=null)
                            {
                                inv_ntf.timestamp_us = mf.timestamp_us;
                                inv_ntf.timestamp_second = mf.timestamp_second;

                                inv_ntf.reader_name = this.reader_setting.reader_information.reader_name;

                                try
                                {
                                    delegateInventoryNtf oin = new delegateInventoryNtf(TriggerAsynInventory);
                                    oin.BeginInvoke(inv_ntf, null, null);
                                }
                                catch { }
                            }
                            break;
                        case OPERATION_NTF.ANTENNA_ALERT:
                            OPERATION_NTF.ANTENNA_ALERT_NTF aan = new OPERATION_NTF.ANTENNA_ALERT_NTF(mf.PAYLOAD);
                            if (aan != null && onAntennaAlertNtfReceived != null)
                            {
                                try
                                {
                                    aan.timestamp_us = mf.timestamp_us;
                                    aan.timestamp_second = mf.timestamp_second;
                                    aan.reader_name = this.reader_setting.reader_information.reader_name;
                                    delegateAntennaAlertNtf daa = new delegateAntennaAlertNtf(TriggerAsynAntennaAlert);
                                    daa.BeginInvoke(aan, null, null);
                                }
                                catch { }
                            }
                            break;
                        case OPERATION_NTF.CHECK_ANTENNA:
                            checkAntennaNtfEvent.data = new byte[mf.payload_len];
                            if(mf.PAYLOAD!=null)Array.Copy(mf.PAYLOAD, checkAntennaNtfEvent.data, mf.payload_len);
                            checkAntennaNtfEvent.evt.Set();
                            break;
                        case OPERATION_NTF.INVENTORY_STATUS:
                            OPERATION_NTF.INVENTORY_STATUS_NTF isn = new OPERATION_NTF.INVENTORY_STATUS_NTF(mf.PAYLOAD);
                            if (isn != null && onInventoryStatusNtfReceived != null)
                            {
                                isn.timestamp_us = mf.timestamp_us;
                                isn.timestamp_second = mf.timestamp_second;
                                isn.reader_name = this.reader_setting.reader_information.reader_name;
                                try
                                {
                                    delegateInventoryStatusNtf dis = new delegateInventoryStatusNtf(TriggerAsynInventoryStatus);
                                    dis.BeginInvoke(isn, null, null);
                                }
                                catch { }
                            }
                            break;
                        case OPERATION_NTF.ACCUMULATION_STATUS:
                            OPERATION_NTF.ACCUMULATION_STATUS_NTF asn = new OPERATION_NTF.ACCUMULATION_STATUS_NTF();
                            asn.code = (OPERATION_NTF.ACCUMULATION_STATUS_NTF.NTF_CODE)mf.PAYLOAD[0];
                            if (asn!=null && onAccumulationStatusNtfReceived != null)
                            {
                                asn.timestamp_us = mf.timestamp_us;
                                asn.timestamp_second = mf.timestamp_second;
                                asn.reader_name = this.reader_setting.reader_information.reader_name;
                                try
                                {
                                    delegateAccumulateStatusNtf dasn = new delegateAccumulateStatusNtf(TriggerAsynAccumulationStatus);
                                    dasn.BeginInvoke(asn, null, null);
                                }
                                catch { }
                            }
                            break;
                        case OPERATION_NTF.MODEM_STOP:
                            OPERATION_NTF.MODEM_STOPPED_NTF msn = new OPERATION_NTF.MODEM_STOPPED_NTF();
                            msn.code = (OPERATION_NTF.MODEM_STOPPED_NTF.NTF_CODE)(mf.PAYLOAD[0]);
                            if (msn!= null&& onModemStopNtfReceived != null)
                            {
                                msn.timestamp_us = mf.timestamp_us;
                                msn.timestamp_second = mf.timestamp_second;
                                msn.reader_name = this.reader_setting.reader_information.reader_name;
                                try
                                {
                                    delegateModemStoppedNtf dmn = new delegateModemStoppedNtf(TriggerAsynModemStopped);
                                    dmn.BeginInvoke(msn, null, null);
                                }
                                catch { }
                            }
                            break;
                        case OPERATION_NTF.RF_SURVEY:
                            OPERATION_NTF.RF_SURVEY_NTF rsn = new OPERATION_NTF.RF_SURVEY_NTF(mf.PAYLOAD);
                            if (rsn != null && onRFSurveyNtfReceived != null)
                            {
                                rsn.timestamp_us = mf.timestamp_us;
                                rsn.timestamp_second = mf.timestamp_second;
                                rsn.reader_name = this.reader_setting.reader_information.reader_name;
                                try
                                {
                                    delegateRfSurveyNtf drn = new delegateRfSurveyNtf(TriggerAsynRfSurvey);
                                    drn.BeginInvoke(rsn, null, null);
                                }
                                catch { }
                            }
                            break;

                        case OPERATION_NTF.TAG_READ:
                            tagReadNtfEvent.data = new byte[mf.payload_len];
                            if(mf.PAYLOAD!=null)Array.Copy(mf.PAYLOAD, tagReadNtfEvent.data, mf.payload_len);
                            tagReadNtfEvent.evt.Set();
                            break;
                        case OPERATION_NTF.TAG_WRITE:
                            tagWriteNtfEvent.data = new byte[mf.payload_len];
                            if(mf.PAYLOAD!=null)Array.Copy(mf.PAYLOAD, tagWriteNtfEvent.data, mf.payload_len);
                            tagWriteNtfEvent.evt.Set();
                            break;
                        case OPERATION_NTF.TAG_LOCK:
                            tagLockNtfEvent.data = new byte[mf.payload_len];
                            if(mf.PAYLOAD!=null)Array.Copy(mf.PAYLOAD, tagLockNtfEvent.data, mf.payload_len);
                            tagLockNtfEvent.evt.Set();
                            break;
                        case OPERATION_NTF.TAG_KILL:
                            tagKillNtfEvent.data = new byte[mf.payload_len];
                            if(mf.PAYLOAD!=null)Array.Copy(mf.PAYLOAD, tagKillNtfEvent.data, mf.payload_len);
                            tagKillNtfEvent.evt.Set();
                            break;
                    }
                    break;
                #endregion 

                #region Test Ntf
                case CATEGORY.TEST:

                    break;
                #endregion

                case CATEGORY.MACH1_PROTOCOL_ERROR:
                    invalidCommandNtfEvent.data = new byte[mf.payload_len];
                    if(mf.PAYLOAD!=null)Array.Copy(mf.PAYLOAD, invalidCommandNtfEvent.data, mf.payload_len);
                    invalidCommandNtfEvent.evt.Set();
                    try
                    {
                        lastError = new InvalidCommandNtf(mf.PAYLOAD);
                    }
                    catch { }
                    break;
            }
        }
        private void ProcessMach1Frame(MACH1_FRAME mf)
        {
            if (mf.header.is_ntf)
            {
                ProcessNTFMach1Frame(mf);
            }
            //if the packet is response                           
            else
            {
                ProcessRSPMach1Frame(mf);
            }
        }
        private void OnDataRead(IAsyncResult ar)
        {
            int offset = 0;
            AsynReadState ss = (AsynReadState)ar.AsyncState;

            byte[] data;

            try
            {
                //if there are reserved data from last packet, add the data to the packet
                if (reserved_data != null)
                {
                    data = new byte[ss.data.Length + reserved_data.Length];
                    Array.Copy(reserved_data, data, reserved_data.Length);
                    Array.Copy(ss.data, 0, data, reserved_data.Length, ss.data.Length);

                    reserved_data = null;
                }
                else
                {
                    data = new byte[ss.data.Length];
                    Array.Copy(ss.data, data, ss.data.Length);
                }

                while (offset < data.Length && (data[offset] == GENERAL_DEFINITION.SOF_CMD || data[offset] == GENERAL_DEFINITION.SOF_RSP))
                {
                    byte[] f_data = new byte[data.Length - offset];
                    Array.Copy(data, offset, f_data, 0, data.Length - offset);

                    try
                    {
                        MACH1_FRAME mf = MACH1_FRAME.ParseMachData(f_data, out reserved_data);

                        if (mf != null)
                        {
                            offset += mf.packet_len;
                            ProcessMach1Frame(mf);
                        }
                        else
                            break;
                    }
                    catch
                    {
                        break;
                    }
                }

                if (connected && ns != null)
                {
                    try
                    {
                        ns.Flush();
                        read_state = new AsynReadState(buffer_size);
                        ns.BeginRead(read_state.data, 0, read_state.data.Length, new AsyncCallback(OnDataRead), read_state);
                    }
                    catch
                    {
                    }
                }
            }
            catch
            {
            }
        }

        #region Management Command
        /// <summary>
        /// Boot modem command
        /// </summary>
        /// <returns></returns>
        public CMD_RETURN BootModem()
        {
            bootModemRspEvent = new SpeedwayManualResetEvent(false);
            bootModemNtfEvent = new SpeedwayManualResetEvent(false);
            invalidCommandNtfEvent = new SpeedwayManualResetEvent(false);
            WaitHandle[] waithandles = new WaitHandle[] { bootModemRspEvent.evt, invalidCommandNtfEvent.evt };    
           
            byte[] data = MANAGEMENT_CMD.GENERATE_BOOT_MODEM_CMD(include_timestamp);
            ns.Write(data, 0, data.Length);

            int wait_result = WaitHandle.WaitAny(waithandles, MODEM_INIT_TIME_OUT, false);
            if (wait_result == 1) return CMD_RETURN.INVALID_COMMAND;
            else if (wait_result>1) return CMD_RETURN.CMD_RESPONSE_TIME_OUT;

            if (!bootModemNtfEvent.evt.WaitOne(MODEM_INIT_TIME_OUT, false)) return CMD_RETURN.COMMAND_FAILED;

            return CMD_RETURN.COMMAND_SUCESS;
        }

        /// <summary>
        /// Get Management command set version. 
        /// </summary>
        /// <param name="rsp"></param>
        /// <returns></returns>
        public CMD_RETURN GetMCSVersion(out MANAGEMENT_CMD.MCS_VERSION_RSP rsp)
        {
            getMCSVersionRspEvent = new SpeedwayManualResetEvent(false);
            invalidCommandNtfEvent = new SpeedwayManualResetEvent(false);
            WaitHandle[] waithandles = new WaitHandle[] { getMCSVersionRspEvent.evt, invalidCommandNtfEvent.evt};
            rsp = null;
            
            byte[] data = MANAGEMENT_CMD.GENERATE_GET_MCS_VERSION_CMD(include_timestamp);
            ns.Write(data, 0, data.Length);

            int wait_result = WaitHandle.WaitAny(waithandles, CMD_RESPONSE_TIME_OUT, false);
            if (wait_result == 1) return CMD_RETURN.INVALID_COMMAND;
            else if (wait_result>1) return CMD_RETURN.CMD_RESPONSE_TIME_OUT;
            else
            {
                try
                {
                    rsp = new MANAGEMENT_CMD.MCS_VERSION_RSP(getMCSVersionRspEvent.data);
                }
                catch
                {
                    return CMD_RETURN.RESPONSE_CAN_NOT_RECOGNIZED;
                }
                
                return CMD_RETURN.COMMAND_SUCESS;
            }
        }

        /// <summary>
        /// Get reader information
        /// </summary>
        /// <param name="rsp"></param>
        /// <returns></returns>
        public CMD_RETURN GetReaderInfo(out MANAGEMENT_CMD.READER_INFO_RSP rsp)
        {
            getReaderInfoRspEvent = new SpeedwayManualResetEvent(false);
            invalidCommandNtfEvent = new SpeedwayManualResetEvent(false);
            WaitHandle[] waithandles = new WaitHandle[] { getReaderInfoRspEvent.evt, invalidCommandNtfEvent.evt};
            byte[] data = MANAGEMENT_CMD.GENERATE_GET_READER_INFO_CMD(include_timestamp);
            ns.Write(data, 0, data.Length);

            rsp = null;
            int wait_result = WaitHandle.WaitAny(waithandles, CMD_RESPONSE_TIME_OUT, false);
            if (wait_result == 1) return CMD_RETURN.INVALID_COMMAND;
            else if (wait_result>1) return CMD_RETURN.CMD_RESPONSE_TIME_OUT; 
            
            try
            {
                rsp = new MANAGEMENT_CMD.READER_INFO_RSP(getReaderInfoRspEvent.data);
                return CMD_RETURN.COMMAND_SUCESS;
            }
            catch
            {
                return CMD_RETURN.RESPONSE_CAN_NOT_RECOGNIZED;
            }
            
        }

        /// <summary>
        /// Get modem state
        /// </summary>
        /// <param name="rsp"></param>
        /// <returns></returns>
        public CMD_RETURN GetState(out MANAGEMENT_CMD.STATE_RSP rsp)
        {
            getStateRspEvent = new SpeedwayManualResetEvent(false);
            invalidCommandNtfEvent = new SpeedwayManualResetEvent(false);
            WaitHandle[] waithandles = new WaitHandle[] { getStateRspEvent.evt, invalidCommandNtfEvent.evt};

            byte[] data = MANAGEMENT_CMD.GENERATE_GET_STATE_CMD(include_timestamp);

            ns.Write(data, 0, data.Length);
            rsp = null;
            int wait_result = WaitHandle.WaitAny(waithandles, CMD_RESPONSE_TIME_OUT, false);
            if (wait_result == 1) return CMD_RETURN.INVALID_COMMAND;
            else if (wait_result>1) return CMD_RETURN.CMD_RESPONSE_TIME_OUT;
            try
            {
                rsp = new MANAGEMENT_CMD.STATE_RSP(getStateRspEvent.data);

                return CMD_RETURN.COMMAND_SUCESS;
            }
            catch
            {
                return CMD_RETURN.RESPONSE_CAN_NOT_RECOGNIZED;
            }

        }

        /// <summary>
        /// Shutdown modem
        /// </summary>
        /// <returns></returns>
        public CMD_RETURN ShutDownModem()
        {
            shutdownModemRspEvent = new SpeedwayManualResetEvent(false);
            invalidCommandNtfEvent = new SpeedwayManualResetEvent(false);
            WaitHandle[] waithandles = new WaitHandle[] { shutdownModemRspEvent.evt, invalidCommandNtfEvent.evt};

            byte[] data = MANAGEMENT_CMD.GENERATE_SHUTDOWN_MODEM_CMD(include_timestamp);
            ns.Write(data, 0, data.Length);

            int wait_result = WaitHandle.WaitAny(waithandles, CMD_RESPONSE_TIME_OUT, false);
            if (wait_result == 1) return CMD_RETURN.INVALID_COMMAND;
            else if (wait_result>1) return CMD_RETURN.CMD_RESPONSE_TIME_OUT;
            else
                return CMD_RETURN.COMMAND_SUCESS;

        }
        
        /// <summary>
        /// Set temperature alarm
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="periodic">alarm report interval, value is 1-6. interms of 10s</param>
        /// <param name="alert_threshold">temperature threshold, celsius degree</param>
        /// <returns></returns>
        public CMD_RETURN SetTempretureAlarm(MANAGEMENT_CMD.TERMPERETURE_ALARM_MODE mode, UInt16 periodic, Int16 alert_threshold)
        {
            setTemperatureAlarmRspEvent = new SpeedwayManualResetEvent(false);
            invalidCommandNtfEvent = new SpeedwayManualResetEvent(false);
            WaitHandle[] waithandles = new WaitHandle[] {setTemperatureAlarmRspEvent.evt, invalidCommandNtfEvent.evt };

            byte[] data = MANAGEMENT_CMD.GENERATE_SET_TEMPERETURE_ALARM_CMD(include_timestamp, mode, periodic, alert_threshold);
            ns.Write(data, 0, data.Length);

            int wait_result = WaitHandle.WaitAny(waithandles, CMD_RESPONSE_TIME_OUT, false);
            if (wait_result == 1) return CMD_RETURN.INVALID_COMMAND;
            else if (wait_result>1) return CMD_RETURN.CMD_RESPONSE_TIME_OUT;
            else
                return CMD_RETURN.COMMAND_SUCESS;

        }

        /// <summary>
        /// Get temperature alarm
        /// </summary>
        /// <param name="rsp"></param>
        /// <returns></returns>
        public CMD_RETURN GetTemperatureAlarm(out MANAGEMENT_CMD.TEMPERETURE_ALARM_RSP rsp)
        {
            getTemperatureAlarmRspEvent = new SpeedwayManualResetEvent(false);
            invalidCommandNtfEvent = new SpeedwayManualResetEvent(false);
            WaitHandle[] waithandles = new WaitHandle[] { getTemperatureAlarmRspEvent.evt, invalidCommandNtfEvent.evt};
            byte[] data = MANAGEMENT_CMD.GENERATE_GET_TEMPERETURE_ALARM_CMD(include_timestamp);

            ns.Write(data, 0, data.Length);

            rsp = null;
            int wait_result = WaitHandle.WaitAny(waithandles, CMD_RESPONSE_TIME_OUT, false);
            if (wait_result == 1) return CMD_RETURN.INVALID_COMMAND;
            else if (wait_result>1) return CMD_RETURN.CMD_RESPONSE_TIME_OUT;
            try
            {
                rsp = new MANAGEMENT_CMD.TEMPERETURE_ALARM_RSP(getTemperatureAlarmRspEvent.data);
                return CMD_RETURN.COMMAND_SUCESS;
            }
            catch { return CMD_RETURN.RESPONSE_CAN_NOT_RECOGNIZED; }

        }
        
        /// <summary>
        /// Set GPO status
        /// </summary>
        /// <param name="gpo_config"></param>
        /// <param name="rsp"></param>
        /// <returns></returns>
        public CMD_RETURN SetGPO(MANAGEMENT_CMD.GPO_CONFIG[] gpo_config, out MANAGEMENT_CMD.SET_GPO_RSP rsp)
        {
            setGPORspEvent = new SpeedwayManualResetEvent(false);
            invalidCommandNtfEvent = new SpeedwayManualResetEvent(false);
            WaitHandle[] waithandles = new WaitHandle[] {setGPORspEvent.evt, invalidCommandNtfEvent.evt };
            byte[] data = MANAGEMENT_CMD.GENERATE_SET_GPO_CMD(include_timestamp, gpo_config);

            ns.Write(data, 0, data.Length);
            rsp = null;
            int wait_result = WaitHandle.WaitAny(waithandles, CMD_RESPONSE_TIME_OUT, false);
            if (wait_result == 1) return CMD_RETURN.INVALID_COMMAND;
            else if (wait_result>1) return CMD_RETURN.CMD_RESPONSE_TIME_OUT; 
            try
            {
                rsp = new MANAGEMENT_CMD.SET_GPO_RSP(setGPORspEvent.data);
                return CMD_RETURN.COMMAND_SUCESS;
            }
            catch { return CMD_RETURN.RESPONSE_CAN_NOT_RECOGNIZED; }


        }
        
        /// <summary>
        /// Set GPI configuration
        /// </summary>
        /// <param name="gpi_config"></param>
        /// <param name="rsp"></param>
        /// <returns></returns>
        public CMD_RETURN SetGPI(MANAGEMENT_CMD.GPI_CONFIG[] gpi_config, out MANAGEMENT_CMD.SET_GPI_RSP rsp)
        {
            setGPIRspEvent = new SpeedwayManualResetEvent(false);
            invalidCommandNtfEvent = new SpeedwayManualResetEvent(false);
            WaitHandle[] waithandles = new WaitHandle[] { setGPIRspEvent.evt, invalidCommandNtfEvent.evt};

            byte[] data = MANAGEMENT_CMD.GENERATE_SET_GPI_CMD(include_timestamp, gpi_config);

            ns.Write(data, 0, data.Length);
            rsp = null;
            int wait_result = WaitHandle.WaitAny(waithandles, CMD_RESPONSE_TIME_OUT, false);
            if (wait_result == 1) return CMD_RETURN.INVALID_COMMAND;
            else if (wait_result>1) return CMD_RETURN.CMD_RESPONSE_TIME_OUT; 
            try
            {
                rsp = new MANAGEMENT_CMD.SET_GPI_RSP(setGPIRspEvent.data);
                return CMD_RETURN.COMMAND_SUCESS;
            }
            catch { return CMD_RETURN.RESPONSE_CAN_NOT_RECOGNIZED; }


        }
        
        /// <summary>
        /// Get GPI status
        /// </summary>
        /// <param name="rsp"></param>
        /// <returns></returns>
        public CMD_RETURN GetGPI(out MANAGEMENT_CMD.GET_GPI_RSP rsp)
        {
            getGPIRspEvent = new SpeedwayManualResetEvent(false);
            invalidCommandNtfEvent = new SpeedwayManualResetEvent(false);
            WaitHandle[] waithandles = new WaitHandle[] { getGPIRspEvent.evt, invalidCommandNtfEvent.evt};

            byte[] data = MANAGEMENT_CMD.GENERATE_GET_GPI_CMD(include_timestamp);

            ns.Write(data, 0, data.Length);
            rsp = null;

            int wait_result = WaitHandle.WaitAny(waithandles, CMD_RESPONSE_TIME_OUT, false);
            if (wait_result == 1) return CMD_RETURN.INVALID_COMMAND;
            else if (wait_result>1) return CMD_RETURN.CMD_RESPONSE_TIME_OUT; 
            try
            {
                rsp = new MANAGEMENT_CMD.GET_GPI_RSP(getGPIRspEvent.data);
                return CMD_RETURN.COMMAND_SUCESS;
            }
            catch { return CMD_RETURN.RESPONSE_CAN_NOT_RECOGNIZED; }

        }
        
        /// <summary>
        /// Enable/Disable status report
        /// </summary>
        /// <param name="enable"></param>
        /// <returns></returns>
        public CMD_RETURN SetStatusReport(bool enable)
        {
            setStatusReportRspEvent = new SpeedwayManualResetEvent(false);
            invalidCommandNtfEvent = new SpeedwayManualResetEvent(false);
            WaitHandle[] waithandles = new WaitHandle[] {setStatusReportRspEvent.evt, invalidCommandNtfEvent.evt };
            byte[] data = MANAGEMENT_CMD.GENERATE_SET_STATUS_REPORT_CMD(include_timestamp, enable);

            ns.Write(data, 0, data.Length);
            int result = WaitHandle.WaitAny(waithandles, CMD_RESPONSE_TIME_OUT, false);
            if (result == 1) return CMD_RETURN.INVALID_COMMAND;
            else if (result > 1) return CMD_RETURN.CMD_RESPONSE_TIME_OUT;
            else
                return CMD_RETURN.COMMAND_SUCESS;

        }

        /// <summary>
        /// Set TCP connection flashing behavior
        /// </summary>
        /// <param name="behavior"></param>
        /// <returns></returns>
        public CMD_RETURN SetTCPConnectionOptions(MANAGEMENT_CMD.MESSAGE_FLUSH_BEHAVIOR behavior)
        {
            setTcpConnectionOptionsRspEvent = new SpeedwayManualResetEvent(false);
            invalidCommandNtfEvent = new SpeedwayManualResetEvent(false);

            WaitHandle[] waithandles = new WaitHandle[] { setTcpConnectionOptionsRspEvent.evt, invalidCommandNtfEvent.evt};
            byte[] data = MANAGEMENT_CMD.GENERATE_SET_TCP_CONNECTION_OPTIONS_CMD(include_timestamp, behavior);

            ns.Write(data, 0, data.Length);
            int result = WaitHandle.WaitAny(waithandles, CMD_RESPONSE_TIME_OUT, false);
            if (result == 1) return CMD_RETURN.INVALID_COMMAND;
            else if (result > 1) return CMD_RETURN.CMD_RESPONSE_TIME_OUT;
            else
                return CMD_RETURN.COMMAND_SUCESS;
        }

        /// <summary>
        /// Get TCP connection flashing behavior
        /// </summary>
        /// <param name="report_behavior"></param>
        /// <param name="behavior"></param>
        /// <returns></returns>
        public CMD_RETURN GetTCPConnectionOptions(bool report_behavior, out MANAGEMENT_CMD.MESSAGE_FLUSH_BEHAVIOR behavior)
        {
            getTcpConnectionOptionsRspEvent = new SpeedwayManualResetEvent(false);
            invalidCommandNtfEvent = new SpeedwayManualResetEvent(false);

            behavior = MANAGEMENT_CMD.MESSAGE_FLUSH_BEHAVIOR.DEFAULT;

            WaitHandle[] waithandles = new WaitHandle[] { getTcpConnectionOptionsRspEvent.evt, invalidCommandNtfEvent.evt };
            byte[] data = MANAGEMENT_CMD.GENERATE_GET_TCP_CONNECTION_OPTIONS_CMD(include_timestamp, report_behavior);

            ns.Write(data, 0, data.Length);
            int result = WaitHandle.WaitAny(waithandles, CMD_RESPONSE_TIME_OUT, false);
            if (result == 1) return CMD_RETURN.INVALID_COMMAND;
            else if (result > 1) return CMD_RETURN.CMD_RESPONSE_TIME_OUT;
            else
            {
                if (getTcpConnectionOptionsRspEvent.data.Length > 2) behavior = (MANAGEMENT_CMD.MESSAGE_FLUSH_BEHAVIOR)getTcpConnectionOptionsRspEvent.data[2];
                return CMD_RETURN.COMMAND_SUCESS;
            }
        }

        #endregion 

        #region Operation Command

        /// <summary>
        /// Set regulatory region
        /// </summary>
        /// <param name="region"></param>
        /// <returns></returns>
        public CMD_RETURN SetRegulatoryRegion(REGULATORY_REGION region)
        {
            setRegionNtfEvent = new SpeedwayManualResetEvent(false);
            setRegionRspEvent = new SpeedwayManualResetEvent(false);
            invalidCommandNtfEvent = new SpeedwayManualResetEvent(false);

            WaitHandle[] waithandles = new WaitHandle[] { setRegionRspEvent.evt, invalidCommandNtfEvent.evt };

            byte[] data = OPERATION_CMD.GENERATE_SET_REGULATORY_REGION(include_timestamp, region);
            ns.Write(data, 0, data.Length);

            int wait_result = WaitHandle.WaitAny(waithandles, CMD_RESPONSE_TIME_OUT, false);
            if (wait_result == 1) return CMD_RETURN.INVALID_COMMAND;
            else if (wait_result>1) return CMD_RETURN.CMD_RESPONSE_TIME_OUT;

            if (!setRegionNtfEvent.evt.WaitOne(MODEM_INIT_TIME_OUT, false)) return CMD_RETURN.COMMAND_FAILED;

            return CMD_RETURN.COMMAND_SUCESS;

        }

        /// <summary>
        /// Invnetory
        /// </summary>
        /// <param name="prama">Inventory parameters</param>
        /// <param name="result">command result code</param>
        /// <returns></returns>
        public CMD_RETURN Inventory(OPERATION_CMD.INVENTORY_PRAMA prama, out OPERATION_CMD.INVENTORY_RESULT result)
        {
            inventoryRspEvent = new SpeedwayManualResetEvent(false);
            invalidCommandNtfEvent = new SpeedwayManualResetEvent(false);

            WaitHandle[] waithandles = new WaitHandle[] {inventoryRspEvent.evt, invalidCommandNtfEvent.evt };

            byte[] data = OPERATION_CMD.GENERATE_INVENTORY_CMD(include_timestamp, prama);

            result = OPERATION_CMD.INVENTORY_RESULT.FAIL_CONFIGURATION_ERROR;
            ns.Write(data, 0, data.Length);

            int wait_result = WaitHandle.WaitAny(waithandles, CMD_RESPONSE_TIME_OUT, false);
            if (wait_result == 1) return CMD_RETURN.INVALID_COMMAND;
            else if (wait_result>1) return CMD_RETURN.CMD_RESPONSE_TIME_OUT;
            else
            {
                try
                {
                    result = (OPERATION_CMD.INVENTORY_RESULT)inventoryRspEvent.data[0];
                    return CMD_RETURN.COMMAND_SUCESS;
                }
                catch { return CMD_RETURN.RESPONSE_CAN_NOT_RECOGNIZED; }
            }
        }

        /// <summary>
        /// Invnetory
        /// </summary>
        /// <param name="prama"></param>
        /// <param name="err_occur"></param>
        /// <returns></returns>
        public CMD_RETURN Inventory(OPERATION_CMD.INVENTORY_PRAMA prama, out bool err_occur)
        {
            inventoryRspEvent = new SpeedwayManualResetEvent(false);
            invalidCommandNtfEvent = new SpeedwayManualResetEvent(false);

            WaitHandle[] waithandles = new WaitHandle[] {inventoryRspEvent.evt, invalidCommandNtfEvent.evt };

            byte[] data = OPERATION_CMD.GENERATE_INVENTORY_CMD(include_timestamp, prama);

            err_occur = true;
            ns.Write(data, 0, data.Length);

            int wait_result = WaitHandle.WaitAny(waithandles, CMD_RESPONSE_TIME_OUT, false);
            if (wait_result == 1) return CMD_RETURN.INVALID_COMMAND;
            else if (wait_result>1) return CMD_RETURN.CMD_RESPONSE_TIME_OUT;
            else
            {
                try
                {
                    err_occur = (inventoryRspEvent.data[0] == 1) ? true : false;
                    return CMD_RETURN.COMMAND_SUCESS;
                }
                catch { return CMD_RETURN.RESPONSE_CAN_NOT_RECOGNIZED; }
            }
        }        
        
        /// <summary>
        /// Inventory continue 
        /// </summary>
        /// <param name="err_ocurr"></param>
        /// <returns></returns>
        public CMD_RETURN InventoryContinue(out bool err_ocurr)
        {
            inventoryContinueRspEvent = new SpeedwayManualResetEvent(false);
            invalidCommandNtfEvent = new SpeedwayManualResetEvent(false);
            WaitHandle[] waithandles = new WaitHandle[] { inventoryContinueRspEvent.evt, invalidCommandNtfEvent.evt};

            byte[] data = OPERATION_CMD.GENERATE_INVENTORY_CONTINUE_CMD(include_timestamp);

            err_ocurr = true;
            ns.Write(data, 0, data.Length);

            int wait_result = WaitHandle.WaitAny(waithandles, CMD_RESPONSE_TIME_OUT, false);
            if (wait_result == 1) return CMD_RETURN.INVALID_COMMAND;
            else if (wait_result>1) return CMD_RETURN.CMD_RESPONSE_TIME_OUT;
            else
            {
                err_ocurr = (inventoryContinueRspEvent.data[0] == 1) ? true : false;
                return CMD_RETURN.COMMAND_SUCESS;
            }
        }

        /// <summary>
        /// Get Operation command set verison
        /// </summary>
        /// <param name="rsp"></param>
        /// <returns></returns>
        public CMD_RETURN GetOCSVersion(out OPERATION_CMD.OCS_VERSION_RSP rsp)
        {
            getOCSVersionRspEvent = new SpeedwayManualResetEvent(false);
            invalidCommandNtfEvent = new SpeedwayManualResetEvent(false);
            WaitHandle[] waithandles = new WaitHandle[] { getOCSVersionRspEvent.evt, invalidCommandNtfEvent.evt};
            
            byte[] data = OPERATION_CMD.GENERATE_GET_OCS_VERSION_CMD(include_timestamp);
            ns.Write(data, 0, data.Length);
            rsp = null;

            int wait_result = WaitHandle.WaitAny(waithandles, CMD_RESPONSE_TIME_OUT, false);
            if (wait_result == 1) return CMD_RETURN.INVALID_COMMAND;
            else if (wait_result>1) return CMD_RETURN.CMD_RESPONSE_TIME_OUT;
            
            try
            {
                rsp = new OPERATION_CMD.OCS_VERSION_RSP(getOCSVersionRspEvent.data);
                return CMD_RETURN.COMMAND_SUCESS;
            }
            catch
            {
                return CMD_RETURN.RESPONSE_CAN_NOT_RECOGNIZED;
            }

        }

        /// <summary>
        /// Load modem parameters from profile
        /// </summary>
        /// <param name="profile_index">profile index, value is 0-15</param>
        /// <returns></returns>
        public CMD_RETURN LoadFromProfile(UInt16 profile_index)
        {
            loadFromProfileRspEvent = new SpeedwayManualResetEvent(false);
            invalidCommandNtfEvent = new SpeedwayManualResetEvent(false);
            WaitHandle[] waithandles = new WaitHandle[] {loadFromProfileRspEvent.evt, invalidCommandNtfEvent.evt};
            
            byte[] data = OPERATION_CMD.GENERATE_LOAD_FROM_PROFILE_CMD(include_timestamp, profile_index);
            ns.Write(data, 0, data.Length);

            int wait_result = WaitHandle.WaitAny(waithandles, CMD_RESPONSE_TIME_OUT, false);
            if (wait_result == 1) return CMD_RETURN.INVALID_COMMAND;
            else if (wait_result>1) return CMD_RETURN.CMD_RESPONSE_TIME_OUT;
            else
                return CMD_RETURN.COMMAND_SUCESS;

        }

        /// <summary>
        /// Store modem parameters to profile
        /// </summary>
        /// <param name="profile_index">profile index, value is 0-15</param>
        /// <param name="dst"></param>
        /// <returns></returns>
        public CMD_RETURN StoreToProfile(UInt16 profile_index, OPERATION_CMD.SOURCE dst)
        {
            storeToProfileRspEvent = new SpeedwayManualResetEvent(false);
            invalidCommandNtfEvent = new SpeedwayManualResetEvent(false);
            WaitHandle[] waithandles = new WaitHandle[] { storeToProfileRspEvent.evt, invalidCommandNtfEvent.evt };

            byte[] data = OPERATION_CMD.GENERATE_STORE_TO_PROFILE_CMD(include_timestamp, profile_index, dst);
            ns.Write(data, 0, data.Length);

            int wait_result = WaitHandle.WaitAny(waithandles, CMD_RESPONSE_TIME_OUT, false);
            if (wait_result == 1) return CMD_RETURN.INVALID_COMMAND;
            else if (wait_result>1) return CMD_RETURN.CMD_RESPONSE_TIME_OUT;
            else
                return CMD_RETURN.COMMAND_SUCESS;
        }
        
        /// <summary>
        /// Set tx power
        /// </summary>
        /// <param name="power">Array of tx power, array size is 1 or 5, if only one element presents, 
        /// power is applied to all the antenna ports. else element 2-5 are individual tx power for 
        /// each antenna port.  Value is 15.0 - 32.5</param>
        /// <param name="result_code"></param>
        /// <returns></returns>
        public CMD_RETURN SetTxPower(float[] power, out OPERATION_CMD.SET_TX_POWER_RESULT result_code)
        {
            byte[] power_index = new byte[power.Length];
            for (int i = 0; i < power.Length; i++) power_index[i] = (byte)(68 + power[i] * 4);
            
            setTxPowerRspEvent = new SpeedwayManualResetEvent(false);
            invalidCommandNtfEvent = new SpeedwayManualResetEvent(false);
            WaitHandle[] waithandles = new WaitHandle[] { setTxPowerRspEvent.evt, invalidCommandNtfEvent.evt};

            byte[] data = OPERATION_CMD.GENERATE_SET_TX_POWER_CMD(include_timestamp, power_index);
            result_code = 0;
            ns.Write(data, 0, data.Length);
            int wait_result = WaitHandle.WaitAny(waithandles, CMD_RESPONSE_TIME_OUT, false);
            if (wait_result == 1) return CMD_RETURN.INVALID_COMMAND;
            else if (wait_result>1) return CMD_RETURN.CMD_RESPONSE_TIME_OUT;
            else
            {
                result_code = (OPERATION_CMD.SET_TX_POWER_RESULT)setTxPowerRspEvent.data[0];
                return CMD_RETURN.COMMAND_SUCESS;
            }
        }

        /// <summary>
        /// Get tx power of each antenna
        /// </summary>
        /// <param name="powers"></param>
        /// <returns></returns>
        public CMD_RETURN GetTxPower(out float[] powers)
        {
            getTxPowerRspEvent = new SpeedwayManualResetEvent(false);
            invalidCommandNtfEvent = new SpeedwayManualResetEvent(false);
            WaitHandle[] waithandles = new WaitHandle[] { getTxPowerRspEvent.evt, invalidCommandNtfEvent.evt};

            byte[] data = OPERATION_CMD.GENERATE_GET_TX_POWER_CMD(include_timestamp);
            powers = null;
            ns.Write(data, 0, data.Length);

            int wait_result = WaitHandle.WaitAny(waithandles, CMD_RESPONSE_TIME_OUT, false);
            if (wait_result == 1) return CMD_RETURN.INVALID_COMMAND;
            else if (wait_result>1) return CMD_RETURN.CMD_RESPONSE_TIME_OUT;
            else
            {

                if (getTxPowerRspEvent.data.Length == 1)
                {
                    powers = new float[1];
                    powers[0] = (float)((getTxPowerRspEvent.data[0] - 68) / 4.0);
                }
                else if(getTxPowerRspEvent.data.Length >= 4)
                {
                    powers = new float[getTxPowerRspEvent.data.Length - 3];
                    powers[0] = (float)((getTxPowerRspEvent.data[0] - 68) / 4.0);

                    for (int i = 1; i < powers.Length; i++) powers[i] = (float)((getTxPowerRspEvent.data[3+i] - 68) / 4.0);
                }
                return CMD_RETURN.COMMAND_SUCESS;
            }

        }

        /// <summary>
        /// Enable/Disable antenna
        /// </summary>
        /// <param name="antennas"> Antenna to be enabled/disabled. e.g. ANTENNA_PORT_1|ANTENNA_PORT_2 </param>
        /// <param name="result"></param>
        /// <returns></returns>
        public CMD_RETURN SetAntenna(byte antennas, out OPERATION_CMD.SET_ANTENNA_RESULT result)
        {
            setAntennaRspEvent = new SpeedwayManualResetEvent(false);
            invalidCommandNtfEvent = new SpeedwayManualResetEvent(false);
            WaitHandle[] waithandles = new WaitHandle[] {setAntennaRspEvent.evt, invalidCommandNtfEvent.evt};

            byte[] data = OPERATION_CMD.GENERATE_SET_ANTENNA_CMD(include_timestamp, antennas);
            result = OPERATION_CMD.SET_ANTENNA_RESULT.PORT_NOT_AVAILABLE;
            ns.Write(data, 0, data.Length);

            int wait_result = WaitHandle.WaitAny(waithandles, CMD_RESPONSE_TIME_OUT, false);
            if (wait_result == 1) return CMD_RETURN.INVALID_COMMAND;
            else if (wait_result>1) return CMD_RETURN.CMD_RESPONSE_TIME_OUT;
            else
            {
                result = (OPERATION_CMD.SET_ANTENNA_RESULT)setAntennaRspEvent.data[0];
                return CMD_RETURN.COMMAND_SUCESS;
            }

        }
        
        /// <summary>
        /// Get antenna port status
        /// </summary>
        /// <param name="antenna_port_status">status array, element 0 is antenna 1</param>
        /// <returns></returns>
        public CMD_RETURN GetAntenna(out bool[] antenna_port_status)
        {
            antenna_port_status = new bool[4];
            getAntennaRspEvent = new SpeedwayManualResetEvent(false);
            invalidCommandNtfEvent = new SpeedwayManualResetEvent(false);
            WaitHandle[] waithandles = new WaitHandle[] {getAntennaRspEvent.evt, invalidCommandNtfEvent.evt };
            byte[] data = OPERATION_CMD.GENERATE_GET_ANTENNA_CMD(include_timestamp);

            //antenna_byte = 0;
            ns.Write(data, 0, data.Length);
            int wait_result = WaitHandle.WaitAny(waithandles, CMD_RESPONSE_TIME_OUT, false);
            if (wait_result == 1) return CMD_RETURN.INVALID_COMMAND;
            else if (wait_result > 1 ) return CMD_RETURN.CMD_RESPONSE_TIME_OUT;
            else
            {
                antenna_port_status[3] = ((getAntennaRspEvent.data[0] & 0x08) == 0x08) ? true : false;
                antenna_port_status[2] = ((getAntennaRspEvent.data[0] & 0x04) == 0x04) ? true : false;
                antenna_port_status[1] = ((getAntennaRspEvent.data[0] & 0x02) == 0x02) ? true : false;
                antenna_port_status[0] = ((getAntennaRspEvent.data[0] & 0x01) == 0x01) ? true : false;

                return CMD_RETURN.COMMAND_SUCESS;
            }
        }

        /// <summary>
        /// Get tx power capacities
        /// </summary>
        /// <param name="reportPower">enable/disable reporting power</param>
        /// <param name="reportFrequencyList">enable/diable report frequency list</param>
        /// <param name="min_support_power"></param>
        /// <param name="max_support_power"></param>
        /// <param name="frequencies">return supported frequency list</param>
        /// <returns></returns>
        public CMD_RETURN GetCapacities(bool reportPower, bool reportFrequencyList, out float min_support_power, out float max_support_power, out ArrayList frequencies)
        {
            frequencies = null;

            getCapabilitiesRspEvent = new SpeedwayManualResetEvent(false);
            invalidCommandNtfEvent = new SpeedwayManualResetEvent(false);
            WaitHandle[] waithandles = new WaitHandle[] { getCapabilitiesRspEvent.evt,invalidCommandNtfEvent.evt };

            byte[] data = OPERATION_CMD.GENERATE_GET_CAPABILITIES_CMD(include_timestamp, reportPower, reportFrequencyList);
            min_support_power = 0F;
            max_support_power = 0F;

            ns.Write(data, 0, data.Length);
            int wait_result = WaitHandle.WaitAny(waithandles, CMD_RESPONSE_TIME_OUT, false);
            if (wait_result == 1) return CMD_RETURN.INVALID_COMMAND;
            else if (wait_result > 1) return CMD_RETURN.CMD_RESPONSE_TIME_OUT;
            else
            {
                try
                {
                    int offset = 0;

                    while (offset < getCapabilitiesRspEvent.data.Length)
                    {
                        switch (getCapabilitiesRspEvent.data[offset++])
                        {
                            case 0x01:
                                min_support_power = (float)((getCapabilitiesRspEvent.data[offset++] - 68) * 0.25);
                                break;
                            case 0x02:
                                max_support_power = (float)((getCapabilitiesRspEvent.data[offset++] - 68) * 0.25);
                                break;
                            case 0x03:
                                frequencies = new ArrayList();
                                int len = getCapabilitiesRspEvent.data[offset++] << 8 + getCapabilitiesRspEvent.data[offset++];
                                for (int i = 0; i < len/2; i++)
                                {
                                    frequencies.Add(getCapabilitiesRspEvent.data[offset++] << 8 + getCapabilitiesRspEvent.data[offset++]);
                                }
                                break;
                            default:
                                offset++;
                                break;
                        }
                    }

                    return CMD_RETURN.COMMAND_SUCESS;
                }
                catch { return CMD_RETURN.RESPONSE_CAN_NOT_RECOGNIZED; }
            }

        }

        /// <summary>
        /// Set tx frequency
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="center_frequency_index">frequency index. valid only when mode is CENTER_FREQUENCY. value is 0-2000, as 860MHz-960Mhz</param>
        /// <param name="frequency_list">List of frequency. valid only when mode is CHOOSE_FROM_LIST. value is 0-2000 </param>
        /// <param name="result"></param>
        /// <returns></returns>
        public CMD_RETURN SetTxFrequency(OPERATION_CMD.FREQUENCY_SET_MODE mode, UInt16 center_frequency_index, UInt16[] frequency_list, out OPERATION_CMD.SET_FREQUENCY_RESULT result)
        {
            setTxFrequencyRspEvent = new SpeedwayManualResetEvent(false);
            invalidCommandNtfEvent = new SpeedwayManualResetEvent(false);
            WaitHandle[] waithandles = new WaitHandle[] { setTxFrequencyRspEvent.evt, invalidCommandNtfEvent.evt};

            byte[] data = OPERATION_CMD.GENERATE_SET_TX_FREQUENCY(include_timestamp, mode, center_frequency_index, frequency_list, new UInt16[0]);

            ns.Write(data, 0, data.Length);
            result = OPERATION_CMD.SET_FREQUENCY_RESULT.ERROR;
            int wait_result = WaitHandle.WaitAny(waithandles, CMD_RESPONSE_TIME_OUT, false);
            if (wait_result == 1) return CMD_RETURN.INVALID_COMMAND;
            else if (wait_result>1) return CMD_RETURN.CMD_RESPONSE_TIME_OUT;
            else
            {
                result = (OPERATION_CMD.SET_FREQUENCY_RESULT)setTxFrequencyRspEvent.data[0];
                return CMD_RETURN.COMMAND_SUCESS;
            }
        }

        /// <summary>
        /// Set tx frequency with Reduced Power Frequecy List as parameters
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="center_frequency_index">frequency index. valid only when mode is CENTER_FREQUENCY. value is 0-2000, as 860MHz-960Mhz</param>
        /// <param name="frequency_list">List of frequency. valid only when mode is CHOOSE_FROM_LIST. value is 0-2000 </param>
        /// <param name="reduced_power_frequency_list">List of reduced power frequecy, only supported by FCC reader</param>
        /// <param name="result"></param>
        /// <returns></returns>
        public CMD_RETURN SetTxFrequency(OPERATION_CMD.FREQUENCY_SET_MODE mode, UInt16 center_frequency_index, UInt16[] frequency_list, UInt16[] reduced_power_frequency_list, out OPERATION_CMD.SET_FREQUENCY_RESULT result)
        {
            setTxFrequencyRspEvent = new SpeedwayManualResetEvent(false);
            invalidCommandNtfEvent = new SpeedwayManualResetEvent(false);
            WaitHandle[] waithandles = new WaitHandle[] { setTxFrequencyRspEvent.evt, invalidCommandNtfEvent.evt };

            byte[] data = OPERATION_CMD.GENERATE_SET_TX_FREQUENCY(include_timestamp, mode, center_frequency_index, frequency_list, reduced_power_frequency_list);

            ns.Write(data, 0, data.Length);
            result = OPERATION_CMD.SET_FREQUENCY_RESULT.ERROR;
            int wait_result = WaitHandle.WaitAny(waithandles, CMD_RESPONSE_TIME_OUT, false);
            if (wait_result == 1) return CMD_RETURN.INVALID_COMMAND;
            else if (wait_result > 1) return CMD_RETURN.CMD_RESPONSE_TIME_OUT;
            else
            {
                result = (OPERATION_CMD.SET_FREQUENCY_RESULT)setTxFrequencyRspEvent.data[0];
                return CMD_RETURN.COMMAND_SUCESS;
            }
        }
        
        /// <summary>
        /// Set Gen 2 parameters
        /// </summary>
        /// <param name="gen2_param"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public CMD_RETURN SetGen2Params(OPERATION_CMD.GEN2_PARAM gen2_param, out OPERATION_CMD.SET_GEN2_PARAMS_RESULT result)
        {
            setGen2ParamsRspEvent = new SpeedwayManualResetEvent(false);
            invalidCommandNtfEvent = new SpeedwayManualResetEvent(false);
            WaitHandle[] waithandles = new WaitHandle[] { setGen2ParamsRspEvent.evt, invalidCommandNtfEvent.evt};

            byte[] data = OPERATION_CMD.GENERATE_SET_GEN2_PARAMS_CMD(include_timestamp, gen2_param);

            result = OPERATION_CMD.SET_GEN2_PARAMS_RESULT.SESSION_AND_INVENTORY_MODE_COMBINATION_NOT_SUPPORTED;
            ns.Write(data, 0, data.Length);
            int wait_result = WaitHandle.WaitAny(waithandles, CMD_RESPONSE_TIME_OUT, false);
            if (wait_result == 1) return CMD_RETURN.INVALID_COMMAND;
            else if (wait_result>1) return CMD_RETURN.CMD_RESPONSE_TIME_OUT;
            else
            {
                result = (OPERATION_CMD.SET_GEN2_PARAMS_RESULT)setGen2ParamsRspEvent.data[0];
                return CMD_RETURN.COMMAND_SUCESS;
            }
            

        }
        
        /// <summary>
        /// Get Gen 2 parameters
        /// </summary>
        /// <param name="gen2_param"></param>
        /// <returns></returns>
        public CMD_RETURN GetGen2Params(out OPERATION_CMD.GEN2_PARAM gen2_param)
        {
            getGen2ParamsRspEvent = new SpeedwayManualResetEvent(false);
            invalidCommandNtfEvent = new SpeedwayManualResetEvent(false);
            WaitHandle[] waithandles = new WaitHandle[] {getGen2ParamsRspEvent.evt, invalidCommandNtfEvent.evt };

            byte[] data = OPERATION_CMD.GENERATE_GET_GEN2_PARAMS_CMD(include_timestamp);

            gen2_param = null;
            ns.Write(data, 0, data.Length);
            int wait_result = WaitHandle.WaitAny(waithandles, CMD_RESPONSE_TIME_OUT, false);
            if (wait_result == 1) return CMD_RETURN.INVALID_COMMAND;
            else if (wait_result>1) return CMD_RETURN.CMD_RESPONSE_TIME_OUT; try
            {
                gen2_param = new OPERATION_CMD.GEN2_PARAM(getGen2ParamsRspEvent.data);
                return CMD_RETURN.COMMAND_SUCESS;
            }
            catch { return CMD_RETURN.RESPONSE_CAN_NOT_RECOGNIZED; }

        }

        /// <summary>
        /// Get Gen 2 parameters
        /// </summary>
        /// <param name="gen2_param"></param>
        /// <returns></returns>
        public CMD_RETURN GetGen2Params(bool report_search_mode, out OPERATION_CMD.GEN2_PARAM gen2_param)
        {
            getGen2ParamsRspEvent = new SpeedwayManualResetEvent(false);
            invalidCommandNtfEvent = new SpeedwayManualResetEvent(false);
            WaitHandle[] waithandles = new WaitHandle[] { getGen2ParamsRspEvent.evt, invalidCommandNtfEvent.evt };

            byte[] data = OPERATION_CMD.GENERATE_GET_GEN2_PARAMS_CMD(report_search_mode, include_timestamp);

            gen2_param = null;
            ns.Write(data, 0, data.Length);
            int wait_result = WaitHandle.WaitAny(waithandles, CMD_RESPONSE_TIME_OUT, false);
            if (wait_result == 1) return CMD_RETURN.INVALID_COMMAND;
            else if (wait_result > 1) return CMD_RETURN.CMD_RESPONSE_TIME_OUT; try
            {
                gen2_param = new OPERATION_CMD.GEN2_PARAM(getGen2ParamsRspEvent.data);
                return CMD_RETURN.COMMAND_SUCESS;
            }
            catch { return CMD_RETURN.RESPONSE_CAN_NOT_RECOGNIZED; }

        }
       
        /// <summary>
        /// Set receive sensitivity.
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="sensitivities">value is -30 to -128. if mode = DEFAULT. This parameter is ignored.</param>
        /// <param name="err_ocurr"></param>
        /// <returns></returns>
        public CMD_RETURN SetRxConfig(OPERATION_CMD.SET_RX_SENSITIVITY_MODE mode, short[] sensitivities, out bool err_ocurr)
        {
            setRxConfigRspEvent = new SpeedwayManualResetEvent(false);
            invalidCommandNtfEvent = new SpeedwayManualResetEvent(false);
            WaitHandle[] waithandles = new WaitHandle[] {setRxConfigRspEvent.evt, invalidCommandNtfEvent.evt };
            byte[] data = OPERATION_CMD.GENERATE_SET_RX_CONFIG_CMD(mode, sensitivities, include_timestamp);

            err_ocurr = true;

            ns.Write(data, 0, data.Length);
            int wait_result = WaitHandle.WaitAny(waithandles, CMD_RESPONSE_TIME_OUT, false);
            if (wait_result == 1) return CMD_RETURN.INVALID_COMMAND;
            else if (wait_result>1) return CMD_RETURN.CMD_RESPONSE_TIME_OUT;
            try
            {
                err_ocurr = (setRxConfigRspEvent.data[0] == 1) ? true : false;
                return CMD_RETURN.COMMAND_SUCESS;
            }
            catch { return CMD_RETURN.RESPONSE_CAN_NOT_RECOGNIZED; }
        }
        
        /// <summary>
        /// Get receive sensitivity
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="sensitivities"></param>
        /// <returns></returns>
        public CMD_RETURN GetRxConfig(out OPERATION_CMD.SET_RX_SENSITIVITY_MODE mode, out short[] sensitivities)
        {
            getRxConfigRspEvent = new SpeedwayManualResetEvent(false);
            invalidCommandNtfEvent = new SpeedwayManualResetEvent(false);
            WaitHandle[] waithandles = new WaitHandle[] { getRxConfigRspEvent.evt, invalidCommandNtfEvent.evt};

            byte[] data = OPERATION_CMD.GENERATE_GET_RX_CONFIG_CMD(include_timestamp);

            mode = OPERATION_CMD.SET_RX_SENSITIVITY_MODE.MAXIMUM_SENSITIVITY;
            sensitivities = null;

            ns.Write(data, 0, data.Length);

            int wait_result = WaitHandle.WaitAny(waithandles, CMD_RESPONSE_TIME_OUT, false);
            if (wait_result == 1) return CMD_RETURN.INVALID_COMMAND;
            else if (wait_result>1) return CMD_RETURN.CMD_RESPONSE_TIME_OUT;
            try
            {
                mode = (OPERATION_CMD.SET_RX_SENSITIVITY_MODE)getRxConfigRspEvent.data[0];
                if (getRxConfigRspEvent.data.Length > 3)
                {
                    int len = getRxConfigRspEvent.data.Length - 4;
                    sensitivities = new short[len];
                    for (int i = 0; i < len; i++)
                    {
                        sensitivities[i] = (short)(0 - ((getRxConfigRspEvent.data[4 + i] ^ 0xFF) + 1));
                    }
                }

                return CMD_RETURN.COMMAND_SUCESS;
            }
            catch
            {
                return CMD_RETURN.RESPONSE_CAN_NOT_RECOGNIZED;
            }
        }
        
        /// <summary>
        /// Get tx frequency
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="center_frequency"></param>
        /// <param name="frequency_list"></param>
        /// <returns></returns>
        public CMD_RETURN GetTxFrequency(out OPERATION_CMD.FREQUENCY_SET_MODE mode, out UInt16 center_frequency, out UInt16[] frequency_list)
        {
            getTxFrequencyRspEvent = new SpeedwayManualResetEvent(false);
            invalidCommandNtfEvent = new SpeedwayManualResetEvent(false);
            WaitHandle[] waithandles = new WaitHandle[] { getTxFrequencyRspEvent.evt, invalidCommandNtfEvent.evt};

            mode = OPERATION_CMD.FREQUENCY_SET_MODE.CHOOSE_FROM_REGULATORY;
            center_frequency = 0;
            frequency_list = null;

            byte[] data = OPERATION_CMD.GENERATE_GET_TX_FREQUENCY(include_timestamp);
            ns.Write(data, 0, data.Length);

            int wait_result = WaitHandle.WaitAny(waithandles, CMD_RESPONSE_TIME_OUT, false);
            if (wait_result == 1) return CMD_RETURN.INVALID_COMMAND;
            else if (wait_result>1) return CMD_RETURN.CMD_RESPONSE_TIME_OUT;

            try
            {
                mode = (OPERATION_CMD.FREQUENCY_SET_MODE)getTxFrequencyRspEvent.data[0];

                if (mode == OPERATION_CMD.FREQUENCY_SET_MODE.CENTER_FREQUENCY)
                {
                    center_frequency = (UInt16)(getTxFrequencyRspEvent.data[2] * 256 + getTxFrequencyRspEvent.data[3]);
                }
                else if (mode == OPERATION_CMD.FREQUENCY_SET_MODE.CHOOSE_FROM_LIST)
                {
                    int len = getTxFrequencyRspEvent.data.Length - 4;
                    frequency_list = new UInt16[len];

                    for (int i = 0; i < len / 2; i++)
                    {
                        try
                        {
                            frequency_list[i] = (UInt16)(getTxFrequencyRspEvent.data[4 + 2 * i] * 256 + getTxFrequencyRspEvent.data[5 + 2 * i]);
                        }
                        catch { }
                    }
                }

                return CMD_RETURN.COMMAND_SUCESS;
            }
            catch
            {
                return CMD_RETURN.RESPONSE_CAN_NOT_RECOGNIZED;
            }

        }

        /// <summary>
        /// Get tx frequency
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="center_frequency"></param>
        /// <param name="frequency_list"></param>
        /// <param name="reducedPowerFrequency_list"></param>
        /// <returns></returns>
        public CMD_RETURN GetTxFrequency(out OPERATION_CMD.FREQUENCY_SET_MODE mode, out UInt16 center_frequency, out UInt16[] frequency_list, out UInt16[] reducedPowerFrequency_list)
        {
            getTxFrequencyRspEvent = new SpeedwayManualResetEvent(false);
            invalidCommandNtfEvent = new SpeedwayManualResetEvent(false);
            WaitHandle[] waithandles = new WaitHandle[] { getTxFrequencyRspEvent.evt, invalidCommandNtfEvent.evt };

            mode = OPERATION_CMD.FREQUENCY_SET_MODE.CHOOSE_FROM_REGULATORY;
            center_frequency = 0;

            frequency_list = null;
            reducedPowerFrequency_list = null;

            byte[] data = OPERATION_CMD.GENERATE_GET_TX_FREQUENCY(include_timestamp);
            ns.Write(data, 0, data.Length);

            int wait_result = WaitHandle.WaitAny(waithandles, CMD_RESPONSE_TIME_OUT, false);
            if (wait_result == 1) return CMD_RETURN.INVALID_COMMAND;
            else if (wait_result > 1) return CMD_RETURN.CMD_RESPONSE_TIME_OUT;

            try
            {
                mode = (OPERATION_CMD.FREQUENCY_SET_MODE)getTxFrequencyRspEvent.data[0];

                if (mode == OPERATION_CMD.FREQUENCY_SET_MODE.CENTER_FREQUENCY)
                {
                    center_frequency = (UInt16)(getTxFrequencyRspEvent.data[2] * 256 + getTxFrequencyRspEvent.data[3]);
                }
                else if (mode == OPERATION_CMD.FREQUENCY_SET_MODE.CHOOSE_FROM_LIST)
                {
                    int len = getTxFrequencyRspEvent.data[2]>>8 + getTxFrequencyRspEvent.data[3];
                    
                    frequency_list = new UInt16[len];

                    for (int i = 0; i < len / 2; i++)
                    {
                        try
                        {
                            frequency_list[i] = (UInt16)(getTxFrequencyRspEvent.data[4 + 2 * i] * 256 + getTxFrequencyRspEvent.data[5 + 2 * i]);
                        }
                        catch { }
                    }
                }
                else if (mode == OPERATION_CMD.FREQUENCY_SET_MODE.REDUCED_POWER_FREQUENCY_LIST)
                {
                    int len = getTxFrequencyRspEvent.data[2] >> 8 + getTxFrequencyRspEvent.data[3];

                    reducedPowerFrequency_list = new UInt16[len];

                    for (int i = 0; i < len / 2; i++)
                    {
                        try
                        {
                            reducedPowerFrequency_list[i] = (UInt16)(getTxFrequencyRspEvent.data[4 + 2 * i] * 256 + getTxFrequencyRspEvent.data[5 + 2 * i]);
                        }
                        catch { }
                    }
                }

                return CMD_RETURN.COMMAND_SUCESS;
            }
            catch
            {
                return CMD_RETURN.RESPONSE_CAN_NOT_RECOGNIZED;
            }

        }
        

        /// <summary>
        /// Get supported Gen2 paramters
        /// </summary>
        /// <param name="include_mode_id">indicate if contain mode id in return data</param>
        /// <param name="include_inventory_search_mode">indicate if contain inventory search mode in return data</param>
        /// <param name="rsp"></param>
        /// <returns></returns>
        public CMD_RETURN GetSupportedGen2Params(bool include_mode_id, bool include_inventory_search_mode, out OPERATION_CMD.SUPPORTED_GEN2_PARAMS_RSP rsp)
        {
            getSupportedGen2ParamsRspEvent = new SpeedwayManualResetEvent(false);
            invalidCommandNtfEvent = new SpeedwayManualResetEvent(false);
            WaitHandle[] waithandles = new WaitHandle[] { getSupportedGen2ParamsRspEvent.evt, invalidCommandNtfEvent.evt};

            byte[] data = OPERATION_CMD.GENERATE_GET_SUPPORTED_GEN2_PARAMS_CMD(include_timestamp, include_mode_id, include_inventory_search_mode);
            ns.Write(data, 0, data.Length);

            rsp = null;
            int wait_result = WaitHandle.WaitAny(waithandles, CMD_RESPONSE_TIME_OUT, false);
            if (wait_result == 1) return CMD_RETURN.INVALID_COMMAND;
            else if (wait_result>1) return CMD_RETURN.CMD_RESPONSE_TIME_OUT;
           
            try
            {
                rsp = new OPERATION_CMD.SUPPORTED_GEN2_PARAMS_RSP(getSupportedGen2ParamsRspEvent.data);
                return CMD_RETURN.COMMAND_SUCESS;
            }
            catch
            {
                return CMD_RETURN.RESPONSE_CAN_NOT_RECOGNIZED;
            }
        }
        
        /// <summary>
        /// Check antenna status
        /// </summary>
        /// <param name="ant_ntf"></param>
        /// <returns></returns>
        public CMD_RETURN CheckAntenna(out OPERATION_NTF.CHECK_ANTENNA_NTF ant_ntf)
        {
            ant_ntf = null;

            checkAntennaRspEvent = new SpeedwayManualResetEvent(false);
            checkAntennaNtfEvent = new SpeedwayManualResetEvent(false);
            invalidCommandNtfEvent = new SpeedwayManualResetEvent(false);
            WaitHandle[] waithandles = new WaitHandle[] { checkAntennaRspEvent.evt, invalidCommandNtfEvent.evt};


            byte[] data = OPERATION_CMD.GENERATE_CHECK_ANTENNA_CMD(include_timestamp);

            ns.Write(data, 0, data.Length);
            int wait_result = WaitHandle.WaitAny(waithandles, CMD_RESPONSE_TIME_OUT, false);
            if (wait_result == 1) return CMD_RETURN.INVALID_COMMAND;
            else if (wait_result>1) return CMD_RETURN.CMD_RESPONSE_TIME_OUT;            
            
            if (!checkAntennaNtfEvent.evt.WaitOne(CMD_RESPONSE_TIME_OUT, false)) return CMD_RETURN.COMMAND_FAILED;

            try
            {
                ant_ntf = new OPERATION_NTF.CHECK_ANTENNA_NTF(checkAntennaNtfEvent.data);
                return CMD_RETURN.COMMAND_SUCESS;
            }
            catch { return CMD_RETURN.COMMAND_FAILED; }
        }
        
        /// <summary>
        /// Enable/Disable inventory report 
        /// </summary>
        /// <param name="enable"></param>
        /// <param name="err_occur"></param>
        /// <returns></returns>
        public CMD_RETURN SetInventoryReport(bool enable, OPERATION_CMD.OPTIONAL_INVENTORY_REPORT_PARAM param, out bool err_occur)
        {
            setInventoryReportRspEvent = new SpeedwayManualResetEvent(false);
            invalidCommandNtfEvent = new SpeedwayManualResetEvent(false);
            WaitHandle[] waithandles = new WaitHandle[] { setInventoryReportRspEvent.evt, invalidCommandNtfEvent.evt};

            byte[] data = OPERATION_CMD.GENERATE_SET_INVENTORY_REPORT(include_timestamp, !enable, param);

            err_occur = true;
            ns.Write(data, 0, data.Length);
            int wait_result = WaitHandle.WaitAny(waithandles, CMD_RESPONSE_TIME_OUT, false);
            if (wait_result == 1) return CMD_RETURN.INVALID_COMMAND;
            else if (wait_result>1) return CMD_RETURN.CMD_RESPONSE_TIME_OUT;
            else
            {
                err_occur = (setInventoryReportRspEvent.data[0] == 1) ? true : false;
                return CMD_RETURN.COMMAND_SUCESS;
            }

        }

        /// <summary>
        /// Enable Inventory report
        /// </summary>
        /// <param name="rst"></param>
        /// <returns></returns>
        public CMD_RETURN ReportInventory(out OPERATION_CMD.REPORT_INVENTORY_RESULT rst)
        {
            reportInventoryRspEvent = new SpeedwayManualResetEvent(false);
            invalidCommandNtfEvent = new SpeedwayManualResetEvent(false);
            WaitHandle[] waithandles = new WaitHandle[] { reportInventoryRspEvent.evt, invalidCommandNtfEvent.evt};

            rst = OPERATION_CMD.REPORT_INVENTORY_RESULT.ERROR;
            byte[] data = OPERATION_CMD.GENERATE_REPORT_INVENTORY_CMD(include_timestamp);

            ns.Write(data, 0, data.Length);
            int wait_result = WaitHandle.WaitAny(waithandles, CMD_RESPONSE_TIME_OUT, false);
            if (wait_result == 1) return CMD_RETURN.INVALID_COMMAND;
            else if (wait_result > 1) return CMD_RETURN.CMD_RESPONSE_TIME_OUT;
            else
            {
                try
                {
                    rst = (OPERATION_CMD.REPORT_INVENTORY_RESULT)reportInventoryRspEvent.data[0];
                    return CMD_RETURN.COMMAND_SUCESS;
                }
                catch { return CMD_RETURN.RESPONSE_CAN_NOT_RECOGNIZED; }
            }
        }
        
        /// <summary>
        /// Set listen before talk parameters
        /// </summary>
        /// <param name="disable_auto_select"></param>
        /// <param name="err_ocurr"></param>
        /// <returns></returns>
        public CMD_RETURN SetLBTParams(bool disable_auto_select, out bool err_ocurr)
        {
            setLBTParamsRspEvent = new SpeedwayManualResetEvent(false);
            invalidCommandNtfEvent = new SpeedwayManualResetEvent(false);
            WaitHandle[] waithandles = new WaitHandle[] { setLBTParamsRspEvent.evt, invalidCommandNtfEvent.evt};
            byte[] data = OPERATION_CMD.GENERATE_SET_LBT_PARAMS_CMD(include_timestamp, disable_auto_select);

            err_ocurr = true;
            ns.Write(data, 0, data.Length);
            int wait_result = WaitHandle.WaitAny(waithandles, CMD_RESPONSE_TIME_OUT, false);
            if (wait_result == 1) return CMD_RETURN.INVALID_COMMAND;
            else if (wait_result>1) return CMD_RETURN.CMD_RESPONSE_TIME_OUT;
            else
            {
                err_ocurr = (setLBTParamsRspEvent.data[0] == 1) ? true : false;
                return CMD_RETURN.COMMAND_SUCESS;
            }
        }
        
        /// <summary>
        /// Get listen before talk paramters
        /// </summary>
        /// <param name="lbt_time_mode"></param>        
        /// <param name="err_occur"></param>
        /// <returns></returns>
        public CMD_RETURN GetLBTParams(out int lbt_time_mode, out bool err_occur)
        {
            getLBTParamRspEvent = new SpeedwayManualResetEvent(false);
            invalidCommandNtfEvent = new SpeedwayManualResetEvent(false);
            WaitHandle[] waithandles = new WaitHandle[] { getLBTParamRspEvent.evt, invalidCommandNtfEvent.evt };
            byte[] data = OPERATION_CMD.GENERATE_GET_LBT_PARAMS_CMD(include_timestamp);

            err_occur = true;
            lbt_time_mode = 0;

            ns.Write(data, 0, data.Length);
            int wait_result = WaitHandle.WaitAny(waithandles, CMD_RESPONSE_TIME_OUT, false);
            if (wait_result == 1) return CMD_RETURN.INVALID_COMMAND;
            else if (wait_result>1) return CMD_RETURN.CMD_RESPONSE_TIME_OUT;
            else
            {
                err_occur = (getLBTParamRspEvent.data[0] == 1)?true:false;
                if(getLBTParamRspEvent.data.Length>2)
                {
                    lbt_time_mode = (int)getLBTParamRspEvent.data[1];
                }

                return CMD_RETURN.COMMAND_SUCESS;
            }
        }
        
        /// <summary>
        /// Stop modem
        /// </summary>
        /// <returns></returns>
        public CMD_RETURN ModemStop()
        {
            modemStopRspEvent = new SpeedwayManualResetEvent(false);
            invalidCommandNtfEvent = new SpeedwayManualResetEvent(false);
            WaitHandle[] waithandles = new WaitHandle[] { modemStopRspEvent.evt, invalidCommandNtfEvent.evt};
            byte[] data = OPERATION_CMD.GENERATE_MODEM_STOP_CMD(include_timestamp);

            ns.Write(data, 0, data.Length);
            int wait_result = WaitHandle.WaitAny(waithandles, MODEM_STOP_TIME_OUT, false);
            if (wait_result == 1) return CMD_RETURN.INVALID_COMMAND;
            else if (wait_result>1) return CMD_RETURN.CMD_RESPONSE_TIME_OUT;
            else
                return CMD_RETURN.COMMAND_SUCESS;
        }
        
        /// <summary>
        /// Set RF Survey parameters
        /// </summary>
        /// <param name="low_frequency_index"></param>
        /// <param name="high_frequency_index"></param>
        /// <param name="mb"></param>
        /// <param name="antenna_byte"></param>
        /// <param name="sample_count"></param>
        /// <param name="err_ocurr"></param>
        /// <returns></returns>
        public CMD_RETURN RFSurvey(UInt16 low_frequency_index, 
            UInt16 high_frequency_index,
            OPERATION_CMD.MEASUREMENT_BANDWIDTH mb,
            byte antenna_byte,
            UInt16 sample_count,
            out bool err_ocurr)
        {
            rfSurveyRspEvent = new SpeedwayManualResetEvent(false);
            invalidCommandNtfEvent = new SpeedwayManualResetEvent(false);
            WaitHandle[] waithandles = new WaitHandle[] { rfSurveyRspEvent.evt, invalidCommandNtfEvent.evt};

            byte[] data = OPERATION_CMD.GENERATE_RF_SURVEY_CMD(include_timestamp, low_frequency_index, high_frequency_index, mb, antenna_byte, sample_count);

            err_ocurr = true;

            ns.Write(data, 0, data.Length);

            int wait_result = WaitHandle.WaitAny(waithandles, CMD_RESPONSE_TIME_OUT, false);
            if (wait_result == 1) return CMD_RETURN.INVALID_COMMAND;
            else if (wait_result>1) return CMD_RETURN.CMD_RESPONSE_TIME_OUT;
            else
            {
                err_ocurr = (rfSurveyRspEvent.data[0] == 1) ? true : false;
                return CMD_RETURN.COMMAND_SUCESS;
            }
        }

        /// <summary>
        /// Read tag data
        /// </summary>
        /// <param name="mb">Memory bank</param>
        /// <param name="addr">Start address. in byte order</param>
        /// <param name="read_len">total words to be read. e.g. 96 bits EPC contains 6 words</param>
        /// <param name="password">access password</param>
        /// <param name="rsp"></param>
        /// <returns></returns>
        public CMD_RETURN TagRead(OPERATION_CMD.MEMORY_BANK mb, UInt16 addr, byte read_len, UInt32 password, out OPERATION_NTF.TAG_READ_NTF rsp)
        {
            tagReadRspEvent = new SpeedwayManualResetEvent(false);
            tagReadNtfEvent = new SpeedwayManualResetEvent(false);
            invalidCommandNtfEvent = new SpeedwayManualResetEvent(false);
            WaitHandle[] waithandles = new WaitHandle[] { tagReadRspEvent.evt, invalidCommandNtfEvent.evt };


            byte[] data = OPERATION_CMD.GENERATE_TAG_READ_CMD(include_timestamp, mb, addr, read_len, password);
            rsp = null;

            ns.Write(data, 0, data.Length);
            int wait_result = WaitHandle.WaitAny(waithandles, CMD_RESPONSE_TIME_OUT, false);
            if (wait_result == 1) return CMD_RETURN.INVALID_COMMAND;
            else if (wait_result>1) return CMD_RETURN.CMD_RESPONSE_TIME_OUT; 
            
            if (!tagReadNtfEvent.evt.WaitOne(NOTIFICTION_TIME_OUT, false)) return CMD_RETURN.CMD_RESPONSE_TIME_OUT;

            try
            {
                rsp = new OPERATION_NTF.TAG_READ_NTF(tagReadNtfEvent.data);
                return CMD_RETURN.COMMAND_SUCESS;
            }
            catch { return CMD_RETURN.RESPONSE_CAN_NOT_RECOGNIZED; }
        }

        /// <summary>
        /// Read tag data
        /// </summary>
        /// <param name="mb">Memory bank</param>
        /// <param name="addr">Start address. in byte order</param>
        /// <param name="read_len">total words to be read. e.g. 96 bits EPC contains 6 words</param>
        /// <param name="rsp"></param>
        /// <returns></returns>
        public CMD_RETURN TagRead(OPERATION_CMD.MEMORY_BANK mb, UInt16 addr, byte read_len, out OPERATION_NTF.TAG_READ_NTF rsp)
        {
            tagReadRspEvent = new SpeedwayManualResetEvent(false);
            tagReadNtfEvent = new SpeedwayManualResetEvent(false);

            invalidCommandNtfEvent = new SpeedwayManualResetEvent(false);
            WaitHandle[] waithandles = new WaitHandle[] { tagReadRspEvent.evt, invalidCommandNtfEvent.evt};

            byte[] data = OPERATION_CMD.GENERATE_TAG_READ_CMD(include_timestamp, mb, addr, read_len);
            rsp = null;

            ns.Write(data, 0, data.Length);
            int wait_result = WaitHandle.WaitAny(waithandles, CMD_RESPONSE_TIME_OUT, false);
            if (wait_result == 1) return CMD_RETURN.INVALID_COMMAND;
            else if (wait_result>1) return CMD_RETURN.CMD_RESPONSE_TIME_OUT;
            
            if (!tagReadNtfEvent.evt.WaitOne(NOTIFICTION_TIME_OUT, false)) return CMD_RETURN.CMD_RESPONSE_TIME_OUT;

            try
            {
                rsp = new OPERATION_NTF.TAG_READ_NTF(tagReadNtfEvent.data);
                return CMD_RETURN.COMMAND_SUCESS;
            }
            catch { return CMD_RETURN.RESPONSE_CAN_NOT_RECOGNIZED; }
        }
  
        /// <summary>
        /// Write data to tag
        /// </summary>
        /// <param name="mb">Memory bank</param>
        /// <param name="addr">Start addres. in byte order</param>
        /// <param name="data">Data to be wrote</param>
        /// <param name="disable_block_write"></param>
        /// <param name="password">access word</param>
        /// <param name="rsp"></param>
        /// <returns></returns>
        public CMD_RETURN TagWrite(OPERATION_CMD.MEMORY_BANK mb, UInt16 addr, UInt16[] data, bool disable_block_write, UInt32  password, out OPERATION_NTF.TAG_WRITE_NTF rsp)
        {
            tagWriteRspEvent = new SpeedwayManualResetEvent(false);
            tagWriteNtfEvent = new SpeedwayManualResetEvent(false);
            invalidCommandNtfEvent = new SpeedwayManualResetEvent(false);
            WaitHandle[] waithandles = new WaitHandle[] { tagWriteRspEvent.evt, invalidCommandNtfEvent.evt };

            byte[] temp = OPERATION_CMD.GENERATE_TAG_WRITE_CMD(include_timestamp, mb, addr, data, disable_block_write, password);

            rsp = null;

            ns.Write(temp, 0, temp.Length);
            int wait_result = WaitHandle.WaitAny(waithandles, CMD_RESPONSE_TIME_OUT, false);
            if (wait_result == 1) return CMD_RETURN.INVALID_COMMAND;
            else if (wait_result>1) return CMD_RETURN.CMD_RESPONSE_TIME_OUT; 
            
            if (!tagWriteNtfEvent.evt.WaitOne(NOTIFICTION_TIME_OUT, false)) return CMD_RETURN.CMD_RESPONSE_TIME_OUT;
            try
            {
                rsp = new OPERATION_NTF.TAG_WRITE_NTF(tagWriteNtfEvent.data);
                return CMD_RETURN.COMMAND_SUCESS;
            }
            catch { return CMD_RETURN.RESPONSE_CAN_NOT_RECOGNIZED; }

        }

        /// <summary>
        /// Write data to tag
        /// </summary>
        /// <param name="mb">Memory bank</param>
        /// <param name="addr">Start addres. in byte order</param>
        /// <param name="data">Data to be wrote</param>
        /// <param name="disable_block_write"></param>
        /// <param name="rsp"></param>
        /// <returns></returns>
        public CMD_RETURN TagWrite(OPERATION_CMD.MEMORY_BANK mb, UInt16 addr, UInt16[] data, bool disable_block_write, out OPERATION_NTF.TAG_WRITE_NTF rsp)
        {
            tagWriteRspEvent = new SpeedwayManualResetEvent(false);
            tagWriteNtfEvent = new SpeedwayManualResetEvent(false);
            invalidCommandNtfEvent = new SpeedwayManualResetEvent(false);
            WaitHandle[] waithandles = new WaitHandle[] { tagWriteRspEvent.evt, invalidCommandNtfEvent.evt};

            byte[] temp = OPERATION_CMD.GENERATE_TAG_WRITE_CMD(include_timestamp, mb, addr, data, disable_block_write);

            rsp = null;
            ns.Write(temp, 0, temp.Length);
            int wait_result = WaitHandle.WaitAny(waithandles, CMD_RESPONSE_TIME_OUT, false);
            if (wait_result == 1) return CMD_RETURN.INVALID_COMMAND;
            else if (wait_result>1) return CMD_RETURN.CMD_RESPONSE_TIME_OUT; 
            
            if (!tagWriteNtfEvent.evt.WaitOne(NOTIFICTION_TIME_OUT, false)) return CMD_RETURN.CMD_RESPONSE_TIME_OUT;
            try
            {
                rsp = new OPERATION_NTF.TAG_WRITE_NTF(tagWriteNtfEvent.data);
                return CMD_RETURN.COMMAND_SUCESS;
            }
            catch { return CMD_RETURN.RESPONSE_CAN_NOT_RECOGNIZED; }

        }

        /// <summary>
        /// Lock data in a tag
        /// </summary>
        /// <param name="tlo"></param>
        /// <param name="rsp"></param>
        /// <returns></returns>
        public CMD_RETURN TagLock(OPERATION_CMD.TAG_LOCK_OPERATION tlo, out OPERATION_NTF.TAG_ACCESS_RESULT_CODE rsp)
        {
            tagLockNtfEvent = new SpeedwayManualResetEvent(false);
            tagLockRspEvent = new SpeedwayManualResetEvent(false);
            invalidCommandNtfEvent = new SpeedwayManualResetEvent(false);
            WaitHandle[] waithandles = new WaitHandle[] { tagLockRspEvent.evt, invalidCommandNtfEvent.evt};

            byte[] data = OPERATION_CMD.GENERATE_TAG_LOCK_CMD(include_timestamp, tlo);
            
            rsp = OPERATION_NTF.TAG_ACCESS_RESULT_CODE.FAIL_OTHER_TAG_ERROR;
            ns.Write(data, 0, data.Length);
            int wait_result = WaitHandle.WaitAny(waithandles, CMD_RESPONSE_TIME_OUT, false);
            if (wait_result == 1) return CMD_RETURN.INVALID_COMMAND;
            else if (wait_result>1) return CMD_RETURN.CMD_RESPONSE_TIME_OUT; 
            
            if (!tagLockNtfEvent.evt.WaitOne(NOTIFICTION_TIME_OUT, false)) return CMD_RETURN.CMD_RESPONSE_TIME_OUT;
            else
            {
                rsp = (OPERATION_NTF.TAG_ACCESS_RESULT_CODE)tagLockNtfEvent.data[0];
                return CMD_RETURN.COMMAND_SUCESS;
            }
        }

        /// <summary>
        /// Lock data in a tag
        /// </summary>
        /// <param name="tlo"></param>
        /// <param name="password"></param>
        /// <param name="rsp"></param>
        /// <returns></returns>
        public CMD_RETURN TagLock(OPERATION_CMD.TAG_LOCK_OPERATION tlo, UInt32 password, out OPERATION_NTF.TAG_ACCESS_RESULT_CODE rsp)
        {
            tagLockNtfEvent = new SpeedwayManualResetEvent(false);
            tagLockRspEvent = new SpeedwayManualResetEvent(false);
            invalidCommandNtfEvent = new SpeedwayManualResetEvent(false);
            WaitHandle[] waithandles = new WaitHandle[] { tagLockRspEvent.evt, invalidCommandNtfEvent.evt};
           
            byte[] data = OPERATION_CMD.GENERATE_TAG_LOCK_CMD(include_timestamp, tlo, password);
            rsp = OPERATION_NTF.TAG_ACCESS_RESULT_CODE.FAIL_OTHER_TAG_ERROR;
            ns.Write(data, 0, data.Length);
            int wait_result = WaitHandle.WaitAny(waithandles, CMD_RESPONSE_TIME_OUT, false);
            if (wait_result == 1) return CMD_RETURN.INVALID_COMMAND;
            else if (wait_result>1) return CMD_RETURN.CMD_RESPONSE_TIME_OUT; 
            
            if (!tagLockNtfEvent.evt.WaitOne(NOTIFICTION_TIME_OUT, false)) return CMD_RETURN.CMD_RESPONSE_TIME_OUT;
            else
            {
                rsp = (OPERATION_NTF.TAG_ACCESS_RESULT_CODE)tagLockNtfEvent.data[0];
                return CMD_RETURN.COMMAND_SUCESS;
            }
        }

        /// <summary>
        /// Kill a tag
        /// </summary>
        /// <param name="password"></param>
        /// <param name="rsp"></param>
        /// <returns></returns>
        public CMD_RETURN TagKill(UInt32 password, out OPERATION_NTF.TAG_ACCESS_RESULT_CODE rsp)
        {
            tagKillNtfEvent = new SpeedwayManualResetEvent(false);
            tagKillRspEvent = new SpeedwayManualResetEvent(false);
            invalidCommandNtfEvent = new SpeedwayManualResetEvent(false);
            WaitHandle[] waithandles = new WaitHandle[] { tagKillRspEvent.evt, invalidCommandNtfEvent.evt};

            byte[] data = OPERATION_CMD.GENERATE_TAG_KILL_CMD(include_timestamp, password);
            rsp = OPERATION_NTF.TAG_ACCESS_RESULT_CODE.FAIL_OTHER_TAG_ERROR;

            ns.Write(data, 0, data.Length);
            int wait_result = WaitHandle.WaitAny(waithandles, CMD_RESPONSE_TIME_OUT, false);
            if (wait_result == 1) return CMD_RETURN.INVALID_COMMAND;
            else if (wait_result>1) return CMD_RETURN.CMD_RESPONSE_TIME_OUT; 
            
            if (!tagKillNtfEvent.evt.WaitOne(NOTIFICTION_TIME_OUT, false)) return CMD_RETURN.CMD_RESPONSE_TIME_OUT;
            else
            {
                rsp = (OPERATION_NTF.TAG_ACCESS_RESULT_CODE)tagKillNtfEvent.data[0];
                return CMD_RETURN.COMMAND_SUCESS;
            }
        }

        /// <summary>
        /// Set profile sequence list
        /// </summary>
        /// <param name="enabled">Enable sequence, default should be false</param>
        /// <param name="sequences">sequence list, the maximum sequence number is 16. the length of the sequence should be equal to the length of the duration list</param>
        /// <param name="durations">duration list</param>
        /// <returns></returns>
        public CMD_RETURN SetProfileSequence(bool enabled, ArrayList sequences, ArrayList durations, out bool err_occur)
        {
            err_occur = true;

            setProfileSequenceRspEvent = new SpeedwayManualResetEvent(false);
            invalidCommandNtfEvent = new SpeedwayManualResetEvent(false);
            WaitHandle[] waithandles = new WaitHandle[] { setProfileSequenceRspEvent.evt, invalidCommandNtfEvent.evt };

            byte[] data = OPERATION_CMD.GENERATE_SET_PROFILE_SEQUENCE_CMD(false, enabled, sequences, durations);

            ns.Write(data, 0, data.Length);
            int wait_result = WaitHandle.WaitAny(waithandles, CMD_RESPONSE_TIME_OUT, false);
            if (wait_result == 1) return CMD_RETURN.INVALID_COMMAND;
            else if (wait_result > 1) return CMD_RETURN.CMD_RESPONSE_TIME_OUT;
            else
            {
                err_occur = setProfileSequenceRspEvent.data[0] == 1 ? true : false;
                return CMD_RETURN.COMMAND_SUCESS;
            }
        }

        #endregion 


        #region Impinj

        public CMD_RETURN SetFixedChannel(short channel)
        {
            short[] data = new short[] { 0 };

            testWriteRspEvent = new SpeedwayManualResetEvent(false);
            invalidCommandNtfEvent = new SpeedwayManualResetEvent(false);
            testWriteNtfEvent = new SpeedwayManualResetEvent(false);
            WaitHandle[] waithandles = new WaitHandle[] { testWriteRspEvent.evt, invalidCommandNtfEvent.evt, testWriteNtfEvent.evt };

            byte[] ns_data = TEST_CMD_SET.GENERATE_WRITE_CMD_DATA(2, 0x00002001, data, include_timestamp);
            ns.Write(ns_data, 0, ns_data.Length);

            int wait_result = WaitHandle.WaitAny(waithandles, CMD_RESPONSE_TIME_OUT, false);
            if (wait_result == 1) return CMD_RETURN.INVALID_COMMAND;
            else if (wait_result > 1) return CMD_RETURN.CMD_RESPONSE_TIME_OUT;
            try
            {
            }
            catch
            {
                return CMD_RETURN.RESPONSE_CAN_NOT_RECOGNIZED;
            }


            if (!testWriteNtfEvent.evt.WaitOne(NOTIFICTION_TIME_OUT, false)) return CMD_RETURN.CMD_RESPONSE_TIME_OUT;


            testWriteRspEvent.evt.Reset();
            invalidCommandNtfEvent.evt.Reset();
            testWriteNtfEvent.evt.Reset();


            data = new short[] { channel };

            ns_data = TEST_CMD_SET.GENERATE_WRITE_CMD_DATA(2, 0x00002003, data, include_timestamp);
            ns.Write(ns_data, 0, ns_data.Length);

            wait_result = WaitHandle.WaitAny(waithandles, CMD_RESPONSE_TIME_OUT, false);
            if (wait_result == 1) return CMD_RETURN.INVALID_COMMAND;
            else if (wait_result > 1) return CMD_RETURN.CMD_RESPONSE_TIME_OUT;
            try
            {
            }
            catch
            {
                return CMD_RETURN.RESPONSE_CAN_NOT_RECOGNIZED;
            }


            if (!testWriteNtfEvent.evt.WaitOne(NOTIFICTION_TIME_OUT, false)) return CMD_RETURN.CMD_RESPONSE_TIME_OUT;


            return CMD_RETURN.COMMAND_SUCESS;
        }
        #endregion

    }
}
