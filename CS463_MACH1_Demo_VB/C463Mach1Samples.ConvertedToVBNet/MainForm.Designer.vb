Partial Class MainForm
	''' <summary>
	''' Required designer variable.
	''' </summary>
	Private components As System.ComponentModel.IContainer = Nothing

	''' <summary>
	''' Clean up any resources being used.
	''' </summary>
	''' <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
	Protected Overrides Sub Dispose(disposing As Boolean)
		If disposing AndAlso (components IsNot Nothing) Then
			components.Dispose()
		End If
		MyBase.Dispose(disposing)
	End Sub

	#Region "Windows Form Designer generated code"

	''' <summary>
	''' Required method for Designer support - do not modify
	''' the contents of this method with the code editor.
	''' </summary>
	Private Sub InitializeComponent()
		Me.components = New System.ComponentModel.Container()
		Me.statusStrip = New System.Windows.Forms.StatusStrip()
		Me.toolStripStatusLabel1 = New System.Windows.Forms.ToolStripStatusLabel()
		Me.groupBox1 = New System.Windows.Forms.GroupBox()
		Me.btnConnect = New System.Windows.Forms.Button()
		Me.btnUpdate = New System.Windows.Forms.Button()
		Me.groupBox2 = New System.Windows.Forms.GroupBox()
		Me.comboBox1 = New System.Windows.Forms.ComboBox()
		Me.btnTest = New System.Windows.Forms.Button()
		Me.btnStop = New System.Windows.Forms.Button()
		Me.groupBox3 = New System.Windows.Forms.GroupBox()
		Me.cbUmLen = New System.Windows.Forms.ComboBox()
		Me.cbEpcLen = New System.Windows.Forms.ComboBox()
		Me.label3 = New System.Windows.Forms.Label()
		Me.label2 = New System.Windows.Forms.Label()
		Me.txtUM2 = New System.Windows.Forms.TextBox()
		Me.txtUM1 = New System.Windows.Forms.TextBox()
		Me.txtEPC = New System.Windows.Forms.TextBox()
		Me.txtPC = New System.Windows.Forms.TextBox()
		Me.chkUM = New System.Windows.Forms.CheckBox()
		Me.chkEPC = New System.Windows.Forms.CheckBox()
		Me.chkPC = New System.Windows.Forms.CheckBox()
		Me.groupBox4 = New System.Windows.Forms.GroupBox()
		Me.cbMBLen = New System.Windows.Forms.ComboBox()
		Me.cbMB = New System.Windows.Forms.ComboBox()
		Me.chkMB = New System.Windows.Forms.CheckBox()
		Me.groupBox5 = New System.Windows.Forms.GroupBox()
		Me.cbMB3Len = New System.Windows.Forms.ComboBox()
		Me.label6 = New System.Windows.Forms.Label()
		Me.cbMB2Len = New System.Windows.Forms.ComboBox()
		Me.label5 = New System.Windows.Forms.Label()
		Me.cbMB0Len = New System.Windows.Forms.ComboBox()
		Me.label4 = New System.Windows.Forms.Label()
		Me.btnClearTag = New System.Windows.Forms.Button()
		Me.btnClearStatus = New System.Windows.Forms.Button()
		Me.lv_tag = New System.Windows.Forms.ListView()
		Me.col_index = DirectCast(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
		Me.col_epc = DirectCast(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
		Me.col_rssi = DirectCast(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
		Me.col_count = DirectCast(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
		Me.col_data = DirectCast(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
		Me.tbStatus = New System.Windows.Forms.TextBox()
		Me.splitContainer1 = New System.Windows.Forms.SplitContainer()
		Me.label7 = New System.Windows.Forms.Label()
		Me.label1 = New System.Windows.Forms.Label()
		Me.lb_rate = New System.Windows.Forms.Label()
		Me.timer_rate = New System.Windows.Forms.Timer(Me.components)
		Me.statusStrip.SuspendLayout()
		Me.groupBox1.SuspendLayout()
		Me.groupBox2.SuspendLayout()
		Me.groupBox3.SuspendLayout()
		Me.groupBox4.SuspendLayout()
		Me.groupBox5.SuspendLayout()
		Me.splitContainer1.Panel1.SuspendLayout()
		Me.splitContainer1.SuspendLayout()
		Me.SuspendLayout()
		' 
		' statusStrip
		' 
		Me.statusStrip.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.toolStripStatusLabel1})
		Me.statusStrip.Location = New System.Drawing.Point(0, 656)
		Me.statusStrip.Name = "statusStrip"
		Me.statusStrip.Size = New System.Drawing.Size(1016, 22)
		Me.statusStrip.TabIndex = 0
		' 
		' toolStripStatusLabel1
		' 
		Me.toolStripStatusLabel1.AutoSize = False
		Me.toolStripStatusLabel1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
		Me.toolStripStatusLabel1.Name = "toolStripStatusLabel1"
		Me.toolStripStatusLabel1.Size = New System.Drawing.Size(400, 17)
		Me.toolStripStatusLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
		' 
		' groupBox1
		' 
		Me.groupBox1.Controls.Add(Me.btnConnect)
		Me.groupBox1.Controls.Add(Me.btnUpdate)
		Me.groupBox1.Location = New System.Drawing.Point(13, 12)
		Me.groupBox1.Name = "groupBox1"
		Me.groupBox1.Size = New System.Drawing.Size(264, 53)
		Me.groupBox1.TabIndex = 4
		Me.groupBox1.TabStop = False
		Me.groupBox1.Text = "Reader"
		' 
		' btnConnect
		' 
		Me.btnConnect.FlatStyle = System.Windows.Forms.FlatStyle.Flat
		Me.btnConnect.Font = New System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CByte(0))
		Me.btnConnect.Location = New System.Drawing.Point(12, 16)
		Me.btnConnect.Name = "btnConnect"
		Me.btnConnect.Size = New System.Drawing.Size(114, 26)
		Me.btnConnect.TabIndex = 0
		Me.btnConnect.Text = "Connect"
		Me.btnConnect.UseVisualStyleBackColor = True
		AddHandler Me.btnConnect.Click, New System.EventHandler(AddressOf Me.btnConnect_Click)
		' 
		' btnUpdate
		' 
		Me.btnUpdate.FlatStyle = System.Windows.Forms.FlatStyle.Flat
		Me.btnUpdate.Font = New System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CByte(0))
		Me.btnUpdate.Location = New System.Drawing.Point(141, 16)
		Me.btnUpdate.Name = "btnUpdate"
		Me.btnUpdate.Size = New System.Drawing.Size(112, 26)
		Me.btnUpdate.TabIndex = 0
		Me.btnUpdate.Text = "Update"
		Me.btnUpdate.UseVisualStyleBackColor = True
		AddHandler Me.btnUpdate.Click, New System.EventHandler(AddressOf Me.btnUpdate_Click)
		' 
		' groupBox2
		' 
		Me.groupBox2.Controls.Add(Me.comboBox1)
		Me.groupBox2.Controls.Add(Me.btnTest)
		Me.groupBox2.Controls.Add(Me.btnStop)
		Me.groupBox2.Location = New System.Drawing.Point(283, 12)
		Me.groupBox2.Name = "groupBox2"
		Me.groupBox2.Size = New System.Drawing.Size(311, 53)
		Me.groupBox2.TabIndex = 5
		Me.groupBox2.TabStop = False
		Me.groupBox2.Text = "Commands"
		' 
		' comboBox1
		' 
		Me.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
		Me.comboBox1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CByte(0))
		Me.comboBox1.FormattingEnabled = True
		Me.comboBox1.Items.AddRange(New Object() {"Inventory", "TagRead", "TagWrite", "SetGPO", "GetGPI"})
		Me.comboBox1.Location = New System.Drawing.Point(10, 20)
		Me.comboBox1.Name = "comboBox1"
		Me.comboBox1.Size = New System.Drawing.Size(105, 21)
		Me.comboBox1.TabIndex = 1
		' 
		' btnTest
		' 
		Me.btnTest.FlatStyle = System.Windows.Forms.FlatStyle.Flat
		Me.btnTest.Font = New System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CByte(0))
		Me.btnTest.Location = New System.Drawing.Point(121, 14)
		Me.btnTest.Name = "btnTest"
		Me.btnTest.Size = New System.Drawing.Size(84, 28)
		Me.btnTest.TabIndex = 0
		Me.btnTest.Text = "Start"
		Me.btnTest.UseVisualStyleBackColor = True
		AddHandler Me.btnTest.Click, New System.EventHandler(AddressOf Me.btnTest_Click)
		' 
		' btnStop
		' 
		Me.btnStop.Enabled = False
		Me.btnStop.FlatStyle = System.Windows.Forms.FlatStyle.Flat
		Me.btnStop.Font = New System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CByte(0))
		Me.btnStop.Location = New System.Drawing.Point(216, 14)
		Me.btnStop.Name = "btnStop"
		Me.btnStop.Size = New System.Drawing.Size(85, 28)
		Me.btnStop.TabIndex = 0
		Me.btnStop.Text = "Stop"
		Me.btnStop.UseVisualStyleBackColor = True
		AddHandler Me.btnStop.Click, New System.EventHandler(AddressOf Me.btnStop_Click)
		' 
		' groupBox3
		' 
		Me.groupBox3.Controls.Add(Me.cbUmLen)
		Me.groupBox3.Controls.Add(Me.cbEpcLen)
		Me.groupBox3.Controls.Add(Me.label3)
		Me.groupBox3.Controls.Add(Me.label2)
		Me.groupBox3.Controls.Add(Me.txtUM2)
		Me.groupBox3.Controls.Add(Me.txtUM1)
		Me.groupBox3.Controls.Add(Me.txtEPC)
		Me.groupBox3.Controls.Add(Me.txtPC)
		Me.groupBox3.Controls.Add(Me.chkUM)
		Me.groupBox3.Controls.Add(Me.chkEPC)
		Me.groupBox3.Controls.Add(Me.chkPC)
		Me.groupBox3.Location = New System.Drawing.Point(13, 71)
		Me.groupBox3.Name = "groupBox3"
		Me.groupBox3.Size = New System.Drawing.Size(1000, 101)
		Me.groupBox3.TabIndex = 7
		Me.groupBox3.TabStop = False
		Me.groupBox3.Text = "Write parameters"
		' 
		' cbUmLen
		' 
		Me.cbUmLen.FormattingEnabled = True
		Me.cbUmLen.Location = New System.Drawing.Point(807, 58)
		Me.cbUmLen.Name = "cbUmLen"
		Me.cbUmLen.Size = New System.Drawing.Size(107, 20)
		Me.cbUmLen.TabIndex = 11
		AddHandler Me.cbUmLen.SelectedIndexChanged, New System.EventHandler(AddressOf Me.cbUmLen_SelectedIndexChanged)
		' 
		' cbEpcLen
		' 
		Me.cbEpcLen.FormattingEnabled = True
		Me.cbEpcLen.Location = New System.Drawing.Point(807, 37)
		Me.cbEpcLen.Name = "cbEpcLen"
		Me.cbEpcLen.Size = New System.Drawing.Size(107, 20)
		Me.cbEpcLen.TabIndex = 10
		AddHandler Me.cbEpcLen.SelectedIndexChanged, New System.EventHandler(AddressOf Me.cbEpcLen_SelectedIndexChanged)
		' 
		' label3
		' 
		Me.label3.AutoSize = True
		Me.label3.Location = New System.Drawing.Point(707, 61)
		Me.label3.Name = "label3"
		Me.label3.Size = New System.Drawing.Size(93, 12)
		Me.label3.TabIndex = 9
		Me.label3.Text = "Word to be written"
		' 
		' label2
		' 
		Me.label2.AutoSize = True
		Me.label2.Location = New System.Drawing.Point(707, 40)
		Me.label2.Name = "label2"
		Me.label2.Size = New System.Drawing.Size(93, 12)
		Me.label2.TabIndex = 8
		Me.label2.Text = "Word to be written"
		' 
		' txtUM2
		' 
		Me.txtUM2.Location = New System.Drawing.Point(141, 77)
		Me.txtUM2.MaxLength = 64
		Me.txtUM2.Name = "txtUM2"
		Me.txtUM2.Size = New System.Drawing.Size(560, 22)
		Me.txtUM2.TabIndex = 6
		Me.txtUM2.Text = "8888888899999999AAAAAAAABBBBBBBBCCCCCCCCDDDDDDDDEEEEEEEEFFFFFFFF"
		Me.txtUM2.Visible = False
		AddHandler Me.txtUM2.TextChanged, New System.EventHandler(AddressOf Me.txtUM2_TextChanged)
		' 
		' txtUM1
		' 
		Me.txtUM1.Location = New System.Drawing.Point(141, 58)
		Me.txtUM1.MaxLength = 64
		Me.txtUM1.Name = "txtUM1"
		Me.txtUM1.Size = New System.Drawing.Size(560, 22)
		Me.txtUM1.TabIndex = 5
		Me.txtUM1.Text = "0123456789ABCDEF0123456789ABCDEF0123456789ABCDEF0123456789ABCDEF"
		AddHandler Me.txtUM1.TextChanged, New System.EventHandler(AddressOf Me.txtUM1_TextChanged)
		' 
		' txtEPC
		' 
		Me.txtEPC.Location = New System.Drawing.Point(141, 37)
		Me.txtEPC.MaxLength = 60
		Me.txtEPC.Name = "txtEPC"
		Me.txtEPC.Size = New System.Drawing.Size(560, 22)
		Me.txtEPC.TabIndex = 4
		Me.txtEPC.Text = "00112233445566778899AABB"
		AddHandler Me.txtEPC.TextChanged, New System.EventHandler(AddressOf Me.txtEPC_TextChanged)
		' 
		' txtPC
		' 
		Me.txtPC.Location = New System.Drawing.Point(141, 16)
		Me.txtPC.MaxLength = 4
		Me.txtPC.Name = "txtPC"
		Me.txtPC.Size = New System.Drawing.Size(47, 22)
		Me.txtPC.TabIndex = 3
		Me.txtPC.Text = "3000"
		AddHandler Me.txtPC.TextChanged, New System.EventHandler(AddressOf Me.txtPC_TextChanged)
		' 
		' chkUM
		' 
		Me.chkUM.AutoSize = True
		Me.chkUM.Checked = True
		Me.chkUM.CheckState = System.Windows.Forms.CheckState.Checked
		Me.chkUM.Location = New System.Drawing.Point(12, 60)
		Me.chkUM.Name = "chkUM"
		Me.chkUM.Size = New System.Drawing.Size(132, 16)
		Me.chkUM.TabIndex = 2
		Me.chkUM.Text = "User memory (256bits)"
		Me.chkUM.UseVisualStyleBackColor = True
		AddHandler Me.chkUM.CheckedChanged, New System.EventHandler(AddressOf Me.chkUM_CheckedChanged)
		' 
		' chkEPC
		' 
		Me.chkEPC.AutoSize = True
		Me.chkEPC.Location = New System.Drawing.Point(12, 39)
		Me.chkEPC.Name = "chkEPC"
		Me.chkEPC.Size = New System.Drawing.Size(84, 16)
		Me.chkEPC.TabIndex = 1
		Me.chkEPC.Text = "EPC (96bits)"
		Me.chkEPC.UseVisualStyleBackColor = True
		AddHandler Me.chkEPC.CheckedChanged, New System.EventHandler(AddressOf Me.chkEPC_CheckedChanged)
		' 
		' chkPC
		' 
		Me.chkPC.AutoSize = True
		Me.chkPC.Location = New System.Drawing.Point(12, 18)
		Me.chkPC.Name = "chkPC"
		Me.chkPC.Size = New System.Drawing.Size(77, 16)
		Me.chkPC.TabIndex = 0
		Me.chkPC.Text = "PC (16bits)"
		Me.chkPC.UseVisualStyleBackColor = True
		AddHandler Me.chkPC.CheckedChanged, New System.EventHandler(AddressOf Me.chkPC_CheckedChanged)
		' 
		' groupBox4
		' 
		Me.groupBox4.Controls.Add(Me.cbMBLen)
		Me.groupBox4.Controls.Add(Me.cbMB)
		Me.groupBox4.Controls.Add(Me.chkMB)
		Me.groupBox4.Location = New System.Drawing.Point(13, 178)
		Me.groupBox4.Name = "groupBox4"
		Me.groupBox4.Size = New System.Drawing.Size(445, 46)
		Me.groupBox4.TabIndex = 9
		Me.groupBox4.TabStop = False
		Me.groupBox4.Text = "Inventory Options"
		' 
		' cbMBLen
		' 
		Me.cbMBLen.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
		Me.cbMBLen.Enabled = False
		Me.cbMBLen.FormattingEnabled = True
		Me.cbMBLen.Location = New System.Drawing.Point(314, 16)
		Me.cbMBLen.Name = "cbMBLen"
		Me.cbMBLen.Size = New System.Drawing.Size(118, 20)
		Me.cbMBLen.TabIndex = 4
		' 
		' cbMB
		' 
		Me.cbMB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
		Me.cbMB.Enabled = False
		Me.cbMB.FormattingEnabled = True
		Me.cbMB.Items.AddRange(New Object() {"MB0 - Reserved", "NONE", "MB2 - TID", "MB3 - User memory"})
		Me.cbMB.Location = New System.Drawing.Point(187, 16)
		Me.cbMB.Name = "cbMB"
		Me.cbMB.Size = New System.Drawing.Size(121, 20)
		Me.cbMB.TabIndex = 3
		' 
		' chkMB
		' 
		Me.chkMB.AutoSize = True
		Me.chkMB.Location = New System.Drawing.Point(12, 18)
		Me.chkMB.Name = "chkMB"
		Me.chkMB.Size = New System.Drawing.Size(171, 16)
		Me.chkMB.TabIndex = 2
		Me.chkMB.Text = "Read Additional Memory Bank"
		Me.chkMB.UseVisualStyleBackColor = True
		AddHandler Me.chkMB.CheckedChanged, New System.EventHandler(AddressOf Me.chkMB_CheckedChanged)
		' 
		' groupBox5
		' 
		Me.groupBox5.Controls.Add(Me.cbMB3Len)
		Me.groupBox5.Controls.Add(Me.label6)
		Me.groupBox5.Controls.Add(Me.cbMB2Len)
		Me.groupBox5.Controls.Add(Me.label5)
		Me.groupBox5.Controls.Add(Me.cbMB0Len)
		Me.groupBox5.Controls.Add(Me.label4)
		Me.groupBox5.Location = New System.Drawing.Point(464, 178)
		Me.groupBox5.Name = "groupBox5"
		Me.groupBox5.Size = New System.Drawing.Size(549, 46)
		Me.groupBox5.TabIndex = 10
		Me.groupBox5.TabStop = False
		Me.groupBox5.Text = "Read Options"
		' 
		' cbMB3Len
		' 
		Me.cbMB3Len.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
		Me.cbMB3Len.FormattingEnabled = True
		Me.cbMB3Len.Location = New System.Drawing.Point(332, 18)
		Me.cbMB3Len.Name = "cbMB3Len"
		Me.cbMB3Len.Size = New System.Drawing.Size(90, 20)
		Me.cbMB3Len.TabIndex = 5
		' 
		' label6
		' 
		Me.label6.AutoSize = True
		Me.label6.Location = New System.Drawing.Point(295, 19)
		Me.label6.Name = "label6"
		Me.label6.Size = New System.Drawing.Size(29, 12)
		Me.label6.TabIndex = 4
		Me.label6.Text = "MB3"
		' 
		' cbMB2Len
		' 
		Me.cbMB2Len.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
		Me.cbMB2Len.FormattingEnabled = True
		Me.cbMB2Len.Location = New System.Drawing.Point(199, 18)
		Me.cbMB2Len.Name = "cbMB2Len"
		Me.cbMB2Len.Size = New System.Drawing.Size(90, 20)
		Me.cbMB2Len.TabIndex = 3
		' 
		' label5
		' 
		Me.label5.AutoSize = True
		Me.label5.Location = New System.Drawing.Point(162, 19)
		Me.label5.Name = "label5"
		Me.label5.Size = New System.Drawing.Size(29, 12)
		Me.label5.TabIndex = 2
		Me.label5.Text = "MB2"
		' 
		' cbMB0Len
		' 
		Me.cbMB0Len.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
		Me.cbMB0Len.FormattingEnabled = True
		Me.cbMB0Len.Location = New System.Drawing.Point(56, 17)
		Me.cbMB0Len.Name = "cbMB0Len"
		Me.cbMB0Len.Size = New System.Drawing.Size(90, 20)
		Me.cbMB0Len.TabIndex = 1
		' 
		' label4
		' 
		Me.label4.AutoSize = True
		Me.label4.Location = New System.Drawing.Point(19, 18)
		Me.label4.Name = "label4"
		Me.label4.Size = New System.Drawing.Size(29, 12)
		Me.label4.TabIndex = 0
		Me.label4.Text = "MB0"
		' 
		' btnClearTag
		' 
		Me.btnClearTag.FlatStyle = System.Windows.Forms.FlatStyle.Flat
		Me.btnClearTag.Font = New System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CByte(0))
		Me.btnClearTag.Location = New System.Drawing.Point(920, 403)
		Me.btnClearTag.Name = "btnClearTag"
		Me.btnClearTag.Size = New System.Drawing.Size(84, 28)
		Me.btnClearTag.TabIndex = 2
		Me.btnClearTag.Text = "Clear"
		Me.btnClearTag.UseVisualStyleBackColor = True
		AddHandler Me.btnClearTag.Click, New System.EventHandler(AddressOf Me.btnClearTag_Click)
		' 
		' btnClearStatus
		' 
		Me.btnClearStatus.FlatStyle = System.Windows.Forms.FlatStyle.Flat
		Me.btnClearStatus.Font = New System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CByte(0))
		Me.btnClearStatus.Location = New System.Drawing.Point(920, 595)
		Me.btnClearStatus.Name = "btnClearStatus"
		Me.btnClearStatus.Size = New System.Drawing.Size(84, 28)
		Me.btnClearStatus.TabIndex = 11
		Me.btnClearStatus.Text = "Clear"
		Me.btnClearStatus.UseVisualStyleBackColor = True
		AddHandler Me.btnClearStatus.Click, New System.EventHandler(AddressOf Me.btnClearStatus_Click)
		' 
		' lv_tag
		' 
		Me.lv_tag.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.col_index, Me.col_epc, Me.col_rssi, Me.col_count, Me.col_data})
		Me.lv_tag.GridLines = True
		Me.lv_tag.Location = New System.Drawing.Point(13, 230)
		Me.lv_tag.Name = "lv_tag"
		Me.lv_tag.Size = New System.Drawing.Size(901, 201)
		Me.lv_tag.TabIndex = 0
		Me.lv_tag.UseCompatibleStateImageBehavior = False
		Me.lv_tag.View = System.Windows.Forms.View.Details
		' 
		' col_index
		' 
		Me.col_index.Text = "Index"
		Me.col_index.Width = 50
		' 
		' col_epc
		' 
		Me.col_epc.Text = "EPC"
		Me.col_epc.Width = 350
		' 
		' col_rssi
		' 
		Me.col_rssi.Text = "RSSI"
		Me.col_rssi.Width = 50
		' 
		' col_count
		' 
		Me.col_count.Text = "Count"
		Me.col_count.Width = 50
		' 
		' col_data
		' 
		Me.col_data.Text = "Data"
		Me.col_data.Width = 380
		' 
		' tbStatus
		' 
		Me.tbStatus.Location = New System.Drawing.Point(12, 437)
		Me.tbStatus.Multiline = True
		Me.tbStatus.Name = "tbStatus"
		Me.tbStatus.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
		Me.tbStatus.Size = New System.Drawing.Size(902, 186)
		Me.tbStatus.TabIndex = 12
		' 
		' splitContainer1
		' 
		Me.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill
		Me.splitContainer1.Location = New System.Drawing.Point(0, 0)
		Me.splitContainer1.Name = "splitContainer1"
		Me.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal
		' 
		' splitContainer1.Panel1
		' 
		Me.splitContainer1.Panel1.Controls.Add(Me.label7)
		Me.splitContainer1.Panel1.Controls.Add(Me.label1)
		Me.splitContainer1.Panel1.Controls.Add(Me.lb_rate)
		Me.splitContainer1.Panel1.Controls.Add(Me.tbStatus)
		Me.splitContainer1.Panel1.Controls.Add(Me.lv_tag)
		Me.splitContainer1.Panel1.Controls.Add(Me.btnClearStatus)
		Me.splitContainer1.Panel1.Controls.Add(Me.btnClearTag)
		Me.splitContainer1.Panel1.Controls.Add(Me.groupBox5)
		Me.splitContainer1.Panel1.Controls.Add(Me.groupBox4)
		Me.splitContainer1.Panel1.Controls.Add(Me.groupBox3)
		Me.splitContainer1.Panel1.Controls.Add(Me.groupBox2)
		Me.splitContainer1.Panel1.Controls.Add(Me.groupBox1)
		Me.splitContainer1.Size = New System.Drawing.Size(1016, 656)
		Me.splitContainer1.SplitterDistance = 626
		Me.splitContainer1.TabIndex = 1
		' 
		' label7
		' 
		Me.label7.AutoSize = True
		Me.label7.Location = New System.Drawing.Point(974, 375)
		Me.label7.Name = "label7"
		Me.label7.Size = New System.Drawing.Size(30, 12)
		Me.label7.TabIndex = 21
		Me.label7.Text = "tags/s"
		' 
		' label1
		' 
		Me.label1.AutoSize = True
		Me.label1.Location = New System.Drawing.Point(918, 375)
		Me.label1.Name = "label1"
		Me.label1.Size = New System.Drawing.Size(29, 12)
		Me.label1.TabIndex = 14
		Me.label1.Text = "Rate:"
		' 
		' lb_rate
		' 
		Me.lb_rate.AutoSize = True
		Me.lb_rate.Location = New System.Drawing.Point(947, 375)
		Me.lb_rate.Name = "lb_rate"
		Me.lb_rate.Size = New System.Drawing.Size(11, 12)
		Me.lb_rate.TabIndex = 13
		Me.lb_rate.Text = "0"
		' 
		' timer_rate
		' 
		Me.timer_rate.Enabled = True
		Me.timer_rate.Interval = 1000
		AddHandler Me.timer_rate.Tick, New System.EventHandler(AddressOf Me.readrate_Tick)
		' 
		' MainForm
		' 
		Me.AutoScaleDimensions = New System.Drawing.SizeF(6F, 12F)
		Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
		Me.ClientSize = New System.Drawing.Size(1016, 678)
		Me.Controls.Add(Me.splitContainer1)
		Me.Controls.Add(Me.statusStrip)
		Me.Name = "MainForm"
		Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
		Me.Text = "CS463 MACH1 Demo"
		AddHandler Me.Load, New System.EventHandler(AddressOf Me.MainForm_Load)
		Me.statusStrip.ResumeLayout(False)
		Me.statusStrip.PerformLayout()
		Me.groupBox1.ResumeLayout(False)
		Me.groupBox2.ResumeLayout(False)
		Me.groupBox3.ResumeLayout(False)
		Me.groupBox3.PerformLayout()
		Me.groupBox4.ResumeLayout(False)
		Me.groupBox4.PerformLayout()
		Me.groupBox5.ResumeLayout(False)
		Me.groupBox5.PerformLayout()
		Me.splitContainer1.Panel1.ResumeLayout(False)
		Me.splitContainer1.Panel1.PerformLayout()
		Me.splitContainer1.ResumeLayout(False)
		Me.ResumeLayout(False)
		Me.PerformLayout()

	End Sub

	#End Region

	Private statusStrip As System.Windows.Forms.StatusStrip
	Private toolStripStatusLabel1 As System.Windows.Forms.ToolStripStatusLabel
	Private groupBox1 As System.Windows.Forms.GroupBox
	Private btnConnect As System.Windows.Forms.Button
	Private btnUpdate As System.Windows.Forms.Button
	Private groupBox2 As System.Windows.Forms.GroupBox
	Private comboBox1 As System.Windows.Forms.ComboBox
	Private btnTest As System.Windows.Forms.Button
	Private btnStop As System.Windows.Forms.Button
	Private groupBox3 As System.Windows.Forms.GroupBox
	Private cbUmLen As System.Windows.Forms.ComboBox
	Private cbEpcLen As System.Windows.Forms.ComboBox
	Private label3 As System.Windows.Forms.Label
	Private label2 As System.Windows.Forms.Label
	Private txtUM2 As System.Windows.Forms.TextBox
	Private txtUM1 As System.Windows.Forms.TextBox
	Private txtEPC As System.Windows.Forms.TextBox
	Private txtPC As System.Windows.Forms.TextBox
	Private chkUM As System.Windows.Forms.CheckBox
	Private chkEPC As System.Windows.Forms.CheckBox
	Private chkPC As System.Windows.Forms.CheckBox
	Private groupBox4 As System.Windows.Forms.GroupBox
	Private cbMBLen As System.Windows.Forms.ComboBox
	Private cbMB As System.Windows.Forms.ComboBox
	Private chkMB As System.Windows.Forms.CheckBox
	Private groupBox5 As System.Windows.Forms.GroupBox
	Private cbMB3Len As System.Windows.Forms.ComboBox
	Private label6 As System.Windows.Forms.Label
	Private cbMB2Len As System.Windows.Forms.ComboBox
	Private label5 As System.Windows.Forms.Label
	Private cbMB0Len As System.Windows.Forms.ComboBox
	Private label4 As System.Windows.Forms.Label
	Private btnClearTag As System.Windows.Forms.Button
	Private btnClearStatus As System.Windows.Forms.Button
	Private lv_tag As System.Windows.Forms.ListView
	Private col_index As System.Windows.Forms.ColumnHeader
	Private col_epc As System.Windows.Forms.ColumnHeader
	Private col_rssi As System.Windows.Forms.ColumnHeader
	Private col_count As System.Windows.Forms.ColumnHeader
	Private col_data As System.Windows.Forms.ColumnHeader
	Private tbStatus As System.Windows.Forms.TextBox
	Private splitContainer1 As System.Windows.Forms.SplitContainer
	Private label1 As System.Windows.Forms.Label
	Private lb_rate As System.Windows.Forms.Label
	Private timer_rate As System.Windows.Forms.Timer
	Private label7 As System.Windows.Forms.Label

End Class

