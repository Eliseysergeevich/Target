using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Target_CNC_GC_08_04_20
{
    
    public class Person
    {
        static public int allScvod;
        private int nomber;
        private int scvod;
        private string name;
        private string fam;
        private string middleName;
        private string gun;

       ObservableCollection<Person> PersonList = new ObservableCollection<Person>();

        public Person(int nomber, int scvod, string name, string fam, string middleName  ) {
            Name = name;
            Fam = fam;
            MiddleName = middleName;
            Nomber = nomber;
            Scvod = scvod;

            }
        
        public string Name {
            get { return name; }
            set { name = value; }
        }
        public string Fam
        {
            get { return fam; }
            set { fam = value; }
        }
        public string MiddleName
        {
            get { return middleName; }
            set { middleName=value; }
        }
        public string Gun
        {
            get { return gun; }
            set { gun=value; }
        }
        public int Scvod
        {
            get { return scvod; }
            set { scvod=value; }
        }
        public int Nomber
        {
            get { return nomber; }
            set { nomber = value; }
        }
    }
   



}
