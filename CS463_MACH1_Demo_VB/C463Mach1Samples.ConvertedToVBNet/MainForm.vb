'*
' ******************************************************************************
' **
' ** @file Mainform.cs
' **
' ** 
' **
' ** 
' **
' *****************************************************************************


'History

Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Data
Imports System.Drawing
Imports System.Text
Imports System.Windows.Forms
Imports System.Threading
Imports System.Collections
Imports System.Text.RegularExpressions

Imports System.IO
Imports CSLMach1.CSL
'include in CSL namespace
Imports CSLMach1.CSL.Mach1
'include in CSL.Mach1 namespace
Public Partial Class MainForm
	Inherits Form
	#Region "Members"

	'Declare a reader
	Private reader As CS461Reader

	'Declare a reader settins
	Private settings As CSLReaderSettings
	Private readerSettingsPath As String = Application.StartupPath + "\reader.xml"

	Private tags As ArrayList
	Private act As TAG_ACTION

	Private writeEpcData As UShort()
	Private writeUm1Data As UShort()
	Private writeUm2Data As UShort()

	Private readMB0 As Integer = 0
	Private readMB2 As Integer = 0
	Private readMB3 As Integer = 0

	Private m_totaltag As Integer = 0
	Private m_tagRate As Integer = 0

	#End Region

	#Region "Definition"

	Friend Enum NTF_TYPE
		INVENTORY = 0
		OTHER
	End Enum

	Friend Enum TAG_ACTION
		INVENTORY = 0
		READ
		WRITE
		WRITE_USER_MEMORY
		'@derek
	End Enum

	#End Region


	Public Sub New()
		InitializeComponent()

		'@derek
		cbEpcLen.BeginUpdate()
		For i As Integer = 1 To 15
			cbEpcLen.Items.Add([String].Format("{0} bits", i * 16))
		Next
		cbEpcLen.EndUpdate()
		'cbEpcLen.SelectedIndex = cbEpcLen.Items.Count - 1;
		cbEpcLen.SelectedIndex = 5

		cbUmLen.BeginUpdate()
		For i As Integer = 1 To 16
			cbUmLen.Items.Add([String].Format("{0} bits", i * 16))
		Next
		cbUmLen.EndUpdate()
		cbUmLen.SelectedIndex = cbUmLen.Items.Count - 1

		cbMB.SelectedIndex = 2

		cbMBLen.BeginUpdate()
		For i As Integer = 1 To 29
			cbMBLen.Items.Add([String].Format("{0} bits", i * 16))
		Next
		cbMBLen.EndUpdate()
		cbMBLen.SelectedIndex = 1

		cbMB0Len.BeginUpdate()
		cbMB2Len.BeginUpdate()
		cbMB3Len.BeginUpdate()
		cbMB0Len.Items.Add("NONE")
		cbMB2Len.Items.Add("NONE")
		cbMB3Len.Items.Add("NONE")
		For i As Integer = 1 To 32
			cbMB0Len.Items.Add([String].Format("{0} bits", i * 16))
			cbMB2Len.Items.Add([String].Format("{0} bits", i * 16))
			cbMB3Len.Items.Add([String].Format("{0} bits", i * 16))
		Next
		cbMB0Len.EndUpdate()
		cbMB2Len.EndUpdate()
		cbMB3Len.EndUpdate()
		cbMB0Len.SelectedIndex = 0
		cbMB2Len.SelectedIndex = 0
		cbMB3Len.SelectedIndex = 0
	End Sub

	Private Sub btnConnect_Click(sender As Object, e As EventArgs)
		Me.Cursor = Cursors.WaitCursor
		If btnConnect.Text = "Connect" Then
			Dim rp_frm As New ReadParamForm()
			rp_frm.mode = CSLReaderParamControl.MODE.[NEW]
			'Notify the reader parameters form to reader initialization status 
			'Use previous settings if exist
			If File.Exists(readerSettingsPath) = True Then
				rp_frm.settings = loadReaderSettings()
			End If

			If rp_frm.ShowDialog() = DialogResult.OK Then
				settings = rp_frm.settings

				toolStripStatusLabel1.Text = String.Format("Trying to connect to {0}", settings.reader_information.reader_name)

				If reader Is Nothing Then
					reader = New CS461Reader()
				End If
				'make sure that the reader is instantialized
				'Initialize reader by applying reader name and region
				If reader.Initialize(settings.reader_information.reader_name, CType(settings.reader_information.region, REGULATORY_REGION)) Then
					'Apply all reader run time parameters. You can apply the parameters individually by using individual API
					'For more information, please check the ApplyReaderSetting code
					If Not reader.ApplyReaderSetting(settings) Then
						MessageBox.Show("Failed to apply reader parameters!")
						If reader.Connected Then
							reader.Disconnect()
						End If
						toolStripStatusLabel1.Text = String.Format("Failted to connect to {0}", settings.reader_information.reader_name)

						Me.Cursor = Cursors.[Default]
						Return
					End If
				Else
					MessageBox.Show("Can not connect to the reader. Please check if the connection or make sure that no other device connected to it.")
					If reader.Connected Then
						reader.Disconnect()
					End If
					toolStripStatusLabel1.Text = String.Format("Failted to connect to {0}", settings.reader_information.reader_name)

					Me.Cursor = Cursors.[Default]
					Return
				End If

				btnConnect.Text = "Disconnect"
				toolStripStatusLabel1.Text = String.Format("Now connected to {0}", settings.reader_information.reader_name)
				saveReaderSettings(settings)
			End If
		Else
			If reader IsNot Nothing Then
				If reader.Disconnect() Then
					btnConnect.Text = "Connect"
					toolStripStatusLabel1.Text = String.Format("{0} is now disconnected!", settings.reader_information.reader_name)
					btnTest.Enabled = True
					btnStop.Enabled = False
				End If
			End If
		End If

		Me.Cursor = Cursors.[Default]
	End Sub

	Private Sub MainForm_Load(sender As Object, e As EventArgs)
		'Create an instance of reader
		reader = New CS461Reader()

		'Demonstration of Add event subscription
		AddHandler reader.onInventoryNtfReceived, New delegateInventoryNtf(AddressOf reader_onInventoryNtfReceived)
		AddHandler reader.onStatusReportNtfReceived, New delegateStatusReportNtf(AddressOf reader_onStatusReportNtfReceived)
		AddHandler reader.onSystemErrorNtfReceived, New delegateSystemErrorNtf(AddressOf reader_onSystemErrorNtfReceived)

		'Create an instance of reader settings. Default parameters are applied
		settings = New CSLReaderSettings()
	End Sub

	Private Sub btnUpdate_Click(sender As Object, e As EventArgs)
		If reader Is Nothing OrElse Not reader.Connected Then
			Return
		End If

		Dim rp_frm As New ReadParamForm()
		rp_frm.mode = CSLReaderParamControl.MODE.UPDATE
		'Notify the reader parameters form to reader initialization status 
		Dim s_settings As CSLReaderSettings
		reader.GetReaderSettings(s_settings)

		If s_settings IsNot Nothing Then
			settings = s_settings
		End If
		rp_frm.settings = settings

		If rp_frm.ShowDialog() = DialogResult.OK Then
			settings = rp_frm.settings

			toolStripStatusLabel1.Text = String.Format("Updating {0}", settings.reader_information.reader_name)

			If Not reader.ApplyReaderSetting(settings) Then
				MessageBox.Show("Failed to apply reader parameters!")
			Else
				toolStripStatusLabel1.Text = String.Format("Parameters of {0} are updated!", settings.reader_information.reader_name)
			End If

			Me.Cursor = Cursors.[Default]
		End If
	End Sub

	Private Sub reader_onSystemErrorNtfReceived(sen As MANAGEMENT_NTF.SYSTEM_ERROR_NTF)
		Dim msg As String = String.Format("System Error! Reason:{0}" & vbCr & vbLf, sen.err_reason.ToString())
		UpdateControls(msg, NTF_TYPE.OTHER)
	End Sub

	Private Sub reader_onStatusReportNtfReceived(srn As MANAGEMENT_NTF.STATUS_REPORT_NTF)
		Dim msg As String = String.Format("Status Report! Reason:{0}" & vbCr & vbLf, srn.modem_overflow_ntf_loss.ToString())
		UpdateControls(msg, NTF_TYPE.OTHER)
	End Sub

	Private Sub reader_onInventoryNtfReceived(inv As OPERATION_NTF.INVENTORY_NTF)
		Dim cmd_return__1 As CMD_RETURN
		Dim err As Boolean

		'To eliminate duplication tags.
		'if (tags.Contains(inv.EPC))
