using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Target_CNC_GC_08_04_20
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
       public static bool q;
        public static bool NomberMore0Int(string st)
        {
            string inputString = st;
            if (inputString != "")
            {
                for (int i = 0; i < inputString.Length; i++)
                    if ((inputString[i] < '0') || (inputString[i] > '9'))

                        return false;

                return true;
            }
            return false;
        }
        public static bool NomberMore0Double(string st)
        {
            int count = 0;
            string inputString = st;
            if (inputString != "")
            {
                for (int i = 0; i < inputString.Length; i++) 
                
                    if (((inputString[i] < '0') || (inputString[i] > '9')) && (inputString[i] != ','))
                    {
                        return false;
                    }
                



                    else
                    {
                        if (inputString[i] == ',') count++;
                        if (count > 1) return false; ;
                    }

                        

                return true;
            }
            return false;
        }
    }
}
