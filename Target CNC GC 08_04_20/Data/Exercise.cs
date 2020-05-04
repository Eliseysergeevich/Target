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


    }
}
