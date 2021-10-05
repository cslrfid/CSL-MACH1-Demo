/**
 ******************************************************************************
 **
 ** @file Mainform.cs
 **
 ** 
 **
 ** 
 **
 ******************************************************************************/

//History

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Collections;
using System.Text.RegularExpressions;

using System.IO;
using CSL;             //include in CSL namespace
using CSL.Mach1;       //include in CSL.Mach1 namespace

namespace CS463Mach1Samples
{
    public partial class MainForm : Form
    {
        #region Members

        //Declare a reader
        CS461Reader reader;

        //Declare a reader settins
        CSLReaderSettings settings;
        string readerSettingsPath = Application.StartupPath + "\\reader.xml";

        ArrayList tags;
        TAG_ACTION act;

        ushort[] writeEpcData;
        ushort[] writeUm1Data;
        ushort[] writeUm2Data;

        int readMB0 = 0;
        int readMB2 = 0;
        int readMB3 = 0;

        int m_totaltag = 0;
        int m_tagRate = 0;

        #endregion

        #region Definition

        internal enum NTF_TYPE
        {
            INVENTORY = 0,
            OTHER
        }

        internal enum TAG_ACTION
        {
            INVENTORY = 0,
            READ,
            WRITE,
            WRITE_USER_MEMORY   //@derek
        }

        #endregion


