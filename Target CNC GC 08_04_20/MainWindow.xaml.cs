﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO.Ports;
using System.Windows.Threading;
using Target_CNC_GC_08_04_20.Data;

namespace Target_CNC_GC_08_04_20
{
   
    public partial class MainWindow : Window
    {
        
        SerialPort ArduinoPort = new SerialPort();//Создаём последовательный порт 
        private delegate void updateDelegate(string txt);
        
       
        public int count;
        //Exercise exercise = new Exercise();
        Random rnd = new Random(50);
        private string nomberSensorBlockTemp;
        private string nomberIndicationBlockTemp;
        private string targetLatitudeTemp;
        private string targetLongitudeTemp;
        private double targetLat;
        private double targetLon;
        private string nameBeforChange;
        

        public MainWindow()
        {
            InitializeComponent();

            //exercise.RealTime100ms = 0;

            //App.LoadTarget();//Формирование коллекции мишеней при запуске программы

           
        } 
        

        private void timer_Tick(object sender, EventArgs e)
        {
            //exercise.RealTime100ms++;
            //TimeNow.Value = exercise.RealTime100ms;
            //// dataSerialTB.Text = exercise.RealTime100ms.ToString();
            //if (count == 100) count = 0;
            //if (exercise.RealTime100ms == 100) exercise.RealTime100ms = 0;
        }


        private void timerCOM_Tick(object sender, EventArgs e)
        {
          
        }

        //Чтение данных с COM порта
        public void updateCOMData(string txt)
        {
            txt = txt.Trim(',');
            string[] inputStr = txt.Split(',');
            if (inputStr[0] == "1")
            {
                if (App.SensorsList.Count == 0)// Если блоки датчиков отсутствуют в коллекции
                {
                    App.SensorsList.Add(new Sensors(int.Parse(inputStr[1]), int.Parse(inputStr[2]), int.Parse(inputStr[3]), int.Parse(inputStr[4])));
                    sensorsDG.ItemsSource = null;
                    sensorsDG.ItemsSource = App.SensorsList;
                }
                else
                {
                    //bool newSens = true;
                    foreach (Sensors sens in App.SensorsList)
                    {
                        if (sens.Nomber == int.Parse(inputStr[1]))
                        {
                            //newSens = false;
                            sens.Voltage = int.Parse(inputStr[2]);
                            sens.VoltageP = int.Parse(inputStr[2]);
                            sens.Sensor1 = Convert.ToBoolean(int.Parse(inputStr[3]));
                            sens.Sensor2 = Convert.ToBoolean(int.Parse(inputStr[4]));
                            sens.LastMessTime = DateTime.Now;
                            sensorsDG.ItemsSource = null;
                            var sortedUsers = App.SensorsList.OrderBy(u => u.Nomber);
                            sensorsDG.ItemsSource = sortedUsers;
                            return;
                        }
                    }
                    App.SensorsList.Add(new Sensors(int.Parse(inputStr[1]), int.Parse(inputStr[2]), int.Parse(inputStr[3]), int.Parse(inputStr[4])));
                    sensorsDG.ItemsSource = null;
                    sensorsDG.ItemsSource = App.SensorsList;
                }
            }
            if (inputStr[0] == "2")
            {
                if (App.IndicationList.Count == 0)// Если блоки датчиков отсутствуют в коллекции
                {
                    App.IndicationList.Add(new IndicationBlock(int.Parse(inputStr[1]), int.Parse(inputStr[2]), int.Parse(inputStr[3])));
                    indicationDG.ItemsSource = null;
                    indicationDG.ItemsSource = App.IndicationList;
                }
                else
                {
                    //bool newSens = true;
                    foreach (IndicationBlock indicationBlock in App.IndicationList)
                    {
                        if (indicationBlock.Nomber == int.Parse(inputStr[1]))
                        {
                            //newSens = false;
                            indicationBlock.Voltage = int.Parse(inputStr[2]);
                            indicationBlock.VoltageP = int.Parse(inputStr[2]);
                            indicationBlock.Type = int.Parse(inputStr[3]);
                            indicationBlock.LastMessTime = DateTime.Now;
                            indicationDG.ItemsSource = null;
                            var sortedUsersInd = App.IndicationList.OrderBy(u => u.Nomber);
                            indicationDG.ItemsSource = sortedUsersInd;
                            return;
                        }
                    }
                    App.IndicationList.Add(new IndicationBlock(int.Parse(inputStr[1]), int.Parse(inputStr[2]), int.Parse(inputStr[3])));
                    indicationDG.ItemsSource = null;
                    indicationDG.ItemsSource = App.IndicationList;
                }
            }
        }


