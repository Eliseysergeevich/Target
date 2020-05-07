using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace Target_CNC_GC_08_04_20
{
    /// <summary>
    /// Логика взаимодействия для Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        private string nmpLatTemp;
        private string nmpLonTemp;

        public Window1()
        {
            InitializeComponent();

        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://www.ngdc.noaa.gov/geomag/data/poles/pole_locations.txt");
        }
       
        //Коррекция долготы Северного магнитного полюса
        private void nmpLon_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (nmpLon.Text != "")
            {
                nmpLon.Text = nmpLon.Text.Replace(".", ","); //Заменяет точку на запятую
                nmpLon.Select(nmpLon.Text.Length, 0);//устанавливает курсор в конец строки
                if (!App.NomberMore0Double(nmpLon.Text))//Проверка на вещественный тип больше 0
                {
                    MessageBox.Show("Некоректный ввод", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    nmpLon.Text = nmpLonTemp;//Возвращиет в поле последнее правильное значение
                    nmpLon.Select(nmpLon.Text.Length, 0);
                }
                else
                    if (Math.Abs(double.Parse(nmpLon.Text)) > 180)
                {
                    MessageBox.Show("Широта должна быть в диапазоне от 0 до 90", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    nmpLon.Text = nmpLonTemp;
                    nmpLon.Select(nmpLon.Text.Length, 0);
                }
                nmpLonTemp = nmpLon.Text;
            }

        }

        //Коррекция широты Северного магнитного полюса
        private void nmpLat_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (nmpLat.Text != "")
            {
                nmpLat.Text = nmpLat.Text.Replace(".", ","); //Заменяет точку на запятую
                nmpLon.Select(nmpLat.Text.Length, 0);//устанавливает курсор в конец строки
                if (!App.NomberMore0Double(nmpLat.Text))//Проверка на вещественный тип больше 0
                {
                    MessageBox.Show("Некоректный ввод", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    nmpLat.Text = nmpLatTemp;//Возвращиет в поле последнее правильное значение
                    nmpLat.Select(nmpLon.Text.Length, 0);
                }
                else
                    if (Math.Abs(double.Parse(nmpLat.Text)) > 90)
                {
                    MessageBox.Show("Широта должна быть в диапазоне от 0 до 90", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    nmpLat.Text = nmpLatTemp;
                    nmpLat.Select(nmpLat.Text.Length, 0);
                }
                nmpLatTemp = nmpLat.Text;
            }
        }

        private void elNMPRB_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void wlNMPRB_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void slNMPRB_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void nlNMPRB_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void NewNMPBut_Click(object sender, RoutedEventArgs e)
        {
            if (nlNMPRB.IsChecked==true)  App.NmpLat = double.Parse(nmpLat.Text);
            else App.NmpLat = double.Parse(nmpLat.Text)*(-1);
            if (elNMPRB.IsChecked == true) App.NmpLon = double.Parse(nmpLon.Text);
            else App.NmpLon = double.Parse(nmpLon.Text) * (-1);
            MessageBox.Show("Координаты обновлены", "Внимание");
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (App.NmpLat >= 0) nlNMPRB.IsChecked = true;
            else slNMPRB.IsChecked = true;
            if (App.NmpLon >= 0) elNMPRB.IsChecked = true;
            else wlNMPRB.IsChecked = true;
            nmpLat.Text = Math.Abs(App.NmpLat).ToString();
            nmpLon.Text = Math.Abs(App.NmpLon).ToString();
        }
    }
}
