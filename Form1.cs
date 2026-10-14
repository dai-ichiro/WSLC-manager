using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinForm
{
    public partial class Form1 : Form
    {
        private bool _isChecking;
        private bool _isBusy;

        public Form1()
        {
            InitializeComponent();
            Load += Form1_Load;
            FormClosing += Form1_FormClosing;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            timerStatus.Stop();
            UseWaitCursor = true;

            // 停止完了まで待ってから閉じる(×ボタン・Alt+F4・Windows終了のいずれでも通る)
            try
            {
                WslManager.Stop();
            }
            catch (Exception)
            {
                // 終了処理を妨げないよう握りつぶす
            }
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            if (AppSettings.Current != null && !string.IsNullOrEmpty(AppSettings.Current.ContainerName))
            {
                this.Text = AppSettings.Current.ContainerName;
            }
            await RefreshStatusAsync();
        }

        private async void timerStatus_Tick(object sender, EventArgs e)
        {
            if (_isChecking || _isBusy)
            {
                return;
            }

            await RefreshStatusAsync();
        }

        private async void btnStart_Click(object sender, EventArgs e)
        {
            bool showTerminal = chkShowTerminal.Checked;
            await RunBusyAsync(() => WslManager.Start(showTerminal));
        }

        private async void btnStop_Click(object sender, EventArgs e)
        {
            await RunBusyAsync(WslManager.Stop);
        }

        private async Task RunBusyAsync(Action action)
        {
            _isBusy = true;
            btnStart.Enabled = false;
            btnStop.Enabled = false;
            chkShowTerminal.Enabled = false;

            try
            {
                await Task.Run(action);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _isBusy = false;
                await RefreshStatusAsync();
            }
        }

        private async Task RefreshStatusAsync()
        {
            _isChecking = true;

            try
            {
                bool running = await Task.Run(() => WslManager.IsRunning());

                lblStatus.Text = running ? "稼働中 (Running)" : "停止中 (Stopped)";
                lblStatus.ForeColor = running ? Color.Green : Color.Gray;

                if (!_isBusy)
                {
                    btnStart.Enabled = !running;
                    btnStop.Enabled = running;
                    chkShowTerminal.Enabled = !running;
                }
            }
            catch (Exception)
            {
                lblStatus.Text = "確認エラー";
                lblStatus.ForeColor = Color.Red;
            }
            finally
            {
                _isChecking = false;
            }
        }
    }
}
