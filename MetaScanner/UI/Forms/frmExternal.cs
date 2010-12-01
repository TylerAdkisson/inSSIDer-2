using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using inSSIDer.UI.Controls;

namespace inSSIDer.UI.Forms
{
    public partial class frmExternal : Form
    {
        public frmExternal()
        {
            InitializeComponent();
        }

        public void TabControl_TabRemoved(object sender, TabRemovedEventArgs args)
        {
            //if(args.AllRemoved)
            //    this.Close();
        }
    }
}
