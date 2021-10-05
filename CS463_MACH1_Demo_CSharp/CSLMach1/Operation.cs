/******************************************************************************
**
** @file Operation.cs
**
** Last update: Jan 05, 2009
**
** This file provides OCS command set definition and implementation.
** 
******************************************************************************/

//History
//Jan 05, 2009
//- Fixed a bug on InventoryNtf that does not read additional memory bank correctly.

using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Xml;

namespace CSL.Mach1
{
    /// <summary>
    /// Mach1 Operation Command Set Class
    /// </summary>
    /// 
    [Serializable]
    public class OPERATION_CMD
    {
        #region CONST
        public const byte GET_OCS_VERSION = 0x00;
        public const byte LOAD_FROM_PROFILE = 0x01;
        public const byte STORE_TO_PROFILE = 0x02;
        public const byte SET_TX_POWER = 0x05;
        public const byte GET_TX_POWER = 0x06;
        public const byte SET_ANTENNA = 0x07;
        public const byte GET_ANTENNA = 0x08;
        public const byte SET_REGULATORY_REGION = 0x09;
        public const byte GET_CAPABILITY = 0x0A;
        public const byte SET_TX_FREQUENCY = 0x0B;
        public const byte GET_TX_FREQUENCY = 0x0C;
        public const byte SET_GEN2_PARAMS = 0x0D;
        public const byte GET_GEN2_PARAMS = 0x0E;
        public const byte GET_SUPPORTED_GEN2_PARAMS = 0x1C;
        public const byte CHECK_ANTENNA = 0x1D;
        public const byte SET_INVENTORY_REPORT = 0x1E;
        public const byte SET_LBT_PARAMS = 0x1F;
        public const byte GET_LBT_PARAMS = 0x20;
        public const byte INVENTORY = 0x13;
        public const byte INVENTORY_CONTINUE = 0x15;
        public const byte MODEM_STOP = 0x16;
        public const byte RF_SURVEY = 0x14;
        public const byte TAG_READ = 0x17;
        public const byte TAG_WRITE = 0x18;
        public const byte TAG_LOCK = 0x19;
        public const byte TAG_KILL = 0x1A;
        public const byte TAG_CUSTOM = 0x1B;
        public const byte REPORT_INVENTORY = 0x21;
        public const byte SET_RX_CONFIG = 0x22;
        public const byte GET_RX_CONFIG = 0x23;
        public const byte SET_PROFILE_SEQUENCE = 0x24;

        public const byte ANTENNA_PORT_1 = 0x01;
        public const byte ANTENNA_PORT_2 = 0x02;
        public const byte ANTENNA_PORT_3 = 0x04;
        public const byte ANTENNA_PORT_4 = 0x08;
        #endregion

        public enum SOURCE
        {
            STORE_TO_MODEM = 0,
            RESET_TO_FACTORY_DEFAULT
        }
        public enum FREQUENCY_SET_MODE
        {
            CENTER_FREQUENCY,
            CHOOSE_FROM_REGULATORY,
            CHOOSE_FROM_LIST,
            REDUCED_POWER_FREQUENCY_LIST
        }
        public enum MEASUREMENT_BANDWIDTH
        {
            HZ_100K = 1,
            HZ_300K = 3
        }
        public enum ANTENNA : byte
        {
            ANT1 = 0x01,
            ANT2 = 0x02,
            ANT3 = 0x04,
            ANT4 = 0X08
        }
        public enum MEMORY_BANK
        {
            RESERVED =0,
            EPC,
            TID,
            USER,
            DEFAULT = 1
        }
        public enum SET_ANTENNA_RESULT
        {
            SUCCESS,
            PORT_NOT_AVAILABLE
        }
        public enum SET_FREQUENCY_RESULT
        {
            SUCCESS,
            ERROR,
            FREQUENCY_LIST_OUT_OF_RANGE,
            AUTOSET_NOT_VALID
        }
        public enum SET_GEN2_PARAMS_RESULT
        {
            SUCCESS,
            COMBINATION_NOT_SUPPORTED,
            COMBINATION_NOT_SUPPORTED_BY_REGULATORY,
            MODE_ID_NOT_SUPPORTED,
            SESSION_AND_INVENTORY_MODE_COMBINATION_NOT_SUPPORTED
        }
        public enum SET_RX_SENSITIVITY_MODE
        {
            MAXIMUM_SENSITIVITY = 0,
            FIXED_PER_ANTENNA = 1
        }
        public enum SET_TX_POWER_RESULT
        {
            SUCESS = 0,
            ERROR_DISALLOWED,
            ERROR_NOT_SUPPORTED,
            ERROR_REQUIRE_PROFESSIONAL_INSTALLATION
        }
        public enum REPORT_INVENTORY_RESULT
        {
            SUCCESS_BUFFER_WILL_BE_FLUSH = 0,
            SUCCESS_FLUSH_IN_PROCESS,
            SUCCESS_BUFFER_EMPTY,
            ERROR
        }

        public enum INVENTORY_RESULT
        {
            SUCCESS = 0,
            FAIL_CONFIGURATION_ERROR,
            GEN2READLENGTH_EXCEED,
            PROFILE_SEQUENCING_DISABLED,
            PROFILE_SEQUENCE_INDEX_INVALID
        }

        /// <summary>
        /// Inventory Parameters Class. For how to use inventory parameters, refer to Mach1 - Operation Command Set
        /// </summary>
        [Serializable]
        public class INVENTORY_PRAMA
        {
            public enum INVENTORY_FILTER_OPERATION
            {
                A = 0,
                A_OR_B,
                A_AND_B,
                NONE
            }
            public enum INVENTORY_HALT_OPERATION
            {
                A = 0,
                A_OR_B,
                A_AND_B,
                HALT_EVERY_TAG
            }
            public enum LOGIC
            {
                EQUALS = 0,
                NOT_EQUAL,
                GREATER_THAN,
                LESS_THAN
            }

            /// <summary>
            /// Inventory Filter Condition Class
            /// </summary>
            [Serializable]
            public class INVENTORY_FILTER_CONDITION
            {
                /// <summary>
                /// Inventory Filter Operation
                /// </summary>
                public INVENTORY_FILTER_OPERATION filter_operation = INVENTORY_FILTER_OPERATION.A;
                /// <summary>
                /// A filter memory bank
                /// </summary>
                public MEMORY_BANK a_filter_memory_bank = MEMORY_BANK.DEFAULT;
                /// <summary>
                /// B filter memory bank
                /// </summary>
                public MEMORY_BANK b_filter_memory_bank = MEMORY_BANK.DEFAULT;
                /// <summary>
                /// A filter offset bit position 
                /// </summary>
                public UInt16 a_bit_offset = 0;
                /// <summary>
                /// B filter offset bit position
                /// </summary>
                public UInt16 b_bit_offset = 0;
                /// <summary>
                /// A filter pattern length
                /// </summary>
                public UInt16 a_length = 0;           
                /// <summary>
                /// B  filter pattern length
                /// </summary>
                public UInt16 b_length = 0;    
                /// <summary>
                /// A filter pattern
                /// </summary>
                public string a_pattern;
                /// <summary>
                /// B filter pattern
                /// </summary>
                public string b_pattern;
                /// <summary>
                /// A filter logic
                /// </summary>
                public LOGIC a_compare = LOGIC.EQUALS; 
                /// <summary>
                /// B filter logic
                /// </summary>
                public LOGIC b_compare = LOGIC.EQUALS;

                public INVENTORY_FILTER_CONDITION()
                {
                    a_pattern = string.Empty;
                    b_pattern = string.Empty;
                }
            }

            /// <summary>
            /// Inventory Halt Condition Class. 
            /// </summary>
            [Serializable]
            public class INVENTORY_HALT_CONDITION
            {
                /// <summary>
                /// Halt Operation
                /// </summary>
                public INVENTORY_HALT_OPERATION halt_operation = INVENTORY_HALT_OPERATION.A;
                /// <summary>
                /// A halt filter memory bank, only valid when halt_operation requires A filter
                /// </summary>
                public MEMORY_BANK halt_a_memory_bank = MEMORY_BANK.DEFAULT;
                /// <summary>
                /// B halt filter memory bank, only valid when halt_operation requires B filter
                /// </summary>
                public MEMORY_BANK halt_b_memory_bank = MEMORY_BANK.DEFAULT;
                /// <summary>
                /// A filter offset
                /// </summary>
                public UInt16 halt_a_bit_offset;
                /// <summary>
                /// B filter offset
                /// </summary>
                public UInt16 halt_b_bit_offset;
                /// <summary>
                /// A filter length
                /// </summary>
                public UInt16 halt_a_length;
                /// <summary>
                /// B filter length
                /// </summary>
                public UInt16 halt_b_length;
                /// <summary>
                /// A filter mask
                /// </summary>
                public string halt_a_mask;
                /// <summary>
                /// B filter mask
                /// </summary>
                public string halt_b_mask;
                /// <summary>
                /// A filter value, in form of bit array
                /// </summary>
                public string halt_a_value;
                /// <summary>
                /// B filter value, in form of bit array
                /// </summary>
                public string halt_b_value;
                /// <summary>
                /// A filter logic
                /// </summary>
                public LOGIC halt_a_compare = LOGIC.EQUALS; 
                /// <summary>
                /// B filter logic
                /// </summary>
                public LOGIC halt_b_compare = LOGIC.EQUALS;

                public INVENTORY_HALT_CONDITION()
                {
                    halt_a_mask = string.Empty;
                    halt_a_value = string.Empty;
                    halt_b_mask = string.Empty;
                    halt_b_value = string.Empty;
                }
            }

