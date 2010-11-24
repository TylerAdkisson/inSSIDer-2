using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Design;
using System.ComponentModel.Design;
using System.ComponentModel;

namespace inSSIDer.UI.Controls.Designers
{
    public class TabControlDesigner : System.Windows.Forms.Design.ControlDesigner
    {
        const int WmLButtonDown = 0x0201;
        const int WmLButtonUp = 0x0202;

        private IToolboxService _toolboxService;
        private IDesignerHost _designerHost;

        private bool mouseOver = false;
        public TabControlDesigner()
        {
        }

        public override void Initialize(System.ComponentModel.IComponent component)
        {
            base.Initialize(component);

            _toolboxService = (IToolboxService)GetService(typeof(IToolboxService));
            _designerHost = (IDesignerHost)GetService(typeof(IDesignerHost));
        }

        protected override void OnPaintAdornments(System.Windows.Forms.PaintEventArgs pe)
        {
            //base.OnPaintAdornments(pe);
            //if (!mouseOver) return;
            //pe.Graphics.DrawRectangle(Pens.Lime, 0, 0, Control.Width, Control.Height);
        }

        protected override void OnMouseEnter()
        {
            mouseOver = true;
        }

        protected override void OnMouseLeave()
        {
            mouseOver = false;
        }

        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            if (Control.Created && m.HWnd == Control.Handle)
            {
                switch (m.Msg)
                {
                    case WmLButtonDown:
                        //Select tab
                        ((ETabControl)Control).SelectedTab = ((ETabControl)Control).GetTabFromPoint(ParseLocation(m.LParam.ToInt32()));
                        Control.Invalidate();
                        break;
                    default:
                        base.WndProc(ref m);
                        break;
                }
            }
            else
            {
                base.WndProc(ref m);
            }
        }

        public Point ParseLocation(int lParam)
        {
            Point p = new Point();
            p.X = lParam & 0xFFFF;
            p.Y = lParam >> 16;

            return p;
        }

        protected override void OnDragEnter(System.Windows.Forms.DragEventArgs de)
        {
            //base.OnDragEnter(de);

            //IDataObject data = de.Data;

            ////If the item is not a toolbox item, ignore it
            //if (!_toolboxService.IsToolboxItem(data))
            //{
            //    de.Effect = DragDropEffects.None;
            //    return;
            //}

            //ToolboxItem ti = (ToolboxItem)_toolboxService.DeserializeToolboxItem(data);
            //Type t = Type.GetType(ti.TypeName);
            //if (t == null)
            //{
            //    de.Effect = DragDropEffects.None;
            //    return;
            //}

            //if (typeof(Control).IsAssignableFrom(t))
            //    de.Effect = DragDropEffects.Copy;
            //else
            //    de.Effect = DragDropEffects.None;
        }

        protected override void OnDragDrop(DragEventArgs de)
        {
            base.OnDragDrop(de);

            //IDataObject data = de.Data;
            //if (!_toolboxService.IsToolboxItem(data)) return;

            //ToolboxItem item = mToolboxService.DeserializeToolboxItem(data, _designerHost);
            //Type t = Type.GetType(item.TypeName);

            
            //IComponent comp = _designerHost.CreateComponent(t, name);
        }
    }
}
