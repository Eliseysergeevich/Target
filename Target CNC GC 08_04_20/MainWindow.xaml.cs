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
using Path = System.Windows.Shapes.Path;
using Jitbit.Utils;

namespace Target_CNC_GC_08_04_20
{

    public partial class MainWindow : Window
    {
        CsvExport CsvExport = new CsvExport();
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
            ResoltGrid.DataContext = App.resoultList;
            
            //Создание и параметризация таймера
            DispatcherTimer timer = new DispatcherTimer(DispatcherPriority.Render);
            timer.Tick += new EventHandler(timer_Tick);
            timer.Interval = new TimeSpan(0, 0, 0, 1);
            timer.Start();
        }

        //Тик таймера 1сек
        private void timer_Tick(object sender, EventArgs e)
        {
            if ((App.exercisePlay!=null) &&(App.exercisePlay.IsActiv == true))
            {
            App.exercisePlay.RealTime100ms++;
                TimeNow.Value = App.exercisePlay.RealTime100ms;
                buttonCaption.FontSize = 100;
                buttonCaption.Text = (App.exercisePlay.AllTime- App.exercisePlay.RealTime100ms).ToString(); 
                
                foreach(Shows sh in App.ShowsPlayList)
                {
                    if (App.exercisePlay.RealTime100ms == sh.StartTime)
                    {
                        String mess="";
                        foreach (Target tg in App.TargetPlayList)
                            if (sh.Target == tg.NameTarget) mess = tg.NomberIndicationBlock.ToString();
                        if (sh.Type=="День")
                        mess += "1";
                        if (sh.Type == "Ночь")
                            mess += "2";
                        if (sh.Type == "Ключ")
                            mess += "4";
                        if (sh.ShowTimeSec > 9) mess += sh.ShowTimeSec.ToString();
                        else mess+='0'+ sh.ShowTimeSec.ToString();
                        mess += 'n';
                        //for (int i = 0; i < 2; i++)
                        //{
                            ArduinoPort.Write(mess);
                           
                        //}
                       
                        dataSerialTB.Text = mess;
                    }
                }
               
                //Блок для отрисовки крестика при промахе
                if ((GetEndShow()!=null)&&(GetEndShow().Struck==false))
                {
                    GetEndShow().NoStruck = true;
                    ProgressExerciseDG.ItemsSource = null;
                    ProgressExerciseDG.ItemsSource = App.ShowsPlayList;
                }
               
                //Определение окончания упражнения по времени
                if (TimeNow.Value >= (App.exercisePlay.AllTime )) 
                { 
                    App.exercisePlay.IsActiv = false;
                    SaveResoultPlayBut.IsEnabled = true;
                    ResetResoultPlayBut.IsEnabled = true;
                    MessageBox.Show("Упражнение окончено!", "Внимание!");
                }
               
            }

        }


        private void timerCOM_Tick(object sender, EventArgs e)
        {

        }