            /// <summary>
            /// Enable inventory filter
            /// </summary>
            public bool enable_inventory_filter = false;
            
            /// <summary>
            /// Enable halt filter
            /// </summary>
            public bool enable_halt_filter = false;

            public INVENTORY_FILTER_CONDITION inventory_filter_condition ;
            public INVENTORY_HALT_CONDITION inventory_halt_condition ;
            public MEMORY_BANK read_memory_bank = MEMORY_BANK.DEFAULT;
            public UInt16 read_word_memory_address = 0;
            public byte read_length = 0;
            public Int16 estimated_tag_population = -1;
            public Int16 estimated_tag_time_in_field = -1;

            public Int16 emptyFieldTimeOut = -1;
            public Int16 fieldPingInterval = -1;
            public Int16 profileSequenceIndex = -1;
            public bool reportNullEPCs = false;

            /// <summary>
            /// Default constructor
            /// </summary>
            public INVENTORY_PRAMA()
            {
                inventory_filter_condition = new INVENTORY_FILTER_CONDITION();
                inventory_halt_condition = new INVENTORY_HALT_CONDITION();
            }
            /// <summary>
            /// Constructor with parameters
            /// </summary>
            /// <param name="enable_inventory_filter"></param>
            /// <param name="enable_halt_filter"></param>
            public INVENTORY_PRAMA(bool enable_inventory_filter, bool enable_halt_filter)
            {
                this.enable_halt_filter = enable_halt_filter;
                this.enable_inventory_filter = enable_inventory_filter;

                inventory_filter_condition = new INVENTORY_FILTER_CONDITION();
                inventory_halt_condition = new INVENTORY_HALT_CONDITION();
            }

