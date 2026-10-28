# WSLC manager

Windows Form アプリケーションから WSLC (WSL Container) のコンテナーを起動・停止・監視するためのアプリケーションです。

## 概要

指定した WSL コンテナー（例: `llamacpp`）を起動し、コンテナー内のスクリプトを実行、停止し、稼働状態をリアルタイムでモニタリングするための GUI ツールです。内部では次の 3 つのコマンドを実行します。

```
wslc start llamacpp
wslc container exec llamacpp sh /home/ubuntu/run_nex.sh
wslc stop llamacpp
```

## 特長・機能

- **コンテナー状態のリアルタイム監視**: 3秒ごとに `wslc container list`（既定では実行中のコンテナーのみ表示）の出力を確認し、稼働中 (Running) / 停止中 (Stopped) のステータスを画面に表示します。
- **ワンクリックでの起動・停止**:
  - **起動**: `wslc start <ContainerName>` を実行し、完了後に `wslc container exec <ContainerName> sh <ScriptPath>` を実行します。
  - **停止**: `wslc stop <ContainerName>` を実行します。
- **ターミナルの表示**: チェックを入れて起動すると、スクリプトの出力を確認できるようコンソールウィンドウ (`cmd /k`) を開いたままにします。
- **アプリ終了時の自動停止**: 右上の「×」や Alt+F4 でフォームを閉じる際、`wslc stop` の完了を待ってから閉じます（最大 120 秒）。タスクマネージャーでの強制終了やクラッシュ、電源断の場合は停止されません。

## 動作環境

- **OS**: Windows 11
- **フレームワーク**: .NET Framework 4.8
- **依存環境**: WSLC (`wslc.exe`) がインストールされ、対象コンテナーが作成済みであること

## 設定ファイル (`settings.json`)

実行ファイル (`WinForm.exe`) と同じフォルダに `settings.json` を配置して動作を設定します。

```json
{
  "ContainerName": "llamacpp",
  "ScriptPath": "/home/ubuntu/run_nex.sh"
}
```

### 設定項目

- **`ContainerName`**: 操作対象のコンテナー名（例: `llamacpp`）
- **`ScriptPath`**: 起動時にコンテナー内で `sh` で実行するスクリプトのパス（例: `/home/ubuntu/run_nex.sh`）

両方とも必須です。未設定または形式が不正な場合は、起動時にエラーダイアログが表示されます。

## 使用方法

1. `settings.json` にコンテナー名とスクリプトパスを指定します。
2. アプリケーションを実行します。
3. **「起動 (Start)」** ボタンをクリックするとコンテナーが起動し、指定スクリプトが実行されます。
4. **「停止 (Stop)」** ボタンをクリックするとコンテナーが停止します。

## 注意事項

- 状態判定は `wslc container list` の最終列がコンテナー名であることを前提にしたテキスト解析です。wslc の出力形式が変わると判定が外れる可能性があります。
- 起動時の `wslc start` は 15 秒以内に完了しなくても待機を打ち切り、続けて exec を実行します。

## Building

### PowerShell

#### Debug build

```powershell
&"C:\Program Files\Microsoft Visual Studio\18\Community\MSBuild\Current\Bin\MSBuild.exe" WinForm.csproj -p:Configuration=Debug
```

#### Release build

```powershell
&"C:\Program Files\Microsoft Visual Studio\18\Community\MSBuild\Current\Bin\MSBuild.exe" WinForm.csproj -p:Configuration=Release
```
