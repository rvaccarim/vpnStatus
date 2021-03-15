using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace vpnStatus {

    public class VpnStatusApp : ApplicationContext {

        private NotifyIcon vpnNotifyIcon;
        private ContextMenuStrip vpnMenu;
        private ToolStripMenuItem exitMenuItem;
        private Timer myTimer;
        private string vpnName;

        public VpnStatusApp() {
            Init();
            ScanNetwork(vpnName);
            myTimer.Enabled = true;
        }

        private void Init() {
            vpnNotifyIcon = new NotifyIcon();
            vpnMenu = new ContextMenuStrip();
            exitMenuItem = new ToolStripMenuItem();

            vpnMenu.ImageScalingSize = new Size(20, 20);
            vpnMenu.Items.AddRange(new ToolStripItem[] {
            exitMenuItem});
            vpnMenu.Name = "vpnMenu";
            vpnMenu.Size = new Size(103, 28);

            exitMenuItem.Name = "exitToolStripMenuItem";
            exitMenuItem.Size = new Size(210, 24);
            exitMenuItem.Text = "Exit";
            exitMenuItem.Click += new EventHandler(this.ExitMenuItem_Click);

            vpnNotifyIcon.ContextMenuStrip = this.vpnMenu;
            vpnNotifyIcon.Icon = Properties.Resources.Inactive;
            vpnNotifyIcon.Text = "VPN Status";
            vpnNotifyIcon.Visible = true;

            myTimer = new Timer {
                Enabled = false,
                Interval = 3000
            };

            myTimer.Tick += new EventHandler(this.TimerElapsed);

            vpnName = Properties.Settings.Default.VpnName;
        }

        private void ScanNetwork(string vpnName) {

            var proc = new Process {
                StartInfo = new ProcessStartInfo {
                    FileName = "CMD.EXE",
                    Arguments = "/C netsh interface show interface",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };

            bool found = false;

            proc.Start();
            while (!proc.StandardOutput.EndOfStream) {
                string line = proc.StandardOutput.ReadLine();

                if (line.Contains(vpnName)) {
                    found = true;
                    break;
                }
            }

            if (found) {
                this.vpnNotifyIcon.Icon = Properties.Resources.Active;
            }
            else {
                this.vpnNotifyIcon.Icon = Properties.Resources.Inactive;
            }

        }

        private void TimerElapsed(object sender, EventArgs e) {
            ScanNetwork(vpnName);
        }

        private void ExitMenuItem_Click(object sender, EventArgs e) {
            Exit();
        }

        private void Exit() {
            vpnNotifyIcon.Visible = false;
            myTimer.Enabled = false;

            Application.Exit();
        }
    }
}
