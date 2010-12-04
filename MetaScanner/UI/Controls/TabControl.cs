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
using inSSIDer.UI.Forms;
using System.Runtime.Remoting;
using System.Drawing.Drawing2D;

namespace inSSIDer.UI.Controls
{
//    [Designer(typeof(TabControlDesigner))]
    //[Designer(typeof(TestDesigner))]
    public partial class ETabControl : UserControl
    {
        private int TabMargin = 24;
        public List<Tab> Tabs = new List<Tab>();

        private float divider = -1;

        private Tab _lastTab;

        public event EventHandler<TabRemovedEventArgs> TabRemoved;

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

            LinearGradientBrush lgb = new LinearGradientBrush(new Point(0, 0), new Point(0, TabMargin), Color.FromArgb(245, 245, 245), Color.FromArgb(169, 169, 169));
            Brush brTab = lgb;//Brushes.DimGray;
            Brush selectedBrush = Brushes.White;

            if (Tabs.Count > 0)
            {
                //If there is only 1 tab, make sure it's selected
                if (Tabs.Count == 1)
                    Tabs[0].Selected = true;

                foreach (Tab tab in Tabs)
                {
                    top = tab.Selected ? 0 : 3;
                    str = e.Graphics.MeasureString(tab.Text, Font);
                    width = str.Width + 10;

                    brTab = tab.Selected ? selectedBrush : lgb;

                    //TODO: Adjust height for non-selected tabs?
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
                    e.Graphics.DrawString(tab.Text, Font, Brushes.Black, tab.TabBounds, new StringFormat() { LineAlignment = StringAlignment.Center, Alignment = StringAlignment.Center });

                    //e.Graphics.FillRectangle(Brushes.Green, x, 0, width, TabMargin);
                    //e.Graphics.DrawRectangle(Pens.White, x, 0, width, TabMargin);

                    //e.Graphics.DrawString(tab, Font, Brushes.White, new RectangleF(x, 0, width, TabMargin), new StringFormat() { LineAlignment = StringAlignment.Center });
                    x += width + 1.1f;

                    //Show contents if selected
                    if (tab.Selected)
                    {
                        try
                        {
                            tab.Parent = this;
                            tab.Location = new Point(0, TabMargin);
                            tab.Width = Width;
                            tab.Height = Height - TabMargin;
                            tab.Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom;
                            tab.Show();
                        }
                        catch (ObjectDisposedException ex)
                        {
                        }
                    }
                    else
                    {
                        tab.Hide();
                    }
                }
                //Draw drop divider
                if (divider > -1)
                {
                    e.Graphics.DrawLine(Pens.Red, divider, 0, divider, TabMargin);
                }
            }
            else
            {
                e.Graphics.ResetClip();

                //Draw text telling the user to drop tabs
                Font bigFont = new Font(Font.FontFamily, 14, FontStyle.Regular);
                e.Graphics.DrawString("Drop tabs here", bigFont, Brushes.Lime, new RectangleF(0, TabMargin, Width, Height - TabMargin), new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
            }
            e.Graphics.ResetClip();
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            Tab t = GetTabFromPoint(e.Location);
            if (t == null) return;
            //DeselectAllTabs();

            SelectedTab = t;
            Invalidate();

            if (e.Button == MouseButtons.Right && new Rectangle(0, 0, Width, TabMargin).Contains(e.Location))
            {
                cms1.ForeColor = SystemColors.ControlText;
                cms1.Show(PointToScreen(e.Location));
            }
        }

        public Tab GetTabFromPoint(Point point)
        {
            foreach (Tab tab in Tabs)
            {
                if (tab.TabBounds.Contains(point)) return tab;
            }
            return null;
        }

        public int GetIndexFromPoint(Point point)
        {
            int index = -1;
            foreach (Tab tab in Tabs)
            {
                if (point.X >= tab.TabBounds.Left && point.X <= tab.TabBounds.Right)
                {
                    index = Tabs.IndexOf(tab);
                }
                else if (point.X >= tab.TabBounds.Right)
                {
                    index = -1;
                }
                else
                {
                    return index;
                }
            }
            return -1;
        }

