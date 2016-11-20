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

        private void FillOriginalDataGrid(RawStockData data)
        {
            originalDataGrid.Columns.Clear();
            originalDataGrid.ItemsSource = null;

            for (int i = 0; i < data.Fields.Length; ++i)
            {
                var col = new DataGridTextColumn();
                col.Header = data.Fields[i];
                col.Binding = new Binding(string.Format("[{0}]", i));
                originalDataGrid.Columns.Add(col);
            }
            originalDataGrid.ItemsSource = data.Entries;
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
            
            var commonFields = filesFields.First().ToArray();
            foreach (var fields in filesFields.Skip(1))
            {
                commonFields = Utils.LongestCommonSubsequence(commonFields, fields);
            }
            // TODO: Examine the existing resultFields collection
            foreach (var field in commonFields)
            {
                var resultField = new ResultField() { Name = field, Accepted = true };
                resultFields.Add(resultField);

                if(resultField.Name == settings.DefaultFirstField)
                {
                    firstFieldComboBox.SelectedItem = resultField;
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
            var data = fileRepo.ReadDataFromFiles(filenames);

            FillOriginalListBoxItemsSource(data);
            FillOriginalDataGrid(data.First());
            UpdateResultFieldsList();
        }

        private void SaveCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void SaveCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {

        }

        private void generateResultButton_Click(object sender, RoutedEventArgs e)
        {
            resultDataGrid.Columns.Clear();
            resultDataGrid.ItemsSource = null;
            List<string> fields = new List<string>();

            //foreach (ResultField field in resultFields)
            //{
            //    if(field.Accepted)
            //    {
            //        foreach (var file in files)
            //        {
            //            fields.Add(field.Name);
            //            var col = new DataGridTextColumn();
            //            col.Header = file.Filename + "_" + field.Name;
            //            resultDataGrid.Columns.Add(col);
            //        }
            //    }
            //}

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
            for (int i = 0; )
        }
    }
}
