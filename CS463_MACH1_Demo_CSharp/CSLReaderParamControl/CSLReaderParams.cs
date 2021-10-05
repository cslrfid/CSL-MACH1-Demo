using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Collections;

using CSL;
using CSL.Mach1;


namespace CSLReaderParamControl
{
    public enum MODE
    {
        NEW,
        UPDATE
    }

    public partial class CSLReaderParams : UserControl
    {
        private MODE opMode = MODE.NEW;

        public CSLReaderParams()
        {
            InitializeComponent();
        }

        private void tpAntenna_Click(object sender, EventArgs e)
        {

        }

        private void cbLBTMode_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void CSLReaderParams_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Update control based on input settings
        /// </summary>
        /// <param name="settings"></param>
        public void UpdateForm(CSLReaderSettings settings)
        {
            if (settings == null)
            {
                tbReaderName.Text = "192.168.25.200";
                cbRegion.SelectedIndex = 0;
                cbFrequencyMode.Enabled = false;
                cbFrequencyList.Enabled = false;


                chAnt1.Checked = true;
                chAnt2.Checked = true;
                chAnt3.Checked = true;
                chAnt4.Checked = true;

                cbInventoryMode.SelectedIndex = 0;
                cbAutoSetMode.SelectedIndex = 0;
                cbSession.SelectedIndex = 0;
                cbReaderMode.SelectedIndex = 1;
                tbPopulation.Text = "32";
            }
            else
            {
                tbReaderName.Text = settings.reader_information.reader_name;

                cbRegion.SelectedIndex = settings.reader_information.region;
                cbFrequencyMode.SelectedIndex = settings.reader_information.frequency_mode;

                chAnt1.Checked = settings.antennas[0].enabled;
                chAnt2.Checked = settings.antennas[1].enabled;
                chAnt3.Checked = settings.antennas[2].enabled;
                chAnt4.Checked = settings.antennas[3].enabled;

                tbPower1.Text = settings.antennas[0].power.ToString();
                tbPower2.Text = settings.antennas[1].power.ToString();
                tbPower3.Text = settings.antennas[2].power.ToString();
                tbPower4.Text = settings.antennas[3].power.ToString();

                if (settings.reader_information.frequency_mode == 0)
                {
                    switch (settings.reader_information.region)
                    {
                        case 2:
                        case 7:
                            cbFrequencyList.SelectedIndex = (int)(settings.reader_information.frequency - 114) / 12;
                            break;
                        case 5:
                        case 6:
                            cbFrequencyList.SelectedIndex = (int)(settings.reader_information.frequency - 1846) / 2;
                            break;
                        default:
                            cbFrequencyList.Enabled = false;
                            break;
                    }
                }

                cbInventoryMode.SelectedIndex = settings.gen2_params.inventory_mode;
                cbSession.SelectedIndex = settings.gen2_params.session;
                cbAutoSetMode.SelectedIndex = 0; // settings.gen2_params.auto_set_mode - 1;
                cbReaderMode.Enabled = settings.gen2_params.auto_set_mode == 2 ? true : false;
                if (cbReaderMode.Enabled) cbReaderMode.SelectedIndex = settings.gen2_params.mode_id;
                tbPopulation.Text = settings.gen2_params.population.ToString();

                lbFirmware.Text = settings.reader_information.firmware_ver;
                lbSoftware.Text = settings.reader_information.software_ver;

                if (opMode == MODE.UPDATE)
                {
                    tbReaderName.Enabled = false;
                    cbRegion.Enabled = false;
                }

            }
        }

        void llCSL_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.convergence.com.hk/");
        }

        /// <summary>
        /// Get reader settings from the control
        /// </summary>
        /// <returns></returns>
        public CSLReaderSettings GetSettings()
        {
            CSLReaderSettings settings = new CSLReaderSettings();

            settings.gen2_params.mode_id = cbReaderMode.SelectedIndex;
            settings.gen2_params.session = cbSession.SelectedIndex;
            settings.gen2_params.auto_set_mode = 2; //cbAutoSetMode.SelectedIndex + 1;
            settings.gen2_params.inventory_mode = cbInventoryMode.SelectedIndex;
            settings.gen2_params.population = Int32.Parse(tbPopulation.Text);

            settings.antennas[0].enabled = chAnt1.Checked;
            settings.antennas[1].enabled = chAnt2.Checked;
            settings.antennas[2].enabled = chAnt3.Checked;
            settings.antennas[3].enabled = chAnt4.Checked;

            settings.antennas[0].power = float.Parse(tbPower1.Text);
            settings.antennas[1].power = float.Parse(tbPower2.Text);
            settings.antennas[2].power = float.Parse(tbPower3.Text);
            settings.antennas[3].power = float.Parse(tbPower4.Text);

            settings.reader_information.reader_name = tbReaderName.Text;
            settings.reader_information.region = cbRegion.SelectedIndex;
            settings.reader_information.frequency_mode = cbFrequencyMode.SelectedIndex;
            if (settings.reader_information.frequency_mode == 0)
            {
                switch (settings.reader_information.region)
                {
                    case 2:
                    case 7:
                        settings.reader_information.frequency = (ushort)(114 + cbFrequencyList.SelectedIndex * 12);
                        break;
                    case 5:
                    case 6:
                        settings.reader_information.frequency = (ushort)(1846 + cbFrequencyList.SelectedIndex * 2);
                        break;
                }
            }

            return settings;
        }

