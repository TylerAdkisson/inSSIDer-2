﻿////////////////////////////////////////////////////////////////
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
using inSSIDer.Scanning;
using MetaGeek.Gps;
using inSSIDer.Localization;

namespace inSSIDer.UI.Controls
{
    public partial class GpsMon : UserControl
    {
        private ScanController _scanner;

        private delegate void DelUpdateView();

        public GpsMon()
        {
            InitializeComponent();
            //Show GPS must be enabled message
            lblNoGps.Visible = true;
            lblNoGps.Dock = DockStyle.Fill;
        }

        public void SetScanner(ref ScanController scanner)
        {
            _scanner = scanner;
            _scanner.GpsControl.GpsStatUpdated += GpsControl_GpsStatUpdated;
            _scanner.GpsControl.GpsMessage += GpsControl_GpsMessage;

            gpsGraph1.SetScanner(ref scanner);
            UpdateView();
        }

        private void GpsControl_GpsMessage(object sender, StringEventArgs e)
        {
            UpdateView();
        }

        private void GpsControl_GpsStatUpdated(object sender, EventArgs e)
        {
            UpdateView();
        }

        private void UpdateView()
        {
            if (InvokeRequired)
            {
                try
                {
                    Invoke(new DelUpdateView(UpdateView));
                }
                // occurs during close...
                catch (InvalidOperationException)
                {
                }
            }
            else
            {
                try
                {
                    lblNoGps.Visible = !_scanner.GpsControl.Enabled;

                    lblPortName.Text = Localizer.GetString("GpsOnPortPlusValue", _scanner.GpsControl.PortName);

                    lblLat.Text = Localizer.GetString("LatitudePlusValue",_scanner.GpsControl.MyGpsData.Latitude.ToString("F6"));
                    lblLon.Text = Localizer.GetString("LongitudePlusValue",_scanner.GpsControl.MyGpsData.Longitude.ToString("F6"));
                    lblAlt.Text = Localizer.GetString("AltitudePlusValue",_scanner.GpsControl.MyGpsData.Altitude.ToString("F2"));
                    lblSpeed.Text = Localizer.GetString("SpeedPlusValue",_scanner.GpsControl.MyGpsData.Speed.ToString("F2"));
                    lblPdop.Text = Localizer.GetString("PDOPPlusValue",_scanner.GpsControl.MyGpsData.Pdop);
                    lblHdop.Text = Localizer.GetString("HDOPPlusValue",_scanner.GpsControl.MyGpsData.Hdop);
                    lblVdop.Text = Localizer.GetString("VDOPPlusValue",_scanner.GpsControl.MyGpsData.Vdop);
                    lblFixType.Text = Localizer.GetString("FixTypePlusValue",_scanner.GpsControl.MyGpsData.FixType);
                    lblSatCount.Text = Localizer.GetString("SatellitesUVPlusValue",_scanner.GpsControl.MyGpsData.SatellitesUsed,_scanner.GpsControl.SatellitesVisible);

                    //Refresh the gps signal graph
                    gpsGraph1.Invalidate();
                }
                catch(Exception)
                {
                    
                }
            }
        }

        /// <summary>
        /// Rleases all hooked external events
        /// </summary>
        public void ReleaseEvents()
        {
            _scanner.GpsControl.GpsStatUpdated -= GpsControl_GpsStatUpdated;
            _scanner.GpsControl.GpsMessage -= GpsControl_GpsMessage;
        }
    }
}
