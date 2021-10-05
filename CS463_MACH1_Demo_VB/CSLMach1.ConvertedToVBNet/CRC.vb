
'*
' /******************************************************************************
' **
' ** @file crc.cs
' **
' ** Last update: Jan 24, 2008
' **
' ** This file provides CRC generation and varification methods.
' **
' *****************************************************************************


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
	''' <summary>
	''' CRC Calculation Class. For more information please refer to Speedway Reference Design - Mach1 Overview
	''' </summary>
	Public Class CRC
		Private Shared API_CrcTable As Byte() = New Byte() {&H0, &H31, &H62, &H53, &Hc4, &Hf5, _
			&Ha6, &H97, &Hb9, &H88, &Hdb, &Hea, _
			&H7d, &H4c, &H1f, &H2e, &H43, &H72, _
			&H21, &H10, &H87, &Hb6, &He5, &Hd4, _
			&Hfa, &Hcb, &H98, &Ha9, &H3e, &Hf, _
			&H5c, &H6d, &H86, &Hb7, &He4, &Hd5, _
			&H42, &H73, &H20, &H11, &H3f, &He, _
			&H5d, &H6c, &Hfb, &Hca, &H99, &Ha8, _
			&Hc5, &Hf4, &Ha7, &H96, &H1, &H30, _
			&H63, &H52, &H7c, &H4d, &H1e, &H2f, _
			&Hb8, &H89, &Hda, &Heb, &H3d, &Hc, _
			&H5f, &H6e, &Hf9, &Hc8, &H9b, &Haa, _
			&H84, &Hb5, &He6, &Hd7, &H40, &H71, _
			&H22, &H13, &H7e, &H4f, &H1c, &H2d, _
			&Hba, &H8b, &Hd8, &He9, &Hc7, &Hf6, _
			&Ha5, &H94, &H3, &H32, &H61, &H50, _
			&Hbb, &H8a, &Hd9, &He8, &H7f, &H4e, _
			&H1d, &H2c, &H2, &H33, &H60, &H51, _
			&Hc6, &Hf7, &Ha4, &H95, &Hf8, &Hc9, _
			&H9a, &Hab, &H3c, &Hd, &H5e, &H6f, _
			&H41, &H70, &H23, &H12, &H85, &Hb4, _
			&He7, &Hd6, &H7a, &H4b, &H18, &H29, _
			&Hbe, &H8f, &Hdc, &Hed, &Hc3, &Hf2, _
			&Ha1, &H90, &H7, &H36, &H65, &H54, _
			&H39, &H8, &H5b, &H6a, &Hfd, &Hcc, _
			&H9f, &Hae, &H80, &Hb1, &He2, &Hd3, _
			&H44, &H75, &H26, &H17, &Hfc, &Hcd, _
			&H9e, &Haf, &H38, &H9, &H5a, &H6b, _
			&H45, &H74, &H27, &H16, &H81, &Hb0, _
			&He3, &Hd2, &Hbf, &H8e, &Hdd, &Hec, _
			&H7b, &H4a, &H19, &H28, &H6, &H37, _
			&H64, &H55, &Hc2, &Hf3, &Ha0, &H91, _
			&H47, &H76, &H25, &H14, &H83, &Hb2, _
			&He1, &Hd0, &Hfe, &Hcf, &H9c, &Had, _
			&H3a, &Hb, &H58, &H69, &H4, &H35, _
			&H66, &H57, &Hc0, &Hf1, &Ha2, &H93, _
			&Hbd, &H8c, &Hdf, &Hee, &H79, &H48, _
			&H1b, &H2a, &Hc1, &Hf0, &Ha3, &H92, _
			&H5, &H34, &H67, &H56, &H78, &H49, _
			&H1a, &H2b, &Hbc, &H8d, &Hde, &Hef, _
			&H82, &Hb3, &He0, &Hd1, &H46, &H77, _
			&H24, &H15, &H3b, &Ha, &H59, &H68, _
			&Hff, &Hce, &H9d, &Hac}

		''' <summary>
		''' Generate CRC based on Mach1 frame
		''' </summary>
		''' <param name="data">Mach1 frame data (byte array)</param>
		''' <param name="len">lenth of the frame</param>
		''' <returns></returns>
		Public Shared Function CalculateCRC(data As Byte(), len As Integer) As Byte
			Dim crc As Byte = &H0
			For i As Integer = 0 To len - 1
				crc = API_CrcTable((crc Xor data(i)) And &Hff)
			Next

			Return crc
		End Function

		''' <summary>
		''' Verify CRC
		''' </summary>
		''' <param name="data">Mach1 frame data (byte array)</param>
		''' <returns></returns>
		Public Shared Function VerifyCRC(data As Byte()) As Boolean
			Try
				Dim crc As Byte = CalculateCRC(data, data.Length - 1)

				If crc <> data(data.Length - 1) Then
					Return False
				End If
				Return True
			Catch
				Return False
			End Try
		End Function
	End Class
End Namespace