        private void cbRegion_SelectedIndexChanged(object sender, EventArgs e)
        {
            cbFrequencyList.Items.Clear();

            tbPower1.Text = "30.00";
            tbPower2.Text = "30.00";
            tbPower3.Text = "30.00";
            tbPower4.Text = "30.00";

            switch (cbRegion.SelectedIndex)
            {
                case 0:
                    cbFrequencyMode.Enabled = false;
                    cbFrequencyList.Enabled = false;
                    AddModeIdListItem(new int[] { 0, 1, 2, 3 });
                    AddAutoSetListItem(new int[] { 2 });
                    break;
                case 1:
                    cbFrequencyMode.Enabled = false;
                    cbFrequencyList.Enabled = false;
                    AddModeIdListItem(new int[] { });
                    AddAutoSetListItem(new int[] { });
                    break;
                case 2:
                    cbFrequencyMode.Enabled = true;
                    cbFrequencyList.Items.Add("4 - 865.70MHz (114)");
                    cbFrequencyList.Items.Add("7 - 866.30MHz (126)");
                    cbFrequencyList.Items.Add("10 - 866.70MHz (138)");
                    cbFrequencyList.Items.Add("13 - 867.50MHz (150)");
                    tbPower1.Text = "28.00";
                    tbPower2.Text = "28.00";
                    tbPower3.Text = "28.00";
                    tbPower4.Text = "28.00";
                    cbFrequencyMode.SelectedIndex = 1;
                    AddModeIdListItem(new int[] { 0, 1, 2, 3 });
                    AddAutoSetListItem(new int[] { 2 });
                    break;
                case 3:
                    cbFrequencyMode.Enabled = false;
                    cbFrequencyList.Enabled = false;
                    AddModeIdListItem(new int[] { 0, 1, 2, 3 });
                    AddAutoSetListItem(new int[] { 2 });
                    break;
                case 4:
                    cbFrequencyMode.Enabled = false;
                    cbFrequencyList.Enabled = false;
                    AddModeIdListItem(new int[] { 0, 1, 2, 3 });
                    AddAutoSetListItem(new int[] { 2 });
                    break;
                case 5:
                    cbFrequencyMode.Enabled = true;
                    //cbFrequencyList.Enabled = true;
                    cbFrequencyList.Items.Add("1 - 952.30MHz (1846)");
                    cbFrequencyList.Items.Add("2 - 952.40MHz (1848)");
                    cbFrequencyList.Items.Add("3 - 952.50MHz (1850)");
                    cbFrequencyList.Items.Add("4 - 952.60MHz (1852)");
                    cbFrequencyList.Items.Add("5 - 952.70MHz (1854)");
                    cbFrequencyList.Items.Add("6 - 952.80MHz (1856)");
                    cbFrequencyList.Items.Add("7 - 952.90MHz (1858)");
                    cbFrequencyList.Items.Add("8 - 953.00MHz (1860)");
                    cbFrequencyList.Items.Add("9 - 953.10MHz (1862)");
                    cbFrequencyList.Items.Add("10 - 953.20MHz (1864)");
                    cbFrequencyList.Items.Add("11 - 953.30MHz (1866)");
                    cbFrequencyList.Items.Add("12 - 953.40MHz (1868)");
                    cbFrequencyList.Items.Add("13 - 953.50MHz (1870)");
                    cbFrequencyList.Items.Add("14 - 953.60MHz (1872)");
                    cbFrequencyList.Items.Add("15 - 953.70MHz (1874)");
                    cbFrequencyList.Items.Add("16 - 953.80MHz (1876)");
                    cbFrequencyMode.SelectedIndex = 1;
                    AddModeIdListItem(new int[] { 0, 1, 2, 3 });
                    AddAutoSetListItem(new int[] { 2 });
                    break;
                case 6:
                    cbFrequencyMode.Enabled = true;
                    //cbFrequencyList.Enabled = true;
                    cbFrequencyList.Items.Add("1 - 952.30MHz (1846)");
                    cbFrequencyList.Items.Add("2 - 952.50MHz (1850)");
                    cbFrequencyList.Items.Add("3 - 952.70MHz (1854)");
                    cbFrequencyList.Items.Add("4 - 952.90MHz (1858)");
                    cbFrequencyList.Items.Add("5 - 953.10MHz (1862)");
                    cbFrequencyList.Items.Add("6 - 953.30MHz (1866)");
                    cbFrequencyList.Items.Add("7 - 953.50MHz (1870)");
                    cbFrequencyList.Items.Add("8 - 953.70MHz (1874)");
                    cbFrequencyList.Items.Add("9 - 953.90MHz (1878)");
                    cbFrequencyList.Items.Add("10 - 954.10MHz (1882)");
                    cbFrequencyList.Items.Add("11 - 954.30MHz (1886)");
                    cbFrequencyList.Items.Add("12 - 954.50MHz (1890)");
                    cbFrequencyList.Items.Add("13 - 954.70MHz (1894)");
                    tbPower1.Text = "10.00";
                    tbPower2.Text = "10.00";
                    tbPower3.Text = "10.00";
                    tbPower4.Text = "10.00";
                    cbFrequencyMode.SelectedIndex = 1;
                    AddModeIdListItem(new int[] { 0, 1, 2, 3 });
                    AddAutoSetListItem(new int[] { 2 });
                    break;
                case 7:
                    cbFrequencyMode.Enabled = true;
                    //cbFrequencyList.Enabled = true;
                    cbFrequencyList.Items.Add("4 - 865.70MHz (114)");
                    cbFrequencyList.Items.Add("7 - 866.30MHz (126)");
                    cbFrequencyList.Items.Add("10 - 866.70MHz (138)");
                    cbFrequencyList.Items.Add("13 - 867.50MHz (150)");
                    tbPower1.Text = "28.00";
                    tbPower2.Text = "28.00";
                    tbPower3.Text = "28.00";
                    tbPower4.Text = "28.00";
                    cbFrequencyMode.SelectedIndex = 1;
                    AddModeIdListItem(new int[] { 0, 1, 2, 3 });
                    AddAutoSetListItem(new int[] { 2 });
                    break;
                case 8:
                    cbFrequencyMode.Enabled = false;
                    cbFrequencyList.Enabled = false;
                    AddModeIdListItem(new int[] { 0, 1, 2, 3 });
                    AddAutoSetListItem(new int[] { 2 });
                    break;
                case 9:
                    cbFrequencyMode.Enabled = false;
                    cbFrequencyList.Enabled = false;
                    AddModeIdListItem(new int[] { 0, 1, 2, 3 });
                    AddAutoSetListItem(new int[] { 2 });
                    break;
                case 10:
                    cbFrequencyMode.Enabled = false;
                    cbFrequencyList.Enabled = false;
                    AddModeIdListItem(new int[] { 0, 1, 2 });
                    AddAutoSetListItem(new int[] { 2 });
                    break;
            }

            try
            {
                cbAutoSetMode.SelectedIndex = 1;
                cbReaderMode.SelectedIndex = 1;
            }
            catch
            {
            }
        }

