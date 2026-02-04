using System;
using System.Windows.Forms;
using System.Management;
using System.Linq;


namespace ListSerialPort
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            UpdateSerialPortList();
        }


        private void btnUpdate_Click(object sender, EventArgs e)
        {
            UpdateSerialPortList();
        }


        private void UpdateSerialPortList()
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                ctlList.Items.Clear();

                // WMIクエリでシリアルポート情報を取得
                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_SerialPort"))
                {
                    ManagementObjectCollection results = searcher.Get();

                    // LINQを使ってCaption順に並べ替える
                    var sortedResults = results.Cast<ManagementObject>()
                                               .OrderBy(mo => mo["DeviceID"]);

                    foreach (ManagementObject port in sortedResults)
                    {
                        string deviceId = port["DeviceID"]?.ToString() ?? "不明";
                        string name = port["Name"]?.ToString() ?? "不明";
                        string description = port["Description"]?.ToString() ?? "不明";

                        //string portInfo = $"{deviceId} - {name} ({description})";
                        string portInfo = $"{deviceId} - {description}";
                        ctlList.Items.Add(portInfo);
                    }
                }

                if (ctlList.Items.Count == 0)
                {
                    ctlList.Items.Add("シリアルポートが見つかりませんでした。");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"エラーが発生しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

    }
}
