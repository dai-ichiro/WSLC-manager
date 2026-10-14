using System;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace WinForm
{
    internal static class WslManager
    {
        private static string ContainerName => AppSettings.Current.ContainerName;
        private static string ScriptPath => AppSettings.Current.ScriptPath;

        // wslc start <container> -> wslc container exec <container> sh <script>
        public static void Start(bool showTerminal = false)
        {
            RunAndWait($"start {ContainerName}");

            string execArgs = $"container exec {ContainerName} sh {ScriptPath}";

            ProcessStartInfo psi;
            if (showTerminal)
            {
                // 出力を確認できるようコンソールを開いたままにする
                psi = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/k wslc {execArgs}",
                    UseShellExecute = false,
                };
            }
            else
            {
                psi = new ProcessStartInfo
                {
                    FileName = "wslc.exe",
                    Arguments = execArgs,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                };
            }

            Process.Start(psi);
        }

        public static void Stop()
        {
            RunAndWait($"stop {ContainerName}", 120000);
        }

        public static bool IsRunning()
        {
            // 既定では実行中のコンテナーのみ表示される。名前は最終列。
            string output = RunAndRead("container list");

            foreach (var rawLine in output.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
            {
                string[] parts = Regex.Split(rawLine.Trim(), @"\s+");

                if (parts.Length >= 2 && string.Equals(parts[parts.Length - 1], ContainerName, StringComparison.Ordinal))
                {
                    return true;
                }
            }

            return false;
        }

        private static void RunAndWait(string arguments, int timeoutMs = 15000)
        {
            var psi = new ProcessStartInfo
            {
                FileName = "wslc.exe",
                Arguments = arguments,
                UseShellExecute = false,
                CreateNoWindow = true,
            };

            using (var process = Process.Start(psi))
            {
                process?.WaitForExit(timeoutMs);
            }
        }

        private static string RunAndRead(string arguments)
        {
            var psi = new ProcessStartInfo
            {
                FileName = "wslc.exe",
                Arguments = arguments,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                StandardOutputEncoding = Encoding.UTF8,
            };

            using (var process = Process.Start(psi))
            {
                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit(5000);
                return output;
            }
        }
    }
}
