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
using System.Windows.Forms;
using System.Drawing;
using inSSIDer.UI.Theme;
using System.Collections.Generic;
using System.Diagnostics;

namespace inSSIDer.UI.Controls
{
    public class GripSplitContainer : SplitContainer,IThemeable
    {
        public override bool Focused
        {
            get
            {
                return false;
            }
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Image grip = Properties.Resources.longGripOff;
            if (Orientation == Orientation.Horizontal)
            {
                int x = (SplitterRectangle.Width - grip.Width) / 2;
                e.Graphics.DrawImageUnscaled(grip, x, SplitterRectangle.Top);
            }
            else
            {
                grip.RotateFlip(RotateFlipType.Rotate90FlipNone);
                int y = (SplitterRectangle.Height - grip.Height) / 2;
                e.Graphics.DrawImageUnscaled(grip, SplitterRectangle.Left, y);
            }
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            Invalidate();
        }

        #region IThemeable Members

        public void SetColorScheme(ColorScheme scheme)
        {
            Debug.WriteLine("SetColorScheme()", this.Name);
            // Apply theme to self and all child themeable controls
            ColorScheme.ApplyColorScheme(scheme, this, ColorClass.Custom);

            // First panel
            foreach (Control control in Panel1.Controls)
            {
                Debug.WriteLine("Control in first panel - " + control.GetType().Name, this.Name);
                if(control is IThemeable)
                ((IThemeable)control).SetColorScheme(scheme);
            }

            // Second panel
            foreach (Control control in Panel2.Controls)
            {
                Debug.WriteLine("Control in second panel - " + control.GetType().Name, this.Name);
                if (control is IThemeable)
                    ((IThemeable)control).SetColorScheme(scheme);
            }
        }

        #endregion
    }
}
