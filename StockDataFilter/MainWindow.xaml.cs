using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.IO;

namespace StockDataFilter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Settings settings = new Settings();
        FileRepository fileRepo = new FileRepository();
        private ObservableCollection<RawStockData> files = new ObservableCollection<RawStockData>();
        private ObservableCollection<ResultField> resultFields = new ObservableCollection<ResultField>();
        private RawStockData resultFile = null;

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            this.filesListBox.ItemsSource = files;
            this.resultFieldsListBox.ItemsSource = resultFields;
            this.firstFieldComboBox.ItemsSource = resultFields;
        }

        private string[] CollectFilenames()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            if (openFileDialog.ShowDialog() == true)
            {
                return openFileDialog.FileNames;
            }
            return null;
        }

        private void ClearDataGrid(DataGrid grid)
        {
            grid.Columns.Clear();
            grid.ItemsSource = null;
        }

        private void FillDataGrid(DataGrid grid, RawStockData data)
        {
            ClearDataGrid(grid);

            if (data != null)
            {
                for (int i = 0; i < data.Fields.Length; ++i)
                {
                    var col = new DataGridTextColumn();
                    col.Header = data.Fields[i];
                    col.Binding = new Binding(string.Format("[{0}]", i));
                    grid.Columns.Add(col);
                }
                grid.ItemsSource = data.Entries;
            }
        }

        private void FillOriginalDataGrid(RawStockData data)
        {
            FillDataGrid(originalDataGrid, data);
        }

        private void FillResultDataGrid(RawStockData data)
        {
            FillDataGrid(resultDataGrid, data);
        }

        private void FillOriginalListBoxItemsSource(IEnumerable<RawStockData> _data)
        {
            foreach (var d in _data)
            {
                files.Add(d);
            }
        }

        private void UpdateResultFieldsList()
        {
            var filesFields = from f in files
                               select f.Fields;
            
            if (!filesFields.Any())
            {
                resultFields.Clear();
                return;
            }

            var commonFields = filesFields.First().ToArray();
            foreach (var fields in filesFields.Skip(1))
            {
                commonFields = Utils.LongestCommonSubsequence(commonFields, fields);
            }

            var result = resultFields.Where(f => !commonFields.Any(f2 => f2 == f.Name));
            foreach (var r in result)
            {
                resultFields.Remove(r);
            }

            foreach (var field in commonFields)
            {
                // Don't add the field if it is already present on the list
                if (!resultFields.Where(f => f.Name.Equals(field)).Any())
                {
                    var resultField = new ResultField() { Name = field, Accepted = settings.DefaultAcceptedColumns.Contains(field) };
                    resultFields.Add(resultField);

                    if (resultField.Name == settings.DefaultFirstField)
                    {
                        firstFieldComboBox.SelectedItem = resultField;
                    }
                }
            }
        }

        private void filesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FillOriginalDataGrid((sender as ListBox).SelectedItem as RawStockData);
        }

        private void OpenCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void OpenCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            string[] filenames = CollectFilenames();
            if(filenames != null)
            {
                var sep = (inputSeparatorComboBox.SelectedValue as ComboBoxItem).Content as string;
                sep = sep == null ? ";" : sep;
                var data = fileRepo.ReadDataFromFiles(filenames, sep);

                FillOriginalListBoxItemsSource(data);
                filesListBox.SelectedIndex = 0;
                UpdateResultFieldsList();
            }
        }

        private void SaveCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = resultFile != null;
        }

        private void SaveCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.DefaultExt = ".csv";
            if (saveFileDialog.ShowDialog() == true)
            {
                string filename = saveFileDialog.FileName;
                using (StreamWriter outputFile = new StreamWriter(filename))
                {
                    outputFile.WriteLine(String.Join(";", resultFile.Fields));
                    foreach (string[] entry in resultFile.Entries)
                    {
                        outputFile.WriteLine(String.Join(";", entry));
                    }
                }
            }
        }

        private void generateResultButton_Click(object sender, RoutedEventArgs e)
        {
            resultDataGrid.Columns.Clear();
            resultDataGrid.ItemsSource = null;

            string leadingColumnName = ((firstFieldComboBox.SelectedItem) as ResultField).Name;
            IEnumerable<ResultField> acceptedFields = resultFields.Where(rf => rf.Accepted == true);
            IEnumerable<ResultField> acceptedFieldsNoLeadingCol = resultFields.Where(rf => rf.Accepted == true).Where(f => f.Name != leadingColumnName);
            List<string[]> filesLeadingColumns = new List<string[]>();
            foreach (var file in files)
            {
                var col = from entry in file.Entries
                          select file.FieldByName(entry, leadingColumnName);
                          //select entry[file.NameToIndex(leadingColumn)];

                filesLeadingColumns.Add(col.ToArray());
            }

            string[] commonLeadingColumn = filesLeadingColumns.First();
            foreach (string[] col in filesLeadingColumns.Skip(1))
            {
                commonLeadingColumn = Utils.LongestCommonSubsequence(commonLeadingColumn, col);
            }

            
            RawStockData[] filesArray = files.ToArray();
            string[,] result = new string[commonLeadingColumn.Length, acceptedFieldsNoLeadingCol.ToArray().Length * filesArray.Length + 1];
            int[] fileIterators = new int[filesArray.Length];
            for (int i = 0; i < commonLeadingColumn.Length; ++i)
            {
                result[i, 0] = commonLeadingColumn[i];
            }
            for (int i = 0; i < commonLeadingColumn.Length; ++i)
            {
                int fill_id = 1;
                for (int j = 0; j < filesArray.Length; ++j)
                {
                    while (fileIterators[j] < filesArray[j].Entries.ToArray().Length &&
                        !filesArray[j].FieldByName(filesArray[j].Entries[fileIterators[j]], leadingColumnName).Equals(result[i, 0]))
                    {
                        ++fileIterators[j];
                    }
                    foreach (ResultField acceptedField in acceptedFieldsNoLeadingCol.Where(f => f.Name != leadingColumnName))
                    {
                        result[i, fill_id] = filesArray[j].FieldByName(filesArray[j].Entries[fileIterators[j]], acceptedField.Name);
                        ++fill_id;
                    }
                }
            }

            List<string> finalFields = new List<string>();
            finalFields.Add(leadingColumnName);

            foreach (var file in files)
            {
                foreach (ResultField field in acceptedFieldsNoLeadingCol)
                {
                    finalFields.Add(System.IO.Path.GetFileName(file.Filename) + "_" + field.Name);
                }
            }
            List<string[]> finalEntries = new List<string[]>();
            for (int i = 0; i < result.GetLength(0); ++i)
            {
                finalEntries.Add(new string[result.GetLength(1)]);
                for (int j = 0; j < result.GetLength(1); ++j)
                {
                    finalEntries.Last()[j] = result[i, j];
                }
            }
            resultFile = new RawStockData("result.csv", finalFields.ToArray(), finalEntries);
            FillResultDataGrid(resultFile);
        }

        private void generateResultButton_Click1(object sender, RoutedEventArgs e)
        {
            resultDataGrid.Columns.Clear();
            resultDataGrid.ItemsSource = null;
            List<string> fields = new List<string>();

            foreach (ResultField field in resultFields)
            {
                if (field.Accepted)
                {
                    foreach (var file in files)
                    {
                        fields.Add(field.Name);
                        var col = new DataGridTextColumn();
                        col.Header = file.Filename + "_" + field.Name;
                        resultDataGrid.Columns.Add(col);
                    }
                }
            }

            string firstColumn = ((firstFieldComboBox.SelectedItem) as ResultField).Name;
            List<string[]> allRows = new List<string[]>();
            foreach (var file in files)
            {
                var dates = from entry in file.Entries
                            select entry[file.NameToIndex(firstColumn)];

                allRows.Add(dates.ToArray());
            }
            string[] common = allRows.First();
            foreach (string[] rows in allRows.Skip(1))
            {
                common = Utils.LongestCommonSubsequence(common, rows);
            }

            List<string[]> result = new List<string[]>();
            foreach (string c in common)
            {
                result.Add(new string[fields.ToArray().Length]);
                result.Last()[0] = c;
            }
        }

        private void DeleteCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (filesListBox != null) && (filesListBox.SelectedItem != null);
        }

        private void DeleteCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            files.Remove(filesListBox.SelectedItem as RawStockData);
            UpdateResultFieldsList();
        }

        private void HelpCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void HelpCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MessageBox.Show("Licencja użytkowania dla Wioletty Kilijańskiej :)" + Environment.NewLine + Environment.NewLine + "© Tomasz Truszkowski" + Environment.NewLine + "Wszystkie prawa zastrzeżone.", "Info");
        }

        private void CloseCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void CloseCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            App.Current.Shutdown();
        }
    }
}