        public void MoveTab(Tab t, int index)
        {
            Tabs.Remove(t);
            Tabs.Insert(index, t);
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
                return Tabs.FirstOrDefault(t => t.Selected);
            }
            set
            {
                //If the tab we're setting is null, or already selected, ignore
                if (value == null || value == SelectedTab) return;
                //Store the last selected tab;
                _lastTab = SelectedTab;
                DeselectAllTabs();
                value.Selected = true;
                
            }
        }

        private void SelectLast()
        {
            if (_lastTab == null && Tabs.Count > 0)
            {
                SelectedTab = Tabs[0];
                return;
            }
            else if (_lastTab == null)
            {
                return;
            }
            SelectedTab = _lastTab;
            _lastTab = null;
        }

        public void RemoveTab(Tab tab)
        {
            if (tab == null) return;
            Tabs.Remove(tab);
            OnTabRemoved(Tabs.Count == 0);
        }

        private void OnTabRemoved(bool all)
        {
            if (TabRemoved == null) return;
            TabRemoved(this, new TabRemovedEventArgs(all));
        }

        //private void TabControl_QueryContinueDrag(object sender, QueryContinueDragEventArgs e)
        //{
        //    e.Action = e.EscapePressed ? DragAction.Cancel : (_mouseDown ? DragAction.Continue : DragAction.Drop);
        //}

        bool _startDrag = false;
        Point LastLocation = Point.Empty;
        Rectangle LastLocDrag = Rectangle.Empty;
        //Simply so the ref isn't SO long
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
                DragDropEffects de = DoDragDrop(t, DragDropEffects.Move);
                Console.WriteLine("EndDragDrop");
                //Only remove the tab if the drop was successful
                if (de == DragDropEffects.Move)
                {
                    RemoveTab(t);
                    SelectLast();
                }
                //this.Parent

                Invalidate();

                Console.WriteLine("DragEnd");
            }
        }

        protected override void OnDragEnter(DragEventArgs drgevent)
        {
            base.OnDragEnter(drgevent);
            if (DesignMode) return;
            //CheckDrag(drgevent, false);
            bool isTab = CheckData(drgevent.Data);
            drgevent.Effect = isTab ? DragDropEffects.Move : DragDropEffects.None;
        }

        protected override void OnDragOver(DragEventArgs drgevent)
        {
            //Console.WriteLine("DragOver");
            base.OnDragOver(drgevent);

            //Show where the tab would show up
            //int index = GetIndexFromPoint(PointToClient(new Point(drgevent.X, drgevent.Y)));
            //Tab t = Tabs[index];
            //divider = t.TabBounds.Left;
            //Invalidate();
        }

        private bool CheckData(IDataObject data)
        {
            if (!data.GetDataPresent(typeof(Tab))) return false;

            //The data is a tab, make sure it's one of ours
            try
            {
                Tab t = data.GetData(typeof(Tab)) as Tab;
                Guid id = t.Id;
                return true;
                //return Tabs.Where(tab => tab.Id == t.Id).Count() > 0;
            }
            catch (RemotingException)
            {
                //The tab came from another instance
                return false;
            }
        }

        protected override void OnDragDrop(DragEventArgs drgevent)
        {
            base.OnDragDrop(drgevent);
            if (DesignMode) return;
            Console.WriteLine("Dropped");

            IDataObject data = drgevent.Data;

            Tab t = data.GetData(typeof(Tab)) as Tab;

            if (t == null) return;
            //DeselectAllTabs();

            //Generate a new Id for the tab
            t.Id = Guid.NewGuid();

            int index = GetIndexFromPoint(PointToClient(new Point(drgevent.X, drgevent.Y)));
            if (index > -1)
                Tabs.Insert(index, t);
            else
                Tabs.Add(t);

            //Tabs.Add(t);
            SelectedTab = t;
            Invalidate();
            //CheckDrag(drgevent, true);
        }

        private void moveToExternalWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmExternal fex = new frmExternal();

            ETabControl et = new ETabControl();
            et.Parent = fex;
            et.Dock = DockStyle.Fill;
            et.Show();

            et.TabRemoved += fex.TabControl_TabRemoved;

            et.Tabs.Add(SelectedTab);

            RemoveTab(SelectedTab);

            SelectLast();

            fex.Show();

            Invalidate();

        }

    }

    public class TabRemovedEventArgs : EventArgs
    {
        public TabRemovedEventArgs(bool allRemoved)
        {
            AllRemoved = allRemoved;
        }

        public bool AllRemoved { get; private set; }
    }
}
