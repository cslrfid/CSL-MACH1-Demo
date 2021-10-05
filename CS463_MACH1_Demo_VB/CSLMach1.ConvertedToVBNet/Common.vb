
'*
' ******************************************************************************
' **
' ** @file common.cs
' **
' ** Last update: Jan 24, 2008
' **
' ** This file provides the Mach1 Frame classes.
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
	''' General definition
	''' </summary>
	''' 
	<Serializable> _
	Public Class GENERAL_DEFINITION
		Public Const RESERVED As Boolean = False

		Public Const SOF_CMD As Byte = &Hee
		'Start of Frame, Command
		Public Const SOF_RSP As Byte = &Hef
		'Start of Frame, Response
	End Class

	''' <summary>
	''' Regulatory regions
	''' </summary>
	Public Enum REGULATORY_REGION
		FCC_PART_15_247 = 0
		ESTI_EN_300_220
		ETSI_EN_302_208_WLBT
		HK_920_925
		TAIWAN_922_928
		JAPAN_952_954
		JAPAN_952_955
		ETSI_EN_302_208_OLBT
		KOREA_910_914
		MALAYSIA_919_923
		CHINA_920_925

	End Enum

	''' <summary>
	''' Modem state
	''' </summary>
	Public Enum MODEM_STATE
		OFF = 0
		NA
		'fixed bug, reported by Ron. 12/10/07
		INIT
		IDLE
		ACTIVE
		HALT
		TEST
		ACCESS
	End Enum

	''' <summary>
	''' System errors
	''' </summary>
	Public Enum SYSTEM_ERROR_REASON
		MODEM_WATCHDOG_EXPIRED = 0
		MACH1_FUP_TIMEOUT
		OTHER_SYSTEM_ERR
	End Enum

	''' <summary>
	''' Socket connection status
	''' </summary>
	Public Enum SOCKET_STATUS
		CONNECTION_SUCCESS = 0
		CONNECTION_FAILED
	End Enum

	''' <summary>
	''' Commmand Category
	''' </summary>
	Public Enum CATEGORY
		MACH1_PROTOCOL_ERROR = 0
		OPERATION_MODEL
		MODEM_MANAGEMENT
		HARDWARE_INTERFACE
		PRODUCTION
		LOGGING
		TEST
	End Enum


	''' <summary>
	''' Invalid command reason codes
	''' </summary>
	Public Enum INVALID_CMD_REASON_CODE
		INVALID_CATEGORY = 0
		INVALID_CRC
		INVALID_MID
		INVALID_OTHER_HEADER
		INVALID_MODEM_STATE
		PARAMS_OUT_RANGE
		MISSING_CMD_PARAMS
		INVALID_OPT_PARAM_ID
		OTHER_PARAM_ERR
		OUT_OF_SYNC
		CMD_IN_PROGRESS
	End Enum

	''' <summary>
	''' Mach1 Notification Class
	''' </summary>
	''' 
	<Serializable> _
	Public MustInherit Class MACH1_NTF
		Public timestamp_second As UInt32
		'Timestamp: total seconds since epoch (12:00:00AM, 1/1/1970)
		Public timestamp_us As UInt32
		'Timestamp: micro seconds
		Public reader_name As String
		'reader name
	End Class

	''' <summary>
	''' Mach1 Frame
	''' </summary>
	''' 
	<Serializable> _
	Public Class MACH1_FRAME

		''' <summary>
		''' Mach Header Abstract Class
		''' </summary>
		''' 
		<Serializable> _
		Public MustInherit Class HEADER
			Public Const bit_15 As Boolean = GENERAL_DEFINITION.RESERVED
			Public timestamp_included As Boolean = False
			Public is_ntf As Boolean = False
			Public bit_12 As Boolean = GENERAL_DEFINITION.RESERVED

			Public category As CATEGORY
			Public message_id As Byte

			Public Function ToByteArray() As Byte()
				Dim h_byte As Integer = 0

				Dim ts As Integer = If(timestamp_included, &H1, &H0)
				Dim rn As Integer = If(is_ntf, &H1, &H0)

				ts = ts << 6
				rn = rn << 5

				Dim ct As Integer = CInt(category)
				h_byte = ts Or rn Or ct

				Dim data As Byte() = New Byte(1) {}
				data(0) = CByte(h_byte)
				data(1) = message_id

				Return data
			End Function
		End Class

		''' <summary>
		''' Mach1 Header Reader -> Host
		''' </summary>
		''' 
		<Serializable> _
		Public Class HEADER_READER_HOST
			Inherits HEADER

			''' <summary>
			''' Constructor for generate header based on byte array
			''' </summary>
			''' <param name="data">Header data</param>
			Public Sub New(data As Byte())
				If data.Length <> 2 Then
					Throw New Exception("The input is not a valid Header")
				End If

				timestamp_included = If((data(0) And &H40) = 0, False, True)
				is_ntf = If((data(0) And &H20) = 0, False, True)
				category = CType(data(0) And &Hf, CATEGORY)
				message_id = data(1)
			End Sub
		End Class

		''' <summary>
		''' Mach1 Header Host -> Reader
		''' </summary>
		''' 
		<Serializable> _
		Public Class HEADER_HOST_READER
			Inherits HEADER
			''' <summary>
			''' Constructor for generate header based on Catogory and Message ID
			''' </summary>
			''' <param name="include_timestamp"></param>
			''' <param name="category">Command category, defined in enum CATEGORY</param>
			''' <param name="message_id">Message id</param>
			Public Sub New(include_timestamp As Boolean, category As CATEGORY, message_id As Byte)
				Me.timestamp_included = include_timestamp
				Me.category = category
				Me.message_id = message_id
				Me.is_ntf = False
			End Sub

		End Class

        Public packet_header As HEADER
        Public packet_len As UInt16 = 6
		'minimum packet length
		Public payload_len As UInt16 = 0

		Public timestamp_second As UInt32
		Public timestamp_us As UInt32

		Private m_packet As Byte()
		Private m_payload As Byte()

		''' <summary>
		''' For more information, refer to Speedway Reference Design - Mach1 Overview
		''' </summary>
		''' <param name="category"></param>
		''' <param name="cmd"></param>
		''' <param name="include_timestamp"></param>
		Private Sub CreateFrameWithoutPayload(category As CATEGORY, cmd As Byte, include_timestamp As Boolean)
			Try
                packet_header = New HEADER_HOST_READER(include_timestamp, category, cmd)

                Dim dt As Long = DateTime.Now.ToUniversalTime().ToFileTimeUtc()
				timestamp_second = CType(dt >> 32, UInt32)
				timestamp_us = CType(dt And CType(&HffffffffUI, Int64), UInt32)

				If include_timestamp Then
					packet_len += 8
				End If
				m_packet = New Byte(packet_len - 1) {}

				m_packet(0) = GENERAL_DEFINITION.SOF_CMD
                Array.Copy(packet_header.ToByteArray(), 0, m_packet, 1, 2)

                m_packet(4) = &H0
				m_packet(3) = &H0

				If include_timestamp Then
					m_packet(5) = CByte(timestamp_second >> 24)
					m_packet(6) = CByte((timestamp_second And &Hff0000) >> 16)
					m_packet(7) = CByte((timestamp_second And &Hff00) >> 8)
					m_packet(8) = CByte((timestamp_second And &Hff))


					m_packet(9) = CByte(timestamp_us >> 24)
					m_packet(10) = CByte((timestamp_us And &Hff0000) >> 16)
					m_packet(11) = CByte((timestamp_us And &Hff00) >> 8)

					m_packet(12) = CByte((timestamp_us And &Hff))
				Else
				End If

				m_packet(packet_len - 1) = CRC.CalculateCRC(m_packet, packet_len - 1)
			Catch
			End Try
			

		End Sub

		''' <summary>
		''' For more information, refer to Speedway Reference Design - Mach1 Overview
		''' </summary>
		''' <param name="category"></param>
		''' <param name="cmd"></param>
		''' <param name="include_timestamp"></param>
		''' <param name="data"></param>
		Private Sub CreateFrameWithPayload(category As CATEGORY, cmd As Byte, include_timestamp As Boolean, data As Byte())
			Try
                packet_header = New HEADER_HOST_READER(include_timestamp, category, cmd)

                m_payload = New Byte(data.Length - 1) {}
				Array.Copy(data, m_payload, data.Length)

				Dim dt As Long = DateTime.Now.ToUniversalTime().ToFileTimeUtc()
				timestamp_second = CType(dt >> 32, UInt32)
				timestamp_us = CType(dt And CType(&HffffffffUI, Int64), UInt32)

				If include_timestamp Then
					packet_len += 8
				End If

				payload_len = CType(data.Length, UInt16)
				packet_len += payload_len

				m_packet = New Byte(packet_len - 1) {}

				m_packet(0) = GENERAL_DEFINITION.SOF_CMD
                Array.Copy(packet_header.ToByteArray(), 0, m_packet, 1, 2)

                m_packet(4) = CByte(payload_len And &Hff)
				m_packet(3) = CByte(payload_len >> 8)

				If include_timestamp Then
					m_packet(5) = CByte(timestamp_second >> 24)
					m_packet(6) = CByte((timestamp_second And &Hffff00000000L) >> 16)
					m_packet(7) = CByte((timestamp_second And &Hffff0000UI) >> 8)
					m_packet(8) = CByte((timestamp_second And &Hffff))


					m_packet(9) = CByte(timestamp_us >> 24)
					m_packet(10) = CByte((timestamp_us And &Hffff00000000L) >> 16)
					m_packet(11) = CByte((timestamp_us And &Hffff0000UI) >> 8)
					m_packet(12) = CByte((timestamp_us And &Hffff))

					Array.Copy(data, 0, m_packet, 13, data.Length)
				Else
					Array.Copy(data, 0, m_packet, 5, data.Length)
				End If

				m_packet(packet_len - 1) = CRC.CalculateCRC(m_packet, packet_len - 1)
			Catch
			End Try
			

		End Sub

		Public Sub New()
		End Sub

		''' <summary>
		''' Constructor for building command without command data 
		''' </summary>
		''' <param name="category"></param>
		''' <param name="cmd"></param>
		''' <param name="include_timestamp"></param>
		Public Sub New(category As CATEGORY, cmd As Byte, include_timestamp As Boolean)
			CreateFrameWithoutPayload(category, cmd, include_timestamp)
		End Sub

		''' <summary>
		''' Constructor for building command with command data
		''' </summary>
		''' <param name="category"></param>
		''' <param name="cmd"></param>
		''' <param name="include_timestamp"></param>
		''' <param name="data"></param>
		Public Sub New(category As CATEGORY, cmd As Byte, include_timestamp As Boolean, data As Byte())
			If data IsNot Nothing Then
				CreateFrameWithPayload(category, cmd, include_timestamp, data)
			Else
				CreateFrameWithoutPayload(category, cmd, include_timestamp)
			End If
		End Sub

		''' <summary>
		''' Constructure for parsing received packet 
		''' </summary>
		''' <param name="data"></param>
		Public Sub New(data As Byte())
			If data(0) <> GENERAL_DEFINITION.SOF_CMD AndAlso data(0) <> GENERAL_DEFINITION.SOF_RSP Then
				Throw New Exception("Input is not a valid Mach1 packet!")
			End If

			Try
				'validate the input data
				Dim h_d As Byte() = New Byte(1) {}
				Array.Copy(data, 1, h_d, 0, 2)
                packet_header = New HEADER_READER_HOST(h_d)

                payload_len = CType((data(3) << 8) + data(4), UInt16)
                If packet_header.timestamp_included Then
                    packet_len += 8
                End If

                packet_len += payload_len

				m_packet = New Byte(packet_len - 1) {}
				Array.Copy(data, m_packet, packet_len)

				'validate the packet
				Dim crc__1 As Byte = CRC.CalculateCRC(m_packet, m_packet.Length - 1)
				If crc__1 <> m_packet(m_packet.Length - 1) Then
					Throw New Exception("Validate CRC failed!")
				End If

                'Calculated Timestamp
                If packet_header.timestamp_included Then
                    Dim d1 As UInteger = m_packet(5)
                    Dim d2 As UInteger = m_packet(6)
                    Dim d3 As UInteger = m_packet(7)
                    Dim d4 As UInteger = m_packet(8)

                    timestamp_second = (d1 << 24) + (d2 << 16) + (d3 << 8) + d4

                    d1 = m_packet(9)
                    d2 = m_packet(10)
                    d3 = m_packet(11)
                    d4 = m_packet(12)

                    timestamp_us = (d1 << 24) + (d2 << 16) + (d3 << 8) + d4

                    'Extract mach1 payload
                    If payload_len > 0 Then
                        m_payload = New Byte(payload_len - 1) {}
                        Array.Copy(m_packet, 13, m_payload, 0, payload_len)
                    End If
                Else
                    If payload_len > 0 Then
						m_payload = New Byte(payload_len - 1) {}
						Array.Copy(m_packet, 5, m_payload, 0, payload_len)
					End If
				End If
			Catch
				Throw New Exception("Input is not a valid Mach1 packet!")
			End Try
		End Sub

		''' <summary>
		''' Parse Mach1 Messages
		''' </summary>
		''' <param name="data">Mach packet data (byte array)</param>
		''' <param name="reserved_data">Used to store fregmented Mach1 message data</param>
		''' <returns></returns>
		Public Shared Function ParseMachData(data As Byte(), ByRef reserved_data As Byte()) As MACH1_FRAME
			Dim mf As New MACH1_FRAME()

			If data(0) <> GENERAL_DEFINITION.SOF_CMD AndAlso data(0) <> GENERAL_DEFINITION.SOF_RSP Then
				Throw New Exception("Input is not a valid Mach1 packet!")
			End If

			reserved_data = Nothing

			Try
				'validate the input data. the minimum length of a mach1 packet is 5
				If data.Length < 5 Then
					reserved_data = New Byte(data.Length - 1) {}
					Array.Copy(data, reserved_data, data.Length)
					Return Nothing
				End If
				Dim h_d As Byte() = New Byte(1) {}

                Array.Copy(data, 1, h_d, 0, 2)
                mf.packet_header = New HEADER_READER_HOST(h_d)

                mf.payload_len = CType(((data(3) And &H3) << 8) + data(4), UInt16)
                If mf.packet_header.timestamp_included Then
                    mf.packet_len += 8
                End If

                mf.packet_len += mf.payload_len

				'if the length of the data is less than the packet len contained in the header, move the data to reserved data
				If data.Length < mf.packet_len Then
					reserved_data = New Byte(data.Length - 1) {}
					Array.Copy(data, reserved_data, data.Length)
					Return Nothing
				End If

                mf.m_packet = New Byte(mf.packet_len - 1) {}
                Array.Copy(data, mf.packet, mf.packet_len)

				'validate the packet
				Dim crc__1 As Byte = CRC.CalculateCRC(mf.packet, mf.packet.Length - 1)
				If crc__1 <> mf.packet(mf.packet.Length - 1) Then
					Return Nothing
				End If

                'Extract timestamp
                If mf.packet_header.timestamp_included Then
                    Dim d1 As UInteger = data(5)
                    Dim d2 As UInteger = data(6)
                    Dim d3 As UInteger = data(7)
                    Dim d4 As UInteger = data(8)

                    mf.timestamp_second = (d1 << 24) + (d2 << 16) + (d3 << 8) + d4

                    d1 = data(9)
                    d2 = data(10)
                    d3 = data(11)
                    d4 = data(12)

                    mf.timestamp_us = (d1 << 24) + (d2 << 16) + (d3 << 8) + d4

                    If mf.payload_len > 0 Then
                        mf.m_payload = New Byte(mf.payload_len - 1) {}
                        Array.Copy(data, 13, mf.PAYLOAD, 0, mf.payload_len)
                    End If
                Else
                    If mf.payload_len > 0 Then
                        mf.m_payload = New Byte(mf.payload_len - 1) {}
                        Array.Copy(data, 5, mf.payload, 0, mf.payload_len)
					End If
				End If


				Return mf
			Catch
				Throw New Exception("Input is not a valid Mach1 packet!")
			End Try
		End Function

		''' <summary>
		''' get command packet
		''' </summary>
		Public ReadOnly Property PACKET() As Byte()
			Get
				If m_packet Is Nothing Then
					Return Nothing
				Else
					Return m_packet
				End If
			End Get
		End Property

		''' <summary>
		''' get payload (command response data)
		''' </summary>
		Public ReadOnly Property PAYLOAD() As Byte()
			Get
				If m_payload IsNot Nothing Then
					Return m_payload
				Else
					Return Nothing
				End If
			End Get
		End Property

	End Class

	#Region "Header"




	''' <summary>
	''' Invalide command notification class
	''' </summary>
	''' 
	<Serializable> _
	Public Class InvalidCommandNtf
		''' <summary>
		''' Error reason code
		''' </summary>
		Public Enum REASON_CODE
			INVALID_CATEGORY = 0
			INVALID_CRC
			INVALID_MID
			OTHER_INVALID_HEADER
			INVALID_MODEM_STATE
			PARAM_OUT_OF_RANGE
			MISSING_COMMAND_PARAM
			INVALID_OPTIONAL_PARAM
			OTHER_PARAM_ERR
			OUT_OF_SYNC
			INVALID_RFU_BITS_IN_LENGTH
			COMMAND_IN_PROGRESS
		End Enum

        Public reason As REASON_CODE
        Public state As MODEM_STATE
		Private received_header As UInt16
		Private received_length As UInt16

		''' <summary>
		''' Constructor for parsing received data
		''' </summary>
		''' <param name="data"></param>
		Public Sub New(data As Byte())
			If data.Length <> 6 Then
				Throw New Exception("Not a valid packet!")
			End If

            reason = CType(data(0), REASON_CODE)
            state = CType(data(1), MODEM_STATE)

			received_header = CType((data(2) << 8) + data(3), UInt16)
			received_length = CType((data(4) << 8) + data(5), UInt16)
		End Sub
	End Class

	#End Region

End Namespace
