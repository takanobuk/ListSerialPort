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
            //
            mnuStartApp.Text = Properties.Settings.Default.StartApplicationMenu;

            // シリアルポートリストを更新
            UpdateSerialPortList();
        }


        private void btnUpdate_Click(object sender, EventArgs e)
        {
            // シリアルポートリストを更新
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
                MessageBox.Show(this, $"エラーが発生しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }


        private void mnuStartApp_Click(object sender, EventArgs e)
        {
            if (ctlList.SelectedItem == null)
            {
                MessageBox.Show(this, "項目が選択されていません。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string selectedText = ctlList.SelectedItem.ToString();

            if (selectedText == "シリアルポートが見つかりませんでした。")
            {
                MessageBox.Show(this, "有効なCOMポートが選択されていません。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 正規表現でCOM*のポート名を抽出
            var match = System.Text.RegularExpressions.Regex.Match(selectedText, @"COM\d+");

            if (match.Success)
            {
                string comPort = match.Value;
                string comNumber = comPort.Substring(3); // "COM"の後の数字部分を取得

                try
                {
                    // 外部アプリケーションを起動する
                    // {0}=COMポート名, {1}=COM番号
                    System.Diagnostics.Process.Start(Properties.Settings.Default.StartApplicationPath,
                        string.Format(Properties.Settings.Default.StartApplicationArguments, comPort, comNumber));
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, $"外部アプリケーションの起動に失敗しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show(this, "COMポート名が見つかりませんでした。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void mnuCopy_Click(object sender, EventArgs e)
        {
            if (ctlList.SelectedItem == null)
            {
                MessageBox.Show(this, "項目が選択されていません。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string selectedText = ctlList.SelectedItem.ToString();

            if (selectedText == "シリアルポートが見つかりませんでした。")
            {
                MessageBox.Show(this, "有効なCOMポートが選択されていません。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 正規表現でCOM*のポート名を抽出
            var match = System.Text.RegularExpressions.Regex.Match(selectedText, @"COM\d+");

            if (match.Success)
            {
                try
                {
                    Clipboard.SetText(match.Value);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, $"クリップボードへのコピーに失敗しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show(this, "COMポート名が見つかりませんでした。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F5:
                    // シリアルポートリストを更新
                    UpdateSerialPortList();
                    e.Handled = true;
                    break;
            }
        }


        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }


        private void mnuItemExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }


        private void mnuItemSetup_Click(object sender, EventArgs e)
        {
            //  設定画面を開く
            SetupForm setupForm = new SetupForm();

            setupForm.AppPath = Properties.Settings.Default.StartApplicationPath;
            setupForm.AppArgument = Properties.Settings.Default.StartApplicationArguments;
            setupForm.MenuTitle = Properties.Settings.Default.StartApplicationMenu;

            if (setupForm.ShowDialog() == DialogResult.OK)
            {
                // 設定を保存
                Properties.Settings.Default.StartApplicationPath = setupForm.AppPath;
                Properties.Settings.Default.StartApplicationArguments = setupForm.AppArgument;
                Properties.Settings.Default.StartApplicationMenu = setupForm.MenuTitle;
                Properties.Settings.Default.Save();

                // メニューのテキストを更新
                mnuStartApp.Text = Properties.Settings.Default.StartApplicationMenu;
            }
        }


        private void ctlList_MouseDown(object sender, MouseEventArgs e)
        {
            // 右クリックされた場合、クリックした項目を選択してコンテキストメニューを表示
            if (e.Button == MouseButtons.Right)
            {
                int index = ctlList.IndexFromPoint(e.Location);

                if (index != ListBox.NoMatches)
                {
                    ctlList.ClearSelected();
                    ctlList.SelectedIndex = index;

                    System.Drawing.Point pos = ctlList.PointToScreen(e.Location);

                    menuStrip1.Location = pos;
                    menuStrip1.Show();
                }
            }
        }
    }
}
