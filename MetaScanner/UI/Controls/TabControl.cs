using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using inSSIDer.Misc;
using System.Diagnostics;
using System.Threading;
using inSSIDer.UI.Controls.Designers;

namespace inSSIDer.UI.Controls
{
//    [Designer(typeof(TabControlDesigner))]
    //[Designer(typeof(TestDesigner))]
    public partial class ETabControl : UserControl
    {
        private int TabMargin = 24;
        public List<Tab> Tabs = new List<Tab>();

        public ETabControl()
        {
            InitializeComponent();

            SetStyle(ControlStyles.UserPaint | 
                ControlStyles.AllPaintingInWmPaint | 
                ControlStyles.OptimizedDoubleBuffer, true);

            AllowDrop = true;

            //Tabs.Add(new Tab("News",Font) {Selected = true});
            //Tabs.Add(new Tab("Time Graph", Font));
            //Tabs.Add(new Tab("2.4 GHz Channels", Font) { Selected = true });
            //Tabs.Add(new Tab("5 GHz Channels", Font));
            //Tabs.Add(new Tab("Filters", Font));
            //Tabs.Add(new Tab("GPS", Font));

        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.SetClip(new RectangleF(0, 0, Width, TabMargin));
            //Clear the tab background
            e.Graphics.FillRectangle(Brushes.Black, 0, 0, Width, TabMargin);
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            e.Graphics.DrawLine(Pens.Gray, 0, TabMargin-1, Width, TabMargin-1);

            float x = 2;
            float width = 0;
            SizeF str;
            float top = 0;

            Brush brTab = Brushes.DimGray;

            if (Tabs.Count > 0)
            {
                foreach (Tab tab in Tabs)
                {
                    top = tab.Selected ? 0 : 3;
                    str = e.Graphics.MeasureString(tab.Text, Font);
                    width = str.Width + 10;

                    //TODO: Adjust height for non-selected tabs
                    tab.TabBounds = new RectangleF(x, top, width, TabMargin);

                    //Top left corner
                    e.Graphics.FillEllipse(brTab, x, top, 12, 12);

                    //Top right corner
                    e.Graphics.FillEllipse(brTab, x + width - 12, top, 12, 12);

                    //Top left to right
                    e.Graphics.FillRectangle(brTab, x + 6, top, width - 12, 12);

                    //Left Side
                    e.Graphics.FillRectangle(brTab, x, top + 6, 6, TabMargin - 6);

                    //Right Side
                    e.Graphics.FillRectangle(brTab, x + width - 6, top + 6, 6, TabMargin - 6);

                    //Main body
                    e.Graphics.FillRectangle(brTab, x + 5, top + 11, width - 10, TabMargin - 11);

                    //Text
                    //                e.Graphics.DrawString(tab.Text, Font, Brushes.Black, x + ((width / 2) - (str.Width / 2)), top + ((TabMargin / 2) - (str.Height / 2)));
                    e.Graphics.DrawString(tab.Text, Font, Brushes.White, tab.TabBounds, new StringFormat() { LineAlignment = StringAlignment.Center, Alignment = StringAlignment.Center });

                    //e.Graphics.FillRectangle(Brushes.Green, x, 0, width, TabMargin);
                    //e.Graphics.DrawRectangle(Pens.White, x, 0, width, TabMargin);

                    //e.Graphics.DrawString(tab, Font, Brushes.White, new RectangleF(x, 0, width, TabMargin), new StringFormat() { LineAlignment = StringAlignment.Center });
                    x += width + 1.0f;

                    //Show contents if selected
                    if (tab.Selected)
                    {
                        tab.Parent = this;
                        tab.Location = new Point(0, TabMargin);
                        tab.Width = Width;
                        tab.Height = Height - TabMargin;
                        tab.Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom;
                        tab.Show();
                    }
                    else
                    {
                        tab.Hide();
                    }
                }
            }
            e.Graphics.ResetClip();
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            Tab t = GetTabFromPoint(e.Location);
            if (t == null) return;
            DeselectAllTabs();

            SelectedTab = t;
            Invalidate();
        }

