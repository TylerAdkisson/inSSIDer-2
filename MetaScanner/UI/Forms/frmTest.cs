using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using inSSIDer.Misc;
using inSSIDer.Scanning;
using inSSIDer.UI.Controls;

namespace inSSIDer.UI.Forms
{
    public partial class frmTest : Form , IScannerUi
    {
        public frmTest()
        {
            InitializeComponent();
            //Tabs.Add(new Tab("News",Font));
            Tab t = new Tab("Time Graph", Font) { Selected = true };
            t.Controls.Add(new TimeGraph() { Dock = DockStyle.Fill });
            tabControl1.Tabs.Add(t);

            t = new Tab("2.4 GHz Channels", Font);
            t.Controls.Add(new ChannelView() { Dock = DockStyle.Fill, Band = ChannelView.BandType.Band2400MHz });
            tabControl1.Tabs.Add(t);

            t = new Tab("5 GHz Channels", Font);
            t.Controls.Add(new ChannelView() { Dock = DockStyle.Fill, Band = ChannelView.BandType.Band5000MHz });
            tabControl1.Tabs.Add(t);
            //Tabs.Add(new Tab("2.4 GHz Channels", Font) { Selected = true });
            //Tabs.Add(new Tab("5 GHz Channels", Font));
            //Tabs.Add(new Tab("Filters", Font));
            //Tabs.Add(new Tab("GPS", Font));
        }

        #region IScannerUi Members

        public void Initalize(ref ScanController scanner)
        {
            
        }

        #endregion

    }
}
