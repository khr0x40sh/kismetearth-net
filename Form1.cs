using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Xml;
//using System.Linq;        //NET 3.5 only
using System.Text;
using System.Windows.Forms;

namespace KismetEarth.NET
{
    public partial class Form1 : Form
    {
        static List<wirelessnetwork> masterList = new List<wirelessnetwork>();   //Master collection for XML and GPS data (never changes after population)
        static List<wirelessnetwork> tempList = new List<wirelessnetwork>();    //Storage collection for temporary use
        static List<wirelessnetwork> writeList = new List<wirelessnetwork>();   //collection for finalized items for KML Write
        static List<wirelessnetwork> blackList = new List<wirelessnetwork>();   //collection for blacklisted networks
        static List<wirelessnetwork> viewList = new List<wirelessnetwork>(); //collection only to build dataview and get info back
        static Dictionary<string, wirelessnetwork> wnet = new Dictionary<string, wirelessnetwork>();  //collection for GPS data (broken right now)
        CoordinatesForm coord = new CoordinatesForm();

        public string version_num = "1.3b";
        public string version_date = "15JUN2010";
        public int wepcount = 0;
        public int wpacount = 0;
        public int infracount = 1;
        public int adhoccount = 0;
        public int probecount = 0;
        private int reset = 0;
        public string XMLfilename;
        public string GPSfilename;
        public string KMLfilename;
        public string initDIR;
        public bool setkismet = false;
        private bool writing = false;       //bool to determine if we are writing our kml yet

        //======================================================
        //  Maybe we will move this junk later, but here is all 
        //  the stuff that needs to computed before we open the file
        //======================================================
        double wepstat;                         //WEP encrypted networks statistics
        double wpastat;                         //WPA encrypted networks statistics
        int totalnets;                          //Total networks parsed (Infrastructure, ad-hoc, probed)
        float totaltime;                        //Total War-Drive Time

        DateTime start = new DateTime();
        TimeSpan tspan = new TimeSpan();

        public Form1()
        {
            InitializeComponent();
        }

        



        static void main_cleaner()
        {
            // clean_others();
            wirelessnetwork obj = new wirelessnetwork();
            masterList.Clear();
            wnet.Clear();
            //Enter default to prevent errors as well as track WarPath
            //=====================================
            obj.BSSID = "GP:SD:TR:AC:KL:OG";
            masterList.Add(obj);
            wnet.Add(obj.BSSID, obj);
            //=====================================
        }

        #region DataView

        private void make_checklist(DataGridView list)
        {
            DataGridViewCheckBoxColumn checkboxColumn = new DataGridViewCheckBoxColumn();
            checkboxColumn.Width = 30;
            checkboxColumn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            
            list.Columns.Insert(0, checkboxColumn);

            // add checkbox header
            Rectangle rect = list.GetCellDisplayRectangle(0, -1, true);
            // set checkbox header to center of header cell. +1 pixel to position correctly.
            rect.X = rect.Location.X + 4;
            rect.Y = rect.Location.Y + 2;

            CheckBox checkboxHeader = new CheckBox();
            checkboxHeader.Name = "checkboxHeader";
            checkboxHeader.Size = new Size(18, 18);
            checkboxHeader.Location = rect.Location;
            checkboxHeader.CheckedChanged += new EventHandler(checkboxHeader_CheckedChanged);
            list.Controls.Add(checkboxHeader);
        }

        private void AutoTable()
        {
            dataGridView1.ColumnHeadersVisible = true;
            for (int i = 0; i < dataGridView1.Columns.Count; i++)
            {
                dataGridView1.Columns[i].Visible = false;
            }
            Progress_LbL.Text = "Publishing Columns...";
            Application.DoEvents();
            System.Diagnostics.Debug.WriteLine(dataGridView1.RowCount);
            dataGridView1.Columns[0].Visible = true;
            dataGridView1.Columns["SSID"].Visible = true;
            dataGridView1.Columns["BSSID"].Visible = true;
            dataGridView1.Columns["encryption"].Visible = true;
            dataGridView1.Columns["encryption"].HeaderText = "ENC";
            dataGridView1.Columns["channel"].Visible = true;
            dataGridView1.Columns["channel"].HeaderText = "CH";
            dataGridView1.Columns["maxlong"].Visible = true;
            dataGridView1.Columns["maxlat"].Visible = true;


            // Configure the DataGridView so that users can manually change 
            // only the column widths, which are set to fill mode. 
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.AllowUserToResizeRows = false;
            dataGridView1.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.AutoResizeColumns();
            Progress_LbL.Text = "Please select networks to be parsed...";
            Application.DoEvents();
            dataGridView1.DefaultCellStyle.NullValue = "N/A";
            //dataGridView1.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }

        #region other helful things for View
        private void conversion(object input)
        {
            clear_DATAtable(dataGridView1);
            make_checklist(dataGridView1);
            dataGridView1.DataSource = Converter.GenericListToDataTable(input);
            AutoTable();
        }
        private void Build_DataGrid()
        {
            ShallowCopy<wirelessnetwork>(masterList, viewList);
            conversion(viewList);
        }
        static void ShallowCopy<T>(List<T> source, List<T> dest)
        {
            dest.Clear();
            foreach (T t in source)
            {
                dest.Add(t);
            }
        }
        private void checkboxHeader_CheckedChanged(object sender, EventArgs e)
        {
            refresh_checker();
        }

        public void remove_networks(List<wirelessnetwork> a)
        {
            for (int i = a.Count - 1; i >= 0; i--)
            {
                if (a[i].remover == true)
                {
                    a.Remove(a[i]);
                }
            }
            //debugging
            string debug = a.Count.ToString();
            InfoGrid_Updater(a);
        }
        private void refresh_checker()
        {
            string test = dataGridView1.Controls[2].ToString();
            string[] strip = test.Split(':');
            if (strip[1].Contains("1"))
            {
                for (int i = 0; i < dataGridView1.RowCount; i++)
                {
                    dataGridView1[0, i].Value = ((CheckBox)dataGridView1.Controls.Find("checkboxHeader", true)[0]).Checked;

                    dataGridView1[0, i].Value = true;
                }
                dataGridView1.EndEdit();
            }
            else
            {
                for (int i = 0; i < dataGridView1.RowCount; i++)
                {
                    //dataGridView1[0, i].Value = ((CheckBox)dataGridView1.Controls.Find("checkboxHeader", true)[0]).Checked;
                    dataGridView1[0, i].Value = false;
                }
                dataGridView1.EndEdit();
            }
        }
        private void selectAll_becauseWrite()
        {
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                //dataGridView1[0, i].Value = ((CheckBox)dataGridView1.Controls.Find("checkboxHeader", true)[0]).Checked;
                dataGridView1[0, i].Value = true;
            }
            dataGridView1.EndEdit();

        }

