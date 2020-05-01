using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Target_CNC_GC_08_04_20.Data;

namespace Target_CNC_GC_08_04_20
{
    
    /// <summary>
    /// Логика взаимодействия для TargetEnvironment.xaml
    /// </summary>
    public partial class TargetEnvironment : Window
    {
        ObservableCollection<Target> TargetList = new ObservableCollection<Target> { };
        ObservableCollection<TargetView> TargetViewList = new ObservableCollection<TargetView> { };
        string nomberSensorBlockTemp, nomberIndicationBlockTemp, targetLatitudeTemp, targetLongitudeTemp;
        double targetLat, targetLon;
        public TargetEnvironment()
        {
            InitializeComponent();
            string path = @"FieldData.txt";
            using (StreamReader st = new StreamReader(path, System.Text.Encoding.UTF8))
            {
                string line;
                if ((line = st.ReadLine()) != null) 
                {
                    line.Trim();
                    string[] words = line.Split(new char[] { ' ' });
                    if (words.Length == 4)
                    {
                        if (double.Parse(words[0])>=0) nlNMPRB.IsChecked=true; 
                        else slNMPRB.IsChecked = true;
                      
                        if (double.Parse(words[1]) >= 0) elNMPRB.IsChecked = true;
                        else wlNMPRB.IsChecked = true;

                        if (double.Parse(words[2]) >= 0) nlRB.IsChecked = true;
                        else slRB.IsChecked = true;

                        if (double.Parse(words[3]) >= 0) elRB.IsChecked = true;
                        else wlRB.IsChecked = true;

                        Field.nmpLat = double.Parse(words[0]);
                        Field.nmpLon = double.Parse(words[1]);
                        Field.startLatitude = double.Parse(words[2]);
                        Field.startLongitude = double.Parse(words[3]);

                    }
                }
            }
            nmpLat.Text = Math.Abs(Field.nmpLat).ToString();
            nmpLon.Text = Math.Abs(Field.nmpLon).ToString();
            startLatitudeTB.Text = Math.Abs(Field.startLatitude).ToString();
            startLongitudeTB.Text = Math.Abs(Field.startLongitude).ToString();
            fieldWidth.Text = "100";
            fieldlengdth.Text = "100";
            NLatNewRB.IsChecked = true;
            ELonNewRB.IsChecked = true;
            TypeOfTargetDGCollumn.ItemsSource = Target.tupeOfTargetArray;
            //TargetTypeCB.ItemsSource = Target.tupeOfTargetArray;
            TargetView target200 = new TargetView("Гонг 200", "Imag/200.png");
            TargetView target300 = new TargetView("Гонг 300", "Imag/300.png");
            TargetViewList.Add(target200);
            TargetViewList.Add(target300);
            //TargetTypeCB.ItemsSource = TargetViewList;
            
        }