            /// <summary>
            /// Convert Inventory Parameters to byte array
            /// </summary>
            /// <returns></returns>
            public byte[] ToByteArray()
            {
                byte[] temp = new byte[1024];
                int idx = 0;

                try
                {
                    #region Inventory Filter
                    if (enable_inventory_filter)
                    {
                        temp[idx++] = 0x01;
                        temp[idx++] = (byte)inventory_filter_condition.filter_operation;

                        //set A filter
                        temp[idx++] = 0x02;
                        temp[idx++] = (byte)inventory_filter_condition.a_filter_memory_bank;
                        temp[idx++] = 0x03;
                        temp[idx++] = (byte)(inventory_filter_condition.a_bit_offset >> 8);
                        temp[idx++] = (byte)(inventory_filter_condition.a_bit_offset & 0x00FF);
                        temp[idx++] = 0x04;
                        temp[idx++] = (byte)(inventory_filter_condition.a_length >> 8);
                        temp[idx++] = (byte)(inventory_filter_condition.a_length & 0x00FF);
                        
                        byte[] patternA = Util.ConvertBinaryStringArrayToBytes(inventory_filter_condition.a_pattern, inventory_filter_condition.a_length);
                        if (patternA.Length > 0)
                        {
                            temp[idx++] = 0x05;
                            temp[idx++] = (byte)((patternA.Length & 0x0300) >> 8);
                            temp[idx++] = (byte)(patternA.Length & 0x00FF);
                            for (int i = 0; i < patternA.Length; i++) temp[idx++] = patternA[i];
                        }
                        
                        temp[idx++] = 0x06;
                        temp[idx++] = (byte)(inventory_filter_condition.a_compare);

                        //set B filter
                        switch (inventory_filter_condition.filter_operation)
                        {
                            case INVENTORY_FILTER_OPERATION.A:
                                break;
                            case INVENTORY_FILTER_OPERATION.A_AND_B:
                            case INVENTORY_FILTER_OPERATION.A_OR_B:
                                temp[idx++] = 0x07;
                                temp[idx++] = (byte)inventory_filter_condition.b_filter_memory_bank;
                                temp[idx++] = 0x08;
                                temp[idx++] = (byte)(inventory_filter_condition.b_bit_offset >> 8);
                                temp[idx++] = (byte)(inventory_filter_condition.b_bit_offset & 0x00FF);
                                temp[idx++] = 0x09;
                                temp[idx++] = (byte)(inventory_filter_condition.b_length >> 8);
                                temp[idx++] = (byte)(inventory_filter_condition.b_length & 0x00FF);
                                byte[] patternB = Util.ConvertBinaryStringArrayToBytes(inventory_filter_condition.b_pattern, inventory_filter_condition.b_length);
                                if (patternB.Length > 0)
                                {
                                    temp[idx++] = 0x0A;
                                    temp[idx++] = (byte)((patternB.Length & 0x0300) >> 8);
                                    temp[idx++] = (byte)(patternB.Length & 0x00FF);
                                    for (int i = 0; i < patternB.Length; i++) temp[idx++] = patternB[i];
                                }
                                temp[idx++] = 0x0B;
                                temp[idx++] = (byte)(inventory_filter_condition.b_compare);
                                break;
                        }

                    }
                    #endregion

                    #region Read Memory Bank
                    if (read_memory_bank != MEMORY_BANK.DEFAULT)
                    {
                        temp[idx++] = 0x0C;
                        temp[idx++] = (byte)read_memory_bank;
                        temp[idx++] = 0x0D;
                        temp[idx++] = (byte)(read_word_memory_address >> 8);
                        temp[idx++] = (byte)(read_word_memory_address & 0x00FF);
                        temp[idx++] = 0x0E;
                        temp[idx++] = read_length;
                    }

                    #endregion

                    #region Halt Filter
                    if (enable_halt_filter)
                    {
                        if (inventory_halt_condition.halt_operation == INVENTORY_HALT_OPERATION.HALT_EVERY_TAG)
                        {
                            temp[idx++] = 0x0F;
                            temp[idx++] = 0x00;
                            temp[idx++] = 0x12;
                            temp[idx++] = 0x00;
                            temp[idx++] = 0x00;
                        }
                        else
                        {
                            temp[idx++] = 0x0F;
                            temp[idx++] = (byte)inventory_halt_condition.halt_operation;
                            temp[idx++] = 0x12;
                            temp[idx++] = (byte)(inventory_halt_condition.halt_a_length >> 8);
                            temp[idx++] = (byte)(inventory_halt_condition.halt_a_length & 0x00FF);
                            
                            temp[idx++] = 0x10;      
                            temp[idx++] = (byte)inventory_halt_condition.halt_a_memory_bank;
                   
                            temp[idx++] = 0x11;
                            temp[idx++] = (byte)(inventory_halt_condition.halt_a_bit_offset >> 8);
                            temp[idx++] = (byte)(inventory_halt_condition.halt_a_bit_offset & 0x00FF);

                            byte[] haltMask = Util.ConvertBinaryStringArrayToBytes(inventory_halt_condition.halt_a_mask, inventory_halt_condition.halt_a_length);
                            if (haltMask.Length > 0)
                            {
                                temp[idx++] = 0x13;
                                temp[idx++] = (byte)((haltMask.Length & 0x0300) >> 8);
                                temp[idx++] = (byte)(haltMask.Length & 0x00FF);
                                for (int i = 0; i < haltMask.Length; i++) temp[idx++] = haltMask[i];
                            }
                             
                            byte[] haltPA = Util.ConvertBinaryStringArrayToBytes(inventory_halt_condition.halt_a_value, inventory_halt_condition.halt_a_length);
                            if (haltPA.Length > 0)
                            {
                                temp[idx++] = 0x14;
                                temp[idx++] = (byte)((haltPA.Length & 0x0300) >> 8);
                                temp[idx++] = (byte)(haltPA.Length & 0x00FF);
                                for (int i = 0; i < haltPA.Length; i++) temp[idx++] = haltPA[i];
                            }
                            
                            temp[idx++] = 0x15;
                            temp[idx++] = (byte)(inventory_halt_condition.halt_a_compare);
                            
                            switch (inventory_halt_condition.halt_operation)
                            {
                                case INVENTORY_HALT_OPERATION.A:
                                    break;
                                case INVENTORY_HALT_OPERATION.A_AND_B:
                                case INVENTORY_HALT_OPERATION.A_OR_B:
                                    temp[idx++] = 0x16;
                                    temp[idx++] = (byte)inventory_halt_condition.halt_b_memory_bank;
                                    temp[idx++] = 0x17;
                                    temp[idx++] = (byte)(inventory_halt_condition.halt_b_bit_offset >> 8);
                                    temp[idx++] = (byte)(inventory_halt_condition.halt_b_bit_offset & 0x00FF);
                                    temp[idx++] = 0x18;
                                    temp[idx++] = (byte)(inventory_halt_condition.halt_b_length >> 8);
                                    temp[idx++] = (byte)(inventory_halt_condition.halt_b_length & 0x00FF);
                                    
                                    haltMask = Util.ConvertBinaryStringArrayToBytes(inventory_halt_condition.halt_b_mask, inventory_halt_condition.halt_b_length);
                                    if (haltMask.Length > 0)
                                    {
                                        temp[idx++] = 0x19;
                                        temp[idx++] = (byte)((haltMask.Length & 0x0300) >> 8);
                                        temp[idx++] = (byte)(haltMask.Length & 0x00FF);
                                        for (int i = 0; i < haltMask.Length; i++) temp[idx++] = haltMask[i];
                                    }
                                    byte[] haltPB = Util.ConvertBinaryStringArrayToBytes(inventory_halt_condition.halt_b_value, inventory_halt_condition.halt_b_length);
                                    if (haltPB.Length > 0)
                                    {
                                        temp[idx++] = 0x1A;
                                        temp[idx++] = (byte)((haltPB.Length & 0x0300) >> 8);
                                        temp[idx++] = (byte)(haltPB.Length & 0x00FF);
                                        for (int i = 0; i < haltPB.Length; i++) temp[idx++] = haltPB[i];
                                    }

                                    temp[idx++] = 0x1B;
                                    temp[idx++] = (byte)(inventory_halt_condition.halt_b_compare);
                                    break;
                            }
                        }
                    }
                    #endregion

                    if (estimated_tag_population >= 0)
                    {
                        temp[idx++] = 0x1C;
                        temp[idx++] = (byte)(estimated_tag_population >> 8);
                        temp[idx++] = (byte)(estimated_tag_population & 0x00FF);
                    }

                    if (estimated_tag_time_in_field >=0)
                    {
                        temp[idx++] = 0x1D;
                        temp[idx++] = (byte)(estimated_tag_time_in_field >> 8);
                        temp[idx++] = (byte)(estimated_tag_time_in_field & 0x00FF);
                    }

                    if (emptyFieldTimeOut >= 0 && fieldPingInterval >= 0)
                    {
                        temp[idx++] = 0x1E;
                        temp[idx++] = (byte)(emptyFieldTimeOut >> 8);
                        temp[idx++] = (byte)(emptyFieldTimeOut & 0x00FF);

                        temp[idx++] = 0x1F;
                        temp[idx++] = (byte)(fieldPingInterval >> 8);
                        temp[idx++] = (byte)(fieldPingInterval & 0x00FF);
                    }

                    if (profileSequenceIndex > -1)
                    {
                        temp[idx++] = 0x20;
                        temp[idx++] = (byte)(profileSequenceIndex & 0x00FF);
                    }

                    if (reportNullEPCs)             
                    {
                        temp[idx++] = 0x21;
                        temp[idx++] = 0x01;
                    }

                    byte[] data;
                    
                    if (idx == 0) data = null;
                    else
                    {
                        data = new byte[idx];
                        Array.Copy(temp, data, idx);
                    }
                    return data;
                }
                catch
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Gen2 Parameters Class. It is recommended to used Mode ID to set these parameters 
        /// </summary>
        [Serializable]
        public class GEN2_PARAM
        {
            public enum SESSION
            {
                SESSION_0 = 0,
                SESSION_1 = 1,
                SESSION_2,
                SESSION_3,
            }
            public enum GEN2_LINK_MODE
            {
                BY_APPLICATION,
                SELF_CONFIGURE_DENSE,
                BY_MODE_ID_OF_APPLICATION,
                SELF_CONFIGURE_SINGLE
            
            }
            public enum MODEM_MODULATION
            {
                PR_ASK,
                DSB_ASK
            }
            public enum TARI
            {
                US_6_25,
                US_7_14,
                US_8_33,
                US_10_0,
                US_12_5,
                US_16_67,
                US_20_0,
                US_25_0
            }
            public enum PIE
            {
                P_1_5_vs_1,
                P_1_67_vs_1,
                P_2_0_vs_1
            }
            public enum PW
            {
                SHORT,
                LONG
            }
            public enum TR_FREQUENCY
            {
                HZ_40K,
                HZ_64K,
                HZ_80K,
                HZ_128K,
                HZ_160K,
                HZ_213_3K,
                HZ_256K,
                HZ_320K,
                HZ_640K
            }
            public enum TR_LINK_MODULATION
            {
                FM0=0,
                MILLER_M2,
                MILLER_M4,
                MILLER_M8
            }
            public enum DEVIDE_RATIO
            {
                RATIO_64_3,
                RATIO_8
            }
            public enum INVENTORY_SEARCH_MODE
            {
                DEFAULT = 0,
                SINGLE_TARGET_INVENTORY,
                SINGLE_TARGET_INVENTORY_WITH_SUPPRESSED_DUPLICATE_REDUNDANCY
            }
            public enum MODE_ID
            {
                MAX_THROUGHPUT = 0,
                HYBRID_MODE,
                DENSE_READER_M4,
                DENSE_READER_M8,
                MAX_THROUGH_PUT_MILLER
            }

            public SESSION session = SESSION.SESSION_1;
            public GEN2_LINK_MODE auto_set_mode = GEN2_LINK_MODE.SELF_CONFIGURE_DENSE;
            public MODEM_MODULATION modem_modulation = MODEM_MODULATION.PR_ASK;
            public TARI tari = TARI.US_12_5;
            public PIE pie= PIE.P_1_5_vs_1;
            public PW pw = PW.SHORT;
            public TR_FREQUENCY tag_to_reader_link_rate = TR_FREQUENCY.HZ_160K;
            public TR_LINK_MODULATION tag_to_reader_link_modulation = TR_LINK_MODULATION.FM0;
            public bool handle_reporting = false;
            public DEVIDE_RATIO devide_ratio = DEVIDE_RATIO.RATIO_64_3;

            public MODE_ID mode_id;
            public INVENTORY_SEARCH_MODE inv_search_mode = INVENTORY_SEARCH_MODE.DEFAULT;

            /// <summary>
            /// Default constructor
            /// </summary>
            public GEN2_PARAM()
            {
            }

            /// <summary>
            /// Constructor for parsing Gen2 paramters 
            /// </summary>
            /// <param name="data"></param>
            public GEN2_PARAM(byte[] data)
            {
                try
                {
                    session = (SESSION)data[0];
                    auto_set_mode = (GEN2_LINK_MODE)data[1];
                    int offset = 2;

                    while (offset < data.Length)
                    {
                        switch (data[offset])
                        {
                            case 0x01:
                                offset++;
                                modem_modulation = (MODEM_MODULATION)data[offset];
                                offset++;
                                break;
                            case 0x02:
                                offset++;
                                tari = (TARI)data[offset];
                                offset++;
                                break;
                            case 0x03:
                                offset++;
                                pie = (PIE)data[offset];
                                offset++;
                                break;
                            case 0x04:
                                offset++;
                                pw = (PW)data[offset];
                                offset++;
                                break;
                            case 0x05:
                                offset++;
                                tag_to_reader_link_rate = (TR_FREQUENCY)data[offset];
                                offset++;
                                break;
                            case 0x06:
                                offset++;
                                tag_to_reader_link_modulation = (TR_LINK_MODULATION)data[offset];
                                offset++;
                                break;
                            case 0x07:
                                offset++;
                                handle_reporting = (data[offset] == 1) ? true : false;
                                offset++;
                                break;
                            case 0x08:
                                offset++;
                                devide_ratio = (DEVIDE_RATIO)data[offset];
                                offset++;
                                break;
                            case 0x09:
                                offset++;
                                mode_id = (MODE_ID)((UInt16)( data[offset] * 256 + data[offset + 1]));
                                offset += 2;
                                break;
                            case 0x0A:
                                offset++;
                                inv_search_mode = (INVENTORY_SEARCH_MODE)data[offset];
                                offset++;
                                break;

                        }
                    }

                }
                catch
                { }
            }

            /// <summary>
            /// Generate byte array
            /// </summary>
            /// <returns></returns>
            public byte[] ToByteArray()
            {
                byte[] temp = new byte[1024];
                int idx = 0;

                temp[idx++] = (byte)session;
                temp[idx++] = (byte)auto_set_mode;
                
                //Bug
                //Reported by Xiaoyong Su,  01/09/2008
                //Fixed by Xiaoyong Su, 01/09/2008
                if (auto_set_mode == GEN2_LINK_MODE.BY_APPLICATION)
                {
                    temp[idx++] = 0x01;
                    temp[idx++] = (byte)modem_modulation;
                    temp[idx++] = 0x02;
                    temp[idx++] = (byte)tari;
                    temp[idx++] = 0x03;
                    temp[idx++] = (byte)pie;
                    temp[idx++] = 0x04;
                    temp[idx++] = (byte)pw;
                    temp[idx++] = 0x05;
                    temp[idx++] = (byte)tag_to_reader_link_rate;
                    temp[idx++] = 0x06;
                    temp[idx++] = (byte)tag_to_reader_link_modulation;
                    temp[idx++] = 0x07;
                    temp[idx++] = (byte)(handle_reporting ? 1 : 0);
                    temp[idx++] = 0x08;
                    temp[idx++] = (byte)devide_ratio;
                }
                else if (auto_set_mode == GEN2_LINK_MODE.BY_MODE_ID_OF_APPLICATION)
                {
                    temp[idx++] = 0x09;
                    temp[idx++] = (byte)(((UInt16)mode_id) >> 8);
                    temp[idx++] = (byte)(((UInt16)mode_id) & 0x00FF);
                }

                temp[idx++] = 0x0A;
                temp[idx++] = (byte)inv_search_mode;

                byte[] data = new byte[idx];

                Array.Copy(temp, data, idx);

                return data;
            }
        }

        /// <summary>
        /// Supported Gen2 Params Rsp Class
        /// </summary>
        [Serializable]
        public class SUPPORTED_GEN2_PARAMS_RSP
        {
            public UInt16 num_supported_sets;
            public GEN2_PARAM.MODEM_MODULATION[] modulations;
            public GEN2_PARAM.TARI[] taris;
            public GEN2_PARAM.PIE[] pies;
            public GEN2_PARAM.PW[] pws;
            public GEN2_PARAM.TR_FREQUENCY[] tr_link_frequencyes;
            public GEN2_PARAM.TR_LINK_MODULATION[] tr_link_modulations;
            public GEN2_PARAM.DEVIDE_RATIO[] d_ratios;
            public GEN2_PARAM.MODE_ID[] mode_ids;
            public GEN2_PARAM.INVENTORY_SEARCH_MODE[] inv_search_modes;

            /// <summary>
            /// Constructor for parsing data
            /// </summary>
            /// <param name="data"></param>
            public SUPPORTED_GEN2_PARAMS_RSP(byte[] data)
            {
                int offset = 0;
                try
                {
                    num_supported_sets = data[0];
                    offset++;
                    int result_sets = data[offset] * 256 + data[offset+1];

                    offset += 2;
                    modulations = new GEN2_PARAM.MODEM_MODULATION[result_sets];
                    for (int i = 0; i < result_sets; i++)
                        modulations[i] = (GEN2_PARAM.MODEM_MODULATION)data[offset++];

                    result_sets = data[offset++] * 256 + data[offset++];
                    taris = new GEN2_PARAM.TARI[result_sets];
                    for (int i = 0; i < result_sets; i++)
                        taris[i] = (GEN2_PARAM.TARI)data[offset++];

                    result_sets = data[offset++] * 256 + data[offset++];
                    pies = new GEN2_PARAM.PIE[result_sets];
                    for (int i = 0; i < result_sets; i++)
                        pies[i] = (GEN2_PARAM.PIE)data[offset++];

                    result_sets = data[offset++] * 256 + data[offset++];
                    pws = new GEN2_PARAM.PW[result_sets];
                    for (int i = 0; i < result_sets; i++)
                        pws[i] = (GEN2_PARAM.PW)data[offset++];

                    result_sets = data[offset++] * 256 + data[offset++];
                    tr_link_frequencyes = new GEN2_PARAM.TR_FREQUENCY[result_sets];
                    for (int i = 0; i < result_sets; i++)
                        tr_link_frequencyes[i] = (GEN2_PARAM.TR_FREQUENCY)data[offset++];

                    result_sets = data[offset++] * 256 + data[offset++];
                    tr_link_modulations = new GEN2_PARAM.TR_LINK_MODULATION[result_sets];
                    for (int i = 0; i < result_sets; i++)
                        tr_link_modulations[i] = (GEN2_PARAM.TR_LINK_MODULATION)data[offset++];

                    result_sets = data[offset++] * 256 + data[offset++];
                    d_ratios = new GEN2_PARAM.DEVIDE_RATIO[result_sets];
                    for (int i = 0; i < result_sets; i++)
                        d_ratios[i] = (GEN2_PARAM.DEVIDE_RATIO)data[offset++];

                    while (offset < data.Length)
                    {
                        if (offset < data.Length && data[offset] == 0x01)
                        {
                            try
                            {
                                offset++;
                                result_sets = data[offset++] * 256 + data[offset++];
                                mode_ids = new GEN2_PARAM.MODE_ID[result_sets];
                                for (int i = 0; i < result_sets; i++) mode_ids[i] = (GEN2_PARAM.MODE_ID)data[offset++];
                            }
                            catch { };
                        }

                        if (offset < data.Length && data[offset] == 0x02)
                        {
                            try
                            {
                                offset++;
                                result_sets = data[offset++] * 256 + data[offset++];
                                inv_search_modes = new GEN2_PARAM.INVENTORY_SEARCH_MODE[result_sets];
                                for (int i = 0; i < result_sets; i++) inv_search_modes[i] = (GEN2_PARAM.INVENTORY_SEARCH_MODE)data[offset++];
                            }
                            catch { };
                        }
                    }

                }
                catch{}
            }
        }

        /// <summary>
        /// Operation Command Set Version Rsp Class
        /// </summary>
        [Serializable]
        public class OCS_VERSION_RSP
        {
            public string version = string.Empty;

            public OCS_VERSION_RSP(byte[] data)
            {
                try
                {
                    version = string.Format("v.{0}.{1}.{2}", data[0], data[1], data[2]);
                }
                catch { }
            }
        }

        /// <summary>
        /// Tag Lock Operation Command Class. Refer to EPC Gen2 protocol for more information
        /// </summary>
        [Serializable]
        public class TAG_LOCK_OPERATION
        {

            public class OPERATION
            {
                public bool enable_read_writeble_bit = false;
                public bool read_writeble = false;
                public bool enable_perma_lock_bit = false;
                public bool perma_lock = false;
            }

            public OPERATION kill_pwd;
            public OPERATION access_pwd;
            public OPERATION epc_memory;
            public OPERATION tid_memory;
            public OPERATION user_memory;

            public TAG_LOCK_OPERATION()
            {
                kill_pwd = new OPERATION();
                access_pwd = new OPERATION();
                epc_memory = new OPERATION();
                tid_memory = new OPERATION();
                user_memory = new OPERATION();
            }

            public byte[] ToByteArray()
            {
                BitArray br = new BitArray(32);
                br.SetAll(false);

                br.Set(12, kill_pwd.enable_read_writeble_bit);
                br.Set(13, kill_pwd.enable_perma_lock_bit);
                br.Set(14, access_pwd.enable_read_writeble_bit);
                br.Set(15, access_pwd.enable_perma_lock_bit);
                br.Set(16, epc_memory.enable_read_writeble_bit);
                br.Set(17, epc_memory.enable_perma_lock_bit);
                br.Set(18, tid_memory.enable_read_writeble_bit);
                br.Set(19, tid_memory.enable_perma_lock_bit);
                br.Set(20, user_memory.enable_read_writeble_bit);
                br.Set(21, user_memory.enable_perma_lock_bit);

                br.Set(22, kill_pwd.read_writeble);
                br.Set(23, kill_pwd.perma_lock);
                br.Set(24, access_pwd.read_writeble);
                br.Set(25, access_pwd.perma_lock);
                br.Set(26, epc_memory.read_writeble);
                br.Set(27, epc_memory.perma_lock);
                br.Set(28, tid_memory.read_writeble);
                br.Set(29, tid_memory.perma_lock);
                br.Set(30, user_memory.read_writeble);
                br.Set(31, user_memory.perma_lock);

                try
                {
                    string s = string.Empty;
                    for (int i = 0; i < br.Length; i++) s += br[i] ? "1" : "0";
                    return Util.ConvertBinaryStringArrayToBytes(s, 0);
                }
                catch
                {
                    return null;
                }

            }
        }

        /// <summary>
        /// Optional Inventory Report Params. Refer to Mach1 - Operation Command Set
        /// </summary>
        [Serializable]
        public class OPTIONAL_INVENTORY_REPORT_PARAM
        {
            public enum INVENTORY_REPORTING_MODE
            {
                IMMEDIATE_REPORT = 0,
                ACCUMULATED_REPORT
            }
            public enum ADD_BEHAVIOR
            {
                DONT_REPORT_WHEN_ADDED =0,
                REPORT_WHEN_ADDED
            }
            public enum INVENTORY_ATTEMPT_COUNT_REPORTING
            {
                DONT_REPORT = 0,
                REPORT
            }

            public enum DROP_BEHAVIOR
            {
                DONT_REPORT_WHEN_DROPPED = 0,
                REPORT_WHEN_DROPPED
            }

            public enum BUFFER_FULL_BEHAVIOR
            {
                DROP_NEWEST_INVENOTRY_NTF = 0,
                DROP_ALL_ENTRIES
            }

            public INVENTORY_REPORTING_MODE inventory_report_mode = INVENTORY_REPORTING_MODE.ACCUMULATED_REPORT;
            public ADD_BEHAVIOR add_behavior = ADD_BEHAVIOR.DONT_REPORT_WHEN_ADDED;
            public INVENTORY_ATTEMPT_COUNT_REPORTING inventory_attemp_count_reporting = INVENTORY_ATTEMPT_COUNT_REPORTING.DONT_REPORT;
            public DROP_BEHAVIOR drop_behavior = DROP_BEHAVIOR.DONT_REPORT_WHEN_DROPPED;
            public BUFFER_FULL_BEHAVIOR buffer_full_behavior = BUFFER_FULL_BEHAVIOR.DROP_ALL_ENTRIES;
        }


        #region GENERATE_PACKETS
        /// <summary>
        /// Generate GetOCSVersionCmd Message
        /// </summary>
        /// <param name="include_timestamp"></param>
        /// <returns></returns>
        public static byte[] GENERATE_GET_OCS_VERSION_CMD(bool include_timestamp)
        {
            MACH1_FRAME cmd = new MACH1_FRAME(CATEGORY.OPERATION_MODEL, OPERATION_CMD.GET_OCS_VERSION, include_timestamp);
            return cmd.PACKET;
        }
        /// <summary>
        /// Generate LoadFromProfileCmd Message
        /// </summary>
        /// <param name="include_timestamp"></param>
        /// <param name="profile_index"></param>
        /// <returns></returns>
        public static byte[] GENERATE_LOAD_FROM_PROFILE_CMD(bool include_timestamp, UInt16 profile_index)
        {
            byte[] data = new byte[1];
            data[0] = (byte)profile_index;

            MACH1_FRAME cmd = new MACH1_FRAME(CATEGORY.OPERATION_MODEL, OPERATION_CMD.LOAD_FROM_PROFILE, include_timestamp, data);
            return cmd.PACKET;
        }

        /// <summary>
        /// Generate StoreToProfileCmd Message
        /// </summary>
        /// <param name="include_timestamp"></param>
        /// <param name="idx"></param>
        /// <param name="dst"></param>
        /// <returns></returns>
        public static byte[] GENERATE_STORE_TO_PROFILE_CMD(bool include_timestamp, UInt16 idx, SOURCE dst)
        {
            byte[] data = new byte[2];

            data[0] = (byte)idx;
            data[1] = (byte)dst;

            MACH1_FRAME cmd = new MACH1_FRAME(CATEGORY.OPERATION_MODEL, OPERATION_CMD.STORE_TO_PROFILE, include_timestamp, data);
            return cmd.PACKET;
        }

        /// <summary>
        /// Generate InventoryCmd message
        /// </summary>
        /// <param name="include_timestamp"></param>
        /// <param name="prama"></param>
        /// <returns></returns>
        public static byte[] GENERATE_INVENTORY_CMD(bool include_timestamp, INVENTORY_PRAMA prama)
        {
            MACH1_FRAME mf = new MACH1_FRAME(CATEGORY.OPERATION_MODEL, OPERATION_CMD.INVENTORY, include_timestamp, prama.ToByteArray());
            return mf.PACKET;
        }

        /// <summary>
        /// Generate SetTxPowerCmd message
        /// </summary>
        /// <param name="include_timestamp"></param>
        /// <param name="txPower"></param>
        /// <returns></returns>
        public static byte[] GENERATE_SET_TX_POWER_CMD(bool include_timestamp, byte[] txPower)
        {
            byte[] data;

            if (txPower.Length == 1)
            {
                data = new byte[1];
                data[0] = txPower[0];
            }
            else
            {
                data = new byte[txPower.Length + 3];
                data[0] = txPower[0];

                data[1] = 0x01;
                data[2] = 0x00;
                data[3] = (byte)(txPower.Length - 1);

                Array.Copy(txPower, 1, data, 4, txPower.Length - 1);
            }  

            MACH1_FRAME mf = new MACH1_FRAME(CATEGORY.OPERATION_MODEL, OPERATION_CMD.SET_TX_POWER, include_timestamp, data);
            return mf.PACKET;
        }

        /// <summary>
        /// Generate GetTxPowerCmd message
        /// </summary>
        /// <param name="include_timestamp"></param>
        /// <returns></returns>
        public static byte[] GENERATE_GET_TX_POWER_CMD(bool include_timestamp)
        {
            MACH1_FRAME mf = new MACH1_FRAME(CATEGORY.OPERATION_MODEL, OPERATION_CMD.GET_TX_POWER, include_timestamp);

            return mf.PACKET;
        }

        /// <summary>
        /// Generate SetAntennaCmd message
        /// </summary>
        /// <param name="include_timerstamp"></param>
        /// <param name="antennas"></param>
        /// <returns></returns>
        public static byte[] GENERATE_SET_ANTENNA_CMD(bool include_timerstamp, byte antennas)
        {

            byte[] data = new byte[1];
            data[0] = antennas;

            MACH1_FRAME mf = new MACH1_FRAME(CATEGORY.OPERATION_MODEL, OPERATION_CMD.SET_ANTENNA, include_timerstamp, data);

            return mf.PACKET;
        }

        /// <summary>
        /// Generate GetAntennaCmd message
        /// </summary>
        /// <param name="include_timestamp"></param>
        /// <returns></returns>
        public static byte[] GENERATE_GET_ANTENNA_CMD(bool include_timestamp)
        {
            MACH1_FRAME mf = new MACH1_FRAME(CATEGORY.OPERATION_MODEL, OPERATION_CMD.GET_ANTENNA, include_timestamp);
            return mf.PACKET;
        }

        /// <summary>
        /// Generate SetRegulatoryRegion message
        /// </summary>
        /// <param name="include_timestamp"></param>
        /// <param name="region"></param>
        /// <returns></returns>
        public static byte[] GENERATE_SET_REGULATORY_REGION(bool include_timestamp, REGULATORY_REGION region)
        {
            byte[] data = new byte[2];

            data[0] = (byte)(((UInt16)region) >> 8);
            data[1] = (byte)(((UInt16)region)& 0x00FF);

            MACH1_FRAME mf = new MACH1_FRAME(CATEGORY.OPERATION_MODEL, OPERATION_CMD.SET_REGULATORY_REGION, include_timestamp, data);
            return mf.PACKET;
        }

        /// <summary>
        /// Generate SetTxFrequency message
        /// </summary>
        /// <param name="include_timestamp"></param>
        /// <param name="mode"></param>
        /// <param name="center_frequency_index"></param>
        /// <param name="frequency_list"></param>
        /// <returns></returns>
        public static byte[] GENERATE_SET_TX_FREQUENCY(bool include_timestamp, FREQUENCY_SET_MODE mode, UInt16 center_frequency_index, UInt16[] frequency_list, UInt16[] reduced_power_frequence_list)
        {
            byte[] data; 

            switch (mode)
            {
                case FREQUENCY_SET_MODE.CENTER_FREQUENCY:
                    data = new byte[4];
                    data[1] = 0x01;
                    data[2] = (byte)(((UInt16)center_frequency_index) >> 8);
                    data[3] = (byte)(((UInt16)center_frequency_index) & 0x00FF);
                    break;
                case FREQUENCY_SET_MODE.CHOOSE_FROM_LIST:
                    data = new byte[5 + frequency_list.Length];
                    data[0] = (byte)mode;
                    data[1] = 0x02;
                    data[2] = (byte)((frequency_list.Length*2 & 0xFF00) >> 8);
                    data[3] = (byte)(frequency_list.Length*2 & 0x00FF);
                    for (int i = 0; i < frequency_list.Length; i++)
                    {
                        data[4 + 2 * i] = (byte)(((UInt16)frequency_list[i]) >> 8);
                        data[5 + 2 * i] = (byte)(((UInt16)frequency_list[i]) & 0x00FF);
                    }
                    break;
                case FREQUENCY_SET_MODE.REDUCED_POWER_FREQUENCY_LIST:
                    data = new byte[5 + reduced_power_frequence_list.Length];
                    data[0] = (byte)mode;
                    data[1] = 0x02;
                    data[2] = (byte)((reduced_power_frequence_list.Length * 2 & 0xFF00) >> 8);
                    data[3] = (byte)(reduced_power_frequence_list.Length * 2 & 0x00FF);
                    for (int i = 0; i < reduced_power_frequence_list.Length; i++)
                    {
                        data[4 + 2 * i] = (byte)(((UInt16)reduced_power_frequence_list[i]) >> 8);
                        data[5 + 2 * i] = (byte)(((UInt16)reduced_power_frequence_list[i]) & 0x00FF);
                    }
                    break;
                case FREQUENCY_SET_MODE.CHOOSE_FROM_REGULATORY:
                    data = new byte[1];
                    data[0] = (byte)mode;
                    break;
                default:
                    data = new byte[1];
                    data[0] = (byte)FREQUENCY_SET_MODE.CHOOSE_FROM_REGULATORY;
                    break;
            }

            MACH1_FRAME mf = new MACH1_FRAME(CATEGORY.OPERATION_MODEL, OPERATION_CMD.SET_TX_FREQUENCY, include_timestamp, data);
            return mf.PACKET;
        }
        /// <summary>
        /// Generate GetTxFrequency message
        /// </summary>
        /// <param name="include_timestamp"></param>
        /// <returns></returns>
        public static byte[] GENERATE_GET_TX_FREQUENCY(bool include_timestamp)
        {
            MACH1_FRAME mf = new MACH1_FRAME(CATEGORY.OPERATION_MODEL, OPERATION_CMD.GET_TX_FREQUENCY, include_timestamp);
            return mf.PACKET;
        }

        /// <summary>
        /// Generate SetGen2ParamsCmd message
        /// </summary>
        /// <param name="include_timestamp"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static byte[] GENERATE_SET_GEN2_PARAMS_CMD(bool include_timestamp, GEN2_PARAM param)
        {
            MACH1_FRAME mf = new MACH1_FRAME(CATEGORY.OPERATION_MODEL, OPERATION_CMD.SET_GEN2_PARAMS, include_timestamp, param.ToByteArray());
            return mf.PACKET;
        }

        /// <summary>
        /// Generate GetGen2ParamsCmd message
        /// </summary>
        /// <param name="include_timestamp"></param>
        /// <returns></returns>
        public static byte[] GENERATE_GET_GEN2_PARAMS_CMD(bool include_timestamp)
        {
            MACH1_FRAME mf = new MACH1_FRAME(CATEGORY.OPERATION_MODEL, OPERATION_CMD.GET_GEN2_PARAMS, include_timestamp);
            return mf.PACKET;
        }

        /// <summary>
        /// Generate GetGen2ParamsCmd message
        /// </summary>
        /// <param name="report_search_mode"></param>
        /// <param name="include_timestamp"></param>
        /// <returns></returns>
        public static byte[] GENERATE_GET_GEN2_PARAMS_CMD(bool report_search_mode, bool include_timestamp)
        {
            byte[] data = new byte[2];
            data[0] = 0x01;
            data[1] = (byte)(report_search_mode ? 1 : 0);
            MACH1_FRAME mf = new MACH1_FRAME(CATEGORY.OPERATION_MODEL, OPERATION_CMD.GET_GEN2_PARAMS, include_timestamp, data);
            return mf.PACKET;
        }

        /// <summary>
        /// Generate GetSupportedGen2ParamsCmd message
        /// </summary>
        /// <param name="include_timestamp"></param>
        /// <param name="includeModeId"></param>
        /// <param name="includeInventorySearchMode"></param>
        /// <returns></returns>
        public static byte[] GENERATE_GET_SUPPORTED_GEN2_PARAMS_CMD(bool include_timestamp, bool includeModeId, bool includeInventorySearchMode)
        {
            byte[] data = new byte[4];
            data[0] = 0x01;
            data[1] = (byte)(includeModeId ? 1 : 0);
            data[2] = 0x02;
            data[3] = (byte)(includeInventorySearchMode? 1:0);

            MACH1_FRAME mf = new MACH1_FRAME(CATEGORY.OPERATION_MODEL, OPERATION_CMD.GET_SUPPORTED_GEN2_PARAMS, include_timestamp, data);
            return mf.PACKET;
        }

        /// <summary>
        /// Generate CheckAntennaCmd Message
        /// </summary>
        /// <param name="include_timestamp"></param>
        /// <returns></returns>
        public static byte[] GENERATE_CHECK_ANTENNA_CMD(bool include_timestamp)
        {
            MACH1_FRAME mf = new MACH1_FRAME(CATEGORY.OPERATION_MODEL, OPERATION_CMD.CHECK_ANTENNA, include_timestamp);
            return mf.PACKET;
        }

        /// <summary>
        /// Generate GetCapabilitiesCmd Message
        /// </summary>
        /// <param name="include_timestamp"></param>
        /// <param name="report"></param>
        /// <returns></returns>
        /// 
        public static byte[] GENERATE_GET_CAPABILITIES_CMD(bool include_timestamp, bool reportPower, bool frequey)
        {
            byte[] data = new byte[4];
            data[0] = 0x01;
            data[1] = (byte)(reportPower ? 1 : 0);
            data[2] = 0x02;
            data[3] = (byte)(frequey ? 1 : 0);

            MACH1_FRAME mf = new MACH1_FRAME(CATEGORY.OPERATION_MODEL, OPERATION_CMD.GET_CAPABILITY, include_timestamp, data);
            return mf.PACKET;
        }
        /// <summary>
        /// Generate SetInventoryReport message
        /// </summary>
        /// <param name="include_timestamp"></param>
        /// <param name="reportStatus"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static byte[] GENERATE_SET_INVENTORY_REPORT(bool include_timestamp, bool reportStatus, OPTIONAL_INVENTORY_REPORT_PARAM param)
        {
            byte[] temp_data = new byte[11];
            temp_data[0] = (byte)(reportStatus ? 0 : 1);
            temp_data[1] = 0x01;
            temp_data[2] = (byte)param.inventory_report_mode;
            
            ///////------------Bug fixed------------///////
            //Reported by:  levinro 
            //Fixed by:     Xiaoyong Su
            //Date:         1/4/2008
            if (param.inventory_report_mode == OPTIONAL_INVENTORY_REPORT_PARAM.INVENTORY_REPORTING_MODE.ACCUMULATED_REPORT)
            {
                temp_data[3] = 0x02;
                temp_data[4] = (byte)param.add_behavior;
                temp_data[5] = 0x03;
                temp_data[6] = (byte)param.inventory_attemp_count_reporting;
                temp_data[7] = 0x04;
                temp_data[8] = (byte)param.drop_behavior;
                temp_data[9] = 0x05;
                temp_data[10] = (byte)param.buffer_full_behavior;

                MACH1_FRAME mf = new MACH1_FRAME(CATEGORY.OPERATION_MODEL, OPERATION_CMD.SET_INVENTORY_REPORT, include_timestamp, temp_data);
                return mf.PACKET;
            }
            else
            {
                byte[] data = new byte[3];
                Array.Copy(temp_data, data, 3);

                MACH1_FRAME mf = new MACH1_FRAME(CATEGORY.OPERATION_MODEL, OPERATION_CMD.SET_INVENTORY_REPORT, include_timestamp, data);
                return mf.PACKET;
            }
        }
        /// <summary>
        /// Generate SetLBTParamsCmd message
        /// </summary>
        /// <param name="include_timestamp"></param>
        /// <param name="autoSet"></param>
        /// <returns></returns>
        public static byte[] GENERATE_SET_LBT_PARAMS_CMD(bool include_timestamp, bool autoSet)
        {
            byte[] data = new byte[1];
            data[0] = (byte)(autoSet ? 0 : 1);

            MACH1_FRAME mf = new MACH1_FRAME(CATEGORY.OPERATION_MODEL, OPERATION_CMD.SET_LBT_PARAMS, include_timestamp, data);
            return mf.PACKET;
        }

        /// <summary>
        /// Generate GetLBTParamsCmd message
        /// </summary>
        /// <param name="include_timestamp"></param>
        /// <returns></returns>
        public static byte[] GENERATE_GET_LBT_PARAMS_CMD(bool include_timestamp)
        {
            MACH1_FRAME mf = new MACH1_FRAME(CATEGORY.OPERATION_MODEL, OPERATION_CMD.GET_LBT_PARAMS, include_timestamp);
            return mf.PACKET;
        }

        /// <summary>
        /// Generate InventoryContinueCmd message
        /// </summary>
        /// <param name="include_timestamp"></param>
        /// <returns></returns>
        public static byte[] GENERATE_INVENTORY_CONTINUE_CMD(bool include_timestamp)
        {
            MACH1_FRAME mf = new MACH1_FRAME(CATEGORY.OPERATION_MODEL, OPERATION_CMD.INVENTORY_CONTINUE, include_timestamp);
            return mf.PACKET;
        }

        /// <summary>
        /// Generate SetRxConfigCmd message
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="sensitivities"></param>
        /// <param name="include_timestamp"></param>
        /// <returns></returns>
        public static byte[] GENERATE_SET_RX_CONFIG_CMD(SET_RX_SENSITIVITY_MODE mode, short[] sensitivities, bool include_timestamp)
        {
            byte[] data;
            if (mode == SET_RX_SENSITIVITY_MODE.MAXIMUM_SENSITIVITY)
            {
                data = new byte[1];
                data[0] = (byte)SET_RX_SENSITIVITY_MODE.MAXIMUM_SENSITIVITY;
            }
            else 
            {
                data = new byte[4 + sensitivities.Length];
                data[0] = (byte)SET_RX_SENSITIVITY_MODE.FIXED_PER_ANTENNA;
                data[1] = 0x01;
                data[2] = (byte)((sensitivities.Length & 0xFF00) >> 8);
                data[3] = (byte)(sensitivities.Length & 0x00FF);
                for (int i = 0; i < sensitivities.Length; i++)
                {
                    data[4 + i] = (byte)(((0 - sensitivities[i]) - 1) ^ 0xFF); 
                }
            }

            MACH1_FRAME mf = new MACH1_FRAME(CATEGORY.OPERATION_MODEL, OPERATION_CMD.SET_RX_CONFIG, include_timestamp, data);
            return mf.PACKET;
        }

        /// <summary>
        /// Generate GetRxConfigCmd message
        /// </summary>
        /// <param name="include_timestamp"></param>
        /// <returns></returns>
        public static byte[] GENERATE_GET_RX_CONFIG_CMD(bool include_timestamp)
        {
            MACH1_FRAME mf = new MACH1_FRAME(CATEGORY.OPERATION_MODEL, OPERATION_CMD.GET_RX_CONFIG, include_timestamp);
            return mf.PACKET;
        }

        /// <summary>
        /// Generate ModemStopCmd message
        /// </summary>
        /// <param name="include_timestamp"></param>
        /// <returns></returns>
        public static byte[] GENERATE_MODEM_STOP_CMD(bool include_timestamp)
        {
            MACH1_FRAME mf = new MACH1_FRAME(CATEGORY.OPERATION_MODEL, OPERATION_CMD.MODEM_STOP, include_timestamp);
            return mf.PACKET;
        }

        /// <summary>
        /// Generate RFSurveyCmd message
        /// </summary>
        /// <param name="include_timestamp"></param>
        /// <param name="low_frequency_index"></param>
        /// <param name="high_frequency_index"></param>
        /// <param name="bw"></param>
        /// <param name="antennas"></param>
        /// <param name="sampleCount"></param>
        /// <returns></returns>
        public static byte[] GENERATE_RF_SURVEY_CMD(bool include_timestamp, 
            UInt16 low_frequency_index, 
            UInt16 high_frequency_index, 
            MEASUREMENT_BANDWIDTH bw, 
            byte antennas,
            UInt16 sampleCount)
        {
            byte[] data = new byte[8];

            data[0] = (byte)(low_frequency_index >> 8);
            data[1] = (byte)(low_frequency_index & 0x00FF);

            data[2] = (byte)(high_frequency_index >> 8);
            data[3] = (byte)(high_frequency_index & 0x00FF);

            data[4] = (byte)bw;

            data[5] = antennas;

            data[6] = (byte)(sampleCount >> 8);
            data[7] = (byte)(sampleCount & 0x00FF);

            MACH1_FRAME mf = new MACH1_FRAME(CATEGORY.OPERATION_MODEL, OPERATION_CMD.RF_SURVEY, include_timestamp, data);

            return mf.PACKET;
        }

        /// <summary>
        /// Generate TagReadCmd message
        /// </summary>
        /// <param name="include_timestamp"></param>
        /// <param name="mb"></param>
        /// <param name="addr"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        public static byte[] GENERATE_TAG_READ_CMD(bool include_timestamp, MEMORY_BANK mb, UInt16 addr, byte len)
        {
            byte[] data = new byte[4];

            data[0] = (byte) mb;
            data[1] = (byte)(addr >> 8);
            data[2] = (byte)(addr & 0x00FF);
            data[3] = len;

            MACH1_FRAME mf = new MACH1_FRAME(CATEGORY.OPERATION_MODEL, OPERATION_CMD.TAG_READ, include_timestamp, data);
            return mf.PACKET;
        }

        /// <summary>
        /// Generate TagReadCmd message
        /// </summary>
        /// <param name="include_timestamp"></param>
        /// <param name="mb"></param>
        /// <param name="addr"></param>
        /// <param name="len"></param>
        /// <param name="pass"></param>
        /// <returns></returns>
        public static byte[] GENERATE_TAG_READ_CMD(bool include_timestamp, MEMORY_BANK mb, UInt16 addr, byte len, UInt32 pass)
        {
            byte[] data = new byte[9];

            data[0] = (byte)mb;
            data[1] = (byte)(addr >> 8);
            data[2] = (byte)(addr & 0x00FF);
            data[3] = len;
            data[4] = 0x01;
            data[5] = (byte)(pass >> 24);
            data[6] = (byte)((pass & 0x00FF0000)>>16);
            data[7] = (byte)((pass & 0x0000FF00)>> 8);
            data[8] = (byte)(pass & 0x000000FF);

            MACH1_FRAME mf = new MACH1_FRAME(CATEGORY.OPERATION_MODEL, OPERATION_CMD.TAG_READ, include_timestamp, data);
            return mf.PACKET;
        }

        /// <summary>
        /// Generate TagWriteCmd message
        /// </summary>
        /// <param name="include_timestamp"></param>
        /// <param name="mb"></param>
        /// <param name="addr"></param>
        /// <param name="data"></param>
        /// <param name="block_write"></param>
        /// <returns></returns>
        public static byte[] GENERATE_TAG_WRITE_CMD(bool include_timestamp, MEMORY_BANK mb, UInt16 addr, UInt16[] data, bool block_write)
        {
            int dataLen = 2*data.Length;
            byte[] temp = new byte[6 + dataLen];
            
            temp[0] = (byte) mb;
            temp[1] = (byte)(addr >> 8);
            temp[2] = (byte)(addr & 0x00FF);
            temp[3] = (byte)((dataLen & 0x0300)>>8);
            temp[4] = (byte)((dataLen) & 0x00FF);

            for (int i = 0; i < data.Length; i++)
            {
                temp[5 + 2 * i] = (byte)(data[i] >> 8);
                temp[6 + 2 * i] = (byte)(data[i] & 0x00FF);
            }

            temp[5 + dataLen] = (byte) (block_write ? 0 : 1);

            MACH1_FRAME mf = new MACH1_FRAME(CATEGORY.OPERATION_MODEL, OPERATION_CMD.TAG_WRITE, include_timestamp, temp);
            return mf.PACKET;

        }

        /// <summary>
        /// Generate TagWriteCmd message
        /// </summary>
        /// <param name="include_timestamp"></param>
        /// <param name="mb"></param>
        /// <param name="addr"></param>
        /// <param name="data"></param>
        /// <param name="block_write"></param>
        /// <param name="pass"></param>
        /// <returns></returns>
        public static byte[] GENERATE_TAG_WRITE_CMD(bool include_timestamp, MEMORY_BANK mb, UInt16 addr, UInt16[] data, bool block_write, UInt32 pass)
        {
            int dataLen = 2*data.Length;
            byte[] temp = new byte[11 + dataLen];

            temp[0] = (byte) mb;
            temp[1] = (byte)(addr >> 8);
            temp[2] = (byte)(addr & 0x00FF);
            temp[3] = (byte)((dataLen & 0x0300) >> 8);
            temp[4] = (byte)(dataLen & 0x00FF);
            for (int i = 0; i < data.Length; i++)
            {
                temp[5 + 2 * i] = (byte)(data[i] >> 8);
                temp[6 + 2 * i] = (byte)(data[i] & 0x00FF);
            }

            temp[5 + dataLen] = (byte)(block_write ? 0 : 1);

            temp[6 + dataLen] = 0x01;
            temp[7 + dataLen] = (byte)(pass >> 24);
            temp[8 + dataLen] = (byte)((pass & 0x00FF0000) >> 16);
            temp[9 + dataLen] = (byte)((pass & 0x0000FF00) >> 8);
            temp[10 + dataLen] = (byte)(pass & 0x000000FF);

            MACH1_FRAME mf = new MACH1_FRAME(CATEGORY.OPERATION_MODEL, OPERATION_CMD.TAG_WRITE, include_timestamp, temp);
            return mf.PACKET;
        }

        /// <summary>
        /// Generate TagLockCmd message
        /// </summary>
        /// <param name="include_timestamp"></param>
        /// <param name="tlo"></param>
        /// <returns></returns>
        public static byte[] GENERATE_TAG_LOCK_CMD(bool include_timestamp, OPERATION_CMD.TAG_LOCK_OPERATION tlo)
        {
            MACH1_FRAME mf = new MACH1_FRAME(CATEGORY.OPERATION_MODEL, OPERATION_CMD.TAG_LOCK, include_timestamp, tlo.ToByteArray());
            return mf.PACKET;
        }

        /// <summary>
        /// Generate TagLockCmd message
        /// </summary>
        /// <param name="include_timestamp"></param>
        /// <param name="tlo"></param>
        /// <param name="pass"></param>
        /// <returns></returns>
        public static byte[] GENERATE_TAG_LOCK_CMD(bool include_timestamp, OPERATION_CMD.TAG_LOCK_OPERATION tlo, UInt32 pass)
        {
            byte[] data = new byte[9];

            Array.Copy(tlo.ToByteArray(), data, 4);
            
            data[4] = 0x01;

            data[5] = (byte)(pass >> 24);
            data[6] = (byte)((pass & 0x00FF0000) >> 16);
            data[7] = (byte)((pass & 0x0000FF00) >> 8);
            data[8] = (byte)(pass & 0x000000FF);

            MACH1_FRAME mf = new MACH1_FRAME(CATEGORY.OPERATION_MODEL, OPERATION_CMD.TAG_LOCK, include_timestamp, data);
            return mf.PACKET;
        }

        /// <summary>
        /// Generate TagKillCmd message
        /// </summary>
        /// <param name="include_timestamp"></param>
        /// <param name="pass"></param>
        /// <returns></returns>
        public static byte[] GENERATE_TAG_KILL_CMD(bool include_timestamp, UInt32 pass)
        {
            byte[] data = new byte[4];
            data[0] = (byte)(pass >> 24);
            data[1] = (byte)((pass & 0x00FF0000) >> 16);
            data[2] = (byte)((pass & 0x0000FF00) >> 8);
            data[3] = (byte)(pass & 0x000000FF);

            MACH1_FRAME mf = new MACH1_FRAME(CATEGORY.OPERATION_MODEL, OPERATION_CMD.TAG_KILL, include_timestamp, data);
            return mf.PACKET;
        }

        /// <summary>
        /// Generate ReportInventoryCmd message
        /// </summary>
        /// <param name="include_timestamp"></param>
        /// <returns></returns>
        public static byte[] GENERATE_REPORT_INVENTORY_CMD(bool include_timestamp)
        {
            MACH1_FRAME mf = new MACH1_FRAME(CATEGORY.OPERATION_MODEL, OPERATION_CMD.REPORT_INVENTORY, include_timestamp);
            return mf.PACKET;
        }


        /// <summary>
        /// Generate SetProfileSequence command
        /// </summary>
        /// <param name="include_timestamp"></param>
        /// <param name="enable">Enable sequence, default should be false</param>
        /// <param name="sequence">sequence list, the maximum sequence number is 16. the length of the sequence should be equal to the length of the duration list</param>
        /// <param name="durations">duration list</param>
        /// <returns></returns>
        public static byte[] GENERATE_SET_PROFILE_SEQUENCE_CMD(bool include_timestamp, bool enabled, ArrayList sequence, ArrayList durations)
        {
            if (sequence.Count != durations.Count) return null;

            byte[] data;

            if(enabled) data = new byte[7+sequence.Count+durations.Count*2];
            else
                data = new byte[1];

            int idx = 0;
            data[idx++] = (byte)(enabled? 1 : 0);

            if (enabled)
            {
                data[idx++] = 0x01;
                data[idx++] = (byte)(sequence.Count >> 8);
                data[idx++] = (byte)(sequence.Count & 0x00FF);

                for (int i = 0; i < sequence.Count; i++ ) data[idx++] = (byte)sequence[i];

                data[idx++] = 0x02;
                data[idx++] = (byte)(durations.Count>>8);
                data[idx++] = (byte)(durations.Count & 0x00FF);

                for(int i=0; i<durations.Count; i++)
                {
                    data[idx++] = (byte)(((int)durations[i])>>8);
                    data[idx++] = (byte)(((int)durations[i]) & 0x00FF);
                }
            }
            return data;
        }

        #endregion
    }

