'*****************************************************************************
'**
'** @file testing.cs
'**
'** Last update: Jan 24, 2008
'**
'** This file provides TCS command set definition and implementation.
'** Currently only definitions are provided
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

Namespace CSL.Mach1

	Public Class TEST_CMD_SET
		Public Const GET_TCS_VERSION As Byte = &H0
		Public Const GET_VIRTUAL_PAGE_VERSION As Byte = &H1
		Public Const TEST_READ As Byte = &H2
		Public Const TEST_WRITE As Byte = &H3

		Public Shared Function GENERATE_WRITE_CMD_DATA(memory_space As Byte, addr As UInt32, data As Short(), include_timestamp As Boolean) As Byte()
			Dim len As Integer = 8 + data.Length

			Dim temp As Byte() = New Byte(len - 1) {}
			temp(0) = memory_space

			temp(1) = CByte(addr >> 24)
			temp(2) = CByte(addr >> 16)
			temp(3) = CByte(addr >> 8)
			temp(4) = CByte(addr And &Hff)

			temp(5) = CByte((data.Length * 2 And &Hff00) >> 8)
			temp(6) = CByte(data.Length * 2 And &Hff)
			For i As Integer = 0 To data.Length - 1
				temp(7 + 2 * i) = CByte(data(i) >> 8)
				temp(8 + 2 * i) = CByte(data(i) And &Hff)
			Next

			Dim mf As New MACH1_FRAME(CATEGORY.TEST, TEST_WRITE, include_timestamp, temp)

			Return mf.PACKET
		End Function

		Public Shared Function GENERATE_READ_CMD_DATA(memory_space As Byte, addr As UInt32, length As UShort, include_timestamp As Boolean) As Byte()
			Dim temp As Byte() = New Byte(5) {}
			temp(0) = memory_space

			temp(1) = CByte(addr >> 24)
			temp(2) = CByte(addr >> 16)
			temp(3) = CByte(addr >> 8)
			temp(4) = CByte(addr And &Hff)

			temp(5) = CByte(length)

			Dim mf As New MACH1_FRAME(CATEGORY.TEST, TEST_WRITE, include_timestamp, temp)
			Return mf.PACKET
		End Function

	End Class

	Public Class TEST_NTF_SET
		Public Const TEST_READ As Byte = &H0
		Public Const TEST_WRITE As Byte = &H1
	End Class
End Namespace