        public MainForm()
        {
            InitializeComponent();

            //@derek
            cbEpcLen.BeginUpdate();
            for (int i = 1; i < 16; i++)
            {
                cbEpcLen.Items.Add(String.Format("{0} bits", i * 16));
            }
            cbEpcLen.EndUpdate();
            //cbEpcLen.SelectedIndex = cbEpcLen.Items.Count - 1;
            cbEpcLen.SelectedIndex = 5;

            cbUmLen.BeginUpdate();
            for (int i = 1; i < 17; i++)
            {
                cbUmLen.Items.Add(String.Format("{0} bits", i * 16));
            }
            cbUmLen.EndUpdate();
            cbUmLen.SelectedIndex = cbUmLen.Items.Count - 1;

            cbMB.SelectedIndex = 2;

            cbMBLen.BeginUpdate();
            for (int i = 1; i < 30; i++)
            {
                cbMBLen.Items.Add(String.Format("{0} bits", i * 16));
            }
            cbMBLen.EndUpdate();
            cbMBLen.SelectedIndex = 1;

            cbMB0Len.BeginUpdate();
            cbMB2Len.BeginUpdate();
            cbMB3Len.BeginUpdate();
            cbMB0Len.Items.Add("NONE");
            cbMB2Len.Items.Add("NONE");
            cbMB3Len.Items.Add("NONE");
            for (int i = 1; i < 33; i++)
            {
                cbMB0Len.Items.Add(String.Format("{0} bits", i * 16));
                cbMB2Len.Items.Add(String.Format("{0} bits", i * 16));
                cbMB3Len.Items.Add(String.Format("{0} bits", i * 16));
            }
            cbMB0Len.EndUpdate();
            cbMB2Len.EndUpdate();
            cbMB3Len.EndUpdate();
            cbMB0Len.SelectedIndex = 0;
            cbMB2Len.SelectedIndex = 0;
            cbMB3Len.SelectedIndex = 0;
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            if (btnConnect.Text == "Connect")
            {
                ReadParamForm rp_frm = new ReadParamForm();
                rp_frm.mode = CSLReaderParamControl.MODE.NEW;                          //Notify the reader parameters form to reader initialization status 

                //Use previous settings if exist
                if (File.Exists(readerSettingsPath) == true)
                {
                    rp_frm.settings = loadReaderSettings();
                }

                if (rp_frm.ShowDialog() == DialogResult.OK)
                {
                    settings = rp_frm.settings;

                    toolStripStatusLabel1.Text = string.Format("Trying to connect to {0}", settings.reader_information.reader_name);

                    if (reader == null) reader = new CS461Reader();      //make sure that the reader is instantialized

                    //Initialize reader by applying reader name and region
                    if (reader.Initialize(settings.reader_information.reader_name, (REGULATORY_REGION)settings.reader_information.region))
                    {
                        //Apply all reader run time parameters. You can apply the parameters individually by using individual API
                        //For more information, please check the ApplyReaderSetting code
                        if (!reader.ApplyReaderSetting(settings))
                        {
                            MessageBox.Show("Failed to apply reader parameters!");
                            if (reader.Connected) reader.Disconnect();
                            toolStripStatusLabel1.Text = string.Format("Failted to connect to {0}", settings.reader_information.reader_name);

                            this.Cursor = Cursors.Default;
                            return;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Can not connect to the reader. Please check if the connection or make sure that no other device connected to it.");
                        if (reader.Connected) reader.Disconnect();
                        toolStripStatusLabel1.Text = string.Format("Failted to connect to {0}", settings.reader_information.reader_name);

                        this.Cursor = Cursors.Default;
                        return;
                    }

                    btnConnect.Text = "Disconnect";
                    toolStripStatusLabel1.Text = string.Format("Now connected to {0}", settings.reader_information.reader_name);
                    saveReaderSettings(settings);
                }
            }
            else
            {
                if (reader != null)
                {
                    if (reader.Disconnect())
                    {
                        btnConnect.Text = "Connect";
                        toolStripStatusLabel1.Text = string.Format("{0} is now disconnected!", settings.reader_information.reader_name);
                        btnTest.Enabled = true;
                        btnStop.Enabled = false;
                    }
                }
            }

            this.Cursor = Cursors.Default;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            //Create an instance of reader
            reader = new CS461Reader();

            //Demonstration of Add event subscription
            reader.onInventoryNtfReceived += new delegateInventoryNtf(reader_onInventoryNtfReceived);
            reader.onStatusReportNtfReceived += new delegateStatusReportNtf(reader_onStatusReportNtfReceived);
            reader.onSystemErrorNtfReceived += new delegateSystemErrorNtf(reader_onSystemErrorNtfReceived);

            //Create an instance of reader settings. Default parameters are applied
            settings = new CSLReaderSettings();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (reader == null || !reader.Connected) return;

            ReadParamForm rp_frm = new ReadParamForm();
            rp_frm.mode = CSLReaderParamControl.MODE.UPDATE;                          //Notify the reader parameters form to reader initialization status 

            CSLReaderSettings s_settings;
            reader.GetReaderSettings(out s_settings);

            if (s_settings != null) settings = s_settings;
            rp_frm.settings = settings;

            if (rp_frm.ShowDialog() == DialogResult.OK)
            {
                settings = rp_frm.settings;

                toolStripStatusLabel1.Text = string.Format("Updating {0}", settings.reader_information.reader_name);

                if (!reader.ApplyReaderSetting(settings))
                {
                    MessageBox.Show("Failed to apply reader parameters!");
                }
                else
                {
                    toolStripStatusLabel1.Text = string.Format("Parameters of {0} are updated!", settings.reader_information.reader_name);
                }

                this.Cursor = Cursors.Default;
            }
        }

        void reader_onSystemErrorNtfReceived(MANAGEMENT_NTF.SYSTEM_ERROR_NTF sen)
        {
            string msg = string.Format("System Error! Reason:{0}\r\n", sen.err_reason.ToString());
            UpdateControls(msg, NTF_TYPE.OTHER);
        }

        void reader_onStatusReportNtfReceived(MANAGEMENT_NTF.STATUS_REPORT_NTF srn)
        {
            string msg = string.Format("Status Report! Reason:{0}\r\n", srn.modem_overflow_ntf_loss.ToString());
            UpdateControls(msg, NTF_TYPE.OTHER);
        }

        void reader_onInventoryNtfReceived(OPERATION_NTF.INVENTORY_NTF inv)
        {
            CMD_RETURN cmd_return;
            bool err;

            //To eliminate duplication tags.
            /*if (tags.Contains(inv.EPC))
            {
                //if the action is inventory, return without executing InventoryContinue 
                if (act == TAG_ACTION.INVENTORY) return;
                //Executing InventoryContinue
                else
                {
                    cmd_return = reader.InventoryContinue(out err);
                    return;
                }
            }

            tags.Add(inv.EPC);*/

            string msg;
            switch (act)
            {
                case TAG_ACTION.INVENTORY:
                    string mb;
                    try
                    {
                        mb = ConvertData(inv.gen2ReadData);
                    }
                    catch
                    {
                        mb = "";
                    }
                    UpdateListView(inv.EPC, (float)(inv.rssi), mb);
                    m_totaltag++;
                    break;
                case TAG_ACTION.READ:
                    {
                        if (inv.halted == false) return;

                        OPERATION_NTF.TAG_READ_NTF r_ntf;

                        //EPC
                        msg = string.Format("\r\nTag Read ==========\r\nEPC: {0}\r\nRSSI: {1}\r\nPC: {2:X2}{3:X2}\r\n", inv.EPC, inv.rssi, inv.gen2PC[0], inv.gen2PC[1]);
                        UpdateControls(msg, NTF_TYPE.OTHER);

                        //Read MB0 from tag
                        if (readMB0 > 0)
                        {
                            cmd_return = reader.TagRead(OPERATION_CMD.MEMORY_BANK.RESERVED, 0/*read from first word*/, (byte)readMB0/*read two words*/, out r_ntf);
                            if (cmd_return == CMD_RETURN.COMMAND_SUCESS)
                            {
                                msg = string.Format("MB0: {0} \r\n", ConvertData(r_ntf.data));
                            }
                            else
                            {
                                msg = string.Format("Error: {0} ({1})\r\n", cmd_return.ToString(), r_ntf.result_code);
                            }
                            UpdateControls(msg, NTF_TYPE.OTHER);
                        }

                        //Read MB2 from tag
                        if (readMB2 > 0)
                        {
                            cmd_return = reader.TagRead(OPERATION_CMD.MEMORY_BANK.TID, 0/*read from first word*/, (byte) readMB2/*read two words*/, out r_ntf);
                            if (cmd_return == CMD_RETURN.COMMAND_SUCESS)
                            {
                                msg = string.Format("MB2: {0} \r\n", ConvertData(r_ntf.data));
                            }
                            else
                            {
                                msg = string.Format("Error: {0} ({1})\r\n", cmd_return.ToString(), r_ntf.result_code);
                            }
                            UpdateControls(msg, NTF_TYPE.OTHER);
                        }

                        //Reader MB3 from tag   //@derek
                        bool firstSection = true;
                        int MB3 = readMB3;
                        while (MB3 > 0)
                        {
                            if (MB3 > 16)
                            {
                                cmd_return = reader.TagRead(OPERATION_CMD.MEMORY_BANK.USER, 0/*read from first word*/, 16/*read two words*/, out r_ntf);
                                MB3 -= 16;
                            }
                            else
                            {
                                cmd_return = reader.TagRead(OPERATION_CMD.MEMORY_BANK.USER, 0/*read from first word*/, (byte) MB3/*read two words*/, out r_ntf);
                                MB3 = 0;
                            }

                            if (cmd_return == CMD_RETURN.COMMAND_SUCESS)
                            {
                                if (firstSection == true)
                                {
                                    msg = string.Format("MB3: {0} \r\n", ConvertData(r_ntf.data));
                                    firstSection = false;
                                }
                                else
                                {
                                    msg = string.Format("{0} \r\n", ConvertData(r_ntf.data));
                                }
                            }
                            else
                            {
                                msg = string.Format("Error: {0} ({1})\r\n", cmd_return.ToString(), r_ntf.result_code);
                            }
                            UpdateControls(msg, NTF_TYPE.OTHER);
                        }

                        reader.InventoryContinue(out err);
                    }
                    break;
                case TAG_ACTION.WRITE:
                    {
                        if (inv.halted == false) return;

                        OPERATION_NTF.TAG_WRITE_NTF w_ntf;
                        //ushort[] data = new ushort[] { 0x3200, 0x1111, 0x2222, 0x3333, 0x4444, 0x5555 };
                        //ushort[] data = new ushort[] { 0x31C1, 0x1111, 0x2222, 0x3333, 0x4444, 0x5555, 0x6666, 0x7777, 0x8888, 0x9999, 0xaaaa, 0xbbbb, 0xcccc, 0xdddd, 0xeeee, 0xffff };

                        //Write prepared data to tag
                        //cmd_return = reader.TagWrite(OPERATION_CMD.MEMORY_BANK.EPC, 1/*EPC start from 3rd word*/, data, true/*don't use block write*/, out w_ntf);
                        if (chkPC.Checked)
                        {
                            cmd_return = reader.TagWrite(OPERATION_CMD.MEMORY_BANK.EPC, 1/*EPC start from 3rd word*/, writeEpcData, true, 0, out w_ntf);
                            msg = string.Format("Tag Write EPC ========== \r\nStatus: {0} \r\nResult: {1}\r\nError Addr: {2}\r\n", cmd_return.ToString(), w_ntf.result_code, w_ntf.err_addr);
                            UpdateControls(msg, NTF_TYPE.OTHER);
                        }
                        else if (chkEPC.Checked)
                        {
                            cmd_return = reader.TagWrite(OPERATION_CMD.MEMORY_BANK.EPC, 2/*EPC start from 3rd word*/, writeEpcData, true, 0, out w_ntf);
                            msg = string.Format("Tag Write EPC ========== \r\nStatus: {0} \r\nResult: {1}\r\nError Addr: {2}\r\n", cmd_return.ToString(), w_ntf.result_code, w_ntf.err_addr);
                            UpdateControls(msg, NTF_TYPE.OTHER);
                        }

                        if (chkUM.Checked)
                        {
                            cmd_return = reader.TagWrite(OPERATION_CMD.MEMORY_BANK.USER, 0/*EPC start from 3rd word*/, writeUm1Data, true, 0, out w_ntf);
                            msg = string.Format("Tag Write User memory ========== \r\nEPC: {0}\r\nStatus: {1} \r\nResult: {2}\r\nError Addr: {3}\r\n", inv.EPC, cmd_return.ToString(), w_ntf.result_code, w_ntf.err_addr);
                            UpdateControls(msg, NTF_TYPE.OTHER);
                        }

                        reader.InventoryContinue(out err);
                    }
                    break;
            }
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            readMB0 = cbMB0Len.SelectedIndex;
            readMB2 = cbMB2Len.SelectedIndex;
            readMB3 = cbMB3Len.SelectedIndex;

            if (reader.Connected == false)
            {
                MessageBox.Show("Please connect to reader first", "Reader not connected", MessageBoxButtons.OK);
                return;
            }

            btnUpdate.Enabled = false;
            btnClearStatus.Enabled = false;
            btnClearTag.Enabled = false;

            switch (comboBox1.Text)
            {
                case "Inventory":
                    lv_tag.Items.Clear();
                    act = TAG_ACTION.INVENTORY;
                    StartInventory();
                    break;
                case "SetGPO":
                    SetGPO();
                    break;
                case "GetGPI":
                    GetGPI();
                    break;
                case "TagRead":
                    TagCommissioningInventory();
                    act = TAG_ACTION.READ;
                    break;
                case "TagWrite":
                    //
                    if ((chkPC.Checked == false) && (chkEPC.Checked == false) && (chkUM.Checked == false))
                    {
                        MessageBox.Show("Nothing to write!!");
                        return;
                    }

                    if (chkPC.Checked == true)
                    {
                        if (txtPC.BackColor == Color.Red)
                        {
                            MessageBox.Show("Invalid PC");
                            txtPC.Focus();
                            return;
                        }
                    }

                    if (chkEPC.Checked)
                    {
                        if (txtEPC.BackColor == Color.Red)
                        {
                            MessageBox.Show("Invalid EPC");
                            txtEPC.Focus();
                            return;
                        }
                    }


                    if (chkUM.Checked)
                    {
                        if (txtUM1.BackColor == Color.Red)
                        {
                            MessageBox.Show("Invalid User memory");
                            txtUM1.Focus();
                            return;
                        }
                        if (txtUM2.BackColor == Color.Red)
                        {
                            MessageBox.Show("Invalid User memory");
                            txtUM2.Focus();
                            return;
                        }
                    }

                    try
                    {
                        StringBuilder sb = new StringBuilder();
                        if (chkPC.Checked)
                        {
                            sb.Append(txtPC.Text);
                        }
                        if (chkEPC.Checked)
                        {
                            sb.Append(txtEPC.Text);
                        }
                        string sEpc = sb.ToString();
                        writeEpcData = new ushort[sEpc.Length / 4];

                        for (int i = 0; i < writeEpcData.Length; i++)
                        {
                            writeEpcData[i] = ushort.Parse(sEpc.Substring(i * 4, 4), System.Globalization.NumberStyles.HexNumber);
                        }
                    }
                    catch
                    {
                        MessageBox.Show("Convert PC / EPC error");
                        return;
                    }

                    try
                    {
                        if (chkUM.Checked)
                        {
                            string sUm1 = txtUM1.Text;
                            writeUm1Data = new ushort[sUm1.Length / 4];
                            for (int i = 0; i < writeUm1Data.Length; i++)
                            {
                                writeUm1Data[i] = ushort.Parse(sUm1.Substring(i * 4, 4), System.Globalization.NumberStyles.HexNumber);
                            }

                            string sUm2 = txtUM2.Text;
                            writeUm2Data = new ushort[sUm2.Length / 4];
                            for (int i = 0; i < writeUm2Data.Length; i++)
                            {
                                writeUm2Data[i] = ushort.Parse(sUm2.Substring(i * 4, 4), System.Globalization.NumberStyles.HexNumber);
                            }
                        }
                    }
                    catch
                    {
                        MessageBox.Show("Convert User memory error");
                        return;
                    }



                    TagCommissioningInventory();
                    act = TAG_ACTION.WRITE;
                    break;
            }
        }

        private void StartInventory()
        {
            CMD_RETURN cmd_return;
            bool err;

            m_totaltag = 0;

            tags = new ArrayList();

            OPERATION_CMD.INVENTORY_PRAMA param = new OPERATION_CMD.INVENTORY_PRAMA();

            //Inventory using defualt setting : No inventory filter, No halting filter
            param.enable_halt_filter = false;
            param.enable_inventory_filter = false;

            //Enable inventory filter
            //Select tags starting with "00001111"
            //param.enable_inventory_filter = true;
            //param.inventory_filter_condition.filter_operation = OPERATION_CMD.INVENTORY_PRAMA.INVENTORY_FILTER_OPERATION.A;
            //param.inventory_filter_condition.a_bit_offset = 0x20;
            //param.inventory_filter_condition.a_compare = OPERATION_CMD.INVENTORY_PRAMA.LOGIC.EQUALS;
            //param.inventory_filter_condition.a_filter_memory_bank = OPERATION_CMD.MEMORY_BANK.EPC;
            //param.inventory_filter_condition.a_length = 32;
            //param.inventory_filter_condition.a_pattern = "00000000000000000001000100010001";

            //Enable halt filter

            //param.enable_halt_filter = true;

            //Halt on every tag
            //param.inventory_halt_condition.halt_operation = OPERATION_CMD.INVENTORY_PRAMA.INVENTORY_HALT_OPERATION.HALT_EVERY_TAG;

            //Use A halt filter
            //param.inventory_halt_condition.halt_operation = OPERATION_CMD.INVENTORY_PRAMA.INVENTORY_HALT_OPERATION.A;       
            //param.inventory_halt_condition.halt_a_bit_offset = 0x70;
            //param.inventory_halt_condition.halt_a_compare = OPERATION_CMD.INVENTORY_PRAMA.LOGIC.EQUALS;
            //param.inventory_halt_condition.halt_a_length = 16;
            //param.inventory_halt_condition.halt_a_memory_bank = OPERATION_CMD.MEMORY_BANK.EPC;
            //param.inventory_halt_condition.halt_a_value = pattern;
            //param.inventory_halt_condition.halt_a_mask = "1111222233334444";

            //Read Other memory bank
            if ((chkMB.Checked == true) && (cbMB.SelectedIndex != 1))
            {
                param.read_memory_bank = (OPERATION_CMD.MEMORY_BANK)cbMB.SelectedIndex;
                param.read_word_memory_address = 0; //start at word 0
                param.read_length = (byte)(cbMBLen.SelectedIndex + 1);  //2 words -> 32bits
            }

            param.estimated_tag_population = (short)settings.gen2_params.population;

            //Start inventory
            cmd_return = reader.Inventory(param, out err);
            if (cmd_return == CMD_RETURN.COMMAND_SUCESS && !err)
            {
                btnTest.Enabled = false;
                btnStop.Enabled = true;
                toolStripStatusLabel1.Text = "Inventory started!";
            }
            else
                toolStripStatusLabel1.Text = "Failed to start inventory!";
        }

        private void SetGPO()
        {
            CMD_RETURN cmd_return;

            //Set one GPO. You can set multiple GPOs too.
            MANAGEMENT_CMD.GPO_CONFIG[] gpo_config = new MANAGEMENT_CMD.GPO_CONFIG[1];
            MANAGEMENT_CMD.SET_GPO_RSP sg_rsp;

            gpo_config[0] = new MANAGEMENT_CMD.GPO_CONFIG();

            //Enable GPO0
            gpo_config[0].id = MANAGEMENT_CMD.GPO_CONFIG.GPO_ID.GPO0;
            //Set Output high
            gpo_config[0].config = MANAGEMENT_CMD.GPO_CONFIG.CONFIG.HIGH;

            cmd_return = reader.SetGPO(gpo_config, out sg_rsp);

            tbStatus.Text += string.Format("SetGPO command : {0}, result: {1}\r\n", cmd_return.ToString(), sg_rsp.result_code.ToString());

        }

        private void SetGPI()
        {
            CMD_RETURN cmd_return;

            MANAGEMENT_CMD.GPI_CONFIG[] gpi_config = new MANAGEMENT_CMD.GPI_CONFIG[2];

            gpi_config[0] = new MANAGEMENT_CMD.GPI_CONFIG();
            gpi_config[1] = new MANAGEMENT_CMD.GPI_CONFIG();

            //Set GPI0 to accept low-to-high trigger
            gpi_config[0].id = MANAGEMENT_CMD.GPI_CONFIG.GPI_ID.GPI0; gpi_config[0].config = MANAGEMENT_CMD.GPI_CONFIG.CONFIG.LO_TO_HIGH;
            //Set GPI1 to accept high-to-low trigger
            gpi_config[1].id = MANAGEMENT_CMD.GPI_CONFIG.GPI_ID.GPI1; gpi_config[1].config = MANAGEMENT_CMD.GPI_CONFIG.CONFIG.HI_TO_LOW;

            MANAGEMENT_CMD.SET_GPI_RSP sgi_rsp;
            cmd_return = reader.SetGPI(gpi_config, out sgi_rsp);

            tbStatus.Text += string.Format("SetGPI command : {0}, result: {1}\r\n", cmd_return.ToString(), sgi_rsp.result_code.ToString());
        }

        private void GetGPI()
        {
            MANAGEMENT_CMD.GET_GPI_RSP gpi_rsp;
            CMD_RETURN cmd_return = reader.GetGPI(out gpi_rsp);

            tbStatus.Text += string.Format("GetGPI command : {0}, result: GPI-{1} is {2}\r\n", cmd_return.ToString(), gpi_rsp.gpi_status[0].id, gpi_rsp.gpi_status[0].status);
            tbStatus.Text += string.Format("GetGPI command : {0}, result: GPI-{1} is {2}\r\n", cmd_return.ToString(), gpi_rsp.gpi_status[1].id, gpi_rsp.gpi_status[1].status);
            tbStatus.Text += string.Format("GetGPI command : {0}, result: GPI-{1} is {2}\r\n", cmd_return.ToString(), gpi_rsp.gpi_status[2].id, gpi_rsp.gpi_status[2].status);
            tbStatus.Text += string.Format("GetGPI command : {0}, result: GPI-{1} is {2}\r\n", cmd_return.ToString(), gpi_rsp.gpi_status[3].id, gpi_rsp.gpi_status[3].status);
        }

        private void TagCommissioningInventory()
        {
            CMD_RETURN cmd_return;
            bool err;

            tags = new ArrayList();

            OPERATION_CMD.INVENTORY_PRAMA param = new OPERATION_CMD.INVENTORY_PRAMA();

            //Enable halt filter
            param.enable_halt_filter = true;

            //Halt on every tag
            param.inventory_halt_condition.halt_operation = OPERATION_CMD.INVENTORY_PRAMA.INVENTORY_HALT_OPERATION.HALT_EVERY_TAG;

            //Halt tags starting with "00001111"
            //param.inventory_halt_condition.halt_operation = OPERATION_CMD.INVENTORY_PRAMA.INVENTORY_HALT_OPERATION.A;
            //param.inventory_halt_condition.halt_a_bit_offset = 0x20;
            //param.inventory_halt_condition.halt_a_compare = OPERATION_CMD.INVENTORY_PRAMA.LOGIC.EQUALS;
            //param.inventory_halt_condition.halt_a_length = 32;
            //param.inventory_halt_condition.halt_a_memory_bank = OPERATION_CMD.MEMORY_BANK.EPC;
            //param.inventory_halt_condition.halt_a_value = "00000000000000000001000100010001";
            //param.inventory_halt_condition.halt_a_mask = "11111111111111111111111111111111";


            //Start inventory
            cmd_return = reader.Inventory(param, out err);

            if (cmd_return == CMD_RETURN.COMMAND_SUCESS && !err)
            {
                btnTest.Enabled = false;
                btnStop.Enabled = true;
                toolStripStatusLabel1.Text = "Tag commissioning inventory started!";
            }
            else
                toolStripStatusLabel1.Text = "Failed to start tag commissioning inventory!";
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            CMD_RETURN cmd_return;
            try
            {
                cmd_return = reader.ModemStop();
                if (cmd_return == CMD_RETURN.COMMAND_SUCESS)
                {
                    btnTest.Enabled = true;
                    btnStop.Enabled = false;
                    btnUpdate.Enabled = true;
                    btnClearStatus.Enabled = true;
                    btnClearTag.Enabled = true;
                    toolStripStatusLabel1.Text = "Inventory stopped!";
                }
                else
                    toolStripStatusLabel1.Text = "Failed to stop inventory!";
            }
            catch
            {
                btnUpdate.Enabled = true;
                btnTest.Enabled = true;
                btnStop.Enabled = false;
                btnClearStatus.Enabled = true;
                btnClearTag.Enabled = true;
            }
        }

        private string ConvertData(byte[] data)
        {

            string s = string.Empty;

            if (data != null) for (int i = 0; i < data.Length; i++) s += string.Format("{0:x2}", data[i]);
            return s;
        }

        private void txtPC_TextChanged(object sender, EventArgs e)
        {
            Regex r = new Regex(String.Format("^(\\d|[a-f]|[A-F]){{{0}}}$", 4));

            txtPC.BackColor = r.IsMatch(txtPC.Text) ? Color.White : Color.Red;
        }

        private void txtEPC_TextChanged(object sender, EventArgs e)
        {
            validateEpcText();
        }

        private void validateEpcText()
        {
            Regex r = new Regex(String.Format("^(\\d|[a-f]|[A-F]){{{0}}}$", txtEPC.MaxLength));

            txtEPC.BackColor = r.IsMatch(txtEPC.Text) ? Color.White : Color.Red;
        }

        private void txtUM1_TextChanged(object sender, EventArgs e)
        {
            validateUmText();
        }

        private void txtUM2_TextChanged(object sender, EventArgs e)
        {
            validateUmText();
        }

        private void validateUmText()
        {
            Regex r = new Regex(String.Format("^(\\d|[a-f]|[A-F]){{{0}}}$", txtUM1.MaxLength));

            txtUM1.BackColor = r.IsMatch(txtUM1.Text) ? Color.White : Color.Red;

            r = new Regex(String.Format("^(\\d|[a-f]|[A-F]){{{0}}}$", txtUM2.MaxLength));

            txtUM2.BackColor = r.IsMatch(txtUM2.Text) ? Color.White : Color.Red;
        }

        private void chkUM_CheckedChanged(object sender, EventArgs e)
        {
            if (chkUM.Checked)
            {
                chkPC.Checked = false;
                chkEPC.Checked = false;
            }
        }

        private void chkPC_CheckedChanged(object sender, EventArgs e)
        {
            if (chkPC.Checked)
            {
                chkUM.Checked = false;
            }
        }

        private void chkEPC_CheckedChanged(object sender, EventArgs e)
        {
            if (chkEPC.Checked)
            {
                chkUM.Checked = false;
            }
        }

        private void cbEpcLen_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtEPC.MaxLength = (cbEpcLen.SelectedIndex + 1) * 4;
            if (txtEPC.Text.Length > txtEPC.MaxLength)
            {
                txtEPC.Text = txtEPC.Text.Substring(0, txtEPC.MaxLength);
            }
            validateEpcText();
        }