    /// <summary>
    /// Mach1 operation notification set
    /// </summary>
    /// 
    [Serializable]
    public class OPERATION_NTF
    {
        public const byte ANTENNA_ALERT = 0x00;
        public const byte SET_REGULATORY_REGION = 0x0A;
        public const byte INVENTORY = 0x01;
        public const byte TAG_CUSTOM = 0x09;
        public const byte TAG_KILL = 0x08;
        public const byte TAG_LOCK = 0x07;
        public const byte TAG_WRITE = 0x06;
        public const byte TAG_READ = 0x05;
        public const byte RF_SURVEY = 0x03;
        public const byte MODEM_STOP = 0x04;
        public const byte INVENTORY_STATUS = 0x02;
        public const byte CHECK_ANTENNA = 0x0B;
        public const byte ACCUMULATION_STATUS = 0x0C;

        [Serializable]
        public class SET_REGULATORY_REGION_NTF : MACH1_NTF
        {
            public enum NTF_CODE
            {
                SUCCESS,
                NOT_VALID_REGULATORY_CALIBRATION,
                ERROR_SETTING
            }

            public NTF_CODE code;

        }

        [Serializable]
        public class MODEM_STOPPED_NTF : MACH1_NTF
        {
            public enum NTF_CODE
            {
                RESULT_OF_COMMAND,
                TO_MEET_REGULATORY,
                HARDWARE_ERROR
            }

