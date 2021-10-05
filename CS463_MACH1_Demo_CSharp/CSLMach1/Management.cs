/******************************************************************************
**
** @file management.cs
**
** Last update: Jan 24, 2008
**
** This file provides MCS command set definition and implementation.
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

namespace CSL.Mach1
{
    /// <summary>
    /// MACH1 Management Command Set Class
    /// </summary>
    /// 
    [Serializable]
    public class MANAGEMENT_CMD
    {
        #region CONST
        public const byte GET_MCS_VERSION = 0x00;
        public const byte GET_READER_INFO = 0x01;
        public const byte GET_STATE = 0x02;
        public const byte BOOT_MODEM = 0x04;
        public const byte SHUTDOWN_MODEM = 0x05;
        public const byte SET_TEMPERATURE_ALART = 0x07;
        public const byte GET_TEMPERATURE_ALART = 0x08;
        public const byte SET_GPO = 0x09;
        public const byte SET_GPI = 0x0A;
        public const byte GET_GPI = 0x0B;
        public const byte SET_STATUS_REPORT = 0x0C;
        public const byte SET_TCP_CONNECTION_OPTIONS = 0x0D;
        public const byte GET_TCP_CONNECTION_OPTIONS = 0x0E;
        #endregion

        #region MANAGEMENT HELP CLASSES

        /// <summary>
        /// Temperature alarm modes
        /// </summary>
        public enum TERMPERETURE_ALARM_MODE
        {
            OFF = 0,
            PERIODIC,
            THRESHOLD,
            ONE_SHOT
        }

        public enum MESSAGE_FLUSH_BEHAVIOR
        {
            DEFAULT = 0,                //Do not flush inventory notification with halted flag and inventory status notification
            FLUSH_ALL_MSG = 1
        }

        /// <summary>
        /// GPI Status Class
        /// </summary>
        /// 
        [Serializable]
        public class GPI_STATUS
        {
            /// <summary>
            /// GPI status value
            /// </summary>
            public enum STATUS
            {
                LOW = 0,
                HIGH,
            }

            public byte id=0;
            public STATUS status = STATUS.LOW;

            public GPI_STATUS()
            {
                id = 0;
                status = STATUS.LOW;
            }
        }

        /// <summary>
        /// GPI Configuration Class
        /// </summary>
        /// 
        [Serializable]
        public class GPI_CONFIG
        {
            /// <summary>
            /// GPI configuration values
            /// </summary>
            public enum CONFIG
            {
                NO_NOTFICATION = 0,
                LO_TO_HIGH,
                HI_TO_LOW,
                BOTH
            }

            /// <summary>
            /// GPI Ids
            /// </summary>
            public enum GPI_ID
            {
                GPI0  = 1,
                GPI1,
                GPI2,
                GPI3
            }

            public GPI_ID id = GPI_ID.GPI0;
            public CONFIG config=CONFIG.NO_NOTFICATION;

            public GPI_CONFIG()
            {
                id = GPI_ID.GPI0;
                config = CONFIG.NO_NOTFICATION;
            }
        }

        /// <summary>
        /// GPO Configuration Class
        /// </summary>
        /// 
        [Serializable]
        public class GPO_CONFIG
        {
            /// <summary>
            /// Config values
            /// </summary>
            public enum CONFIG
            {
                LOW = 0,
                HIGH,
            }

            /// <summary>
            /// GPO ids
            /// </summary>
            public enum GPO_ID
            {
                GPO0 = 1,
                GPO1,
                GPO2,
                GPO3,
                GPO4,
                GPO5,
                GPO6,
                GPO7,
            }

            public GPO_ID id = GPO_ID.GPO0;
            public CONFIG config = CONFIG.LOW;

            public GPO_CONFIG()
            {
                id = GPO_ID.GPO0;
                config = CONFIG.LOW;
            }
            public GPO_CONFIG(GPO_ID id, CONFIG conf)
            {
                this.id = id;
                this.config = conf;
            }
        }




        #endregion

        #region GENERATE_PACKETS

        /// <summary>
        /// generate GetMcsVersionCmd command packet
        /// </summary>
        /// <param name="include_timestamp"></param>
        /// <returns></returns>
        public static byte[] GENERATE_GET_MCS_VERSION_CMD(bool include_timestamp)
        {
            MACH1_FRAME cmd = new MACH1_FRAME(CATEGORY.MODEM_MANAGEMENT, MANAGEMENT_CMD.GET_MCS_VERSION, include_timestamp);
            return cmd.PACKET;
        }

        /// <summary>
        /// Generate GetReaderInfoCmd commmand packet
        /// </summary>
        /// <param name="include_timestamp"></param>
        /// <returns></returns>
        public static byte[] GENERATE_GET_READER_INFO_CMD(bool include_timestamp)
        {
            MACH1_FRAME cmd = new MACH1_FRAME(CATEGORY.MODEM_MANAGEMENT, MANAGEMENT_CMD.GET_READER_INFO, include_timestamp);
            return cmd.PACKET;
        }

        /// <summary>
        /// Generate GetStateCmd command package
        /// </summary>
        /// <param name="include_timestamp"></param>
        /// <returns></returns>
        public static byte[] GENERATE_GET_STATE_CMD(bool include_timestamp)
        {
            MACH1_FRAME cmd = new MACH1_FRAME(CATEGORY.MODEM_MANAGEMENT, MANAGEMENT_CMD.GET_STATE, include_timestamp);
            return cmd.PACKET;
        }

        /// <summary>
        /// Generate BootModemCmd command package
        /// </summary>
        /// <param name="include_timestamp"></param>
        /// <returns></returns>
        public static byte[] GENERATE_BOOT_MODEM_CMD(bool include_timestamp)
        {
            MACH1_FRAME cmd = new MACH1_FRAME(CATEGORY.MODEM_MANAGEMENT, MANAGEMENT_CMD.BOOT_MODEM, include_timestamp);
            return cmd.PACKET;
        }

        /// <summary>
        /// Generate ShutDownModemCmd command package
        /// </summary>
        /// <param name="include_timestamp"></param>
        /// <returns></returns>
        public static byte[] GENERATE_SHUTDOWN_MODEM_CMD(bool include_timestamp)
        {
            MACH1_FRAME cmd = new MACH1_FRAME(CATEGORY.MODEM_MANAGEMENT, MANAGEMENT_CMD.SHUTDOWN_MODEM, include_timestamp);
            return cmd.PACKET;
        }

        /// <summary>
        /// Generate SetTemperatureAlarmCmd command package
        /// </summary>
        /// <param name="include_timestamp"></param>
        /// <param name="mode"></param>
        /// <param name="periodicTemperetureRate"></param>
        /// <param name="alertThreshold"></param>
        /// <returns></returns>
        public static byte[] GENERATE_SET_TEMPERETURE_ALARM_CMD(bool include_timestamp, TERMPERETURE_ALARM_MODE mode, UInt16 periodicTemperetureRate, Int16 alertThreshold)
        {
            byte[] data = new byte[1];

            switch (mode)
            {
                case TERMPERETURE_ALARM_MODE.OFF:
                case TERMPERETURE_ALARM_MODE.ONE_SHOT:
                    data = new byte[1];
                    data[0] = (byte)mode;
                    break;
                case TERMPERETURE_ALARM_MODE.PERIODIC:
                    data = new byte[3];
                    data[0] = (byte)mode;
                    data[1] = 0x01;
                    data[2] = (byte)periodicTemperetureRate;
                    break;
                case TERMPERETURE_ALARM_MODE.THRESHOLD:
                    data = new byte[3];
                    data[0] = (byte)mode;
                    data[1] = 0x02;
                    data[2] = (byte)alertThreshold;
                    break;
            }

            MACH1_FRAME cmd = new MACH1_FRAME(CATEGORY.MODEM_MANAGEMENT, MANAGEMENT_CMD.SET_TEMPERATURE_ALART, include_timestamp, data);
            return cmd.PACKET;
        }

        /// <summary>
        /// Generate GetTemperatureAlarmCmd command set
        /// </summary>
        /// <param name="include_timestamp"></param>
        /// <returns></returns>
        public static byte[] GENERATE_GET_TEMPERETURE_ALARM_CMD(bool include_timestamp)
        {
            MACH1_FRAME cmd = new MACH1_FRAME(CATEGORY.MODEM_MANAGEMENT, MANAGEMENT_CMD.GET_TEMPERATURE_ALART, include_timestamp);
            return cmd.PACKET;
        }

        /// <summary>
        /// Generate SetGPOCmd command package
        /// </summary>
        /// <param name="include_timestamp"></param>
        /// <param name="gpo"></param>
        /// <returns></returns>
        public static byte[] GENERATE_SET_GPO_CMD(bool include_timestamp, GPO_CONFIG[] gpo)
        {
            int len = gpo.Length;
            byte[] data = new byte[len * 2];

            for (int i = 0; i < len; i++)
            {
                data[2 * i] = (byte)(gpo[i].id);
                data[2 * i + 1] = (byte)(gpo[i].config);
            }
            MACH1_FRAME cmd = new MACH1_FRAME(CATEGORY.MODEM_MANAGEMENT, MANAGEMENT_CMD.SET_GPO, include_timestamp, data);
            return cmd.PACKET;
        }

        /// <summary>
        /// Generate SetGPICmd command package
        /// </summary>
        /// <param name="include_timestamp"></param>
        /// <param name="gpi"></param>
        /// <returns></returns>
        public static byte[] GENERATE_SET_GPI_CMD(bool include_timestamp, GPI_CONFIG[] gpi)
        {
            int len = gpi.Length;
            byte[] data = new byte[len * 2];

            for (int i = 0; i < len; i++)
            {
                data[2 * i] = (byte)(gpi[i].id);
                data[2 * i + 1] = (byte)(gpi[i].config);
            }

            MACH1_FRAME cmd = new MACH1_FRAME(CATEGORY.MODEM_MANAGEMENT, MANAGEMENT_CMD.SET_GPI, include_timestamp, data);
            return cmd.PACKET;
        }

        /// <summary>
        /// Generate GetGpiCmd command package
        /// </summary>
        /// <param name="include_timestamp"></param>
        /// <returns></returns>
        public static byte[] GENERATE_GET_GPI_CMD(bool include_timestamp)
        {
            MACH1_FRAME cmd = new MACH1_FRAME(CATEGORY.MODEM_MANAGEMENT, MANAGEMENT_CMD.GET_GPI, include_timestamp);
            return cmd.PACKET;
        }

        /// <summary>
        /// Generate SetStatusReportCmd command package
        /// </summary>
        /// <param name="include_timestamp"></param>
        /// <param name="enable_reporting"></param>
        /// <returns></returns>
        public static byte[] GENERATE_SET_STATUS_REPORT_CMD(bool include_timestamp, bool enable_reporting)
        {
            byte[] data = new byte[2];
            data[0] = 0x01;
            data[1] = (byte)(enable_reporting ? 1 : 0);

            MACH1_FRAME cmd = new MACH1_FRAME(CATEGORY.MODEM_MANAGEMENT, MANAGEMENT_CMD.SET_STATUS_REPORT, include_timestamp, data);
            return cmd.PACKET;
        }

        /// <summary>
        /// Generate SetTcpConnectionOptionsCmd message
        /// </summary>
        /// <param name="include_timestamp"></param>
        /// <param name="behavior"></param>
        /// <returns></returns>
        public static byte[] GENERATE_SET_TCP_CONNECTION_OPTIONS_CMD(bool include_timestamp, MESSAGE_FLUSH_BEHAVIOR behavior)
        {
            byte[] data = new byte[2];
            data[0] = 0x01;
            data[1] = (byte)behavior;

            MACH1_FRAME cmd = new MACH1_FRAME(CATEGORY.MODEM_MANAGEMENT, MANAGEMENT_CMD.SET_TCP_CONNECTION_OPTIONS, include_timestamp, data);
            return cmd.PACKET;
        }

        /// <summary>
        /// Generate GetTcpConnectionOptionsCmd message
        /// </summary>
        /// <param name="include_timestamp"></param>
        /// <param name="report_behavior"></param>
        /// <returns></returns>
        public static byte[] GENERATE_GET_TCP_CONNECTION_OPTIONS_CMD(bool include_timestamp, bool report_behavior)
        {
            byte[] data = new byte[2];
            data[0] = 0x01;
            data[1] = (byte)(report_behavior?1:0);

            MACH1_FRAME cmd = new MACH1_FRAME(CATEGORY.MODEM_MANAGEMENT, MANAGEMENT_CMD.GET_TCP_CONNECTION_OPTIONS, include_timestamp, data);
            return cmd.PACKET;
        }

        #endregion

        #region PARSE_RESPONSE
        /// <summary>
        /// Mach1 Software Version Class
        /// </summary>
        public class MCS_VERSION_RSP
        {
            public string version = string.Empty;

            public MCS_VERSION_RSP(byte[] data)
            {
                try
                {
                    version = string.Format("v.{0}.{1}.{2}", data[0], data[1], data[2]);
                }
                catch { }
            }

        }

        /// <summary>
        /// Reader information class
        /// </summary>
        public class READER_INFO_RSP
        {
            public string software_version;
            public string firmware_verison;
            public string FPGA_version;
            public UInt32 uptime;

            public READER_INFO_RSP(byte[] data)
            {
                try
                {
                    software_version = string.Format("v.{0}.{1}.{2}.{3}", data[0], data[1], data[2], data[3]);
                    firmware_verison = string.Format("v.{0}.{1}.{2}.{3}", data[4], data[5], data[6], data[7]);
                    FPGA_version = string.Format("v.{0}.{1}.{2}.{3}", data[8], data[9], data[10], data[11]);

                    UInt32 d1 = data[12];
                    UInt32 d2 = data[13];
                    UInt32 d3 = data[14];
                    UInt32 d4 = data[15];

                    uptime = (d1 << 24) + (d2 << 16) + (d3 << 8) + d4;
                }
                catch { };
            }
        }

        /// <summary>
        /// State Response Class
        /// </summary>
        public class STATE_RSP
        {
            public MODEM_STATE state;

            public STATE_RSP(byte[] data)
            {
                try
                {
                    state = (MODEM_STATE)data[0];
                }
                catch { }
            }
        }

        /// <summary>
        /// Temperature Alarm Class
        /// </summary>
        public class TEMPERETURE_ALARM_RSP
        {
            public Int16 temperature;
            public TERMPERETURE_ALARM_MODE mode;
            public Int16 periodic_tempreture_rate;
            public Int16 alarm_threshold;

            public TEMPERETURE_ALARM_RSP(byte[] data)
            {
                try
                {
                    temperature = data[0];
                    mode = (TERMPERETURE_ALARM_MODE)data[1];

                    switch (data[2])
                    {
                        case 0x01:
                            periodic_tempreture_rate = data[3];
                            if (data.Length >= 6 && data[4] == 0x01)
                                alarm_threshold = data[5];
                            break;
                        case 0x02:
                            alarm_threshold = data[3];
                            if (data.Length >= 6 && data[4] == 0x02)
                                periodic_tempreture_rate = data[5];
                            break;
                    }
                }
                catch { }
            }
        }

        /// <summary>
        /// SetGpoCmd Response
        /// </summary>
        public class SET_GPO_RSP
        {
            /// <summary>
            /// Result Code
            /// </summary>
            public enum RESULT_CODE
            {
                GPO_SET_SUCCESSFUL = 0,
                ONE_OR_MORE_GPOS_SPECIFIED_NOT_SUPPORTED
            }

            public RESULT_CODE result_code;
            public SET_GPO_RSP(byte[] data)
            {
                try
                {
                    result_code = (RESULT_CODE)data[0];
                }
                catch { }
            }
        }

        /// <summary>
        /// SetGpI Response Class
        /// </summary>
        public class SET_GPI_RSP
        {
            public enum RESULT_CODE
            {
                GPI_SET_SUCCESSFUL = 0,
                ONE_OR_MORE_GPIS_SPECIFIED_NOT_SUPPORTED,
                ATTEMPT_TO_CONFIGURE_ACTIVE_TRIGGER
            }

            public RESULT_CODE result_code;

            public SET_GPI_RSP(byte[] data)
            {
                try
                {
                    result_code = (RESULT_CODE)data[0];
                }
                catch { }
            }
        }

        /// <summary>
        /// GetGpi Response Class
        /// </summary>
        public class GET_GPI_RSP
        {
            public GPI_STATUS[] gpi_status; 
            public GPI_CONFIG[] gpi_config; 

            public GET_GPI_RSP(byte[] data)
            {
                gpi_status = new GPI_STATUS[4];
                gpi_config = new GPI_CONFIG[4];

                try
                {
                    for (int i = 0; i < 4; i++)
                    {
                        gpi_config[i] = new GPI_CONFIG();
                        gpi_config[i].id = (GPI_CONFIG.GPI_ID)(i+1);
                        gpi_config[i].config = (GPI_CONFIG.CONFIG)data[i];

                        gpi_status[i] = new GPI_STATUS();
                        gpi_status[i].id = (byte)(i+1);
                        gpi_status[i].status = (GPI_STATUS.STATUS)data[i + 4];
                    }
                }
                catch { }
            }
        }

        #endregion
    }

    /// <summary>
    /// Mach1 Management Notification Set
    /// </summary>
    /// 
    [Serializable]
    public class MANAGEMENT_NTF 
    {
        #region CONST
        public const byte SOCKET_CONNECTION_STATUS = 0x00;
        public const byte SYSTEM_ERROR = 0x01;
        public const byte BOOT_MODEM = 0x02;
        public const byte TEMPERATURE_ALART = 0x04;
        public const byte GPI_ALERT = 0x05;
        public const byte STATUS_REPORT = 0x06;
        #endregion

        /// <summary>
        /// Socket Connection Status Notification Class
        /// </summary>
        [Serializable]
        public class SOCKET_CONNECTION_STATUS_NTF : MACH1_NTF
        {
            public SOCKET_STATUS socket_status = SOCKET_STATUS.CONNECTION_FAILED;
            public string ip;

            public SOCKET_CONNECTION_STATUS_NTF(byte[] data)
            {
                socket_status = (SOCKET_STATUS)data[0];
                if (socket_status == SOCKET_STATUS.CONNECTION_FAILED && data[1] == 0x01) { ip = string.Format("{0}.{1}.{2}.{3}", data[2], data[3], data[4], data[5]); }
            }
        }

        /// <summary>
        /// System Error Notification Class
        /// </summary>
        /// 
        [Serializable]
        public class SYSTEM_ERROR_NTF : MACH1_NTF
        {
            public SYSTEM_ERROR_REASON err_reason;
            public MODEM_STATE current_state;
            public UInt32 err_code;

            public SYSTEM_ERROR_NTF(byte[] data)
            {
                err_reason = (SYSTEM_ERROR_REASON)data[0];
                current_state = (MODEM_STATE)data[1];

                if (data.Length >= 7 && data[2] == 0x01)
                {
                    UInt32 d1 = data[3];
                    UInt32 d2 = data[4];
                    UInt32 d3 = data[5];
                    UInt32 d4 = data[6];

                    err_code = (d1 << 24) + (d2 << 16) + (d3 << 8) + d4;
                }
            }
        }

        /// <summary>
        /// Temperature alarm notification class
        /// </summary>
        /// 
        [Serializable]
        public class TEMPERETURE_ALARM_NTF : MACH1_NTF
        {
            public enum REASON
            {
                PERIODIC_REPORTING = 0,
                ALARM_SHRESHOLD_REACHED,
                ONE_SHOT_REPORT
            }

            public REASON reason;
            public int temperature;

            public TEMPERETURE_ALARM_NTF(byte[] data)
            {
                try
                {
                    reason = (REASON)data[0];
                    temperature = data[1];
                }
                catch { };
            }
        }

        /// <summary>
        /// GPI Alert Notification Class
        /// </summary>
        /// 
        [Serializable]
        public class GPI_ALERT_NTF : MACH1_NTF
        {
            public enum GPI_TRIGGER
            {
                GPI_0_TRIGGERED = 0,
                GPI_1_TRIGGERED,
                GPI_2_TRIGGERED,
                GPI_3_TRIGGERED
            }

            public enum GPI_STATUS
            {
                LOW = 0,
                HIGH = 1
            }

            public GPI_TRIGGER gpi;
            public GPI_STATUS status;

            public GPI_ALERT_NTF(byte[] data)
            {
                gpi = (GPI_TRIGGER)data[0];

                try
                {
                    if (data[1] == 0x01) status = (GPI_STATUS)data[2];
                }
                catch { }
            }
        }

        /// <summary>
        /// BootModem Notification Class
        /// </summary>
        /// 
        [Serializable]
        public class BOOT_MODEM_NTF : MACH1_NTF
        {
            public enum BOOT_RESULT_CODE
            {
                BOOT_SUCESSFUL = 0,
                BOOT_IN_PROGRESS,
                BOOT_FAIL_DUE_TO_INVALID_FIRMWARE,
                BOOT_FAIL_DUE_TO_UNKNOWN_HARDWARE
            }

            public BOOT_RESULT_CODE boot_result_code;
            public UInt16 percent_done;

            public BOOT_MODEM_NTF(byte[] data)
            {
                boot_result_code = (BOOT_RESULT_CODE)data[0];
                try
                {
                    if(data.Length>1 && data[1] == 0x01) percent_done = data[2];
                }
                catch { }
            }
        }

        /// <summary>
        /// StatusReport notification class
        /// </summary>
        /// 
        [Serializable]
        public class STATUS_REPORT_NTF : MACH1_NTF
        {
            public UInt32 modem_overflow_ntf_loss = 0;

            public STATUS_REPORT_NTF(byte[] data)
            {
                if (data[0] == 0x01)
                {
                    UInt32 d1 = data[1];
                    UInt32 d2 = data[2];
                    UInt32 d3 = data[3];
                    UInt32 d4 = data[4];

                    modem_overflow_ntf_loss = (d1 << 24) + (d2 << 16) + (d3 << 8) + d4;
                }
            }
        }
    }

}