        private void situation_Click(object sender, RoutedEventArgs e)
        {
            TargetEnvironment TargetEnvironment = new TargetEnvironment();
            TargetEnvironment.Show();
        }

   
        private void Setings_Click(object sender, RoutedEventArgs e)
        {
            Window1 window1 = new Window1();
            window1.Show();
        }
       

        //Random rnd1 = new Random(50);

        private void rect_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //Canvas.SetLeft(rect, e.GetPosition(can).X);
            //Canvas.SetTop(rect, e.GetPosition(can).Y);


        }

        private void rect_MouseMove(object sender, MouseEventArgs e)
        {
            //Canvas.SetLeft(rect, e.GetPosition(can).X);
            //Canvas.SetTop(rect, e.GetPosition(can).Y);
        }

        private void Person_Click(object sender, RoutedEventArgs e)
        {
            if (!App.q)
            {
                PeopleWin person = new PeopleWin();
                person.Show();
                App.q = true;
            }
        }
        
        //Соединение с COM портом
        private void conectBT_Click(object sender, RoutedEventArgs e)
        {
            if (portsCB.Text != "")
            {
                ArduinoPort.PortName = portsCB.Text;
                ArduinoPort.BaudRate = 9600;
                ArduinoPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
                try
                {
                    ArduinoPort.Open();


                }
                catch
                {
                    MessageBox.Show("Неудачно");
                }
            }
            else MessageBox.Show("Порт не выбран");

        }
        
        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            if (ArduinoPort.IsOpen)
            {
                try
                {
                    string indata = ArduinoPort.ReadLine();

                    dataSerialTB.Dispatcher.BeginInvoke(new updateDelegate(updateCOMData), indata);
                }
                catch
                {
                    MessageBox.Show("Что-то не так");


                }

            }


        }

        private void disConectBT_Click(object sender, RoutedEventArgs e)
        {

            ArduinoPort.Close();
        }

        private void portsCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }



        private void portsCB_MouseMove(object sender, MouseEventArgs e)
        {
            string[] ports = SerialPort.GetPortNames();
            portsCB.ItemsSource = ports;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (ArduinoPort.IsOpen) ArduinoPort.Close();
            //App.UploadTarget();
            string settingsNameFile = @"Settings.txt";
            using (StreamWriter st = new StreamWriter(settingsNameFile, false, System.Text.Encoding.UTF8))
            {
                
                string str = App.NmpLat+" "+App.NmpLon;
                st.WriteLine(str);
            }
            string ExerciseNameFile = @"ExerciseName.txt";
            if (File.Exists(ExerciseNameFile))
            {
                File.Delete(ExerciseNameFile);
                File.Create(ExerciseNameFile).Close();
                foreach (Exercise ex in App.ExerciseList)
                {
                    using (StreamWriter st = new StreamWriter(ExerciseNameFile, true, System.Text.Encoding.UTF8))
                    {

                        string str = ex.Name + " " + ex.StartLatitude + " " + ex.StartLongitude + " " + ex.Description;

                        st.WriteLine(str);
                    }
                }
            }
            if (App.exerciseActiv != null)
            {
                string nameUploadFile = "Exercise/" + App.exerciseActiv.Name + ".txt";
                App.UploadTarget(nameUploadFile);
            }
        }

        //Добавление показа
        private void ButtonAddShow_Click(object sender, RoutedEventArgs e)
        {
            if (App.TargetList.Count == 0)
            {
                MessageBox.Show("Список доступных мишеней пуст", "Внимание!");
                return;
            }
            int serialTemp = 1;
            string targetTemp = App.TargetList[0].NameTarget;
            string type = Shows.arrayTypeShows[0];
            int preTimeTemp = 5;
            int showTimeTemp = 10;
            int startTimeTemp = 0;
            if (App.ShowsList.Count > 0)
            {
                serialTemp = App.ShowsList.Count + 1;
                startTimeTemp = AlltimeBefor();
            }
            App.ShowsList.Add(new Shows(serialTemp, targetTemp, type, preTimeTemp, showTimeTemp));
            
            TypeShowsOfTargetDG.ItemsSource = Shows.arrayTypeShows;
            Shows.StartTimeRefresh();
            ShowsDG.ItemsSource = App.ShowsList;
        }

        private int AlltimeBefor()
        {
            int sum = 0;
            foreach (Shows sh in App.ShowsList)
            {
                sum += sh.PreTimeSec + sh.ShowTimeSec;
            }
            return sum;
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }

        private void ShowsDG_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            
            if (e.Column.DisplayIndex == 3)
            {
                var editedTextbox = e.EditingElement as TextBox;
                if (!App.NomberMore0Int(editedTextbox.Text))
                {
                    MessageBox.Show("Некоректный ввод", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    e.Cancel = true;
                    return;
                }
                var sh = e.Row.Item as Shows;
                sh.PreTimeSec = int.Parse(editedTextbox.Text);
                Shows.StartTimeRefresh();
                ShowsDG.ItemsSource = null;
                ShowsDG.ItemsSource = App.ShowsList;

            }

            if (e.Column.DisplayIndex == 4)
            {
                var editedTextbox = e.EditingElement as TextBox;
                if (!App.NomberMore0Int(editedTextbox.Text))
                {
                    MessageBox.Show("Некоректный ввод", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    e.Cancel = true;
                    return;
                }
                var sh = e.Row.Item as Shows;
                sh.ShowTimeSec = int.Parse(editedTextbox.Text);
                Shows.StartTimeRefresh();
                ShowsDG.ItemsSource = null;
                ShowsDG.ItemsSource = App.ShowsList;
            }
            
        }

        private void ShowsDG_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            if (e.Column.DisplayIndex == 6)
            {
                var temp = e.Row.Item as Shows;
                if (temp.Serial < App.ShowsList.Count)
                {
                    foreach (Shows sh in App.ShowsList)
                    {
                        if (sh.Serial == temp.Serial + 1)
                        {
                            sh.Serial = temp.Serial;
                            sh.StartTime -= temp.PreTimeSec+temp.ShowTimeSec;
                            temp.StartTime+= sh.PreTimeSec + sh.ShowTimeSec;
                        }
                    }
                    temp.Serial++;
                }
                ShowsDG.ItemsSource = null;
                ShowsDG.ItemsSource = App.ShowsList;
            }
            if (e.Column.DisplayIndex == 7)
            {
                var temp = e.Row.Item as Shows;
                if (temp.Serial>1)
                {
                    foreach (Shows sh in App.ShowsList)
                    {
                        if (sh.Serial == temp.Serial - 1)
                        {
                            sh.Serial = temp.Serial;
                            sh.StartTime += temp.PreTimeSec + temp.ShowTimeSec;
                            temp.StartTime -= sh.PreTimeSec + sh.ShowTimeSec;
                        }
                    }
                    temp.Serial--;
                }
                ShowsDG.ItemsSource = null;
                ShowsDG.ItemsSource = App.ShowsList;
            }
           
        }

        private void up_Click(object sender, RoutedEventArgs e)
        {
            
           
        }

        private void down_Click(object sender, RoutedEventArgs e)
        {

        }

        //Кнопка добавить упражнение
        private void AddExerciseBut_Click(object sender, RoutedEventArgs e)
        {
            string tempName;
            double latitudeTemp, longitudeTemp;
            if (ExerciseNameTB.Text == "")
            {
                MessageBox.Show("Отсутствует наименование упражнения!", "Ошибка");
                ExerciseNameTB.Focus();
                return;
            }
            tempName = ExerciseNameTB.Text.Trim();
            if (!OriginalNameExercise(tempName))
            {
                MessageBox.Show($"Упражнение с наименованием {tempName} уже есть!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                ExerciseNameTB.Focus();
                return;

            }
            if (((startLatitudeTB.Text!="")&&(startLongitudeTB.Text==""))|| ((startLatitudeTB.Text == "") && (startLongitudeTB.Text != "")))
            {
                MessageBox.Show("Должны быть заполнены или обе координаты, или ни одной!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if ((startLatitudeTB.Text == "") && (startLongitudeTB.Text == ""))
                App.ExerciseList.Add(new Exercise(tempName, AboutExerciseTB.Text));
            else
            {
                if (slRB.IsChecked == true) latitudeTemp = Math.Abs(double.Parse(startLatitudeTB.Text)) * (-1);
                else latitudeTemp = Math.Abs(double.Parse(startLatitudeTB.Text));
                if (wlRB.IsChecked == true) longitudeTemp = Math.Abs(double.Parse(startLongitudeTB.Text)) * (-1);
                else longitudeTemp = Math.Abs(double.Parse(startLongitudeTB.Text));
                App.ExerciseList.Add(new Exercise(tempName, latitudeTemp, longitudeTemp, AboutExerciseTB.Text));
                
            }
            ExerciseDG.ItemsSource = App.ExerciseList;          

            //Добавление нового упражнения в файл
            string exerciseNameFile = @"ExerciseName.txt";
            using (StreamWriter st = new StreamWriter(exerciseNameFile, true, System.Text.Encoding.UTF8))
            {

                string str = tempName+" "+ App.ExerciseList[App.ExerciseList.Count-1].StartLatitude + " "+ App.ExerciseList[App.ExerciseList.Count - 1].StartLongitude + " "+ AboutExerciseTB.Text;

                st.WriteLine(str);
            }
            
            ExerciseNameTB.Text = "";
            startLatitudeTB.Text = "";
            startLongitudeTB.Text = "";
            AboutExerciseTB.Text = "";
            try
            {
                string exerciseName= "Exercise/"+tempName+".txt";
                string exerciseNamefile = @exerciseName;
                File.Create(exerciseNamefile).Close();

            }
            catch
            {
                MessageBox.Show("Не получилось создать файл");
            }
        }

        private bool OriginalNameExercise(string newName)//Проверка на оригинальность обозначение мишени
        {

            foreach (Exercise ex in App.ExerciseList)
                if (ex.Name == newName) return false;
            return true;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            NLatNewRB.IsChecked = true;
            ELonNewRB.IsChecked = true;

            nlRB.IsChecked = true;
            elRB.IsChecked = true;

            TypeOfTargetDGCollumn.ItemsSource = Target.tupeOfTargetArray;
            TypeShowsOfTargetDG.ItemsSource = Shows.arrayTypeShows;
            // TypeTargetOfShowDG.ItemsSource = App.
            // массив строк с именами всех COM портов в системе
            string[] ports = SerialPort.GetPortNames();
            portsCB.ItemsSource = ports;

            //Создание и параметризация таймера
            DispatcherTimer timer = new DispatcherTimer(DispatcherPriority.Render);
            timer.Tick += new EventHandler(timer_Tick);
            timer.Interval = new TimeSpan(0, 0, 0, 0, 100);
            timer.Start();
       
            //Выгрузка координат Северное магнитного полюса
            string settingsFileName = @"Settings.txt";
            using (StreamReader st = new StreamReader(settingsFileName, System.Text.Encoding.UTF8))
            {
                string line;
                if ((line = st.ReadLine()) != null)
                {
                    line.Trim();
                    string[] words = line.Split(new char[] { ' ' });
                    if (words.Length == 2)
                    {
                        App.NmpLat = double.Parse(words[0]);
                        App.NmpLon = double.Parse(words[1]);
                    }
                    else MessageBox.Show("Не корректное содержание файла Settings", "Ошибка");
                }
            }

            //Выгрузка данных об упражнениях при загрузке программы
            string path = @"ExerciseName.txt";
            using (StreamReader st = new StreamReader(path, System.Text.Encoding.UTF8))
            {
                string line;
                while ((line = st.ReadLine()) != null)
                {
                    line.Trim();
                    string[] words = line.Split(new char[] { ' ' });
                    if (words.Length > 3)
                    {
                        string discriptionTemp = "";
                        for (int i = 3; i < words.Length; i++)
                            discriptionTemp = discriptionTemp + words[i] + " ";
                        App.ExerciseList.Add(new Exercise(words[0], double.Parse(words[1]), double.Parse(words[2]), discriptionTemp));
                        ExerciseDG.ItemsSource = App.ExerciseList;
                    }
                }
            }

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

        private void TargeLatTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TargeLatTB.Text != "")
            {
                TargeLatTB.Text = TargeLatTB.Text.Replace(".", ","); //Заменяет точку на запятую
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

        private void DistanceCalculete()
        {
            if ((TargeLatTB.Text != "") && (TargeLongTB.Text != ""))
            {
                double targetLatitude, targetLongitude;              
                var ex = ExerciseDG.SelectedItem as Exercise;
                double startLatitude = ex.StartLatitude;
                double startLongitude = ex.StartLongitude;
                if (NLatNewRB.IsChecked == true) targetLatitude = double.Parse(TargeLatTB.Text); 
                else targetLatitude = double.Parse(TargeLatTB.Text) * (-1);
                if (ELonNewRB.IsChecked == true) targetLongitude = double.Parse(TargeLongTB.Text); 
                else targetLongitude = double.Parse(TargeLongTB.Text) * (-1);
                DistanceTB.Text = Math.Round(App.DistanceCulc(targetLatitude, targetLongitude, startLatitude, startLongitude)).ToString();
                AngleTB.Text = Math.Round((App.AngleField(targetLatitude, targetLongitude, startLatitude, startLongitude) - App.AngleField(Field.nmpLat, Field.nmpLon, startLatitude, startLongitude)), 2).ToString();
            }
        }

        private void ExerciseDG_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (App.exerciseActiv!=null)
            {
                string nameUploadFile = "Exercise/" + App.exerciseActiv.Name + ".txt";
                App.UploadTarget(nameUploadFile);
            }
            App.ShowsList.Clear();        
            var ex = ExerciseDG.SelectedItem as Exercise;
            App.exerciseActiv = ex;
            if (ex != null)
            {
                ExerciseSelectName.Text = ex.Name;
                DistanceCalculete();
                string nameLoadFile = "Exercise/" + App.exerciseActiv.Name + ".txt";
                App.LoadTarget(nameLoadFile, ex.StartLatitude, ex.StartLongitude);
                Shows.StartTimeRefresh();
                TargetsDataGrid.ItemsSource = null;
                TargetsDataGrid.ItemsSource = App.TargetList;
                ShowsDG.ItemsSource = App.ShowsList;
            }
            App.RefreshAllTargetName();
            TypeTargetOfShowDG.ItemsSource = App.AllTargetName;
        }

        private void TargeLongTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TargeLongTB.Text != "")
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

        private bool OriginalName(string newName)//Проверка на оригинальность обозначение мишени
        {

            foreach (Target tar in App.TargetList)
                if (tar.NameTarget == newName) return false;
            return true;
        }

        private void AddTargenButton_Click(object sender, RoutedEventArgs e)
        {

            var ex = ExerciseDG.SelectedItem as Exercise;
            double startLatitude = ex.StartLatitude;
            double startLongitude = ex.StartLongitude;

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
                if (!OriginalSensorNomber(SensorBlockсTB.Text))
                {
                    MessageBox.Show($"Блок датчиков № {SensorBlockсTB.Text} уже используется!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (((TargeLongTB.Text == "") && (TargeLatTB.Text != "")) || ((TargeLongTB.Text != "") && (TargeLatTB.Text == "")))
                {
                    MessageBox.Show("Должны быть заполнены или обе координаты, или ни одной!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if ((TargeLongTB.Text == "") && (TargeLatTB.Text == ""))
                    App.TargetList.Add(new Target(TargenNameTB.Text, int.Parse(SensorBlockсTB.Text), int.Parse(IndicationBlockTB.Text), startLatitude, startLongitude, TargetTypeCB.SelectedIndex));
                else
                {
                    if (ELatNewRB.IsChecked == true) targetLat = Math.Abs(double.Parse(TargeLatTB.Text)) * (-1);
                    else targetLat = Math.Abs(double.Parse(TargeLatTB.Text));
                    if (WLonNewRB.IsChecked == true) targetLon = Math.Abs(double.Parse(TargeLongTB.Text)) * (-1);
                    else targetLon = Math.Abs(double.Parse(TargeLongTB.Text));
                    App.TargetList.Add(new Target(TargenNameTB.Text, int.Parse(SensorBlockсTB.Text), int.Parse(IndicationBlockTB.Text), targetLat, targetLon, startLatitude, startLongitude, TargetTypeCB.SelectedIndex));
                }
            App.RefreshAllTargetName();
            TargetsDataGrid.ItemsSource = App.TargetList;

            
        }
        private bool OriginalSensorNomber(string SensorNomber)
        {

            foreach (Target tar in App.TargetList)
                if (tar.NomberSensorsBlock.ToString() == SensorNomber) return false;
            return true;
        }

        private void ELatNewRB_Checked(object sender, RoutedEventArgs e)
        {
            DistanceCalculete();
        }

        private void TargetTypeCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void TargetsDataGrid_KeyDown(object sender, KeyEventArgs e)
        {
            
        }

        private void TargetsDataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                var tar= TargetsDataGrid.SelectedItem as Target;
               foreach (Shows sh in App.ShowsList)
                {
                    if (sh.Target==tar.NameTarget)
                    {
                        App.ShowsList.Remove(sh);
                    }
                }
                Shows.StartTimeRefresh();
                ShowsDG.ItemsSource = null;
                ShowsDG.ItemsSource = App.ShowsList;
            }
        }

        //Действия при удалении упражнения
        private void ExerciseDG_PreviewKeyDown(object sender, KeyEventArgs e)
        {

            if (e.Key == Key.Delete) 
            {
                string nameUploadFile = "Exercise/" + App.exerciseActiv.Name + ".txt";
               
                File.Delete(nameUploadFile);
                App.exerciseActiv = null;
                
                
            }
        }

        private void ExerciseDG_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                string ExerciseNameFile = @"ExerciseName.txt";
                if (File.Exists(ExerciseNameFile))
                {
                    File.Delete(ExerciseNameFile);
                    File.Create(ExerciseNameFile).Close();
                    foreach (Exercise ex in App.ExerciseList)
                    {
                        using (StreamWriter st = new StreamWriter(ExerciseNameFile, true, System.Text.Encoding.UTF8))
                        {

                            string str = ex.Name + " " + ex.StartLatitude + " " + ex.StartLongitude + " " + ex.Description;

                            st.WriteLine(str);
                        }
                    }
                }
            }
        }

        private void ExerciseDG_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            string nameFile = @"Exercise/" + nameBeforChange + ".txt";
            if (File.Exists(nameFile)) File.Delete(nameFile);

        }

        private void ExerciseDG_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            if (e.Column.DisplayIndex == 0)
            {
                var temp = e.Row.Item as Exercise;
                nameBeforChange = temp.Name;
            }
            

        }
        string nameTargetGBTemp, nomberSensorBlockDGTemp;
        private void TargetsDataGrid_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            Target dataRow = e.Row.Item as Target;
            nameTargetGBTemp = dataRow.NameTarget;
            nomberSensorBlockDGTemp = dataRow.NomberSensorsBlock.ToString();
        }

        private void ShowsDG_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
            
        }

        private void TargetsDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.Column.DisplayIndex == 0)
            {
                var editedTextbox = e.EditingElement as TextBox;
                if (editedTextbox.Text == "")
                {
                    MessageBox.Show("Обозначение мишени необходимо указать", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    e.Cancel = true;
                    return;
                }
                if (!OriginalName(editedTextbox.Text) && (nameTargetGBTemp != editedTextbox.Text))
                {
                    MessageBox.Show($"Мишень {editedTextbox.Text} уже имеется!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    e.Cancel = true;
                    return;
                }
            }
            if (e.Column.DisplayIndex == 2)
            {
                var editedTextbox = e.EditingElement as TextBox;
                if (editedTextbox.Text == "")
                {
                    MessageBox.Show("Номер блока датчиков необходимо указать", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    e.Cancel = true;
                    return;
                }
                if (!App.NomberMore0Int(editedTextbox.Text))
                {
                    MessageBox.Show("Недопустимое значение", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    e.Cancel = true;
                    return;
                }
                if (!OriginalSensorNomber(editedTextbox.Text) && (nomberSensorBlockDGTemp != editedTextbox.Text))
                {
                    MessageBox.Show($"Блок датчиков {editedTextbox.Text} уже используется!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    e.Cancel = true;
                    return;
                }
            }
            if (e.Column.DisplayIndex == 3)
            {
                var editedTextbox = e.EditingElement as TextBox;
                if (editedTextbox.Text == "")
                {
                    MessageBox.Show("Номер блока индикации необходимо указать", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    e.Cancel = true;
                    return;
                }

            }
            if (e.Column.DisplayIndex == 4)
            {
                var editedTextbox = e.EditingElement as TextBox;
                editedTextbox.Text = editedTextbox.Text.Replace(".", ",");

                if (editedTextbox.Text == "")
                {
                    editedTextbox.Text = "0";

                }

                else
                {
                    if (editedTextbox.Text[0] == '-')
                    {
                        string strTemp = editedTextbox.Text.Substring(1); if (!App.NomberMore0Double(strTemp))
                        {
                            MessageBox.Show("Некоректный ввод", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                            e.Cancel = true;
                            return;
                        }
                    }
                    if (Math.Abs(double.Parse(editedTextbox.Text)) > 90)
                    {
                        MessageBox.Show("Широта должна быть в диапазоне от -90 до 90", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                        e.Cancel = true;
                        return;
                    }
                }
                var editedTarget = e.Row.Item as Target;
                editedTarget.Distance = Math.Round(Field.DistanceCulc(double.Parse(editedTextbox.Text), editedTarget.Longitude, Field.startLatitude, Field.startLongitude));
                editedTarget.Angle = Math.Round(editedTarget.AngleField(double.Parse(editedTextbox.Text), editedTarget.Longitude, Field.startLatitude, Field.startLongitude) - editedTarget.AngleField(Field.nmpLat, Field.nmpLon, Field.startLatitude, Field.startLongitude), 2);
                editedTarget.Latitude = double.Parse(editedTextbox.Text);
                //MessageBox.Show(editedTarget.Distance.ToString());
                editedTextbox.Text = editedTextbox.Text.Replace(",", ".");
                //e.Cancel = false;
                TargetsDataGrid.ItemsSource = null;
                TargetsDataGrid.ItemsSource = App.TargetList;

            }
            if (e.Column.DisplayIndex == 5)
            {
                var editedTextbox = e.EditingElement as TextBox;
                editedTextbox.Text = editedTextbox.Text.Replace(".", ",");

                if (editedTextbox.Text == "")
                {
                    editedTextbox.Text = "0";

                }

                else
                {
                    if (editedTextbox.Text[0] == '-')
                    {
                        string strTemp = editedTextbox.Text.Substring(1); if (!App.NomberMore0Double(strTemp))
                        {
                            MessageBox.Show("Некоректный ввод", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                            e.Cancel = true;
                            return;
                        }
                    }
                    if (Math.Abs(double.Parse(editedTextbox.Text)) > 180)
                    {
                        MessageBox.Show("Широта должна быть в диапазоне от -180 до 180", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                        e.Cancel = true;
                        return;
                    }
                }
                var editedTarget = e.Row.Item as Target;
                editedTarget.Distance = Math.Round(Field.DistanceCulc(editedTarget.Latitude, double.Parse(editedTextbox.Text), Field.startLatitude, Field.startLongitude));
                editedTarget.Angle = Math.Round(editedTarget.AngleField(editedTarget.Latitude, double.Parse(editedTextbox.Text), Field.startLatitude, Field.startLongitude) - editedTarget.AngleField(Field.nmpLat, Field.nmpLon, Field.startLatitude, Field.startLongitude), 2);
                editedTarget.Longitude = double.Parse(editedTextbox.Text);
                //MessageBox.Show(editedTarget.Distance.ToString());
                editedTextbox.Text = editedTextbox.Text.Replace(",", ".");
                //e.Cancel = false;
                TargetsDataGrid.ItemsSource = null;
                TargetsDataGrid.ItemsSource = App.TargetList;

            }
        }
    }
    

}
