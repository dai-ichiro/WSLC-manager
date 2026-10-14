# WSL Controller

Windows Form アプリケーションから WSL2 (Windows Subsystem for Linux) のディストリビューションを制御・監視するためのアプリケーションです。

## 概要

指定した WSL2 ディストリビューション上で、スタートアップスクリプトをバックグラウンド起動および停止し、稼働状態をリアルタイムでモニタリングするための GUI ツールです。

## 特長・機能

- **WSL 状態のリアルタイム監視**: 3秒ごとに `wsl.exe -l -v` の出力を確認し、稼働中 (Running) / 停止中 (Stopped) のステータスを画面に表示します。
- **ワンクリックでの起動・停止**:
  - **起動**: 設定された WSL ディストリビューション上で指定のスクリプトを `bash -lc` 経由で実行します。
  - **停止**: 指定された WSL ディストリビューションを強制停止 (`wsl.exe -t <DistroName>`) します。
- **アプリ終了時の自動シャットダウン**: フォームを閉じる際に対象の WSL ディストリビューションを自動で停止します。

## 動作環境

- **OS**: Windows 10 / Windows 11
- **フレームワーク**: .NET Framework 4.8
- **依存環境**: WSL2 (Windows Subsystem for Linux)

## 設定ファイル (`settings.json`)

実行ファイル (`WinForm.exe`) と同じフォルダに `settings.json` を配置して動作を設定します。

```json
{
  "DistroName": "Ubuntu-26.04",
  "StartupScript": "~/sample.sh"
}
```

### 設定項目

- **`DistroName`**: 操作対象の WSL ディストリビューション名（例: `Ubuntu-26.04`）
- **`StartupScript`**: 起動時に WSL 上で実行するスクリプトまたはコマンド（例: `~/sample.sh`）

## 使用方法

1. `settings.json` に操作したいディストリビューション名と起動スクリプトを指定します。
2. アプリケーションを実行します。
3. **「起動 (Start)」** ボタンをクリックすると WSL ディストリビューションおよび指定スクリプトが起動します。
4. **「停止 (Stop)」** ボタンをクリックすると WSL ディストリビューションが停止します。

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