            public NTF_CODE code;
        }

        public enum TAG_ACCESS_RESULT_CODE
        {
            SUCCEEDED,
            FAIL_NO_RESPONSE,
            FAIL_MEMORY_LOCKED,
            FAIL_MEMORY_OVERRUN,
            FAIL_INSUFFICIENT_POWER,
            FAIL_INVALID_PASSWORD,
            FAIL_OTHER_TAG_ERROR,
            FAIL_TAG_LOST,
            FAIL_READER_ERROR
        }

        public enum ANTENNA_STATUS
        {
            READY = 1,
            DISCONNECTED = 3
        }

        [Serializable]
        public class ACCUMULATION_STATUS_NTF : MACH1_NTF
        {
            public enum NTF_CODE
            {
                BUFFER_EMPTIED = 0,
                BUFFER_FILLED
            }

            public NTF_CODE code;
        }


        [Serializable]
        public class ANTENNA_ALERT_NTF : MACH1_NTF
        {
            public OPERATION_CMD.ANTENNA antenna;
            public ANTENNA_STATUS antenna_status;
            
            public ANTENNA_ALERT_NTF(byte[] data)
            {
                try
                {
                    antenna = (OPERATION_CMD.ANTENNA)data[0];
                    antenna_status = (ANTENNA_STATUS)data[1];
                }
                catch
                {
                }
            }
        }

