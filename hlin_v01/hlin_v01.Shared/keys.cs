using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace hlin_v01
{
    class keys
    {
        public string userid { get; set; }
        public string dateTime { get; set; }
        public string rememberString { get; set; }

        public keys() { }
        public keys(string Userid , string DateTime, string RememberString)
        {
            this.userid = Userid;
            this.dateTime = DateTime;
            this.rememberString = RememberString;
        }
   
    }
}
