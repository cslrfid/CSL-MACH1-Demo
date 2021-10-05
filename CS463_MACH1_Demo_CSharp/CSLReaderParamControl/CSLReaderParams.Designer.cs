namespace CSLReaderParamControl
{
    partial class CSLReaderParams
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CSLReaderParams));
            this.tpGen2 = new System.Windows.Forms.TabPage();
            this.cbSession = new System.Windows.Forms.ComboBox();
            this.label16 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.cbAutoSetMode = new System.Windows.Forms.ComboBox();
            this.cbInventoryMode = new System.Windows.Forms.ComboBox();
            this.cbReaderMode = new System.Windows.Forms.ComboBox();
            this.label15 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.tpAntenna = new System.Windows.Forms.TabPage();
            this.label9 = new System.Windows.Forms.Label();
            this.tbPower1 = new System.Windows.Forms.TextBox();
            this.tbPower3 = new System.Windows.Forms.TextBox();
            this.tbPower4 = new System.Windows.Forms.TextBox();
            this.tbPower2 = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.chAnt4 = new System.Windows.Forms.CheckBox();
            this.chAnt1 = new System.Windows.Forms.CheckBox();
            this.chAnt3 = new System.Windows.Forms.CheckBox();
            this.chAnt2 = new System.Windows.Forms.CheckBox();
            this.tpReader = new System.Windows.Forms.TabPage();
            this.cbFrequencyList = new System.Windows.Forms.ComboBox();
            this.cbRegion = new System.Windows.Forms.ComboBox();
            this.cbFrequencyMode = new System.Windows.Forms.ComboBox();
            this.tbReaderName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.llCSL = new System.Windows.Forms.LinkLabel();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.label11 = new System.Windows.Forms.Label();
            this.lbFirmware = new System.Windows.Forms.Label();
            this.lbSoftware = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.tbPopulation = new System.Windows.Forms.TextBox();
            this.tpGen2.SuspendLayout();
            this.tpAntenna.SuspendLayout();
            this.tpReader.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // tpGen2
            // 
            this.tpGen2.Controls.Add(this.tbPopulation);
            this.tpGen2.Controls.Add(this.label7);
            this.tpGen2.Controls.Add(this.cbSession);
            this.tpGen2.Controls.Add(this.label16);
            this.tpGen2.Controls.Add(this.label4);
            this.tpGen2.Controls.Add(this.cbAutoSetMode);
            this.tpGen2.Controls.Add(this.cbInventoryMode);
            this.tpGen2.Controls.Add(this.cbReaderMode);
            this.tpGen2.Controls.Add(this.label15);
            this.tpGen2.Controls.Add(this.label3);
            this.tpGen2.Location = new System.Drawing.Point(4, 21);
            this.tpGen2.Name = "tpGen2";
            this.tpGen2.Size = new System.Drawing.Size(381, 162);
            this.tpGen2.TabIndex = 2;
            this.tpGen2.Text = "Gen2 Parameters";
            this.tpGen2.UseVisualStyleBackColor = true;
            // 
            // cbSession
            // 
            this.cbSession.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbSession.FormattingEnabled = true;
            this.cbSession.Items.AddRange(new object[] {
            "Session 0",
            "Session 1",
            "Session 2",
            "Session 3"});
            this.cbSession.Location = new System.Drawing.Point(154, 14);
            this.cbSession.Name = "cbSession";
            this.cbSession.Size = new System.Drawing.Size(137, 20);
            this.cbSession.TabIndex = 2;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(28, 96);
            this.label16.Name = "label16";
            this.label16.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.label16.Size = new System.Drawing.Size(115, 12);
            this.label16.TabIndex = 1;
            this.label16.Text = "Inventory Search Mode";
            this.label16.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(74, 71);
            this.label4.Name = "label4";
            this.label4.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.label4.Size = new System.Drawing.Size(67, 12);
            this.label4.TabIndex = 1;
            this.label4.Text = "Mode ID List";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cbAutoSetMode
            // 
            this.cbAutoSetMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbAutoSetMode.FormattingEnabled = true;
            this.cbAutoSetMode.Items.AddRange(new object[] {
            "2 - By Mode ID"});
            this.cbAutoSetMode.Location = new System.Drawing.Point(154, 41);
            this.cbAutoSetMode.Name = "cbAutoSetMode";
            this.cbAutoSetMode.Size = new System.Drawing.Size(137, 20);
            this.cbAutoSetMode.TabIndex = 2;
            this.cbAutoSetMode.SelectedIndexChanged += new System.EventHandler(this.cbAutoSetMode_SelectedIndexChanged);
            // 
            // cbInventoryMode
            // 
            this.cbInventoryMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbInventoryMode.FormattingEnabled = true;
            this.cbInventoryMode.Items.AddRange(new object[] {
            "0 - Dual Target",
            "1 - Single Target (A)"});
            this.cbInventoryMode.Location = new System.Drawing.Point(154, 93);
            this.cbInventoryMode.Name = "cbInventoryMode";
            this.cbInventoryMode.Size = new System.Drawing.Size(137, 20);
            this.cbInventoryMode.TabIndex = 2;
            // 
            // cbReaderMode
            // 
            this.cbReaderMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbReaderMode.FormattingEnabled = true;
            this.cbReaderMode.Items.AddRange(new object[] {
            "0",
            "1",
            "2",
            "3"});
            this.cbReaderMode.Location = new System.Drawing.Point(154, 68);
            this.cbReaderMode.Name = "cbReaderMode";
            this.cbReaderMode.Size = new System.Drawing.Size(137, 20);
            this.cbReaderMode.TabIndex = 2;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(68, 44);
            this.label15.Name = "label15";
            this.label15.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.label15.Size = new System.Drawing.Size(75, 12);
            this.label15.TabIndex = 1;
            this.label15.Text = "Auto Set Mode";
            this.label15.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(102, 17);
            this.label3.Name = "label3";
            this.label3.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.label3.Size = new System.Drawing.Size(39, 12);
            this.label3.TabIndex = 1;
            this.label3.Text = "Session";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // tpAntenna
            // 
            this.tpAntenna.Controls.Add(this.label9);
            this.tpAntenna.Controls.Add(this.tbPower1);
            this.tpAntenna.Controls.Add(this.tbPower3);
            this.tpAntenna.Controls.Add(this.tbPower4);
            this.tpAntenna.Controls.Add(this.tbPower2);
            this.tpAntenna.Controls.Add(this.label12);
            this.tpAntenna.Controls.Add(this.label13);
            this.tpAntenna.Controls.Add(this.chAnt4);
            this.tpAntenna.Controls.Add(this.chAnt1);
            this.tpAntenna.Controls.Add(this.chAnt3);
            this.tpAntenna.Controls.Add(this.chAnt2);
            this.tpAntenna.Location = new System.Drawing.Point(4, 21);
            this.tpAntenna.Name = "tpAntenna";
            this.tpAntenna.Padding = new System.Windows.Forms.Padding(3);
            this.tpAntenna.Size = new System.Drawing.Size(381, 162);
            this.tpAntenna.TabIndex = 1;
            this.tpAntenna.Text = "Antenna Setting";
            this.tpAntenna.UseVisualStyleBackColor = true;
            this.tpAntenna.Click += new System.EventHandler(this.tpAntenna_Click);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(137, 19);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(0, 12);
            this.label9.TabIndex = 13;
            // 
            // tbPower1
            // 
            this.tbPower1.Location = new System.Drawing.Point(86, 47);
            this.tbPower1.Name = "tbPower1";
            this.tbPower1.Size = new System.Drawing.Size(38, 22);
            this.tbPower1.TabIndex = 9;
            this.tbPower1.Text = "30.00";
            // 
            // tbPower3
            // 
            this.tbPower3.Location = new System.Drawing.Point(86, 84);
            this.tbPower3.Name = "tbPower3";
            this.tbPower3.Size = new System.Drawing.Size(38, 22);
            this.tbPower3.TabIndex = 9;
            this.tbPower3.Text = "30.00";
            // 
            // tbPower4
            // 
            this.tbPower4.Location = new System.Drawing.Point(86, 102);
            this.tbPower4.Name = "tbPower4";
            this.tbPower4.Size = new System.Drawing.Size(38, 22);
            this.tbPower4.TabIndex = 9;
            this.tbPower4.Text = "30.00";
            // 
            // tbPower2
            // 
            this.tbPower2.Location = new System.Drawing.Point(86, 66);
            this.tbPower2.Name = "tbPower2";
            this.tbPower2.Size = new System.Drawing.Size(38, 22);
            this.tbPower2.TabIndex = 9;
            this.tbPower2.Text = "30.00";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(83, 31);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(66, 12);
            this.label12.TabIndex = 13;
            this.label12.Text = "Power (dbm)";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(29, 31);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(28, 12);
            this.label13.TabIndex = 13;
            this.label13.Text = "Ant#";
            // 
            // chAnt4
            // 
            this.chAnt4.AutoSize = true;
            this.chAnt4.Location = new System.Drawing.Point(24, 105);
            this.chAnt4.Name = "chAnt4";
            this.chAnt4.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.chAnt4.Size = new System.Drawing.Size(30, 16);
            this.chAnt4.TabIndex = 12;
            this.chAnt4.Text = "4";
            this.chAnt4.UseVisualStyleBackColor = true;
            // 
            // chAnt1
            // 
            this.chAnt1.AutoSize = true;
            this.chAnt1.Checked = true;
            this.chAnt1.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chAnt1.Location = new System.Drawing.Point(24, 50);
            this.chAnt1.Name = "chAnt1";
            this.chAnt1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.chAnt1.Size = new System.Drawing.Size(30, 16);
            this.chAnt1.TabIndex = 11;
            this.chAnt1.Text = "1";
            this.chAnt1.UseVisualStyleBackColor = true;
            // 
            // chAnt3
            // 
            this.chAnt3.AutoSize = true;
            this.chAnt3.Location = new System.Drawing.Point(24, 88);
            this.chAnt3.Name = "chAnt3";
            this.chAnt3.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.chAnt3.Size = new System.Drawing.Size(30, 16);
            this.chAnt3.TabIndex = 12;
            this.chAnt3.Text = "3";
            this.chAnt3.UseVisualStyleBackColor = true;
            // 
            // chAnt2
            // 
            this.chAnt2.AutoSize = true;
            this.chAnt2.Location = new System.Drawing.Point(24, 70);
            this.chAnt2.Name = "chAnt2";
            this.chAnt2.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.chAnt2.Size = new System.Drawing.Size(30, 16);
            this.chAnt2.TabIndex = 12;
            this.chAnt2.Text = "2";
            this.chAnt2.UseVisualStyleBackColor = true;
            // 
            // tpReader
            // 
            this.tpReader.Controls.Add(this.cbFrequencyList);
            this.tpReader.Controls.Add(this.cbRegion);
            this.tpReader.Controls.Add(this.cbFrequencyMode);
            this.tpReader.Controls.Add(this.tbReaderName);
            this.tpReader.Controls.Add(this.label1);
            this.tpReader.Controls.Add(this.label6);
            this.tpReader.Controls.Add(this.label2);
            this.tpReader.Controls.Add(this.label5);
            this.tpReader.Location = new System.Drawing.Point(4, 21);
            this.tpReader.Name = "tpReader";
            this.tpReader.Padding = new System.Windows.Forms.Padding(3);
            this.tpReader.Size = new System.Drawing.Size(381, 162);
            this.tpReader.TabIndex = 0;
            this.tpReader.Text = "Reader Information";
            this.tpReader.UseVisualStyleBackColor = true;
            // 
            // cbFrequencyList
            // 
            this.cbFrequencyList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbFrequencyList.Enabled = false;
            this.cbFrequencyList.FormattingEnabled = true;
            this.cbFrequencyList.Location = new System.Drawing.Point(85, 95);
            this.cbFrequencyList.MaxDropDownItems = 13;
            this.cbFrequencyList.Name = "cbFrequencyList";
            this.cbFrequencyList.Size = new System.Drawing.Size(196, 20);
            this.cbFrequencyList.TabIndex = 2;
            // 
            // cbRegion
            // 
            this.cbRegion.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbRegion.FormattingEnabled = true;
            this.cbRegion.Items.AddRange(new object[] {
            "0 - US, North America, FCC Part 15.247",
            "1 - ETSI EN 300-220",
            "2 - ETSI EN 302-208 (With LBT)",
            "3 - Hong Kong 920-925 MHz",
            "4 - Taiwan 922-928 MHz",
            "5 - Japan 952-954 MHz",
            "6 - Japan 952-955 MHz, 10mW",
            "7 - ETSI EN 302-208 (Without LBT)",
            "8 - Korea 910-914 MHz",
            "9 - Malaysia 919-923 MHz",
            "10 - China 920-925 MHz"});
            this.cbRegion.Location = new System.Drawing.Point(85, 45);
            this.cbRegion.Name = "cbRegion";
            this.cbRegion.Size = new System.Drawing.Size(252, 20);
            this.cbRegion.TabIndex = 2;
            this.cbRegion.SelectedIndexChanged += new System.EventHandler(this.cbRegion_SelectedIndexChanged);
            // 
            // cbFrequencyMode
            // 
            this.cbFrequencyMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbFrequencyMode.Enabled = false;
            this.cbFrequencyMode.FormattingEnabled = true;
            this.cbFrequencyMode.Items.AddRange(new object[] {
            "0 - Use Center Frequency",
            "1 - Frequency by Region (Automatic)"});
            this.cbFrequencyMode.Location = new System.Drawing.Point(85, 70);
            this.cbFrequencyMode.Name = "cbFrequencyMode";
            this.cbFrequencyMode.Size = new System.Drawing.Size(196, 20);
            this.cbFrequencyMode.TabIndex = 2;
            this.cbFrequencyMode.SelectedIndexChanged += new System.EventHandler(this.cbFrequencyMode_SelectedIndexChanged);
            // 
            // tbReaderName
            // 
            this.tbReaderName.Location = new System.Drawing.Point(85, 21);
            this.tbReaderName.Name = "tbReaderName";
            this.tbReaderName.Size = new System.Drawing.Size(252, 22);
            this.tbReaderName.TabIndex = 0;
            this.tbReaderName.Text = "192.168.25.200";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(47, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(32, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "Name";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(32, 99);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(49, 12);
            this.label6.TabIndex = 1;
            this.label6.Text = "Freq. List";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(41, 49);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(39, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "Region";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(22, 74);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(59, 12);
            this.label5.TabIndex = 1;
            this.label5.Text = "Freq. Mode";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tpReader);
            this.tabControl1.Controls.Add(this.tpAntenna);
            this.tabControl1.Controls.Add(this.tpGen2);
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Location = new System.Drawing.Point(3, 3);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(389, 187);
            this.tabControl1.TabIndex = 19;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.llCSL);
            this.tabPage1.Controls.Add(this.pictureBox2);
            this.tabPage1.Controls.Add(this.label11);
            this.tabPage1.Controls.Add(this.lbFirmware);
            this.tabPage1.Controls.Add(this.lbSoftware);
            this.tabPage1.Controls.Add(this.label8);
            this.tabPage1.Location = new System.Drawing.Point(4, 21);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Size = new System.Drawing.Size(381, 160);
            this.tabPage1.TabIndex = 3;
            this.tabPage1.Text = "About Solution";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // llCSL
            // 
            this.llCSL.AutoSize = true;
            this.llCSL.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.llCSL.Location = new System.Drawing.Point(69, 7);
            this.llCSL.Name = "llCSL";
            this.llCSL.Size = new System.Drawing.Size(84, 19);
            this.llCSL.TabIndex = 23;
            this.llCSL.TabStop = true;
            this.llCSL.Text = "CSL RFID";
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox2.Image")));
            this.pictureBox2.Location = new System.Drawing.Point(20, 28);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(200, 69);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox2.TabIndex = 22;
            this.pictureBox2.TabStop = false;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(248, 126);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(52, 12);
            this.label11.TabIndex = 21;
            this.label11.Text = "Firmware:";
            // 
            // lbFirmware
            // 
            this.lbFirmware.AutoSize = true;
            this.lbFirmware.Location = new System.Drawing.Point(301, 127);
            this.lbFirmware.Name = "lbFirmware";
            this.lbFirmware.Size = new System.Drawing.Size(17, 12);
            this.lbFirmware.TabIndex = 21;
            this.lbFirmware.Text = "    ";
            // 
            // lbSoftware
            // 
            this.lbSoftware.AutoSize = true;
            this.lbSoftware.Location = new System.Drawing.Point(301, 113);
            this.lbSoftware.Name = "lbSoftware";
            this.lbSoftware.Size = new System.Drawing.Size(20, 12);
            this.lbSoftware.TabIndex = 21;
            this.lbSoftware.Text = "     ";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(248, 112);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(49, 12);
            this.label8.TabIndex = 21;
            this.label8.Text = "Software:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(65, 124);
            this.label7.Name = "label7";
            this.label7.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.label7.Size = new System.Drawing.Size(76, 12);
            this.label7.TabIndex = 3;
            this.label7.Text = "Tag Population";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // tbPopulation
            // 
            this.tbPopulation.Location = new System.Drawing.Point(154, 121);
            this.tbPopulation.MaxLength = 5;
            this.tbPopulation.Name = "tbPopulation";
            this.tbPopulation.Size = new System.Drawing.Size(48, 22);
            this.tbPopulation.TabIndex = 10;
            this.tbPopulation.Text = "32";
            // 
            // CSLReaderParams
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tabControl1);
            this.Name = "CSLReaderParams";
            this.Size = new System.Drawing.Size(395, 190);
            this.Load += new System.EventHandler(this.CSLReaderParams_Load);
            this.tpGen2.ResumeLayout(false);
            this.tpGen2.PerformLayout();
            this.tpAntenna.ResumeLayout(false);
            this.tpAntenna.PerformLayout();
            this.tpReader.ResumeLayout(false);
            this.tpReader.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabPage tpGen2;
        private System.Windows.Forms.ComboBox cbSession;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cbAutoSetMode;
        private System.Windows.Forms.ComboBox cbInventoryMode;
        private System.Windows.Forms.ComboBox cbReaderMode;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TabPage tpAntenna;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox tbPower1;
        private System.Windows.Forms.TextBox tbPower3;
        private System.Windows.Forms.TextBox tbPower4;
        private System.Windows.Forms.TextBox tbPower2;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.CheckBox chAnt4;
        private System.Windows.Forms.CheckBox chAnt1;
        private System.Windows.Forms.CheckBox chAnt3;
        private System.Windows.Forms.CheckBox chAnt2;
        private System.Windows.Forms.TabPage tpReader;
        private System.Windows.Forms.ComboBox cbFrequencyList;
        private System.Windows.Forms.ComboBox cbRegion;
        private System.Windows.Forms.ComboBox cbFrequencyMode;
        private System.Windows.Forms.TextBox tbReaderName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.LinkLabel llCSL;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label lbFirmware;
        private System.Windows.Forms.Label lbSoftware;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox tbPopulation;
        private System.Windows.Forms.Label label7;


    }
}
