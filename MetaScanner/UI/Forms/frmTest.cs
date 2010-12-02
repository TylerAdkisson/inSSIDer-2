using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using inSSIDer.Misc;
using inSSIDer.Scanning;
using inSSIDer.UI.Controls;
using System.Reflection;
using MetaGeek.WiFi;

namespace inSSIDer.UI.Forms
{
    public partial class frmTest : Form , IScannerUi
    {
        public frmTest()
        {
            InitializeComponent();
            ////Tabs.Add(new Tab("News",Font));
            //Tab t = new Tab("Time Graph", Font) { Selected = true };
            //t.Controls.Add(new TimeGraph() { Dock = DockStyle.Fill });
            //tabControl1.Tabs.Add(t);

            //t = new Tab("2.4 GHz Channels", Font);
            //t.Controls.Add(new ChannelView() { Dock = DockStyle.Fill, Band = ChannelView.BandType.Band2400MHz });
            //tabControl1.Tabs.Add(t);

            //t = new Tab("5 GHz Channels", Font);
            //t.Controls.Add(new ChannelView() { Dock = DockStyle.Fill, Band = ChannelView.BandType.Band5000MHz });
            //tabControl1.Tabs.Add(t);
            //Tabs.Add(new Tab("2.4 GHz Channels", Font) { Selected = true });
            //Tabs.Add(new Tab("5 GHz Channels", Font));
            //Tabs.Add(new Tab("Filters", Font));
            //Tabs.Add(new Tab("GPS", Font));
        }

        #region IScannerUi Members

        public void Initalize(ref ScanController scanner)
        {
            
        }

        #endregion

        //private void button1_Click(object sender, EventArgs e)
        //{
        //    //Dictionary<string, object> data = new Dictionary<string, object>() { 
        //    //                                    { "Ssid", "lolwut" }, 
        //    //                                    { "Rssi", -60 }};
        //    //MetaGeek.WiFi.NetworkData t = Test<MetaGeek.WiFi.NetworkData>(data);
        //    NetworkDataObjectConverter c = new NetworkDataObjectConverter();
        //    c.Convert(null, new NetworkData(new byte[] { 0x00, 0x11, 0x22, 0x33, 0x44, 0x55 }));
        //}


        public T Test<T>(Dictionary<string,object> data)
        {
            Type t2 = typeof(T);

            T obj = Activator.CreateInstance<T>();
            //Console.WriteLine("Type is {0}", t2.FullName);

            foreach (PropertyInfo item in t2.GetProperties())
            {
                if (data.ContainsKey(item.Name))
                {
                    //Check for a sub-object
                    if (data[item.Name] is Dictionary<string, object>)
                    {
                        //object o = Test<Nullable>((Dictionary<string, object>)data[item.Name]);
                    }
                    else
                    {
                        item.SetValue(obj, Convert.ChangeType(data[item.Name], item.PropertyType), null);
                    }
                }
                //if (item.Name == "Test")
                //    item.SetValue(obj, "lol", null);
                //else
                //    item.SetValue(obj, Convert.ChangeType("1", item.PropertyType), null);
            }

            return obj;
        }

        public void Test2(Dictionary<string, object> data, Type type)
        {
            //object obj = Activator.CreateInstance(type);

            //foreach (PropertyInfo item in type.GetProperties())
            //{
            //    if (data.ContainsKey(item.Name))
            //    {
            //        //Check for a sub-object
            //        if (data[item.Name] is Dictionary<string, object>)
            //        {
            //            //Parse the type from data
            //            object o = Test2(data[item.Name] as Dictionary<string, object>, item.PropertyType);
            //            item.SetValue(obj, o, null);
            //        }
            //        else
            //        {
            //            item.SetValue(obj, Convert.ChangeType(data[item.Name], item.PropertyType), null);
            //        }
            //    }
            //}
        }

        public class test
        {
            public string Test { get; set; }

            public int Test2 { get; set; }
        }

    }
}