        private void clear_DATAtable(DataGridView data)
        {
            data.DataSource = null;
            data.Columns.Clear();
        }

        private void searcher(List<wirelessnetwork> list)
        {
            string searchtext = Search_txt.Text.ToString().ToUpper();
            if (Search_txt.Text == null || Search_txt.Text == "")
            {
                revert_data();
            }
            wirelessnetwork obj = new wirelessnetwork();
            //int[] skip_me;
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                for (int j = 1; j < dataGridView1.Rows[i].Cells.Count - 1; j++)
                {
                    string temp_bssid = dataGridView1["BSSID", i].Value.ToString();
                    if (dataGridView1.Rows[i].Cells[j].Value.ToString().ToUpper().Contains(searchtext))
                    {
                        System.Diagnostics.Debug.WriteLine(searchtext + " " + dataGridView1.Rows[i].Cells["SSID"].Value.ToString());
                        obj.remover = false;
                        if (list[i].BSSID.ToUpper() == temp_bssid.ToUpper())
                        {
                            list[i].remover = obj.remover;
                        }
                        else
                        {
                            for (int k = 0; k < list.Count; k++)
                            {
                                if (list[k].BSSID.ToUpper() == temp_bssid.ToUpper())
                                {
                                    list[k].remover = obj.remover;
                                    break;
                                }
                            }
                        }
                        break;
                    }
                    else
                    {
                        obj.remover = true;
                        if (list[i].BSSID.ToUpper() == temp_bssid.ToUpper())
                        {
                           list[i].remover = obj.remover;
                        }
                        else
                        {
                            for (int k = 0; k < list.Count; k++)
                            {
                                if (list[k].BSSID.ToUpper() == temp_bssid.ToUpper())
                                {
                                    list[k].remover = obj.remover;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            remove_networks(list);
        }
        private void Search_txt_Click(object sender, EventArgs e)
        {
            Search_txt.Clear();
        }
        #endregion


        #endregion

        #region XML Parsing

        private void get_XML()
        {
            wirelessnetwork obj = new wirelessnetwork();
            OpenFileDialog openXMLDialog = new OpenFileDialog();
            openXMLDialog.Title = "Choose XML WarDrive File";
            openXMLDialog.InitialDirectory = @obj.AppPath;
            openXMLDialog.Filter = "XML files (*.xml)|*.xml|Netxml files (*.netxml)|*.netxml|All files (*.*)|*.*";
            openXMLDialog.FilterIndex = 1;
            openXMLDialog.RestoreDirectory = true;

            if (openXMLDialog.ShowDialog() == DialogResult.OK)
            {
                string kismetstring;
                string[] placeholder_KML;
                int getinitDIR = 0;
                int kismetcheck;

                getinitDIR = openXMLDialog.FileName.Length - openXMLDialog.SafeFileName.Length;

                placeholder_KML = openXMLDialog.SafeFileName.Split('.');
                KMLfilename = placeholder_KML[0];
                initDIR = openXMLDialog.FileName.Substring(0, getinitDIR);

                XMLfilename = openXMLDialog.FileName;

                kismetcheck = XMLfilename.Length - 6;
                kismetstring = XMLfilename.Substring(kismetcheck);

                if (kismetstring.ToLower() == "netxml")
                {
                    setkismet = true;
                }
            }
        }

        private void LoopXML(XmlNode nodes)          //First level of XML document that we need to examine
        {
            int networksz_counter = 0;
            foreach (XmlNode se in nodes.ChildNodes)
            {
                if (se.Name.ToUpper() == "BSSID")
                {
                    networksz_counter++;
                    if (networksz_counter<20 && networksz_counter % 10 == 0)
                    {
                        Progress_LbL.Text = "Parsing - " + se.InnerText;
                    }
                    if (networksz_counter>20 && networksz_counter<2000 && networksz_counter % 100 == 0)
                    {
                        Progress_LbL.Text = "Parsing - " + se.InnerText;
                    }
                    if (networksz_counter>2000 && networksz_counter<20000 && networksz_counter % 1000 == 0)
                    {
                        Progress_LbL.Text = "Parsing - " + se.InnerText;
                    }
                    if (networksz_counter>20000 && networksz_counter % 10000 == 0)
                    {
                        Progress_LbL.Text = "Parsing - " + se.InnerText;
                    }

                }
                if (setkismet == false)
                {
                    System.Diagnostics.Debug.WriteLine(se.Name);

                    if (se.Name == "wireless-network")   //TOP LEVEL element, I hope
                    {
                        Processwirelessnetwork(se);
                    }
                }
                else
                {
                    if (se.Name == "wireless-network")
                    {
                        ProcessWirelessKismet(se);
                    }
                }
                LoopXML(se);
                
            }
        }

        private void ProcessWirelessKismet(XmlNode nodes)
        {
            wirelessnetwork obj = new wirelessnetwork();
            XmlElement e = (XmlElement)nodes;
            #region foreach Attrib
            foreach (XmlAttribute a in e.Attributes)
            {
                switch (a.Name.ToUpper())
                {
                    case "TYPE":
                        obj.type = a.Value;
                        switch (a.Value.ToLower())
                        {
                            case "infrastructure":
                                infracount++;
                                break;
                            case "ad-hoc":
                                adhoccount++;
                                break;
                            case "probe":
                                probecount++;
                                break;
                        }
                        break;
                    case "WEP":
                        obj.wep = a.Value;
                        if (obj.wep.ToLower() == "true")
                            wepcount++;
                        break;
                    case "CLOAKED":
                        obj.cloaked = a.Value;
                        break;
                    case "FIRST-TIME":
                        obj.firsttime = a.Value;
                        break;
                    case "LAST-TIME":
                        obj.lasttime = a.Value;
                        break;
                    default:
                        break;
                    //
                }
                //Console.WriteLine("Value: " + a.Value);   //debug
            }
            #endregion
            #region foreach Node, childnodes
            foreach (XmlElement se in e.ChildNodes)
            {
                switch (se.Name.ToUpper())
                {
                    case "SSID":
                        foreach (XmlElement s1 in se.ChildNodes)
                        {
                            switch (s1.Name.ToUpper())
                            {
                                case "MAX-RATE":
                                    obj.maxrate = s1.InnerText;
                                    break;
                                case "ENCRYPTION":
                                    obj.encryption = s1.InnerText;
                                    if (s1.InnerText.Substring(0, 3).ToUpper() == "WPA")
                                    {
                                        if (s1.PreviousSibling.Name != s1.Name)
                                            wpacount++;
                                    }
                                    if (s1.InnerText.Substring(0, 3).ToUpper() == "WEP")
                                    {
                                        wepcount++;
                                    }
                                    break;
                                case "ESSID":
                                    obj.SSID_ = s1.InnerText;
                                    foreach (XmlAttribute a in s1.Attributes)
                                    {
                                        switch (a.Name.ToUpper())
                                        {
                                            case "CLOAKED":
                                                obj.cloaked = a.Value;
                                                break;
                                            default:
                                                break;
                                            //
                                        }
                                    }
                                    break;
                                case "WEAK":
                                    obj.weak = s1.InnerText;
                                    break;
                                case "DUPEIV":
                                    obj.dupeiv = s1.InnerText;
                                    break;
                                case "TOTAL":
                                    obj.total = s1.InnerText;
                                    break;
                                default:
                                    break;
                            }
                        }
                        break;
                    case "BSSID":
                        obj.BSSID = se.InnerText;
                        break;
                    case "CHANNEL":
                        obj.channel = System.Convert.ToInt16(se.InnerText);
                        break;
                    // case "MANUF":
                    //obj.encryption = se.InnerText;
                    //   break;
                    case "PACKETS":
                        //   ProcessPackets(se);
                        //    break;
                        foreach (XmlElement s1 in se.ChildNodes)
                        {
                            switch (s1.Name.ToUpper())
                            {
                                case "LLC":
                                    obj.LLC = s1.InnerText;
                                    break;
                                case "DATA":
                                    obj.data = s1.InnerText;
                                    break;
                                case "CRYPT":
                                    obj.crypt = s1.InnerText;
                                    break;
                                case "WEAK":
                                    obj.weak = s1.InnerText;
                                    break;
                                case "DUPEIV":
                                    obj.dupeiv = s1.InnerText;
                                    break;
                                case "TOTAL":
                                    obj.total = s1.InnerText;
                                    break;
                                default:
                                    break;
                            }
                            //Console.WriteLine(s1.Name + " " + s1.InnerText);
                        }
                        break;
                    case "DATASIZE":
                        obj.datasize = se.InnerText;
                        break;
                    case "WIRELESS-CLIENT":
                        wirelessclient wc = new wirelessclient();
                        foreach (XmlAttribute b in se.Attributes)
                        {
                            switch (b.Name.ToUpper())
                            {
                                case "NUMBER":
                                    wc.number = System.Convert.ToInt32(b.Value, 10);
                                    break;
                                case "FIRST-TIME":
                                    wc.cfirsttime = b.Value;
                                    break;
                                case "LAST-TIME":
                                    wc.clasttime = b.Value;
                                    break;
                                default:
                                    break;
                                //
                            }
                            //Console.WriteLine(b.Name + " : " + b.Value);   //debug
                        }
                        //ADD one client
                        foreach (XmlElement s1 in se.ChildNodes)
                        {
                            switch (s1.Name.ToUpper())
                            {
                                case "CLIENT-MAC":
                                    wc.clientmac = s1.InnerText;
                                    break;
                                case "CLIENT-IP-ADDRESS":
                                    wc.ciprange = s1.InnerText;
                                    break;
                                //case "DATASIZE":
                                //    obj.datasize = s1.InnerText;
                                //    break;
                                default:
                                    break;
                            }

                        }
                        obj.Addwirelessclient(wc);
                        break;
                    case "IP-ADDRESS":
                        //Console.WriteLine("Found: " + se.Name);
                        foreach (XmlAttribute b in se.Attributes)
                        {
                            switch (b.Name.ToUpper())
                            {
                                case "TYPE":
                                    obj.iptype = b.Value;
                                    break;
                                default:
                                    break;
                            }
                            //Console.WriteLine("IP-TYPE: " + b.Value);
                        }
                        break;
                    case "IP-RANGE":
                        obj.iprange = se.InnerText;
                        //Console.WriteLine("IP-RANGE: " + se.InnerText);
                        break;
                    default:
                        break;
                    // 
                }
                //Console.WriteLine(se.Name + " " + se.InnerText);   //debug
                //Processwirelessnetwork(se);
            }
            #endregion
            masterList.Add(obj);
            Progress_LbL.Text = "Parsing - Network: " + obj.BSSID.ToString();
            Application.DoEvents();
            wnet.Add(obj.BSSID, obj);
        }

        private void Processwirelessnetwork(XmlNode nodes)
        {
            wirelessnetwork obj = new wirelessnetwork();
            //obj.count = 0;
            //Console.WriteLine("Element found");
            XmlElement e = (XmlElement)nodes;
            //Console.WriteLine(e.ChildNodes.Count);

            foreach (XmlAttribute a in e.Attributes)
            {
                switch (a.Name.ToUpper())
                {
                    case "TYPE":
                        obj.type = a.Value;
                        switch (a.Value.ToLower())
                        {
                            case "infrastructure":
                                infracount++;
                                break;
                            case "ad-hoc":
                                adhoccount++;
                                break;
                            case "probe":
                                probecount++;
                                break;
                        }
                        break;
                    case "WEP":
                        obj.wep = a.Value;
                        if (obj.wep.ToLower() == "true")
                            wepcount++;
                        break;
                    case "CLOAKED":
                        obj.cloaked = a.Value;
                        break;
                    case "FIRST-TIME":
                        obj.firsttime = a.Value;
                        break;
                    case "LAST-TIME":
                        obj.lasttime = a.Value;
                        break;
                    default:
                        break;
                    //
                }
                //Console.WriteLine("Value: " + a.Value);   //debug
            }
            foreach (XmlElement se in e.ChildNodes)
            {
                switch (se.Name.ToUpper())
                {
                    case "SSID":
                        obj.SSID_ = se.InnerText;
                        //Console.WriteLine(se.Name + " " + se.Value);
                        break;
                    case "BSSID":
                        obj.BSSID = se.InnerText;
                        break;
                    case "CHANNEL":
                        obj.channel = System.Convert.ToInt16(se.InnerText);
                        break;
                    case "MAXRATE":
                        obj.maxrate = se.InnerText;
                        break;
                    case "MAXSEENRATE":
                        obj.maxseenrate = se.InnerText;
                        break;
                    case "ENCRYPTION":
                        obj.encryption = se.InnerText;
                        if (se.InnerText.ToUpper() == "WPA")
                        {
                            wpacount++;
                        }
                        break;
                    case "PACKETS":
                        //   ProcessPackets(se);
                        //    break;
                        foreach (XmlElement s1 in se.ChildNodes)
                        {
                            switch (s1.Name.ToUpper())
                            {
                                case "LLC":
                                    obj.LLC = s1.InnerText;
                                    break;
                                case "DATA":
                                    obj.data = s1.InnerText;
                                    break;
                                case "CRYPT":
                                    obj.crypt = s1.InnerText;
                                    break;
                                case "WEAK":
                                    obj.weak = s1.InnerText;
                                    break;
                                case "DUPEIV":
                                    obj.dupeiv = s1.InnerText;
                                    break;
                                case "TOTAL":
                                    obj.total = s1.InnerText;
                                    break;
                                default:
                                    break;
                            }
                            //Console.WriteLine(s1.Name + " " + s1.InnerText);
                        }
                        break;
                    case "DATASIZE":
                        obj.datasize = se.InnerText;
                        break;
                    case "WIRELESS-CLIENT":
                        wirelessclient wc = new wirelessclient();
                        foreach (XmlAttribute b in se.Attributes)
                        {
                            switch (b.Name.ToUpper())
                            {
                                case "NUMBER":
                                    wc.number = System.Convert.ToInt32(b.Value, 10);
                                    break;
                                case "FIRST-TIME":
                                    wc.cfirsttime = b.Value;
                                    break;
                                case "LAST-TIME":
                                    wc.clasttime = b.Value;
                                    break;
                                default:
                                    break;
                                //
                            }
                            //Console.WriteLine(b.Name + " : " + b.Value);   //debug
                        }
                        //ADD one client
                        foreach (XmlElement s1 in se.ChildNodes)
                        {
                            switch (s1.Name.ToUpper())
                            {
                                case "CLIENT-MAC":
                                    wc.clientmac = s1.InnerText;
                                    break;
                                case "CLIENT-IP-ADDRESS":
                                    wc.ciprange = s1.InnerText;
                                    break;
                                //case "DATASIZE":
                                //    obj.datasize = s1.InnerText;
                                //    break;
                                default:
                                    break;
                            }

                        }
                        obj.Addwirelessclient(wc);
                        break;
                    case "IP-ADDRESS":
                        //Console.WriteLine("Found: " + se.Name);
                        foreach (XmlAttribute b in se.Attributes)
                        {
                            switch (b.Name.ToUpper())
                            {
                                case "TYPE":
                                    obj.iptype = b.Value;
                                    break;
                                default:
                                    break;
                            }
                            //Console.WriteLine("IP-TYPE: " + b.Value);
                        }
                        break;
                    case "IP-RANGE":
                        obj.iprange = se.InnerText;
                        //Console.WriteLine("IP-RANGE: " + se.InnerText);
                        break;
                    default:
                        break;
                    // 
                }
                //Console.WriteLine(se.Name + " " + se.InnerText);   //debug
                //Processwirelessnetwork(se);

            }

            masterList.Add(obj);
            Progress_LbL.Text = "Parsing - Network: " + obj.BSSID.ToString();
            Application.DoEvents();
            wnet.Add(obj.BSSID, obj);
        }

        private void run_XML_parse()
        {
            Progress_LbL.Text = "Parsing - " + XMLfilename;
            //Load chosen XML to be parsed
            XmlDocument x = new XmlDocument();
            x.Load(XMLfilename);

            //Loop the XML to extract vital data
            LoopXML(x);
        }
        #endregion

        #region GPS_parsing
        public void LoopGPS(XmlNode nodes)
        {
            int networksz_counter = 0;
            foreach (XmlNode n in nodes.ChildNodes)
            {

                if (n.Name == "gps-point")
                {
                    int gps_bssid_index = n.OuterXml.ToString().IndexOf("bssid=\"");
                    string[] temp = n.OuterXml.ToString().Substring(gps_bssid_index, 24).Split('\"');
                    string gps_bssid = temp[1];
                    System.Diagnostics.Debug.WriteLine(n.OuterXml);
                    if (networksz_counter<20)
                    {
                        Progress_LbL.Text = "Parsing GPS Point for: " + gps_bssid;
                    }
                    else 
                    {
                        if (networksz_counter<200 && networksz_counter%10==0)
                            Progress_LbL.Text = "Parsing GPS Point for: " + gps_bssid;
                        if (networksz_counter>200 && networksz_counter%100==0 && networksz_counter<2000)
                            Progress_LbL.Text = "Parsing GPS Point for: " + gps_bssid;
                        if (networksz_counter > 2000 && networksz_counter % 1000 == 0)
                            Progress_LbL.Text = "Parsing GPS Point for: " + gps_bssid;
                    }
                    Application.DoEvents();
                    ProcessGPS(n);
                }
                LoopGPS(n);
                networksz_counter++;
            }
        }

        //                    (XmlNodeList nodes)
        public void ProcessGPS(XmlNode nodes)
        {
            // *** Read the xml-formatted GPS file ***
            gpsdata obj2 = new gpsdata();


            XmlElement element = (XmlElement)nodes;


            foreach (XmlAttribute attrib in element.Attributes)
            {
                switch (attrib.Name.ToUpper())
                {
                    case "BSSID":
                        obj2.bssid = attrib.Value;
                        break;
                    case "SOURCE":
                        obj2.souce = attrib.Value;
                        break;
                    case "TIME-SEC":
                        obj2.timesec = System.Convert.ToInt64(attrib.Value, 10);
                        break;
                    case "TIME-USEC":
                        obj2.timeusec = System.Convert.ToInt64(attrib.Value, 10);
                        break;
                    case "LAT":
                        obj2.latitude = System.Convert.ToDouble(attrib.Value);
                        break;
                    case "LON":
                        obj2.longitude = System.Convert.ToDouble(attrib.Value);
                        break;
                    case "ALT":
                        obj2.altitude = System.Convert.ToDouble(attrib.Value);
                        break;
                    case "SPD":
                        obj2.spd = attrib.Value;
                        break;
                    case "HEADING":
                        obj2.heading = attrib.Value;
                        break;
                    case "FIX":
                        obj2.fix = attrib.Value;
                        break;
                    case "SIGNAL":
                        if (setkismet == false)
                        {
                            obj2.signal = System.Convert.ToInt16(attrib.Value, 10);
                        }
                        else
                        {
                            //do nothing
                        }
                        break;
                    case "SIGNAL_DBM":
                        obj2.signal = System.Convert.ToInt16(attrib.Value, 10);
                        break;
                    case "NOISE":
                        obj2.noise = System.Convert.ToInt16(attrib.Value, 10);
                        break;
                    default:
                        break;
                }
            }
            wirelessnetwork n;
            if (obj2.bssid == "FF:FF:FF:FF:FF:FF")
            {
                obj2.bssid = obj2.souce;
            }
            try
            {
                //masterList.Add(n);
                n = wnet[obj2.bssid];
            }
            catch
            {
                Console.WriteLine("Null data for BSSID: " + obj2.bssid);
                n = new wirelessnetwork();
                n.BSSID = obj2.bssid;
                n.SSID_ = "no ssid";
                n.channel = 0;
                n.encryption = "unknown";
                masterList.Add(n);
                wnet.Add(n.BSSID, n);
                //n = wnet["GP:SD:TR:AC:KL:OG"]; 
            }
            n.AddGPSPoints(obj2.latitude, obj2.longitude, obj2.altitude, obj2.signal, obj2.timesec, obj2.bssid);
        }

        public void GPSDTRACKLOG_unduper()
        {
            for (int i = 0; i < masterList[0].gpspoints.Count - 1; i++)
            {
                if (i == masterList[0].gpspoints.Count - 1 || masterList[0].gpspoints[i].latitude == 200)
                {
                    //do nothing... for now
                }
                else
                {

                    foreach (var noo in masterList[0].gpspoints)
                    {
                        if (noo.latitude == masterList[0].gpspoints[i].latitude && noo.longitude == masterList[0].gpspoints[i].longitude)
                        {
                            if (noo.latitude == masterList[0].gpspoints[i - 1].latitude && noo.longitude == masterList[0].gpspoints[i - 1].longitude)
                            {
                            }
                        }

                    }
                    //if (masterList[0].gpspoints[i].latitude == masterList[0].gpspoints[i + 1].latitude && masterList[0].gpspoints[i].longitude == masterList[0].gpspoints[i + 1].longitude)
                    //{
                    //   masterList[0].gpspoints[i + 1].latitude = 200;
                    //   masterList[0].gpspoints[i + 1].longitude = 200;
                    //   i++;
                    //}
                }
            }

        }

        private void run_GPSparse()
        {
            Progress_LbL.Text = "Parsing - " + GPSfilename;
            XmlDocument doc = new XmlDocument();
            doc.Load(GPSfilename);
            LoopGPS(doc);
        }
        private void get_GPS()
        {
            OpenFileDialog openGPSDialog = new OpenFileDialog();
            openGPSDialog.Title = "Choose XML WarDrive File";
            openGPSDialog.InitialDirectory = @initDIR;
            openGPSDialog.Filter = "GPS files (*.gps)|*.gps|Gpsxml files (*.gpsxml)|*.gpsxml|All files (*.*)|*.*";
            openGPSDialog.FilterIndex = 1;
            openGPSDialog.RestoreDirectory = true;

            if (openGPSDialog.ShowDialog() == DialogResult.OK)
            {
                GPSfilename = openGPSDialog.FileName;
            }
        }
        #endregion

        #region KML_Items
        #region Writing_KML
        private void runKML()
        {
            if (writeList.Count < 2)
            {
                DialogResult dlgResult = MessageBox.Show("No networks selected or only WarDrive Path selected.\nCreate KML with networks currently in view?", "Continue?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dlgResult == DialogResult.Yes)
                {
                    selectAll_becauseWrite();
                    //ShallowCopy(masterList, viewList);
                    int test = viewList.Count;
                    SaveKML_worker(viewList);
                    remove_networks(viewList);
                    ShallowCopy<wirelessnetwork>(viewList, writeList);
                    runKML();
                }
                else if (dlgResult == DialogResult.No)
                {
                    //Do Nothing   
                }
            }
            else
            {
                ShallowCopy<wirelessnetwork>(viewList, writeList);
                string debug = writeList[0].SSID_.ToString();
                get_KML();

                WriteKML_new(KMLfilename);
            }
            revert_data();
        }
        private void get_KML()
        {
            SaveFileDialog saveKMLDialog = new SaveFileDialog();
            saveKMLDialog.Title = "Choose KML Save Name";
            saveKMLDialog.InitialDirectory = @initDIR;
            saveKMLDialog.DefaultExt = "kml";
            saveKMLDialog.Filter = "KML files (*.kml)|*.kml";
            saveKMLDialog.FileName = KMLfilename;
            saveKMLDialog.FilterIndex = 2;
            // Adds a extension if the user does not
            saveKMLDialog.AddExtension = true;

            saveKMLDialog.RestoreDirectory = true;

            if (saveKMLDialog.ShowDialog() == DialogResult.OK)
            {
                //MessageBox.Show("Selected: " + saveKMLDialog.FileName);
                KMLfilename = saveKMLDialog.FileName;
                //richTextBox1.Text = "Saving to " + saveKMLDialog.FileName + "...\r\n";
            }
            else
            {
                MessageBox.Show("You hit cancel or closed the dialog.");
            }
        }
        private void WriteKML_new(string savefilename)
        {
            try
            {
                // *** Write to file ***

                // Specify file, instructions, and privelegdes
                FileStream file = new FileStream(savefilename, FileMode.Create, FileAccess.Write);

                // Create a new stream to write to the file
                StreamWriter sw = new StreamWriter(file);

                Stats_builder(writeList);
                // Write KML header information pre-wireless networks
                sw.Write("<?xml version='1.0' encoding='UTF-8'?>" + "\r\n");
                sw.Write("<kml xmlns='http://www.opengis.net/kml/2.2'>" + "\r\n");
                sw.Write("<Document>\r\n");

                sw.Write("<name>" + tspan.Hours + " hrs, " + tspan.Minutes + " min, " + tspan.Seconds + " sec.  " + infracount + " Infrastructure, " + adhoccount + " adhoc, " + probecount + " probes (" + totalnets + " total parsed) " + start.DayOfWeek + " - " + start + "</name>\r\n");

                sw.Write("<visibility>1</visibility><description>encrypted: " + String.Format("{0:0.##}", wepstat) + "% (" + String.Format("{0:.##}", wpastat) + "% WPA)</description>" + "\r\n");
                sw.Write("<LookAt>" + "\r\n");
                sw.Write("\t" + "<longitude>" + String.Format("{0:0.######}", writeList[0].gpspoints[0].longitude) + "</longitude>" + "\r\n");
                sw.Write("\t" + "<latitude>" + String.Format("{0:0.######}", writeList[0].gpspoints[0].latitude) + "</latitude>" + "\r\n");
                sw.Write("\t" + "<range>1000</range><tilt>0</tilt><heading>0</heading>" + "\r\n");
                sw.Write("</LookAt>" + "\r\n");
                //sw.Write("<Placemark>" + "\r\n");
                sw.Write("<Folder>" + "\r\n");
                sw.Write("<name>Overall GPS Drive</name>\r\n");
                sw.Write("<Placemark>\r\n");
                sw.Write("<name>Route Driven</name>\r\n");
                sw.Write("<visibility>0</visibility>" + "\r\n");
                sw.Write("\t" + "<Style><LineStyle>" + "<color>BB00FF00</color><width>3</width></LineStyle></Style>" + "\r\n");
                sw.Write("\t" + "<LineString>" + "<tessellate>1</tessellate><coordinates>" + "\r\n");
                foreach (var noo in writeList[0].gpspoints)
                {
                    sw.Write(String.Format("{0:0.######}", noo.longitude) + "," + String.Format("{0:0.######}", noo.latitude) + "," + String.Format("{0:0.######}", noo.altitude) + " ");
                }
                sw.Write("</coordinates>" + "\r\n" + "</LineString>" + "\r\n" + "</Placemark>\r\n");
                sw.Write("<Placemark>\r\n<name>START</name><visibility>0</visibility>\r\n");
                sw.Write("<Style>\r\n<IconStyle>\r\n<scale>.65</scale>\r\n");
                sw.Write("<Icon><href>root://icons/palette-2.png</href>\r\n");
                sw.Write("</Icon></IconStyle></Style>\r\n");
                sw.Write("<Point>\r\n<coordinates>");
                sw.Write(String.Format("{0:0.######}", writeList[0].gpspoints[0].longitude) + ", " + String.Format("{0:0.######}", writeList[0].gpspoints[0].latitude) + ", " + String.Format("{0:0.######}", writeList[0].gpspoints[0].altitude));
                sw.Write(" </coordinates>\r\n</Point>\r\n</Placemark>\r\n");
                sw.Write("<Placemark>\r\n<name>FINISH</name><visibility>0</visibility>\r\n");
                sw.Write("<Style>\r\n<IconStyle>\r\n<scale>.65</scale>\r\n");
                sw.Write("<Icon><href>root://icons/palette-2.png</href>\r\n");
                sw.Write("</Icon></IconStyle></Style>\r\n");
                sw.Write("<Point>\r\n<coordinates>");
                sw.Write(String.Format("{0:0.######}", writeList[0].gpspoints[writeList[0].gpspoints.Count - 1].longitude) + ", " + String.Format("{0:0.######}", writeList[0].gpspoints[writeList[0].gpspoints.Count - 1].latitude) + ", " + String.Format("{0:0.######}", writeList[0].gpspoints[writeList[0].gpspoints.Count - 1].altitude));
                sw.Write(" </coordinates>\r\n</Point>\r\n</Placemark>\r\n");
                sw.Write("</Folder>\r\n");

                // Cycle all networks now...
                foreach (var noo in writeList)
                {
                    if (noo.BSSID == "GP:SD:TR:AC:KL:OG" || noo.gpspoints.Count == 0)
                    {
                        //do nothing
                        if (noo.BSSID != "GP:SD:TR:AC:KL:OG")
                        {
                            Console.WriteLine(noo.BSSID);
                        }
                    }
                    else
                    {
                        totaltime = noo.gpspoints[noo.gpspoints.Count - 1].timesec - noo.gpspoints[0].timesec;
                        TimeSpan t1 = TimeSpan.FromSeconds(totaltime);
                        sw.Write("<Placemark>\r\n<name>" + noo.SSID_ + "</name>\r\n<description><![CDATA[seen for " + t1.Hours + " hrs " + t1.Minutes + " min " + t1.Seconds + " sec <br>first seen: " + noo.firsttime + "<br>last seen: " + noo.lasttime + "<br><hr>\r\n");
                        sw.Write("Net Type: " + noo.type + "<br>\r\n");
                        sw.Write("BSSID: " + noo.BSSID + "<br>\r\n");
                        sw.Write("channel: " + noo.channel + "<br>\r\n");
                        sw.Write("<font color=");
                        if (noo.encryption == "None" || noo.encryption == "unknown")
                        {
                            sw.Write("\"red\"");
                        }
                        else
                        {
                            sw.Write("\"green\"");
                        }
                        sw.Write(">encryption: " + noo.encryption + "</font><br>\r\n");
                        sw.Write("cloaked: " + noo.cloaked + "<br>\r\n");
                        sw.Write("max rate: " + noo.maxrate + "<br>\r\n");
                        sw.Write("max signal: " + noo.maxsignal + "<hr>\r\n");
                        sw.Write("<b>GPS coordinates</b><br>\r\n");
                        sw.Write("first-seen: " + String.Format("{0:0.######}", noo.gpspoints[0].latitude) + " lat, " + String.Format("{0:0.######}", noo.gpspoints[0].longitude) + " lon, " + String.Format("{0:0.######}", noo.gpspoints[0].altitude) + " alt<br>\r\n");
                        sw.Write("last-seen: " + String.Format("{0:0.######}", noo.gpspoints[noo.gpspoints.Count - 1].latitude) + " lat, " + String.Format("{0:0.######}", noo.gpspoints[noo.gpspoints.Count - 1].longitude) + " lon, " + String.Format("{0:0.######}", noo.gpspoints[noo.gpspoints.Count - 1].altitude) + " alt<hr>\r\n");
                        sw.Write("<b>captured packets</b><br>\r\n");
                        sw.Write("LLC: " + noo.LLC + "<br>\r\n");
                        sw.Write("data: " + noo.data + "<br>\r\n");
                        sw.Write("crypt: " + noo.crypt + "<br>\r\n");
                        sw.Write("weak: " + noo.weak + "<br>\r\n");
                        sw.Write("dupeiv: " + noo.dupeiv + "<br>\r\n");
                        sw.Write("total: " + noo.total + "<br>\r\n");
                        sw.Write("total datasize captured: " + noo.datasize + "<hr>\r\n");
                        sw.Write("<b>captured " + noo.cli.Count + " attached clients</b><br>\r\n");
                        if (noo.cli.Count > 0)
                        {
                            //if we have clients then we put this code in, if not, leave out
                            sw.Write("<ul>\r\n");
                            foreach (var c in noo.cli)
                            {
                                sw.Write("<li>MAC: " + c.clientmac + "<br>IP: " + c.ciprange + "</li>\r\n");
                            }
                            sw.Write("</ul>");
                        }
                        sw.Write("]]></description>\r\n");
                        sw.Write("<View>\r\n");
                        sw.Write("<longitude>" + noo.maxlong + "</longitude>\r\n");
                        sw.Write("<latitude>" + noo.maxlat + "</latitude>\r\n");
                        sw.Write("</View>\r\n");
                        // Added for SSID searching and lon lat limiters.  NOTE: These do not exclude data from kml, just from default visibility.

                        sw.WriteLine("<visibility>" + noo.visibility + "</visibility>");

                        //sw.Write("\r\n");
                        sw.Write("<Style><Icon>" + noo.customicon + "</Icon></Style>\r\n<Point>\r\n");
                        sw.Write("<coordinates>" + noo.maxlong + ", " + noo.maxlat + ", " + noo.maxalt + "</coordinates>\r\n</Point>\r\n");
                        sw.Write("</Placemark>\r\n");

                        if (noo.cli.Count > 0)
                        {
                            sw.Write("<Folder>\r\n<name>Clients</name>\r\n");
                            sw.Write("<visibility>0</visibility>\r\n<open>0</open>\r\n");
                            foreach (var c in noo.cli)
                            {
                                sw.Write("<Placemark>\r\n<name>" + c.clientmac + "</name>\r\n</Placemark>\r\n");
                            }
                            sw.Write("</Folder>\r\n");
                        }

                        sw.Write("<Folder>\r\n<name>Plotted Signal</name>\r\n");
                        sw.Write("<visibility>0</visibility>\r\n<open>0</open>\r\n");
                        foreach (var g in noo.gpspoints)
                        {
                            sw.Write("<Placemark>\r\n<name>" + g.signal + "</name>\r\n<visibility>0</visibility>\r\n");
                            sw.Write("<Style>\r\n<IconStyle>\r\n<color>" + g.color + "</color><scale>" + g.scale + "</scale>\r\n");
                            sw.Write("<Icon><href>" + noo.AppPath + "whitecircle.png</href></Icon>\r\n</IconStyle></Style>\r\n");
                            sw.Write("<Point>\r\n<coordinates>" + String.Format("{0:0.######}", g.longitude) + "," + String.Format("{0:0.######}", g.latitude) + ", " + String.Format("{0:0.######}", g.altitude) + "</coordinates>\r\n</Point>\r\n</Placemark>\r\n");
                            //sw.Write(noo.longitude + "," + noo.latitude + "," + noo.altitude + " ");
                        }
                        sw.Write("</Folder>\r\n");
                    }
                }
                sw.Write("</Document>\r\n</kml>");
                // Close StreamWriter
                sw.Close();

                // Close file
                file.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show("There was an unexpected error... Exiting..." + e.ToString());
                Application.Exit();
            }
        }
        #endregion

        #region Buttons_Workers


        private void SaveKML_worker(List<wirelessnetwork> list)
        {
            string temp_bssid;
            wirelessnetwork obj = new wirelessnetwork();
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                //temporary variables to store items
                temp_bssid = dataGridView1["BSSID", i].Value.ToString();


                //Set Selected Cells to true to prevent weird bug?
                if (dataGridView1.Rows[i].Cells[0].Selected == true)
                    dataGridView1[0, i].Value = true;

                //Set null to false to prevent other weird bug
                if (dataGridView1[0, i].Value == null)
                    dataGridView1[0, i].Value = false;
                string bullsh = dataGridView1[0, i].Value.ToString().ToLower();
                //99.9% of the time this will ensure data integrity... during a sort
                if (temp_bssid.ToUpper().Contains("GP:SD"))
                {
                    dataGridView1[0, i].Value = true;
                    bullsh = "true";
                }
                if (bullsh == "true")
                {
                    obj.remover = false;
                }
                else
                {
                    dataGridView1[0, i].Value = false;
                    obj.remover = true;
                }
                //Test to ensure BSSID is the same as network we selected (in case of sort)
                if (list[i].BSSID.ToUpper() == temp_bssid.ToUpper())
                {
                    list[i].remover = obj.remover;
                }
                else
                {
                    //Long way of doing things, I know, but I don't like static keys in case of duplicate BSSIDs
                    for (int j = 0; j < list.Count; j++)
                    {
                        if (list[j].BSSID.ToUpper() == temp_bssid.ToUpper())
                        {
                            list[j].remover = obj.remover;
                            break;
                        }
                    }
                }
            }
        }


        #endregion

        





        #endregion

        

        private void RunParse()
        {
            main_cleaner();
            get_XML();
            get_GPS();
            if (XMLfilename == null || GPSfilename == null)
            {
                MessageBox.Show("Please Select an XML and a matching GPS file!");
            }
            else
            {
                run_XML_parse();
                run_GPSparse();
                Build_DataGrid();
                fill_InfoGrid();
            }
        }
        #region toolStrip_Btn
        private void ParseNew_toolBtn_Click(object sender, EventArgs e)
        {
            clear_DATAtable(dataGridView1);
            RunParse();
        }

        private void WriteKML_toolstripBtn_Click(object sender, EventArgs e)
        {
            try
            {
                SaveKML_worker(viewList);
                ShallowCopy<wirelessnetwork>(viewList, writeList);
                remove_networks(writeList);
                runKML();
            }
            catch (Exception e1)
            {
                MessageBox.Show("No networks were selected...\n Please Try Again" + e1);
            }
        }

        private void Revert_Tool_Btn_Click(object sender, EventArgs e)
        {
            revert_data();
        }

        private void revert_data()
        {

            clear_DATAtable(dataGridView1);
            ShallowCopy<wirelessnetwork>(masterList, viewList);
            conversion(viewList);
            reset = 0;
        }

        private void Refresh_tool_Btn_Click(object sender, EventArgs e)
        {
            SaveKML_worker(viewList);
            remove_networks(viewList);
            clear_DATAtable(dataGridView1);
            conversion(viewList);
        }
        #endregion

        #region Search Button
        private void Search_txt_TextChanged_1(object sender, EventArgs e)
        {
            search_timer.Enabled = false;
            search_timer.Enabled = true;
        }
        private void search_timer_Tick(object sender, EventArgs e)
        {
            search_timer.Enabled = false;
            searcher(viewList);
            clear_DATAtable(dataGridView1);
            conversion(viewList);
        }
        private void Search_txt_Click_1(object sender, EventArgs e)
        {
            Search_txt.Clear();
        }
        #endregion

        #region File
        private void parsenewMenu_Click(object sender, EventArgs e)
        {
            main_cleaner();
            ParseNew_toolBtn_Click(sender, e);
        }

        private void runKMLMenu_Click(object sender, EventArgs e)
        {
            runKML();
            MessageBox.Show("Finished!");
        }
        private void ConfigMenu_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Reserved for future use...  sorry!");
        }
        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you SURE you want to QUIT?", "Exit", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                Application.Exit();
            }
        }

        #endregion
        #region View
        #region Sort
        private void allToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Revert_Tool_Btn_Click(sender, e);
        }

        private void selectedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Refresh_tool_Btn_Click(sender, e);
        }
        #endregion
        private void debugToolStripMenuItem_Click(object sender, EventArgs e)
        {
            conversion(writeList);
        }


