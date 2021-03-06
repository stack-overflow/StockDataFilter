﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockDataFilter
{
    class RawStockData : INotifyPropertyChanged
    {
        public string Filename { get; set; }
        public string[] Fields { get; set; }
        public List<string[]> Entries { get; set; }

        private Dictionary<string, int> nameToIndex;

        public event PropertyChangedEventHandler PropertyChanged;

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

        public int NameToIndex(string _name)
        {
            return nameToIndex[_name];
        }

        public string FieldByName(string[] _entry, string _name)
        {
            return _entry[nameToIndex[_name]];
        }

        public override string ToString()
        {
            return Filename;
        }
    }
}
