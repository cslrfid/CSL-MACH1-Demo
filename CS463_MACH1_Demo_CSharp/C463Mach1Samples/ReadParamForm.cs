using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using CSL;
using CSL.Mach1;

namespace CS463Mach1Samples
{
    public partial class ReadParamForm : Form
    {
        public CSLReaderParamControl.MODE mode = CSLReaderParamControl.MODE.NEW;
        public CSLReaderSettings settings;

        public ReadParamForm()
        {
            InitializeComponent();
        }

        private void ReadParamForm_Load(object sender, EventArgs e)
        {
            readerCtrl.SetMode(mode);
            readerCtrl.UpdateForm(settings);
        }

        private void ReadParamForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            settings = readerCtrl.GetSettings();
        }
    }
}