        private void cbUmLen_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbUmLen.SelectedIndex > 15)
            {
                txtUM1.MaxLength = 16 * 4;
                txtUM2.MaxLength = (cbUmLen.SelectedIndex - 15) * 4;
                txtUM2.Visible = true;

                if (txtUM1.Text.Length > txtUM1.MaxLength)
                {
                    txtUM1.Text = txtUM1.Text.Substring(0, txtUM1.MaxLength);
                }

                if (txtUM2.Text.Length > txtUM2.MaxLength)
                {
                    txtUM2.Text = txtUM2.Text.Substring(0, txtUM2.MaxLength); ;
                }
            }
            else
            {
                txtUM1.MaxLength = (cbUmLen.SelectedIndex + 1) * 4;
                if (txtUM1.Text.Length > txtUM1.MaxLength)
                {
                    txtUM1.Text = txtUM1.Text.Substring(0, txtUM1.MaxLength);
                }
                txtUM2.MaxLength = 0;
                txtUM2.Text = "";
                txtUM2.Visible = false;
            }
            validateUmText();
        }

        private void saveReaderSettings(CSLReaderSettings settings)
        {
            StreamWriter sw = new StreamWriter(readerSettingsPath, false);
            sw.Write(settings.ToString());
            sw.Close();
        }

        private CSLReaderSettings loadReaderSettings()
        {
            try
            {
                StreamReader sr = new StreamReader(readerSettingsPath);
                string s = sr.ReadToEnd();
                sr.Close();
                CSLReaderSettings rs = new CSLReaderSettings(s);
                return rs;
            }
            catch
            {
                return null;
            }
        }

        private void chkMB_CheckedChanged(object sender, EventArgs e)
        {
            cbMB.Enabled = chkMB.Checked;
            cbMBLen.Enabled = chkMB.Checked;
        }

        private void btnClearTag_Click(object sender, EventArgs e)
        {
            lv_tag.Items.Clear();
        }

        private void btnClearStatus_Click(object sender, EventArgs e)
        {
            tbStatus.Clear();
        }

        private delegate void UpdateListViewDeleg(String EPC, float rssi, String data);
        private void UpdateListView(String EPC, float rssi, String data)
        {
            if (this.InvokeRequired)
            {
                Invoke(new UpdateListViewDeleg(UpdateListView), new object[] { EPC, rssi, data });
                return;
            }

            int foundIndex = -1;
            int count = lv_tag.Items.Count;
            if (count > 0)
            {
                ListViewItem item = lv_tag.FindItemWithText(EPC, true, 0);
                if (item != null)
                {
                    foundIndex = lv_tag.Items.IndexOf(item);
                }
            }
            if (foundIndex >= 0)
            {
                ListViewItem listViewItem = lv_tag.Items[foundIndex];
                listViewItem.SubItems[1].Text = EPC;
                listViewItem.SubItems[2].Text = rssi.ToString();
                listViewItem.SubItems[3].Text = (Convert.ToInt32(listViewItem.SubItems[3].Text) + 1).ToString();
                listViewItem.SubItems[4].Text = data;
            }
            else
            {
                string[] row = { lv_tag.Items.Count.ToString(), EPC, rssi.ToString(), "1", data };
                ListViewItem listViewItem = new ListViewItem(row);
                lv_tag.Items.Add(listViewItem);
            }
        }

        private delegate void UpdateControlsDeleg(string msg, NTF_TYPE type);
        private void UpdateControls(string msg, NTF_TYPE type)
        {
            if (this.InvokeRequired)
            {
                Invoke(new UpdateControlsDeleg(UpdateControls), new object[] { msg, type });
                return;
            }
            switch (type)
            {
                case NTF_TYPE.OTHER:
                    tbStatus.Text += msg;
                    break;
            }
        }

        private delegate void UpdateRateDeleg(int rate);
        private void UpdateRate(int rate)
        {
            if (this.InvokeRequired)
            {
                Invoke(new UpdateRateDeleg(UpdateRate), new object[] { rate });
                return;
            }
            lb_rate.Text = rate.ToString();
        }

        private void readrate_Tick(object sender, EventArgs e)
        {
            m_tagRate = m_totaltag;
            m_totaltag = 0;
            UpdateRate(m_tagRate);
        }
    }
}