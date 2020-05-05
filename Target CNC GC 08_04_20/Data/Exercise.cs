using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Target_CNC_GC_08_04_20.Data
{
    class Exercise
    {
        private int realTime100ms;
        public int RealTime100ms
        {
            get { return realTime100ms; }
            set { realTime100ms = value;
               
            }
        }
        public int AllTime { get; set; } //Полное время упражнения
        public string Name { get; set; }
        public double StartLatitude { get; set; }// = 55.812264; Широта рубежа
        public  double StartLongitude { get; set; }// = 38.04094; Долгота рубежа

        public string Description { get; set; }//Описание к упражнению

        public Exercise(string name, string st)
        {
            Name = name;
            StartLatitude = 55.751999;
            StartLongitude = 37.617734;
            if (st == "") Description = "Без описания"; 
            else Description = st;
        }
        public Exercise(string name,double latitude, double longitude, string st)
        {
            Name = name;
            StartLatitude = latitude;
            StartLongitude = longitude;
            if (st == "") Description = "Без описания";
            else Description = st;
        }
    }
}
