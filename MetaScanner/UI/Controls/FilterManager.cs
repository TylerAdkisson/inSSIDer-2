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
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using inSSIDer.Scanning;
using inSSIDer.UI.Forms;
using MetaGeek.WiFi;
using MetaGeek.WiFi.Filters;

namespace inSSIDer.UI.Controls
{
    public partial class FilterManager : UserControl
    {
        private ScanController _sc;
        //selection ignore flag - set to makes it ignore the first the auto-select when a new rows is added
        private bool _ignoreSelection = true;

        private int _selectedRow = -1;

        public FilterManager()
        {
            InitializeComponent();
        }

        public void SetScanner(ref ScanController s)
        {
            _sc = s;
            _sc.Cache.FiltersChanged += Cache_FiltersChanged;
            UpdateFilters();
        }

        private void Cache_FiltersChanged(object sender, EventArgs e)
        {
            UpdateFilters();
        }

        private void UpdateFilters()
        {
            List<DataGridViewRow> allrows = new List<DataGridViewRow>();
            allrows.AddRange(dgvFilters.Rows.Cast<DataGridViewRow>());
            foreach (Filter2 filter in _sc.Cache.Filters)
            {
                DataGridViewRow row = FindRow(filter.Id);
                if (row == null)
                {
                    //New Filter!
                    DataGridViewRow newrow = new DataGridViewRow();
                    newrow.CreateCells(dgvFilters, filter.GetData());

                    dgvFilters.Rows.Add(newrow);
                }
                else
                {
                    allrows.Remove(row);
                    //update old filters
                    row.Cells["exprColumn"].Value = filter.ToString();
                    row.Cells["checkColumn"].Value = filter.Enabled;
                }
            }
            for (int i = 0; i < dgvFilters.Rows.Count; i++)
            {
                if (allrows.Contains(dgvFilters.Rows[i])) { dgvFilters.Rows.RemoveAt(i); continue; }
            }
        }

        private DataGridViewRow FindRow(Guid id)
        {
            foreach (DataGridViewRow row2 in dgvFilters.Rows)
            {
                if ((Guid) row2.Cells["idColumn"].Value == id)
                {
                    return row2;
                }
            }
            return null;
        }

        private void DgvFiltersCellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            //Header!
            if (e.RowIndex != -1) return;
            // fill gradient background 
            LinearGradientBrush gradientBrush = new LinearGradientBrush(
                e.CellBounds,
                Color.FromArgb(202, 202, 202),
                Color.FromArgb(158, 158, 158),
                LinearGradientMode.Vertical);
            e.Graphics.FillRectangle(gradientBrush, e.CellBounds);
            gradientBrush.Dispose();

            // paint rest of cell 
            e.Paint(e.CellBounds, DataGridViewPaintParts.Border | DataGridViewPaintParts.ContentForeground);
            e.Handled = true;
        }

        private void DgvFiltersCellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            // Clicked on row selector 
            if (e.RowIndex == -1)
            {
                _ignoreSelection = true;
                return;
            }

            switch (e.ColumnIndex)
            {
                case 1:
                    if (e.Button == MouseButtons.Left)
                    {
                        //if((int) scannerView.Rows[e.RowIndex].Cells["idColumn"].Value > 20)
                        //    Console.WriteLine("Hi");
                        //AccessPoint ap =
                        //    _sc.Cache.GetApByIndex((long)scannerView.Rows[e.RowIndex].Cells["idColumn"].Value);
                        //if (ap == null) break;
                        //ap.Graph = !ap.Graph;

                        Filter2 f = _sc.Cache.GetFilterById((Guid)dgvFilters.Rows[e.RowIndex].Cells["idColumn"].Value);
                        f.Enabled = !f.Enabled;
                        dgvFilters.Rows[e.RowIndex].Cells["checkColumn"].Value = f.Enabled;
                    }
                    break;
            }

