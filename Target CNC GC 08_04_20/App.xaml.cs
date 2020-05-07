using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Target_CNC_GC_08_04_20.Data;

namespace Target_CNC_GC_08_04_20
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    /// 

    public partial class App : Application
    {
        public static double NmpLat { get; set; }// = 86.391;
        public static double NmpLon { get; set; }
        public static bool q;
        public static ObservableCollection<Exercise> ExerciseList = new ObservableCollection<Exercise> { }; //Коллекция упражнений
        public static ObservableCollection<Target> TargetList = new ObservableCollection<Target> { };//Коллекция мишеней в упражнении
        public static ObservableCollection<Sensors> SensorsList = new ObservableCollection<Sensors> { }; //Коллекция блоков датчиков
        public static ObservableCollection<IndicationBlock> IndicationList = new ObservableCollection<IndicationBlock> { }; //Коллекция блоков индикации
        public static ObservableCollection<Shows> ShowsList = new ObservableCollection<Shows> { }; //Коллекция показов
        public static List<string> AllTargetName = new List<string>();

        public static Exercise exerciseActiv;
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
        
        public static void LoadTarget(string name, double startLat, double startLong)
        {
            string path = @name;
            if (File.Exists(path))
            {
                using (StreamReader st = new StreamReader(path, System.Text.Encoding.Default))
                {

                    string line;
                    TargetList.Clear();
                    while ((line = st.ReadLine()) != null)
                    {
                        line = line.Trim();
                        string[] words = line.Split(new char[] { ' ' });
                        if (words.Length == 6)
                            App.TargetList.Add(new Target(words[0], int.Parse(words[1]), int.Parse(words[2]), double.Parse(words[3]), double.Parse(words[4]), startLat, startLong, int.Parse(words[5])));
                        else if (words.Length == 4) App.TargetList.Add(new Target(words[0], int.Parse(words[1]), int.Parse(words[2]), startLat, startLong, int.Parse(words[3])));
                        if (words.Length == 5)
                            App.ShowsList.Add(new Shows(int.Parse(words[0]), words[1], words[2], int.Parse(words[3]), int.Parse(words[4])));
                    }
                }
            }
            else
            {
                MessageBox.Show("Файл с данными о мишенях не обнаружен. \nОн будет создан автоматически.", "Внимание!");
                File.Create(path).Close();
            }
        }

        public static void UploadTarget(string name)
        {
            string path = @name;


            if (File.Exists(path))
            {
                File.Delete(path);
                File.Create(path).Close();

            }

            foreach (Target tar in App.TargetList)
                using (StreamWriter st1 = new StreamWriter(path, true, System.Text.Encoding.UTF8))
                {
                    int typeIndex = 0;
                    for (int i = 0; i < Target.tupeOfTargetArray.Length; i++)
                        if (Target.tupeOfTargetArray[i] == tar.TypeOfTarget)
                            typeIndex = i;
                    string str = tar.NameTarget + " " + tar.NomberSensorsBlock.ToString() + " " + tar.NomberIndicationBlock.ToString() + " " + tar.Latitude.ToString() + " " + tar.Longitude.ToString() + " " + typeIndex.ToString();
                    st1.WriteLine(str);
                }
            foreach (Shows sh in App.ShowsList)
                using (StreamWriter st1 = new StreamWriter(path, true, System.Text.Encoding.UTF8))
                {
                    
                    string str = sh.Serial + " " + sh.Target + " " + sh.Type + " " + sh.PreTimeSec.ToString() + " " + sh.ShowTimeSec.ToString();
                    st1.WriteLine(str);
                }

        }
        public static double DistanceCulc(double latitude, double longitude, double latitudeStart, double longitudeStart)
        {
            double lat1Rad = latitudeStart * Math.PI / 180.0;
            double long1Rad = longitudeStart * Math.PI / 180.0;
            double lat2Rad = latitude * Math.PI / 180.0;
            double long2Rad = longitude * Math.PI / 180.0;
            double sinlat1Rad = Math.Sin(lat1Rad);
            double sinlat2Rad = Math.Sin(lat2Rad);
            double coslat1Rad = Math.Cos(lat1Rad);
            double coslat2Rad = Math.Cos(lat2Rad);
            double sinDeltaLong = Math.Sin(long2Rad - long1Rad);
            double cosDeltaLong = Math.Cos(long2Rad - long1Rad);
            double temp1 = coslat2Rad * sinDeltaLong;
            temp1 = temp1 * temp1;
            double temp2 = coslat1Rad * sinlat2Rad;
            double temp3 = sinlat1Rad * coslat2Rad;
            temp3 = temp3 * cosDeltaLong;
            temp3 = temp2 - temp3;
            temp3 = temp3 * temp3;
            temp3 = temp1 + temp3;
            temp3 = Math.Sqrt(temp3);
            double temp4 = sinlat1Rad * sinlat2Rad;
            double temp5 = coslat1Rad * coslat2Rad;
            temp5 = temp5 * cosDeltaLong;
            temp5 = temp4 + temp5;
            double temp6 = temp3 / temp5;
            temp6 = Math.Atan(temp6);
            double s = 6372795 * temp6;
            return s;

        }


        public static double AngleField(double latitude, double longitude, double latitudeStart, double longitudeStart)
        {
            double lat1Rad = latitudeStart * Math.PI / 180.0;
            double long1Rad = longitudeStart * Math.PI / 180.0;
            double lat2Rad = latitude * Math.PI / 180.0;
            double long2Rad = longitude * Math.PI / 180.0;
            double sinlat1Rad = Math.Sin(lat1Rad);
            double sinlat2Rad = Math.Sin(lat2Rad);
            double coslat1Rad = Math.Cos(lat1Rad);
            double coslat2Rad = Math.Cos(lat2Rad);
            double sinDeltaLong = Math.Sin(long2Rad - long1Rad);
            double cosDeltaLong = Math.Cos(long2Rad - long1Rad);
            double temp1 = coslat1Rad * sinlat2Rad - sinlat1Rad * coslat2Rad * cosDeltaLong;
            double temp2 = sinDeltaLong * coslat2Rad;
            double temp3 = Math.Atan2(-temp2, temp1) * 180.0 / Math.PI;
            if (temp1 < 0) temp3 = temp3 + 180;
            double temp4 = -(temp3 + 180 % 360 - 180) * Math.PI / 180.0;
            double anglerad = temp4 - ((2 * Math.PI) * (Math.Floor(temp4 / (2 * Math.PI))));
            double angle = anglerad * 180.0 / Math.PI;
            return angle;
        }
        public static void RefreshAllTargetName()
        {
            AllTargetName.Clear();
            if (TargetList.Count>0)
            for (int i=0; i < TargetList.Count; i++)
            {
                    AllTargetName.Add(TargetList[i].NameTarget);
            }
        }

    }

}