        #endregion

        #region Help

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("KismetEarth.NET\nVersion: " + version_num + "\nReleased: " + version_date + "\n" + "Author: ktolbert5@gmail.com");
        }

        #endregion

        

        #region TreeView (InfoPane)
        private void fill_InfoGrid()
        {
            string[] xml_temp;
            string[] gps_temp;
            if (XMLfilename.Contains("\\"))
            {
                xml_temp = XMLfilename.Split('\\');
                gps_temp = GPSfilename.Split('\\'); 
            }
            else
            {
                xml_temp = XMLfilename.Split('/');
                gps_temp = GPSfilename.Split('/');
            }
            clear_DATAtable(InfoGrid);
            Stats_builder(masterList);
            start = new DateTime(1970, 1,1).AddSeconds(masterList[0].gpspoints[0].timesec);
            DateTime end_ = new DateTime(1970, 1, 1).AddSeconds(masterList[0].gpspoints[masterList[0].gpspoints.Count - 1].timesec);
            DataTable dt = new DataTable();
            //dt.Columns.Add("Spacing",typeof(string));
            dt.Columns.Add("Current Data", typeof(string));
            //dt.Columns.Add(" ", typeof(string));

            //dt.Rows.Add("", "");
            dt.Rows.Add("XML File: "+ xml_temp[xml_temp.Length-1]);
            dt.Rows.Add("GPS File: "+ gps_temp[gps_temp.Length-1]);
            dt.Rows.Add("Start Time: "+ start.ToString());
            dt.Rows.Add("End Time: "+ end_.ToString());
            dt.Rows.Add("Networks Seen: "+ totalnets);
            dt.Rows.Add("Infrastructure:  " + infracount);
            dt.Rows.Add("Ad-Hoc (P2P):  " + adhoccount);
            dt.Rows.Add("Probing Clients:  " + probecount);
            //dt.Rows.Add("--------------------------------");
            dt.Rows.Add("Networks chosen for KML:  "+viewList.Count);

            InfoGrid.DataSource = dt;
            InfoGrid.EnableHeadersVisualStyles = false;
            InfoGrid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            InfoGrid.ColumnHeadersDefaultCellStyle.BackColor = Color.CornflowerBlue;
            InfoGrid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            InfoGrid.Rows[0].DataGridView.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            InfoGrid.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
            //InfoGrid.RowsDefaultCellStyle.SelectionBackColor = Color.CornflowerBlue;
            //InfoGrid.RowsDefaultCellStyle.SelectionForeColor = Color.White;
            //InfoGrid.Rows[InfoGrid.Rows.Count - 1].DefaultCellStyle.Font.Bold
            InfoGrid.Rows[InfoGrid.Rows.Count-1].DefaultCellStyle.Font = new Font("Sans Serif", 10F);
        }

