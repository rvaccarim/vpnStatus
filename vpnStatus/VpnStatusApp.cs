using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Windows.Forms;

namespace vpnStatus {
    public class VpnStatusApp : ApplicationContext {
        private NotifyIcon vpnNotifyIcon;
        private ContextMenuStrip vpnMenu;
        private ToolStripMenuItem exitMenuItem;
        private Timer myTimer;
        private readonly List<String> vpns = new List<String>();

        public VpnStatusApp() {
            Init();
            ScanNetwork(vpns);
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

            foreach (var v in Properties.Settings.Default.VpnList.Split(',')) {
                vpns.Add(v.Trim());
            }

            myTimer = new Timer
            {
                Enabled = false,
                // in milliseconds
                Interval = 3000
            };

            myTimer.Tick += new EventHandler(this.TimerElapsed);

        }


        private void ScanNetwork(List<String> vpns) {
            bool found = false;
            string connectedVpns = "";

            foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces()) {
                if (vpns.Contains(ni.Name) && ni.OperationalStatus == OperationalStatus.Up) {
                    connectedVpns = connectedVpns + ni.Name + Environment.NewLine;
                    found = true;

                    if (vpns.Count == 1) {
                        break;
                    }
                }
            }

            if (found) {
                vpnNotifyIcon.Text = "VPN Status - Connected:" + Environment.NewLine + connectedVpns;
                this.vpnNotifyIcon.Icon = Properties.Resources.Active;
            }
            else {
                vpnNotifyIcon.Text = "VPN Status - Not connected";
                this.vpnNotifyIcon.Icon = Properties.Resources.Inactive;
            }

        }

        private void TimerElapsed(object sender, EventArgs e) {
            ScanNetwork(vpns);
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
