using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;

namespace KismetEarth.NET
{
    public class wirelessnetwork
    {
        //Network Main Data
        public string SSID_;                 //Wireless Network Name; default is no ssid
        public string BSSID;                //Wireless network MAC
        public int channel;
        public string maxrate;                  //max speed in mbps detected
        public string maxseenrate;
        public string encryption;               //encryption type
        //public string cipher;                 //follow on to encryption
        public bool remover = false;

        //Packet Information
        public string LLC;          //crashes program... only datqasize works properly
        public string data;
        public string crypt;
        public string weak;
        public string dupeiv;
        public string total;
        public string datasize;
        public string iptype; 
        public string iprange;

        //wireless-network attributes
        public string type;         //Infrastructure, ad-hoc, probing client (mode of operation)
        public string wep;          //not really wep or not, more like layer 3 encryption (WEP, WPA-PSK, etc)
        public string cloaked;      //SSID braodcast off = cloaked
        public string firsttime;    //first time seen
        public string lasttime;     //last time seen

        //for gps sorting
        public double maxlat;
        public double maxlong;
        public int maxsignal;
        public double maxalt;

        public double checklat;
        public double checklon;
        public int checksignal;
        public string checkbssid;

        public string iconpath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);

        public int visibility = 1;

        //public bool SSIDsearch_bool;
        //public string SSIDsearch;

        #region properties
        //=============================
        //PROPERTIES BELOW for DATAGRID
        //=============================
        public string SSID
        {
            get 
            {
                if (SSID_ == null)
                {
                    SSID_ = "(no ssid)";
                }
                if (SSID_ == "")
                {
                    SSID_ = "(no ssid)";
                }
                if (SSID_.Contains("&amp"))
                {
                    SSID_ = SSID_;
                }
                else
                {
                    SSID_ = SSID_.ToString().Replace("&", "&amp;");
                }
                if (SSID_.Length > 32)
                {
                    SSID_ = SSID_.Substring(0, 31);
                }
                return SSID_;
            }
        }

        //public string BSSID
        //{
        //    get
        //    {
        //        if (BSSID == null)
        //        {
        //            BSSID = "(no MAC)";
        //        }
        //        if (BSSID == "")
        //        {
        //            BSSID = "(no MAC)";
        //        }
        //        //mstrBSSID = mstrBSSID.ToString().Replace("&", "&amp;");
        //        return BSSID;
        //    }
        //}

        //public string ENC
        //{
        //    get
        //    {
        //        if (encryption == null)
        //        {
        //            encryption = "N/A";
        //        }
        //        return encryption;
        //    }
        //}

        //public int CH
        //{
        //    get
        //    {
        //        return channel;
        //    }
        //}

        //public double LAT
        //{
        //    get
        //    {
        //        return maxlat;
        //    }
        //}

        //public double LONG
        //{
        //    get
        //    {
        //        return maxlong;
        //    }
        //}

        public string seenfor
        {
            get
            {
                //Will be fixed later
                return "TBD";
            }
        }

        //public string SSID_visibility
        //{
        //    get
        //    {
        //        if (SSIDsearch_bool == false)
        //        {
        //            return "1";
        //        }
        //        else
        //        {
                
        //        if (SSIDsearch == SSID)
        //        {
        //            return "1";
        //        }
        //        else
        //        {
        //            return "0";
        //        }
        //    }



        //}
        public string AppPath
        {
            get
            {
                 if (iconpath.Contains("\\"))
                 {
                     return iconpath + "\\files\\";
                 }
                 else
                 {
                     return iconpath + "/files/";
                 }

            }
        }

        public string customicon
        {
            get
            {
                if (type == null)
                {
                    return AppPath + "INFRASTRUCTUREOPENunknown.png";
                }
                else
                {
                    if (type.ToUpper() == "AD-HOC")
                    {
                        //return "file:///C:/Programming/files/" + "ADHOC" + ".png";
                        return  AppPath +"ADHOC" + ".png";
                    }
                    else
                    {
                        if (type.ToUpper() == "PROBE")
                        {
                            //return "file:///C:/Programming/files/" + "PROBED" + ".png";
                            return AppPath + "PROBED" + ".png";
                        }
                        else
                        {
                            //Console.WriteLine(encryption);
                            if (encryption == "unknown" || encryption == null)
                            {
                                    //should be AppPath + type.ToUpper() + INFRASTRUCTUREOPENunknown.png 
                                    return AppPath + type.ToUpper() + "OPENunknown.png";
                            }
                            else
                            {
                                switch (encryption.ToUpper())
                                {
                                    case "NONE":
                                        if (channel > 0 && channel < 162)
                                        {
                                            return AppPath + type.ToUpper() + "OPEN" + channel.ToString() + ".png";
                                            break;
                                        }
                                        else
                                        {
                                            //should be AppPath + type.ToUpper() + INFRASTRUCTUREOPENunknown.png 
                                            return AppPath + type.ToUpper() + "OPENunknown.png";
                                        }
                                        break;
                                    case "WEP":
                                        //return "file:///C:/Programming/files/" + type.ToUpper() + "WEP" + channel + ".png";
                                        if (channel > 0 && channel < 162)
                                        {
                                            return AppPath + type.ToUpper() + "WEP" + channel.ToString() + ".png";
                                            break;
                                        }
                                        else
                                        {
                                            return AppPath + type.ToUpper() + "WEPunknown.png";
                                            break;
                                        }
                                    case "WEP40":
                                        //return "file:///C:/Programming/files/" + type.ToUpper() + "WEP" + channel + ".png";
                                        if (channel > 0 && channel < 162)
                                        {
                                            return AppPath + type.ToUpper() + "WEP" + channel.ToString() + ".png";
                                            break;
                                        }
                                        else
                                        {
                                            return AppPath + type.ToUpper() + "WEPunknown.png";
                                            break;
                                        }
                                    default:
                                        //return "file:///C:/Programming/files/" + type.ToUpper() + "WPA" + channel + ".png";
                                        if (channel > 0 || channel < 162)
                                        {
                                            return AppPath + type.ToUpper() + "WPA" + channel.ToString() + ".png";
                                            break;
                                        }
                                        else
                                        {
                                            return AppPath + type.ToUpper() + "WPAunknown.png";
                                            break;
                                        }
                                }
                            }

                        }
                    }

                }

            }
        }
        #endregion

