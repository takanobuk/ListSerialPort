using System;
using System.Windows.Forms;
using System.Management;
using System.Linq;
using System.IO.Ports;
using System.Xml.Linq;
using System.Threading.Tasks;


namespace ListSerialPort
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }


        public const int WM_DEVICECHANGE = 0x00000219;  //デバイス変化のWindowsイベントの値
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            switch (m.Msg)
            {
                case WM_DEVICECHANGE:   //デバイス状況の変化イベント
                    if (m.WParam == (IntPtr)0x0007) // DBT_DEVNODES_CHANGED
                        Task.Run(() => CheckDevice());      //デバイスをチェック
                    break;
            }
        }
        
        
        private void CheckDevice()
        {
            //UIスレッドでリストを更新
            this.Invoke((MethodInvoker)delegate
            {
                UpdateSerialPortList();
            });
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
                var CheckComNum = new System.Text.RegularExpressions.Regex("COM[1-9][0-9]?[0-9]?");

                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity"))
                {
                    ManagementObjectCollection results = searcher.Get();

                    foreach (var item in results)
                    {
                        string name = item["Name"]?.ToString() ?? "";

                        if (CheckComNum.IsMatch(name))
                        {
                            ctlList.Items.Add(name);
                        }
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