        private void AddTargenButton_Click(object sender, RoutedEventArgs e)
        {
            if (!OriginalName(TargenNameTB.Text))
            {
                MessageBox.Show($"Мишень с наименованием {TargenNameTB.Text} уже есть!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            
            if (TargenNameTB.Text == "")
            {
                MessageBox.Show("Обозначение не может быть пустым!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (TargetTypeCB.Text == "")
            {
                MessageBox.Show("Необходимо выбрать тип мишени!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (SensorBlockсTB.Text == "")
            {
                MessageBox.Show("Необходимо указать номер блока датчиков!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (IndicationBlockTB.Text == "")
            {
                MessageBox.Show("Необходимо указать номер блока индикации!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!OriginalName(SensorBlockсTB.Text))
            {
                MessageBox.Show($"Блок датчиков № {SensorBlockсTB.Text} уже используется!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (((TargeLongTB.Text == "") && (TargeLatTB.Text != ""))|| ((TargeLongTB.Text!= "") && (TargeLatTB.Text == "")))
            {
                MessageBox.Show("Должны быть заполнены или обе координаты, или ни одной!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if ((TargeLongTB.Text == "") && (TargeLatTB.Text == ""))
                TargetList.Add(new Target(TargenNameTB.Text, int.Parse(SensorBlockсTB.Text), int.Parse(IndicationBlockTB.Text), TargetTypeCB.SelectedIndex));
            else
            {
                if (SLatNewRB.IsChecked == true) targetLat = Math.Abs(double.Parse(TargeLatTB.Text)) * (-1);
                else targetLat = Math.Abs(double.Parse(TargeLatTB.Text));
                if (WLonNewRB.IsChecked == true) targetLon = Math.Abs(double.Parse(TargeLongTB.Text)) * (-1);
                else targetLon = Math.Abs(double.Parse(TargeLongTB.Text));
                TargetList.Add(new Target(TargenNameTB.Text, int.Parse(SensorBlockсTB.Text), int.Parse(IndicationBlockTB.Text), targetLat, targetLon, TargetTypeCB.SelectedIndex));
            }
            
            
            TargetsDataGrid.ItemsSource = TargetList;

        }
        private bool OriginalName(string newName)
        {

            foreach (Target tar in TargetList)
                if (tar.NameTarget == newName) return false;
            return true;
        }

        private void TargetTypeCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }

        private void startLatitudeTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (App.NomberMore0Double(startLatitudeTB.Text)) 
            {
                Field.startLatitude = double.Parse(startLatitudeTB.Text);
                DistanceCalculete();
            } else MessageBox.Show("Некоректный ввод", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void startLongitudeTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (App.NomberMore0Double(startLongitudeTB.Text)) 
            { 
                Field.startLongitude = double.Parse(startLongitudeTB.Text);
                DistanceCalculete(); 
            }
            else MessageBox.Show("Некоректный ввод", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void nmpLat_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (App.NomberMore0Double(nmpLat.Text)) Field.nmpLat = double.Parse(nmpLat.Text); else MessageBox.Show("Некоректный ввод", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void nmpLon_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (App.NomberMore0Double(nmpLon.Text)) Field.nmpLon = double.Parse(nmpLon.Text); else MessageBox.Show("Некоректный ввод", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            string path = @"FieldData.txt";

            if (File.Exists(path))
            {
                File.Delete(path);
                File.Create(path).Close();

            }           
                using (StreamWriter st = new StreamWriter(path, true, System.Text.Encoding.UTF8))
                {
                    string str = Field.nmpLat.ToString()+" "+Field.nmpLon.ToString()+" "+Field.startLatitude.ToString()+" "+Field.startLongitude.ToString();
                    st.WriteLine(str);
                }
        }

        private void nlNMPRB_Checked(object sender, RoutedEventArgs e)
        {
            Field.nmpLat = Math.Abs(Field.nmpLat);
        }

        private void slNMPRB_Checked(object sender, RoutedEventArgs e)
        {
            Field.nmpLat = Math.Abs(Field.nmpLat)*(-1);
        }

        private void wlNMPRB_Checked(object sender, RoutedEventArgs e)
        {
            Field.nmpLon = Math.Abs(Field.nmpLon)*(-1);
        }

        private void elNMPRB_Checked(object sender, RoutedEventArgs e)
        {
            Field.nmpLon = Math.Abs(Field.nmpLon);
        }

        private void nlRB_Checked(object sender, RoutedEventArgs e)
        {
            Field.startLatitude = Math.Abs( Field.startLatitude);
        }

        private void slRB_Checked(object sender, RoutedEventArgs e)
        {
            Field.startLatitude = Math.Abs(Field.startLatitude)*(-1);
        }

        private void wlRB_Checked(object sender, RoutedEventArgs e)
        {
            Field.startLongitude = Math.Abs(Field.startLongitude) * (-1);
        }

        private void elRB_Checked(object sender, RoutedEventArgs e)
        {
            Field.startLongitude = Math.Abs(Field.startLongitude);
        }

        private void TargenNameTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            TargenNameTB.Text = TargenNameTB.Text.Trim();
        }

        private void SensorBlockсTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!App.NomberMore0Int(SensorBlockсTB.Text))
            {
                
                MessageBox.Show("Недопустимое значение", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                SensorBlockсTB.Text = nomberSensorBlockTemp;
                SensorBlockсTB.Select(SensorBlockсTB.Text.Length, 0);
            }
            nomberSensorBlockTemp = SensorBlockсTB.Text;
        }

       
        void DistanceCalculete()
        {
            if ((TargeLatTB.Text != "") && (TargeLongTB.Text != ""))
            {
                double targetLatitude, targetLongitude;
                if (NLatNewRB.IsChecked == true) targetLatitude = double.Parse(TargeLatTB.Text); else targetLatitude = double.Parse(TargeLatTB.Text) * (-1);
                if (ELonNewRB.IsChecked == true) targetLongitude = double.Parse(TargeLongTB.Text); else targetLongitude = double.Parse(TargeLongTB.Text) * (-1);
                DistanceTB.Text = Math.Round(Field.DistanceCulc(targetLatitude, targetLongitude, Field.startLatitude, Field.startLongitude)).ToString();
                AngleTB.Text = Math.Round((Field.AngleField(targetLatitude, targetLongitude, Field.startLatitude, Field.startLongitude)- Field.AngleField(Field.nmpLat, Field.nmpLon, Field.startLatitude, Field.startLongitude)),2).ToString();
            }
        }

        private void SLatNewRB_Checked(object sender, RoutedEventArgs e)
        {
            DistanceCalculete();
        }

        private void NLatNewRB_Checked(object sender, RoutedEventArgs e)
        {
            DistanceCalculete();
        }

        private void WLonNewRB_Checked(object sender, RoutedEventArgs e)
        {
            DistanceCalculete();
        }

        private void ELonNewRB_Checked(object sender, RoutedEventArgs e)
        {
            DistanceCalculete();

        }

        private void TargetsDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
           
            foreach (Target tar in TargetList)
            {

            }
        }

        private void TargeLatTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            
            if (TargeLatTB.Text != "")
            {
                TargeLatTB.Text=TargeLatTB.Text.Replace(".",","); //Заменяет точку на запятую
                TargeLatTB.Select(TargeLatTB.Text.Length, 0);//устанавливает курсор в конец строки
                if (!App.NomberMore0Double(TargeLatTB.Text))//Проверка на целый тип больше 0
                {
                    MessageBox.Show("Некоректный ввод", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    TargeLatTB.Text = targetLatitudeTemp;//Возвращиет в поле последнее правильное значение
                    TargeLatTB.Select(TargeLatTB.Text.Length, 0);
                }
                else
                    if (Math.Abs(double.Parse(TargeLatTB.Text)) > 90)
                {
                    MessageBox.Show("Широта должна быть в диапазоне от 0 до 90", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    TargeLatTB.Text = targetLatitudeTemp;
                    TargeLatTB.Select(TargeLatTB.Text.Length, 0);
                }
                targetLatitudeTemp = TargeLatTB.Text;
            }
           
            DistanceCalculete();
        }

        

        private void TargeLongTB_TextChanged_1(object sender, TextChangedEventArgs e)
        { if (TargeLongTB.Text != "")
            {
                TargeLongTB.Text = TargeLongTB.Text.Replace(".", ",");
                TargeLongTB.Select(TargeLongTB.Text.Length, 0);
                if (!App.NomberMore0Double(TargeLongTB.Text))
                {
                    MessageBox.Show("Некоректный ввод", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    TargeLongTB.Text = targetLongitudeTemp;
                    TargeLongTB.Select(TargeLongTB.Text.Length, 0);
                }
                else
                     if (Math.Abs(double.Parse(TargeLongTB.Text)) > 180)
                {
                    MessageBox.Show("Широта должна быть в диапазоне от 0 до 180", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    TargeLongTB.Text = targetLongitudeTemp;
                    TargeLongTB.Select(TargeLongTB.Text.Length, 0);
                }
                targetLongitudeTemp = TargeLongTB.Text;
            }
            DistanceCalculete();
        }

        private void IndicationBlockTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!App.NomberMore0Int(IndicationBlockTB.Text))
            {

                MessageBox.Show("Недопустимое значение", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                IndicationBlockTB.Text = nomberIndicationBlockTemp;
                IndicationBlockTB.Select(IndicationBlockTB.Text.Length, 0);
            }
            nomberIndicationBlockTemp = IndicationBlockTB.Text;

        }

       

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://www.ngdc.noaa.gov/geomag/data/poles/pole_locations.txt");
        }
    }
    



}
