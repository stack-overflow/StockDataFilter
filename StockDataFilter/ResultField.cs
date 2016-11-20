using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockDataFilter
{
    class ResultField : INotifyPropertyChanged
    {
        public string Name { get; set; }

        private bool accepted;
        public bool Accepted
        {
            get
            {
                return accepted;
            }
            set
            {
                accepted = value;
                NotifyPropertyChanged("Accepted");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string name)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
