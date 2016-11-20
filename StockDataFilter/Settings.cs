using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockDataFilter
{
    class Settings
    {
        public string defaultFirstField = "Data";
        public string DefaultFirstField
        {
            get
            {
                return defaultFirstField;
            }
            set
            {
                defaultFirstField = value;
            }
        }
    }
}