        [Serializable]
        public class CHECK_ANTENNA_NTF : MACH1_NTF
        {
            public ANTENNA_STATUS[] antenna_status;
            public CHECK_ANTENNA_NTF(byte[] data)
            {
                try
                {
                    int len = data.Length;
                    antenna_status = new ANTENNA_STATUS[len];
                    for (int i = 0; i < len; i++)
                        antenna_status[i] = (ANTENNA_STATUS)data[i];
                }
                catch { }
            }
        }

        [Serializable]
        public class INVENTORY_NTF : MACH1_NTF
        {
            public enum GEN2_READ_RESULT_CODE
            {
                READ_SUCESS = 0,
                NO_RESPONSE,
                CRC_ERROR,
                MEMORY_LOCKED,
                MEMORY_OVERRUN,
                OTHER_TAG_ERROR,
                OTHER_READER_ERROR
            }
            public enum ANTENNA
            {
                ANTENNA1=1,
                ANTENNA2=2,
                ANTENNA3=4,
                ANTENNA4=8
            }
            public enum ACCUMULATION_STATUS
            {
                INVENOTRY_NTF_BYPASSED = 0,
                INVENTORY_NTF_ADDED,
                INVENTORY_NTF_REMOVED,
                INVENTORY_NTF_OVERFLOWED
            }

