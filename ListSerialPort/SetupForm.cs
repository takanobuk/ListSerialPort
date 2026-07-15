using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ListSerialPort
{
    public partial class SetupForm : Form
    {
        public string AppPath
        {
            get { return txtAppPath.Text; }
            set { txtAppPath.Text = value; }
        }


        public string AppArgument
        {
            get { return txtArg.Text; }
            set { txtArg.Text = value; }
        }


        public string MenuTitle
        {
            get { return txtMenu.Text; }
            set { txtMenu.Text = value; }
        }


        public SetupForm()
        {
            InitializeComponent();
        }


        private void btnSelect_Click(object sender, EventArgs e)
        {
            FileDialog fileDialog = new OpenFileDialog();

            fileDialog.FileName = txtAppPath.Text;

            fileDialog.Filter = "実行ファイル (*.exe)|*.exe|すべてのファイル (*.*)|*.*";
            fileDialog.Title = "実行ファイルを選択してください";
            fileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            fileDialog.RestoreDirectory = true;
            fileDialog.CheckFileExists = true;
            fileDialog.CheckPathExists = true;
            fileDialog.ValidateNames = true;

            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                txtAppPath.Text = fileDialog.FileName;
            }
        }


        private void btnOk_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }


        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult= DialogResult.Cancel;
        }

        private void txtAppPath_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrEmpty(txtAppPath.Text))
                return;

            if (System.IO.File.Exists(txtAppPath.Text))
                e.Cancel = false;
            else
                e.Cancel = true;
        }
    }
}
