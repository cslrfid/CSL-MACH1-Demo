namespace CS463Mach1Samples
{
    partial class MainForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnConnect = new System.Windows.Forms.Button();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.btnTest = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.cbUmLen = new System.Windows.Forms.ComboBox();
            this.cbEpcLen = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtUM2 = new System.Windows.Forms.TextBox();
            this.txtUM1 = new System.Windows.Forms.TextBox();
            this.txtEPC = new System.Windows.Forms.TextBox();
            this.txtPC = new System.Windows.Forms.TextBox();
            this.chkUM = new System.Windows.Forms.CheckBox();
            this.chkEPC = new System.Windows.Forms.CheckBox();
            this.chkPC = new System.Windows.Forms.CheckBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.cbMBLen = new System.Windows.Forms.ComboBox();
            this.cbMB = new System.Windows.Forms.ComboBox();
            this.chkMB = new System.Windows.Forms.CheckBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.cbMB3Len = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.cbMB2Len = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.cbMB0Len = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.btnClearTag = new System.Windows.Forms.Button();
            this.btnClearStatus = new System.Windows.Forms.Button();
            this.lv_tag = new System.Windows.Forms.ListView();
            this.col_index = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.col_epc = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.col_rssi = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.col_count = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.col_data = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tbStatus = new System.Windows.Forms.TextBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.label7 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.lb_rate = new System.Windows.Forms.Label();
            this.timer_rate = new System.Windows.Forms.Timer(this.components);
            this.statusStrip.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip.Location = new System.Drawing.Point(0, 656);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(1016, 22);
            this.statusStrip.TabIndex = 0;
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.AutoSize = false;
            this.toolStripStatusLabel1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(400, 17);
            this.toolStripStatusLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnConnect);
            this.groupBox1.Controls.Add(this.btnUpdate);
            this.groupBox1.Location = new System.Drawing.Point(13, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(264, 53);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Reader";
            // 
            // btnConnect
            // 
            this.btnConnect.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnConnect.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnConnect.Location = new System.Drawing.Point(12, 16);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(114, 26);
            this.btnConnect.TabIndex = 0;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // btnUpdate
            // 
            this.btnUpdate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUpdate.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnUpdate.Location = new System.Drawing.Point(141, 16);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(112, 26);
            this.btnUpdate.TabIndex = 0;
            this.btnUpdate.Text = "Update";
            this.btnUpdate.UseVisualStyleBackColor = true;
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.comboBox1);
            this.groupBox2.Controls.Add(this.btnTest);
            this.groupBox2.Controls.Add(this.btnStop);
            this.groupBox2.Location = new System.Drawing.Point(283, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(311, 53);
            this.groupBox2.TabIndex = 5;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Commands";
            // 
            // comboBox1
            // 
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "Inventory",
            "TagRead",
            "TagWrite",
            "SetGPO",
            "GetGPI"});
            this.comboBox1.Location = new System.Drawing.Point(10, 20);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(105, 21);
            this.comboBox1.TabIndex = 1;
            // 
            // btnTest
            // 
            this.btnTest.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTest.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnTest.Location = new System.Drawing.Point(121, 14);
            this.btnTest.Name = "btnTest";
            this.btnTest.Size = new System.Drawing.Size(84, 28);
            this.btnTest.TabIndex = 0;
            this.btnTest.Text = "Start";
            this.btnTest.UseVisualStyleBackColor = true;
            this.btnTest.Click += new System.EventHandler(this.btnTest_Click);
            // 
            // btnStop
            // 
            this.btnStop.Enabled = false;
            this.btnStop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStop.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnStop.Location = new System.Drawing.Point(216, 14);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(85, 28);
            this.btnStop.TabIndex = 0;
            this.btnStop.Text = "Stop";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.cbUmLen);
            this.groupBox3.Controls.Add(this.cbEpcLen);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Controls.Add(this.txtUM2);
            this.groupBox3.Controls.Add(this.txtUM1);
            this.groupBox3.Controls.Add(this.txtEPC);
            this.groupBox3.Controls.Add(this.txtPC);
            this.groupBox3.Controls.Add(this.chkUM);
            this.groupBox3.Controls.Add(this.chkEPC);
            this.groupBox3.Controls.Add(this.chkPC);
            this.groupBox3.Location = new System.Drawing.Point(13, 71);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(1000, 101);
            this.groupBox3.TabIndex = 7;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Write parameters";
            // 
            // cbUmLen
            // 
            this.cbUmLen.FormattingEnabled = true;
            this.cbUmLen.Location = new System.Drawing.Point(807, 58);
            this.cbUmLen.Name = "cbUmLen";
            this.cbUmLen.Size = new System.Drawing.Size(107, 20);
            this.cbUmLen.TabIndex = 11;
            this.cbUmLen.SelectedIndexChanged += new System.EventHandler(this.cbUmLen_SelectedIndexChanged);
            // 
            // cbEpcLen
            // 
            this.cbEpcLen.FormattingEnabled = true;
            this.cbEpcLen.Location = new System.Drawing.Point(807, 37);
            this.cbEpcLen.Name = "cbEpcLen";
            this.cbEpcLen.Size = new System.Drawing.Size(107, 20);
            this.cbEpcLen.TabIndex = 10;
            this.cbEpcLen.SelectedIndexChanged += new System.EventHandler(this.cbEpcLen_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(707, 61);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(93, 12);
            this.label3.TabIndex = 9;
            this.label3.Text = "Word to be written";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(707, 40);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(93, 12);
            this.label2.TabIndex = 8;
            this.label2.Text = "Word to be written";
            // 
            // txtUM2
            // 
            this.txtUM2.Location = new System.Drawing.Point(141, 77);
            this.txtUM2.MaxLength = 64;
            this.txtUM2.Name = "txtUM2";
            this.txtUM2.Size = new System.Drawing.Size(560, 22);
            this.txtUM2.TabIndex = 6;
            this.txtUM2.Text = "8888888899999999AAAAAAAABBBBBBBBCCCCCCCCDDDDDDDDEEEEEEEEFFFFFFFF";
            this.txtUM2.Visible = false;
            this.txtUM2.TextChanged += new System.EventHandler(this.txtUM2_TextChanged);
            // 
            // txtUM1
            // 
            this.txtUM1.Location = new System.Drawing.Point(141, 58);
            this.txtUM1.MaxLength = 64;
            this.txtUM1.Name = "txtUM1";
            this.txtUM1.Size = new System.Drawing.Size(560, 22);
            this.txtUM1.TabIndex = 5;
            this.txtUM1.Text = "0123456789ABCDEF0123456789ABCDEF0123456789ABCDEF0123456789ABCDEF";
            this.txtUM1.TextChanged += new System.EventHandler(this.txtUM1_TextChanged);
            // 
            // txtEPC
            // 
            this.txtEPC.Location = new System.Drawing.Point(141, 37);
            this.txtEPC.MaxLength = 60;
            this.txtEPC.Name = "txtEPC";
            this.txtEPC.Size = new System.Drawing.Size(560, 22);
            this.txtEPC.TabIndex = 4;
            this.txtEPC.Text = "00112233445566778899AABB";
            this.txtEPC.TextChanged += new System.EventHandler(this.txtEPC_TextChanged);
            // 
            // txtPC
            // 
            this.txtPC.Location = new System.Drawing.Point(141, 16);
            this.txtPC.MaxLength = 4;
            this.txtPC.Name = "txtPC";
            this.txtPC.Size = new System.Drawing.Size(47, 22);
            this.txtPC.TabIndex = 3;
            this.txtPC.Text = "3000";
            this.txtPC.TextChanged += new System.EventHandler(this.txtPC_TextChanged);
            // 
            // chkUM
            // 
            this.chkUM.AutoSize = true;
            this.chkUM.Checked = true;
            this.chkUM.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkUM.Location = new System.Drawing.Point(12, 60);
            this.chkUM.Name = "chkUM";
            this.chkUM.Size = new System.Drawing.Size(132, 16);
            this.chkUM.TabIndex = 2;
            this.chkUM.Text = "User memory (256bits)";
            this.chkUM.UseVisualStyleBackColor = true;
            this.chkUM.CheckedChanged += new System.EventHandler(this.chkUM_CheckedChanged);
            // 
            // chkEPC
            // 
            this.chkEPC.AutoSize = true;
            this.chkEPC.Location = new System.Drawing.Point(12, 39);
            this.chkEPC.Name = "chkEPC";
            this.chkEPC.Size = new System.Drawing.Size(84, 16);
            this.chkEPC.TabIndex = 1;
            this.chkEPC.Text = "EPC (96bits)";
            this.chkEPC.UseVisualStyleBackColor = true;
            this.chkEPC.CheckedChanged += new System.EventHandler(this.chkEPC_CheckedChanged);
            // 
            // chkPC
            // 
            this.chkPC.AutoSize = true;
            this.chkPC.Location = new System.Drawing.Point(12, 18);
            this.chkPC.Name = "chkPC";
            this.chkPC.Size = new System.Drawing.Size(77, 16);
            this.chkPC.TabIndex = 0;
            this.chkPC.Text = "PC (16bits)";
            this.chkPC.UseVisualStyleBackColor = true;
            this.chkPC.CheckedChanged += new System.EventHandler(this.chkPC_CheckedChanged);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.cbMBLen);
            this.groupBox4.Controls.Add(this.cbMB);
            this.groupBox4.Controls.Add(this.chkMB);
            this.groupBox4.Location = new System.Drawing.Point(13, 178);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(445, 46);
            this.groupBox4.TabIndex = 9;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Inventory Options";
            // 
            // cbMBLen
            // 
            this.cbMBLen.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbMBLen.Enabled = false;
            this.cbMBLen.FormattingEnabled = true;
            this.cbMBLen.Location = new System.Drawing.Point(314, 16);
            this.cbMBLen.Name = "cbMBLen";
            this.cbMBLen.Size = new System.Drawing.Size(118, 20);
            this.cbMBLen.TabIndex = 4;
            // 
            // cbMB
            // 
            this.cbMB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbMB.Enabled = false;
            this.cbMB.FormattingEnabled = true;
            this.cbMB.Items.AddRange(new object[] {
            "MB0 - Reserved",
            "NONE",
            "MB2 - TID",
            "MB3 - User memory"});
            this.cbMB.Location = new System.Drawing.Point(187, 16);
            this.cbMB.Name = "cbMB";
            this.cbMB.Size = new System.Drawing.Size(121, 20);
            this.cbMB.TabIndex = 3;
            // 
            // chkMB
            // 
            this.chkMB.AutoSize = true;
            this.chkMB.Location = new System.Drawing.Point(12, 18);
            this.chkMB.Name = "chkMB";
            this.chkMB.Size = new System.Drawing.Size(171, 16);
            this.chkMB.TabIndex = 2;
            this.chkMB.Text = "Read Additional Memory Bank";
            this.chkMB.UseVisualStyleBackColor = true;
            this.chkMB.CheckedChanged += new System.EventHandler(this.chkMB_CheckedChanged);
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.cbMB3Len);
            this.groupBox5.Controls.Add(this.label6);
            this.groupBox5.Controls.Add(this.cbMB2Len);
            this.groupBox5.Controls.Add(this.label5);
            this.groupBox5.Controls.Add(this.cbMB0Len);
            this.groupBox5.Controls.Add(this.label4);
            this.groupBox5.Location = new System.Drawing.Point(464, 178);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(549, 46);
            this.groupBox5.TabIndex = 10;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Read Options";
            // 
            // cbMB3Len
            // 
            this.cbMB3Len.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbMB3Len.FormattingEnabled = true;
            this.cbMB3Len.Location = new System.Drawing.Point(332, 18);
            this.cbMB3Len.Name = "cbMB3Len";
            this.cbMB3Len.Size = new System.Drawing.Size(90, 20);
            this.cbMB3Len.TabIndex = 5;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(295, 19);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(29, 12);
            this.label6.TabIndex = 4;
            this.label6.Text = "MB3";
            // 
            // cbMB2Len
            // 
            this.cbMB2Len.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbMB2Len.FormattingEnabled = true;
            this.cbMB2Len.Location = new System.Drawing.Point(199, 18);
            this.cbMB2Len.Name = "cbMB2Len";
            this.cbMB2Len.Size = new System.Drawing.Size(90, 20);
            this.cbMB2Len.TabIndex = 3;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(162, 19);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(29, 12);
            this.label5.TabIndex = 2;
            this.label5.Text = "MB2";
            // 
            // cbMB0Len
            // 
            this.cbMB0Len.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbMB0Len.FormattingEnabled = true;
            this.cbMB0Len.Location = new System.Drawing.Point(56, 17);
            this.cbMB0Len.Name = "cbMB0Len";
            this.cbMB0Len.Size = new System.Drawing.Size(90, 20);
            this.cbMB0Len.TabIndex = 1;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(19, 18);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(29, 12);
            this.label4.TabIndex = 0;
            this.label4.Text = "MB0";
            // 
            // btnClearTag
            // 
            this.btnClearTag.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClearTag.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClearTag.Location = new System.Drawing.Point(920, 403);
            this.btnClearTag.Name = "btnClearTag";
            this.btnClearTag.Size = new System.Drawing.Size(84, 28);
            this.btnClearTag.TabIndex = 2;
            this.btnClearTag.Text = "Clear";
            this.btnClearTag.UseVisualStyleBackColor = true;
            this.btnClearTag.Click += new System.EventHandler(this.btnClearTag_Click);
            // 
            // btnClearStatus
            // 
            this.btnClearStatus.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClearStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClearStatus.Location = new System.Drawing.Point(920, 595);
            this.btnClearStatus.Name = "btnClearStatus";
            this.btnClearStatus.Size = new System.Drawing.Size(84, 28);
            this.btnClearStatus.TabIndex = 11;
            this.btnClearStatus.Text = "Clear";
            this.btnClearStatus.UseVisualStyleBackColor = true;
            this.btnClearStatus.Click += new System.EventHandler(this.btnClearStatus_Click);
            // 
            // lv_tag
            // 
            this.lv_tag.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.col_index,
            this.col_epc,
            this.col_rssi,
            this.col_count,
            this.col_data});
            this.lv_tag.GridLines = true;
            this.lv_tag.Location = new System.Drawing.Point(13, 230);
            this.lv_tag.Name = "lv_tag";
            this.lv_tag.Size = new System.Drawing.Size(901, 201);
            this.lv_tag.TabIndex = 0;
            this.lv_tag.UseCompatibleStateImageBehavior = false;
            this.lv_tag.View = System.Windows.Forms.View.Details;
            // 
            // col_index
            // 
            this.col_index.Text = "Index";
            this.col_index.Width = 50;
            // 
            // col_epc
            // 
            this.col_epc.Text = "EPC";
            this.col_epc.Width = 350;
            // 
            // col_rssi
            // 
            this.col_rssi.Text = "RSSI";
            this.col_rssi.Width = 50;
            // 
            // col_count
            // 
            this.col_count.Text = "Count";
            this.col_count.Width = 50;
            // 
            // col_data
            // 
            this.col_data.Text = "Data";
            this.col_data.Width = 380;
            // 
            // tbStatus
            // 
            this.tbStatus.Location = new System.Drawing.Point(12, 437);
            this.tbStatus.Multiline = true;
            this.tbStatus.Name = "tbStatus";
            this.tbStatus.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbStatus.Size = new System.Drawing.Size(902, 186);
            this.tbStatus.TabIndex = 12;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.label7);
            this.splitContainer1.Panel1.Controls.Add(this.label1);
            this.splitContainer1.Panel1.Controls.Add(this.lb_rate);
            this.splitContainer1.Panel1.Controls.Add(this.tbStatus);
            this.splitContainer1.Panel1.Controls.Add(this.lv_tag);
            this.splitContainer1.Panel1.Controls.Add(this.btnClearStatus);
            this.splitContainer1.Panel1.Controls.Add(this.btnClearTag);
            this.splitContainer1.Panel1.Controls.Add(this.groupBox5);
            this.splitContainer1.Panel1.Controls.Add(this.groupBox4);
            this.splitContainer1.Panel1.Controls.Add(this.groupBox3);
            this.splitContainer1.Panel1.Controls.Add(this.groupBox2);
            this.splitContainer1.Panel1.Controls.Add(this.groupBox1);
            this.splitContainer1.Size = new System.Drawing.Size(1016, 656);
            this.splitContainer1.SplitterDistance = 626;
            this.splitContainer1.TabIndex = 1;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(974, 375);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(30, 12);
            this.label7.TabIndex = 21;
            this.label7.Text = "tags/s";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(918, 375);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 12);
            this.label1.TabIndex = 14;
            this.label1.Text = "Rate:";
            // 
            // lb_rate
            // 
            this.lb_rate.AutoSize = true;
            this.lb_rate.Location = new System.Drawing.Point(947, 375);
            this.lb_rate.Name = "lb_rate";
            this.lb_rate.Size = new System.Drawing.Size(11, 12);
            this.lb_rate.TabIndex = 13;
            this.lb_rate.Text = "0";
            // 
            // timer_rate
            // 
            this.timer_rate.Enabled = true;
            this.timer_rate.Interval = 1000;
            this.timer_rate.Tick += new System.EventHandler(this.readrate_Tick);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1016, 678);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.statusStrip);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "CS463 MACH1 Demo";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Button btnUpdate;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Button btnTest;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.ComboBox cbUmLen;
        private System.Windows.Forms.ComboBox cbEpcLen;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtUM2;
        private System.Windows.Forms.TextBox txtUM1;
        private System.Windows.Forms.TextBox txtEPC;
        private System.Windows.Forms.TextBox txtPC;
        private System.Windows.Forms.CheckBox chkUM;
        private System.Windows.Forms.CheckBox chkEPC;
        private System.Windows.Forms.CheckBox chkPC;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.ComboBox cbMBLen;
        private System.Windows.Forms.ComboBox cbMB;
        private System.Windows.Forms.CheckBox chkMB;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.ComboBox cbMB3Len;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox cbMB2Len;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox cbMB0Len;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnClearTag;
        private System.Windows.Forms.Button btnClearStatus;
        private System.Windows.Forms.ListView lv_tag;
        private System.Windows.Forms.ColumnHeader col_index;
        private System.Windows.Forms.ColumnHeader col_epc;
        private System.Windows.Forms.ColumnHeader col_rssi;
        private System.Windows.Forms.ColumnHeader col_count;
        private System.Windows.Forms.ColumnHeader col_data;
        private System.Windows.Forms.TextBox tbStatus;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lb_rate;
        private System.Windows.Forms.Timer timer_rate;
        private System.Windows.Forms.Label label7;

    }
}

