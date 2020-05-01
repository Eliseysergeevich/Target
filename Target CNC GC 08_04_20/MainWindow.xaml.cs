using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Threading;
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
        int count;
        public MainWindow()
        {
            InitializeComponent();
        
            temp.WindowsName = WindowsViews.WindowsView.situation;

            DispatcherTimer timer = new DispatcherTimer();
            //timer.Tick += new EventHandler(timer_Tick);
            //timer.Interval = new TimeSpan(0, 0, 0,0,10);
            //timer.Start();
        }

        //private void timer_Tick(object sender, EventArgs e)
        //{
        //    count++;
        //    TimeNow.Value = count;
        //    if (count == 100) count = 0;
        //}
        //public void AddCount()
        //{
        //    count++;
        //    TimeNow.Value = count;
        //}


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

       

     

      
    }
    

}