            if (dgvFilters.Rows[e.RowIndex].Selected && _selectedRow != e.RowIndex)
            {
                _selectedRow = e.RowIndex;
            }
            else if (dgvFilters.Rows[e.RowIndex].Selected && _selectedRow == e.RowIndex)
            {
                //If the selected row is the current row and is selected, deselect it.
                dgvFilters.Rows[e.RowIndex].Selected = false;
                _selectedRow = -1;
            }
        }

        private void DgvFiltersSelectionChanged(object sender, EventArgs e)
        {
            if (_ignoreSelection)
            {
                //scannerView.SelectedRows.Clear();
                foreach (DataGridViewRow row in dgvFilters.Rows)
                {
                    row.Selected = false;
                }
                _ignoreSelection = false;
            }
            // Edit/delete buttons
            SetButtonsState(dgvFilters.SelectedRows.Count > 0);
        }

        private void SetButtonsState(bool enabled)
        {
            editSelectedToolStripMenuItem.Enabled = enabled;
            deleteSelectedToolStripMenuItem.Enabled = enabled;
        }

        private void DeleteSelectedToolStripMenuItemClick(object sender, EventArgs e)
        {
            if(dgvFilters.SelectedRows.Count < 1) return;

            _sc.Cache.RemoveFilterById((Guid)dgvFilters.SelectedRows[0].Cells["idColumn"].Value);
        }

        private void CreateNewToolStripMenuItemClick(object sender, EventArgs e)
        {
            using(FormFilterBuilder form = new FormFilterBuilder())
            {
                if(form.ShowDialog(ParentForm) == DialogResult.OK && !string.IsNullOrEmpty(form.Expression))
                {
                    _sc.Cache.AddFilter(new Filter2(form.Expression));
                }
            }
        }

        private void EditSelectedToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (dgvFilters.SelectedRows.Count < 1) return;
            Filter2 f = _sc.Cache.GetFilterById((Guid)dgvFilters.SelectedRows[0].Cells["idColumn"].Value);
            if (f == null) return;

            using (FormFilterBuilder form = new FormFilterBuilder(f.ToString()))
            {
                if (form.ShowDialog(ParentForm) == DialogResult.OK && !string.IsNullOrEmpty(form.Expression))
                {
                    f.SetExpression(form.Expression);
                }
            }
            UpdateFilters();
        }

        private void Is80211NToolStripMenuItemClick(object sender, EventArgs e)
        {
            Filter2 fNew = new Filter2("IsN == True");
            _sc.Cache.AddFilter(fNew);
        }

        private void Uses40MHzChannelToolStripMenuItemClick(object sender, EventArgs e)
        {
            Filter2 fNew = new Filter2("Is40MHz == True");
            _sc.Cache.AddFilter(fNew);
        }

        private void RSsiToolStripMenuItemDropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem == null) return;
            string rssi = e.ClickedItem.Text.Split(' ')[1];
            Filter2 fNew = new Filter2("Rssi > " + rssi);
            _sc.Cache.AddFilter(fNew);
        }

        private void OnlyOpenToolStripMenuItemClick(object sender, EventArgs e)
        {
            Filter2 fNew = new Filter2("Security == None");
            _sc.Cache.AddFilter(fNew);
        }

        private void OnlySecuredToolStripMenuItemClick(object sender, EventArgs e)
        {
            Filter2 fNew = new Filter2("Security > None");
            _sc.Cache.AddFilter(fNew);
        }

        private void FilterMgr_Load(object sender, EventArgs e)
        {
            //Load last filters ro menu
            Filter2[] lastfilters = SettingsMgr.GetFilterList();
            if(lastfilters.Length > 0)
            {
                lastFiltersUsedToolStripMenuItem.Visible = true;
                lastFiltersUsedToolStripMenuItem.DropDownItems.Clear();
                ToolStripMenuItem newItem;
                foreach (Filter2 f in lastfilters)
                {
                    newItem = new ToolStripMenuItem(f.ToString().Replace("&&", "&&&&")) {Name = f.Id + "_LastFilter"};
                    lastFiltersUsedToolStripMenuItem.DropDownItems.Add(newItem);
                }
                //Add the last 2 buttons back
                lastFiltersUsedToolStripMenuItem.DropDownItems.Add(toolStripSeparator1);
                lastFiltersUsedToolStripMenuItem.DropDownItems.Add(addAllToolStripMenuItem);

            }
        }

        private void LastFiltersUsedToolStripMenuItemDropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            //Console.WriteLine(e.ClickedItem.Name);
            if(e.ClickedItem.Name.EndsWith("LastFilter"))
            {
                Filter2 fNew = new Filter2(e.ClickedItem.Text);
                _sc.Cache.AddFilter(fNew);
            }
        }

        private void AddAllToolStripMenuItemClick(object sender, EventArgs e)
        {
            foreach (ToolStripMenuItem item in lastFiltersUsedToolStripMenuItem.DropDownItems.OfType<ToolStripMenuItem>())
            {
                if(item.Name.EndsWith("LastFilter"))
                {
                    Filter2 fNew = new Filter2(item.Text);
                    _sc.Cache.AddFilter(fNew);
                }
            }
        }

        /// <summary>
        /// Rleases all hooked external events
        /// </summary>
        public void ReleaseEvents()
        {
            _sc.Cache.FiltersChanged -= Cache_FiltersChanged;
        }

        #region Age Quick Filters

        private void oneSecondToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _sc.Cache.AddFilter(new Filter2("Age <= 1"));
        }

        private void fiveSecondsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _sc.Cache.AddFilter(new Filter2("Age <= 5"));
        }

        private void tenSecondsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _sc.Cache.AddFilter(new Filter2("Age <= 10"));
        }

        private void twentySecondsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _sc.Cache.AddFilter(new Filter2("Age <= 20"));
        }

        private void thirtySecondsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _sc.Cache.AddFilter(new Filter2("Age <= 30"));
        }

        private void oneMinuteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _sc.Cache.AddFilter(new Filter2("Age <= 60"));
        }

        private void twoMinutesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _sc.Cache.AddFilter(new Filter2("Age <= 120"));
        }

        #endregion

        #region Channel Quick Filters

        private void TwoFourGHzBandToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _sc.Cache.AddFilter(new Filter2("Channel <= 14"));
        }

        private void FiveGHzBandToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            _sc.Cache.AddFilter(new Filter2("Channel >= 36"));
        }

        private void channel1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _sc.Cache.AddFilter(new Filter2("Channel == 1"));
        }

        private void channel2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _sc.Cache.AddFilter(new Filter2("Channel == 2"));
        }

        private void channel3ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _sc.Cache.AddFilter(new Filter2("Channel == 3"));
        }

        private void channel4ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _sc.Cache.AddFilter(new Filter2("Channel == 4"));
        }

        private void channel5ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _sc.Cache.AddFilter(new Filter2("Channel == 5"));
        }

        private void channel6ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _sc.Cache.AddFilter(new Filter2("Channel == 6"));
        }

        private void channel7ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _sc.Cache.AddFilter(new Filter2("Channel == 7"));
        }

        private void channel8ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _sc.Cache.AddFilter(new Filter2("Channel == 8"));
        }

        private void channel9ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _sc.Cache.AddFilter(new Filter2("Channel == 9"));
        }

        private void channel10ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _sc.Cache.AddFilter(new Filter2("Channel == 10"));
        }

        private void channel11ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _sc.Cache.AddFilter(new Filter2("Channel == 11"));
        }

        private void channel12ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _sc.Cache.AddFilter(new Filter2("Channel == 12"));
        }

        private void channel13ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _sc.Cache.AddFilter(new Filter2("Channel == 13"));
        }

        private void channel14ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _sc.Cache.AddFilter(new Filter2("Channel == 14"));
        }

        #endregion

    }
}