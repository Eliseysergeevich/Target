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
        System.Timers.Timer aTimer;
        int count;
        public MainWindow()
        {
            InitializeComponent();
        
            temp.WindowsName = WindowsViews.WindowsView.situation;
            string[] ports = SerialPort.GetPortNames();
            portsCB.ItemsSource = ports;

            DispatcherTimer timer = new DispatcherTimer();
            timer.Tick += new EventHandler(timer_Tick);
            timer.Tick += new EventHandler(timerCOM_Tick);
            timer.Interval = new TimeSpan(0, 0, 0, 0, 20);
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
        public void AddCount()
        {
            count++;
            TimeNow.Value = count;
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
           // ArduinoPort.ReceivedBytesThreshold = 16;
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

        private  void DataReceivedHandler(
                       object sender,
                       SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            string indata = sp.ReadExisting();
            dataSerialTB.Dispatcher.Invoke(new updateDelegate(updateTextBox), indata);

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
        
    }
    

}
