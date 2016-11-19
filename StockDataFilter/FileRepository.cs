using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Next steps:
// 1. Tabs with separate files
// 2. Option to replace something in a particular column
// 3. Selecting columns to be exported into one
// 4. Longest common sequence of selected column
//

namespace StockDataFilter
{
    class FileRepository
    {
        List<string> filenames = new List<string>();

        public void AddFilename(string _filename)
        {
            if(!File.Exists(_filename))
            {
                throw new FileNotFoundException("Nie znaleziono pliku", _filename);
            }
            filenames.Add(_filename);
        }

        private string[] ResolveDuplicateFields(string[] _fields)
        {
            Dictionary<string, int> count_seen_fields = new Dictionary<string, int>();
            for (int i = 0; i < _fields.Length; ++i)
            {
                string field = _fields[i];
                if (!count_seen_fields.ContainsKey(field))
                {
                    count_seen_fields[field] = 1;
                }
                else
                {
                    field += "_" + count_seen_fields[field].ToString();
                    ++count_seen_fields[field];
                    _fields[i] = field;
                }
            }
            return _fields;
        }

        private RawStockData ReadDataFromFile(string _filename, string _separator)
        {
            var separators = new string[] { _separator };
            var lines = File.ReadAllLines(_filename);
            var header = lines.First().Split(separators, StringSplitOptions.None);
          
            header = ResolveDuplicateFields(header);

            var entries = from l in lines.Skip(1)
                          select l.Split(separators, StringSplitOptions.None).ToArray();

            return new RawStockData(_filename, header, entries);
        }

        public IEnumerable<RawStockData> GetData()
        {
            return from fn in filenames
                   select ReadDataFromFile(fn, ";"); ;
        }
    }
}
