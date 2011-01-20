////////////////////////////////////////////////////////////////
//
// Copyright (c) 2007-2011 MetaGeek, LLC
//
// Licensed under the Apache License, Version 2.0 (the "License"); 
// you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at 
//
//	http://www.apache.org/licenses/LICENSE-2.0 
//
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS, 
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
// See the License for the specific language governing permissions and 
// limitations under the License. 
//
////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;

namespace inSSIDer.UI.Forms
{
    public partial class frmTest : Form
    {
        StringWriter tw;
        System.Timers.Timer t = new System.Timers.Timer(500);

        delegate void TimerTicked(object sender, System.Timers.ElapsedEventArgs e);
        public frmTest()
        {
            InitializeComponent();
            t.Elapsed += t_Elapsed;
        }

        void t_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (InvokeRequired)
            {
                try
                {
                    Invoke(new TimerTicked(t_Elapsed), sender, e);
                }
                catch (ObjectDisposedException) { }
                catch (InvalidOperationException) { }
                return;
            }

            txtDebug.AppendText(tw.ToString());
            StringBuilder sb = tw.GetStringBuilder();
            //Clear it
            sb.Remove(0, sb.Length);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Debug.AutoFlush = true;
            tw = new StringWriter();
            TraceListener tl = new TextWriterTraceListener(tw);
            Debug.Listeners.Add(tl);
            Debug.WriteLine("Debug output console enabled");

            t.Start();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            timer1.Stop();
            t.Stop();
        }

        delegate void timertick(object sender, EventArgs e);

        private void timer1_Tick(object sender, EventArgs e)
        {
            
        }
    }
}
