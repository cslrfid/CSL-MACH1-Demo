
/**
 ******************************************************************************
 **
 ** @file common.cs
 **
 ** Last update: Jan 24, 2008
 **
 ** This file provides the Mach1 Frame classes.
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
    /// General definition
    /// </summary>
    /// 
    [Serializable]
    public class GENERAL_DEFINITION
    {
        public const bool RESERVED = false;    

        public const byte SOF_CMD = 0xEE;       //Start of Frame, Command
        public const byte SOF_RSP = 0xEF;       //Start of Frame, Response
    }

    /// <summary>
    /// Regulatory regions
    /// </summary>
    public enum REGULATORY_REGION
    {
        FCC_PART_15_247 = 0,
        ESTI_EN_300_220,
        ETSI_EN_302_208_WLBT,
        HK_920_925,
        TAIWAN_922_928,
        JAPAN_952_954,
        JAPAN_952_955,
        ETSI_EN_302_208_OLBT,
        KOREA_910_914,
        MALAYSIA_919_923,
        CHINA_920_925

    }

    /// <summary>
    /// Modem state
    /// </summary>
    public enum MODEM_STATE
    {
        OFF = 0,
        NA,                     //fixed bug, reported by Ron. 12/10/07
        INIT,
        IDLE,
        ACTIVE,
        HALT,
        TEST,
        ACCESS
    }

    /// <summary>
    /// System errors
    /// </summary>
    public enum SYSTEM_ERROR_REASON
    {
        MODEM_WATCHDOG_EXPIRED = 0,
        MACH1_FUP_TIMEOUT,
        OTHER_SYSTEM_ERR
    }

    /// <summary>
    /// Socket connection status
    /// </summary>
    public enum SOCKET_STATUS
    {
        CONNECTION_SUCCESS = 0,
        CONNECTION_FAILED
    }

    /// <summary>
    /// Commmand Category
    /// </summary>
    public enum CATEGORY
    {
        MACH1_PROTOCOL_ERROR = 0,
        OPERATION_MODEL,
        MODEM_MANAGEMENT,
        HARDWARE_INTERFACE,
        PRODUCTION,
        LOGGING,
        TEST
    }


    /// <summary>
    /// Invalid command reason codes
    /// </summary>
    public enum INVALID_CMD_REASON_CODE
    {
        INVALID_CATEGORY = 0,
        INVALID_CRC,
        INVALID_MID,
        INVALID_OTHER_HEADER,
        INVALID_MODEM_STATE,
        PARAMS_OUT_RANGE,
        MISSING_CMD_PARAMS,
        INVALID_OPT_PARAM_ID,
        OTHER_PARAM_ERR,
        OUT_OF_SYNC,
        CMD_IN_PROGRESS
    }

    /// <summary>
    /// Mach1 Notification Class
    /// </summary>
    /// 
    [Serializable]
    public abstract class MACH1_NTF
    {
        public UInt32 timestamp_second;     //Timestamp: total seconds since epoch (12:00:00AM, 1/1/1970)
        public UInt32 timestamp_us;         //Timestamp: micro seconds

        public string reader_name;          //reader name
    }

    /// <summary>
    /// Mach1 Frame
    /// </summary>
    /// 
    [Serializable]
    public class MACH1_FRAME
    {

        /// <summary>
        /// Mach Header Abstract Class
        /// </summary>
        /// 
        [Serializable]
        public abstract class HEADER
        {
            public const bool bit_15 = GENERAL_DEFINITION.RESERVED;
            public bool timestamp_included = false;
            public bool is_ntf = false;
            public bool bit_12 = GENERAL_DEFINITION.RESERVED;

            public CATEGORY category;
            public byte message_id;

            public byte[] ToByteArray()
            {
                int h_byte = 0;

                int ts = timestamp_included ? 0x01 : 0x00;
                int rn = is_ntf ? 0x01 : 0x00;

                ts = ts << 6;
                rn = rn << 5;

                int ct = (int)category;
                h_byte = ts | rn | ct;

                byte[] data = new byte[2];
                data[0] = (byte)h_byte; data[1] = message_id;

                return data;
            }
        }

        /// <summary>
        /// Mach1 Header Reader -> Host
        /// </summary>
        /// 
        [Serializable]
        public class HEADER_READER_HOST : HEADER
        {

            /// <summary>
            /// Constructor for generate header based on byte array
            /// </summary>
            /// <param name="data">Header data</param>
            public HEADER_READER_HOST(byte[] data)
            {
                if (data.Length != 2) throw new Exception("The input is not a valid Header");

                timestamp_included = (data[0] & 0x40) == 0 ? false : true;
                is_ntf = (data[0] & 0x20) == 0 ? false : true;
                category = (CATEGORY)(data[0] & 0x0F);
                message_id = data[1];
            }
        }

        /// <summary>
        /// Mach1 Header Host -> Reader
        /// </summary>
        /// 
        [Serializable]
        public class HEADER_HOST_READER : HEADER
        {
            /// <summary>
            /// Constructor for generate header based on Catogory and Message ID
            /// </summary>
            /// <param name="include_timestamp"></param>
            /// <param name="category">Command category, defined in enum CATEGORY</param>
            /// <param name="message_id">Message id</param>
            public HEADER_HOST_READER(bool include_timestamp, CATEGORY category, byte message_id)
            {
                this.timestamp_included = include_timestamp;
                this.category = category;
                this.message_id = message_id;
                this.is_ntf = false;
            }

        }

        public HEADER header;
        public UInt16 packet_len = 6;   //minimum packet length
        public UInt16 payload_len = 0; 

        public UInt32 timestamp_second;
        public UInt32 timestamp_us;

        private byte[] packet;
        private byte[] payload;

        /// <summary>
        /// For more information, refer to Speedway Reference Design - Mach1 Overview
        /// </summary>
        /// <param name="category"></param>
        /// <param name="cmd"></param>
        /// <param name="include_timestamp"></param>
        private void CreateFrameWithoutPayload(CATEGORY category, byte cmd, bool include_timestamp)
        {
            try
            {
                header = new HEADER_HOST_READER(include_timestamp, category, cmd);

                long dt = DateTime.Now.ToUniversalTime().ToFileTimeUtc();
                timestamp_second = (UInt32)(dt >> 32);
                timestamp_us = (UInt32)(dt & ((Int64)(0x00000000FFFFFFFF)));

                if (include_timestamp) packet_len += 8;
                packet = new byte[packet_len];

                packet[0] = GENERAL_DEFINITION.SOF_CMD;
                Array.Copy(header.ToByteArray(), 0, packet, 1, 2);

                packet[4] = 0x00;
                packet[3] = 0x00;

                if (include_timestamp)
                {
                    packet[5] = (byte)(timestamp_second >> 24);
                    packet[6] = (byte)((timestamp_second & 0x00FF0000) >> 16);
                    packet[7] = (byte)((timestamp_second & 0x0000FF00) >> 8);
                    packet[8] = (byte)((timestamp_second & 0x000000FF));


                    packet[9] = (byte)(timestamp_us >> 24);
                    packet[10] = (byte)((timestamp_us & 0x00FF0000) >> 16);
                    packet[11] = (byte)((timestamp_us & 0x0000FF00) >> 8);
                    packet[12] = (byte)((timestamp_us & 0x000000FF));

                }
                else { }

                packet[packet_len - 1] = CRC.CalculateCRC(packet, packet_len - 1);
            }
            catch { };
        }

        /// <summary>
        /// For more information, refer to Speedway Reference Design - Mach1 Overview
        /// </summary>
        /// <param name="category"></param>
        /// <param name="cmd"></param>
        /// <param name="include_timestamp"></param>
        /// <param name="data"></param>
        private void CreateFrameWithPayload(CATEGORY category, byte cmd, bool include_timestamp, byte[] data)
        {
            try
            {
                header = new HEADER_HOST_READER(include_timestamp, category, cmd);

                payload = new byte[data.Length];
                Array.Copy(data, payload, data.Length);

                long dt = DateTime.Now.ToUniversalTime().ToFileTimeUtc();
                timestamp_second = (UInt32)(dt >> 32);
                timestamp_us = (UInt32)(dt & ((Int64)(0x00000000FFFFFFFF)));

                if (include_timestamp) packet_len += 8;

                payload_len = (UInt16)data.Length;
                packet_len += payload_len;

                packet = new byte[packet_len];

                packet[0] = GENERAL_DEFINITION.SOF_CMD;
                Array.Copy(header.ToByteArray(), 0, packet, 1, 2);

                packet[4] = (byte)(payload_len & 0x00FF);
                packet[3] = (byte)(payload_len >> 8);

                if (include_timestamp)
                {
                    packet[5] = (byte)(timestamp_second >> 24);
                    packet[6] = (byte)((timestamp_second & 0x0000FFFF00000000) >> 16);
                    packet[7] = (byte)((timestamp_second & 0x00000000FFFF0000) >> 8);
                    packet[8] = (byte)((timestamp_second & 0x000000000000FFFF));


                    packet[9] = (byte)(timestamp_us >> 24);
                    packet[10] = (byte)((timestamp_us & 0x0000FFFF00000000) >> 16);
                    packet[11] = (byte)((timestamp_us & 0x00000000FFFF0000) >> 8);
                    packet[12] = (byte)((timestamp_us & 0x000000000000FFFF));

                    Array.Copy(data, 0, packet, 13, data.Length);
                }
                else { Array.Copy(data, 0, packet, 5, data.Length); }

                packet[packet_len - 1] = CRC.CalculateCRC(packet, packet_len - 1);
            }
            catch { };
        }

        public MACH1_FRAME() { }

        /// <summary>
        /// Constructor for building command without command data 
        /// </summary>
        /// <param name="category"></param>
        /// <param name="cmd"></param>
        /// <param name="include_timestamp"></param>
        public MACH1_FRAME(CATEGORY category, byte cmd, bool include_timestamp)
        {
            CreateFrameWithoutPayload(category, cmd, include_timestamp);
        }

        /// <summary>
        /// Constructor for building command with command data
        /// </summary>
        /// <param name="category"></param>
        /// <param name="cmd"></param>
        /// <param name="include_timestamp"></param>
        /// <param name="data"></param>
        public MACH1_FRAME(CATEGORY category, byte cmd, bool include_timestamp, byte[] data)
        {
            if(data!=null)
                CreateFrameWithPayload(category, cmd, include_timestamp, data);
            else
                CreateFrameWithoutPayload(category, cmd, include_timestamp);
        }

        /// <summary>
        /// Constructure for parsing received packet 
        /// </summary>
        /// <param name="data"></param>
        public MACH1_FRAME(byte[] data)
        {
            if (data[0] != GENERAL_DEFINITION.SOF_CMD && data[0] != GENERAL_DEFINITION.SOF_RSP)
                throw new Exception("Input is not a valid Mach1 packet!");

            try
            {
                //validate the input data
                byte[] h_d = new byte[2];
                Array.Copy(data, 1, h_d, 0, 2);
                header = new HEADER_READER_HOST(h_d);

                payload_len = (UInt16)((data[3] << 8) + data[4]);
                if (header.timestamp_included) packet_len += 8;

                packet_len += payload_len;

                packet = new byte[packet_len];
                Array.Copy(data, packet, packet_len);

                //validate the packet
                byte crc = CRC.CalculateCRC(packet, packet.Length - 1);
                if (crc != packet[packet.Length - 1]) throw new Exception("Validate CRC failed!");

                //Calculated Timestamp
                if (header.timestamp_included)
                {
                    uint d1 = packet[5];
                    uint d2 = packet[6];
                    uint d3 = packet[7];
                    uint d4 = packet[8];

                    timestamp_second = (d1 << 24) + (d2 << 16) + (d3 << 8) + d4;

                    d1 = packet[9];
                    d2 = packet[10];
                    d3 = packet[11];
                    d4 = packet[12];

                    timestamp_us = (d1 << 24) + (d2 << 16) + (d3 << 8) + d4;

                    //Extract mach1 payload
                    if (payload_len > 0)
                    {
                        payload = new byte[payload_len];
                        Array.Copy(packet, 13, payload, 0, payload_len);
                    }
                }
                else
                {
                    if (payload_len > 0)
                    {
                        payload = new byte[payload_len];
                        Array.Copy(packet, 5, payload, 0, payload_len);
                    }
                }
            }
            catch
            {
                throw new Exception("Input is not a valid Mach1 packet!");
            }
        }

        /// <summary>
        /// Parse Mach1 Messages
        /// </summary>
        /// <param name="data">Mach packet data (byte array)</param>
        /// <param name="reserved_data">Used to store fregmented Mach1 message data</param>
        /// <returns></returns>
        public static MACH1_FRAME ParseMachData(byte[] data, out byte[] reserved_data)
        {
            MACH1_FRAME mf = new MACH1_FRAME();

            if (data[0] != GENERAL_DEFINITION.SOF_CMD && data[0] != GENERAL_DEFINITION.SOF_RSP)
                throw new Exception("Input is not a valid Mach1 packet!");
                
            reserved_data = null;

            try
            {
                //validate the input data. the minimum length of a mach1 packet is 5
                if (data.Length < 5)
                {
                    reserved_data = new byte[data.Length];
                    Array.Copy(data, reserved_data, data.Length);
                    return null;
                }
                byte[] h_d = new byte[2];

                Array.Copy(data, 1, h_d, 0, 2);
                mf.header = new HEADER_READER_HOST(h_d);

                mf.payload_len = (UInt16)(((data[3]&0x03) << 8) + data[4]);
                if (mf.header.timestamp_included) mf.packet_len += 8;

                mf.packet_len += mf.payload_len;

                //if the length of the data is less than the packet len contained in the header, move the data to reserved data
                if (data.Length < mf.packet_len)
                {
                    reserved_data = new byte[data.Length];
                    Array.Copy(data, reserved_data, data.Length);
                    return null;
                }

                mf.packet = new byte[mf.packet_len];
                Array.Copy(data, mf.packet, mf.packet_len);

                //validate the packet
                byte crc = CRC.CalculateCRC(mf.packet, mf.packet.Length - 1);
                if (crc != mf.packet[mf.packet.Length - 1]) return null;

                //Extract timestamp
                if (mf.header.timestamp_included)
                {
                    uint d1 = data[5];
                    uint d2 = data[6];
                    uint d3 = data[7];
                    uint d4 = data[8];

                    mf.timestamp_second = (d1 << 24) + (d2 << 16) + (d3 << 8) + d4;

                    d1 = data[9];
                    d2 = data[10];
                    d3 = data[11];
                    d4 = data[12];

                    mf.timestamp_us = (d1 << 24) + (d2 << 16) + (d3 << 8) + d4;

                    if (mf.payload_len > 0)
                    {
                        mf.payload = new byte[mf.payload_len];
                        Array.Copy(data, 13, mf.payload, 0, mf.payload_len);
                    }
                }
                else
                {
                    if (mf.payload_len > 0)
                    {
                        mf.payload = new byte[mf.payload_len];
                        Array.Copy(data, 5, mf.payload, 0, mf.payload_len);
                    }
                }

                return mf;

            }
            catch
            {
                throw new Exception("Input is not a valid Mach1 packet!");
            }
        }

        /// <summary>
        /// get command packet
        /// </summary>
        public byte[] PACKET
        {
            get
            {
                if (packet == null) return null;
                else
                    return packet;
            }
        }

        /// <summary>
        /// get payload (command response data)
        /// </summary>
        public byte[] PAYLOAD
        {
            get
            {
                if (payload != null) return payload;
                else
                    return null;
            }
        }

    }

    #region Header


    

    /// <summary>
    /// Invalide command notification class
    /// </summary>
    /// 
    [Serializable]
    public class InvalidCommandNtf
    {
        /// <summary>
        /// Error reason code
        /// </summary>
        public enum REASON_CODE
        {
            INVALID_CATEGORY = 0,
            INVALID_CRC,
            INVALID_MID,
            OTHER_INVALID_HEADER,
            INVALID_MODEM_STATE,
            PARAM_OUT_OF_RANGE,
            MISSING_COMMAND_PARAM,
            INVALID_OPTIONAL_PARAM,
            OTHER_PARAM_ERR,
            OUT_OF_SYNC,
            INVALID_RFU_BITS_IN_LENGTH,
            COMMAND_IN_PROGRESS
        }

        public REASON_CODE reason_code;
        public MODEM_STATE state;
        UInt16 received_header;
        UInt16 received_length;

        /// <summary>
        /// Constructor for parsing received data
        /// </summary>
        /// <param name="data"></param>
        public InvalidCommandNtf(byte[] data)
        {
            if (data.Length != 6) throw new Exception("Not a valid packet!");

            reason_code = (REASON_CODE)data[0];
            state = (MODEM_STATE)data[1];

            received_header = (UInt16)((data[2] << 8) + data[3]);
            received_length = (UInt16)((data[4] << 8) + data[5]);
        }
    }

    #endregion

}
