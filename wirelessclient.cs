using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;

namespace KismetEarth.NET
{
    public class wirelessclient
    {
        public int number = 0;          //client number
        //public int count = 0;
        public string cfirsttime;
        public string clasttime;

        ////IP Data
        public string ciptype;      //type of packet with IP information seen (TCP, UDP, etc)
        public string ciprange;   //client IP Address... if any

        ////client Main data
        public string clientmac;  //client MAC address
       
    }
}