        public Tab GetTabFromPoint(Point point)
        {
            foreach (Tab tab in Tabs)
            {
                if (tab.TabBounds.Contains(point)) return tab;
            }
            return null;
        }

        public void DeselectAllTabs()
        {
            foreach (Tab tab in Tabs)
            {
                tab.Selected = false;
            }
        }

        public Tab SelectedTab
        {
            get
            {
                if (Tabs.Count == 0) return null; 
                return Tabs.First(t => t.Selected);
            }
            set
            {
                if (value == null) return;
                DeselectAllTabs();
                value.Selected = true;
            }
        }

        //private void TabControl_QueryContinueDrag(object sender, QueryContinueDragEventArgs e)
        //{
        //    e.Action = e.EscapePressed ? DragAction.Cancel : (_mouseDown ? DragAction.Continue : DragAction.Drop);
        //}

        bool _startDrag = false;
        Point LastLocation = Point.Empty;
        Rectangle LastLocDrag = Rectangle.Empty;
        //Simply so the ref is SO long
        Size dragSize = SystemInformation.DragSize;
        bool _mouseDown = false;

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (DesignMode) return;
            if (e.Button == MouseButtons.Left)
            {
                //Find and select the tab clicked
                Tab t = GetTabFromPoint(e.Location);
                if (t == null) return;
                SelectedTab = t;

                //Refresh the control
                Invalidate();

                _mouseDown = true;
                Console.WriteLine("MouseDown");
                _startDrag = true;
                LastLocDrag = new Rectangle(e.X - (dragSize.Width / 2),
                e.Y - (dragSize.Height / 2),
                dragSize.Width, dragSize.Height);
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            Console.WriteLine("MouseUp");
            _mouseDown = false;
            _startDrag = false;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (DesignMode) return;
            if (_startDrag && !LastLocDrag.Contains(e.Location))
            {
                Console.WriteLine("DragStart");
                _startDrag = false;

                //Drag the selected tab
                Tab t = SelectedTab;
                DoDragDrop(t, DragDropEffects.Move);

                Tabs.Remove(SelectedTab);

                Invalidate();

                if (Tabs.Count > 0)
                    SelectedTab = Tabs[0];
                
                Console.WriteLine("DragEnd");
            }
        }

        protected override void OnDragEnter(DragEventArgs drgevent)
        {
            base.OnDragEnter(drgevent);
            if (DesignMode) return;
            //CheckDrag(drgevent, false);
            drgevent.Effect = DragDropEffects.Move;
        }

        protected override void OnDragOver(DragEventArgs drgevent)
        {
            base.OnDragOver(drgevent);
            if (DesignMode) return;
            //CheckDrag(drgevent, false);
            drgevent.Effect = DragDropEffects.Move;
        }

        private void CheckDrag(DragEventArgs e, bool dropped)
        {
            //Reject if not within the tab margin
            Point pt = this.PointToClient(new Point(e.X, e.Y));

            if (!new Rectangle(0, 0, Width, TabMargin).Contains(pt))
            {
                //Console.WriteLine("NOT CONTAINED {0} {1}", pt.X, pt.Y);
                //e.AllowedEffect = DragDropEffects.None; 
                e.Effect = DragDropEffects.None;
                return;
            }
            //Console.WriteLine("CONTAINED {0} {1}", pt.X, pt.Y);
            e.Effect = DragDropEffects.Move;

            if (dropped)
            {
                IDataObject ido = e.Data;
                string data = (string)ido.GetData(typeof(string));
                Console.WriteLine("Data dropped: {0}", data);
            }
        }

        protected override void OnDragDrop(DragEventArgs drgevent)
        {
            base.OnDragDrop(drgevent);
            if (DesignMode) return;
            Console.WriteLine("Dropped");

            IDataObject data = drgevent.Data;

            Tab t = (Tab)data.GetData(typeof(Tab));

            if (t == null) return;
            //DeselectAllTabs();

            Tabs.Add(t);
            SelectedTab = t;
            Invalidate();
            //CheckDrag(drgevent, true);
        }

    }
}