        //private void AddSessionListItem(int[] sessions)
        //{
        //    cbSession.Items.Clear();
        //    foreach (int i in sessions)
        //    {
        //        cbSession.Items.Add(string.Format("Session {0}", i));
        //    }
        //}

        private void AddModeIdListItem(int[] modeIDs)
        {
            cbReaderMode.Items.Clear();
            foreach (int i in modeIDs)
            {
                string modeIdStr = null;
                switch (i)
                {
                    case 0:
                        modeIdStr = "0";
                        break;
                    case 1:
                        modeIdStr = "1";
                        break;
                    case 2:
                        modeIdStr = "2";
                        break;
                    case 3:
                        modeIdStr = "3";
                        break;
                }
                cbReaderMode.Items.Add(modeIdStr);
            }
        }

        private void AddAutoSetListItem(int[] autoSetIDs)
        {
            cbAutoSetMode.Items.Clear();

            foreach (int id in autoSetIDs)
            {
                string autosetId = null;
                switch (id)
                {
                    case 2:
                        autosetId = "2 - By Mode ID";
                        break;

                }
                cbAutoSetMode.Items.Add(autosetId);

            }
        }


        private void chSingleTarget_CheckedChanged(object sender, EventArgs e)
        {
            cbSession.SelectedIndex = 0;
        }

        public void SetMode(MODE mode)
        {
            opMode = mode;
        }

        private void cbAutoSetMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            cbReaderMode.Enabled = cbAutoSetMode.SelectedIndex == 0 ? true : false;
        }

        private void cbFrequencyMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            cbFrequencyList.Enabled = cbFrequencyMode.SelectedIndex == 0 ? true : false;
        }
    }
}