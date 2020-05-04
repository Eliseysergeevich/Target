using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Target_CNC_GC_08_04_20
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    /// 

    public partial class App : Application
    {
        public static bool q;
        public static ObservableCollection<Target> TargetList = new ObservableCollection<Target>();

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
        public static void LoadTarget()
        {
            string TargetDataFile = @"TargetData.txt";
            if (File.Exists(TargetDataFile))
            {
                using (StreamReader st = new StreamReader(TargetDataFile, System.Text.Encoding.Default))
                {
                    string line;

                    while ((line = st.ReadLine()) != null)
                    {
                        line = line.Trim();
                        string[] words = line.Split(new char[] { ' ' });
                        if (words.Length == 6)
                            App.TargetList.Add(new Target(words[0], int.Parse(words[1]), int.Parse(words[2]), double.Parse(words[3]), double.Parse(words[4]), int.Parse(words[5])));
                        else if (words.Length == 4) App.TargetList.Add(new Target(words[0], int.Parse(words[1]), int.Parse(words[2]), int.Parse(words[3])));
                    }
                    //TargetsDataGrid.ItemsSource = App.TargetList;

                }
            }
            else
            {
                MessageBox.Show("Файл с данными о мишенях не обнаружен. \nОн будет создан автоматически.", "Внимание!");
                File.Create(TargetDataFile);
            }
        }
        public static void UploadTarget()
        {
            string path = @"FieldData.txt";

            if (File.Exists(path))
            {
                File.Delete(path);
                File.Create(path).Close();

            }
            using (StreamWriter st = new StreamWriter(path, true, System.Text.Encoding.UTF8))
            {
                string str = Field.nmpLat.ToString() + " " + Field.nmpLon.ToString() + " " + Field.startLatitude.ToString() + " " + Field.startLongitude.ToString();
                st.WriteLine(str);
            }

            string TargetDataFile = @"TargetData.txt";

            if (File.Exists(TargetDataFile))
            {
                File.Delete(TargetDataFile);
                File.Create(TargetDataFile).Close();

            }

            foreach (Target tar in App.TargetList)
                using (StreamWriter st1 = new StreamWriter(TargetDataFile, true, System.Text.Encoding.UTF8))
                {
                    int typeIndex = 0;
                    for (int i = 0; i < Target.tupeOfTargetArray.Length; i++)
                        if (Target.tupeOfTargetArray[i] == tar.TypeOfTarget.ToString())
                            typeIndex = i;
                    string str = tar.NameTarget + " " + tar.NomberSensorsBlock.ToString() + " " + tar.NomberIndicationBlock.ToString() + " " + tar.Latitude.ToString() + " " + tar.Longitude.ToString() + " " + typeIndex.ToString();
                    st1.WriteLine(str);
                }
        }

    }

}
