'*****************************************************************************
'**
'** @file speedway.cs
'**
'** Last update: Jan 24, 2008
'**
'** This file provides speedway reader implementation.
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
Imports System.Net
Imports System.Net.Sockets
Imports System.Threading
Imports System.Runtime.Remoting
Imports System.Collections
Imports System.Xml
Imports System.Xml.Serialization
Imports System.Data

Namespace CSL.Mach1
	''' <summary>
	''' Pre-defined delegates that are used for provent event-based notification
	''' </summary>
	''' <param name="bmn"></param>
	#Region "Delegates"
	Public Delegate Sub delegateBootModemNtf(bmn As MANAGEMENT_NTF.BOOT_MODEM_NTF)
	Public Delegate Sub delegateGPIAlertNtf(gan As MANAGEMENT_NTF.GPI_ALERT_NTF)
	Public Delegate Sub delegateSocketConnectionStatusNtf(scsn As MANAGEMENT_NTF.SOCKET_CONNECTION_STATUS_NTF)
	Public Delegate Sub delegateStatusReportNtf(srn As MANAGEMENT_NTF.STATUS_REPORT_NTF)
	Public Delegate Sub delegateSystemErrorNtf(sen As MANAGEMENT_NTF.SYSTEM_ERROR_NTF)
	Public Delegate Sub delegateTemperatureAlarmNtf(tan As MANAGEMENT_NTF.TEMPERETURE_ALARM_NTF)
	Public Delegate Sub delegateInventoryNtf(inv As OPERATION_NTF.INVENTORY_NTF)
	Public Delegate Sub delegateAntennaAlertNtf(aan As OPERATION_NTF.ANTENNA_ALERT_NTF)
	Public Delegate Sub delegateInventoryStatusNtf(isn As OPERATION_NTF.INVENTORY_STATUS_NTF)
	Public Delegate Sub delegateRfSurveyNtf(rsn As OPERATION_NTF.RF_SURVEY_NTF)
	Public Delegate Sub delegateModemStoppedNtf(msn As OPERATION_NTF.MODEM_STOPPED_NTF)
	Public Delegate Sub delegateAccumulateStatusNtf(asn As OPERATION_NTF.ACCUMULATION_STATUS_NTF)
	Public Delegate Sub delegateProcessMach1Message(mf As MACH1_FRAME)


	Public Delegate Sub delegateTagRead(tag As Tag)
	#End Region

	''' <summary>
	''' Command return results
	''' </summary>
	Public Enum CMD_RETURN
		COMMAND_SUCESS = 0
		CMD_RESPONSE_TIME_OUT
		COMMAND_FAILED
		RESPONSE_CAN_NOT_RECOGNIZED
		INVALID_COMMAND
	End Enum

	<Serializable> _
	Public Class SpeedwayManualResetEvent
		Public evt As ManualResetEvent
		Public data As Byte()

		Public Sub New(status As Boolean)
			evt = New ManualResetEvent(status)
		End Sub
	End Class

	<Serializable> _
	Public Class TIME_STAMP
		Public seconds As UInt32
		Public u_seconds As UInt32
	End Class

	<Serializable> _
	<XmlInclude(GetType(Tag))> _
	Public Class Tag
		<XmlElement("EPC")> _
		Public epc As String

		<XmlElement("Reader")> _
		Public reader_name As String

		<XmlElement("Antenna")> _
		Public antenna As Integer

		<XmlElement("Rssi")> _
		Public rssi As Integer

		<XmlElement("PhaseI")> _
		Public phaseI As Integer
		<XmlElement("PhaseQ")> _
		Public phaseQ As Integer

		<XmlElement("Frequency")> _
		Public frequency As Single

		<XmlElement("TimeStamp")> _
		Public timeStamp As New TIME_STAMP()

		Public Overrides Function ToString() As String
			Dim s As String
			s = String.Format("<Tag><ID>{0}</ID><Reader>{1}</Reader><Antenna>{2}</Antenna><RSSI>{3}</RSSI><TimeStamp><Second>{4}</Second><u_Second>{5}</u_Second></TimeStamp></Tag>", epc, reader_name, antenna, rssi, timeStamp.seconds, _
				timeStamp.u_seconds)
			Return s
		End Function
	End Class

	''' <summary>
	''' Speedway Reader Parameters
	''' </summary>
	<Serializable> _
	Public Class CSLReaderSettings
		Inherits MarshalByRefObject

		<Serializable> _
		Public Class Gen2Params
			Public inventory_mode As Integer = 0
			Public session As Integer = 0
			Public auto_set_mode As Integer = 2
			Public mode_id As Integer = 1
			Public population As Integer = 32
		End Class

		<Serializable> _
		Public Class ReaderInformation
			Public reader_name As String = "default"
			Public region As Integer = 0
			Public frequency_mode As Integer
			Public frequencies As UShort()
			Public reduced_power_frequencies As UShort()

			'Bug
			'Bug reported by Xiaoyong Su
			'Bug Fixed by Xiaoyong Su, 01/09/2008
			Public frequency As UShort

			''' <summary>
			''' 0 - auto select, 1 - 4 seconds
			''' </summary>
			Public lbt_time As Integer

			Public software_ver As String = String.Empty
			Public firmware_ver As String = String.Empty
			Public fpga_ver As String = String.Empty
		End Class

		<Serializable> _
		Public Class Antenna
			Public enabled As Boolean = False
			Public power As Single = 30
			Public rssi As Short = -60
		End Class

		Public gen2_params As Gen2Params
		Public reader_information As ReaderInformation
		Public antennas As Antenna() = New Antenna(3) {}
		Public maximum_sensitivity As Boolean = True
		Public enable_buffering As Boolean = False


		Public Sub New()
			gen2_params = New Gen2Params()
			reader_information = New ReaderInformation()
			For i As Integer = 0 To 3
				antennas(i) = New Antenna()
			Next
		End Sub

		Public Sub New(xmlstring As String)
			Dim data As Byte() = System.Text.Encoding.ASCII.GetBytes(xmlstring)

			Dim ms As New System.IO.MemoryStream()
			ms.Write(data, 0, data.Length)
			ms.Position = 0
			Dim serializer As New XmlSerializer(GetType(CSLReaderSettings))
			Dim settings As CSLReaderSettings = DirectCast(serializer.Deserialize(ms), CSLReaderSettings)

			gen2_params = settings.gen2_params
			reader_information = settings.reader_information
			antennas = settings.antennas

			'Bug
			'Reported by Xiaoyong Su, 01/09/2008
			'Fixed by Xiaoyong Su, 01/09/2008
			maximum_sensitivity = settings.maximum_sensitivity
			enable_buffering = settings.enable_buffering
		End Sub

		Public Overrides Function ToString() As String
			Dim serializer As New XmlSerializer(GetType(CSLReaderSettings))
			Dim ms As New System.IO.MemoryStream()
			Dim xtw As New XmlTextWriter(ms, Encoding.ASCII)
			serializer.Serialize(xtw, Me)

			Dim data As Byte() = ms.GetBuffer()
			Return System.Text.Encoding.ASCII.GetString(data)
		End Function
	End Class

	''' <summary>
	''' Use to contain asyn. read data
	''' </summary>
	''' 
	<Serializable> _
	Public Class AsynReadState
		Public data As Byte()
		Public Sub New(buffer_size As Integer)
			data = New Byte(buffer_size - 1) {}
		End Sub
	End Class

	''' <summary>
	''' Speedway reader implementation class
	''' </summary>
	''' 
	<Serializable> _
	Public Class CS461Reader
		Inherits MarshalByRefObject
		Implements IDisposable
		#Region "Reader control parameters"
		'will be defined later
		#End Region

		#Region "Adjustable Parameters"
		Public MODEM_INIT_TIME_OUT As Integer = 8000
		'Modem initialization time out
		Public CMD_RESPONSE_TIME_OUT As Integer = 500
		'General command response time out
		Public MODEM_STOP_TIME_OUT As Integer = 2000
		'Modem stop time out
		Public NOTIFICTION_TIME_OUT As Integer = 500
		'Notification time out
		#End Region
		#Region "Members"
		Public Const buffer_size As UInt16 = 2048
		'used to store data read from socket
		Public include_timestamp As Boolean = False
		'By default, all command send to reader do not include timestamp        
		Public reader_setting As New CSLReaderSettings()

		#Region "Private Members"
		Private Const port As Integer = 49380
		'Speedway default port to accept message is 49380
		Private m_connected As Boolean = False
		'connnect status
		Private client As TcpClient
		'tcp client for commnunicate with reader
		Private ns As NetworkStream
		Private read_state As AsynReadState
		Private lastError As InvalidCommandNtf
		'The last command error
		Private reserved_data As Byte() = Nothing
		'used to store un-complete data from last read
		Private inventoryBuffer As ArrayList
		'Buffer for containing inventory data
		Private inventory_start_time As New TIME_STAMP()
		Private inventory_stop_time As New TIME_STAMP()
		#End Region

		#End Region

		#Region "Events"
		Public Event onBootModemNtfReceived As delegateBootModemNtf
		Public Event onGPIAlertNtfReceived As delegateGPIAlertNtf
		Public Event onSocketConnectionStatusNtfReceived As delegateSocketConnectionStatusNtf
		Public Event onStatusReportNtfReceived As delegateStatusReportNtf
		Public Event onSystemErrorNtfReceived As delegateSystemErrorNtf
		Public Event onTemperatureAlarmNtfReceived As delegateTemperatureAlarmNtf
		Public Event onInventoryNtfReceived As delegateInventoryNtf
		Public Event onAntennaAlertNtfReceived As delegateAntennaAlertNtf
		Public Event onInventoryStatusNtfReceived As delegateInventoryStatusNtf
		Public Event onRFSurveyNtfReceived As delegateRfSurveyNtf
		Public Event onModemStopNtfReceived As delegateModemStoppedNtf
		Public Event onAccumulationStatusNtfReceived As delegateAccumulateStatusNtf
		Public Event onTagRead As delegateTagRead

		'public event
		#End Region

		#Region "reader events"
		Private bootModemNtfEvent As SpeedwayManualResetEvent
		Private bootModemRspEvent As SpeedwayManualResetEvent
		Private getMCSVersionRspEvent As SpeedwayManualResetEvent
		Private getReaderInfoRspEvent As SpeedwayManualResetEvent
		Private getStateRspEvent As SpeedwayManualResetEvent
		Private shutdownModemRspEvent As SpeedwayManualResetEvent
		Private getTemperatureAlarmRspEvent As SpeedwayManualResetEvent
		Private setTemperatureAlarmRspEvent As SpeedwayManualResetEvent
		Private setGPORspEvent As SpeedwayManualResetEvent
		Private setGPIRspEvent As SpeedwayManualResetEvent
		Private getGPIRspEvent As SpeedwayManualResetEvent
		Private setStatusReportRspEvent As SpeedwayManualResetEvent
		Private setTcpConnectionOptionsRspEvent As SpeedwayManualResetEvent
		Private getTcpConnectionOptionsRspEvent As SpeedwayManualResetEvent
		Private getOCSVersionRspEvent As SpeedwayManualResetEvent
		Private loadFromProfileRspEvent As SpeedwayManualResetEvent
		Private storeToProfileRspEvent As SpeedwayManualResetEvent
		Private setTxPowerRspEvent As SpeedwayManualResetEvent
		Private getTxPowerRspEvent As SpeedwayManualResetEvent
		Private getSupportedGen2ParamsRspEvent As SpeedwayManualResetEvent
		Private setAntennaRspEvent As SpeedwayManualResetEvent
		Private getAntennaRspEvent As SpeedwayManualResetEvent
		Private setTxFrequencyRspEvent As SpeedwayManualResetEvent
		Private getTxFrequencyRspEvent As SpeedwayManualResetEvent
		Private setGen2ParamsRspEvent As SpeedwayManualResetEvent
		Private getGen2ParamsRspEvent As SpeedwayManualResetEvent
		Private checkAntennaRspEvent As SpeedwayManualResetEvent
		Private checkAntennaNtfEvent As SpeedwayManualResetEvent
		Private setInventoryReportRspEvent As SpeedwayManualResetEvent
		Private setLBTParamsRspEvent As SpeedwayManualResetEvent
		Private getLBTParamRspEvent As SpeedwayManualResetEvent
		Private inventoryContinueRspEvent As SpeedwayManualResetEvent
		Private inventoryRspEvent As SpeedwayManualResetEvent
		Private modemStopRspEvent As SpeedwayManualResetEvent
		Private rfSurveyRspEvent As SpeedwayManualResetEvent
		Private tagReadRspEvent As SpeedwayManualResetEvent
		Private tagWriteRspEvent As SpeedwayManualResetEvent
		Private tagLockRspEvent As SpeedwayManualResetEvent
		Private tagKillRspEvent As SpeedwayManualResetEvent
		Private tagCustomRspEvent As SpeedwayManualResetEvent
		Private tagReadNtfEvent As SpeedwayManualResetEvent
		Private tagWriteNtfEvent As SpeedwayManualResetEvent
		Private tagLockNtfEvent As SpeedwayManualResetEvent
		Private tagKillNtfEvent As SpeedwayManualResetEvent
		Private invalidCommandNtfEvent As SpeedwayManualResetEvent
		Private setRxConfigRspEvent As SpeedwayManualResetEvent
		Private getRxConfigRspEvent As SpeedwayManualResetEvent
		Private setRegionNtfEvent As SpeedwayManualResetEvent
		Private setRegionRspEvent As SpeedwayManualResetEvent
		Private reportInventoryRspEvent As SpeedwayManualResetEvent
		Private getCapabilitiesRspEvent As SpeedwayManualResetEvent
		Private testWriteRspEvent As SpeedwayManualResetEvent
		Private testWriteNtfEvent As SpeedwayManualResetEvent
		Private setProfileSequenceRspEvent As SpeedwayManualResetEvent

		#End Region

		#Region "Trigger Asyn Function Call"

		#Region "Operation NTF"
		Private Sub TriggerAsynInventory(inv As OPERATION_NTF.INVENTORY_NTF)
			RaiseEvent onInventoryNtfReceived(inv)
		End Sub
		Private Sub TriggerAsynAntennaAlert(aan As OPERATION_NTF.ANTENNA_ALERT_NTF)
			RaiseEvent onAntennaAlertNtfReceived(aan)
		End Sub
		Private Sub TriggerAsynRfSurvey(rsn As OPERATION_NTF.RF_SURVEY_NTF)
			RaiseEvent onRFSurveyNtfReceived(rsn)
		End Sub
		Private Sub TriggerAsynModemStopped(msn As OPERATION_NTF.MODEM_STOPPED_NTF)
			RaiseEvent onModemStopNtfReceived(msn)
		End Sub
		Private Sub TriggerAsynInventoryStatus(isn As OPERATION_NTF.INVENTORY_STATUS_NTF)
			RaiseEvent onInventoryStatusNtfReceived(isn)
		End Sub
		Private Sub TriggerAsynAccumulationStatus(asn As OPERATION_NTF.ACCUMULATION_STATUS_NTF)
			RaiseEvent onAccumulationStatusNtfReceived(asn)
		End Sub

		Private Sub TriggerAsynTagRead(tag As Tag)
			RaiseEvent onTagRead(tag)
		End Sub
		#End Region

		#Region "Management NTF"
		Private Sub TriggerAsynTemperatureAlarm(tan As MANAGEMENT_NTF.TEMPERETURE_ALARM_NTF)
			RaiseEvent onTemperatureAlarmNtfReceived(tan)
		End Sub
		Private Sub TriggerAsynSystemError(sen As MANAGEMENT_NTF.SYSTEM_ERROR_NTF)
			RaiseEvent onSystemErrorNtfReceived(sen)
		End Sub
		Private Sub TriggerAsynStatusReport(srn As MANAGEMENT_NTF.STATUS_REPORT_NTF)
			RaiseEvent onStatusReportNtfReceived(srn)
		End Sub
		Private Sub TriggerAsynGPIAlert(gan As MANAGEMENT_NTF.GPI_ALERT_NTF)
			RaiseEvent onGPIAlertNtfReceived(gan)
		End Sub
		Private Sub TriggerAsynModemBootStatus(bmn As MANAGEMENT_NTF.BOOT_MODEM_NTF)
			RaiseEvent onBootModemNtfReceived(bmn)
		End Sub
		Private Sub TriggerSocketConnectionStatus(ssn As MANAGEMENT_NTF.SOCKET_CONNECTION_STATUS_NTF)
			RaiseEvent onSocketConnectionStatusNtfReceived(ssn)
		End Sub
		#End Region

		#End Region

		#Region "Properties"

		''' <summary>
		''' Check if the reader is connected.
		''' </summary>
		Public ReadOnly Property Connected() As Boolean
			Get
				Return m_connected
			End Get
		End Property

		''' <summary>
		''' Return the speedway reader settings.
		''' </summary>
		Public ReadOnly Property Settings() As CSLReaderSettings
			Get
				Return reader_setting
			End Get
		End Property

		''' <summary>
		''' Return inventory start time
		''' </summary>
		Public ReadOnly Property InventoryStartTime() As TIME_STAMP
			Get
				Return inventory_start_time
			End Get
		End Property

		''' <summary>
		''' Return invenotry stop time
		''' </summary>
		Public ReadOnly Property InventoryStopTime() As TIME_STAMP
			Get
				Return inventory_stop_time
			End Get
		End Property


		#End Region

		''' <summary>
		''' Remoting lease time is disabled
		''' </summary>
		''' <returns></returns>
		Public Overrides Function InitializeLifetimeService() As Object
			Return Nothing
		End Function

		''' <summary>
		''' Constructor
		''' </summary>
		Public Sub New()
			inventoryBuffer = New ArrayList()
		End Sub


		Private Sub FlushInventoryBuffer()
			inventoryBuffer = New ArrayList()
		End Sub

		''' <summary>
		''' Return buffered inventory data
		''' </summary>
		''' <returns></returns>
		Public Function GetBufferedInventoryData() As DataSet
			Dim ds As New DataSet()
			Dim dt As New DataTable()
			dt.Columns.Add("EPC")
			dt.Columns.Add("Antenna")
			dt.Columns.Add("Rssi")
			dt.Columns.Add("TimeStamp_Sec")
			dt.Columns.Add("TimeStamp_uSec")

			ds.Tables.Add(dt)

			For Each tag As Tag In inventoryBuffer
				ds.Tables(0).Rows.Add(New String() {tag.epc, tag.antenna.ToString(), tag.rssi.ToString(), tag.timeStamp.seconds.ToString(), tag.timeStamp.u_seconds.ToString()})
			Next

			Return ds
		End Function

		''' <summary>
		''' Apply reader setting to connected reader.
		''' </summary>
		''' <param name="setting"></param>
		''' <returns></returns>
		Public Function ApplyReaderSetting(setting As CSLReaderSettings) As Boolean
			If Not m_connected Then
				If Not Initialize(setting.reader_information.reader_name, CType(setting.reader_information.region, REGULATORY_REGION)) Then
					Return False
				End If
			End If

			Dim ret As CMD_RETURN

			'Set Antenna
			Dim antennas As Byte = 0
			For i As Integer = 0 To 3
				antennas = antennas Or CByte(If(setting.antennas(i).enabled, (1 << i), 0))
			Next
			Dim sar As OPERATION_CMD.SET_ANTENNA_RESULT

			ret = SetAntenna(antennas, sar)
			If ret <> CMD_RETURN.COMMAND_SUCESS Then
				Return False
			End If

			'Set gen2 parameters
			Dim spr As OPERATION_CMD.SET_GEN2_PARAMS_RESULT
			Dim gen2Param As New OPERATION_CMD.GEN2_PARAM()
			gen2Param.auto_set_mode = CType(setting.gen2_params.auto_set_mode, OPERATION_CMD.GEN2_PARAM.GEN2_LINK_MODE)
			gen2Param.inv_search_mode = CType(setting.gen2_params.inventory_mode, OPERATION_CMD.GEN2_PARAM.INVENTORY_SEARCH_MODE)
            gen2Param._mode_id = CType(setting.gen2_params.mode_id, OPERATION_CMD.GEN2_PARAM.MODE_ID)
            gen2Param._session = CType(setting.gen2_params.session, OPERATION_CMD.GEN2_PARAM.SESSION)

            ret = SetGen2Params(gen2Param, spr)
			If ret <> CMD_RETURN.COMMAND_SUCESS Then
				Return False
			End If

			'Set Sensitivity
			'if(!setting.maximum_sensitivity)
'            {
'                short[] sens = new short[4] { setting.antennas[0].rssi, setting.antennas[1].rssi, setting.antennas[2].rssi, setting.antennas[3].rssi};
'                ret = SetRxConfig(OPERATION_CMD.SET_RX_SENSITIVITY_MODE.FIXED_PER_ANTENNA, sens, out err);
'
'                if (ret != CMD_RETURN.COMMAND_SUCESS)return false;
'            }



			'Set Tx Power
			Dim stpr As OPERATION_CMD.SET_TX_POWER_RESULT
			Dim power As Single() = New Single() {setting.antennas(0).power, setting.antennas(0).power, setting.antennas(1).power, setting.antennas(2).power, setting.antennas(3).power}

			ret = SetTxPower(power, stpr)
			If ret <> CMD_RETURN.COMMAND_SUCESS Then
				Return False
			End If

			'Set Tx Frequency
			Dim sfr As OPERATION_CMD.SET_FREQUENCY_RESULT
			Dim mode As OPERATION_CMD.FREQUENCY_SET_MODE = CType(setting.reader_information.frequency_mode, OPERATION_CMD.FREQUENCY_SET_MODE)
			Select Case mode
				Case OPERATION_CMD.FREQUENCY_SET_MODE.CENTER_FREQUENCY
					ret = SetTxFrequency(OPERATION_CMD.FREQUENCY_SET_MODE.CENTER_FREQUENCY, setting.reader_information.frequency, Nothing, sfr)
					Exit Select
				Case OPERATION_CMD.FREQUENCY_SET_MODE.CHOOSE_FROM_LIST
					ret = SetTxFrequency(OPERATION_CMD.FREQUENCY_SET_MODE.CHOOSE_FROM_LIST, 0, setting.reader_information.frequencies, sfr)
					Exit Select
				Case OPERATION_CMD.FREQUENCY_SET_MODE.REDUCED_POWER_FREQUENCY_LIST

					Exit Select
                Case OPERATION_CMD.FREQUENCY_SET_MODE.CHOOSE_FROM_REGULATORY
                    ret = SetTxFrequency(OPERATION_CMD.FREQUENCY_SET_MODE.CHOOSE_FROM_REGULATORY, 0, Nothing, sfr)
					Exit Select
			End Select
			If ret <> CMD_RETURN.COMMAND_SUCESS Then
				Return False
			End If


			reader_setting = setting
			Return True
		End Function

		''' <summary>
		''' Get reader settings
		''' </summary>
		''' <param name="setting"></param>
		''' <returns></returns>
		Public Sub GetReaderSettings(ByRef setting As CSLReaderSettings)
			Dim ret As CMD_RETURN

			setting = New CSLReaderSettings()
			setting.reader_information.reader_name = Me.reader_setting.reader_information.reader_name
			setting.reader_information.region = Me.reader_setting.reader_information.region

			'Get antenna information
			Dim ant_port_status As Boolean() = New Boolean() {False, False, False, False}
			ret = GetAntenna(ant_port_status)
			If ret = CMD_RETURN.COMMAND_SUCESS Then
				For i As Integer = 0 To 3
					setting.antennas(i).enabled = ant_port_status(i)
				Next
			End If

			'Get gen2 parameters
			Dim gen2_params As OPERATION_CMD.GEN2_PARAM
			ret = GetGen2Params(True, gen2_params)
			If ret = CMD_RETURN.COMMAND_SUCESS Then
                setting.gen2_params.session = CInt(gen2_params._session)
                setting.gen2_params.mode_id = CInt(gen2_params._mode_id)
                setting.gen2_params.inventory_mode = CInt(gen2_params.inv_search_mode)
				setting.gen2_params.auto_set_mode = CInt(gen2_params.auto_set_mode)
			End If

			'Get reader software information
			Dim ris As MANAGEMENT_CMD.READER_INFO_RSP
			ret = GetReaderInfo(ris)
			If ret = CMD_RETURN.COMMAND_SUCESS Then
				setting.reader_information.software_ver = ris.software_version
				setting.reader_information.firmware_ver = ris.firmware_verison
				setting.reader_information.fpga_ver = ris.FPGA_version
			End If

			'Get Tx Power
			Dim powers As Single() = New Single(3) {}
			ret = GetTxPower(powers)
			If ret = CMD_RETURN.COMMAND_SUCESS Then
				For i As Integer = 0 To 3
					If powers.Length = 4 Then
						setting.antennas(i).power = powers(i)
					Else
						setting.antennas(i).power = powers(0)
					End If
				Next
			End If
			


			'Get Tx Frequency
			Dim freq_mode As OPERATION_CMD.FREQUENCY_SET_MODE
			ret = GetTxFrequency(freq_mode, setting.reader_information.frequency, setting.reader_information.frequencies, setting.reader_information.reduced_power_frequencies)

			setting.reader_information.frequency_mode = CInt(freq_mode)
		End Sub


		''' <summary>
		''' Initialize Speedway reader
		''' </summary>
		''' <param name="deviceName">Device name or IP address</param>
		''' <param name="region">Regulatory region</param>
		''' <returns></returns>
		Public Function Initialize(deviceName As String, region As REGULATORY_REGION) As Boolean
			Try
				client = New TcpClient(deviceName, port)

				ns = client.GetStream()
				If ns Is Nothing Then
					Return False
				End If
			Catch
				Return False
			End Try

			read_state = New AsynReadState(buffer_size)
			ns.BeginRead(read_state.data, 0, read_state.data.Length, New AsyncCallback(AddressOf OnDataRead), read_state)

			If BootModem() = CMD_RETURN.COMMAND_SUCESS Then
				If SetRegulatoryRegion(region) = CMD_RETURN.COMMAND_SUCESS Then
					Return True
				Else
					Return False
				End If
			Else
				Return False
			End If
		End Function

		''' <summary>
		''' Disconnect reader
		''' </summary>
		''' <returns></returns>
		Public Function Disconnect() As Boolean
			Try
				Try
					ModemStop()
				Catch
				End Try

				m_connected = False
				client.Close()

				Return True
			Catch
				Return False
			End Try
		End Function

		''' <summary>
		''' Implement disposable interface
		''' </summary>
		Public Sub Dispose() Implements IDisposable.Dispose
			Disconnect()
		End Sub

		''' <summary>
		''' Return last invalid command notification
		''' </summary>
		Public ReadOnly Property GetLastInvalidCommandNotification() As InvalidCommandNtf
			Get
				Try
					Return lastError
				Catch
					Return Nothing
				End Try
			End Get
		End Property

		Private Sub ProcessRSPMach1Frame(mf As MACH1_FRAME)
            Select Case mf.packet_header.category
                '#Region "Management RSP"
                Case CATEGORY.MODEM_MANAGEMENT
                    Select Case mf.packet_header.message_id
                        Case MANAGEMENT_CMD.BOOT_MODEM
                            If bootModemRspEvent IsNot Nothing Then
                                bootModemRspEvent.evt.[Set]()
                            End If
                            Exit Select
                        Case MANAGEMENT_CMD.GET_MCS_VERSION
                            getMCSVersionRspEvent.data = New Byte(mf.payload_len - 1) {}
                            If mf.PAYLOAD IsNot Nothing Then
                                Array.Copy(mf.PAYLOAD, getMCSVersionRspEvent.data, mf.payload_len)
                            End If
                            getMCSVersionRspEvent.evt.[Set]()
                            Exit Select
                        Case MANAGEMENT_CMD.SHUTDOWN_MODEM
                            shutdownModemRspEvent.evt.[Set]()
                            Exit Select
                        Case MANAGEMENT_CMD.SET_TEMPERATURE_ALART
                            setTemperatureAlarmRspEvent.evt.[Set]()
                            Exit Select
                        Case MANAGEMENT_CMD.GET_READER_INFO
                            getReaderInfoRspEvent.data = New Byte(mf.payload_len - 1) {}
                            If mf.PAYLOAD IsNot Nothing Then
                                Array.Copy(mf.PAYLOAD, getReaderInfoRspEvent.data, mf.payload_len)
                            End If
                            getReaderInfoRspEvent.evt.[Set]()
                            Exit Select
                        Case MANAGEMENT_CMD.GET_STATE
                            getStateRspEvent.data = New Byte(mf.payload_len - 1) {}
                            If mf.PAYLOAD IsNot Nothing Then
                                Array.Copy(mf.PAYLOAD, getStateRspEvent.data, mf.payload_len)
                            End If
                            getStateRspEvent.evt.[Set]()
                            Exit Select
                        Case MANAGEMENT_CMD.GET_TEMPERATURE_ALART
                            getTemperatureAlarmRspEvent.data = New Byte(mf.payload_len - 1) {}
                            If mf.PAYLOAD IsNot Nothing Then
                                Array.Copy(mf.PAYLOAD, getTemperatureAlarmRspEvent.data, mf.payload_len)
                            End If
                            getTemperatureAlarmRspEvent.evt.[Set]()
                            Exit Select
                        Case MANAGEMENT_CMD.SET_GPO
                            setGPORspEvent.data = New Byte(mf.payload_len - 1) {}
                            If mf.PAYLOAD IsNot Nothing Then
                                Array.Copy(mf.PAYLOAD, setGPORspEvent.data, mf.payload_len)
                            End If
                            setGPORspEvent.evt.[Set]()
                            Exit Select
                        Case MANAGEMENT_CMD.SET_GPI
                            setGPIRspEvent.data = New Byte(mf.payload_len - 1) {}
                            If mf.PAYLOAD IsNot Nothing Then
                                Array.Copy(mf.PAYLOAD, setGPIRspEvent.data, mf.payload_len)
                            End If
                            setGPIRspEvent.evt.[Set]()
                            Exit Select
                        Case MANAGEMENT_CMD.GET_GPI
                            getGPIRspEvent.data = New Byte(mf.payload_len - 1) {}
                            If mf.PAYLOAD IsNot Nothing Then
                                Array.Copy(mf.PAYLOAD, getGPIRspEvent.data, mf.payload_len)
                            End If
                            getGPIRspEvent.evt.[Set]()
                            Exit Select
                        Case MANAGEMENT_CMD.SET_STATUS_REPORT
                            setStatusReportRspEvent.evt.[Set]()
                            Exit Select
                        Case MANAGEMENT_CMD.SET_TCP_CONNECTION_OPTIONS
                            setTcpConnectionOptionsRspEvent.evt.[Set]()
                            Exit Select
                        Case MANAGEMENT_CMD.GET_TCP_CONNECTION_OPTIONS
                            getTcpConnectionOptionsRspEvent.data = New Byte(mf.packet_len - 1) {}
                            If mf.PAYLOAD IsNot Nothing Then
                                Array.Copy(mf.PAYLOAD, getTcpConnectionOptionsRspEvent.data, mf.payload_len)
                            End If
                            getTcpConnectionOptionsRspEvent.evt.[Set]()
                            Exit Select
                    End Select
                    Exit Select
                '#End Region

                '#Region "Operation RSP"
                Case CATEGORY.OPERATION_MODEL
                    Select Case mf.packet_header.message_id
                        Case OPERATION_CMD.SET_REGULATORY_REGION
                            If setRegionRspEvent IsNot Nothing Then
                                setRegionRspEvent.evt.[Set]()
                            End If
                            Exit Select
                        Case OPERATION_CMD.INVENTORY
                            inventoryRspEvent.data = New Byte(mf.payload_len - 1) {}
                            If mf.PAYLOAD IsNot Nothing Then
                                Array.Copy(mf.PAYLOAD, inventoryRspEvent.data, mf.payload_len)
                            End If
                            Me.inventory_start_time.seconds = mf.timestamp_second
                            Me.inventory_start_time.u_seconds = mf.timestamp_us
                            inventoryRspEvent.evt.[Set]()
                            Exit Select
                        Case OPERATION_CMD.INVENTORY_CONTINUE
                            inventoryContinueRspEvent.data = New Byte(mf.payload_len - 1) {}
                            If mf.PAYLOAD IsNot Nothing Then
                                Array.Copy(mf.PAYLOAD, inventoryContinueRspEvent.data, mf.payload_len)
                            End If
                            inventoryContinueRspEvent.evt.[Set]()
                            Exit Select
                        Case OPERATION_CMD.GET_OCS_VERSION
                            getOCSVersionRspEvent.data = New Byte(mf.payload_len - 1) {}
                            If mf.PAYLOAD IsNot Nothing Then
                                Array.Copy(mf.PAYLOAD, getOCSVersionRspEvent.data, mf.payload_len)
                            End If
                            getOCSVersionRspEvent.evt.[Set]()
                            Exit Select
                        Case OPERATION_CMD.LOAD_FROM_PROFILE
                            loadFromProfileRspEvent.evt.[Set]()
                            Exit Select
                        Case OPERATION_CMD.STORE_TO_PROFILE
                            storeToProfileRspEvent.evt.[Set]()
                            Exit Select
                        Case OPERATION_CMD.GET_CAPABILITY
                            getCapabilitiesRspEvent.data = New Byte(mf.payload_len - 1) {}
                            If mf.PAYLOAD IsNot Nothing Then
                                Array.Copy(mf.PAYLOAD, getCapabilitiesRspEvent.data, mf.packet_len)
                            End If
                            getCapabilitiesRspEvent.evt.[Set]()
                            Exit Select
                        Case OPERATION_CMD.SET_TX_POWER
                            setTxPowerRspEvent.data = New Byte(mf.payload_len - 1) {}
                            If mf.PAYLOAD IsNot Nothing Then
                                Array.Copy(mf.PAYLOAD, setTxPowerRspEvent.data, mf.payload_len)
                            End If
                            setTxPowerRspEvent.evt.[Set]()
                            Exit Select
                        Case OPERATION_CMD.GET_TX_POWER
                            getTxPowerRspEvent.data = New Byte(mf.payload_len - 1) {}
                            If mf.PAYLOAD IsNot Nothing Then
                                Array.Copy(mf.PAYLOAD, getTxPowerRspEvent.data, mf.payload_len)
                            End If
                            getTxPowerRspEvent.evt.[Set]()
                            Exit Select
                        Case OPERATION_CMD.SET_ANTENNA
                            setAntennaRspEvent.data = New Byte(mf.payload_len - 1) {}
                            If mf.PAYLOAD IsNot Nothing Then
                                Array.Copy(mf.PAYLOAD, setAntennaRspEvent.data, mf.payload_len)
                            End If
                            setAntennaRspEvent.evt.[Set]()
                            Exit Select
                        Case OPERATION_CMD.GET_ANTENNA
                            getAntennaRspEvent.data = New Byte(mf.payload_len - 1) {}
                            If mf.PAYLOAD IsNot Nothing Then
                                Array.Copy(mf.PAYLOAD, getAntennaRspEvent.data, mf.payload_len)
                            End If
                            getAntennaRspEvent.evt.[Set]()
                            Exit Select
                        Case OPERATION_CMD.SET_TX_FREQUENCY
                            setTxFrequencyRspEvent.data = New Byte(mf.payload_len - 1) {}
                            If mf.PAYLOAD IsNot Nothing Then
                                Array.Copy(mf.PAYLOAD, setTxFrequencyRspEvent.data, mf.payload_len)
                            End If
                            setTxFrequencyRspEvent.evt.[Set]()
                            Exit Select
                        Case OPERATION_CMD.GET_TX_FREQUENCY
                            getTxFrequencyRspEvent.data = New Byte(mf.payload_len - 1) {}
                            If mf.PAYLOAD IsNot Nothing Then
                                Array.Copy(mf.PAYLOAD, getTxFrequencyRspEvent.data, mf.payload_len)
                            End If
                            getTxFrequencyRspEvent.evt.[Set]()
                            Exit Select
                        Case OPERATION_CMD.SET_GEN2_PARAMS
                            setGen2ParamsRspEvent.data = New Byte(mf.payload_len - 1) {}
                            If mf.PAYLOAD IsNot Nothing Then
                                Array.Copy(mf.PAYLOAD, setGen2ParamsRspEvent.data, mf.payload_len)
                            End If
                            setGen2ParamsRspEvent.evt.[Set]()
                            Exit Select
                        Case OPERATION_CMD.GET_GEN2_PARAMS
                            getGen2ParamsRspEvent.data = New Byte(mf.payload_len - 1) {}
                            If mf.PAYLOAD IsNot Nothing Then
                                Array.Copy(mf.PAYLOAD, getGen2ParamsRspEvent.data, mf.payload_len)
                            End If
                            getGen2ParamsRspEvent.evt.[Set]()
                            Exit Select
                        Case OPERATION_CMD.CHECK_ANTENNA
                            checkAntennaRspEvent.evt.[Set]()
                            Exit Select
                        Case OPERATION_CMD.SET_INVENTORY_REPORT
                            setInventoryReportRspEvent.data = New Byte(mf.payload_len - 1) {}
                            If mf.PAYLOAD IsNot Nothing Then
                                Array.Copy(mf.PAYLOAD, setInventoryReportRspEvent.data, mf.payload_len)
                            End If
                            setInventoryReportRspEvent.evt.[Set]()
                            Exit Select
                        Case OPERATION_CMD.SET_LBT_PARAMS
                            setLBTParamsRspEvent.data = New Byte(mf.payload_len - 1) {}
                            If mf.PAYLOAD IsNot Nothing Then
                                Array.Copy(mf.PAYLOAD, setLBTParamsRspEvent.data, mf.payload_len)
                            End If
                            setLBTParamsRspEvent.evt.[Set]()
                            Exit Select
                        Case OPERATION_CMD.GET_LBT_PARAMS
                            getLBTParamRspEvent.data = New Byte(mf.payload_len - 1) {}
                            If mf.PAYLOAD IsNot Nothing Then
                                Array.Copy(mf.PAYLOAD, getLBTParamRspEvent.data, mf.payload_len)
                            End If
                            getLBTParamRspEvent.evt.[Set]()
                            Exit Select
                        Case OPERATION_CMD.REPORT_INVENTORY
                            reportInventoryRspEvent.data = New Byte(mf.payload_len - 1) {}
                            If mf.PAYLOAD IsNot Nothing Then
                                Array.Copy(mf.PAYLOAD, reportInventoryRspEvent.data, mf.payload_len)
                            End If
                            reportInventoryRspEvent.evt.[Set]()
                            Exit Select
                        Case OPERATION_CMD.MODEM_STOP
                            modemStopRspEvent.evt.[Set]()
                            Me.inventory_stop_time.seconds = mf.timestamp_second
                            Me.inventory_stop_time.u_seconds = mf.timestamp_us
                            Exit Select
                        Case OPERATION_CMD.RF_SURVEY
                            rfSurveyRspEvent.data = New Byte(mf.payload_len - 1) {}
                            If mf.PAYLOAD IsNot Nothing Then
                                Array.Copy(mf.PAYLOAD, rfSurveyRspEvent.data, mf.payload_len)
                            End If
                            rfSurveyRspEvent.evt.[Set]()
                            Exit Select
                        Case OPERATION_CMD.TAG_READ
                            tagReadRspEvent.evt.[Set]()
                            Exit Select
                        Case OPERATION_CMD.TAG_CUSTOM
                            tagCustomRspEvent.data = New Byte(mf.payload_len - 1) {}
                            If mf.PAYLOAD IsNot Nothing Then
                                Array.Copy(mf.PAYLOAD, tagCustomRspEvent.data, mf.payload_len)
                            End If
                            tagCustomRspEvent.evt.[Set]()
                            Exit Select
                        Case OPERATION_CMD.TAG_KILL
                            tagKillRspEvent.data = New Byte(mf.payload_len - 1) {}
                            If mf.PAYLOAD IsNot Nothing Then
                                Array.Copy(mf.PAYLOAD, tagKillRspEvent.data, mf.payload_len)
                            End If
                            tagKillRspEvent.evt.[Set]()
                            Exit Select
                        Case OPERATION_CMD.TAG_LOCK
                            tagLockRspEvent.data = New Byte(mf.payload_len - 1) {}
                            If mf.PAYLOAD IsNot Nothing Then
                                Array.Copy(mf.PAYLOAD, tagLockRspEvent.data, mf.payload_len)
                            End If
                            tagLockRspEvent.evt.[Set]()
                            Exit Select
                        Case OPERATION_CMD.TAG_WRITE
                            tagWriteRspEvent.data = New Byte(mf.payload_len - 1) {}
                            If mf.PAYLOAD IsNot Nothing Then
                                Array.Copy(mf.PAYLOAD, tagWriteRspEvent.data, mf.payload_len)
                            End If
                            tagWriteRspEvent.evt.[Set]()
                            Exit Select
                        Case OPERATION_CMD.SET_RX_CONFIG
                            setRxConfigRspEvent.data = New Byte(mf.payload_len - 1) {}
                            If mf.PAYLOAD IsNot Nothing Then
                                Array.Copy(mf.PAYLOAD, setRxConfigRspEvent.data, mf.payload_len)
                            End If
                            setRxConfigRspEvent.evt.[Set]()
                            Exit Select
                        Case OPERATION_CMD.GET_RX_CONFIG
                            getRxConfigRspEvent.data = New Byte(mf.payload_len - 1) {}
                            If mf.PAYLOAD IsNot Nothing Then
                                Array.Copy(mf.PAYLOAD, getRxConfigRspEvent.data, mf.payload_len)
                            End If
                            getRxConfigRspEvent.evt.[Set]()
                            Exit Select
                        Case OPERATION_CMD.GET_SUPPORTED_GEN2_PARAMS
                            getSupportedGen2ParamsRspEvent.data = New Byte(mf.payload_len - 1) {}
                            If mf.PAYLOAD IsNot Nothing Then
                                Array.Copy(mf.PAYLOAD, getSupportedGen2ParamsRspEvent.data, mf.payload_len)
                            End If
                            getSupportedGen2ParamsRspEvent.evt.[Set]()
                            Exit Select
                        Case OPERATION_CMD.SET_PROFILE_SEQUENCE
                            setProfileSequenceRspEvent.data = New Byte(mf.payload_len - 1) {}
                            If mf.PAYLOAD IsNot Nothing Then
                                Array.Copy(mf.PAYLOAD, setProfileSequenceRspEvent.data, mf.payload_len)
                            End If
                            setProfileSequenceRspEvent.evt.[Set]()
                            Exit Select
                    End Select
                    Exit Select

                '#End Region

                '#Region "Test RSP"
                Case CATEGORY.TEST
                    Select Case mf.packet_header.message_id
                        Case TEST_CMD_SET.TEST_WRITE
                            testWriteRspEvent.data = New Byte(mf.payload_len - 1) {}
                            If mf.PAYLOAD IsNot Nothing Then
                                Array.Copy(mf.PAYLOAD, testWriteRspEvent.data, mf.payload_len)
                            End If
                            testWriteRspEvent.evt.[Set]()
                            Exit Select
                    End Select
                    Exit Select
                '#End Region

                Case CATEGORY.MACH1_PROTOCOL_ERROR
                    Exit Select
            End Select
        End Sub
		Private Sub ProcessNTFMach1Frame(mf As MACH1_FRAME)
            Select Case mf.packet_header.category
                '#Region "Management NTF"
                Case CATEGORY.MODEM_MANAGEMENT
                    Select Case mf.packet_header.message_id
                        Case MANAGEMENT_NTF.BOOT_MODEM
                            Try
                                Dim bmn As New MANAGEMENT_NTF.BOOT_MODEM_NTF(mf.PAYLOAD)
                                If bootModemNtfEvent IsNot Nothing AndAlso bmn.boot_result = 0 Then
                                    bootModemNtfEvent.evt.[Set]()
                                End If
                                If bmn IsNot Nothing AndAlso onBootModemNtfReceivedEvent IsNot Nothing Then
                                    bmn.timestamp_us = mf.timestamp_us
                                    bmn.timestamp_second = mf.timestamp_second
                                    bmn.reader_name = Me.reader_setting.reader_information.reader_name
                                    Dim dbn As New delegateBootModemNtf(AddressOf TriggerAsynModemBootStatus)
                                    dbn.BeginInvoke(bmn, Nothing, Nothing)

                                End If
                            Catch
                            End Try
                            Exit Select
                        Case MANAGEMENT_NTF.SOCKET_CONNECTION_STATUS
                            Try
                                Dim ssn As New MANAGEMENT_NTF.SOCKET_CONNECTION_STATUS_NTF(mf.PAYLOAD)

                                If ssn.socket_status = SOCKET_STATUS.CONNECTION_SUCCESS Then
                                    m_connected = True
                                End If
                                If ssn IsNot Nothing AndAlso onSocketConnectionStatusNtfReceivedEvent IsNot Nothing Then
                                    ssn.timestamp_us = mf.timestamp_us
                                    ssn.timestamp_second = mf.timestamp_second
                                    ssn.reader_name = Me.reader_setting.reader_information.reader_name
                                    Dim dss As New delegateSocketConnectionStatusNtf(AddressOf TriggerSocketConnectionStatus)

                                    dss.BeginInvoke(ssn, Nothing, Nothing)

                                End If
                            Catch
                            End Try
                            Exit Select
                        Case MANAGEMENT_NTF.TEMPERATURE_ALART
                            Try
                                Dim tan As New MANAGEMENT_NTF.TEMPERETURE_ALARM_NTF(mf.PAYLOAD)
                                If tan IsNot Nothing AndAlso onTemperatureAlarmNtfReceivedEvent IsNot Nothing Then
                                    tan.timestamp_us = mf.timestamp_us
                                    tan.timestamp_second = mf.timestamp_second
                                    tan.reader_name = Me.reader_setting.reader_information.reader_name
                                    Dim tanNtf As New delegateTemperatureAlarmNtf(AddressOf TriggerAsynTemperatureAlarm)
                                    tanNtf.BeginInvoke(tan, Nothing, Nothing)
                                End If
                            Catch
                            End Try
                            Exit Select
                        Case MANAGEMENT_NTF.GPI_ALERT
                            Try
                                Dim gan As New MANAGEMENT_NTF.GPI_ALERT_NTF(mf.PAYLOAD)
                                If gan IsNot Nothing AndAlso onGPIAlertNtfReceivedEvent IsNot Nothing Then
                                    gan.timestamp_us = mf.timestamp_us
                                    gan.timestamp_second = mf.timestamp_second
                                    gan.reader_name = Me.reader_setting.reader_information.reader_name
                                    Dim dgn As New delegateGPIAlertNtf(AddressOf TriggerAsynGPIAlert)
                                    dgn.BeginInvoke(gan, Nothing, Nothing)
                                End If
                            Catch
                            End Try
                            Exit Select
                        Case MANAGEMENT_NTF.SYSTEM_ERROR
                            Try
                                Dim sen As New MANAGEMENT_NTF.SYSTEM_ERROR_NTF(mf.PAYLOAD)
                                If sen IsNot Nothing AndAlso onSystemErrorNtfReceivedEvent IsNot Nothing Then
                                    sen.timestamp_us = mf.timestamp_us
                                    sen.timestamp_second = mf.timestamp_second
                                    sen.reader_name = Me.reader_setting.reader_information.reader_name
                                    Dim dsn As New delegateSystemErrorNtf(AddressOf TriggerAsynSystemError)
                                    dsn.BeginInvoke(sen, Nothing, Nothing)
                                End If
                            Catch
                            End Try
                            Exit Select
                        Case MANAGEMENT_NTF.STATUS_REPORT
                            Try
                                Dim srn As New MANAGEMENT_NTF.STATUS_REPORT_NTF(mf.PAYLOAD)
                                If srn IsNot Nothing AndAlso onStatusReportNtfReceivedEvent IsNot Nothing Then
                                    srn.timestamp_us = mf.timestamp_us
                                    srn.timestamp_second = mf.timestamp_second
                                    srn.reader_name = Me.reader_setting.reader_information.reader_name
                                    Dim drn As New delegateStatusReportNtf(AddressOf TriggerAsynStatusReport)
                                    drn.BeginInvoke(srn, Nothing, Nothing)
                                End If
                            Catch
                            End Try
                            Exit Select
                    End Select
                    Exit Select
                '#End Region

                '#Region "Operation NTF"
                Case CATEGORY.OPERATION_MODEL
                    Select Case mf.packet_header.message_id
                        Case OPERATION_NTF.SET_REGULATORY_REGION
                            If setRegionNtfEvent IsNot Nothing Then
                                setRegionNtfEvent.evt.[Set]()
                            End If
                            Exit Select
                        Case OPERATION_NTF.INVENTORY
                            Dim inv_ntf As New OPERATION_NTF.INVENTORY_NTF(mf.PAYLOAD)

                            Dim tag As New Tag()
                            Select Case inv_ntf._antenna
                                Case OPERATION_NTF.INVENTORY_NTF.ANTENNA.ANTENNA1
                                    tag.antenna = 1
                                    Exit Select
                                Case OPERATION_NTF.INVENTORY_NTF.ANTENNA.ANTENNA2
                                    tag.antenna = 2
                                    Exit Select
                                Case OPERATION_NTF.INVENTORY_NTF.ANTENNA.ANTENNA3
                                    tag.antenna = 3
                                    Exit Select
                                Case OPERATION_NTF.INVENTORY_NTF.ANTENNA.ANTENNA4
                                    tag.antenna = 4
                                    Exit Select
                                Case Else
                                    tag.antenna = 1
                                    Exit Select
                            End Select

                            tag.epc = inv_ntf.EPC
                            tag.rssi = inv_ntf.rssi
                            tag.phaseI = inv_ntf.phaseI
                            tag.phaseQ = inv_ntf.phaseQ

                            tag.reader_name = Me.Settings.reader_information.reader_name
                            Try
                                If tag.reader_name Is Nothing OrElse tag.reader_name.Length = 0 Then
                                    tag.reader_name = "default"
                                End If
                            Catch
                                tag.reader_name = "default"
                            End Try
                            tag.timeStamp.seconds = mf.timestamp_second
                            tag.timeStamp.u_seconds = mf.timestamp_us

                            If onTagReadEvent IsNot Nothing Then
                                Try
                                    Dim dtr As New delegateTagRead(AddressOf TriggerAsynTagRead)
                                    dtr.BeginInvoke(tag, Nothing, Nothing)
                                Catch
                                End Try
                            End If


                            If reader_setting.enable_buffering Then
                                Dim existed As Boolean = False
                                For Each t As Tag In inventoryBuffer
                                    If t.epc = tag.epc Then
                                        existed = True
                                        Exit For
                                    End If
                                Next

                                If Not existed Then
                                    inventoryBuffer.Add(tag)
                                End If
                            End If

                            If inv_ntf IsNot Nothing AndAlso onInventoryNtfReceivedEvent IsNot Nothing Then
                                inv_ntf.timestamp_us = mf.timestamp_us
                                inv_ntf.timestamp_second = mf.timestamp_second

                                inv_ntf.reader_name = Me.reader_setting.reader_information.reader_name

                                Try
                                    Dim oin As New delegateInventoryNtf(AddressOf TriggerAsynInventory)
                                    oin.BeginInvoke(inv_ntf, Nothing, Nothing)
                                Catch
                                End Try
                            End If
                            Exit Select
                        Case OPERATION_NTF.ANTENNA_ALERT
                            Dim aan As New OPERATION_NTF.ANTENNA_ALERT_NTF(mf.PAYLOAD)
                            If aan IsNot Nothing AndAlso onAntennaAlertNtfReceivedEvent IsNot Nothing Then
                                Try
                                    aan.timestamp_us = mf.timestamp_us
                                    aan.timestamp_second = mf.timestamp_second
                                    aan.reader_name = Me.reader_setting.reader_information.reader_name
                                    Dim daa As New delegateAntennaAlertNtf(AddressOf TriggerAsynAntennaAlert)
                                    daa.BeginInvoke(aan, Nothing, Nothing)
                                Catch
                                End Try
                            End If
                            Exit Select
                        Case OPERATION_NTF.CHECK_ANTENNA
                            checkAntennaNtfEvent.data = New Byte(mf.payload_len - 1) {}
                            If mf.PAYLOAD IsNot Nothing Then
                                Array.Copy(mf.PAYLOAD, checkAntennaNtfEvent.data, mf.payload_len)
                            End If
                            checkAntennaNtfEvent.evt.[Set]()
                            Exit Select
                        Case OPERATION_NTF.INVENTORY_STATUS
                            Dim isn As New OPERATION_NTF.INVENTORY_STATUS_NTF(mf.PAYLOAD)
                            If isn IsNot Nothing AndAlso onInventoryStatusNtfReceivedEvent IsNot Nothing Then
                                isn.timestamp_us = mf.timestamp_us
                                isn.timestamp_second = mf.timestamp_second
                                isn.reader_name = Me.reader_setting.reader_information.reader_name
                                Try
                                    Dim dis As New delegateInventoryStatusNtf(AddressOf TriggerAsynInventoryStatus)
                                    dis.BeginInvoke(isn, Nothing, Nothing)
                                Catch
                                End Try
                            End If
                            Exit Select
                        Case OPERATION_NTF.ACCUMULATION_STATUS
                            Dim asn As New OPERATION_NTF.ACCUMULATION_STATUS_NTF()
                            asn.code = CType(mf.PAYLOAD(0), OPERATION_NTF.ACCUMULATION_STATUS_NTF.NTF_CODE)
                            If asn IsNot Nothing AndAlso onAccumulationStatusNtfReceivedEvent IsNot Nothing Then
                                asn.timestamp_us = mf.timestamp_us
                                asn.timestamp_second = mf.timestamp_second
                                asn.reader_name = Me.reader_setting.reader_information.reader_name
                                Try
                                    Dim dasn As New delegateAccumulateStatusNtf(AddressOf TriggerAsynAccumulationStatus)
                                    dasn.BeginInvoke(asn, Nothing, Nothing)
                                Catch
                                End Try
                            End If
                            Exit Select
                        Case OPERATION_NTF.MODEM_STOP
                            Dim msn As New OPERATION_NTF.MODEM_STOPPED_NTF()
                            msn.code = CType(mf.PAYLOAD(0), OPERATION_NTF.MODEM_STOPPED_NTF.NTF_CODE)
                            If msn IsNot Nothing AndAlso onModemStopNtfReceivedEvent IsNot Nothing Then
                                msn.timestamp_us = mf.timestamp_us
                                msn.timestamp_second = mf.timestamp_second
                                msn.reader_name = Me.reader_setting.reader_information.reader_name
                                Try
                                    Dim dmn As New delegateModemStoppedNtf(AddressOf TriggerAsynModemStopped)
                                    dmn.BeginInvoke(msn, Nothing, Nothing)
                                Catch
                                End Try
                            End If
                            Exit Select
                        Case OPERATION_NTF.RF_SURVEY
                            Dim rsn As New OPERATION_NTF.RF_SURVEY_NTF(mf.PAYLOAD)
                            If rsn IsNot Nothing AndAlso onRFSurveyNtfReceivedEvent IsNot Nothing Then
                                rsn.timestamp_us = mf.timestamp_us
                                rsn.timestamp_second = mf.timestamp_second
                                rsn.reader_name = Me.reader_setting.reader_information.reader_name
                                Try
                                    Dim drn As New delegateRfSurveyNtf(AddressOf TriggerAsynRfSurvey)
                                    drn.BeginInvoke(rsn, Nothing, Nothing)
                                Catch
                                End Try
                            End If
                            Exit Select

                        Case OPERATION_NTF.TAG_READ
                            tagReadNtfEvent.data = New Byte(mf.payload_len - 1) {}
                            If mf.PAYLOAD IsNot Nothing Then
                                Array.Copy(mf.PAYLOAD, tagReadNtfEvent.data, mf.payload_len)
                            End If
                            tagReadNtfEvent.evt.[Set]()
                            Exit Select
                        Case OPERATION_NTF.TAG_WRITE
                            tagWriteNtfEvent.data = New Byte(mf.payload_len - 1) {}
                            If mf.PAYLOAD IsNot Nothing Then
                                Array.Copy(mf.PAYLOAD, tagWriteNtfEvent.data, mf.payload_len)
                            End If
                            tagWriteNtfEvent.evt.[Set]()
                            Exit Select
                        Case OPERATION_NTF.TAG_LOCK
                            tagLockNtfEvent.data = New Byte(mf.payload_len - 1) {}
                            If mf.PAYLOAD IsNot Nothing Then
                                Array.Copy(mf.PAYLOAD, tagLockNtfEvent.data, mf.payload_len)
                            End If
                            tagLockNtfEvent.evt.[Set]()
                            Exit Select
                        Case OPERATION_NTF.TAG_KILL
                            tagKillNtfEvent.data = New Byte(mf.payload_len - 1) {}
                            If mf.PAYLOAD IsNot Nothing Then
                                Array.Copy(mf.PAYLOAD, tagKillNtfEvent.data, mf.payload_len)
                            End If
                            tagKillNtfEvent.evt.[Set]()
                            Exit Select
                    End Select
                    Exit Select
                '#End Region

                '#Region "Test Ntf"
                Case CATEGORY.TEST

                    Exit Select
                '#End Region

                Case CATEGORY.MACH1_PROTOCOL_ERROR
                    invalidCommandNtfEvent.data = New Byte(mf.payload_len - 1) {}
                    If mf.PAYLOAD IsNot Nothing Then
                        Array.Copy(mf.PAYLOAD, invalidCommandNtfEvent.data, mf.payload_len)
                    End If
                    invalidCommandNtfEvent.evt.[Set]()
                    Try
                        lastError = New InvalidCommandNtf(mf.PAYLOAD)
                    Catch
                    End Try
                    Exit Select
            End Select
        End Sub
		Private Sub ProcessMach1Frame(mf As MACH1_FRAME)
            If mf.packet_header.is_ntf Then
                ProcessNTFMach1Frame(mf)
            Else
                'if the packet is response                           
                ProcessRSPMach1Frame(mf)
			End If
		End Sub
		Private Sub OnDataRead(ar As IAsyncResult)
			Dim offset As Integer = 0
			Dim ss As AsynReadState = DirectCast(ar.AsyncState, AsynReadState)

			Dim data As Byte()

			Try
				'if there are reserved data from last packet, add the data to the packet
				If reserved_data IsNot Nothing Then
					data = New Byte(ss.data.Length + (reserved_data.Length - 1)) {}
					Array.Copy(reserved_data, data, reserved_data.Length)
					Array.Copy(ss.data, 0, data, reserved_data.Length, ss.data.Length)

					reserved_data = Nothing
				Else
					data = New Byte(ss.data.Length - 1) {}
					Array.Copy(ss.data, data, ss.data.Length)
				End If

				While offset < data.Length AndAlso (data(offset) = GENERAL_DEFINITION.SOF_CMD OrElse data(offset) = GENERAL_DEFINITION.SOF_RSP)
					Dim f_data As Byte() = New Byte(data.Length - offset - 1) {}
					Array.Copy(data, offset, f_data, 0, data.Length - offset)

					Try
						Dim mf As MACH1_FRAME = MACH1_FRAME.ParseMachData(f_data, reserved_data)

						If mf IsNot Nothing Then
							offset += mf.packet_len
							ProcessMach1Frame(mf)
						Else
							Exit Try
						End If
					Catch
						Exit Try
					End Try
				End While

				If m_connected AndAlso ns IsNot Nothing Then
					Try
						ns.Flush()
						read_state = New AsynReadState(buffer_size)
						ns.BeginRead(read_state.data, 0, read_state.data.Length, New AsyncCallback(AddressOf OnDataRead), read_state)
					Catch
					End Try
				End If
			Catch
			End Try
		End Sub

		#Region "Management Command"
		''' <summary>
		''' Boot modem command
		''' </summary>
		''' <returns></returns>
		Public Function BootModem() As CMD_RETURN
			bootModemRspEvent = New SpeedwayManualResetEvent(False)
			bootModemNtfEvent = New SpeedwayManualResetEvent(False)
			invalidCommandNtfEvent = New SpeedwayManualResetEvent(False)
			Dim waithandles As WaitHandle() = New WaitHandle() {bootModemRspEvent.evt, invalidCommandNtfEvent.evt}

			Dim data As Byte() = MANAGEMENT_CMD.GENERATE_BOOT_MODEM_CMD(include_timestamp)
			ns.Write(data, 0, data.Length)

			Dim wait_result As Integer = WaitHandle.WaitAny(waithandles, MODEM_INIT_TIME_OUT, False)
			If wait_result = 1 Then
				Return CMD_RETURN.INVALID_COMMAND
			ElseIf wait_result > 1 Then
				Return CMD_RETURN.CMD_RESPONSE_TIME_OUT
			End If

			If Not bootModemNtfEvent.evt.WaitOne(MODEM_INIT_TIME_OUT, False) Then
				Return CMD_RETURN.COMMAND_FAILED
			End If

			Return CMD_RETURN.COMMAND_SUCESS
		End Function

		''' <summary>
		''' Get Management command set version. 
		''' </summary>
		''' <param name="rsp"></param>
		''' <returns></returns>
		Public Function GetMCSVersion(ByRef rsp As MANAGEMENT_CMD.MCS_VERSION_RSP) As CMD_RETURN
			getMCSVersionRspEvent = New SpeedwayManualResetEvent(False)
			invalidCommandNtfEvent = New SpeedwayManualResetEvent(False)
			Dim waithandles As WaitHandle() = New WaitHandle() {getMCSVersionRspEvent.evt, invalidCommandNtfEvent.evt}
			rsp = Nothing

			Dim data As Byte() = MANAGEMENT_CMD.GENERATE_GET_MCS_VERSION_CMD(include_timestamp)
			ns.Write(data, 0, data.Length)

			Dim wait_result As Integer = WaitHandle.WaitAny(waithandles, CMD_RESPONSE_TIME_OUT, False)
			If wait_result = 1 Then
				Return CMD_RETURN.INVALID_COMMAND
			ElseIf wait_result > 1 Then
				Return CMD_RETURN.CMD_RESPONSE_TIME_OUT
			Else
				Try
					rsp = New MANAGEMENT_CMD.MCS_VERSION_RSP(getMCSVersionRspEvent.data)
				Catch
					Return CMD_RETURN.RESPONSE_CAN_NOT_RECOGNIZED
				End Try

				Return CMD_RETURN.COMMAND_SUCESS
			End If
		End Function

		''' <summary>
		''' Get reader information
		''' </summary>
		''' <param name="rsp"></param>
		''' <returns></returns>
		Public Function GetReaderInfo(ByRef rsp As MANAGEMENT_CMD.READER_INFO_RSP) As CMD_RETURN
			getReaderInfoRspEvent = New SpeedwayManualResetEvent(False)
			invalidCommandNtfEvent = New SpeedwayManualResetEvent(False)
			Dim waithandles As WaitHandle() = New WaitHandle() {getReaderInfoRspEvent.evt, invalidCommandNtfEvent.evt}
			Dim data As Byte() = MANAGEMENT_CMD.GENERATE_GET_READER_INFO_CMD(include_timestamp)
			ns.Write(data, 0, data.Length)

			rsp = Nothing
			Dim wait_result As Integer = WaitHandle.WaitAny(waithandles, CMD_RESPONSE_TIME_OUT, False)
			If wait_result = 1 Then
				Return CMD_RETURN.INVALID_COMMAND
			ElseIf wait_result > 1 Then
				Return CMD_RETURN.CMD_RESPONSE_TIME_OUT
			End If

			Try
				rsp = New MANAGEMENT_CMD.READER_INFO_RSP(getReaderInfoRspEvent.data)
				Return CMD_RETURN.COMMAND_SUCESS
			Catch
				Return CMD_RETURN.RESPONSE_CAN_NOT_RECOGNIZED
			End Try

		End Function

		''' <summary>
		''' Get modem state
		''' </summary>
		''' <param name="rsp"></param>
		''' <returns></returns>
		Public Function GetState(ByRef rsp As MANAGEMENT_CMD.STATE_RSP) As CMD_RETURN
			getStateRspEvent = New SpeedwayManualResetEvent(False)
			invalidCommandNtfEvent = New SpeedwayManualResetEvent(False)
			Dim waithandles As WaitHandle() = New WaitHandle() {getStateRspEvent.evt, invalidCommandNtfEvent.evt}

			Dim data As Byte() = MANAGEMENT_CMD.GENERATE_GET_STATE_CMD(include_timestamp)

			ns.Write(data, 0, data.Length)
			rsp = Nothing
			Dim wait_result As Integer = WaitHandle.WaitAny(waithandles, CMD_RESPONSE_TIME_OUT, False)
			If wait_result = 1 Then
				Return CMD_RETURN.INVALID_COMMAND
			ElseIf wait_result > 1 Then
				Return CMD_RETURN.CMD_RESPONSE_TIME_OUT
			End If
			Try
				rsp = New MANAGEMENT_CMD.STATE_RSP(getStateRspEvent.data)

				Return CMD_RETURN.COMMAND_SUCESS
			Catch
				Return CMD_RETURN.RESPONSE_CAN_NOT_RECOGNIZED
			End Try

		End Function

		''' <summary>
		''' Shutdown modem
		''' </summary>
		''' <returns></returns>
		Public Function ShutDownModem() As CMD_RETURN
			shutdownModemRspEvent = New SpeedwayManualResetEvent(False)
			invalidCommandNtfEvent = New SpeedwayManualResetEvent(False)
			Dim waithandles As WaitHandle() = New WaitHandle() {shutdownModemRspEvent.evt, invalidCommandNtfEvent.evt}

			Dim data As Byte() = MANAGEMENT_CMD.GENERATE_SHUTDOWN_MODEM_CMD(include_timestamp)
			ns.Write(data, 0, data.Length)

			Dim wait_result As Integer = WaitHandle.WaitAny(waithandles, CMD_RESPONSE_TIME_OUT, False)
			If wait_result = 1 Then
				Return CMD_RETURN.INVALID_COMMAND
			ElseIf wait_result > 1 Then
				Return CMD_RETURN.CMD_RESPONSE_TIME_OUT
			Else
				Return CMD_RETURN.COMMAND_SUCESS
			End If

		End Function

		''' <summary>
		''' Set temperature alarm
		''' </summary>
		''' <param name="mode"></param>
		''' <param name="periodic">alarm report interval, value is 1-6. interms of 10s</param>
		''' <param name="alert_threshold">temperature threshold, celsius degree</param>
		''' <returns></returns>
		Public Function SetTempretureAlarm(mode As MANAGEMENT_CMD.TERMPERETURE_ALARM_MODE, periodic As UInt16, alert_threshold As Int16) As CMD_RETURN
			setTemperatureAlarmRspEvent = New SpeedwayManualResetEvent(False)
			invalidCommandNtfEvent = New SpeedwayManualResetEvent(False)
			Dim waithandles As WaitHandle() = New WaitHandle() {setTemperatureAlarmRspEvent.evt, invalidCommandNtfEvent.evt}

			Dim data As Byte() = MANAGEMENT_CMD.GENERATE_SET_TEMPERETURE_ALARM_CMD(include_timestamp, mode, periodic, alert_threshold)
			ns.Write(data, 0, data.Length)

			Dim wait_result As Integer = WaitHandle.WaitAny(waithandles, CMD_RESPONSE_TIME_OUT, False)
			If wait_result = 1 Then
				Return CMD_RETURN.INVALID_COMMAND
			ElseIf wait_result > 1 Then
				Return CMD_RETURN.CMD_RESPONSE_TIME_OUT
			Else
				Return CMD_RETURN.COMMAND_SUCESS
			End If

		End Function

		''' <summary>
		''' Get temperature alarm
		''' </summary>
		''' <param name="rsp"></param>
		''' <returns></returns>
		Public Function GetTemperatureAlarm(ByRef rsp As MANAGEMENT_CMD.TEMPERETURE_ALARM_RSP) As CMD_RETURN
			getTemperatureAlarmRspEvent = New SpeedwayManualResetEvent(False)
			invalidCommandNtfEvent = New SpeedwayManualResetEvent(False)
			Dim waithandles As WaitHandle() = New WaitHandle() {getTemperatureAlarmRspEvent.evt, invalidCommandNtfEvent.evt}
			Dim data As Byte() = MANAGEMENT_CMD.GENERATE_GET_TEMPERETURE_ALARM_CMD(include_timestamp)

			ns.Write(data, 0, data.Length)

			rsp = Nothing
			Dim wait_result As Integer = WaitHandle.WaitAny(waithandles, CMD_RESPONSE_TIME_OUT, False)
			If wait_result = 1 Then
				Return CMD_RETURN.INVALID_COMMAND
			ElseIf wait_result > 1 Then
				Return CMD_RETURN.CMD_RESPONSE_TIME_OUT
			End If
			Try
				rsp = New MANAGEMENT_CMD.TEMPERETURE_ALARM_RSP(getTemperatureAlarmRspEvent.data)
				Return CMD_RETURN.COMMAND_SUCESS
			Catch
				Return CMD_RETURN.RESPONSE_CAN_NOT_RECOGNIZED
			End Try

		End Function

		''' <summary>
		''' Set GPO status
		''' </summary>
		''' <param name="gpo_config"></param>
		''' <param name="rsp"></param>
		''' <returns></returns>
		Public Function SetGPO(gpo_config As MANAGEMENT_CMD.GPO_CONFIG(), ByRef rsp As MANAGEMENT_CMD.SET_GPO_RSP) As CMD_RETURN
			setGPORspEvent = New SpeedwayManualResetEvent(False)
			invalidCommandNtfEvent = New SpeedwayManualResetEvent(False)
			Dim waithandles As WaitHandle() = New WaitHandle() {setGPORspEvent.evt, invalidCommandNtfEvent.evt}
			Dim data As Byte() = MANAGEMENT_CMD.GENERATE_SET_GPO_CMD(include_timestamp, gpo_config)

			ns.Write(data, 0, data.Length)
			rsp = Nothing
			Dim wait_result As Integer = WaitHandle.WaitAny(waithandles, CMD_RESPONSE_TIME_OUT, False)
			If wait_result = 1 Then
				Return CMD_RETURN.INVALID_COMMAND
			ElseIf wait_result > 1 Then
				Return CMD_RETURN.CMD_RESPONSE_TIME_OUT
			End If
			Try
				rsp = New MANAGEMENT_CMD.SET_GPO_RSP(setGPORspEvent.data)
				Return CMD_RETURN.COMMAND_SUCESS
			Catch
				Return CMD_RETURN.RESPONSE_CAN_NOT_RECOGNIZED
			End Try


		End Function

		''' <summary>
		''' Set GPI configuration
		''' </summary>
		''' <param name="gpi_config"></param>
		''' <param name="rsp"></param>
		''' <returns></returns>
		Public Function SetGPI(gpi_config As MANAGEMENT_CMD.GPI_CONFIG(), ByRef rsp As MANAGEMENT_CMD.SET_GPI_RSP) As CMD_RETURN
			setGPIRspEvent = New SpeedwayManualResetEvent(False)
			invalidCommandNtfEvent = New SpeedwayManualResetEvent(False)
			Dim waithandles As WaitHandle() = New WaitHandle() {setGPIRspEvent.evt, invalidCommandNtfEvent.evt}

			Dim data As Byte() = MANAGEMENT_CMD.GENERATE_SET_GPI_CMD(include_timestamp, gpi_config)

			ns.Write(data, 0, data.Length)
			rsp = Nothing
			Dim wait_result As Integer = WaitHandle.WaitAny(waithandles, CMD_RESPONSE_TIME_OUT, False)
			If wait_result = 1 Then
				Return CMD_RETURN.INVALID_COMMAND
			ElseIf wait_result > 1 Then
				Return CMD_RETURN.CMD_RESPONSE_TIME_OUT
			End If
			Try
				rsp = New MANAGEMENT_CMD.SET_GPI_RSP(setGPIRspEvent.data)
				Return CMD_RETURN.COMMAND_SUCESS
			Catch
				Return CMD_RETURN.RESPONSE_CAN_NOT_RECOGNIZED
			End Try


		End Function

		''' <summary>
		''' Get GPI status
		''' </summary>
		''' <param name="rsp"></param>
		''' <returns></returns>
		Public Function GetGPI(ByRef rsp As MANAGEMENT_CMD.GET_GPI_RSP) As CMD_RETURN
			getGPIRspEvent = New SpeedwayManualResetEvent(False)
			invalidCommandNtfEvent = New SpeedwayManualResetEvent(False)
			Dim waithandles As WaitHandle() = New WaitHandle() {getGPIRspEvent.evt, invalidCommandNtfEvent.evt}

			Dim data As Byte() = MANAGEMENT_CMD.GENERATE_GET_GPI_CMD(include_timestamp)

			ns.Write(data, 0, data.Length)
			rsp = Nothing

			Dim wait_result As Integer = WaitHandle.WaitAny(waithandles, CMD_RESPONSE_TIME_OUT, False)
			If wait_result = 1 Then
				Return CMD_RETURN.INVALID_COMMAND
			ElseIf wait_result > 1 Then
				Return CMD_RETURN.CMD_RESPONSE_TIME_OUT
			End If
			Try
				rsp = New MANAGEMENT_CMD.GET_GPI_RSP(getGPIRspEvent.data)
				Return CMD_RETURN.COMMAND_SUCESS
			Catch
				Return CMD_RETURN.RESPONSE_CAN_NOT_RECOGNIZED
			End Try

		End Function

		''' <summary>
		''' Enable/Disable status report
		''' </summary>
		''' <param name="enable"></param>
		''' <returns></returns>
		Public Function SetStatusReport(enable As Boolean) As CMD_RETURN
			setStatusReportRspEvent = New SpeedwayManualResetEvent(False)
			invalidCommandNtfEvent = New SpeedwayManualResetEvent(False)
			Dim waithandles As WaitHandle() = New WaitHandle() {setStatusReportRspEvent.evt, invalidCommandNtfEvent.evt}
			Dim data As Byte() = MANAGEMENT_CMD.GENERATE_SET_STATUS_REPORT_CMD(include_timestamp, enable)

			ns.Write(data, 0, data.Length)
			Dim result As Integer = WaitHandle.WaitAny(waithandles, CMD_RESPONSE_TIME_OUT, False)
			If result = 1 Then
				Return CMD_RETURN.INVALID_COMMAND
			ElseIf result > 1 Then
				Return CMD_RETURN.CMD_RESPONSE_TIME_OUT
			Else
				Return CMD_RETURN.COMMAND_SUCESS
			End If

		End Function

		''' <summary>
		''' Set TCP connection flashing behavior
		''' </summary>
		''' <param name="behavior"></param>
		''' <returns></returns>
		Public Function SetTCPConnectionOptions(behavior As MANAGEMENT_CMD.MESSAGE_FLUSH_BEHAVIOR) As CMD_RETURN
			setTcpConnectionOptionsRspEvent = New SpeedwayManualResetEvent(False)
			invalidCommandNtfEvent = New SpeedwayManualResetEvent(False)

			Dim waithandles As WaitHandle() = New WaitHandle() {setTcpConnectionOptionsRspEvent.evt, invalidCommandNtfEvent.evt}
			Dim data As Byte() = MANAGEMENT_CMD.GENERATE_SET_TCP_CONNECTION_OPTIONS_CMD(include_timestamp, behavior)

			ns.Write(data, 0, data.Length)
			Dim result As Integer = WaitHandle.WaitAny(waithandles, CMD_RESPONSE_TIME_OUT, False)
			If result = 1 Then
				Return CMD_RETURN.INVALID_COMMAND
			ElseIf result > 1 Then
				Return CMD_RETURN.CMD_RESPONSE_TIME_OUT
			Else
				Return CMD_RETURN.COMMAND_SUCESS
			End If
		End Function

		''' <summary>
		''' Get TCP connection flashing behavior
		''' </summary>
		''' <param name="report_behavior"></param>
		''' <param name="behavior"></param>
		''' <returns></returns>
		Public Function GetTCPConnectionOptions(report_behavior As Boolean, ByRef behavior As MANAGEMENT_CMD.MESSAGE_FLUSH_BEHAVIOR) As CMD_RETURN
			getTcpConnectionOptionsRspEvent = New SpeedwayManualResetEvent(False)
			invalidCommandNtfEvent = New SpeedwayManualResetEvent(False)

			behavior = MANAGEMENT_CMD.MESSAGE_FLUSH_BEHAVIOR.[DEFAULT]

			Dim waithandles As WaitHandle() = New WaitHandle() {getTcpConnectionOptionsRspEvent.evt, invalidCommandNtfEvent.evt}
			Dim data As Byte() = MANAGEMENT_CMD.GENERATE_GET_TCP_CONNECTION_OPTIONS_CMD(include_timestamp, report_behavior)

			ns.Write(data, 0, data.Length)
			Dim result As Integer = WaitHandle.WaitAny(waithandles, CMD_RESPONSE_TIME_OUT, False)
			If result = 1 Then
				Return CMD_RETURN.INVALID_COMMAND
			ElseIf result > 1 Then
				Return CMD_RETURN.CMD_RESPONSE_TIME_OUT
			Else
				If getTcpConnectionOptionsRspEvent.data.Length > 2 Then
					behavior = CType(getTcpConnectionOptionsRspEvent.data(2), MANAGEMENT_CMD.MESSAGE_FLUSH_BEHAVIOR)
				End If
				Return CMD_RETURN.COMMAND_SUCESS
			End If
		End Function

		#End Region

		#Region "Operation Command"

		''' <summary>
		''' Set regulatory region
		''' </summary>
		''' <param name="region"></param>
		''' <returns></returns>
		Public Function SetRegulatoryRegion(region As REGULATORY_REGION) As CMD_RETURN
			setRegionNtfEvent = New SpeedwayManualResetEvent(False)
			setRegionRspEvent = New SpeedwayManualResetEvent(False)
			invalidCommandNtfEvent = New SpeedwayManualResetEvent(False)

			Dim waithandles As WaitHandle() = New WaitHandle() {setRegionRspEvent.evt, invalidCommandNtfEvent.evt}

			Dim data As Byte() = OPERATION_CMD.GENERATE_SET_REGULATORY_REGION(include_timestamp, region)
			ns.Write(data, 0, data.Length)

			Dim wait_result As Integer = WaitHandle.WaitAny(waithandles, CMD_RESPONSE_TIME_OUT, False)
			If wait_result = 1 Then
				Return CMD_RETURN.INVALID_COMMAND
			ElseIf wait_result > 1 Then
				Return CMD_RETURN.CMD_RESPONSE_TIME_OUT
			End If

			If Not setRegionNtfEvent.evt.WaitOne(MODEM_INIT_TIME_OUT, False) Then
				Return CMD_RETURN.COMMAND_FAILED
			End If

			Return CMD_RETURN.COMMAND_SUCESS

		End Function

		''' <summary>
		''' Invnetory
		''' </summary>
		''' <param name="prama">Inventory parameters</param>
		''' <param name="result">command result code</param>
		''' <returns></returns>
		Public Function Inventory(prama As OPERATION_CMD.INVENTORY_PRAMA, ByRef result As OPERATION_CMD.INVENTORY_RESULT) As CMD_RETURN
			inventoryRspEvent = New SpeedwayManualResetEvent(False)
			invalidCommandNtfEvent = New SpeedwayManualResetEvent(False)

			Dim waithandles As WaitHandle() = New WaitHandle() {inventoryRspEvent.evt, invalidCommandNtfEvent.evt}

			Dim data As Byte() = OPERATION_CMD.GENERATE_INVENTORY_CMD(include_timestamp, prama)

			result = OPERATION_CMD.INVENTORY_RESULT.FAIL_CONFIGURATION_ERROR
			ns.Write(data, 0, data.Length)

			Dim wait_result As Integer = WaitHandle.WaitAny(waithandles, CMD_RESPONSE_TIME_OUT, False)
			If wait_result = 1 Then
				Return CMD_RETURN.INVALID_COMMAND
			ElseIf wait_result > 1 Then
				Return CMD_RETURN.CMD_RESPONSE_TIME_OUT
			Else
				Try
					result = CType(inventoryRspEvent.data(0), OPERATION_CMD.INVENTORY_RESULT)
					Return CMD_RETURN.COMMAND_SUCESS
				Catch
					Return CMD_RETURN.RESPONSE_CAN_NOT_RECOGNIZED
				End Try
			End If
		End Function

		''' <summary>
		''' Invnetory
		''' </summary>
		''' <param name="prama"></param>
		''' <param name="err_occur"></param>
		''' <returns></returns>
		Public Function Inventory(prama As OPERATION_CMD.INVENTORY_PRAMA, ByRef err_occur As Boolean) As CMD_RETURN
			inventoryRspEvent = New SpeedwayManualResetEvent(False)
			invalidCommandNtfEvent = New SpeedwayManualResetEvent(False)

			Dim waithandles As WaitHandle() = New WaitHandle() {inventoryRspEvent.evt, invalidCommandNtfEvent.evt}

			Dim data As Byte() = OPERATION_CMD.GENERATE_INVENTORY_CMD(include_timestamp, prama)

			err_occur = True
			ns.Write(data, 0, data.Length)

			Dim wait_result As Integer = WaitHandle.WaitAny(waithandles, CMD_RESPONSE_TIME_OUT, False)
			If wait_result = 1 Then
				Return CMD_RETURN.INVALID_COMMAND
			ElseIf wait_result > 1 Then
				Return CMD_RETURN.CMD_RESPONSE_TIME_OUT
			Else
				Try
					err_occur = If((inventoryRspEvent.data(0) = 1), True, False)
					Return CMD_RETURN.COMMAND_SUCESS
				Catch
					Return CMD_RETURN.RESPONSE_CAN_NOT_RECOGNIZED
				End Try
			End If
		End Function

		''' <summary>
		''' Inventory continue 
		''' </summary>
		''' <param name="err_ocurr"></param>
		''' <returns></returns>
		Public Function InventoryContinue(ByRef err_ocurr As Boolean) As CMD_RETURN
			inventoryContinueRspEvent = New SpeedwayManualResetEvent(False)
			invalidCommandNtfEvent = New SpeedwayManualResetEvent(False)
			Dim waithandles As WaitHandle() = New WaitHandle() {inventoryContinueRspEvent.evt, invalidCommandNtfEvent.evt}

			Dim data As Byte() = OPERATION_CMD.GENERATE_INVENTORY_CONTINUE_CMD(include_timestamp)

			err_ocurr = True
			ns.Write(data, 0, data.Length)

			Dim wait_result As Integer = WaitHandle.WaitAny(waithandles, CMD_RESPONSE_TIME_OUT, False)
			If wait_result = 1 Then
				Return CMD_RETURN.INVALID_COMMAND
			ElseIf wait_result > 1 Then
				Return CMD_RETURN.CMD_RESPONSE_TIME_OUT
			Else
				err_ocurr = If((inventoryContinueRspEvent.data(0) = 1), True, False)
				Return CMD_RETURN.COMMAND_SUCESS
			End If
		End Function

		''' <summary>
		''' Get Operation command set verison
		''' </summary>
		''' <param name="rsp"></param>
		''' <returns></returns>
		Public Function GetOCSVersion(ByRef rsp As OPERATION_CMD.OCS_VERSION_RSP) As CMD_RETURN
			getOCSVersionRspEvent = New SpeedwayManualResetEvent(False)
			invalidCommandNtfEvent = New SpeedwayManualResetEvent(False)
			Dim waithandles As WaitHandle() = New WaitHandle() {getOCSVersionRspEvent.evt, invalidCommandNtfEvent.evt}

			Dim data As Byte() = OPERATION_CMD.GENERATE_GET_OCS_VERSION_CMD(include_timestamp)
			ns.Write(data, 0, data.Length)
			rsp = Nothing

			Dim wait_result As Integer = WaitHandle.WaitAny(waithandles, CMD_RESPONSE_TIME_OUT, False)
			If wait_result = 1 Then
				Return CMD_RETURN.INVALID_COMMAND
			ElseIf wait_result > 1 Then
				Return CMD_RETURN.CMD_RESPONSE_TIME_OUT
			End If

			Try
				rsp = New OPERATION_CMD.OCS_VERSION_RSP(getOCSVersionRspEvent.data)
				Return CMD_RETURN.COMMAND_SUCESS
			Catch
				Return CMD_RETURN.RESPONSE_CAN_NOT_RECOGNIZED
			End Try

		End Function

		''' <summary>
		''' Load modem parameters from profile
		''' </summary>
		''' <param name="profile_index">profile index, value is 0-15</param>
		''' <returns></returns>
		Public Function LoadFromProfile(profile_index As UInt16) As CMD_RETURN
			loadFromProfileRspEvent = New SpeedwayManualResetEvent(False)
			invalidCommandNtfEvent = New SpeedwayManualResetEvent(False)
			Dim waithandles As WaitHandle() = New WaitHandle() {loadFromProfileRspEvent.evt, invalidCommandNtfEvent.evt}

			Dim data As Byte() = OPERATION_CMD.GENERATE_LOAD_FROM_PROFILE_CMD(include_timestamp, profile_index)
			ns.Write(data, 0, data.Length)

			Dim wait_result As Integer = WaitHandle.WaitAny(waithandles, CMD_RESPONSE_TIME_OUT, False)
			If wait_result = 1 Then
				Return CMD_RETURN.INVALID_COMMAND
			ElseIf wait_result > 1 Then
				Return CMD_RETURN.CMD_RESPONSE_TIME_OUT
			Else
				Return CMD_RETURN.COMMAND_SUCESS
			End If

		End Function

		''' <summary>
		''' Store modem parameters to profile
		''' </summary>
		''' <param name="profile_index">profile index, value is 0-15</param>
		''' <param name="dst"></param>
		''' <returns></returns>
		Public Function StoreToProfile(profile_index As UInt16, dst As OPERATION_CMD.SOURCE) As CMD_RETURN
			storeToProfileRspEvent = New SpeedwayManualResetEvent(False)
			invalidCommandNtfEvent = New SpeedwayManualResetEvent(False)
			Dim waithandles As WaitHandle() = New WaitHandle() {storeToProfileRspEvent.evt, invalidCommandNtfEvent.evt}

			Dim data As Byte() = OPERATION_CMD.GENERATE_STORE_TO_PROFILE_CMD(include_timestamp, profile_index, dst)
			ns.Write(data, 0, data.Length)

			Dim wait_result As Integer = WaitHandle.WaitAny(waithandles, CMD_RESPONSE_TIME_OUT, False)
			If wait_result = 1 Then
				Return CMD_RETURN.INVALID_COMMAND
			ElseIf wait_result > 1 Then
				Return CMD_RETURN.CMD_RESPONSE_TIME_OUT
			Else
				Return CMD_RETURN.COMMAND_SUCESS
			End If
		End Function

		''' <summary>
		''' Set tx power
		''' </summary>
		''' <param name="power">Array of tx power, array size is 1 or 5, if only one element presents, 
		''' power is applied to all the antenna ports. else element 2-5 are individual tx power for 
		''' each antenna port.  Value is 15.0 - 32.5</param>
		''' <param name="result_code"></param>
		''' <returns></returns>
		Public Function SetTxPower(power As Single(), ByRef result_code As OPERATION_CMD.SET_TX_POWER_RESULT) As CMD_RETURN
			Dim power_index As Byte() = New Byte(power.Length - 1) {}
			For i As Integer = 0 To power.Length - 1
				power_index(i) = CByte(Math.Truncate(68 + power(i) * 4))
			Next

			setTxPowerRspEvent = New SpeedwayManualResetEvent(False)
			invalidCommandNtfEvent = New SpeedwayManualResetEvent(False)
			Dim waithandles As WaitHandle() = New WaitHandle() {setTxPowerRspEvent.evt, invalidCommandNtfEvent.evt}

			Dim data As Byte() = OPERATION_CMD.GENERATE_SET_TX_POWER_CMD(include_timestamp, power_index)
			result_code = 0
			ns.Write(data, 0, data.Length)
			Dim wait_result As Integer = WaitHandle.WaitAny(waithandles, CMD_RESPONSE_TIME_OUT, False)
			If wait_result = 1 Then
				Return CMD_RETURN.INVALID_COMMAND
			ElseIf wait_result > 1 Then
				Return CMD_RETURN.CMD_RESPONSE_TIME_OUT
			Else
				result_code = CType(setTxPowerRspEvent.data(0), OPERATION_CMD.SET_TX_POWER_RESULT)
				Return CMD_RETURN.COMMAND_SUCESS
			End If
		End Function

		''' <summary>
		''' Get tx power of each antenna
		''' </summary>
		''' <param name="powers"></param>
		''' <returns></returns>
		Public Function GetTxPower(ByRef powers As Single()) As CMD_RETURN
			getTxPowerRspEvent = New SpeedwayManualResetEvent(False)
			invalidCommandNtfEvent = New SpeedwayManualResetEvent(False)
			Dim waithandles As WaitHandle() = New WaitHandle() {getTxPowerRspEvent.evt, invalidCommandNtfEvent.evt}

			Dim data As Byte() = OPERATION_CMD.GENERATE_GET_TX_POWER_CMD(include_timestamp)
			powers = Nothing
			ns.Write(data, 0, data.Length)

			Dim wait_result As Integer = WaitHandle.WaitAny(waithandles, CMD_RESPONSE_TIME_OUT, False)
			If wait_result = 1 Then
				Return CMD_RETURN.INVALID_COMMAND
			ElseIf wait_result > 1 Then
				Return CMD_RETURN.CMD_RESPONSE_TIME_OUT
			Else

				If getTxPowerRspEvent.data.Length = 1 Then
					powers = New Single(0) {}
					powers(0) = CSng((getTxPowerRspEvent.data(0) - 68) / 4.0)
				ElseIf getTxPowerRspEvent.data.Length >= 4 Then
					powers = New Single(getTxPowerRspEvent.data.Length - 4) {}
					powers(0) = CSng((getTxPowerRspEvent.data(0) - 68) / 4.0)

					For i As Integer = 1 To powers.Length - 1
						powers(i) = CSng((getTxPowerRspEvent.data(3 + i) - 68) / 4.0)
					Next
				End If
				Return CMD_RETURN.COMMAND_SUCESS
			End If

		End Function

		''' <summary>
		''' Enable/Disable antenna
		''' </summary>
		''' <param name="antennas"> Antenna to be enabled/disabled. e.g. ANTENNA_PORT_1|ANTENNA_PORT_2 </param>
		''' <param name="result"></param>
		''' <returns></returns>
		Public Function SetAntenna(antennas As Byte, ByRef result As OPERATION_CMD.SET_ANTENNA_RESULT) As CMD_RETURN
			setAntennaRspEvent = New SpeedwayManualResetEvent(False)
			invalidCommandNtfEvent = New SpeedwayManualResetEvent(False)
			Dim waithandles As WaitHandle() = New WaitHandle() {setAntennaRspEvent.evt, invalidCommandNtfEvent.evt}

			Dim data As Byte() = OPERATION_CMD.GENERATE_SET_ANTENNA_CMD(include_timestamp, antennas)
			result = OPERATION_CMD.SET_ANTENNA_RESULT.PORT_NOT_AVAILABLE
			ns.Write(data, 0, data.Length)

			Dim wait_result As Integer = WaitHandle.WaitAny(waithandles, CMD_RESPONSE_TIME_OUT, False)
			If wait_result = 1 Then
				Return CMD_RETURN.INVALID_COMMAND
			ElseIf wait_result > 1 Then
				Return CMD_RETURN.CMD_RESPONSE_TIME_OUT
			Else
				result = CType(setAntennaRspEvent.data(0), OPERATION_CMD.SET_ANTENNA_RESULT)
				Return CMD_RETURN.COMMAND_SUCESS
			End If

		End Function

		''' <summary>
		''' Get antenna port status
		''' </summary>
		''' <param name="antenna_port_status">status array, element 0 is antenna 1</param>
		''' <returns></returns>
		Public Function GetAntenna(ByRef antenna_port_status As Boolean()) As CMD_RETURN
			antenna_port_status = New Boolean(3) {}
			getAntennaRspEvent = New SpeedwayManualResetEvent(False)
			invalidCommandNtfEvent = New SpeedwayManualResetEvent(False)
			Dim waithandles As WaitHandle() = New WaitHandle() {getAntennaRspEvent.evt, invalidCommandNtfEvent.evt}
			Dim data As Byte() = OPERATION_CMD.GENERATE_GET_ANTENNA_CMD(include_timestamp)

			'antenna_byte = 0;
			ns.Write(data, 0, data.Length)
			Dim wait_result As Integer = WaitHandle.WaitAny(waithandles, CMD_RESPONSE_TIME_OUT, False)
			If wait_result = 1 Then
				Return CMD_RETURN.INVALID_COMMAND
			ElseIf wait_result > 1 Then
				Return CMD_RETURN.CMD_RESPONSE_TIME_OUT
			Else
				antenna_port_status(3) = If(((getAntennaRspEvent.data(0) And &H8) = &H8), True, False)
				antenna_port_status(2) = If(((getAntennaRspEvent.data(0) And &H4) = &H4), True, False)
				antenna_port_status(1) = If(((getAntennaRspEvent.data(0) And &H2) = &H2), True, False)
				antenna_port_status(0) = If(((getAntennaRspEvent.data(0) And &H1) = &H1), True, False)

				Return CMD_RETURN.COMMAND_SUCESS
			End If
		End Function

		''' <summary>
		''' Get tx power capacities
		''' </summary>
		''' <param name="reportPower">enable/disable reporting power</param>
		''' <param name="reportFrequencyList">enable/diable report frequency list</param>
		''' <param name="min_support_power"></param>
		''' <param name="max_support_power"></param>
		''' <param name="frequencies">return supported frequency list</param>
		''' <returns></returns>
		Public Function GetCapacities(reportPower As Boolean, reportFrequencyList As Boolean, ByRef min_support_power As Single, ByRef max_support_power As Single, ByRef frequencies As ArrayList) As CMD_RETURN
			frequencies = Nothing

			getCapabilitiesRspEvent = New SpeedwayManualResetEvent(False)
			invalidCommandNtfEvent = New SpeedwayManualResetEvent(False)
			Dim waithandles As WaitHandle() = New WaitHandle() {getCapabilitiesRspEvent.evt, invalidCommandNtfEvent.evt}

			Dim data As Byte() = OPERATION_CMD.GENERATE_GET_CAPABILITIES_CMD(include_timestamp, reportPower, reportFrequencyList)
			min_support_power = 0F
			max_support_power = 0F

			ns.Write(data, 0, data.Length)
			Dim wait_result As Integer = WaitHandle.WaitAny(waithandles, CMD_RESPONSE_TIME_OUT, False)
			If wait_result = 1 Then
				Return CMD_RETURN.INVALID_COMMAND
			ElseIf wait_result > 1 Then
				Return CMD_RETURN.CMD_RESPONSE_TIME_OUT
			Else
				Try
					Dim offset As Integer = 0

					While offset < getCapabilitiesRspEvent.data.Length
                        Select Case getCapabilitiesRspEvent.data(System.Math.Min(System.Threading.Interlocked.Increment(offset), offset - 1))
                            Case &H1
                                min_support_power = CSng((getCapabilitiesRspEvent.data(System.Math.Min(System.Threading.Interlocked.Increment(offset), offset - 1)) - 68) * 0.25)
                                Exit Select
                            Case &H2
                                max_support_power = CSng((getCapabilitiesRspEvent.data(System.Math.Min(System.Threading.Interlocked.Increment(offset), offset - 1)) - 68) * 0.25)
                                Exit Select
                            Case &H3
                                frequencies = New ArrayList()
                                Dim len As Integer = getCapabilitiesRspEvent.data(System.Math.Min(System.Threading.Interlocked.Increment(offset), offset - 1)) << 8 + getCapabilitiesRspEvent.data(System.Math.Min(System.Threading.Interlocked.Increment(offset), offset - 1))
                                For i As Integer = 0 To len \ 2 - 1
                                    frequencies.Add(getCapabilitiesRspEvent.data(System.Math.Min(System.Threading.Interlocked.Increment(offset), offset - 1)) << 8 + getCapabilitiesRspEvent.data(System.Math.Min(System.Threading.Interlocked.Increment(offset), offset - 1)))
                                Next
								Exit Select
							Case Else
								offset += 1
								Exit Select
						End Select
					End While

					Return CMD_RETURN.COMMAND_SUCESS
				Catch
					Return CMD_RETURN.RESPONSE_CAN_NOT_RECOGNIZED
				End Try
			End If

		End Function

		''' <summary>
		''' Set tx frequency
		''' </summary>
		''' <param name="mode"></param>
		''' <param name="center_frequency_index">frequency index. valid only when mode is CENTER_FREQUENCY. value is 0-2000, as 860MHz-960Mhz</param>
		''' <param name="frequency_list">List of frequency. valid only when mode is CHOOSE_FROM_LIST. value is 0-2000 </param>
		''' <param name="result"></param>
		''' <returns></returns>
		Public Function SetTxFrequency(mode As OPERATION_CMD.FREQUENCY_SET_MODE, center_frequency_index As UInt16, frequency_list As UInt16(), ByRef result As OPERATION_CMD.SET_FREQUENCY_RESULT) As CMD_RETURN
			setTxFrequencyRspEvent = New SpeedwayManualResetEvent(False)
			invalidCommandNtfEvent = New SpeedwayManualResetEvent(False)
			Dim waithandles As WaitHandle() = New WaitHandle() {setTxFrequencyRspEvent.evt, invalidCommandNtfEvent.evt}

			Dim data As Byte() = OPERATION_CMD.GENERATE_SET_TX_FREQUENCY(include_timestamp, mode, center_frequency_index, frequency_list, New UInt16(-1) {})

			ns.Write(data, 0, data.Length)
			result = OPERATION_CMD.SET_FREQUENCY_RESULT.[ERROR]
			Dim wait_result As Integer = WaitHandle.WaitAny(waithandles, CMD_RESPONSE_TIME_OUT, False)
			If wait_result = 1 Then
				Return CMD_RETURN.INVALID_COMMAND
			ElseIf wait_result > 1 Then
				Return CMD_RETURN.CMD_RESPONSE_TIME_OUT
			Else
				result = CType(setTxFrequencyRspEvent.data(0), OPERATION_CMD.SET_FREQUENCY_RESULT)
				Return CMD_RETURN.COMMAND_SUCESS
			End If
		End Function

		''' <summary>
		''' Set tx frequency with Reduced Power Frequecy List as parameters
		''' </summary>
		''' <param name="mode"></param>
		''' <param name="center_frequency_index">frequency index. valid only when mode is CENTER_FREQUENCY. value is 0-2000, as 860MHz-960Mhz</param>
		''' <param name="frequency_list">List of frequency. valid only when mode is CHOOSE_FROM_LIST. value is 0-2000 </param>
		''' <param name="reduced_power_frequency_list">List of reduced power frequecy, only supported by FCC reader</param>
		''' <param name="result"></param>
		''' <returns></returns>
		Public Function SetTxFrequency(mode As OPERATION_CMD.FREQUENCY_SET_MODE, center_frequency_index As UInt16, frequency_list As UInt16(), reduced_power_frequency_list As UInt16(), ByRef result As OPERATION_CMD.SET_FREQUENCY_RESULT) As CMD_RETURN
			setTxFrequencyRspEvent = New SpeedwayManualResetEvent(False)
			invalidCommandNtfEvent = New SpeedwayManualResetEvent(False)
			Dim waithandles As WaitHandle() = New WaitHandle() {setTxFrequencyRspEvent.evt, invalidCommandNtfEvent.evt}

			Dim data As Byte() = OPERATION_CMD.GENERATE_SET_TX_FREQUENCY(include_timestamp, mode, center_frequency_index, frequency_list, reduced_power_frequency_list)

			ns.Write(data, 0, data.Length)
			result = OPERATION_CMD.SET_FREQUENCY_RESULT.[ERROR]
			Dim wait_result As Integer = WaitHandle.WaitAny(waithandles, CMD_RESPONSE_TIME_OUT, False)
			If wait_result = 1 Then
				Return CMD_RETURN.INVALID_COMMAND
			ElseIf wait_result > 1 Then
				Return CMD_RETURN.CMD_RESPONSE_TIME_OUT
			Else
				result = CType(setTxFrequencyRspEvent.data(0), OPERATION_CMD.SET_FREQUENCY_RESULT)
				Return CMD_RETURN.COMMAND_SUCESS
			End If
		End Function

		''' <summary>
		''' Set Gen 2 parameters
		''' </summary>
		''' <param name="gen2_param"></param>
		''' <param name="result"></param>
		''' <returns></returns>
		Public Function SetGen2Params(gen2_param As OPERATION_CMD.GEN2_PARAM, ByRef result As OPERATION_CMD.SET_GEN2_PARAMS_RESULT) As CMD_RETURN
			setGen2ParamsRspEvent = New SpeedwayManualResetEvent(False)
			invalidCommandNtfEvent = New SpeedwayManualResetEvent(False)
			Dim waithandles As WaitHandle() = New WaitHandle() {setGen2ParamsRspEvent.evt, invalidCommandNtfEvent.evt}

			Dim data As Byte() = OPERATION_CMD.GENERATE_SET_GEN2_PARAMS_CMD(include_timestamp, gen2_param)

			result = OPERATION_CMD.SET_GEN2_PARAMS_RESULT.SESSION_AND_INVENTORY_MODE_COMBINATION_NOT_SUPPORTED
			ns.Write(data, 0, data.Length)
			Dim wait_result As Integer = WaitHandle.WaitAny(waithandles, CMD_RESPONSE_TIME_OUT, False)
			If wait_result = 1 Then
				Return CMD_RETURN.INVALID_COMMAND
			ElseIf wait_result > 1 Then
				Return CMD_RETURN.CMD_RESPONSE_TIME_OUT
			Else
				result = CType(setGen2ParamsRspEvent.data(0), OPERATION_CMD.SET_GEN2_PARAMS_RESULT)
				Return CMD_RETURN.COMMAND_SUCESS
			End If


		End Function

		''' <summary>
		''' Get Gen 2 parameters
		''' </summary>
		''' <param name="gen2_param"></param>
		''' <returns></returns>
		Public Function GetGen2Params(ByRef gen2_param As OPERATION_CMD.GEN2_PARAM) As CMD_RETURN
			getGen2ParamsRspEvent = New SpeedwayManualResetEvent(False)
			invalidCommandNtfEvent = New SpeedwayManualResetEvent(False)
			Dim waithandles As WaitHandle() = New WaitHandle() {getGen2ParamsRspEvent.evt, invalidCommandNtfEvent.evt}

			Dim data As Byte() = OPERATION_CMD.GENERATE_GET_GEN2_PARAMS_CMD(include_timestamp)

			gen2_param = Nothing
			ns.Write(data, 0, data.Length)
			Dim wait_result As Integer = WaitHandle.WaitAny(waithandles, CMD_RESPONSE_TIME_OUT, False)
			If wait_result = 1 Then
				Return CMD_RETURN.INVALID_COMMAND
			ElseIf wait_result > 1 Then
				Return CMD_RETURN.CMD_RESPONSE_TIME_OUT
			End If
			Try
				gen2_param = New OPERATION_CMD.GEN2_PARAM(getGen2ParamsRspEvent.data)
				Return CMD_RETURN.COMMAND_SUCESS
			Catch
				Return CMD_RETURN.RESPONSE_CAN_NOT_RECOGNIZED
			End Try

		End Function

		''' <summary>
		''' Get Gen 2 parameters
		''' </summary>
		''' <param name="gen2_param"></param>
		''' <returns></returns>
		Public Function GetGen2Params(report_search_mode As Boolean, ByRef gen2_param As OPERATION_CMD.GEN2_PARAM) As CMD_RETURN
			getGen2ParamsRspEvent = New SpeedwayManualResetEvent(False)
			invalidCommandNtfEvent = New SpeedwayManualResetEvent(False)
			Dim waithandles As WaitHandle() = New WaitHandle() {getGen2ParamsRspEvent.evt, invalidCommandNtfEvent.evt}

			Dim data As Byte() = OPERATION_CMD.GENERATE_GET_GEN2_PARAMS_CMD(report_search_mode, include_timestamp)

			gen2_param = Nothing
			ns.Write(data, 0, data.Length)
			Dim wait_result As Integer = WaitHandle.WaitAny(waithandles, CMD_RESPONSE_TIME_OUT, False)
			If wait_result = 1 Then
				Return CMD_RETURN.INVALID_COMMAND
			ElseIf wait_result > 1 Then
				Return CMD_RETURN.CMD_RESPONSE_TIME_OUT
			End If
			Try
				gen2_param = New OPERATION_CMD.GEN2_PARAM(getGen2ParamsRspEvent.data)
				Return CMD_RETURN.COMMAND_SUCESS
			Catch
				Return CMD_RETURN.RESPONSE_CAN_NOT_RECOGNIZED
			End Try

		End Function

		''' <summary>
		''' Set receive sensitivity.
		''' </summary>
		''' <param name="mode"></param>
		''' <param name="sensitivities">value is -30 to -128. if mode = DEFAULT. This parameter is ignored.</param>
		''' <param name="err_ocurr"></param>
		''' <returns></returns>
		Public Function SetRxConfig(mode As OPERATION_CMD.SET_RX_SENSITIVITY_MODE, sensitivities As Short(), ByRef err_ocurr As Boolean) As CMD_RETURN
			setRxConfigRspEvent = New SpeedwayManualResetEvent(False)
			invalidCommandNtfEvent = New SpeedwayManualResetEvent(False)
			Dim waithandles As WaitHandle() = New WaitHandle() {setRxConfigRspEvent.evt, invalidCommandNtfEvent.evt}
			Dim data As Byte() = OPERATION_CMD.GENERATE_SET_RX_CONFIG_CMD(mode, sensitivities, include_timestamp)

			err_ocurr = True

			ns.Write(data, 0, data.Length)
			Dim wait_result As Integer = WaitHandle.WaitAny(waithandles, CMD_RESPONSE_TIME_OUT, False)
			If wait_result = 1 Then
				Return CMD_RETURN.INVALID_COMMAND
			ElseIf wait_result > 1 Then
				Return CMD_RETURN.CMD_RESPONSE_TIME_OUT
			End If
			Try
				err_ocurr = If((setRxConfigRspEvent.data(0) = 1), True, False)
				Return CMD_RETURN.COMMAND_SUCESS
			Catch
				Return CMD_RETURN.RESPONSE_CAN_NOT_RECOGNIZED
			End Try
		End Function

		''' <summary>
		''' Get receive sensitivity
		''' </summary>
		''' <param name="mode"></param>
		''' <param name="sensitivities"></param>
		''' <returns></returns>
		Public Function GetRxConfig(ByRef mode As OPERATION_CMD.SET_RX_SENSITIVITY_MODE, ByRef sensitivities As Short()) As CMD_RETURN
			getRxConfigRspEvent = New SpeedwayManualResetEvent(False)
			invalidCommandNtfEvent = New SpeedwayManualResetEvent(False)
			Dim waithandles As WaitHandle() = New WaitHandle() {getRxConfigRspEvent.evt, invalidCommandNtfEvent.evt}

			Dim data As Byte() = OPERATION_CMD.GENERATE_GET_RX_CONFIG_CMD(include_timestamp)

			mode = OPERATION_CMD.SET_RX_SENSITIVITY_MODE.MAXIMUM_SENSITIVITY
			sensitivities = Nothing

			ns.Write(data, 0, data.Length)

			Dim wait_result As Integer = WaitHandle.WaitAny(waithandles, CMD_RESPONSE_TIME_OUT, False)
			If wait_result = 1 Then
				Return CMD_RETURN.INVALID_COMMAND
			ElseIf wait_result > 1 Then
				Return CMD_RETURN.CMD_RESPONSE_TIME_OUT
			End If
			Try
				mode = CType(getRxConfigRspEvent.data(0), OPERATION_CMD.SET_RX_SENSITIVITY_MODE)
				If getRxConfigRspEvent.data.Length > 3 Then
					Dim len As Integer = getRxConfigRspEvent.data.Length - 4
					sensitivities = New Short(len - 1) {}
					For i As Integer = 0 To len - 1
						sensitivities(i) = CShort(0 - ((getRxConfigRspEvent.data(4 + i) Xor &Hff) + 1))
					Next
				End If

				Return CMD_RETURN.COMMAND_SUCESS
			Catch
				Return CMD_RETURN.RESPONSE_CAN_NOT_RECOGNIZED
			End Try
		End Function

		''' <summary>
		''' Get tx frequency
		''' </summary>
		''' <param name="mode"></param>
		''' <param name="center_frequency"></param>
		''' <param name="frequency_list"></param>
		''' <returns></returns>
		Public Function GetTxFrequency(ByRef mode As OPERATION_CMD.FREQUENCY_SET_MODE, ByRef center_frequency As UInt16, ByRef frequency_list As UInt16()) As CMD_RETURN
			getTxFrequencyRspEvent = New SpeedwayManualResetEvent(False)
			invalidCommandNtfEvent = New SpeedwayManualResetEvent(False)
			Dim waithandles As WaitHandle() = New WaitHandle() {getTxFrequencyRspEvent.evt, invalidCommandNtfEvent.evt}

			mode = OPERATION_CMD.FREQUENCY_SET_MODE.CHOOSE_FROM_REGULATORY
			center_frequency = 0
			frequency_list = Nothing

			Dim data As Byte() = OPERATION_CMD.GENERATE_GET_TX_FREQUENCY(include_timestamp)
			ns.Write(data, 0, data.Length)

			Dim wait_result As Integer = WaitHandle.WaitAny(waithandles, CMD_RESPONSE_TIME_OUT, False)
			If wait_result = 1 Then
				Return CMD_RETURN.INVALID_COMMAND
			ElseIf wait_result > 1 Then
				Return CMD_RETURN.CMD_RESPONSE_TIME_OUT
			End If

			Try
				mode = CType(getTxFrequencyRspEvent.data(0), OPERATION_CMD.FREQUENCY_SET_MODE)

				If mode = OPERATION_CMD.FREQUENCY_SET_MODE.CENTER_FREQUENCY Then
					center_frequency = CType(getTxFrequencyRspEvent.data(2) * 256 + getTxFrequencyRspEvent.data(3), UInt16)
				ElseIf mode = OPERATION_CMD.FREQUENCY_SET_MODE.CHOOSE_FROM_LIST Then
					Dim len As Integer = getTxFrequencyRspEvent.data.Length - 4
					frequency_list = New UInt16(len - 1) {}

					For i As Integer = 0 To len \ 2 - 1
						Try
							frequency_list(i) = CType(getTxFrequencyRspEvent.data(4 + 2 * i) * 256 + getTxFrequencyRspEvent.data(5 + 2 * i), UInt16)
						Catch
						End Try
					Next
				End If

				Return CMD_RETURN.COMMAND_SUCESS
			Catch
				Return CMD_RETURN.RESPONSE_CAN_NOT_RECOGNIZED
			End Try

		End Function

		''' <summary>
		''' Get tx frequency
		''' </summary>
		''' <param name="mode"></param>
		''' <param name="center_frequency"></param>
		''' <param name="frequency_list"></param>
		''' <param name="reducedPowerFrequency_list"></param>
		''' <returns></returns>
		Public Function GetTxFrequency(ByRef mode As OPERATION_CMD.FREQUENCY_SET_MODE, ByRef center_frequency As UInt16, ByRef frequency_list As UInt16(), ByRef reducedPowerFrequency_list As UInt16()) As CMD_RETURN
			getTxFrequencyRspEvent = New SpeedwayManualResetEvent(False)
			invalidCommandNtfEvent = New SpeedwayManualResetEvent(False)
			Dim waithandles As WaitHandle() = New WaitHandle() {getTxFrequencyRspEvent.evt, invalidCommandNtfEvent.evt}

			mode = OPERATION_CMD.FREQUENCY_SET_MODE.CHOOSE_FROM_REGULATORY
			center_frequency = 0

			frequency_list = Nothing
			reducedPowerFrequency_list = Nothing

			Dim data As Byte() = OPERATION_CMD.GENERATE_GET_TX_FREQUENCY(include_timestamp)
			ns.Write(data, 0, data.Length)

			Dim wait_result As Integer = WaitHandle.WaitAny(waithandles, CMD_RESPONSE_TIME_OUT, False)
			If wait_result = 1 Then
				Return CMD_RETURN.INVALID_COMMAND
			ElseIf wait_result > 1 Then
				Return CMD_RETURN.CMD_RESPONSE_TIME_OUT
			End If

			Try
				mode = CType(getTxFrequencyRspEvent.data(0), OPERATION_CMD.FREQUENCY_SET_MODE)

				If mode = OPERATION_CMD.FREQUENCY_SET_MODE.CENTER_FREQUENCY Then
					center_frequency = CType(getTxFrequencyRspEvent.data(2) * 256 + getTxFrequencyRspEvent.data(3), UInt16)
				ElseIf mode = OPERATION_CMD.FREQUENCY_SET_MODE.CHOOSE_FROM_LIST Then
					Dim len As Integer = getTxFrequencyRspEvent.data(2) >> 8 + getTxFrequencyRspEvent.data(3)

					frequency_list = New UInt16(len - 1) {}

					For i As Integer = 0 To len \ 2 - 1
						Try
							frequency_list(i) = CType(getTxFrequencyRspEvent.data(4 + 2 * i) * 256 + getTxFrequencyRspEvent.data(5 + 2 * i), UInt16)
						Catch
						End Try
					Next
				ElseIf mode = OPERATION_CMD.FREQUENCY_SET_MODE.REDUCED_POWER_FREQUENCY_LIST Then
					Dim len As Integer = getTxFrequencyRspEvent.data(2) >> 8 + getTxFrequencyRspEvent.data(3)

					reducedPowerFrequency_list = New UInt16(len - 1) {}

					For i As Integer = 0 To len \ 2 - 1
						Try
							reducedPowerFrequency_list(i) = CType(getTxFrequencyRspEvent.data(4 + 2 * i) * 256 + getTxFrequencyRspEvent.data(5 + 2 * i), UInt16)
						Catch
						End Try
					Next
				End If

				Return CMD_RETURN.COMMAND_SUCESS
			Catch
				Return CMD_RETURN.RESPONSE_CAN_NOT_RECOGNIZED
			End Try

		End Function


		''' <summary>
		''' Get supported Gen2 paramters
		''' </summary>
		''' <param name="include_mode_id">indicate if contain mode id in return data</param>
		''' <param name="include_inventory_search_mode">indicate if contain inventory search mode in return data</param>
		''' <param name="rsp"></param>
		''' <returns></returns>
		Public Function GetSupportedGen2Params(include_mode_id As Boolean, include_inventory_search_mode As Boolean, ByRef rsp As OPERATION_CMD.SUPPORTED_GEN2_PARAMS_RSP) As CMD_RETURN
			getSupportedGen2ParamsRspEvent = New SpeedwayManualResetEvent(False)
			invalidCommandNtfEvent = New SpeedwayManualResetEvent(False)
			Dim waithandles As WaitHandle() = New WaitHandle() {getSupportedGen2ParamsRspEvent.evt, invalidCommandNtfEvent.evt}

			Dim data As Byte() = OPERATION_CMD.GENERATE_GET_SUPPORTED_GEN2_PARAMS_CMD(include_timestamp, include_mode_id, include_inventory_search_mode)
			ns.Write(data, 0, data.Length)

			rsp = Nothing
			Dim wait_result As Integer = WaitHandle.WaitAny(waithandles, CMD_RESPONSE_TIME_OUT, False)
			If wait_result = 1 Then
				Return CMD_RETURN.INVALID_COMMAND
			ElseIf wait_result > 1 Then
				Return CMD_RETURN.CMD_RESPONSE_TIME_OUT
			End If

			Try
				rsp = New OPERATION_CMD.SUPPORTED_GEN2_PARAMS_RSP(getSupportedGen2ParamsRspEvent.data)
				Return CMD_RETURN.COMMAND_SUCESS
			Catch
				Return CMD_RETURN.RESPONSE_CAN_NOT_RECOGNIZED
			End Try
		End Function

		''' <summary>
		''' Check antenna status
		''' </summary>
		''' <param name="ant_ntf"></param>
		''' <returns></returns>
		Public Function CheckAntenna(ByRef ant_ntf As OPERATION_NTF.CHECK_ANTENNA_NTF) As CMD_RETURN
			ant_ntf = Nothing

			checkAntennaRspEvent = New SpeedwayManualResetEvent(False)
			checkAntennaNtfEvent = New SpeedwayManualResetEvent(False)
			invalidCommandNtfEvent = New SpeedwayManualResetEvent(False)
			Dim waithandles As WaitHandle() = New WaitHandle() {checkAntennaRspEvent.evt, invalidCommandNtfEvent.evt}


			Dim data As Byte() = OPERATION_CMD.GENERATE_CHECK_ANTENNA_CMD(include_timestamp)

			ns.Write(data, 0, data.Length)
			Dim wait_result As Integer = WaitHandle.WaitAny(waithandles, CMD_RESPONSE_TIME_OUT, False)
			If wait_result = 1 Then
				Return CMD_RETURN.INVALID_COMMAND
			ElseIf wait_result > 1 Then
				Return CMD_RETURN.CMD_RESPONSE_TIME_OUT
			End If

			If Not checkAntennaNtfEvent.evt.WaitOne(CMD_RESPONSE_TIME_OUT, False) Then
				Return CMD_RETURN.COMMAND_FAILED
			End If

			Try
				ant_ntf = New OPERATION_NTF.CHECK_ANTENNA_NTF(checkAntennaNtfEvent.data)
				Return CMD_RETURN.COMMAND_SUCESS
			Catch
				Return CMD_RETURN.COMMAND_FAILED
			End Try
		End Function

		''' <summary>
		''' Enable/Disable inventory report 
		''' </summary>
		''' <param name="enable"></param>
		''' <param name="err_occur"></param>
		''' <returns></returns>
		Public Function SetInventoryReport(enable As Boolean, param As OPERATION_CMD.OPTIONAL_INVENTORY_REPORT_PARAM, ByRef err_occur As Boolean) As CMD_RETURN
			setInventoryReportRspEvent = New SpeedwayManualResetEvent(False)
			invalidCommandNtfEvent = New SpeedwayManualResetEvent(False)
			Dim waithandles As WaitHandle() = New WaitHandle() {setInventoryReportRspEvent.evt, invalidCommandNtfEvent.evt}

			Dim data As Byte() = OPERATION_CMD.GENERATE_SET_INVENTORY_REPORT(include_timestamp, Not enable, param)

			err_occur = True
			ns.Write(data, 0, data.Length)
			Dim wait_result As Integer = WaitHandle.WaitAny(waithandles, CMD_RESPONSE_TIME_OUT, False)
			If wait_result = 1 Then
				Return CMD_RETURN.INVALID_COMMAND
			ElseIf wait_result > 1 Then
				Return CMD_RETURN.CMD_RESPONSE_TIME_OUT
			Else
				err_occur = If((setInventoryReportRspEvent.data(0) = 1), True, False)
				Return CMD_RETURN.COMMAND_SUCESS
			End If

		End Function

		''' <summary>
		''' Enable Inventory report
		''' </summary>
		''' <param name="rst"></param>
		''' <returns></returns>
		Public Function ReportInventory(ByRef rst As OPERATION_CMD.REPORT_INVENTORY_RESULT) As CMD_RETURN
			reportInventoryRspEvent = New SpeedwayManualResetEvent(False)
			invalidCommandNtfEvent = New SpeedwayManualResetEvent(False)
			Dim waithandles As WaitHandle() = New WaitHandle() {reportInventoryRspEvent.evt, invalidCommandNtfEvent.evt}

			rst = OPERATION_CMD.REPORT_INVENTORY_RESULT.[ERROR]
			Dim data As Byte() = OPERATION_CMD.GENERATE_REPORT_INVENTORY_CMD(include_timestamp)

			ns.Write(data, 0, data.Length)
			Dim wait_result As Integer = WaitHandle.WaitAny(waithandles, CMD_RESPONSE_TIME_OUT, False)
			If wait_result = 1 Then
				Return CMD_RETURN.INVALID_COMMAND
			ElseIf wait_result > 1 Then
				Return CMD_RETURN.CMD_RESPONSE_TIME_OUT
			Else
				Try
					rst = CType(reportInventoryRspEvent.data(0), OPERATION_CMD.REPORT_INVENTORY_RESULT)
					Return CMD_RETURN.COMMAND_SUCESS
				Catch
					Return CMD_RETURN.RESPONSE_CAN_NOT_RECOGNIZED
				End Try
			End If
		End Function

		''' <summary>
		''' Set listen before talk parameters
		''' </summary>
		''' <param name="disable_auto_select"></param>
		''' <param name="err_ocurr"></param>
		''' <returns></returns>
		Public Function SetLBTParams(disable_auto_select As Boolean, ByRef err_ocurr As Boolean) As CMD_RETURN
			setLBTParamsRspEvent = New SpeedwayManualResetEvent(False)
			invalidCommandNtfEvent = New SpeedwayManualResetEvent(False)
			Dim waithandles As WaitHandle() = New WaitHandle() {setLBTParamsRspEvent.evt, invalidCommandNtfEvent.evt}
			Dim data As Byte() = OPERATION_CMD.GENERATE_SET_LBT_PARAMS_CMD(include_timestamp, disable_auto_select)

			err_ocurr = True
			ns.Write(data, 0, data.Length)
			Dim wait_result As Integer = WaitHandle.WaitAny(waithandles, CMD_RESPONSE_TIME_OUT, False)
			If wait_result = 1 Then
				Return CMD_RETURN.INVALID_COMMAND
			ElseIf wait_result > 1 Then
				Return CMD_RETURN.CMD_RESPONSE_TIME_OUT
			Else
				err_ocurr = If((setLBTParamsRspEvent.data(0) = 1), True, False)
				Return CMD_RETURN.COMMAND_SUCESS
			End If
		End Function

		''' <summary>
		''' Get listen before talk paramters
		''' </summary>
		''' <param name="lbt_time_mode"></param>        
		''' <param name="err_occur"></param>
		''' <returns></returns>
		Public Function GetLBTParams(ByRef lbt_time_mode As Integer, ByRef err_occur As Boolean) As CMD_RETURN
			getLBTParamRspEvent = New SpeedwayManualResetEvent(False)
			invalidCommandNtfEvent = New SpeedwayManualResetEvent(False)
			Dim waithandles As WaitHandle() = New WaitHandle() {getLBTParamRspEvent.evt, invalidCommandNtfEvent.evt}
			Dim data As Byte() = OPERATION_CMD.GENERATE_GET_LBT_PARAMS_CMD(include_timestamp)

			err_occur = True
			lbt_time_mode = 0

			ns.Write(data, 0, data.Length)
			Dim wait_result As Integer = WaitHandle.WaitAny(waithandles, CMD_RESPONSE_TIME_OUT, False)
			If wait_result = 1 Then
				Return CMD_RETURN.INVALID_COMMAND
			ElseIf wait_result > 1 Then
				Return CMD_RETURN.CMD_RESPONSE_TIME_OUT
			Else
				err_occur = If((getLBTParamRspEvent.data(0) = 1), True, False)
				If getLBTParamRspEvent.data.Length > 2 Then
					lbt_time_mode = CInt(getLBTParamRspEvent.data(1))
				End If

				Return CMD_RETURN.COMMAND_SUCESS
			End If
		End Function

		''' <summary>
		''' Stop modem
		''' </summary>
		''' <returns></returns>
		Public Function ModemStop() As CMD_RETURN
			modemStopRspEvent = New SpeedwayManualResetEvent(False)
			invalidCommandNtfEvent = New SpeedwayManualResetEvent(False)
			Dim waithandles As WaitHandle() = New WaitHandle() {modemStopRspEvent.evt, invalidCommandNtfEvent.evt}
			Dim data As Byte() = OPERATION_CMD.GENERATE_MODEM_STOP_CMD(include_timestamp)

			ns.Write(data, 0, data.Length)
			Dim wait_result As Integer = WaitHandle.WaitAny(waithandles, MODEM_STOP_TIME_OUT, False)
			If wait_result = 1 Then
				Return CMD_RETURN.INVALID_COMMAND
			ElseIf wait_result > 1 Then
				Return CMD_RETURN.CMD_RESPONSE_TIME_OUT
			Else
				Return CMD_RETURN.COMMAND_SUCESS
			End If
		End Function

		''' <summary>
		''' Set RF Survey parameters
		''' </summary>
		''' <param name="low_frequency_index"></param>
		''' <param name="high_frequency_index"></param>
		''' <param name="mb"></param>
		''' <param name="antenna_byte"></param>
		''' <param name="sample_count"></param>
		''' <param name="err_ocurr"></param>
		''' <returns></returns>
		Public Function RFSurvey(low_frequency_index As UInt16, high_frequency_index As UInt16, mb As OPERATION_CMD.MEASUREMENT_BANDWIDTH, antenna_byte As Byte, sample_count As UInt16, ByRef err_ocurr As Boolean) As CMD_RETURN
			rfSurveyRspEvent = New SpeedwayManualResetEvent(False)
			invalidCommandNtfEvent = New SpeedwayManualResetEvent(False)
			Dim waithandles As WaitHandle() = New WaitHandle() {rfSurveyRspEvent.evt, invalidCommandNtfEvent.evt}

			Dim data As Byte() = OPERATION_CMD.GENERATE_RF_SURVEY_CMD(include_timestamp, low_frequency_index, high_frequency_index, mb, antenna_byte, sample_count)

			err_ocurr = True

			ns.Write(data, 0, data.Length)

			Dim wait_result As Integer = WaitHandle.WaitAny(waithandles, CMD_RESPONSE_TIME_OUT, False)
			If wait_result = 1 Then
				Return CMD_RETURN.INVALID_COMMAND
			ElseIf wait_result > 1 Then
				Return CMD_RETURN.CMD_RESPONSE_TIME_OUT
			Else
				err_ocurr = If((rfSurveyRspEvent.data(0) = 1), True, False)
				Return CMD_RETURN.COMMAND_SUCESS
			End If
		End Function

		''' <summary>
		''' Read tag data
		''' </summary>
		''' <param name="mb">Memory bank</param>
		''' <param name="addr">Start address. in byte order</param>
		''' <param name="read_len">total words to be read. e.g. 96 bits EPC contains 6 words</param>
		''' <param name="password">access password</param>
		''' <param name="rsp"></param>
		''' <returns></returns>
		Public Function TagRead(mb As OPERATION_CMD.MEMORY_BANK, addr As UInt16, read_len As Byte, password As UInt32, ByRef rsp As OPERATION_NTF.TAG_READ_NTF) As CMD_RETURN
			tagReadRspEvent = New SpeedwayManualResetEvent(False)
			tagReadNtfEvent = New SpeedwayManualResetEvent(False)
			invalidCommandNtfEvent = New SpeedwayManualResetEvent(False)
			Dim waithandles As WaitHandle() = New WaitHandle() {tagReadRspEvent.evt, invalidCommandNtfEvent.evt}


			Dim data As Byte() = OPERATION_CMD.GENERATE_TAG_READ_CMD(include_timestamp, mb, addr, read_len, password)
			rsp = Nothing

			ns.Write(data, 0, data.Length)
			Dim wait_result As Integer = WaitHandle.WaitAny(waithandles, CMD_RESPONSE_TIME_OUT, False)
			If wait_result = 1 Then
				Return CMD_RETURN.INVALID_COMMAND
			ElseIf wait_result > 1 Then
				Return CMD_RETURN.CMD_RESPONSE_TIME_OUT
			End If

			If Not tagReadNtfEvent.evt.WaitOne(NOTIFICTION_TIME_OUT, False) Then
				Return CMD_RETURN.CMD_RESPONSE_TIME_OUT
			End If

			Try
				rsp = New OPERATION_NTF.TAG_READ_NTF(tagReadNtfEvent.data)
				Return CMD_RETURN.COMMAND_SUCESS
			Catch
				Return CMD_RETURN.RESPONSE_CAN_NOT_RECOGNIZED
			End Try
		End Function

		''' <summary>
		''' Read tag data
		''' </summary>
		''' <param name="mb">Memory bank</param>
		''' <param name="addr">Start address. in byte order</param>
		''' <param name="read_len">total words to be read. e.g. 96 bits EPC contains 6 words</param>
		''' <param name="rsp"></param>
		''' <returns></returns>
		Public Function TagRead(mb As OPERATION_CMD.MEMORY_BANK, addr As UInt16, read_len As Byte, ByRef rsp As OPERATION_NTF.TAG_READ_NTF) As CMD_RETURN
			tagReadRspEvent = New SpeedwayManualResetEvent(False)
			tagReadNtfEvent = New SpeedwayManualResetEvent(False)

			invalidCommandNtfEvent = New SpeedwayManualResetEvent(False)
			Dim waithandles As WaitHandle() = New WaitHandle() {tagReadRspEvent.evt, invalidCommandNtfEvent.evt}

			Dim data As Byte() = OPERATION_CMD.GENERATE_TAG_READ_CMD(include_timestamp, mb, addr, read_len)
			rsp = Nothing

			ns.Write(data, 0, data.Length)
			Dim wait_result As Integer = WaitHandle.WaitAny(waithandles, CMD_RESPONSE_TIME_OUT, False)
			If wait_result = 1 Then
				Return CMD_RETURN.INVALID_COMMAND
			ElseIf wait_result > 1 Then
				Return CMD_RETURN.CMD_RESPONSE_TIME_OUT
			End If

			If Not tagReadNtfEvent.evt.WaitOne(NOTIFICTION_TIME_OUT, False) Then
				Return CMD_RETURN.CMD_RESPONSE_TIME_OUT
			End If

			Try
				rsp = New OPERATION_NTF.TAG_READ_NTF(tagReadNtfEvent.data)
				Return CMD_RETURN.COMMAND_SUCESS
			Catch
				Return CMD_RETURN.RESPONSE_CAN_NOT_RECOGNIZED
			End Try
		End Function

		''' <summary>
		''' Write data to tag
		''' </summary>
		''' <param name="mb">Memory bank</param>
		''' <param name="addr">Start addres. in byte order</param>
		''' <param name="data">Data to be wrote</param>
		''' <param name="disable_block_write"></param>
		''' <param name="password">access word</param>
		''' <param name="rsp"></param>
		''' <returns></returns>
		Public Function TagWrite(mb As OPERATION_CMD.MEMORY_BANK, addr As UInt16, data As UInt16(), disable_block_write As Boolean, password As UInt32, ByRef rsp As OPERATION_NTF.TAG_WRITE_NTF) As CMD_RETURN
			tagWriteRspEvent = New SpeedwayManualResetEvent(False)
			tagWriteNtfEvent = New SpeedwayManualResetEvent(False)
			invalidCommandNtfEvent = New SpeedwayManualResetEvent(False)
			Dim waithandles As WaitHandle() = New WaitHandle() {tagWriteRspEvent.evt, invalidCommandNtfEvent.evt}

			Dim temp As Byte() = OPERATION_CMD.GENERATE_TAG_WRITE_CMD(include_timestamp, mb, addr, data, disable_block_write, password)

			rsp = Nothing

			ns.Write(temp, 0, temp.Length)
			Dim wait_result As Integer = WaitHandle.WaitAny(waithandles, CMD_RESPONSE_TIME_OUT, False)
			If wait_result = 1 Then
				Return CMD_RETURN.INVALID_COMMAND
			ElseIf wait_result > 1 Then
				Return CMD_RETURN.CMD_RESPONSE_TIME_OUT
			End If

			If Not tagWriteNtfEvent.evt.WaitOne(NOTIFICTION_TIME_OUT, False) Then
				Return CMD_RETURN.CMD_RESPONSE_TIME_OUT
			End If
			Try
				rsp = New OPERATION_NTF.TAG_WRITE_NTF(tagWriteNtfEvent.data)
				Return CMD_RETURN.COMMAND_SUCESS
			Catch
				Return CMD_RETURN.RESPONSE_CAN_NOT_RECOGNIZED
			End Try

		End Function

		''' <summary>
		''' Write data to tag
		''' </summary>
		''' <param name="mb">Memory bank</param>
		''' <param name="addr">Start addres. in byte order</param>
		''' <param name="data">Data to be wrote</param>
		''' <param name="disable_block_write"></param>
		''' <param name="rsp"></param>
		''' <returns></returns>
		Public Function TagWrite(mb As OPERATION_CMD.MEMORY_BANK, addr As UInt16, data As UInt16(), disable_block_write As Boolean, ByRef rsp As OPERATION_NTF.TAG_WRITE_NTF) As CMD_RETURN
			tagWriteRspEvent = New SpeedwayManualResetEvent(False)
			tagWriteNtfEvent = New SpeedwayManualResetEvent(False)
			invalidCommandNtfEvent = New SpeedwayManualResetEvent(False)
			Dim waithandles As WaitHandle() = New WaitHandle() {tagWriteRspEvent.evt, invalidCommandNtfEvent.evt}

			Dim temp As Byte() = OPERATION_CMD.GENERATE_TAG_WRITE_CMD(include_timestamp, mb, addr, data, disable_block_write)

			rsp = Nothing
			ns.Write(temp, 0, temp.Length)
			Dim wait_result As Integer = WaitHandle.WaitAny(waithandles, CMD_RESPONSE_TIME_OUT, False)
			If wait_result = 1 Then
				Return CMD_RETURN.INVALID_COMMAND
			ElseIf wait_result > 1 Then
				Return CMD_RETURN.CMD_RESPONSE_TIME_OUT
			End If

			If Not tagWriteNtfEvent.evt.WaitOne(NOTIFICTION_TIME_OUT, False) Then
				Return CMD_RETURN.CMD_RESPONSE_TIME_OUT
			End If
			Try
				rsp = New OPERATION_NTF.TAG_WRITE_NTF(tagWriteNtfEvent.data)
				Return CMD_RETURN.COMMAND_SUCESS
			Catch
				Return CMD_RETURN.RESPONSE_CAN_NOT_RECOGNIZED
			End Try

		End Function

		''' <summary>
		''' Lock data in a tag
		''' </summary>
		''' <param name="tlo"></param>
		''' <param name="rsp"></param>
		''' <returns></returns>
		Public Function TagLock(tlo As OPERATION_CMD.TAG_LOCK_OPERATION, ByRef rsp As OPERATION_NTF.TAG_ACCESS_RESULT_CODE) As CMD_RETURN
			tagLockNtfEvent = New SpeedwayManualResetEvent(False)
			tagLockRspEvent = New SpeedwayManualResetEvent(False)
			invalidCommandNtfEvent = New SpeedwayManualResetEvent(False)
			Dim waithandles As WaitHandle() = New WaitHandle() {tagLockRspEvent.evt, invalidCommandNtfEvent.evt}

			Dim data As Byte() = OPERATION_CMD.GENERATE_TAG_LOCK_CMD(include_timestamp, tlo)

			rsp = OPERATION_NTF.TAG_ACCESS_RESULT_CODE.FAIL_OTHER_TAG_ERROR
			ns.Write(data, 0, data.Length)
			Dim wait_result As Integer = WaitHandle.WaitAny(waithandles, CMD_RESPONSE_TIME_OUT, False)
			If wait_result = 1 Then
				Return CMD_RETURN.INVALID_COMMAND
			ElseIf wait_result > 1 Then
				Return CMD_RETURN.CMD_RESPONSE_TIME_OUT
			End If

			If Not tagLockNtfEvent.evt.WaitOne(NOTIFICTION_TIME_OUT, False) Then
				Return CMD_RETURN.CMD_RESPONSE_TIME_OUT
			Else
				rsp = CType(tagLockNtfEvent.data(0), OPERATION_NTF.TAG_ACCESS_RESULT_CODE)
				Return CMD_RETURN.COMMAND_SUCESS
			End If
		End Function

		''' <summary>
		''' Lock data in a tag
		''' </summary>
		''' <param name="tlo"></param>
		''' <param name="password"></param>
		''' <param name="rsp"></param>
		''' <returns></returns>
		Public Function TagLock(tlo As OPERATION_CMD.TAG_LOCK_OPERATION, password As UInt32, ByRef rsp As OPERATION_NTF.TAG_ACCESS_RESULT_CODE) As CMD_RETURN
			tagLockNtfEvent = New SpeedwayManualResetEvent(False)
			tagLockRspEvent = New SpeedwayManualResetEvent(False)
			invalidCommandNtfEvent = New SpeedwayManualResetEvent(False)
			Dim waithandles As WaitHandle() = New WaitHandle() {tagLockRspEvent.evt, invalidCommandNtfEvent.evt}

			Dim data As Byte() = OPERATION_CMD.GENERATE_TAG_LOCK_CMD(include_timestamp, tlo, password)
			rsp = OPERATION_NTF.TAG_ACCESS_RESULT_CODE.FAIL_OTHER_TAG_ERROR
			ns.Write(data, 0, data.Length)
			Dim wait_result As Integer = WaitHandle.WaitAny(waithandles, CMD_RESPONSE_TIME_OUT, False)
			If wait_result = 1 Then
				Return CMD_RETURN.INVALID_COMMAND
			ElseIf wait_result > 1 Then
				Return CMD_RETURN.CMD_RESPONSE_TIME_OUT
			End If

			If Not tagLockNtfEvent.evt.WaitOne(NOTIFICTION_TIME_OUT, False) Then
				Return CMD_RETURN.CMD_RESPONSE_TIME_OUT
			Else
				rsp = CType(tagLockNtfEvent.data(0), OPERATION_NTF.TAG_ACCESS_RESULT_CODE)
				Return CMD_RETURN.COMMAND_SUCESS
			End If
		End Function

		''' <summary>
		''' Kill a tag
		''' </summary>
		''' <param name="password"></param>
		''' <param name="rsp"></param>
		''' <returns></returns>
		Public Function TagKill(password As UInt32, ByRef rsp As OPERATION_NTF.TAG_ACCESS_RESULT_CODE) As CMD_RETURN
			tagKillNtfEvent = New SpeedwayManualResetEvent(False)
			tagKillRspEvent = New SpeedwayManualResetEvent(False)
			invalidCommandNtfEvent = New SpeedwayManualResetEvent(False)
			Dim waithandles As WaitHandle() = New WaitHandle() {tagKillRspEvent.evt, invalidCommandNtfEvent.evt}

			Dim data As Byte() = OPERATION_CMD.GENERATE_TAG_KILL_CMD(include_timestamp, password)
			rsp = OPERATION_NTF.TAG_ACCESS_RESULT_CODE.FAIL_OTHER_TAG_ERROR

			ns.Write(data, 0, data.Length)
			Dim wait_result As Integer = WaitHandle.WaitAny(waithandles, CMD_RESPONSE_TIME_OUT, False)
			If wait_result = 1 Then
				Return CMD_RETURN.INVALID_COMMAND
			ElseIf wait_result > 1 Then
				Return CMD_RETURN.CMD_RESPONSE_TIME_OUT
			End If

			If Not tagKillNtfEvent.evt.WaitOne(NOTIFICTION_TIME_OUT, False) Then
				Return CMD_RETURN.CMD_RESPONSE_TIME_OUT
			Else
				rsp = CType(tagKillNtfEvent.data(0), OPERATION_NTF.TAG_ACCESS_RESULT_CODE)
				Return CMD_RETURN.COMMAND_SUCESS
			End If
		End Function

		''' <summary>
		''' Set profile sequence list
		''' </summary>
		''' <param name="enabled">Enable sequence, default should be false</param>
		''' <param name="sequences">sequence list, the maximum sequence number is 16. the length of the sequence should be equal to the length of the duration list</param>
		''' <param name="durations">duration list</param>
		''' <returns></returns>
		Public Function SetProfileSequence(enabled As Boolean, sequences As ArrayList, durations As ArrayList, ByRef err_occur As Boolean) As CMD_RETURN
			err_occur = True

			setProfileSequenceRspEvent = New SpeedwayManualResetEvent(False)
			invalidCommandNtfEvent = New SpeedwayManualResetEvent(False)
			Dim waithandles As WaitHandle() = New WaitHandle() {setProfileSequenceRspEvent.evt, invalidCommandNtfEvent.evt}

			Dim data As Byte() = OPERATION_CMD.GENERATE_SET_PROFILE_SEQUENCE_CMD(False, enabled, sequences, durations)

			ns.Write(data, 0, data.Length)
			Dim wait_result As Integer = WaitHandle.WaitAny(waithandles, CMD_RESPONSE_TIME_OUT, False)
			If wait_result = 1 Then
				Return CMD_RETURN.INVALID_COMMAND
			ElseIf wait_result > 1 Then
				Return CMD_RETURN.CMD_RESPONSE_TIME_OUT
			Else
				err_occur = If(setProfileSequenceRspEvent.data(0) = 1, True, False)
				Return CMD_RETURN.COMMAND_SUCESS
			End If
		End Function

		#End Region


		#Region "Impinj"

		Public Function SetFixedChannel(channel As Short) As CMD_RETURN
			Dim data As Short() = New Short() {0}

			testWriteRspEvent = New SpeedwayManualResetEvent(False)
			invalidCommandNtfEvent = New SpeedwayManualResetEvent(False)
			testWriteNtfEvent = New SpeedwayManualResetEvent(False)
			Dim waithandles As WaitHandle() = New WaitHandle() {testWriteRspEvent.evt, invalidCommandNtfEvent.evt, testWriteNtfEvent.evt}

			Dim ns_data As Byte() = TEST_CMD_SET.GENERATE_WRITE_CMD_DATA(2, &H2001, data, include_timestamp)
			ns.Write(ns_data, 0, ns_data.Length)

			Dim wait_result As Integer = WaitHandle.WaitAny(waithandles, CMD_RESPONSE_TIME_OUT, False)
			If wait_result = 1 Then
				Return CMD_RETURN.INVALID_COMMAND
			ElseIf wait_result > 1 Then
				Return CMD_RETURN.CMD_RESPONSE_TIME_OUT
			End If
			Try
			Catch
				Return CMD_RETURN.RESPONSE_CAN_NOT_RECOGNIZED
			End Try


			If Not testWriteNtfEvent.evt.WaitOne(NOTIFICTION_TIME_OUT, False) Then
				Return CMD_RETURN.CMD_RESPONSE_TIME_OUT
			End If


			testWriteRspEvent.evt.Reset()
			invalidCommandNtfEvent.evt.Reset()
			testWriteNtfEvent.evt.Reset()


			data = New Short() {channel}

			ns_data = TEST_CMD_SET.GENERATE_WRITE_CMD_DATA(2, &H2003, data, include_timestamp)
			ns.Write(ns_data, 0, ns_data.Length)

			wait_result = WaitHandle.WaitAny(waithandles, CMD_RESPONSE_TIME_OUT, False)
			If wait_result = 1 Then
				Return CMD_RETURN.INVALID_COMMAND
			ElseIf wait_result > 1 Then
				Return CMD_RETURN.CMD_RESPONSE_TIME_OUT
			End If
			Try
			Catch
				Return CMD_RETURN.RESPONSE_CAN_NOT_RECOGNIZED
			End Try


			If Not testWriteNtfEvent.evt.WaitOne(NOTIFICTION_TIME_OUT, False) Then
				Return CMD_RETURN.CMD_RESPONSE_TIME_OUT
			End If


			Return CMD_RETURN.COMMAND_SUCESS
		End Function
		#End Region

	End Class
End Namespace
