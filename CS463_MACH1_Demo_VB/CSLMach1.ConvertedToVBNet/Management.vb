'*****************************************************************************
'**
'** @file management.cs
'**
'** Last update: Jan 24, 2008
'**
'** This file provides MCS command set definition and implementation.
'** 
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
	''' <summary>
	''' MACH1 Management Command Set Class
	''' </summary>
	''' 
	<Serializable> _
	Public Class MANAGEMENT_CMD
		#Region "CONST"
		Public Const GET_MCS_VERSION As Byte = &H0
		Public Const GET_READER_INFO As Byte = &H1
		Public Const GET_STATE As Byte = &H2
		Public Const BOOT_MODEM As Byte = &H4
		Public Const SHUTDOWN_MODEM As Byte = &H5
		Public Const SET_TEMPERATURE_ALART As Byte = &H7
		Public Const GET_TEMPERATURE_ALART As Byte = &H8
		Public Const SET_GPO As Byte = &H9
		Public Const SET_GPI As Byte = &Ha
		Public Const GET_GPI As Byte = &Hb
		Public Const SET_STATUS_REPORT As Byte = &Hc
		Public Const SET_TCP_CONNECTION_OPTIONS As Byte = &Hd
		Public Const GET_TCP_CONNECTION_OPTIONS As Byte = &He
		#End Region

		#Region "MANAGEMENT HELP CLASSES"

		''' <summary>
		''' Temperature alarm modes
		''' </summary>
		Public Enum TERMPERETURE_ALARM_MODE
			OFF = 0
			PERIODIC
			THRESHOLD
			ONE_SHOT
		End Enum

		Public Enum MESSAGE_FLUSH_BEHAVIOR
			[DEFAULT] = 0
			'Do not flush inventory notification with halted flag and inventory status notification
			FLUSH_ALL_MSG = 1
		End Enum

		''' <summary>
		''' GPI Status Class
		''' </summary>
		''' 
		<Serializable> _
		Public Class GPI_STATUS
			''' <summary>
			''' GPI status value
			''' </summary>
			Public Enum STATUS
				LOW = 0
				HIGH
			End Enum

			Public id As Byte = 0
            Public m_status As STATUS = STATUS.LOW

            Public Sub New()
				id = 0
                m_status = STATUS.LOW
            End Sub
		End Class

		''' <summary>
		''' GPI Configuration Class
		''' </summary>
		''' 
		<Serializable> _
		Public Class GPI_CONFIG
			''' <summary>
			''' GPI configuration values
			''' </summary>
			Public Enum CONFIG
				NO_NOTFICATION = 0
				LO_TO_HIGH
				HI_TO_LOW
				BOTH
			End Enum

			''' <summary>
			''' GPI Ids
			''' </summary>
			Public Enum GPI_ID
				GPI0 = 1
				GPI1
				GPI2
				GPI3
			End Enum

			Public id As GPI_ID = GPI_ID.GPI0
            Public m_config As CONFIG = CONFIG.NO_NOTFICATION

            Public Sub New()
				id = GPI_ID.GPI0
                m_config = CONFIG.NO_NOTFICATION
            End Sub
		End Class

		''' <summary>
		''' GPO Configuration Class
		''' </summary>
		''' 
		<Serializable> _
		Public Class GPO_CONFIG
			''' <summary>
			''' Config values
			''' </summary>
			Public Enum CONFIG
				LOW = 0
				HIGH
			End Enum

			''' <summary>
			''' GPO ids
			''' </summary>
			Public Enum GPO_ID
				GPO0 = 1
				GPO1
				GPO2
				GPO3
				GPO4
				GPO5
				GPO6
				GPO7
			End Enum

			Public id As GPO_ID = GPO_ID.GPO0
            Public m_config As CONFIG = CONFIG.LOW

            Public Sub New()
				id = GPO_ID.GPO0
                m_config = CONFIG.LOW
            End Sub
			Public Sub New(id As GPO_ID, conf As CONFIG)
				Me.id = id
                Me.m_config = conf
            End Sub
		End Class




		#End Region

		#Region "GENERATE_PACKETS"

		''' <summary>
		''' generate GetMcsVersionCmd command packet
		''' </summary>
		''' <param name="include_timestamp"></param>
		''' <returns></returns>
		Public Shared Function GENERATE_GET_MCS_VERSION_CMD(include_timestamp As Boolean) As Byte()
			Dim cmd As New MACH1_FRAME(CATEGORY.MODEM_MANAGEMENT, MANAGEMENT_CMD.GET_MCS_VERSION, include_timestamp)
			Return cmd.PACKET
		End Function

		''' <summary>
		''' Generate GetReaderInfoCmd commmand packet
		''' </summary>
		''' <param name="include_timestamp"></param>
		''' <returns></returns>
		Public Shared Function GENERATE_GET_READER_INFO_CMD(include_timestamp As Boolean) As Byte()
			Dim cmd As New MACH1_FRAME(CATEGORY.MODEM_MANAGEMENT, MANAGEMENT_CMD.GET_READER_INFO, include_timestamp)
			Return cmd.PACKET
		End Function

		''' <summary>
		''' Generate GetStateCmd command package
		''' </summary>
		''' <param name="include_timestamp"></param>
		''' <returns></returns>
		Public Shared Function GENERATE_GET_STATE_CMD(include_timestamp As Boolean) As Byte()
			Dim cmd As New MACH1_FRAME(CATEGORY.MODEM_MANAGEMENT, MANAGEMENT_CMD.GET_STATE, include_timestamp)
			Return cmd.PACKET
		End Function

		''' <summary>
		''' Generate BootModemCmd command package
		''' </summary>
		''' <param name="include_timestamp"></param>
		''' <returns></returns>
		Public Shared Function GENERATE_BOOT_MODEM_CMD(include_timestamp As Boolean) As Byte()
			Dim cmd As New MACH1_FRAME(CATEGORY.MODEM_MANAGEMENT, MANAGEMENT_CMD.BOOT_MODEM, include_timestamp)
			Return cmd.PACKET
		End Function

		''' <summary>
		''' Generate ShutDownModemCmd command package
		''' </summary>
		''' <param name="include_timestamp"></param>
		''' <returns></returns>
		Public Shared Function GENERATE_SHUTDOWN_MODEM_CMD(include_timestamp As Boolean) As Byte()
			Dim cmd As New MACH1_FRAME(CATEGORY.MODEM_MANAGEMENT, MANAGEMENT_CMD.SHUTDOWN_MODEM, include_timestamp)
			Return cmd.PACKET
		End Function

		''' <summary>
		''' Generate SetTemperatureAlarmCmd command package
		''' </summary>
		''' <param name="include_timestamp"></param>
		''' <param name="mode"></param>
		''' <param name="periodicTemperetureRate"></param>
		''' <param name="alertThreshold"></param>
		''' <returns></returns>
		Public Shared Function GENERATE_SET_TEMPERETURE_ALARM_CMD(include_timestamp As Boolean, mode As TERMPERETURE_ALARM_MODE, periodicTemperetureRate As UInt16, alertThreshold As Int16) As Byte()
			Dim data As Byte() = New Byte(0) {}

			Select Case mode
				Case TERMPERETURE_ALARM_MODE.OFF, TERMPERETURE_ALARM_MODE.ONE_SHOT
					data = New Byte(0) {}
					data(0) = CByte(mode)
					Exit Select
				Case TERMPERETURE_ALARM_MODE.PERIODIC
					data = New Byte(2) {}
					data(0) = CByte(mode)
					data(1) = &H1
					data(2) = CByte(periodicTemperetureRate)
					Exit Select
				Case TERMPERETURE_ALARM_MODE.THRESHOLD
					data = New Byte(2) {}
					data(0) = CByte(mode)
					data(1) = &H2
					data(2) = CByte(alertThreshold)
					Exit Select
			End Select

			Dim cmd As New MACH1_FRAME(CATEGORY.MODEM_MANAGEMENT, MANAGEMENT_CMD.SET_TEMPERATURE_ALART, include_timestamp, data)
			Return cmd.PACKET
		End Function

		''' <summary>
		''' Generate GetTemperatureAlarmCmd command set
		''' </summary>
		''' <param name="include_timestamp"></param>
		''' <returns></returns>
		Public Shared Function GENERATE_GET_TEMPERETURE_ALARM_CMD(include_timestamp As Boolean) As Byte()
			Dim cmd As New MACH1_FRAME(CATEGORY.MODEM_MANAGEMENT, MANAGEMENT_CMD.GET_TEMPERATURE_ALART, include_timestamp)
			Return cmd.PACKET
		End Function

		''' <summary>
		''' Generate SetGPOCmd command package
		''' </summary>
		''' <param name="include_timestamp"></param>
		''' <param name="gpo"></param>
		''' <returns></returns>
		Public Shared Function GENERATE_SET_GPO_CMD(include_timestamp As Boolean, gpo As GPO_CONFIG()) As Byte()
			Dim len As Integer = gpo.Length
			Dim data As Byte() = New Byte(len * 2 - 1) {}

			For i As Integer = 0 To len - 1
				data(2 * i) = CByte(gpo(i).id)
                data(2 * i + 1) = CByte(gpo(i).m_config)
            Next
			Dim cmd As New MACH1_FRAME(CATEGORY.MODEM_MANAGEMENT, MANAGEMENT_CMD.SET_GPO, include_timestamp, data)
			Return cmd.PACKET
		End Function

		''' <summary>
		''' Generate SetGPICmd command package
		''' </summary>
		''' <param name="include_timestamp"></param>
		''' <param name="gpi"></param>
		''' <returns></returns>
		Public Shared Function GENERATE_SET_GPI_CMD(include_timestamp As Boolean, gpi As GPI_CONFIG()) As Byte()
			Dim len As Integer = gpi.Length
			Dim data As Byte() = New Byte(len * 2 - 1) {}

			For i As Integer = 0 To len - 1
				data(2 * i) = CByte(gpi(i).id)
                data(2 * i + 1) = CByte(gpi(i).m_config)
            Next

			Dim cmd As New MACH1_FRAME(CATEGORY.MODEM_MANAGEMENT, MANAGEMENT_CMD.SET_GPI, include_timestamp, data)
			Return cmd.PACKET
		End Function

		''' <summary>
		''' Generate GetGpiCmd command package
		''' </summary>
		''' <param name="include_timestamp"></param>
		''' <returns></returns>
		Public Shared Function GENERATE_GET_GPI_CMD(include_timestamp As Boolean) As Byte()
			Dim cmd As New MACH1_FRAME(CATEGORY.MODEM_MANAGEMENT, MANAGEMENT_CMD.GET_GPI, include_timestamp)
			Return cmd.PACKET
		End Function

		''' <summary>
		''' Generate SetStatusReportCmd command package
		''' </summary>
		''' <param name="include_timestamp"></param>
		''' <param name="enable_reporting"></param>
		''' <returns></returns>
		Public Shared Function GENERATE_SET_STATUS_REPORT_CMD(include_timestamp As Boolean, enable_reporting As Boolean) As Byte()
			Dim data As Byte() = New Byte(1) {}
			data(0) = &H1
			data(1) = CByte(If(enable_reporting, 1, 0))

			Dim cmd As New MACH1_FRAME(CATEGORY.MODEM_MANAGEMENT, MANAGEMENT_CMD.SET_STATUS_REPORT, include_timestamp, data)
			Return cmd.PACKET
		End Function

		''' <summary>
		''' Generate SetTcpConnectionOptionsCmd message
		''' </summary>
		''' <param name="include_timestamp"></param>
		''' <param name="behavior"></param>
		''' <returns></returns>
		Public Shared Function GENERATE_SET_TCP_CONNECTION_OPTIONS_CMD(include_timestamp As Boolean, behavior As MESSAGE_FLUSH_BEHAVIOR) As Byte()
			Dim data As Byte() = New Byte(1) {}
			data(0) = &H1
			data(1) = CByte(behavior)

			Dim cmd As New MACH1_FRAME(CATEGORY.MODEM_MANAGEMENT, MANAGEMENT_CMD.SET_TCP_CONNECTION_OPTIONS, include_timestamp, data)
			Return cmd.PACKET
		End Function

		''' <summary>
		''' Generate GetTcpConnectionOptionsCmd message
		''' </summary>
		''' <param name="include_timestamp"></param>
		''' <param name="report_behavior"></param>
		''' <returns></returns>
		Public Shared Function GENERATE_GET_TCP_CONNECTION_OPTIONS_CMD(include_timestamp As Boolean, report_behavior As Boolean) As Byte()
			Dim data As Byte() = New Byte(1) {}
			data(0) = &H1
			data(1) = CByte(If(report_behavior, 1, 0))

			Dim cmd As New MACH1_FRAME(CATEGORY.MODEM_MANAGEMENT, MANAGEMENT_CMD.GET_TCP_CONNECTION_OPTIONS, include_timestamp, data)
			Return cmd.PACKET
		End Function

		#End Region

		#Region "PARSE_RESPONSE"
		''' <summary>
		''' Mach1 Software Version Class
		''' </summary>
		Public Class MCS_VERSION_RSP
			Public version As String = String.Empty

			Public Sub New(data As Byte())
				Try
					version = String.Format("v.{0}.{1}.{2}", data(0), data(1), data(2))
				Catch
				End Try
			End Sub

		End Class

		''' <summary>
		''' Reader information class
		''' </summary>
		Public Class READER_INFO_RSP
			Public software_version As String
			Public firmware_verison As String
			Public FPGA_version As String
			Public uptime As UInt32

			Public Sub New(data As Byte())
				Try
					software_version = String.Format("v.{0}.{1}.{2}.{3}", data(0), data(1), data(2), data(3))
					firmware_verison = String.Format("v.{0}.{1}.{2}.{3}", data(4), data(5), data(6), data(7))
					FPGA_version = String.Format("v.{0}.{1}.{2}.{3}", data(8), data(9), data(10), data(11))

					Dim d1 As UInt32 = data(12)
					Dim d2 As UInt32 = data(13)
					Dim d3 As UInt32 = data(14)
					Dim d4 As UInt32 = data(15)

					uptime = (d1 << 24) + (d2 << 16) + (d3 << 8) + d4
				Catch
				End Try
				

			End Sub
		End Class

		''' <summary>
		''' State Response Class
		''' </summary>
		Public Class STATE_RSP
			Public state As MODEM_STATE

			Public Sub New(data As Byte())
				Try
					state = CType(data(0), MODEM_STATE)
				Catch
				End Try
			End Sub
		End Class

		''' <summary>
		''' Temperature Alarm Class
		''' </summary>
		Public Class TEMPERETURE_ALARM_RSP
			Public temperature As Int16
			Public mode As TERMPERETURE_ALARM_MODE
			Public periodic_tempreture_rate As Int16
			Public alarm_threshold As Int16

			Public Sub New(data As Byte())
				Try
					temperature = data(0)
					mode = CType(data(1), TERMPERETURE_ALARM_MODE)

					Select Case data(2)
						Case &H1
							periodic_tempreture_rate = data(3)
							If data.Length >= 6 AndAlso data(4) = &H1 Then
								alarm_threshold = data(5)
							End If
							Exit Select
						Case &H2
							alarm_threshold = data(3)
							If data.Length >= 6 AndAlso data(4) = &H2 Then
								periodic_tempreture_rate = data(5)
							End If
							Exit Select
					End Select
				Catch
				End Try
			End Sub
		End Class

		''' <summary>
		''' SetGpoCmd Response
		''' </summary>
		Public Class SET_GPO_RSP
			''' <summary>
			''' Result Code
			''' </summary>
			Public Enum RESULT_CODE
				GPO_SET_SUCCESSFUL = 0
				ONE_OR_MORE_GPOS_SPECIFIED_NOT_SUPPORTED
			End Enum

            Public result As RESULT_CODE
            Public Sub New(data As Byte())
                Try
                    result = CType(data(0), RESULT_CODE)
                Catch
                End Try
            End Sub
        End Class

		''' <summary>
		''' SetGpI Response Class
		''' </summary>
		Public Class SET_GPI_RSP
			Public Enum RESULT_CODE
				GPI_SET_SUCCESSFUL = 0
				ONE_OR_MORE_GPIS_SPECIFIED_NOT_SUPPORTED
				ATTEMPT_TO_CONFIGURE_ACTIVE_TRIGGER
			End Enum

            Public result As RESULT_CODE

            Public Sub New(data As Byte())
				Try
                    result = CType(data(0), RESULT_CODE)
                Catch
				End Try
			End Sub
		End Class

		''' <summary>
		''' GetGpi Response Class
		''' </summary>
		Public Class GET_GPI_RSP
			Public gpi_status As GPI_STATUS()
			Public gpi_config As GPI_CONFIG()

			Public Sub New(data As Byte())
				gpi_status = New GPI_STATUS(3) {}
				gpi_config = New GPI_CONFIG(3) {}

				Try
					For i As Integer = 0 To 3
						gpi_config(i) = New GPI_CONFIG()
						gpi_config(i).id = DirectCast(i + 1, GPI_CONFIG.GPI_ID)
                        gpi_config(i).m_config = CType(data(i), GPI_CONFIG.CONFIG)

                        gpi_status(i) = New GPI_STATUS()
						gpi_status(i).id = CByte(i + 1)
                        gpi_status(i).m_status = CType(data(i + 4), GPI_STATUS.STATUS)
                    Next
				Catch
				End Try
			End Sub
		End Class

		#End Region
	End Class

	''' <summary>
	''' Mach1 Management Notification Set
	''' </summary>
	''' 
	<Serializable> _
	Public Class MANAGEMENT_NTF
		#Region "CONST"
		Public Const SOCKET_CONNECTION_STATUS As Byte = &H0
		Public Const SYSTEM_ERROR As Byte = &H1
		Public Const BOOT_MODEM As Byte = &H2
		Public Const TEMPERATURE_ALART As Byte = &H4
		Public Const GPI_ALERT As Byte = &H5
		Public Const STATUS_REPORT As Byte = &H6
		#End Region

		''' <summary>
		''' Socket Connection Status Notification Class
		''' </summary>
		<Serializable> _
		Public Class SOCKET_CONNECTION_STATUS_NTF
			Inherits MACH1_NTF
			Public socket_status As SOCKET_STATUS = SOCKET_STATUS.CONNECTION_FAILED
			Public ip As String

			Public Sub New(data As Byte())
				socket_status = CType(data(0), SOCKET_STATUS)
				If socket_status = SOCKET_STATUS.CONNECTION_FAILED AndAlso data(1) = &H1 Then
					ip = String.Format("{0}.{1}.{2}.{3}", data(2), data(3), data(4), data(5))
				End If
			End Sub
		End Class

		''' <summary>
		''' System Error Notification Class
		''' </summary>
		''' 
		<Serializable> _
		Public Class SYSTEM_ERROR_NTF
			Inherits MACH1_NTF
			Public err_reason As SYSTEM_ERROR_REASON
			Public current_state As MODEM_STATE
			Public err_code As UInt32

			Public Sub New(data As Byte())
				err_reason = CType(data(0), SYSTEM_ERROR_REASON)
				current_state = CType(data(1), MODEM_STATE)

				If data.Length >= 7 AndAlso data(2) = &H1 Then
					Dim d1 As UInt32 = data(3)
					Dim d2 As UInt32 = data(4)
					Dim d3 As UInt32 = data(5)
					Dim d4 As UInt32 = data(6)

					err_code = (d1 << 24) + (d2 << 16) + (d3 << 8) + d4
				End If
			End Sub
		End Class

		''' <summary>
		''' Temperature alarm notification class
		''' </summary>
		''' 
		<Serializable> _
		Public Class TEMPERETURE_ALARM_NTF
			Inherits MACH1_NTF
			Public Enum REASON
				PERIODIC_REPORTING = 0
				ALARM_SHRESHOLD_REACHED
				ONE_SHOT_REPORT
			End Enum

            Public m_reason As REASON
            Public temperature As Integer

			Public Sub New(data As Byte())
				Try
                    m_reason = CType(data(0), REASON)
                    temperature = data(1)
				Catch
				End Try
				

			End Sub
		End Class

		''' <summary>
		''' GPI Alert Notification Class
		''' </summary>
		''' 
		<Serializable> _
		Public Class GPI_ALERT_NTF
			Inherits MACH1_NTF
			Public Enum GPI_TRIGGER
				GPI_0_TRIGGERED = 0
				GPI_1_TRIGGERED
				GPI_2_TRIGGERED
				GPI_3_TRIGGERED
			End Enum

			Public Enum GPI_STATUS
				LOW = 0
				HIGH = 1
			End Enum

			Public gpi As GPI_TRIGGER
			Public status As GPI_STATUS

			Public Sub New(data As Byte())
				gpi = CType(data(0), GPI_TRIGGER)

				Try
					If data(1) = &H1 Then
						status = CType(data(2), GPI_STATUS)
					End If
				Catch
				End Try
			End Sub
		End Class

		''' <summary>
		''' BootModem Notification Class
		''' </summary>
		''' 
		<Serializable> _
		Public Class BOOT_MODEM_NTF
			Inherits MACH1_NTF
			Public Enum BOOT_RESULT_CODE
				BOOT_SUCESSFUL = 0
				BOOT_IN_PROGRESS
				BOOT_FAIL_DUE_TO_INVALID_FIRMWARE
				BOOT_FAIL_DUE_TO_UNKNOWN_HARDWARE
			End Enum

            Public boot_result As BOOT_RESULT_CODE
            Public percent_done As UInt16

			Public Sub New(data As Byte())
                boot_result = CType(data(0), BOOT_RESULT_CODE)
                Try
					If data.Length > 1 AndAlso data(1) = &H1 Then
						percent_done = data(2)
					End If
				Catch
				End Try
			End Sub
		End Class

		''' <summary>
		''' StatusReport notification class
		''' </summary>
		''' 
		<Serializable> _
		Public Class STATUS_REPORT_NTF
			Inherits MACH1_NTF
			Public modem_overflow_ntf_loss As UInt32 = 0

			Public Sub New(data As Byte())
				If data(0) = &H1 Then
					Dim d1 As UInt32 = data(1)
					Dim d2 As UInt32 = data(2)
					Dim d3 As UInt32 = data(3)
					Dim d4 As UInt32 = data(4)

					modem_overflow_ntf_loss = (d1 << 24) + (d2 << 16) + (d3 << 8) + d4
				End If
			End Sub
		End Class
	End Class

End Namespace
