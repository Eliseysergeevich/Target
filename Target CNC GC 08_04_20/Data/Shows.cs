using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Target_CNC_GC_08_04_20.Data
{
    class Shows
    {
        public string Target { get; set; }
        public int PreTimeSec { get; set; }
        public int ShowTimeSec { get; set; }
        public int Type { get; set; }
        public int Serial { get; set; }
        public bool Activ { get; set; }
        public int StartTime { get; set; }
        public bool Struck { get; set; }
        public int StruckTime100ms { get; set; }

        public Shows(int serial, string target, int type, int pretimesec, int showtimesec, int starttimesec )
        {
            Target = target;
            Serial = serial;
            Type = type;
            PreTimeSec = pretimesec;
            ShowTimeSec = showtimesec;
            StartTime = starttimesec;
            Activ = false;
        }
    }
}
