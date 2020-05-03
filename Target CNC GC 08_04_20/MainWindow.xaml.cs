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
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    /// 
    
    class WindowsViews
    {
        public enum WindowsView
        {
            situation,
            qwe,
            tyu,
            jkl

        }

       public WindowsView WindowsName { get; set; }

    }
   

    public partial class MainWindow : Window
    {
        WindowsViews temp = new WindowsViews();
        SerialPort ArduinoPort = new SerialPort();
        private delegate void updateDelegate(string txt);
        ObservableCollection<Sensors> SensorsList = new ObservableCollection<Sensors> { }; //Коллекция блоков датчиков
        ObservableCollection<IndicationBlock> IndicationList = new ObservableCollection<IndicationBlock> { }; //Коллекция блоков индикации
        int count;
        public MainWindow()
        {
            InitializeComponent();
        
            temp.WindowsName = WindowsViews.WindowsView.situation;
            string[] ports = SerialPort.GetPortNames();
            portsCB.ItemsSource = ports;

            DispatcherTimer timer = new DispatcherTimer(DispatcherPriority.Render);
            
            timer.Tick += new EventHandler(timer_Tick);
           
            // timer.Tick += new EventHandler(timerCOM_Tick);
            timer.Interval = new TimeSpan(0, 0, 0, 0,100);
            timer.Start();

            //aTimer = new System.Timers.Timer(1500);
            //aTimer.Elapsed += timerCOM_Tick;
            //aTimer.AutoReset = true;
            //aTimer.Enabled = true;

            //DispatcherTimer timerCOM = new DispatcherTimer();
            //timerCOM.Tick += new EventHandler(timerCOM_Tick);
            //timerCOM.Interval = new TimeSpan(0, 0, 0, 0, 20);
            //timerCOM.Start();

            //ArduinoPort.PortName = portsCB.Text;
            //ArduinoPort.BaudRate = 9600;
            //ArduinoPort.ReceivedBytesThreshold = 40;
            ////ArduinoPort.RtsEnable = true;
            ////ArduinoPort.ReadTimeout = 1000;
            //ArduinoPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
            //try
            //{
            //    ArduinoPort.Open();


            //}
            //catch
            //{
            //    MessageBox.Show("Неудачно");
            //}
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            count++;
            TimeNow.Value = count;
            if (count == 100) count = 0;
        }
       

        private void timerCOM_Tick(object sender, EventArgs e)
        {
            //if (ArduinoPort.IsOpen)
            //{
            //    try
            //    {
            //        // ArduinoPort.DiscardInBuffer();
            //        string returnMessage = ArduinoPort.ReadLine();
            //        dataSerialTB.Dispatcher.BeginInvoke(new updateDelegate(updateTextBox), returnMessage);

            //        //ArduinoPort.Close();
            //    }
            //    catch { ArduinoPort.Close(); }
            //}
        }

        public  void updateTextBox(string txt)
        {
            dataSerialTB.Text = txt;
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
                            sens.VoltageP= int.Parse(inputStr[2]);
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

        private void Exercise_Click(object sender, RoutedEventArgs e)
        {
            temp.WindowsName = WindowsViews.WindowsView.qwe;
        }

       
        private void Competitions_Click(object sender, RoutedEventArgs e)
        {
            temp.WindowsName = WindowsViews.WindowsView.tyu;
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

           // PeopleWin.Visibility = Visibility.Visible;
        }

        private void conectBT_Click(object sender, RoutedEventArgs e)
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

        private  void DataReceivedHandler( object sender, SerialDataReceivedEventArgs e)
        {
            if (ArduinoPort.IsOpen) {
                try
                {
                    string indata = ArduinoPort.ReadLine();

                    dataSerialTB.Dispatcher.BeginInvoke(new updateDelegate(updateTextBox), indata);
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
        }
    }
    

}