        //Чтение данных с COM порта
        public void updateCOMData(string txt)
        {
            dataSerialTB1.Text = txt;
            txt = txt.Trim(',');
            dataSerialTB1.Text = txt;
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
                            sens.Sensor1 = !Convert.ToBoolean(int.Parse(inputStr[3]));
                            sens.Sensor2 = !Convert.ToBoolean(int.Parse(inputStr[4]));
                            sens.LastMessTime = DateTime.Now;
                            sensorsDG.ItemsSource = null;
                            var sortedUsers = App.SensorsList.OrderBy(u => u.Nomber);
                            sensorsDG.ItemsSource = sortedUsers;
                            TargetTest(int.Parse(inputStr[1]), int.Parse(inputStr[2]), !Convert.ToBoolean(int.Parse(inputStr[3])), !Convert.ToBoolean(int.Parse(inputStr[4])));
                            return;
                        }
                    }
                    App.SensorsList.Add(new Sensors(int.Parse(inputStr[1]), int.Parse(inputStr[2]), int.Parse(inputStr[3]), int.Parse(inputStr[4])));
                    sensorsDG.ItemsSource = null;
                    sensorsDG.ItemsSource = App.SensorsList;
                }
                //TargetTest(int.Parse(inputStr[1]), int.Parse(inputStr[2]), Convert.ToBoolean(int.Parse(inputStr[3])), Convert.ToBoolean(int.Parse(inputStr[4])));
            }
            if (inputStr[0] == "2")
            {
                if (App.IndicationList.Count == 0)// Если блоки индикации отсутствуют в коллекции
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

        private void TargetTest(int nomber, int v2, bool sens1, bool sens2)
        {
            if (App.exercisePlay != null)
            {
                if (App.exercisePlay.IsActiv)
                {
                    if ((sens1==true) || (sens2==true))
                    {
                        if ((GetShows()!=null)&&(GetShows().Struck!=true))
                        if (GetShows().Target == GetSendTarget(nomber))
                        {

                            GetShows().Struck = true;
                            GetShows().StruckTime100ms = App.exercisePlay.RealTime100ms - GetShows().StartTime;
                            ProgressExerciseDG.ItemsSource = null;
                            ProgressExerciseDG.ItemsSource = App.ShowsPlayList;
                                
                                    
                                        String mess = "";
                                        foreach (Target tg in App.TargetPlayList)
                                            if (GetShows().Target == tg.NameTarget) mess = tg.NomberIndicationBlock.ToString();//
                                          mess += "3";
                                       
                                        mess += "05"+'n';
                                
                                    //for (int i = 0; i < 5; i++) {
                                    ArduinoPort.Write(mess); 
                                    //Thread.Sleep(100); }

                                       
                                        dataSerialTB.Text = mess;
   

                            }
                    }
                }
            }
        }

        //Даёт имя мишени от которой пришёл сигнал
        private string GetSendTarget(int nomber)
        {
            string st = "";
            foreach (Target tg in App.TargetPlayList)
            {
                if (tg.NomberSensorsBlock == nomber)
                    st = tg.NameTarget;
            }
            return st;
        }

        //Даёт показ активный в данный момент времени
        private Shows GetShows()
        {
            Shows SH = null;
            foreach(Shows sh in App.ShowsPlayList)
            {
                if ((App.exercisePlay.RealTime100ms > sh.StartTime) && (App.exercisePlay.RealTime100ms < sh.StartTime + sh.ShowTimeSec))
                {
                    SH = sh;
                    return SH;
                }
                    
            }
            return SH;
        }
        
        //Даёт только что завершившийся показ
        private Shows GetEndShow()
        {
            Shows SH = null;
            foreach (Shows sh in App.ShowsPlayList)
            {
                if ((App.exercisePlay.RealTime100ms <= sh.StartTime + sh.ShowTimeSec+1) && (App.exercisePlay.RealTime100ms >= sh.StartTime + sh.ShowTimeSec))
                {
                    SH = sh;
                    return SH;
                }

            }
            return SH;
        }

        
        private void Setings_Click(object sender, RoutedEventArgs e)
        {
            Window1 window1 = new Window1();
            window1.Show();
        }

        //private void Person_Click(object sender, RoutedEventArgs e)
        //{
        //    if (!App.q)
        //    {
        //        PeopleWin person = new PeopleWin();
        //        person.Show();
        //        App.q = true;
        //    }
        //}

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
                if (ArduinoPort.IsOpen)
                {
                    conectBT.IsEnabled = false;
                    portsCB.IsEnabled = false;
                    App.IndicationList.Clear();
                    App.SensorsList.Clear();
                    indicationDG.ItemsSource = null;
                    indicationDG.ItemsSource = App.IndicationList;
                    sensorsDG.ItemsSource = null;
                    sensorsDG.ItemsSource = App.SensorsList;
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
            if (ArduinoPort.IsOpen)
              
                ArduinoPort.Close();
                conectBT.IsEnabled = true;
                portsCB.IsEnabled = true;
            
        }
           
        

        private void portsCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }



        private void portsCB_MouseMove(object sender, MouseEventArgs e)
       {

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (ArduinoPort.IsOpen) ArduinoPort.Close();
            //App.UploadTarget();
            string settingsNameFile = @"Settings.txt";
            using (StreamWriter st = new StreamWriter(settingsNameFile, false, System.Text.Encoding.UTF8))
            {

                string str = App.NmpLat + " " + App.NmpLon;
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
            string path = @"Person.txt";

            if (File.Exists(path))
            {
                File.Delete(path);
                File.Create(path).Close();

            }
            foreach (Person man in App.PersonList)
                using (StreamWriter st = new StreamWriter(path, true, System.Text.Encoding.UTF8))
                {
                    string str = man.Nomber.ToString() + " " + man.Scvod.ToString() + " " + man.Name + " " + man.Fam + " " + man.MiddleName;
                    st.WriteLine(str);
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
            Shows.StartTimeRefresh(false);
            ShowsDG.ItemsSource = App.ShowsList;
            AllTimeExerciseLB.Content = App.AllTimeExercise(false);
            
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
            if (SettingsExercise.IsSelected == false)
            {
                if (App.exerciseActiv != null)
                {
                    string nameUploadFile = "Exercise/" + App.exerciseActiv.Name + ".txt";
                    App.UploadTarget(nameUploadFile);
                }
            }
            
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
                Shows.StartTimeRefresh(false);
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
                Shows.StartTimeRefresh(false);
                ShowsDG.ItemsSource = null;
                ShowsDG.ItemsSource = App.ShowsList;
            }

            AllTimeExerciseLB.Content = App.AllTimeExercise(false);
            
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
                            sh.StartTime -= temp.PreTimeSec + temp.ShowTimeSec;
                            temp.StartTime += sh.PreTimeSec + sh.ShowTimeSec;
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
                if (temp.Serial > 1)
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
            if (((startLatitudeTB.Text != "") && (startLongitudeTB.Text == "")) || ((startLatitudeTB.Text == "") && (startLongitudeTB.Text != "")))
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

                string str = tempName + " " + App.ExerciseList[App.ExerciseList.Count - 1].StartLatitude + " " + App.ExerciseList[App.ExerciseList.Count - 1].StartLongitude + " " + AboutExerciseTB.Text;

                st.WriteLine(str);
            }

            ExerciseNameTB.Text = "";
            startLatitudeTB.Text = "";
            startLongitudeTB.Text = "";
            AboutExerciseTB.Text = "";
            try
            {
                string exerciseName = "Exercise/" + tempName + ".txt";
                string exerciseNamefile = @exerciseName;
                File.Create(exerciseNamefile).Close();

            }
            catch
            {
                MessageBox.Show("Не получилось создать файл");
            }
            App.ExerciseListUbdate();

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
            
            
            scvodCB.ItemsSource = App.allScvodList;
            App.PersonList.Clear();
            string PersonFile = @"Person.txt";
            using (StreamReader st = new StreamReader(PersonFile, System.Text.Encoding.Default))
            {
                string line;

                while ((line = st.ReadLine()) != null)
                {
                    line = line.Trim();
                    string[] words = line.Split(new char[] { ' ' });
                    if (words.Length == 5)
                    {
                        App.PersonList.Add(new Person(int.Parse(words[0]), int.Parse(words[1]), words[2], words[3], words[4]));
                    }

                }
                personLB.ItemsSource = App.PersonList;
            }
            allScvodTextBox.Text = Person.MaxScvod().ToString();
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
            if (App.exerciseActiv != null)
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
                Shows.StartTimeRefresh(false);
                TargetsDataGrid.ItemsSource = null;
                TargetsDataGrid.ItemsSource = App.TargetList;
                ShowsDG.ItemsSource = App.ShowsList;
            }
            App.RefreshAllTargetName();
            TypeTargetOfShowDG.ItemsSource = App.AllTargetName;
            AllTimeExerciseLB.Content = App.AllTimeExercise(false);
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

        //Удаление мишени
        private void TargetsDataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                var tar = TargetsDataGrid.SelectedItem as Target; //Получаю выбранную запись в переменную tar как объект Target
                foreach (Shows sh in App.ShowsList)
                {
                    if (sh.Target == tar.NameTarget)
                    {
                        MessageBox.Show("Данная мишень задействована в показах./n Исключите её из показаов прежде чем удалять", "Внимание!");
                        e.Handled = true;
                    }
                }

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

        private void ShowsDG_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                AllTimeExerciseLB.Content = App.AllTimeExercise(false);
                
            }
        }

        //СТАРТ упражнение
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (App.exercisePlay != null)
            {
            startTime.Text = DateTime.Now.ToString();
            App.exercisePlay.RealTime100ms = 0;
            App.exercisePlay.IsActiv = true;
                if (App.exercisePlay.IsActiv)
                {
                    SartBut.IsEnabled = false;
                    if (ArduinoPort.IsOpen)
                    ArduinoPort.Write("an");
                }
            }
            
        }

        private void ExerciseNameForPlayCB_DropDownOpened(object sender, EventArgs e)
        {
            ExerciseNameForPlayCB.ItemsSource = null;
            App.ExerciseListUbdate();
            ExerciseNameForPlayCB.ItemsSource = App.AllExerciseName;
        }

        private void ScvodNomberForPlayCB_DropDownOpened(object sender, EventArgs e)
        {
            ScvodNomberForPlayCB.ItemsSource = App.allScvodList;
        }

        private void PersonNomberForPlayCB_DropDownOpened(object sender, EventArgs e)
        {
            if (App.PersonList.Count > 0)
            {
                PersonNomberForPlayCB.ItemsSource = null;
                App.PersonInScvod.Clear();
                foreach (Person per in App.PersonList)
                {
                    if (ScvodNomberForPlayCB.Text == per.Scvod.ToString())
                        App.PersonInScvod.Add(per.Nomber);
                }
                PersonNomberForPlayCB.ItemsSource = App.PersonInScvod;
            }


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


        private void nomberPeorsonTB_GotFocus(object sender, RoutedEventArgs e)
        {
            if (nomberPeorsonTB.Text == "Номер...")
            {
                nomberPeorsonTB.Text = "";
                nomberPeorsonTB.Foreground = Brushes.Black;
                nomberPeorsonTB.FontStyle = FontStyles.Normal;
            }

        }

        private void nomberPeorsonTB_LostFocus(object sender, RoutedEventArgs e)
        {
            if (nomberPeorsonTB.Text == "")
            {
                nomberPeorsonTB.Text = "Номер...";
                nomberPeorsonTB.Foreground = Brushes.Gray;
                nomberPeorsonTB.FontStyle = FontStyles.Italic;
            }
        }

        private void personNameTB_GotFocus(object sender, RoutedEventArgs e)
        {
            if (personNameTB.Text == "Имя...")
            {
                personNameTB.Text = "";
                personNameTB.Foreground = Brushes.Black;
                personNameTB.FontStyle = FontStyles.Normal;
            }

        }

        private void personNameTB_LostFocus(object sender, RoutedEventArgs e)
        {
            if (personNameTB.Text == "")
            {
                personNameTB.Text = "Имя...";
                personNameTB.Foreground = Brushes.Gray;
                personNameTB.FontStyle = FontStyles.Italic;
            }

        }

        private void personFamTB_GotFocus(object sender, RoutedEventArgs e)
        {
            if (personFamTB.Text == "Фамилия...")
            {
                personFamTB.Text = "";
                personFamTB.Foreground = Brushes.Black;
                personFamTB.FontStyle = FontStyles.Normal;
            }

        }

        private void personFamTB_LostFocus(object sender, RoutedEventArgs e)
        {
            if (personFamTB.Text == "")
            {
                personFamTB.Text = "Фамилия...";
                personFamTB.Foreground = Brushes.Gray;
                personFamTB.FontStyle = FontStyles.Italic;
            }

        }

        private void allScvodTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

            if (App.NomberMore0Int(allScvodTextBox.Text))
            {
                Person.allScvod = int.Parse(allScvodTextBox.Text);
                App.allScvodList.Clear();
                for (int i = 1; i <= Person.allScvod; i++)
                {
                    int str = i;
                    App.allScvodList.Add(str);
                }

            }

            scvodCB.ItemsSource = App.allScvodList;



        }

        private void midddleNameTB_GotFocus(object sender, RoutedEventArgs e)
        {
            if (midddleNameTB.Text == "Отчество...")
            {
                midddleNameTB.Text = "";
                midddleNameTB.Foreground = Brushes.Black;
                midddleNameTB.FontStyle = FontStyles.Normal;
            }

        }

        private void midddleNameTB_LostFocus(object sender, RoutedEventArgs e)
        {
            if (midddleNameTB.Text == "")
            {
                midddleNameTB.Text = "Отчество...";
                midddleNameTB.Foreground = Brushes.Gray;
                midddleNameTB.FontStyle = FontStyles.Italic;
            }

        }

        private void allScvodTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (allScvodTextBox.Text == "1")
            {
                allScvodTextBox.Text = "";
                allScvodTextBox.Foreground = Brushes.Black;
                allScvodTextBox.FontStyle = FontStyles.Normal;
            }
        }

        private void allScvodTextBox_LostFocus(object sender, RoutedEventArgs e)
        {

        }



        private void nomberPeorsonTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            if ((nomberPeorsonTB.Text != "Номер...") && (nomberPeorsonTB.Text != ""))
                if (!App.NomberMore0Int(nomberPeorsonTB.Text))
                {
                    MessageBox.Show("Номер должен быть числом", "Ошибка!");
                    nomberPeorsonTB.Text = "";
                }
        }

        private void addPersonBut_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void addPersonBut_Click(object sender, RoutedEventArgs e)
        {
            if (App.NomberMore0Int(nomberPeorsonTB.Text) && (App.NomberMore0Int(scvodCB.Text)))
            {
                if (OriginalNomber(nomberPeorsonTB.Text))
                {
                    App.PersonList.Add(new Person(int.Parse(nomberPeorsonTB.Text), int.Parse(scvodCB.Text), personNameTB.Text, personFamTB.Text, midddleNameTB.Text));
                    personLB.ItemsSource = App.PersonList;
                    string path = @"file.txt";
                    using (StreamWriter st = new StreamWriter(path, true, System.Text.Encoding.UTF8))
                    {

                        string str = nomberPeorsonTB.Text + " " + scvodCB.Text + " " + personNameTB.Text + " " + personFamTB.Text + " " + midddleNameTB.Text;

                        st.WriteLine(str);
                    }
                    midddleNameTB.Text = "Отчество...";
                    midddleNameTB.Foreground = Brushes.Gray;
                    midddleNameTB.FontStyle = FontStyles.Italic;
                    personFamTB.Text = "Фамилия...";
                    personFamTB.Foreground = Brushes.Gray;
                    personFamTB.FontStyle = FontStyles.Italic;
                    personNameTB.Text = "Имя...";
                    personNameTB.Foreground = Brushes.Gray;
                    personNameTB.FontStyle = FontStyles.Italic;
                    nomberPeorsonTB.Text = "Номер...";
                    nomberPeorsonTB.Foreground = Brushes.Gray;
                    nomberPeorsonTB.FontStyle = FontStyles.Italic;
                }
                else MessageBox.Show("Вводимый номер участника уже есть!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);


            }
            else if (!App.NomberMore0Int(nomberPeorsonTB.Text)) MessageBox.Show("Номер должен быть числом!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning); else MessageBox.Show("Сквод не должен быть пустым!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
        }


        private void ExerciseNameForPlayCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            String selectedItem = (String)comboBox.SelectedItem;  
            if (App.ExerciseList.Count > 0)
            {
                //foreach (Exercise ex in App.ExerciseList)
                //{
                //    if (ex.Name == selectedItem)
                //    {
                //        App.exercisePlay = ex;// Определяем упражнение для выполнения по совпадению выб имени

                //        string nameLoadFile = "Exercise/" + App.exercisePlay.Name + ".txt";//Формирование имени файла с данными
                //        App.LoadTargetForPlay(nameLoadFile, ex.StartLatitude, ex.StartLongitude);//Загрузка данных из требуемого файла в коллекции для выполнения упражнения
                //        Shows.StartTimeRefresh(true);//Обновление времён начала показов
                //        ProgressExerciseDG.ItemsSource = null;
                //        ProgressExerciseDG.ItemsSource = App.ShowsPlayList;// 
                //        AboutExercisePlayTB.Text = App.exercisePlay.Description;
                //        AllTimeForPlayTB.Text = App.AllTimeExercise(true).ToString();
                //        App.exercisePlay.AllTime = App.AllTimeExercise(true);
                //        DrawGant1();
                //        TimeNow.Maximum = App.exercisePlay.AllTime;
                //        TimeNow.VerticalAlignment = VerticalAlignment.Top;
                //        TimeNow.Height = 35 * App.TargetPlayList.Count;


                //    }
                //}
                CleanExercise(selectedItem);

            }
        }



        private void ExerciseNameForPlayCB_DropDownClosed(object sender, EventArgs e)
        {
            
        }

        private void SaveResoultPlayBut_Click(object sender, RoutedEventArgs e)
        {
            string Name="NoName";
            int Scvod = 0;

            if (PersonNomberForPlayCB.Text == "")
            {
                MessageBoxResult messageBoxResult = MessageBox.Show("Не выбран Участник! Сохранить результаты?", "Внимание!", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No);
                if (messageBoxResult == MessageBoxResult.No)
                {
                    return;
                }
            }
            else
            {
                Name = PersonNomberForPlayCB.Text;
                Scvod = int.Parse(ScvodNomberForPlayCB.Text);
            }
            if (!App.exercisePlay.IsActiv)
            {
                App.resoultList.Add(new Resoult(ExerciseNameForPlayCB.Text, Name, Scvod, Convert.ToDateTime(startTime.Text), CommentPlayTB.Text, App.ShowsPlayList));
                CleanExercise(ExerciseNameForPlayCB.Text); //Подготовка формы упражнения для нового участника
                SaveResoultPlayBut.IsEnabled = false;
                ResetResoultPlayBut.IsEnabled = false;
            }
        }

        public void CleanExercise(string name)
        {
            foreach (Exercise ex in App.ExerciseList)
            {
                if (ex.Name == name)
                {
                    App.exercisePlay = ex;// Определяем упражнение для выполнения по совпадению выб имени

                    string nameLoadFile = "Exercise/" + App.exercisePlay.Name + ".txt";//Формирование имени файла с данными
                    App.LoadTargetForPlay(nameLoadFile, ex.StartLatitude, ex.StartLongitude);//Загрузка данных из требуемого файла в коллекции для выполнения упражнения
                    Shows.StartTimeRefresh(true);//Обновление времён начала показов
                    ProgressExerciseDG.ItemsSource = null;
                    ProgressExerciseDG.ItemsSource = App.ShowsPlayList;// 
                    AboutExercisePlayTB.Text = App.exercisePlay.Description;
                    AllTimeForPlayTB.Text = App.AllTimeExercise(true).ToString();
                    App.exercisePlay.AllTime = App.AllTimeExercise(true);
                    DrawGant1();
                    TimeNow.Maximum = App.exercisePlay.AllTime;
                    TimeNow.VerticalAlignment = VerticalAlignment.Top;
                    TimeNow.Height = 35 * App.TargetPlayList.Count;
                    SartBut.IsEnabled = true;
                    buttonCaption.FontSize = 72;
                    buttonCaption.Text = "СТАРТ";
                    CommentPlayTB.Text = "";
                }
            }
            TimeNow.Value = 0;
            PersonNomberForPlayCB.Text = "";
        }

        private bool OriginalNomber(string newNomber)
        {

            foreach (Person man in App.PersonList)
                if (man.Nomber.ToString() == newNomber) return false;
            return true;
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (ExerciseNameForPlayCB.Text!="")
            DrawGant1();
        }

        private void StopPlayBut_Click(object sender, RoutedEventArgs e)
        {
            if (App.exercisePlay != null) 
            { 
                App.exercisePlay.IsActiv = false;
                SartBut.IsEnabled = true;
                SaveResoultPlayBut.IsEnabled = true;
                ResetResoultPlayBut.IsEnabled = true;
            }
        }

        //Отрисовка диаграммы ганта
        private void DrawGant()
        {
            Gant.Children.Clear();

            double longGant = Gant.ActualWidth - 150;
            double step = longGant / App.exercisePlay.AllTime;

            for (int i = 0; i < App.exercisePlay.AllTime; i++)
            {
                Line line = new Line();
                line.X1 = 150;
                line.Y1 = 0;
                line.X2 = 150;
                line.Y2 = 35 * App.ShowsPlayList.Count;
                line.Stroke = Brushes.Gray;
                line.StrokeThickness = 0.5;
                TranslateTransform TranslateLine = new TranslateTransform();
                TranslateLine.X = step * i;
                line.RenderTransform = TranslateLine;
                Gant.Children.Add(line);
            }
            for (int i = 0; i < App.ShowsPlayList.Count; i++)
            {
                Rectangle rec = new Rectangle();
                rec.Width = Gant.ActualWidth;
                rec.Height = 35;
                rec.Stroke = Brushes.Black;
                rec.StrokeThickness = 0.5;
                TranslateTransform myTranslate = new TranslateTransform();
                myTranslate.Y = rec.Height * i;
                Label label = new Label();
                label.Content = "Показ № " + (i + 1);
                label.FontFamily = new FontFamily("Segoe UI");
                label.FontWeight = FontWeights.Bold;
                rec.RenderTransform = myTranslate;
                label.RenderTransform = myTranslate;
                Gant.Children.Add(rec);
                Gant.Children.Add(label);
                Rectangle recShow = new Rectangle();
                recShow.Fill = Brushes.Gray;
                recShow.Stroke = Brushes.White;
                recShow.StrokeThickness = 3;
                recShow.Width = step * App.ShowsPlayList[i].ShowTimeSec;
                recShow.Height = 35;
                TranslateTransform translate = new TranslateTransform();
                translate.Y = rec.Height * i;
                translate.X = 150 + App.ShowsPlayList[i].StartTime * step;
                recShow.RenderTransform = translate;
                Gant.Children.Add(recShow);

            }


        }

        //Отрисовка диаграммы соревнований, где по вертикали мишени, а по горизонтали показы
        private void DrawGant1()
        {
            Gant.Children.Clear();//Очищаем поле перед новым рисунком

            double longGant = Gant.ActualWidth - 150;//Вычисляется актуальная длина поля
            double step = longGant / App.exercisePlay.AllTime;//Вычисляется размер шага, соответствующий 1 сек

            //Отрисовываются вертикальные линии секундной сетки
            for (int i = 0; i < App.exercisePlay.AllTime; i++)
            {
                Line line = new Line();
                line.X1 = 150;
                line.Y1 = 35;
                line.X2 = 150;
                line.Y2 = 35 * App.TargetPlayList.Count+35;
                line.Stroke = Brushes.Gray;
                line.StrokeThickness = 0.5;
                TranslateTransform TranslateLine = new TranslateTransform();
                TranslateLine.X = step * i;
                line.RenderTransform = TranslateLine;
                Gant.Children.Add(line);
            }
            DrawTimeScal(step);
            //Отрисовка основной части диаграммы
            for (int i =0; i < App.ShowsPlayList.Count; i++)
            {
                //Отрисовка шапки
                Rectangle rec = new Rectangle();
                rec.Width = (App.ShowsPlayList[i].PreTimeSec+ App.ShowsPlayList[i].ShowTimeSec)*step;
                rec.Height = 35+35*App.TargetPlayList.Count;
                rec.Stroke = Brushes.Black;
                rec.StrokeThickness = 0.2;
                TranslateTransform myTranslate = new TranslateTransform();
                myTranslate.X = 150+ (App.ShowsPlayList[i].StartTime- App.ShowsPlayList[i].PreTimeSec) * step; 
                Label label = new Label();
                label.Content = "Показ  " + App.ShowsPlayList[i].Serial;
                label.FontFamily = new FontFamily("Segoe UI");
                label.FontWeight = FontWeights.Bold;
                rec.RenderTransform = myTranslate;
                label.RenderTransform = myTranslate; 
                Gant.Children.Add(rec);
                Gant.Children.Add(label);

                //Отрисовка прямоугольников, соответствующих показам
                Rectangle recShow = new Rectangle();
                recShow.Fill = Brushes.Gray;
                recShow.Stroke = Brushes.White;
                recShow.StrokeThickness = 3;
                recShow.Width = step * App.ShowsPlayList[i].ShowTimeSec;
                recShow.Height = 35;
                TranslateTransform translate = new TranslateTransform();
                for (int j=0;j<App.TargetPlayList.Count;j++)
                    if (App.TargetPlayList[j].NameTarget==App.ShowsPlayList[i].Target)
                translate.Y = 35+35 * j;
                translate.X = 150 + App.ShowsPlayList[i].StartTime * step;
                recShow.RenderTransform = translate;
                Gant.Children.Add(recShow);

                //Отрисовка размерных линий
                TranslateTransform translate1 = new TranslateTransform();
                translate1.X = 150 + (App.ShowsPlayList[i].StartTime - App.ShowsPlayList[i].PreTimeSec) * step;
                for (int j = 0; j < App.TargetPlayList.Count; j++)
                    if (App.TargetPlayList[j].NameTarget == App.ShowsPlayList[i].Target)
                        translate1.Y = 35 + 35 * j;
                TranslateTransform translatelabelPre = new TranslateTransform();
                translatelabelPre.X = 150 + (App.ShowsPlayList[i].StartTime - App.ShowsPlayList[i].PreTimeSec) * step+  App.ShowsPlayList[i].PreTimeSec * step/2.5;
                for (int j = 0; j < App.TargetPlayList.Count; j++)
                    if (App.TargetPlayList[j].NameTarget == App.ShowsPlayList[i].Target)
                        translatelabelPre.Y = 30 + 35 * j;
                TranslateTransform translatelabeShow = new TranslateTransform();
                translatelabeShow.X = 150 + App.ShowsPlayList[i].StartTime * step+ App.ShowsPlayList[i].ShowTimeSec*step/2.5;
                for (int j = 0; j < App.TargetPlayList.Count; j++)
                    if (App.TargetPlayList[j].NameTarget == App.ShowsPlayList[i].Target)
                        translatelabeShow.Y = 30 + 35 * j;
                Label labelPre = new Label();
                Label labelShow = new Label();
                labelPre.Content = App.ShowsPlayList[i].PreTimeSec;
                labelPre.RenderTransform = translatelabelPre;
                labelPre.RenderTransform = translatelabelPre;
                labelPre.Foreground = Brushes.Red;
                labelShow.Content = App.ShowsPlayList[i].ShowTimeSec;
                labelShow.RenderTransform = translatelabeShow;
                
                Gant.Children.Add(labelPre);
                Gant.Children.Add(labelShow);
                DrawDimensionLine(Convert.ToInt32(App.ShowsPlayList[i].PreTimeSec * step), 20, translate1, Brushes.Blue);
                DrawDimensionLine(Convert.ToInt32(App.ShowsPlayList[i].ShowTimeSec*step),20, translate, Brushes.White);
            }

            //Отрисовка наименований мишеней по вертикали
            for (int i = 0; i < App.TargetPlayList.Count; i++)
            {
                Rectangle rec = new Rectangle();
                rec.Width = Gant.ActualWidth;
                rec.Height = 35;
                rec.Stroke = Brushes.Black;
                rec.StrokeThickness = 0.5;
                TranslateTransform myTranslate = new TranslateTransform();
                myTranslate.Y = rec.Height * (i+1);
                Label label = new Label();
                label.Content = "Мишень  " + App.TargetPlayList[i].NameTarget;
                label.FontFamily = new FontFamily("Segoe UI");
                label.FontWeight = FontWeights.Bold;
                rec.RenderTransform = myTranslate;
                label.RenderTransform = myTranslate;
                Gant.Children.Add(rec);
                Gant.Children.Add(label);
            }
            
        }

        public void DrawTimeScal(double step)
        {
            for (int i=0; i < App.exercisePlay.AllTime; i++)
            {
                Label label = new Label();
                Line line = new Line();
                line.X1 = 150;
                
                line.X2 = 150;
                if (i % 10 == 0)
                {
                    line.Y1 = 35;
                    line.Y2 = 35 * App.TargetPlayList.Count+35+15;
                    line.StrokeThickness = 1.5;
                    label.Content = i;
                    TranslateTransform TranslateLable = new TranslateTransform();
                    TranslateLable.X = 140+step * i;
                    TranslateLable.Y = line.Y2 ;
                    label.RenderTransform = TranslateLable;
                    Gant.Children.Add(label);
                } else
                {
                    line.Y1 = 35 * App.TargetPlayList.Count + 35;
                    line.Y2 = line.Y1 + 10;
                    line.StrokeThickness = 0.5;
                }
                
                line.Stroke = Brushes.Black;
                
                TranslateTransform TranslateLine = new TranslateTransform();
                TranslateLine.X = step * i;
                line.RenderTransform = TranslateLine;
                Gant.Children.Add(line);
            }
        }



        private void ResetResoultPlayBut_Click(object sender, RoutedEventArgs e)
        {
            CleanExercise(ExerciseNameForPlayCB.Text); //Подготовка формы упражнения для нового участника
            SaveResoultPlayBut.IsEnabled = false;
            ResetResoultPlayBut.IsEnabled = false;
        }

        private void portsCB_DropDownOpened(object sender, EventArgs e)
        {

        string[] ports = SerialPort.GetPortNames();
        portsCB.ItemsSource = ports;
    }

        private void FilterCB_DropDownOpened(object sender, EventArgs e)
        {
            App.ExerciseListForFilter.Clear();
            if (ResoltGrid.DataContext== App.resoultList)
            {
                foreach (Resoult res in App.resoultList)
                            {
                                if (App.ExerciseListForFilter.Contains(res.Exercise)==false)
                                    App.ExerciseListForFilter.Add(res.Exercise);

                            }
            }
            if (ResoltGrid.DataContext == App.resoultListFilter)
            {
                foreach (Resoult res in App.resoultListFilter)
                {
                    if (App.ExerciseListForFilter.Contains(res.Exercise) == false)
                        App.ExerciseListForFilter.Add(res.Exercise);

                }
            }
            App.ExerciseListForFilter.Add("Все упражнения");
            FilterCB.ItemsSource = null;
            FilterCB.ItemsSource = App.ExerciseListForFilter;
        }

        private void FilterCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            String selectedItem = (String)comboBox.SelectedItem;
            if (selectedItem != "")
            {
                if (selectedItem== "Все упражнения") ResoltGrid.DataContext = App.resoultList; else
            {
                App.resoultListFilter.Clear();
                foreach (Resoult res in App.resoultList)
                {
                 if (res.Exercise== selectedItem)
                    {
                    App.resoultListFilter.Add(res);
                    }
                }
                ResoltGrid.DataContext = App.resoultListFilter;

            }
            
            }            
        }

        private void SaveResoltToExele_Click(object sender, RoutedEventArgs e)
        {
           
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "Document"; // Default file name
            dlg.DefaultExt = ".csv"; // Default file extension
            string filename;
            // Show save file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process save file dialog box results
            if (result == true)
            {
                // Save document
                 filename = dlg.FileName;
                if (ResoltGrid.DataContext == App.resoultList)
            {
                foreach (Resoult res in App.resoultList)
                {
                    SaveInCsv(res.PersonNomber, res.Scvod, res.StartTime, res.Summ);
                }
            }
            if (ResoltGrid.DataContext == App.resoultListFilter)
            {
                foreach (Resoult res in App.resoultListFilter)
                {
                    SaveInCsv(res.PersonNomber, res.Scvod, res.StartTime, res.Summ);
                }
            }
            CsvExport.ExportToFile(filename);
            }
            
                
        }

        void SaveInCsv(string person, int scvod, string starttime, int points)
        {
            CsvExport.AddRow();
            CsvExport["Участник"] = person;
            CsvExport["ФИО"] = person;
            CsvExport["Сквод"] = scvod;
            CsvExport["Время начала упражнения"] = starttime;
            CsvExport["Очки"] = points;
            
        }

       
        private void Test_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var temp = indicationDG.SelectedItem as IndicationBlock;
                String messs = temp.Nomber.ToString() + "3" + "05" + 'n';
                if (ArduinoPort.IsOpen)
                    for (int i = 0; i < 3; i++) { 
                ArduinoPort.Write(messs);


                    }
               
                dataSerialTB.Text = messs;
                
            }
            catch { MessageBox.Show("О-оу"); }
        }

        private void sensorsDG_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            

        }

        private void indicationDG_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            //if (e.Column.DisplayIndex == 5)
            //{
            //    var temp = e.Row.Item as IndicationBlock;
            //    String messs = temp.Nomber.ToString() + "3" + "05" + 'n';
            //    if (ArduinoPort.IsOpen)
            //        ArduinoPort.Write(messs);
            //    dataSerialTB.Text = messs;
            //}
        }

        private void indicationDG_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //if (e.Column.DisplayIndex == 5)
            //{
            //    var temp = indicationDG.SelectedItem as IndicationBlock;
            //    String messs = temp.Nomber.ToString() + "3" + "05" + 'n';
            //    if (ArduinoPort.IsOpen)
            //        ArduinoPort.Write(messs);
            //    dataSerialTB.Text = messs;
            //}
        }

        //Метод отрисовки размерных линий
        public void DrawDimensionLine(int x, int y,Transform transform, Brush brushes) 
        {
            GeometryGroup group = new GeometryGroup();
            Path p = new Path();
            LineGeometry line1 = new LineGeometry();
            LineGeometry line2 = new LineGeometry();
            LineGeometry line3 = new LineGeometry();
            LineGeometry line4 = new LineGeometry();
            LineGeometry line5 = new LineGeometry();
            line1.StartPoint = new Point(0, 20);
            line1.EndPoint = new Point(x, y);
            line2.StartPoint = new Point(0, 20);
            line2.EndPoint = new Point(10, 15);
            line3.StartPoint = new Point(0, 20);
            line3.EndPoint = new Point(10, 25);
            line4.StartPoint = new Point(x, y);
            line4.EndPoint = new Point(x-10,y-5);
            line5.StartPoint = new Point(x, y);
            line5.EndPoint = new Point(x-10, y+5);
            group.Children.Add(line1);
            group.Children.Add(line2);
            group.Children.Add(line3);
            group.Children.Add(line4);
            group.Children.Add(line5);
            p.Data = group;
            p.Stroke = brushes;
            p.RenderTransform = transform;
            Gant.Children.Add(p);

        }
        
        




    }
}
