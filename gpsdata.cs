using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;

namespace KismetEarth.NET
{
    public class gpsdata
    {
        public int x=0;
        public string bssid;        //MAC address of wireless network or GP:SD:TR:AC:KL:OG (gps tracker)
        public float timesec;       //Time from epoch (?)
        public float timeusec;      //Unknown use (will research)
        public double longitude;    //longitude from gps file
        public double latitude;     //latitude from gps file
        public double altitude;     //altitude from gps file
        public string spd;          //Detected GPS speed
        public string heading;      //GPS Heading 
        public string fix;          //GPS FIX (probably not used)
        public int signal;          //signal from gps file
        public int quality;         //GPS signal quality (probably not used)
        public int noise;           //Background RF noise detected
        public string souce;        //source (BSSID) from signal emitted
        public string color;
        public double scale;


       
    }
}
