using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockDataFilter
{
    class RawStockData
    {
        public RawStockData(
            string _filename,
            string[] _fields,
            IEnumerable<string[]> _entries)
        {
            Filename = _filename;
            Fields = _fields;
            Entries = _entries.ToList();

            nameToIndex = Enumerable.Range(0, _fields.Length)
                                .ToDictionary(x => _fields[x], x => x);
        }

        public string Filename { get; set; }
        public string[] Fields { get; set; }
        public List<string[]> Entries { get; set; }

        private Dictionary<string, int> nameToIndex;
    }
}
