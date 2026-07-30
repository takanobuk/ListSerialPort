
# ListSerialPort

Windowsにおいて、シリアルポートを一覧表示する.NET Frameworkフォームアプリケーション

## Features

- WMIを使用してシリアルポート情報を取得し、一覧表示します。
- USBシリアルなど接続および切断すると、自動更新します。
- 一覧のCOMポートを右クリックして、ポート名をクリップボードにコピーしたり、任意のアプリケーション(TeraTerm等)を起動することができます。

## Install

ダウンロードしたzipファイルを解凍し、任意のフォルダーに配置し、ListSerialPort.exeを実行してください。

必要に応じてデスクトップなどにショートカットを作成しすると便利です。

## Setting

ListSerialPort.exeと同じフォルダーにある、config.xmlを編集することで、起動するアプリケーションを追加変更することができます。

メニュー項目は複数設定可能です。ファイルは起動時に読み込まれます。

```
<Menu>
	<Path>
		C:\Program Files (x86)\teraterm\ttermpro.exe
	</Path>

		<!-- 起動したいアプリケーションのパスを指定してください。 -->

	<Arguments>
		/C={1} /SPEED=115200 /CDATABIT=8 /CPARITY=none /CSTOPBIT=1 /CFLOWCTRL=none
	</Arguments>

		<!-- 起動したいアプリケーションの引数を指定してください。{0}はCOMポート名(COM1～)、{1}はCOMポート番号(1～)に置換されます。 -->

	<MenuTitle>
		TeraTermを起動(115200bps)
	</MenuTitle>

		<!-- メニューに表示するタイトルを指定してください。 -->
</Menu>
```

## Build

Visual Studioで開いてコンパイル。

VS2022, VS2026で確認済み。

## Note

今のところ、Windows 11のみ動作確認しています。

## License

This software is released under the MIT License, see LICENSE.

## Thanks

- [TeraTerm](https://teratermproject.github.io/)
- [icon-icon](https://icon-icons.com/)
- [Faviconジェネレーター](https://favicon-generator.mintsu-dev.com/)
