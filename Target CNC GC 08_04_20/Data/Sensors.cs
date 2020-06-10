using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Target_CNC_GC_08_04_20.Data
{
    public class Sensors
    {
        private double voltage;
        private int voltageP;
        public int Nomber { get; set; }
        public double Voltage
        {
            get
            {
                return voltage;
            }
            set
            {
                voltage = (value / 10);
            }
        }
        public int VoltageP
        {
            get
            {
                return voltageP;
            }
            set
            {
                voltageP = (int)Math.Round((value / 10 * 42.735) - 269.231);
            }
        }    
        public bool Sensor1 { get; set; }
        public bool Sensor2 { get; set; }

        public DateTime LastMessTime { get; set; }

        public Sensors(int nomber, int volt, int sens1, int sens2)
        {
            Nomber = nomber;
            Voltage = volt;
            VoltageP = volt;
            Sensor1 = !Convert.ToBoolean(sens1);
            Sensor2 = !Convert.ToBoolean(sens2);
            LastMessTime = DateTime.Now;
        }

    }
}
