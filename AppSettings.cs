using System;
using System.IO;
using System.Text;
using System.Web.Script.Serialization;
using System.Windows.Forms;

namespace WinForm
{
    internal sealed class AppSettings
    {
        public const string FileName = "settings.json";

        public string ContainerName { get; set; }
        public string ScriptPath { get; set; }

        public static AppSettings Current { get; private set; }

        public static string FilePath =>
            Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), FileName);

        public static void Load()
        {
            string path = FilePath;

            if (!File.Exists(path))
            {
                throw new FileNotFoundException(
                    $"設定ファイルが見つかりません。\r\n{path}\r\n\r\n" +
                    "exe と同じフォルダに settings.json を配置してください。", path);
            }

            AppSettings settings;
            try
            {
                var serializer = new JavaScriptSerializer();
                settings = serializer.Deserialize<AppSettings>(File.ReadAllText(path, Encoding.UTF8));
            }
            catch (Exception ex)
            {
                throw new InvalidDataException(
                    $"settings.json の形式が正しくありません。\r\n{ex.Message}", ex);
            }

            if (settings == null
                || string.IsNullOrWhiteSpace(settings.ContainerName)
                || string.IsNullOrWhiteSpace(settings.ScriptPath))
            {
                throw new InvalidDataException(
                    "settings.json に ContainerName と ScriptPath を指定してください。");
            }

            settings.ContainerName = settings.ContainerName.Trim();
            settings.ScriptPath = settings.ScriptPath.Trim();
            Current = settings;
        }
    }
}