        public List<wirelessclient> cli;
        public List<gpsdata> gpspoints;

        public wirelessnetwork()
        {
            cli = new List<wirelessclient>();
            gpspoints = new List<gpsdata>();

            maxlat = 0;
            maxlong = 0;
            maxalt = 0;
            maxsignal = -200;

            checklat = 0;
            checklon = 0;
            checksignal = -200;
            checkbssid = "0";

        }

        public void Addwirelessclient(int num, string macaddr, string first, string last, string iprange, string iptype)
        {
            wirelessclient obj = new wirelessclient();
            obj.number = num;
            obj.clientmac = macaddr;
            obj.cfirsttime = first;
            obj.clasttime = last;
            obj.ciprange = iprange;
            obj.ciptype = iptype;

            cli.Add(obj);
        }

        public void Addwirelessclient(wirelessclient obj)
        {
            cli.Add(obj);
        }
        public void AddGPSPoints(double lat, double lon, double alt, int signal, float timesec, string bssid)
        {
            gpsdata obj = new gpsdata();
            obj.latitude = lat;
            //obj.bssid = bssid;
            obj.longitude = lon;
            obj.altitude = alt;
            obj.signal = signal;
            obj.timesec = timesec;

            double scale;
            string color;

            if (signal > 0)
                signal = signal * -1;

            if (signal > 0)
                signal = signal * -1;

            if (signal > -80)
            {
                scale = ((System.Convert.ToDouble(signal) - -80) * 0.03) + 1;
            }
            else
            {
                scale = (1 - (-1 * (0.03 * ((System.Convert.ToDouble(signal) + 80)))));
            }
            obj.scale = scale;
            /********************************************************
                       SIGNAL to COLOR TABLE
                                                         ATHEROS
                    MIN     MAX		PRIMARY COLOR		RSSI equiv
                    -67     -60		MAROON			    23-30+
                    -74     -68		RED				    16-22
                    -79     -75		YELLOW			    11-15
                    -84     -80		GREEN			     6-10
                        ***** BAD SIGNAL FROM HERE*****
                    -89     -85		CYAN                 1-5
                    -99     -90		BLUE                 0
                            -100	NAVY
                ************************************************************/
            //initialize colors
            double c_red = 0;
            double c_green = 0;
            double c_blue = 0;
            double c_temp = 0;
            string hex_red;
            string hex_green;
            string hex_blue;


            if (signal > -60)   //MAROON
            {
                c_red = 128;
            }
            else
            {
                if (signal > -68) //Goes to MAROON
                {
                    c_temp = signal - -67;
                    c_red = 255 - (32 * c_temp);
                }
                else
                {
                    if (signal > -75) //goes to RED
                    {
                        c_temp = signal - -74;
                        c_green = 255 - (42 * c_temp);
                        c_red = 255;
                    }
                    else
                    {
                        if (signal > -80)   //goes to YELLOW
                        {
                            c_temp = signal - -79;
                            c_green = 255;
                            c_red = 51 * c_temp;

                        }
                        else
                        {
                            if (signal > -85)    //goes to GREEN
                            {
                                c_temp = signal - -84;
                                c_blue = 255 - 51 * c_temp;
                                c_green = 255;
                            }
                            else
                            {
                                if (signal > -90)   //goes to Cyan
                                {
                                    c_temp = signal - -89;
                                    c_blue = 255;
                                    c_green = 51 * c_temp;
                                }
                                else
                                {
                                    if (signal < -100)  //goes to blue
                                    {
                                        c_temp = signal - -100;
                                        c_blue = 255 - (c_temp * 12);
                                    }
                                    else
                                    {
                                        //signals over -100
                                        c_blue = 123;
                                    }
                                }
                            }
                        }
                    }
                }

            }
            hex_red = dechex(c_red);
            hex_green = dechex(c_green);
            hex_blue = dechex(c_blue);

            color = "BF" + hex_blue + hex_green + hex_red;
            //Console.WriteLine(color);
            obj.color = color;

            if (signal > maxsignal)
            {
                maxsignal = signal;
                maxlat = lat;
                maxlong = lon;
                maxalt = alt;
            }

            if (lat == checklat && lon == checklon && signal <= checksignal && bssid == checkbssid)
            {
                //Console.WriteLine("fired this point: " + obj.latitude + " " + obj.longitude);
                checklat = lat;
                checklon = lon;
                checksignal = signal;
                checkbssid = obj.bssid;
            }
            else
            {
                checklat = lat;
                checklon = lon;
                checksignal = signal;
                checkbssid = bssid;
                gpspoints.Add(obj);
            }

        }

        public string dechex(double a)
        {
            string strDec = System.Convert.ToString(a);
            string strHex = String.Format("{0:x2}", (uint)System.Convert.ToUInt32(strDec));
            return strHex;
        }
    }
}