'            {
'                //if the action is inventory, return without executing InventoryContinue 
'                if (act == TAG_ACTION.INVENTORY) return;
'                //Executing InventoryContinue
'                else
'                {
'                    cmd_return = reader.InventoryContinue(out err);
'                    return;
'                }
'            }
'
'            tags.Add(inv.EPC);


		Dim msg As String
		Select Case act
			Case TAG_ACTION.INVENTORY
				Dim mb As String
				Try
					mb = ConvertData(inv.gen2ReadData)
				Catch
					mb = ""
				End Try
				UpdateListView(inv.EPC, CSng(inv.rssi), mb)
				m_totaltag += 1
				Exit Select
			Case TAG_ACTION.READ
				If True Then
					If inv.halted = False Then
						Return
					End If

					Dim r_ntf As OPERATION_NTF.TAG_READ_NTF

					'EPC
					msg = String.Format(vbCr & vbLf & "Tag Read ==========" & vbCr & vbLf & "EPC: {0}" & vbCr & vbLf & "RSSI: {1}" & vbCr & vbLf & "PC: {2:X2}{3:X2}" & vbCr & vbLf, inv.EPC, inv.rssi, inv.gen2PC(0), inv.gen2PC(1))
					UpdateControls(msg, NTF_TYPE.OTHER)

					'Read MB0 from tag
					If readMB0 > 0 Then
						'read from first word
						'read two words
						cmd_return__1 = reader.TagRead(OPERATION_CMD.MEMORY_BANK.RESERVED, 0, CByte(readMB0), r_ntf)
						If cmd_return__1 = CMD_RETURN.COMMAND_SUCESS Then
							msg = String.Format("MB0: {0} " & vbCr & vbLf, ConvertData(r_ntf.data))
						Else
                            msg = String.Format("Error: {0} ({1})" & vbCr & vbLf, cmd_return__1.ToString(), r_ntf._result_code)
                        End If
						UpdateControls(msg, NTF_TYPE.OTHER)
					End If

					'Read MB2 from tag
					If readMB2 > 0 Then
						'read from first word
						'read two words
						cmd_return__1 = reader.TagRead(OPERATION_CMD.MEMORY_BANK.TID, 0, CByte(readMB2), r_ntf)
						If cmd_return__1 = CMD_RETURN.COMMAND_SUCESS Then
							msg = String.Format("MB2: {0} " & vbCr & vbLf, ConvertData(r_ntf.data))
						Else
                            msg = String.Format("Error: {0} ({1})" & vbCr & vbLf, cmd_return__1.ToString(), r_ntf._result_code)
                        End If
						UpdateControls(msg, NTF_TYPE.OTHER)
					End If

					'Reader MB3 from tag   //@derek
					Dim firstSection As Boolean = True
					Dim MB3 As Integer = readMB3
					While MB3 > 0
						If MB3 > 16 Then
							'read from first word
							'read two words
							cmd_return__1 = reader.TagRead(OPERATION_CMD.MEMORY_BANK.USER, 0, 16, r_ntf)
							MB3 -= 16
						Else
							'read from first word
							'read two words
							cmd_return__1 = reader.TagRead(OPERATION_CMD.MEMORY_BANK.USER, 0, CByte(MB3), r_ntf)
							MB3 = 0
						End If

						If cmd_return__1 = CMD_RETURN.COMMAND_SUCESS Then
							If firstSection = True Then
								msg = String.Format("MB3: {0} " & vbCr & vbLf, ConvertData(r_ntf.data))
								firstSection = False
							Else
								msg = String.Format("{0} " & vbCr & vbLf, ConvertData(r_ntf.data))
							End If
						Else
                            msg = String.Format("Error: {0} ({1})" & vbCr & vbLf, cmd_return__1.ToString(), r_ntf._result_code)
                        End If
						UpdateControls(msg, NTF_TYPE.OTHER)
					End While

					reader.InventoryContinue(err)
				End If
				Exit Select
			Case TAG_ACTION.WRITE
				If True Then
					If inv.halted = False Then
						Return
					End If

					Dim w_ntf As OPERATION_NTF.TAG_WRITE_NTF
					'ushort[] data = new ushort[] { 0x3200, 0x1111, 0x2222, 0x3333, 0x4444, 0x5555 };
					'ushort[] data = new ushort[] { 0x31C1, 0x1111, 0x2222, 0x3333, 0x4444, 0x5555, 0x6666, 0x7777, 0x8888, 0x9999, 0xaaaa, 0xbbbb, 0xcccc, 0xdddd, 0xeeee, 0xffff };

					'Write prepared data to tag
					'cmd_return = reader.TagWrite(OPERATION_CMD.MEMORY_BANK.EPC, 1/*EPC start from 3rd word*/, data, true/*don't use block write*/, out w_ntf);
					If chkPC.Checked Then
						'EPC start from 3rd word
						cmd_return__1 = reader.TagWrite(OPERATION_CMD.MEMORY_BANK.EPC, 1, writeEpcData, True, 0, w_ntf)
                        msg = String.Format("Tag Write EPC ========== " & vbCr & vbLf & "Status: {0} " & vbCr & vbLf & "Result: {1}" & vbCr & vbLf & "Error Addr: {2}" & vbCr & vbLf, cmd_return__1.ToString(), w_ntf._result_code, w_ntf.err_addr)
                        UpdateControls(msg, NTF_TYPE.OTHER)
					ElseIf chkEPC.Checked Then
						'EPC start from 3rd word
						cmd_return__1 = reader.TagWrite(OPERATION_CMD.MEMORY_BANK.EPC, 2, writeEpcData, True, 0, w_ntf)
                        msg = String.Format("Tag Write EPC ========== " & vbCr & vbLf & "Status: {0} " & vbCr & vbLf & "Result: {1}" & vbCr & vbLf & "Error Addr: {2}" & vbCr & vbLf, cmd_return__1.ToString(), w_ntf._result_code, w_ntf.err_addr)
                        UpdateControls(msg, NTF_TYPE.OTHER)
					End If

					If chkUM.Checked Then
						'EPC start from 3rd word
						cmd_return__1 = reader.TagWrite(OPERATION_CMD.MEMORY_BANK.USER, 0, writeUm1Data, True, 0, w_ntf)
                        msg = String.Format("Tag Write User memory ========== " & vbCr & vbLf & "EPC: {0}" & vbCr & vbLf & "Status: {1} " & vbCr & vbLf & "Result: {2}" & vbCr & vbLf & "Error Addr: {3}" & vbCr & vbLf, inv.EPC, cmd_return__1.ToString(), w_ntf._result_code, w_ntf.err_addr)
                        UpdateControls(msg, NTF_TYPE.OTHER)
					End If

					reader.InventoryContinue(err)
				End If
				Exit Select
		End Select
	End Sub

	Private Sub btnTest_Click(sender As Object, e As EventArgs)
		readMB0 = cbMB0Len.SelectedIndex
		readMB2 = cbMB2Len.SelectedIndex
		readMB3 = cbMB3Len.SelectedIndex

		If reader.Connected = False Then
			MessageBox.Show("Please connect to reader first", "Reader not connected", MessageBoxButtons.OK)
			Return
		End If

		btnUpdate.Enabled = False
		btnClearStatus.Enabled = False
		btnClearTag.Enabled = False

		Select Case comboBox1.Text
			Case "Inventory"
				lv_tag.Items.Clear()
				act = TAG_ACTION.INVENTORY
				StartInventory()
				Exit Select
			Case "SetGPO"
				SetGPO()
				Exit Select
			Case "GetGPI"
				GetGPI()
				Exit Select
			Case "TagRead"
				TagCommissioningInventory()
				act = TAG_ACTION.READ
				Exit Select
			Case "TagWrite"
				'
				If (chkPC.Checked = False) AndAlso (chkEPC.Checked = False) AndAlso (chkUM.Checked = False) Then
					MessageBox.Show("Nothing to write!!")
					Return
				End If

				If chkPC.Checked = True Then
					If txtPC.BackColor = Color.Red Then
						MessageBox.Show("Invalid PC")
						txtPC.Focus()
						Return
					End If
				End If

				If chkEPC.Checked Then
					If txtEPC.BackColor = Color.Red Then
						MessageBox.Show("Invalid EPC")
						txtEPC.Focus()
						Return
					End If
				End If


				If chkUM.Checked Then
					If txtUM1.BackColor = Color.Red Then
						MessageBox.Show("Invalid User memory")
						txtUM1.Focus()
						Return
					End If
					If txtUM2.BackColor = Color.Red Then
						MessageBox.Show("Invalid User memory")
						txtUM2.Focus()
						Return
					End If
				End If

				Try
					Dim sb As New StringBuilder()
					If chkPC.Checked Then
						sb.Append(txtPC.Text)
					End If
					If chkEPC.Checked Then
						sb.Append(txtEPC.Text)
					End If
					Dim sEpc As String = sb.ToString()
					writeEpcData = New UShort(sEpc.Length \ 4 - 1) {}

					For i As Integer = 0 To writeEpcData.Length - 1
						writeEpcData(i) = UShort.Parse(sEpc.Substring(i * 4, 4), System.Globalization.NumberStyles.HexNumber)
					Next
				Catch
					MessageBox.Show("Convert PC / EPC error")
					Return
				End Try

				Try
					If chkUM.Checked Then
						Dim sUm1 As String = txtUM1.Text
						writeUm1Data = New UShort(sUm1.Length \ 4 - 1) {}
						For i As Integer = 0 To writeUm1Data.Length - 1
							writeUm1Data(i) = UShort.Parse(sUm1.Substring(i * 4, 4), System.Globalization.NumberStyles.HexNumber)
						Next

						Dim sUm2 As String = txtUM2.Text
						writeUm2Data = New UShort(sUm2.Length \ 4 - 1) {}
						For i As Integer = 0 To writeUm2Data.Length - 1
							writeUm2Data(i) = UShort.Parse(sUm2.Substring(i * 4, 4), System.Globalization.NumberStyles.HexNumber)
						Next
					End If
				Catch
					MessageBox.Show("Convert User memory error")
					Return
				End Try



				TagCommissioningInventory()
				act = TAG_ACTION.WRITE
				Exit Select
		End Select
	End Sub

    Private Sub StartInventory()
        Dim cmd_return__1 As CMD_RETURN
        Dim err As Boolean

        m_totaltag = 0

        tags = New ArrayList()

        Dim param As New OPERATION_CMD.INVENTORY_PRAMA()

        'Inventory using defualt setting : No inventory filter, No halting filter
        param.enable_halt_filter = False
        param.enable_inventory_filter = False

        'Enable inventory filter
        'Select tags starting with "00001111"
        'param.enable_inventory_filter = true;
        'param.inventory_filter_condition.filter_operation = OPERATION_CMD.INVENTORY_PRAMA.INVENTORY_FILTER_OPERATION.A;
        'param.inventory_filter_condition.a_bit_offset = 0x20;
        'param.inventory_filter_condition.a_compare = OPERATION_CMD.INVENTORY_PRAMA.LOGIC.EQUALS;
        'param.inventory_filter_condition.a_filter_memory_bank = OPERATION_CMD.MEMORY_BANK.EPC;
        'param.inventory_filter_condition.a_length = 32;
        'param.inventory_filter_condition.a_pattern = "00000000000000000001000100010001";

        'Enable halt filter

        'param.enable_halt_filter = true;

        'Halt on every tag
        'param.inventory_halt_condition.halt_operation = OPERATION_CMD.INVENTORY_PRAMA.INVENTORY_HALT_OPERATION.HALT_EVERY_TAG;

        'Use A halt filter
        'param.inventory_halt_condition.halt_operation = OPERATION_CMD.INVENTORY_PRAMA.INVENTORY_HALT_OPERATION.A;       
        'param.inventory_halt_condition.halt_a_bit_offset = 0x70;
        'param.inventory_halt_condition.halt_a_compare = OPERATION_CMD.INVENTORY_PRAMA.LOGIC.EQUALS;
        'param.inventory_halt_condition.halt_a_length = 16;
        'param.inventory_halt_condition.halt_a_memory_bank = OPERATION_CMD.MEMORY_BANK.EPC;
        'param.inventory_halt_condition.halt_a_value = pattern;
        'param.inventory_halt_condition.halt_a_mask = "1111222233334444";

        'Read Other memory bank
        If (chkMB.Checked = True) AndAlso (cbMB.SelectedIndex <> 1) Then
            param.read_memory_bank = CType(cbMB.SelectedIndex, OPERATION_CMD.MEMORY_BANK)
            param.read_word_memory_address = 0
            'start at word 0
            '2 words -> 32bits
            param.read_length = CByte(cbMBLen.SelectedIndex + 1)
        End If

        param.estimated_tag_population = CShort(settings.gen2_params.population)

        'Start inventory
        cmd_return__1 = reader.Inventory(param, err)
        If cmd_return__1 = CMD_RETURN.COMMAND_SUCESS AndAlso Not err Then
            btnTest.Enabled = False
            btnStop.Enabled = True
            toolStripStatusLabel1.Text = "Inventory started!"
        Else
            toolStripStatusLabel1.Text = "Failed to start inventory!"
        End If
    End Sub

    Private Sub SetGPO()
		Dim cmd_return As CMD_RETURN

		'Set one GPO. You can set multiple GPOs too.
		Dim gpo_config As MANAGEMENT_CMD.GPO_CONFIG() = New MANAGEMENT_CMD.GPO_CONFIG(0) {}
        Dim sg_rsp As MANAGEMENT_CMD.SET_GPO_RSP

        gpo_config(0) = New MANAGEMENT_CMD.GPO_CONFIG()

		'Enable GPO0
		gpo_config(0).id = MANAGEMENT_CMD.GPO_CONFIG.GPO_ID.GPO0
        'Set Output high
        gpo_config(0).m_config = MANAGEMENT_CMD.GPO_CONFIG.CONFIG.HIGH

        cmd_return = reader.SetGPO(gpo_config, sg_rsp)

        tbStatus.Text += String.Format("SetGPO command : {0}, result: {1}" & vbCr & vbLf, cmd_return.ToString(), sg_rsp.result.ToString)

    End Sub

	Private Sub SetGPI()
		Dim cmd_return As CMD_RETURN

		Dim gpi_config As MANAGEMENT_CMD.GPI_CONFIG() = New MANAGEMENT_CMD.GPI_CONFIG(1) {}

		gpi_config(0) = New MANAGEMENT_CMD.GPI_CONFIG()
		gpi_config(1) = New MANAGEMENT_CMD.GPI_CONFIG()

		'Set GPI0 to accept low-to-high trigger
		gpi_config(0).id = MANAGEMENT_CMD.GPI_CONFIG.GPI_ID.GPI0
        gpi_config(0).m_config = MANAGEMENT_CMD.GPI_CONFIG.CONFIG.LO_TO_HIGH
        'Set GPI1 to accept high-to-low trigger
        gpi_config(1).id = MANAGEMENT_CMD.GPI_CONFIG.GPI_ID.GPI1
        gpi_config(1).m_config = MANAGEMENT_CMD.GPI_CONFIG.CONFIG.HI_TO_LOW

        Dim sgi_rsp As MANAGEMENT_CMD.SET_GPI_RSP
		cmd_return = reader.SetGPI(gpi_config, sgi_rsp)

        tbStatus.Text += String.Format("SetGPI command : {0}, result: {1}" & vbCr & vbLf, cmd_return.ToString(), sgi_rsp.result.ToString)
    End Sub

    Private Sub GetGPI()
		Dim gpi_rsp As MANAGEMENT_CMD.GET_GPI_RSP
		Dim cmd_return As CMD_RETURN = reader.GetGPI(gpi_rsp)

        tbStatus.Text += String.Format("GetGPI command : {0}, result: GPI-{1} is {2}" & vbCr & vbLf, cmd_return.ToString(), gpi_rsp.gpi_status(0).id, gpi_rsp.gpi_status(0).m_status)
        tbStatus.Text += String.Format("GetGPI command : {0}, result: GPI-{1} is {2}" & vbCr & vbLf, cmd_return.ToString(), gpi_rsp.gpi_status(1).id, gpi_rsp.gpi_status(1).m_status)
        tbStatus.Text += String.Format("GetGPI command : {0}, result: GPI-{1} is {2}" & vbCr & vbLf, cmd_return.ToString(), gpi_rsp.gpi_status(2).id, gpi_rsp.gpi_status(2).m_status)
        tbStatus.Text += String.Format("GetGPI command : {0}, result: GPI-{1} is {2}" & vbCr & vbLf, cmd_return.ToString(), gpi_rsp.gpi_status(3).id, gpi_rsp.gpi_status(3).m_status)
    End Sub

	Private Sub TagCommissioningInventory()
		Dim cmd_return__1 As CMD_RETURN
		Dim err As Boolean

		tags = New ArrayList()

		Dim param As New OPERATION_CMD.INVENTORY_PRAMA()

		'Enable halt filter
		param.enable_halt_filter = True

        'Halt on every tag
        param._inventory_halt_condition.halt_operation = OPERATION_CMD.INVENTORY_PRAMA.INVENTORY_HALT_OPERATION.HALT_EVERY_TAG

        'Halt tags starting with "00001111"
        'param.inventory_halt_condition.halt_operation = OPERATION_CMD.INVENTORY_PRAMA.INVENTORY_HALT_OPERATION.A;
        'param.inventory_halt_condition.halt_a_bit_offset = 0x20;
        'param.inventory_halt_condition.halt_a_compare = OPERATION_CMD.INVENTORY_PRAMA.LOGIC.EQUALS;
        'param.inventory_halt_condition.halt_a_length = 32;
        'param.inventory_halt_condition.halt_a_memory_bank = OPERATION_CMD.MEMORY_BANK.EPC;
        'param.inventory_halt_condition.halt_a_value = "00000000000000000001000100010001";
        'param.inventory_halt_condition.halt_a_mask = "11111111111111111111111111111111";


        'Start inventory
        cmd_return__1 = reader.Inventory(param, err)

		If cmd_return__1 = CMD_RETURN.COMMAND_SUCESS AndAlso Not err Then
			btnTest.Enabled = False
			btnStop.Enabled = True
			toolStripStatusLabel1.Text = "Tag commissioning inventory started!"
		Else
			toolStripStatusLabel1.Text = "Failed to start tag commissioning inventory!"
		End If
	End Sub

	Private Sub btnStop_Click(sender As Object, e As EventArgs)
		Dim cmd_return__1 As CMD_RETURN
		Try
			cmd_return__1 = reader.ModemStop()
			If cmd_return__1 = CMD_RETURN.COMMAND_SUCESS Then
				btnTest.Enabled = True
				btnStop.Enabled = False
				btnUpdate.Enabled = True
				btnClearStatus.Enabled = True
				btnClearTag.Enabled = True
				toolStripStatusLabel1.Text = "Inventory stopped!"
			Else
				toolStripStatusLabel1.Text = "Failed to stop inventory!"
			End If
		Catch
			btnUpdate.Enabled = True
			btnTest.Enabled = True
			btnStop.Enabled = False
			btnClearStatus.Enabled = True
			btnClearTag.Enabled = True
		End Try
	End Sub

	Private Function ConvertData(data As Byte()) As String

		Dim s As String = String.Empty

		If data IsNot Nothing Then
			For i As Integer = 0 To data.Length - 1
				s += String.Format("{0:x2}", data(i))
			Next
		End If
		Return s
	End Function

	Private Sub txtPC_TextChanged(sender As Object, e As EventArgs)
		Dim r As New Regex([String].Format("^(\d|[a-f]|[A-F]){{{0}}}$", 4))

		txtPC.BackColor = If(r.IsMatch(txtPC.Text), Color.White, Color.Red)
	End Sub

	Private Sub txtEPC_TextChanged(sender As Object, e As EventArgs)
		validateEpcText()
	End Sub

	Private Sub validateEpcText()
		Dim r As New Regex([String].Format("^(\d|[a-f]|[A-F]){{{0}}}$", txtEPC.MaxLength))

		txtEPC.BackColor = If(r.IsMatch(txtEPC.Text), Color.White, Color.Red)
	End Sub

	Private Sub txtUM1_TextChanged(sender As Object, e As EventArgs)
		validateUmText()
	End Sub

	Private Sub txtUM2_TextChanged(sender As Object, e As EventArgs)
		validateUmText()
	End Sub

	Private Sub validateUmText()
		Dim r As New Regex([String].Format("^(\d|[a-f]|[A-F]){{{0}}}$", txtUM1.MaxLength))

		txtUM1.BackColor = If(r.IsMatch(txtUM1.Text), Color.White, Color.Red)

		r = New Regex([String].Format("^(\d|[a-f]|[A-F]){{{0}}}$", txtUM2.MaxLength))

		txtUM2.BackColor = If(r.IsMatch(txtUM2.Text), Color.White, Color.Red)
	End Sub

	Private Sub chkUM_CheckedChanged(sender As Object, e As EventArgs)
		If chkUM.Checked Then
			chkPC.Checked = False
			chkEPC.Checked = False
		End If
	End Sub

	Private Sub chkPC_CheckedChanged(sender As Object, e As EventArgs)
		If chkPC.Checked Then
			chkUM.Checked = False
		End If
	End Sub

	Private Sub chkEPC_CheckedChanged(sender As Object, e As EventArgs)
		If chkEPC.Checked Then
			chkUM.Checked = False
		End If
	End Sub

	Private Sub cbEpcLen_SelectedIndexChanged(sender As Object, e As EventArgs)
		txtEPC.MaxLength = (cbEpcLen.SelectedIndex + 1) * 4
		If txtEPC.Text.Length > txtEPC.MaxLength Then
			txtEPC.Text = txtEPC.Text.Substring(0, txtEPC.MaxLength)
		End If
		validateEpcText()
	End Sub

	Private Sub cbUmLen_SelectedIndexChanged(sender As Object, e As EventArgs)
		If cbUmLen.SelectedIndex > 15 Then
			txtUM1.MaxLength = 16 * 4
			txtUM2.MaxLength = (cbUmLen.SelectedIndex - 15) * 4
			txtUM2.Visible = True

			If txtUM1.Text.Length > txtUM1.MaxLength Then
				txtUM1.Text = txtUM1.Text.Substring(0, txtUM1.MaxLength)
			End If

			If txtUM2.Text.Length > txtUM2.MaxLength Then
				txtUM2.Text = txtUM2.Text.Substring(0, txtUM2.MaxLength)
				

			End If
		Else
			txtUM1.MaxLength = (cbUmLen.SelectedIndex + 1) * 4
			If txtUM1.Text.Length > txtUM1.MaxLength Then
				txtUM1.Text = txtUM1.Text.Substring(0, txtUM1.MaxLength)
			End If
			txtUM2.MaxLength = 0
			txtUM2.Text = ""
			txtUM2.Visible = False
		End If
		validateUmText()
	End Sub

	Private Sub saveReaderSettings(settings As CSLReaderSettings)
		Dim sw As New StreamWriter(readerSettingsPath, False)
		sw.Write(settings.ToString())
		sw.Close()
	End Sub

	Private Function loadReaderSettings() As CSLReaderSettings
		Try
			Dim sr As New StreamReader(readerSettingsPath)
			Dim s As String = sr.ReadToEnd()
			sr.Close()
			Dim rs As New CSLReaderSettings(s)
			Return rs
		Catch
			Return Nothing
		End Try
	End Function

	Private Sub chkMB_CheckedChanged(sender As Object, e As EventArgs)
		cbMB.Enabled = chkMB.Checked
		cbMBLen.Enabled = chkMB.Checked
	End Sub

	Private Sub btnClearTag_Click(sender As Object, e As EventArgs)
		lv_tag.Items.Clear()
	End Sub

	Private Sub btnClearStatus_Click(sender As Object, e As EventArgs)
		tbStatus.Clear()
	End Sub

	Private Delegate Sub UpdateListViewDeleg(EPC As [String], rssi As Single, data As [String])
	Private Sub UpdateListView(EPC As [String], rssi As Single, data As [String])
		If Me.InvokeRequired Then
			Invoke(New UpdateListViewDeleg(AddressOf UpdateListView), New Object() {EPC, rssi, data})
			Return
		End If

		Dim foundIndex As Integer = -1
		Dim count As Integer = lv_tag.Items.Count
		If count > 0 Then
			Dim item As ListViewItem = lv_tag.FindItemWithText(EPC, True, 0)
			If item IsNot Nothing Then
				foundIndex = lv_tag.Items.IndexOf(item)
			End If
		End If
		If foundIndex >= 0 Then
			Dim listViewItem As ListViewItem = lv_tag.Items(foundIndex)
			listViewItem.SubItems(1).Text = EPC
			listViewItem.SubItems(2).Text = rssi.ToString()
			listViewItem.SubItems(3).Text = (Convert.ToInt32(listViewItem.SubItems(3).Text) + 1).ToString()
			listViewItem.SubItems(4).Text = data
		Else
			Dim row As String() = {lv_tag.Items.Count.ToString(), EPC, rssi.ToString(), "1", data}
			Dim listViewItem As New ListViewItem(row)
			lv_tag.Items.Add(listViewItem)
		End If
	End Sub

	Private Delegate Sub UpdateControlsDeleg(msg As String, type As NTF_TYPE)
	Private Sub UpdateControls(msg As String, type As NTF_TYPE)
		If Me.InvokeRequired Then
			Invoke(New UpdateControlsDeleg(AddressOf UpdateControls), New Object() {msg, type})
			Return
		End If
		Select Case type
			Case NTF_TYPE.OTHER
				tbStatus.Text += msg
				Exit Select
		End Select
	End Sub

	Private Delegate Sub UpdateRateDeleg(rate As Integer)
	Private Sub UpdateRate(rate As Integer)
		If Me.InvokeRequired Then
			Invoke(New UpdateRateDeleg(AddressOf UpdateRate), New Object() {rate})
			Return
		End If
		lb_rate.Text = rate.ToString()
	End Sub

	Private Sub readrate_Tick(sender As Object, e As EventArgs)
		m_tagRate = m_totaltag
		m_totaltag = 0
		UpdateRate(m_tagRate)
	End Sub
End Class