            public byte[] epc;
            public bool halted;
            public short rssi;
            public byte[] gen2PC = new byte[2];
            public byte[] epcCrc = new byte[2];
            public GEN2_READ_RESULT_CODE gen2_read_result_code;
            public byte[] gen2ReadData;
            public UInt16 gen2Handle;
            public ANTENNA antenna;
            public UInt16 read_count;
            public UInt32 first_seen_ago;
            public UInt32 last_seen_ago;
            public ACCUMULATION_STATUS accumulation_status;
            public short phaseI;
            public short phaseQ;

            public string EPC
            {
                get
                {
                    string epc_code = string.Empty;
                    for (int i = 0; i < epc.Length; i++) epc_code += epc[i].ToString("X2");

                    return epc_code;
                }

            }

            public INVENTORY_NTF()
            {
            }

            public INVENTORY_NTF(byte[] data)
            {
                try
                {
                    int offset = 1;
                    epc = new byte[data[offset]];
                    offset++;
                    Array.Copy(data, offset, epc, 0, data[1]);

                    offset += data[1];
                    halted = (data[offset] == 1) ? true : false;

                    offset++;

                    rssi = (short)(0 - ((data[offset] ^ 0xFF) + 1));

                    offset++;
                    gen2PC[0] = data[offset++]; gen2PC[1] = data[offset++];
                    epcCrc[0] = data[offset++]; epcCrc[1] = data[offset++];

                    if ((epcCrc[0] & 0x80) != 0) phaseI = (short)(0 - ((epcCrc[0] ^ 0xFF) + 1));
                    else
                        phaseI = epcCrc[0];

                    if ((epcCrc[1] & 0x80) != 0) phaseQ = (short)(0 - ((epcCrc[1] ^ 0xFF) + 1));
                    else
                        phaseQ = epcCrc[1];

                    //phaseI = epcCrc[0];
                    //phaseQ = epcCrc[1];

                    while (offset < data.Length)
                    {
                        switch (data[offset++])
                        {
                            case 0x01:
                                gen2_read_result_code = (GEN2_READ_RESULT_CODE)data[offset++];
                                break;
                            case 0x02:
                                int gen2dataLen = data[offset++]*0x0100 + data[offset++];   //@derek
                                gen2ReadData = new byte[gen2dataLen];
                                Array.Copy(data, offset, gen2ReadData, 0, gen2dataLen);
                                offset += gen2dataLen;
                                break;
                            case 0x03:
                                gen2Handle = (ushort)(data[offset++] * 256 + data[offset++]);
                                break;
                            case 0x04:
                                antenna = (ANTENNA)data[offset++];
                                break;
                            case 0x05:
                                read_count = (UInt16)(data[offset++] * 256 + data[offset++]);
                                break;
                            case 0x06:
                                first_seen_ago = (UInt32)((data[offset++] << 24) + (data[offset++] << 16) + (data[offset++] << 8) + data[offset++]);
                                break;
                            case 0x07:
                                last_seen_ago = (UInt32)((data[offset++] << 24) + (data[offset++] << 16) + (data[offset++] << 8) + data[offset++]);
                                break;
                            case 0x08:
                                accumulation_status = (ACCUMULATION_STATUS)data[offset++];
                                break;
                            default:
                                offset++;
                                break;
                        }
                    }
                }
                catch
                {
                }
            }
        }

