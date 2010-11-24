using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace inSSIDer.UI.Controls.Designers
{
    public class TabControlDesigner : System.Windows.Forms.Design.ControlDesigner
    {
        private bool mouseOver = false;
        public TabControlDesigner()
        {
        }

        protected override void OnPaintAdornments(System.Windows.Forms.PaintEventArgs pe)
        {
            //base.OnPaintAdornments(pe);
            if (!mouseOver) return;
            pe.Graphics.DrawRectangle(Pens.Lime, 0, 0, Control.Width, Control.Height);
        }

        protected override void OnMouseEnter()
        {
            mouseOver = true;
        }

        protected override void OnMouseLeave()
        {
            mouseOver = false;
        }
    }
}
