/******************************************************************************
**
** @file Util.cs
**
** Last update: Jan 24, 2008
**
** This file provides data conversion methods
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

namespace CSL
{
    public class Util
    {      
        
        /// <summary>
        /// Convert a binary string to a byte array. if the length of binary string can not be 
        /// devided by 8, the least important port of last byte will be appended zero as many 
        /// as need.  
        /// </summary>
        /// <param name="binaryString">binary string to be converted. e.g. "0101100100100"</param>
        /// <param name="mask_len">not used</param>
        /// <returns></returns>
        public static byte[] ConvertBinaryStringArrayToBytes(string binaryString, int mask_len)
        {
            try
            {
                int reserved = 0;

                long len = Math.DivRem(binaryString.Length, 8, out reserved);

                string pad = "";
                if (reserved != 0)
                {
                    pad = pad.PadRight(8-reserved, '0');
                    binaryString += pad;
                    len++;
                }

                byte[] data = new byte[len];
                for (int i = 0; i < len; i++)
                {
                    string s = binaryString.Substring(i * 8, 8);
                    data[i] = Convert.ToByte(s, 2);
                }

                return data;
            }
            catch
            {
                return null;
            }
        }
    }
}