        private void InfoGrid_SelectionChanged(object sender, EventArgs e)
        {
            foreach (DataGridViewCell cell in ((DataGridView)sender).SelectedCells)
            {
                cell.Style = new DataGridViewCellStyle()
                {
                    BackColor = Color.White,
                    Font = new Font("Sans Serif", 9F),
                    ForeColor = SystemColors.WindowText,
                    SelectionBackColor = Color.CornflowerBlue,
                    //SelectionForeColor = SystemColors.HighlightText
                    SelectionForeColor = Color.White,
                };
            }
        }

        #endregion

        private void Stats_builder(List<wirelessnetwork> obj)
        {
            //First, compute encrypted statistics (WEP and WPA)
            wepcount = wepcount + wpacount;
            wepstat = ((System.Convert.ToDouble(wepcount) / System.Convert.ToDouble(masterList.Count)) * 100);
            wpastat = ((System.Convert.ToDouble(wpacount) / System.Convert.ToDouble(masterList.Count)) * 100);
            totalnets = obj.Count;

            //Next, compute total war-drive time for title of KML
            totaltime = obj[0].gpspoints[obj[0].gpspoints.Count - 1].timesec - obj[0].gpspoints[0].timesec;
            tspan = TimeSpan.FromSeconds(totaltime);

            //Now we will convert epoch time from gps log file to YYYY MM DD  (or something like that)
            start = new DateTime(1970, 1, 1).AddSeconds(obj[0].gpspoints[0].timesec);

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
        #region Menus I think
        private void parseNewToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            parsenewMenu_Click(sender, e);
        }

