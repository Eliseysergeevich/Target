using System;
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
        ObservableCollection<Sensors> SensorsList = new ObservableCollection<Sensors> { }; //Коллекция блоков датчиков
        ObservableCollection<IndicationBlock> IndicationList = new ObservableCollection<IndicationBlock> { }; //Коллекция блоков индикации
        ObservableCollection<Exercise> ExerciseList = new ObservableCollection<Exercise> { }; //Коллекция упражнений
        ObservableCollection<Shows> ShowsList = new ObservableCollection<Shows> { }; //Коллекция показов
        List<string> AllTargetName = new List<string>();
        public int count;
        //Exercise exercise = new Exercise();


        public MainWindow()
        {
            InitializeComponent();

            //exercise.RealTime100ms = 0;

            App.LoadTarget();//Формирование коллекции мишеней при запуске программы

            string[] ports = SerialPort.GetPortNames();// массив строк с именами всех COM портов в системе
            portsCB.ItemsSource = ports;

            //Создание и параметризация таймера
            DispatcherTimer timer = new DispatcherTimer(DispatcherPriority.Render);
            timer.Tick += new EventHandler(timer_Tick);
            // timer.Tick += new EventHandler(timerCOM_Tick);
            timer.Interval = new TimeSpan(0, 0, 0, 0, 100);
            timer.Start();

            nlRB.IsChecked = true;
            elRB.IsChecked = true;

            //aTimer = new System.Timers.Timer(1500);
            //aTimer.Elapsed += timerCOM_Tick;
            //aTimer.AutoReset = true;
            //aTimer.Enabled = true;

            //DispatcherTimer timerCOM = new DispatcherTimer();
            //timerCOM.Tick += new EventHandler(timerCOM_Tick);
            //timerCOM.Interval = new TimeSpan(0, 0, 0, 0, 20);
            //timerCOM.Start();

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
                        ExerciseList.Add(new Exercise(words[0], double.Parse(words[1]), double.Parse(words[2]), discriptionTemp));
                        ExerciseDG.ItemsSource = ExerciseList;
                    }
                }
            }
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
                if (SensorsList.Count == 0)// Если блоки датчиков отсутствуют в коллекции
                {
                    SensorsList.Add(new Sensors(int.Parse(inputStr[1]), int.Parse(inputStr[2]), int.Parse(inputStr[3]), int.Parse(inputStr[4])));
                    sensorsDG.ItemsSource = null;
                    sensorsDG.ItemsSource = SensorsList;
                }
                else
                {
                    //bool newSens = true;
                    foreach (Sensors sens in SensorsList)
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
                            var sortedUsers = SensorsList.OrderBy(u => u.Nomber);
                            sensorsDG.ItemsSource = sortedUsers;
                            return;
                        }
                    }
                    SensorsList.Add(new Sensors(int.Parse(inputStr[1]), int.Parse(inputStr[2]), int.Parse(inputStr[3]), int.Parse(inputStr[4])));
                    sensorsDG.ItemsSource = null;
                    sensorsDG.ItemsSource = SensorsList;
                }
            }
            if (inputStr[0] == "2")
            {
                if (IndicationList.Count == 0)// Если блоки датчиков отсутствуют в коллекции
                {
                    IndicationList.Add(new IndicationBlock(int.Parse(inputStr[1]), int.Parse(inputStr[2]), int.Parse(inputStr[3])));
                    indicationDG.ItemsSource = null;
                    indicationDG.ItemsSource = IndicationList;
                }
                else
                {
                    //bool newSens = true;
                    foreach (IndicationBlock indicationBlock in IndicationList)
                    {
                        if (indicationBlock.Nomber == int.Parse(inputStr[1]))
                        {
                            //newSens = false;
                            indicationBlock.Voltage = int.Parse(inputStr[2]);
                            indicationBlock.VoltageP = int.Parse(inputStr[2]);
                            indicationBlock.Type = int.Parse(inputStr[3]);
                            indicationBlock.LastMessTime = DateTime.Now;
                            indicationDG.ItemsSource = null;
                            var sortedUsersInd = IndicationList.OrderBy(u => u.Nomber);
                            indicationDG.ItemsSource = sortedUsersInd;
                            return;
                        }
                    }
                    IndicationList.Add(new IndicationBlock(int.Parse(inputStr[1]), int.Parse(inputStr[2]), int.Parse(inputStr[3])));
                    indicationDG.ItemsSource = null;
                    indicationDG.ItemsSource = IndicationList;
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
        Random rnd = new Random(50);
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
                //ArduinoPort.ReadBufferSize = 1024;
                //ArduinoPort.ReceivedBytesThreshold = 5;
                //ArduinoPort.RtsEnable = true;
                //ArduinoPort.ReadTimeout = 1000;
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
            App.UploadTarget();
        }

        //Добавление показа
        private void ButtonAddShow_Click(object sender, RoutedEventArgs e)
        {
            int serialTemp = 1;
            string targetTemp = App.TargetList[0].NameTarget;
            int type = 1;
            int preTimeTemp = 5;
            int showTimeTemp = 10;
            int startTimeTemp = 0;
            if (ShowsList.Count > 0)
            {
                serialTemp = ShowsList.Count + 1;
                startTimeTemp = AlltimeBefor();
            }
            ShowsList.Add(new Shows(serialTemp, targetTemp, type, preTimeTemp, showTimeTemp, startTimeTemp));
            ShowsDG.ItemsSource = ShowsList;
        }

        private int AlltimeBefor()
        {
            int sum = 0;
            foreach (Shows sh in ShowsList)
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
            

        }

        private void ShowsDG_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            if (e.Column.DisplayIndex == 5)
            {
                var temp = e.Row.Item as Shows;
                if (temp.Serial < ShowsList.Count)
                {
                    foreach (Shows sh in ShowsList)
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
                ShowsDG.ItemsSource = ShowsList;
            }
            if (e.Column.DisplayIndex == 6)
            {
                var temp = e.Row.Item as Shows;
                if (temp.Serial>1)
                {
                    foreach (Shows sh in ShowsList)
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
                ShowsDG.ItemsSource = ShowsList;
            }
           
        }

        private void up_Click(object sender, RoutedEventArgs e)
        {
            
           
        }

        private void down_Click(object sender, RoutedEventArgs e)
        {

        }

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
                ExerciseList.Add(new Exercise(tempName, AboutExerciseTB.Text));
            else
            {
                if (slRB.IsChecked == true) latitudeTemp = Math.Abs(double.Parse(startLatitudeTB.Text)) * (-1);
                else latitudeTemp = Math.Abs(double.Parse(startLatitudeTB.Text));
                if (wlRB.IsChecked == true) longitudeTemp = Math.Abs(double.Parse(startLongitudeTB.Text)) * (-1);
                else longitudeTemp = Math.Abs(double.Parse(startLongitudeTB.Text));
                ExerciseList.Add(new Exercise(tempName, latitudeTemp, longitudeTemp, AboutExerciseTB.Text));
                
            }
            ExerciseDG.ItemsSource = ExerciseList;          

            //Добавление нового упражнения в файл
            string exerciseNameFile = @"ExerciseName.txt";
            using (StreamWriter st = new StreamWriter(exerciseNameFile, true, System.Text.Encoding.UTF8))
            {

                string str = tempName+" "+ ExerciseList[ExerciseList.Count-1].StartLatitude + " "+ ExerciseList[ExerciseList.Count - 1].StartLongitude + " "+ AboutExerciseTB.Text;

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
                File.Create(exerciseNamefile);

            }
            catch
            {
                MessageBox.Show("Не получилось создать файл");
            }
        }

        private bool OriginalNameExercise(string newName)//Проверка на оригинальность обозначение мишени
        {

            foreach (Exercise ex in ExerciseList)
                if (ex.Name == newName) return false;
            return true;
        }
    }
    

}
