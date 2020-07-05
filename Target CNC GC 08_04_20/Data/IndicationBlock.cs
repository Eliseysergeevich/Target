﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Target_CNC_GC_08_04_20.Data
{
   public class IndicationBlock
    {
        private double voltage;
        private int voltageP;
        public int Nomber { get; set; }
        public DateTime LastMessTime { get; set; }
        public int Type { get; set; }
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
                voltageP = (int)Math.Round((value /10.0 * 58.8) - 648.8);
            }
        }
        public IndicationBlock(int nomber, int volt, int type)
        {
            Nomber = nomber;
            Voltage = volt;
            VoltageP = volt;
            Type = type;
            LastMessTime = DateTime.Now;
        }
    }
}
