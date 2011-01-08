﻿//#define LOG
using System;
using System.Windows.Forms;
using inSSIDer.Misc;
using inSSIDer.Scanning;
using inSSIDer.UI.Forms;
using inSSIDer.UI.Mini;
using inSSIDer.UnhandledException;
using MetaGeek.Utils;
using inSSIDer.Localization;
using inSSIDer.Properties;
using System.Net.NetworkInformation;
using MetaGeek.WiFi.Filters;
using MetaGeek.WiFi;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;

namespace inSSIDer
{
    static class Program
    {
        //Mode of switch
        public static Utilities.SwitchMode Switching = Utilities.SwitchMode.None;
        public static Utilities.SwitchMode LastSwitch = Utilities.SwitchMode.None;
        public static bool WasScanning;

        static void InitializeExceptionHandler(UnhandledExceptionDlg exDlg)
        {
            exDlg.UserPrefChecked = false;

            // Add handling of OnShowErrorReport.
            // If you skip this then link to report details won't be showing.
            exDlg.OnShowErrorReportClick += delegate(object sender, SendExceptionClickEventArgs ar)
            {
                MessageBox.Show("[Message]\r\n" + ar.UnhandledException.Message + "\r\n\r\n"
                     + "[Version]\r\n" + Application.ProductVersion + "\r\n\r\n"
                     + "[WinVer]\r\n" + Environment.OSVersion.VersionString + "\r\n\r\n"
                     + "[Platform]\r\n" + Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE") + "\r\n\r\n"
                     + "[StackTrace]\r\n" + PathScrubber.Scrub(ar.UnhandledException.ToString()) + "\r\n\r\n");
            };

            // Add handling of OnCopytoClipbooard
            // if you skip, the button is disabled
            exDlg.OnCopyToClipboardClick += delegate(object sender, SendExceptionClickEventArgs ar)
            {
                try
                {
                    // TUT: needs to be STA apt. thread for accessing clipboard
                    System.Threading.Thread clipThread = new System.Threading.Thread(delegate()
                    {
                        String body = Localizer.GetString("ErrorDescription");
                        body += "\r\n\r\n[Description]\r\n\r\n\r\n\r\n";
                        body += "[Message]\r\n" + ar.UnhandledException.Message + "\r\n\r\n";
                        body += "[Version]\r\n" + Application.ProductVersion + "\r\n\r\n";
                        body += "[WinVer]\r\n" + Environment.OSVersion.VersionString + "\r\n\r\n";
                        body += "[Platform]\r\n" + Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE") + "\r\n\r\n";
                        body += "[StackTrace]\r\n" + PathScrubber.Scrub(ar.UnhandledException.ToString()) + "\r\n\r\n";

                        Clipboard.SetText(body);
                    });
                    clipThread.SetApartmentState(System.Threading.ApartmentState.STA);
                    clipThread.Start();
                }
                catch (Exception)
                {
                    // Ignore
                }
            };

            // Implement your sending protocol here. You can use any information from System.Exception
            exDlg.OnSendExceptionClick += delegate(object sender, SendExceptionClickEventArgs ar)
            {
                // User clicked on "Send Error Report" button:
                if (ar.SendExceptionDetails)
                {
                    String body = Localizer.GetString("ErrorDescription");
                    body += "\r\n\r\n[Description]\r\n\r\n\r\n\r\n";
                    body += "[Message]\r\n" + ar.UnhandledException.Message + "\r\n\r\n";
                    body += "[Version]\r\n" + Application.ProductVersion + "\r\n\r\n";
                    body += "[WinVer]\r\n" + Environment.OSVersion.VersionString + "\r\n\r\n";
                    body += "[Platform]\r\n" + Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE") + "\r\n\r\n";
                    body += "[StackTrace]\r\n" + PathScrubber.Scrub(ar.UnhandledException.ToString()) + "\r\n\r\n";

                    MapiMailMessage message = new MapiMailMessage(@"inSSIDer 2 Error Report", body);
                    message.Recipients.Add("error.reports@metageek.net");
                    message.ShowDialog(true);
                }

                Application.Exit();
            };
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            //var props = FlakExpressionParser.GetFilterableProperties(typeof(AccessPoint));

            //foreach (var item in props)
            //{
            //    if(item.Key[1] == "")
            //        Console.WriteLine("{0}\t{1}", item.Key[0], item.Value);
            //    else
            //        Console.WriteLine("{0}({1})\t{2}", item.Key[0], item.Key[1], item.Value);
            //}

            #region Filter Testing
            /*
            string expr = "ssId == \"fuji-2\" (&& SSID sw \"Hot Rod\" && Channel == 1";
            ParsingError pe;

            string expr2 = expr;

            expr2 = FlakExpressionParser.MiscCheck(expr2, out pe);
            if (!pe.IsError)
            {
                expr2 = FlakExpressionParser.Parenthesize(expr2, out pe);
                if (!pe.IsError)
                {
                    bool isGood = FlakExpressionParser.CheckSections(expr2, out pe);
                }
            }
            

            if (pe.IsError)
            {
                // Parse error
                switch (pe.Error)
                {
                    case ErrorType.None:
                        MessageBox.Show("No Error", "Parsing Error", MessageBoxButtons.OK);
                        break;
                    case ErrorType.ParentheseMismatch:
                        MessageBox.Show(string.Format("Parenthese count mismatch", pe.ErrorData), "Parsing Error", MessageBoxButtons.OK);
                        break;
                    case ErrorType.QuoteMismatch:
                        MessageBox.Show(string.Format("Quotation mark count mismatch near '{0}'", pe.ErrorData), "Parsing Error", MessageBoxButtons.OK);
                        break;
                    case ErrorType.SectionLengthToShort:
                        MessageBox.Show(string.Format("Expression too short: '{0}'", pe.ErrorData), "Parsing Error", MessageBoxButtons.OK);
                        break;
                    case ErrorType.SectionLengthToLong:
                        MessageBox.Show(string.Format("Expression too long: '{0}'", pe.ErrorData), "Parsing Error", MessageBoxButtons.OK);
                        break;
                    case ErrorType.PropertyNameInvalid:
                        MessageBox.Show(string.Format("Property name invalid: '{0}'", pe.ErrorData), "Parsing Error", MessageBoxButtons.OK);
                        break;
                    case ErrorType.ExpressionBlank:
                        MessageBox.Show("Expression is blank", "Parsing Error", MessageBoxButtons.OK);
                        break;
                    case ErrorType.InvalidOperator:
                        MessageBox.Show(string.Format("Invalid operator: '{0}'", pe.ErrorData), "Parsing Error", MessageBoxButtons.OK);
                        break;
                    case ErrorType.ValueNotComparable:
                        MessageBox.Show(string.Format("Value '{0}' not comparable with property '{1}'", pe.ErrorData, pe.ErrorData2), "Parsing Error", MessageBoxButtons.OK);
                        break;
                    case ErrorType.OtherError:
                    default:
                        MessageBox.Show(string.Format("Undefined error. Data: '{0}'", pe.ErrorData), "Parsing Error", MessageBoxButtons.OK);
                        break;
                }
            }
            */
            #endregion
            #region Base filter testing
            //IFilterComparable ifc = CorrectedExpressionParser.Parse("(Is40MHz == True && (macaddress sw 00:00:00 && Rssi > -60))");

            //NetworkData nd = new NetworkData(DateTime.Now, new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, "None", "Test", 6, -50, 78, "1/2/5.5/6/7/9/11/24/36/48/54", "Access Point", 0);
            //nd.NSettings =new ManagedWifi.IeParser.TypeNSettings() { Is40MHz = true };
            //AccessPoint ap = new AccessPoint(nd);

            //bool b2 = true;
            //Stopwatch sw = new Stopwatch();
            //sw.Reset();
            //sw.Start();
            //for (int i = 0; i < 100; i++)
            //{
            //    b2 &= ifc.Solve(ap);
            //}
            //sw.Stop();

            //Console.WriteLine("Time: {0}ms\tResult: {1}", sw.ElapsedMilliseconds,b2);

            //return;
            #endregion
            //TODO: Make conmmand line option to enable logging on debug builds. Like /log
#if DEBUG && LOG
            Log.Start();
#endif
            Debug.WriteLine("Hook exception handlers");
            // Create new instance of UnhandledExceptionDlg:
            // NOTE: this hooks up the exception handler functions 
            UnhandledExceptionDlg exDlg = new UnhandledExceptionDlg();
            InitializeExceptionHandler(exDlg);

            Debug.WriteLine("Check .NET configuration system");
            //Check for config system condition here
            if(!Settings.Default.CheckSettingsSystem())
            {
                //The settings system is broken, notify and exit
                MessageBox.Show(
                    Localizer.GetString("ConfigSystemError"),
                    "Error", MessageBoxButtons.OK,MessageBoxIcon.Error);
                return;
            }


            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

#if DEBUG && LOG
            frmTest ft = new frmTest();
            Thread debugThread = new Thread(() => Application.Run(ft));
            debugThread.Start();
#endif

            //Initalize the scanner object before passing it to any interface
            ScanController scanner = new ScanController();
            Exception error;

            Debug.WriteLine("Initalize ScanController");
            scanner.Initalize(out error);
            if (error != null)
            {
                //An error!
                scanner.Dispose();
                scanner = null;
                //So the error handler will catch it
                //throw ex;

                //Log it
                Log.WriteLine(string.Format("Exception message:\r\n\r\n{0}\r\n\r\nStack trace:\r\n{1}", error.Message, error.StackTrace));

                if (error is System.ComponentModel.Win32Exception)
                {
                    //The wireless system failed
                    if (Utilities.IsXp())
                    {
                        MessageBox.Show(Localizer.GetString("WlanServiceNotFoundXP"), "Error", MessageBoxButtons.OK,
                                        MessageBoxIcon.Hand);
                    }
                    else
                    {
                        MessageBox.Show(Localizer.GetString("WlanServiceNotFound7"), "Error", MessageBoxButtons.OK,
                                        MessageBoxIcon.Hand);
                    }
                }
                else
                {
                    //Any other exceptions
                    MessageBox.Show(error.Message, "Error", MessageBoxButtons.OK,
                                        MessageBoxIcon.Hand);
                }
            }

            if (scanner == null) return;

            //Start the scanning if it was last time and we have the last interface
            //Otherwise, if we only have the interface, but not scanning, just set the interface selector to the last interface.
            //TODO: Actually have the auto-start as an option. :)

            NetworkInterface netInterface = InterfaceManager.Instance.LastInterface;
            if (netInterface != null)
            {
                Debug.WriteLine("We have a last interface, start scanning with it.");
                //Set the interface
                scanner.Interface = netInterface;
                if (Settings.Default.scanLastEnabled)
                    scanner.StartScanning();
            }

            //The main form will run unless mini is specified
            IScannerUi form = null;

            Switching = Settings.Default.lastMini ? Utilities.SwitchMode.ToMini : Utilities.SwitchMode.ToMain;

            //if(Settings.Default.lastMini)
            //{
            //    Switching = Utilities.SwitchMode.ToMini;
            //    form = new FormMini();
            //    SettingsMgr.ApplyMiniFormSettings((Form)form);
            //}
            //else
            //{
            //    Switching = Utilities.SwitchMode.ToMain;
            //    form = new FormMain();
            //    SettingsMgr.ApplyMainFormSettings((Form)form);
            //}

            //Apply settings now 
            SettingsMgr.ApplyGpsSettings(scanner.GpsControl);
            

            do
            {
                //Check for switching
                switch (Switching)
                {
                    case Utilities.SwitchMode.None:
                        //We're not switching, close program
                        break;
                    case Utilities.SwitchMode.ToMain:
                        //We're switching to the main form
                        Debug.WriteLine("Switch to main form");
                        form = new FormMain();
                        SettingsMgr.ApplyMainFormSettings((Form)form);
                        break;
                    case Utilities.SwitchMode.ToMini:
                        //We're switching to the mini form
                        Debug.WriteLine("Switch to mini form");
                        form = new FormMini();
                        SettingsMgr.ApplyMiniFormSettings((Form)form);
                        break;
                }
                LastSwitch = Switching;
                //If we've switched, we don't need to get stuck in a loop
                Switching = Utilities.SwitchMode.None;

                form.Initalize(ref scanner);
                try
                {
                    Application.Run(form as Form);
                }
                catch (ObjectDisposedException)
                {

                }
                catch (AccessViolationException)
                {
                    Debug.WriteLine("AccessViolationException, attempt to recover");
                    if (Application.VisualStyleState == System.Windows.Forms.VisualStyles.VisualStyleState.NonClientAreaEnabled)
                        throw;
                    // This could be caused by visual styles
                    Application.VisualStyleState = System.Windows.Forms.VisualStyles.VisualStyleState.NonClientAreaEnabled;
                    // Trigger restart
                    Switching = LastSwitch;//Utilities.SwitchMode.ToMain;
                }

            } while (Switching != Utilities.SwitchMode.None);

            Settings.Default.lastMini = form.GetType() == typeof(FormMini);

            //GPS enabled setting
            Settings.Default.gpsEnabled = scanner.GpsControl.Enabled;
			
            // Save Filters
            SettingsMgr.SaveFilterList(scanner.Cache.Filters.ToArray());

            // Save Filters
            SettingsMgr.SaveFilterList(scanner.Cache.Filters.ToArray());

            //Save settings before exit
            Settings.Default.Save();

            //Dispose the scanner, we're done with it
            scanner.Dispose();

            Debug.WriteLine("Execution Finished, you may now close this window", "Program.Main()");
        }
    }
}
