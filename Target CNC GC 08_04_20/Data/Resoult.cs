using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Target_CNC_GC_08_04_20.Data
{
    public class Resoult : ObservableCollection<Resoult>
    {
        //public static ObservableCollection<String> PlayList1 { get; set; }
        public string Exercise { get; set; }
        public string PersonNomber { get; set; }
        public int Scvod { get; set; }
        public string StartDate { get; set; }
        public string StartTime { get; set; }
        public string Comment { get; set; }
        public ObservableCollection<Shows> PlayList { get; set; }
        public int Summ { get; set; }
        public Resoult(string exercise,string nomber, int scvod, DateTime time, string comment, ObservableCollection<Shows> shows)
        {
            this.Exercise = exercise;
            this.PersonNomber = nomber;
            this.Scvod = scvod;
            this.StartTime = time.ToLongTimeString();
            this.StartDate = time.ToLongDateString();
            this.Comment = comment;
            //this.PlayList = shows;
            this.PlayList = new ObservableCollection<Shows>();
            foreach (Shows sh in shows)
            {
                this.PlayList.Add(sh);
            }
            foreach (Shows sh in this.PlayList)
            {
                if (sh.Struck)
                {
                    this.Summ += 10;
                    this.Summ += sh.ShowTimeSec - sh.StruckTime100ms;
                }
            }
            
        }

       
    }
}