        [Serializable]
        public class INVENTORY_STATUS_NTF : MACH1_NTF
        {
            public bool transmitter_status_enabled = false;
            public byte enabled_antenna_byte = 0x00;
            public UInt16 center_frequncy_index = 0;
            public UInt16 inventory_attempt_count = 0;

            public INVENTORY_STATUS_NTF(byte[] data)
            {
                transmitter_status_enabled = (data[0] == 1) ? true : false;

                try
                {
                    int offset = 1;
                    while (offset < data.Length-1)
                    {
                        switch (data[offset])
                        {
                            case 0x01:
                                offset++;
                                enabled_antenna_byte = data[offset];
                                offset++;
                                break;
                            case 0x02:
                                offset ++;
                                center_frequncy_index = (UInt16)(data[offset] * 256 + data[offset + 1]);
                                offset += 2;
                                break;
                            case 0x03:
                                offset++;
                                inventory_attempt_count = (UInt16)(data[offset] * 256 + data[offset + 1]);
                                offset += 2;
                                break;
                            default:
                                offset++;
                                break;
                        }
                    }
                }
                catch { }

            }
        }

        [Serializable]
        public class RF_SURVEY_NTF : MACH1_NTF
        {
            public enum RESULT_CODE
            {
                SURVEY_IN_PROGRESS,
                SUCCESS,
                ABORT
            }

            public RESULT_CODE result_code;
            public UInt16 low_frequency_index;
            public UInt16 high_frequency_index;
            public byte bandwidth_index;
            public byte antenna_byte;
            public UInt32 low_time;
            public UInt32 high_time;
            public Int16[] rssi_data;

            public RF_SURVEY_NTF(byte[] data)
            {
                try
                {
                    result_code = (RESULT_CODE)data[0];
                    low_frequency_index = (UInt16)(data[1] * 256 + data[2]);
                    high_frequency_index = (UInt16)(data[3] * 256 + data[4]);

                    bandwidth_index = data[5];
                    antenna_byte = data[6];

                    low_time = ((UInt32)data[7] << 24) & ((UInt32)data[8] << 16) & ((UInt32)data[9] << 8) & ((UInt32)data[10]);
                    high_time = ((UInt32)data[11] << 24) & ((UInt32)data[12] << 16) & ((UInt32)data[13] << 8) & ((UInt32)data[14]);

                    if (data.Length > 17)
                    {
                        if (data[15] == 0x01)
                        {
                            int rssi_data_len = (data[16] << 8) + data[17];

                            rssi_data = new short[rssi_data_len];

                            for (int i = 0; i < rssi_data_len; i++)
                            {
                                rssi_data[i] = (short)(0 - ((data[18 + i] ^ 0xFF) + 1));
                            }

                        }
                    }
                }
                catch
                {
                }
                
            }

        }

        [Serializable]
        public class TAG_READ_NTF : MACH1_NTF
        {
            public enum RESULT_CODE
            {
                READ_SUCCEEDED,
                NO_RESPONSE,
                CRC_ERROR,
                MEMORY_LOCKED,
                MEMORY_OVERRUN,
                INVALID_PASSWORD,
                OTHER_TAG_ERROR,
                TAG_LOST,
                READER_ERROR
            }

            public RESULT_CODE result_code;
            public byte[] data;

            public TAG_READ_NTF(byte[] data)
            {
                try
                {
                    result_code = (RESULT_CODE)data[0];
                    if (data.Length > 4)
                    {
                        int len = data[2] * 256 + data[3];
                        this.data = new byte[len];

                        Array.Copy(data, 4, this.data, 0, len);
                    }
                }
                catch
                {
                }
            }
        }

        [Serializable]
        public class TAG_WRITE_NTF : MACH1_NTF
        {
            public TAG_ACCESS_RESULT_CODE result_code;
            public UInt16 err_addr;

            public TAG_WRITE_NTF(byte[] data)
            {
                try
                {
                    result_code = (TAG_ACCESS_RESULT_CODE)data[0];
                    if(data.Length>4)err_addr = (UInt16)(data[2] * 256 + data[3]);
                }
                catch { }
            }
        }

    }
}
