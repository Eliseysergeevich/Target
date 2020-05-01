using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace Target_CNC_GC_08_04_20
{
    /// <summary>
    /// Логика взаимодействия для People.xaml
    /// </summary>
    public partial class PeopleWin : Window
    {
        ObservableCollection<string> allScvodList;
        ObservableCollection<Person> PersonList;
        public PeopleWin()
        {
            InitializeComponent();
            PersonList = new ObservableCollection<Person> { };
            allScvodList = new ObservableCollection<string> { };
            allScvodTextBox.Text = "1";
            scvodCB.ItemsSource = allScvodList;
            string path = @"file.txt";
            using (StreamReader st = new StreamReader(path, System.Text.Encoding.Default))
            {
                string line;
                
                while ((line = st.ReadLine()) != null)
                {
                    line=line.Trim();
                    string[] words = line.Split(new char[] { ' ' });
                    if (words.Length == 5)
                    {
                        PersonList.Add(new Person(int.Parse(words[0]), int.Parse(words[1]), words[2], words[3], words[4]));
                    }
                    
                }
                personLB.ItemsSource = PersonList;
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
            if (personNameTB.Text == "Имя...") {
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
                allScvodList.Clear();
                for (int i = 1; i <= Person.allScvod; i++)
                {
                    String str = i.ToString();
                    allScvodList.Add(str);
                }

            }

            scvodCB.ItemsSource = allScvodList;



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
            if (App.NomberMore0Int(nomberPeorsonTB.Text)&&(App.NomberMore0Int(scvodCB.Text))) {
                if (OriginalNomber(nomberPeorsonTB.Text))
                {
                    PersonList.Add(new Person(int.Parse(nomberPeorsonTB.Text), int.Parse(scvodCB.Text), personNameTB.Text, personFamTB.Text, midddleNameTB.Text));
                    personLB.ItemsSource = PersonList;
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


            } else if (!App.NomberMore0Int(nomberPeorsonTB.Text)) MessageBox.Show("Номер должен быть числом!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning); else MessageBox.Show("Сквод не должен быть пустым!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private bool OriginalNomber(string newNomber)
        {
            
            foreach (Person man in PersonList)
                if (man.Nomber.ToString() == newNomber) return false;
            return true;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            string path = @"file.txt";
         
            if (File.Exists(path))
            {
                File.Delete(path);
                File.Create(path).Close();
                
            }                          
            foreach (Person man in PersonList )
                using (StreamWriter st = new StreamWriter(path, true, System.Text.Encoding.UTF8))
                {
                    string str = man.Nomber.ToString() + " " + man.Scvod.ToString() + " " + man.Name + " " + man.Fam + " " + man.MiddleName;
                    st.WriteLine(str);
                }
            App.q = false;
        }
    }
}
