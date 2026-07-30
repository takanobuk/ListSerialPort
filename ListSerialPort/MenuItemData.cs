using System;

namespace ListSerialPort
{
    public class MenuItemData
    {
        /// <summary>
        /// 実行ファイルのパス
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// 実行ファイルへの引数
        /// {0} = COMポート名（例: COM1）
        /// {1} = COM番号（例: 1）
        /// </summary>
        public string Arguments { get; set; }

        /// <summary>
        /// メニュー表示タイトル
        /// </summary>
        public string MenuTitle { get; set; }
    }
}
