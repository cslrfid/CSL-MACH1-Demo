Partial Class CSLReaderParams
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

	#Region "Component Designer generated code"

	''' <summary>
	''' Required method for Designer support - do not modify 
	''' the contents of this method with the code editor.
	''' </summary>
	Private Sub InitializeComponent()
		Dim resources As New System.ComponentModel.ComponentResourceManager(GetType(CSLReaderParams))
		Me.tpGen2 = New System.Windows.Forms.TabPage()
		Me.cbSession = New System.Windows.Forms.ComboBox()
		Me.label16 = New System.Windows.Forms.Label()
		Me.label4 = New System.Windows.Forms.Label()
		Me.cbAutoSetMode = New System.Windows.Forms.ComboBox()
		Me.cbInventoryMode = New System.Windows.Forms.ComboBox()
		Me.cbReaderMode = New System.Windows.Forms.ComboBox()
		Me.label15 = New System.Windows.Forms.Label()
		Me.label3 = New System.Windows.Forms.Label()
		Me.tpAntenna = New System.Windows.Forms.TabPage()
		Me.label9 = New System.Windows.Forms.Label()
		Me.tbPower1 = New System.Windows.Forms.TextBox()
		Me.tbPower3 = New System.Windows.Forms.TextBox()
		Me.tbPower4 = New System.Windows.Forms.TextBox()
		Me.tbPower2 = New System.Windows.Forms.TextBox()
		Me.label12 = New System.Windows.Forms.Label()
		Me.label13 = New System.Windows.Forms.Label()
		Me.chAnt4 = New System.Windows.Forms.CheckBox()
		Me.chAnt1 = New System.Windows.Forms.CheckBox()
		Me.chAnt3 = New System.Windows.Forms.CheckBox()
		Me.chAnt2 = New System.Windows.Forms.CheckBox()
		Me.tpReader = New System.Windows.Forms.TabPage()
		Me.cbFrequencyList = New System.Windows.Forms.ComboBox()
		Me.cbRegion = New System.Windows.Forms.ComboBox()
		Me.cbFrequencyMode = New System.Windows.Forms.ComboBox()
		Me.tbReaderName = New System.Windows.Forms.TextBox()
		Me.label1 = New System.Windows.Forms.Label()
		Me.label6 = New System.Windows.Forms.Label()
		Me.label2 = New System.Windows.Forms.Label()
		Me.label5 = New System.Windows.Forms.Label()
		Me.tabControl1 = New System.Windows.Forms.TabControl()
		Me.tabPage1 = New System.Windows.Forms.TabPage()
		Me.llCSL = New System.Windows.Forms.LinkLabel()
		Me.pictureBox2 = New System.Windows.Forms.PictureBox()
		Me.label11 = New System.Windows.Forms.Label()
		Me.lbFirmware = New System.Windows.Forms.Label()
		Me.lbSoftware = New System.Windows.Forms.Label()
		Me.label8 = New System.Windows.Forms.Label()
		Me.label7 = New System.Windows.Forms.Label()
		Me.tbPopulation = New System.Windows.Forms.TextBox()
		Me.tpGen2.SuspendLayout()
		Me.tpAntenna.SuspendLayout()
		Me.tpReader.SuspendLayout()
		Me.tabControl1.SuspendLayout()
		Me.tabPage1.SuspendLayout()
		DirectCast(Me.pictureBox2, System.ComponentModel.ISupportInitialize).BeginInit()
		Me.SuspendLayout()
		' 
		' tpGen2
		' 
		Me.tpGen2.Controls.Add(Me.tbPopulation)
		Me.tpGen2.Controls.Add(Me.label7)
		Me.tpGen2.Controls.Add(Me.cbSession)
		Me.tpGen2.Controls.Add(Me.label16)
		Me.tpGen2.Controls.Add(Me.label4)
		Me.tpGen2.Controls.Add(Me.cbAutoSetMode)
		Me.tpGen2.Controls.Add(Me.cbInventoryMode)
		Me.tpGen2.Controls.Add(Me.cbReaderMode)
		Me.tpGen2.Controls.Add(Me.label15)
		Me.tpGen2.Controls.Add(Me.label3)
		Me.tpGen2.Location = New System.Drawing.Point(4, 21)
		Me.tpGen2.Name = "tpGen2"
		Me.tpGen2.Size = New System.Drawing.Size(381, 162)
		Me.tpGen2.TabIndex = 2
		Me.tpGen2.Text = "Gen2 Parameters"
		Me.tpGen2.UseVisualStyleBackColor = True
		' 
		' cbSession
		' 
		Me.cbSession.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
		Me.cbSession.FormattingEnabled = True
		Me.cbSession.Items.AddRange(New Object() {"Session 0", "Session 1", "Session 2", "Session 3"})
		Me.cbSession.Location = New System.Drawing.Point(154, 14)
		Me.cbSession.Name = "cbSession"
		Me.cbSession.Size = New System.Drawing.Size(137, 20)
		Me.cbSession.TabIndex = 2
		' 
		' label16
		' 
		Me.label16.AutoSize = True
		Me.label16.Location = New System.Drawing.Point(28, 96)
		Me.label16.Name = "label16"
		Me.label16.RightToLeft = System.Windows.Forms.RightToLeft.Yes
		Me.label16.Size = New System.Drawing.Size(115, 12)
		Me.label16.TabIndex = 1
		Me.label16.Text = "Inventory Search Mode"
		Me.label16.TextAlign = System.Drawing.ContentAlignment.MiddleRight
		' 
		' label4
		' 
		Me.label4.AutoSize = True
		Me.label4.Location = New System.Drawing.Point(74, 71)
		Me.label4.Name = "label4"
		Me.label4.RightToLeft = System.Windows.Forms.RightToLeft.Yes
		Me.label4.Size = New System.Drawing.Size(67, 12)
		Me.label4.TabIndex = 1
		Me.label4.Text = "Mode ID List"
		Me.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight
		' 
		' cbAutoSetMode
		' 
		Me.cbAutoSetMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
		Me.cbAutoSetMode.FormattingEnabled = True
		Me.cbAutoSetMode.Items.AddRange(New Object() {"2 - By Mode ID"})
		Me.cbAutoSetMode.Location = New System.Drawing.Point(154, 41)
		Me.cbAutoSetMode.Name = "cbAutoSetMode"
		Me.cbAutoSetMode.Size = New System.Drawing.Size(137, 20)
		Me.cbAutoSetMode.TabIndex = 2
		AddHandler Me.cbAutoSetMode.SelectedIndexChanged, New System.EventHandler(AddressOf Me.cbAutoSetMode_SelectedIndexChanged)
		' 
		' cbInventoryMode
		' 
		Me.cbInventoryMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
		Me.cbInventoryMode.FormattingEnabled = True
		Me.cbInventoryMode.Items.AddRange(New Object() {"0 - Dual Target", "1 - Single Target (A)"})
		Me.cbInventoryMode.Location = New System.Drawing.Point(154, 93)
		Me.cbInventoryMode.Name = "cbInventoryMode"
		Me.cbInventoryMode.Size = New System.Drawing.Size(137, 20)
		Me.cbInventoryMode.TabIndex = 2
		' 
		' cbReaderMode
		' 
		Me.cbReaderMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
		Me.cbReaderMode.FormattingEnabled = True
		Me.cbReaderMode.Items.AddRange(New Object() {"0", "1", "2", "3"})
		Me.cbReaderMode.Location = New System.Drawing.Point(154, 68)
		Me.cbReaderMode.Name = "cbReaderMode"
		Me.cbReaderMode.Size = New System.Drawing.Size(137, 20)
		Me.cbReaderMode.TabIndex = 2
		' 
		' label15
		' 
		Me.label15.AutoSize = True
		Me.label15.Location = New System.Drawing.Point(68, 44)
		Me.label15.Name = "label15"
		Me.label15.RightToLeft = System.Windows.Forms.RightToLeft.Yes
		Me.label15.Size = New System.Drawing.Size(75, 12)
		Me.label15.TabIndex = 1
		Me.label15.Text = "Auto Set Mode"
		Me.label15.TextAlign = System.Drawing.ContentAlignment.MiddleRight
		' 
		' label3
		' 
		Me.label3.AutoSize = True
		Me.label3.Location = New System.Drawing.Point(102, 17)
		Me.label3.Name = "label3"
		Me.label3.RightToLeft = System.Windows.Forms.RightToLeft.Yes
		Me.label3.Size = New System.Drawing.Size(39, 12)
		Me.label3.TabIndex = 1
		Me.label3.Text = "Session"
		Me.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight
		' 
		' tpAntenna
		' 
		Me.tpAntenna.Controls.Add(Me.label9)
		Me.tpAntenna.Controls.Add(Me.tbPower1)
		Me.tpAntenna.Controls.Add(Me.tbPower3)
		Me.tpAntenna.Controls.Add(Me.tbPower4)
		Me.tpAntenna.Controls.Add(Me.tbPower2)
		Me.tpAntenna.Controls.Add(Me.label12)
		Me.tpAntenna.Controls.Add(Me.label13)
		Me.tpAntenna.Controls.Add(Me.chAnt4)
		Me.tpAntenna.Controls.Add(Me.chAnt1)
		Me.tpAntenna.Controls.Add(Me.chAnt3)
		Me.tpAntenna.Controls.Add(Me.chAnt2)
		Me.tpAntenna.Location = New System.Drawing.Point(4, 21)
		Me.tpAntenna.Name = "tpAntenna"
		Me.tpAntenna.Padding = New System.Windows.Forms.Padding(3)
		Me.tpAntenna.Size = New System.Drawing.Size(381, 162)
		Me.tpAntenna.TabIndex = 1
		Me.tpAntenna.Text = "Antenna Setting"
		Me.tpAntenna.UseVisualStyleBackColor = True
		AddHandler Me.tpAntenna.Click, New System.EventHandler(AddressOf Me.tpAntenna_Click)
		' 
		' label9
		' 
		Me.label9.AutoSize = True
		Me.label9.Location = New System.Drawing.Point(137, 19)
		Me.label9.Name = "label9"
		Me.label9.Size = New System.Drawing.Size(0, 12)
		Me.label9.TabIndex = 13
		' 
		' tbPower1
		' 
		Me.tbPower1.Location = New System.Drawing.Point(86, 47)
		Me.tbPower1.Name = "tbPower1"
		Me.tbPower1.Size = New System.Drawing.Size(38, 22)
		Me.tbPower1.TabIndex = 9
		Me.tbPower1.Text = "30.00"
		' 
		' tbPower3
		' 
		Me.tbPower3.Location = New System.Drawing.Point(86, 84)
		Me.tbPower3.Name = "tbPower3"
		Me.tbPower3.Size = New System.Drawing.Size(38, 22)
		Me.tbPower3.TabIndex = 9
		Me.tbPower3.Text = "30.00"
		' 
		' tbPower4
		' 
		Me.tbPower4.Location = New System.Drawing.Point(86, 102)
		Me.tbPower4.Name = "tbPower4"
		Me.tbPower4.Size = New System.Drawing.Size(38, 22)
		Me.tbPower4.TabIndex = 9
		Me.tbPower4.Text = "30.00"
		' 
		' tbPower2
		' 
		Me.tbPower2.Location = New System.Drawing.Point(86, 66)
		Me.tbPower2.Name = "tbPower2"
		Me.tbPower2.Size = New System.Drawing.Size(38, 22)
		Me.tbPower2.TabIndex = 9
		Me.tbPower2.Text = "30.00"
		' 
		' label12
		' 
		Me.label12.AutoSize = True
		Me.label12.Location = New System.Drawing.Point(83, 31)
		Me.label12.Name = "label12"
		Me.label12.Size = New System.Drawing.Size(66, 12)
		Me.label12.TabIndex = 13
		Me.label12.Text = "Power (dbm)"
		' 
		' label13
		' 
		Me.label13.AutoSize = True
		Me.label13.Location = New System.Drawing.Point(29, 31)
		Me.label13.Name = "label13"
		Me.label13.Size = New System.Drawing.Size(28, 12)
		Me.label13.TabIndex = 13
		Me.label13.Text = "Ant#"
		' 
		' chAnt4
		' 
		Me.chAnt4.AutoSize = True
		Me.chAnt4.Location = New System.Drawing.Point(24, 105)
		Me.chAnt4.Name = "chAnt4"
		Me.chAnt4.RightToLeft = System.Windows.Forms.RightToLeft.Yes
		Me.chAnt4.Size = New System.Drawing.Size(30, 16)
		Me.chAnt4.TabIndex = 12
		Me.chAnt4.Text = "4"
		Me.chAnt4.UseVisualStyleBackColor = True
		' 
		' chAnt1
		' 
		Me.chAnt1.AutoSize = True
		Me.chAnt1.Checked = True
		Me.chAnt1.CheckState = System.Windows.Forms.CheckState.Checked
		Me.chAnt1.Location = New System.Drawing.Point(24, 50)
		Me.chAnt1.Name = "chAnt1"
		Me.chAnt1.RightToLeft = System.Windows.Forms.RightToLeft.Yes
		Me.chAnt1.Size = New System.Drawing.Size(30, 16)
		Me.chAnt1.TabIndex = 11
		Me.chAnt1.Text = "1"
		Me.chAnt1.UseVisualStyleBackColor = True
		' 
		' chAnt3
		' 
		Me.chAnt3.AutoSize = True
		Me.chAnt3.Location = New System.Drawing.Point(24, 88)
		Me.chAnt3.Name = "chAnt3"
		Me.chAnt3.RightToLeft = System.Windows.Forms.RightToLeft.Yes
		Me.chAnt3.Size = New System.Drawing.Size(30, 16)
		Me.chAnt3.TabIndex = 12
		Me.chAnt3.Text = "3"
		Me.chAnt3.UseVisualStyleBackColor = True
		' 
		' chAnt2
		' 
		Me.chAnt2.AutoSize = True
		Me.chAnt2.Location = New System.Drawing.Point(24, 70)
		Me.chAnt2.Name = "chAnt2"
		Me.chAnt2.RightToLeft = System.Windows.Forms.RightToLeft.Yes
		Me.chAnt2.Size = New System.Drawing.Size(30, 16)
		Me.chAnt2.TabIndex = 12
		Me.chAnt2.Text = "2"
		Me.chAnt2.UseVisualStyleBackColor = True
		' 
		' tpReader
		' 
		Me.tpReader.Controls.Add(Me.cbFrequencyList)
		Me.tpReader.Controls.Add(Me.cbRegion)
		Me.tpReader.Controls.Add(Me.cbFrequencyMode)
		Me.tpReader.Controls.Add(Me.tbReaderName)
		Me.tpReader.Controls.Add(Me.label1)
		Me.tpReader.Controls.Add(Me.label6)
		Me.tpReader.Controls.Add(Me.label2)
		Me.tpReader.Controls.Add(Me.label5)
		Me.tpReader.Location = New System.Drawing.Point(4, 21)
		Me.tpReader.Name = "tpReader"
		Me.tpReader.Padding = New System.Windows.Forms.Padding(3)
		Me.tpReader.Size = New System.Drawing.Size(381, 162)
		Me.tpReader.TabIndex = 0
		Me.tpReader.Text = "Reader Information"
		Me.tpReader.UseVisualStyleBackColor = True
		' 
		' cbFrequencyList
		' 
		Me.cbFrequencyList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
		Me.cbFrequencyList.Enabled = False
		Me.cbFrequencyList.FormattingEnabled = True
		Me.cbFrequencyList.Location = New System.Drawing.Point(85, 95)
		Me.cbFrequencyList.MaxDropDownItems = 13
		Me.cbFrequencyList.Name = "cbFrequencyList"
		Me.cbFrequencyList.Size = New System.Drawing.Size(196, 20)
		Me.cbFrequencyList.TabIndex = 2
		' 
		' cbRegion
		' 
		Me.cbRegion.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
		Me.cbRegion.FormattingEnabled = True
		Me.cbRegion.Items.AddRange(New Object() {"0 - US, North America, FCC Part 15.247", "1 - ETSI EN 300-220", "2 - ETSI EN 302-208 (With LBT)", "3 - Hong Kong 920-925 MHz", "4 - Taiwan 922-928 MHz", "5 - Japan 952-954 MHz", _
			"6 - Japan 952-955 MHz, 10mW", "7 - ETSI EN 302-208 (Without LBT)", "8 - Korea 910-914 MHz", "9 - Malaysia 919-923 MHz", "10 - China 920-925 MHz"})
		Me.cbRegion.Location = New System.Drawing.Point(85, 45)
		Me.cbRegion.Name = "cbRegion"
		Me.cbRegion.Size = New System.Drawing.Size(252, 20)
		Me.cbRegion.TabIndex = 2
		AddHandler Me.cbRegion.SelectedIndexChanged, New System.EventHandler(AddressOf Me.cbRegion_SelectedIndexChanged)
		' 
		' cbFrequencyMode
		' 
		Me.cbFrequencyMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
		Me.cbFrequencyMode.Enabled = False
		Me.cbFrequencyMode.FormattingEnabled = True
		Me.cbFrequencyMode.Items.AddRange(New Object() {"0 - Use Center Frequency", "1 - Frequency by Region (Automatic)"})
		Me.cbFrequencyMode.Location = New System.Drawing.Point(85, 70)
		Me.cbFrequencyMode.Name = "cbFrequencyMode"
		Me.cbFrequencyMode.Size = New System.Drawing.Size(196, 20)
		Me.cbFrequencyMode.TabIndex = 2
		AddHandler Me.cbFrequencyMode.SelectedIndexChanged, New System.EventHandler(AddressOf Me.cbFrequencyMode_SelectedIndexChanged)
		' 
		' tbReaderName
		' 
		Me.tbReaderName.Location = New System.Drawing.Point(85, 21)
		Me.tbReaderName.Name = "tbReaderName"
		Me.tbReaderName.Size = New System.Drawing.Size(252, 22)
		Me.tbReaderName.TabIndex = 0
		Me.tbReaderName.Text = "192.168.25.200"
		' 
		' label1
		' 
		Me.label1.AutoSize = True
		Me.label1.Location = New System.Drawing.Point(47, 24)
		Me.label1.Name = "label1"
		Me.label1.Size = New System.Drawing.Size(32, 12)
		Me.label1.TabIndex = 1
		Me.label1.Text = "Name"
		' 
		' label6
		' 
		Me.label6.AutoSize = True
		Me.label6.Location = New System.Drawing.Point(32, 99)
		Me.label6.Name = "label6"
		Me.label6.Size = New System.Drawing.Size(49, 12)
		Me.label6.TabIndex = 1
		Me.label6.Text = "Freq. List"
		' 
		' label2
		' 
		Me.label2.AutoSize = True
		Me.label2.Location = New System.Drawing.Point(41, 49)
		Me.label2.Name = "label2"
		Me.label2.Size = New System.Drawing.Size(39, 12)
		Me.label2.TabIndex = 1
		Me.label2.Text = "Region"
		' 
		' label5
		' 
		Me.label5.AutoSize = True
		Me.label5.Location = New System.Drawing.Point(22, 74)
		Me.label5.Name = "label5"
		Me.label5.Size = New System.Drawing.Size(59, 12)
		Me.label5.TabIndex = 1
		Me.label5.Text = "Freq. Mode"
		' 
		' tabControl1
		' 
		Me.tabControl1.Controls.Add(Me.tpReader)
		Me.tabControl1.Controls.Add(Me.tpAntenna)
		Me.tabControl1.Controls.Add(Me.tpGen2)
		Me.tabControl1.Controls.Add(Me.tabPage1)
		Me.tabControl1.Location = New System.Drawing.Point(3, 3)
		Me.tabControl1.Name = "tabControl1"
		Me.tabControl1.SelectedIndex = 0
		Me.tabControl1.Size = New System.Drawing.Size(389, 187)
		Me.tabControl1.TabIndex = 19
		' 
		' tabPage1
		' 
		Me.tabPage1.Controls.Add(Me.llCSL)
		Me.tabPage1.Controls.Add(Me.pictureBox2)
		Me.tabPage1.Controls.Add(Me.label11)
		Me.tabPage1.Controls.Add(Me.lbFirmware)
		Me.tabPage1.Controls.Add(Me.lbSoftware)
		Me.tabPage1.Controls.Add(Me.label8)
		Me.tabPage1.Location = New System.Drawing.Point(4, 21)
		Me.tabPage1.Name = "tabPage1"
		Me.tabPage1.Size = New System.Drawing.Size(381, 160)
		Me.tabPage1.TabIndex = 3
		Me.tabPage1.Text = "About Solution"
		Me.tabPage1.UseVisualStyleBackColor = True
		' 
		' llCSL
		' 
		Me.llCSL.AutoSize = True
		Me.llCSL.Font = New System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CByte(0))
		Me.llCSL.Location = New System.Drawing.Point(69, 7)
		Me.llCSL.Name = "llCSL"
		Me.llCSL.Size = New System.Drawing.Size(84, 19)
		Me.llCSL.TabIndex = 23
		Me.llCSL.TabStop = True
		Me.llCSL.Text = "CSL RFID"
		' 
		' pictureBox2
		' 
		Me.pictureBox2.Image = DirectCast(resources.GetObject("pictureBox2.Image"), System.Drawing.Image)
		Me.pictureBox2.Location = New System.Drawing.Point(20, 28)
		Me.pictureBox2.Name = "pictureBox2"
		Me.pictureBox2.Size = New System.Drawing.Size(200, 69)
		Me.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
		Me.pictureBox2.TabIndex = 22
		Me.pictureBox2.TabStop = False
		' 
		' label11
		' 
		Me.label11.AutoSize = True
		Me.label11.Location = New System.Drawing.Point(248, 126)
		Me.label11.Name = "label11"
		Me.label11.Size = New System.Drawing.Size(52, 12)
		Me.label11.TabIndex = 21
		Me.label11.Text = "Firmware:"
		' 
		' lbFirmware
		' 
		Me.lbFirmware.AutoSize = True
		Me.lbFirmware.Location = New System.Drawing.Point(301, 127)
		Me.lbFirmware.Name = "lbFirmware"
		Me.lbFirmware.Size = New System.Drawing.Size(17, 12)
		Me.lbFirmware.TabIndex = 21
		Me.lbFirmware.Text = "    "
		' 
		' lbSoftware
		' 
		Me.lbSoftware.AutoSize = True
		Me.lbSoftware.Location = New System.Drawing.Point(301, 113)
		Me.lbSoftware.Name = "lbSoftware"
		Me.lbSoftware.Size = New System.Drawing.Size(20, 12)
		Me.lbSoftware.TabIndex = 21
		Me.lbSoftware.Text = "     "
		' 
		' label8
		' 
		Me.label8.AutoSize = True
		Me.label8.Location = New System.Drawing.Point(248, 112)
		Me.label8.Name = "label8"
		Me.label8.Size = New System.Drawing.Size(49, 12)
		Me.label8.TabIndex = 21
		Me.label8.Text = "Software:"
		' 
		' label7
		' 
		Me.label7.AutoSize = True
		Me.label7.Location = New System.Drawing.Point(65, 124)
		Me.label7.Name = "label7"
		Me.label7.RightToLeft = System.Windows.Forms.RightToLeft.Yes
		Me.label7.Size = New System.Drawing.Size(76, 12)
		Me.label7.TabIndex = 3
		Me.label7.Text = "Tag Population"
		Me.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight
		' 
		' tbPopulation
		' 
		Me.tbPopulation.Location = New System.Drawing.Point(154, 121)
		Me.tbPopulation.MaxLength = 5
		Me.tbPopulation.Name = "tbPopulation"
		Me.tbPopulation.Size = New System.Drawing.Size(48, 22)
		Me.tbPopulation.TabIndex = 10
		Me.tbPopulation.Text = "32"
		' 
		' CSLReaderParams
		' 
		Me.AutoScaleDimensions = New System.Drawing.SizeF(6F, 12F)
		Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
		Me.Controls.Add(Me.tabControl1)
		Me.Name = "CSLReaderParams"
		Me.Size = New System.Drawing.Size(395, 190)
		AddHandler Me.Load, New System.EventHandler(AddressOf Me.CSLReaderParams_Load)
		Me.tpGen2.ResumeLayout(False)
		Me.tpGen2.PerformLayout()
		Me.tpAntenna.ResumeLayout(False)
		Me.tpAntenna.PerformLayout()
		Me.tpReader.ResumeLayout(False)
		Me.tpReader.PerformLayout()
		Me.tabControl1.ResumeLayout(False)
		Me.tabPage1.ResumeLayout(False)
		Me.tabPage1.PerformLayout()
		DirectCast(Me.pictureBox2, System.ComponentModel.ISupportInitialize).EndInit()
		Me.ResumeLayout(False)

	End Sub

	#End Region

	Private tpGen2 As System.Windows.Forms.TabPage
	Private cbSession As System.Windows.Forms.ComboBox
	Private label16 As System.Windows.Forms.Label
	Private label4 As System.Windows.Forms.Label
	Private cbAutoSetMode As System.Windows.Forms.ComboBox
	Private cbInventoryMode As System.Windows.Forms.ComboBox
	Private cbReaderMode As System.Windows.Forms.ComboBox
	Private label15 As System.Windows.Forms.Label
	Private label3 As System.Windows.Forms.Label
	Private tpAntenna As System.Windows.Forms.TabPage
	Private label9 As System.Windows.Forms.Label
	Private tbPower1 As System.Windows.Forms.TextBox
	Private tbPower3 As System.Windows.Forms.TextBox
	Private tbPower4 As System.Windows.Forms.TextBox
	Private tbPower2 As System.Windows.Forms.TextBox
	Private label12 As System.Windows.Forms.Label
	Private label13 As System.Windows.Forms.Label
	Private chAnt4 As System.Windows.Forms.CheckBox
	Private chAnt1 As System.Windows.Forms.CheckBox
	Private chAnt3 As System.Windows.Forms.CheckBox
	Private chAnt2 As System.Windows.Forms.CheckBox
	Private tpReader As System.Windows.Forms.TabPage
	Private cbFrequencyList As System.Windows.Forms.ComboBox
	Private cbRegion As System.Windows.Forms.ComboBox
	Private cbFrequencyMode As System.Windows.Forms.ComboBox
	Private tbReaderName As System.Windows.Forms.TextBox
	Private label1 As System.Windows.Forms.Label
	Private label6 As System.Windows.Forms.Label
	Private label2 As System.Windows.Forms.Label
	Private label5 As System.Windows.Forms.Label
	Private tabControl1 As System.Windows.Forms.TabControl
	Private tabPage1 As System.Windows.Forms.TabPage
	Private llCSL As System.Windows.Forms.LinkLabel
	Private pictureBox2 As System.Windows.Forms.PictureBox
	Private label11 As System.Windows.Forms.Label
	Private lbFirmware As System.Windows.Forms.Label
	Private lbSoftware As System.Windows.Forms.Label
	Private label8 As System.Windows.Forms.Label
	Private tbPopulation As System.Windows.Forms.TextBox
	Private label7 As System.Windows.Forms.Label


End Class
