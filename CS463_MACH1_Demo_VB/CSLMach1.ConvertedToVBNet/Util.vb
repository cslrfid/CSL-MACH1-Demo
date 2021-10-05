'*****************************************************************************
'**
'** @file Util.cs
'**
'** Last update: Jan 24, 2008
'**
'** This file provides data conversion methods
'*****************************************************************************



'
' *****************************************************************************
'
' * IMPINJ EXPRESSLY DISCLAIMS ANY AND ALL WARRANTIES CONCERNING THIS SOFTWARE AND 
' * DOCUMENTATION, INCLUDING ANY WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR 
' * ANY PARTICULAR PURPOSE, AND WARRANTIES OF PERFORMANCE, AND ANY WARRANTY THAT 
' * MIGHT OTHERWISE ARISE FROM COURSE OF DEALING OR USAGE OF TRADE. NO WARRANTY IS 
' * EITHER EXPRESS OR IMPLIED WITH RESPECT TO THE USE OF THE SOFTWARE OR DOCUMENTATION. 
' * Under no circumstances shall Impinj be liable for incidental, special, indirect, 
' * direct or consequential damages or loss of profits, interruption of business, 
' * or related expenses which may arise from use of software or documentation, 
' * including but not limited to those resulting from defects in software and/or 
' * documentation, or loss or inaccuracy of data of any kind.
' * 
' * (c) Copyright Impinj, Inc. 2007. All rights reserved.  
'
' ****************************************************************************


Imports System.Collections.Generic
Imports System.Text

Namespace CSL
	Public Class Util

		''' <summary>
		''' Convert a binary string to a byte array. if the length of binary string can not be 
		''' devided by 8, the least important port of last byte will be appended zero as many 
		''' as need.  
		''' </summary>
		''' <param name="binaryString">binary string to be converted. e.g. "0101100100100"</param>
		''' <param name="mask_len">not used</param>
		''' <returns></returns>
		Public Shared Function ConvertBinaryStringArrayToBytes(binaryString As String, mask_len As Integer) As Byte()
			Try
				Dim reserved As Integer = 0

				Dim len As Long = Math.DivRem(binaryString.Length, 8, reserved)

				Dim pad As String = ""
				If reserved <> 0 Then
					pad = pad.PadRight(8 - reserved, "0"C)
					binaryString += pad
					len += 1
				End If

				Dim data As Byte() = New Byte(len - 1) {}
				For i As Integer = 0 To len - 1
					Dim s As String = binaryString.Substring(i * 8, 8)
					data(i) = Convert.ToByte(s, 2)
				Next

				Return data
			Catch
				Return Nothing
			End Try
		End Function
	End Class
End Namespace
