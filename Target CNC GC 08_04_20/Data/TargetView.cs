using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Target_CNC_GC_08_04_20.Data
{
    public class TargetView
    {
        
        public string Name { get; set; }
        public string Sourse { get; set; }

        public TargetView(string name, string sourse) {
            Name = name;
            Sourse = sourse;
            }
    }
}
