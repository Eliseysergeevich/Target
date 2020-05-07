using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Target_CNC_GC_08_04_20.Data
{
    public class Shows
    {
        public string Target { get; set; }
        public int PreTimeSec { get; set; }
        public int ShowTimeSec { get; set; }
        public string Type { get; set; }
        public int Serial { get; set; }
        public bool Activ { get; set; }
        public int StartTime { get; set; }
        public bool Struck { get; set; }
        public int StruckTime100ms { get; set; }

        public static string[] arrayTypeShows = {"День","Ночь"};

        public Shows(int serial, string target, string type, int pretimesec, int showtimesec )
        {
            Target = target;
            Serial = serial;          
            Type = type;
            PreTimeSec = pretimesec;
            ShowTimeSec = showtimesec;
            Activ = false;
        }

        public static void StartTimeRefresh()
        {
            for ( int i=0; i<App.ShowsList.Count; i++)
            {
                App.ShowsList[i].StartTime = App.ShowsList[i].PreTimeSec;
                if (App.ShowsList[i].Serial != 1)
                {
                    for (int j = 0; j < App.ShowsList.Count; j++)
                        if (App.ShowsList[i].Serial > App.ShowsList[j].Serial)
                            App.ShowsList[i].StartTime += App.ShowsList[j].PreTimeSec + App.ShowsList[j].ShowTimeSec;
                }
            }
        }

    }
}
