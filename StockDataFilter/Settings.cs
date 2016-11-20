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

        private HashSet<string> defaultAcceptedColumns = new HashSet<string>() { "Data", "Zamkniecie" };
        public HashSet<string> DefaultAcceptedColumns
        {
            get
            {
                return defaultAcceptedColumns;
            }
            set
            {
                defaultAcceptedColumns = value;
            }
        }
    }
}
