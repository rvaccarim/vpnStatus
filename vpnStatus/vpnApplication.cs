using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace vpnStatus {

    public class vpnApplication : ApplicationContext {

        private NotifyIcon vpnNotifyIcon;
        private ContextMenuStrip vpnMenu;
        private ToolStripMenuItem exitMenuItem;
        private Timer myTimer;

        public vpnApplication() {
            Init();
            ScanNetwork();
            myTimer.Enabled = true;
        }

        private void Init() {
            this.vpnNotifyIcon = new NotifyIcon();
            this.vpnMenu = new ContextMenuStrip();
            this.exitMenuItem = new ToolStripMenuItem();

            this.vpnMenu.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.vpnMenu.Items.AddRange(new ToolStripItem[] {
            this.exitMenuItem});
            this.vpnMenu.Name = "vpnMenu";
            this.vpnMenu.Size = new System.Drawing.Size(103, 28);

            this.exitMenuItem.Name = "exitToolStripMenuItem";
            this.exitMenuItem.Size = new System.Drawing.Size(210, 24);
            this.exitMenuItem.Text = "Exit";
            this.exitMenuItem.Click += new System.EventHandler(this.exitMenuItem_Click);

            this.vpnNotifyIcon.ContextMenuStrip = this.vpnMenu;
            this.vpnNotifyIcon.Icon = new Icon(@"vpn.ico");
            this.vpnNotifyIcon.Text = "VPN Status";
            this.vpnNotifyIcon.Visible = true;

            myTimer = new Timer {
                Enabled = false,
                Interval = 3000
            };

            myTimer.Tick += new System.EventHandler(this.timerElapsed);
        }

        private void ScanNetwork() {
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

                if (line.Contains("VPN DLYA L2TP")) {
                    found = true;
                    break;
                }
            }

            if (found) {
                this.vpnNotifyIcon.Icon = new Icon(@"vpnActive.ico");
            }
            else {
                this.vpnNotifyIcon.Icon = new Icon(@"vpn.ico");
            }

        }

        private void timerElapsed(object sender, EventArgs e) {
            ScanNetwork();
        }

        private void exitMenuItem_Click(object sender, EventArgs e) {
            Exit();
        }

        private void Exit() {
            vpnNotifyIcon.Visible = false;
            myTimer.Enabled = false;

            Application.Exit();
        }
    }
}
