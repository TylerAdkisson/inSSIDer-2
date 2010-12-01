﻿////////////////////////////////////////////////////////////////
//
// Copyright (c) 2007-2010 MetaGeek, LLC
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

namespace inSSIDer.UI.Controls
{
    public class GripSplitContainer : SplitContainer
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

            if (Orientation == Orientation.Horizontal)
            {
                int x = (SplitterRectangle.Width - Properties.Resources.longGripOff.Width) / 2;
                e.Graphics.DrawImageUnscaled(Properties.Resources.longGripOff, x, SplitterRectangle.Top);
            }
            else
            {
                int y = (SplitterRectangle.Height - Properties.Resources.longGripOff.Height) / 2;
                e.Graphics.DrawImageUnscaled(Properties.Resources.longGripOff, SplitterRectangle.Left, y);
            }
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            Invalidate();
        }
    }
}
