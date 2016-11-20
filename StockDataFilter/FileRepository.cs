using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Next steps:
// 1. Tabs with separate files DONE
// 2. Option to replace something in a particular column
// 3. Selecting columns to be exported into one DONE
// 4. Longest common sequence of selected column DONE
// 5. Remove file DONE
// 6. Labels DONE
// 7. Default options DONE
//

namespace StockDataFilter
{
    class FileRepository
    {
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
            if (!File.Exists(_filename))
            {
                throw new FileNotFoundException("Nie znaleziono pliku", _filename);
            }

            var separators = new string[] { _separator };
            var lines = File.ReadAllLines(_filename);
            var header = lines.First().Split(separators, StringSplitOptions.None);
          
            header = ResolveDuplicateFields(header);

            var entries = from l in lines.Skip(1)
                          select l.Split(separators, StringSplitOptions.None).ToArray();

            return new RawStockData(_filename, header, entries);
        }

        public IEnumerable<RawStockData> ReadDataFromFiles(string[] _filenames, string _separator)
        {
            return from fn in _filenames
                   select ReadDataFromFile(fn, _separator); ;
        }
    }
}