        private void saveKMLToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            saveKMLToolStripMenuItem1_Click(sender, e);
        }

        private void allToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            allToolStripMenuItem_Click(sender, e);
        }

        private void selectedToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            selectedToolStripMenuItem_Click(sender, e);
        }

        private void preferencesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Reserved for future use
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {

        }

        private void coordFilterBtn_Click(object sender, EventArgs e)
        {
            coord.ShowDialog();
            coordfilter_run();
            remove_networks(viewList);
            clear_DATAtable(dataGridView1);
            conversion(viewList);
        }
        #endregion

        private void coordfilter_run()
        {
            
            for (int i = 0; i < viewList.Count; i++)
            {
                if (coord.exclude == true)
                {
                    double test = coord.check1;
                    if (viewList[i].maxlat <= coord.check2 || viewList[i].maxlat >= coord.check1 || viewList[i].maxlong <= coord.check4 || viewList[i].maxlong >= coord.check3)
                    {
                        viewList[i].remover = true;
                    }
                }
                else
                {
                    if (viewList[i].maxlat >= coord.check2 || viewList[i].maxlat <= coord.check1 || viewList[i].maxlong >= coord.check4 || viewList[i].maxlong <= coord.check3)
                    {
                        viewList[i].remover = true;
                    }
                }
            }
        }



        private void Networks_Blacklist_Click(object sender, EventArgs e)
        {
            if (reset == 0)
            {
                ShallowCopy<wirelessnetwork>(masterList, tempList);
            }
            else
            {
                ShallowCopy<wirelessnetwork>(viewList, tempList);
            }
            SaveKML_worker(viewList);                               //identify what needs to be removed
            ShallowCopy<wirelessnetwork>(viewList, blackList);      //Copy to blacklist collection
            remove_networks(blackList);
            ShallowCopy<wirelessnetwork>(tempList, viewList);     //copy master back to dataview_collection
            reverser_remove(viewList, blackList);                  //remove those that are in blackList from writeList
            blacklistview();                                        //show networks waiting to be written
            reset = 1;
        }

        private void reverser_remove(List<wirelessnetwork> dest, List<wirelessnetwork> source)
        {
            //remove_networks(dest);
            for (int i = dest.Count - 1; i >= 0; i--)
            {
                if (dest[i].BSSID.ToUpper().Contains("GP:SD"))
                {
                    //do nothing
                }
                else
                {
                    for (int j = 0; j < source.Count; j++)
                    {
                        if (dest[i].BSSID == source[j].BSSID)
                        {
                            dest.Remove(dest[i]);
                            break;
                        }
                    }
                }
            }
            //debugging
            string debug = viewList.Count.ToString();
            InfoGrid_Updater(dest);
        }

        private void InfoGrid_Updater(List<wirelessnetwork> input)
        {
            try
            {
                InfoGrid.Rows[InfoGrid.Rows.Count - 1].Cells[0].Value = "Networks chosen for KML:  " + input.Count;
                InfoGrid.Update();
                writing = true;
                refresh_checker();
            }
            catch { }
        }

        private void blacklistview()
        {
            clear_DATAtable(dataGridView1);
            conversion(viewList);
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}
