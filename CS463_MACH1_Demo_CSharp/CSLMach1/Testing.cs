/******************************************************************************
**
** @file testing.cs
**
** Last update: Jan 24, 2008
**
** This file provides TCS command set definition and implementation.
** Currently only definitions are provided
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

    public class TEST_CMD_SET
    {
        public const byte GET_TCS_VERSION = 0x00;
        public const byte GET_VIRTUAL_PAGE_VERSION = 0x01;
        public const byte TEST_READ = 0x02;
        public const byte TEST_WRITE = 0x03;

        public static byte[] GENERATE_WRITE_CMD_DATA(byte memory_space, UInt32 addr, short[] data, bool include_timestamp)
        {
            int len = 8 + data.Length;

            byte[] temp = new byte[len];
            temp[0] = memory_space;

            temp[1] = (byte)(addr >> 24);
            temp[2] = (byte)(addr >> 16);
            temp[3] = (byte)(addr >> 8);
            temp[4] = (byte)(addr & 0x000000FF);

            temp[5] = (byte)((data.Length*2 & 0xFF00) >> 8);
            temp[6] = (byte)(data.Length*2 & 0x00FF);
            for (int i = 0; i < data.Length; i++)
            {
                temp[7 + 2*i] = (byte)(data[i]>>8);
                temp[8 + 2*i] = (byte)(data[i]&0x00FF);
            }

            MACH1_FRAME mf = new MACH1_FRAME(CATEGORY.TEST, TEST_WRITE, include_timestamp, temp);

            return mf.PACKET;
        }

        public static byte[] GENERATE_READ_CMD_DATA(byte memory_space, UInt32 addr, ushort length, bool include_timestamp)
        {
            byte[] temp = new byte[6];
            temp[0] = memory_space;

            temp[1] = (byte)(addr >> 24);
            temp[2] = (byte)(addr >> 16);
            temp[3] = (byte)(addr >> 8);
            temp[4] = (byte)(addr & 0x000000FF);

            temp[5] = (byte)length;

            MACH1_FRAME mf = new MACH1_FRAME(CATEGORY.TEST, TEST_WRITE, include_timestamp, temp);
            return mf.PACKET;
        }

    }

    public class TEST_NTF_SET
    {
        public const byte TEST_READ = 0x00;
        public const byte TEST_WRITE = 0x01;
    }
}
