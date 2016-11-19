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

namespace StockDataFilter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        FileRepository fileRepo = new FileRepository();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void OpenCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void CollectFilenames()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            if (openFileDialog.ShowDialog() == true)
            {
                foreach (string filename in openFileDialog.FileNames)
                {
                    fileRepo.AddFilename(filename);
                    textBlock.Text += "Opened: " + filename + "\n";
                }
            }
        }

        private void OpenCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            CollectFilenames();
            var data = fileRepo.GetData();
            foreach(var d in data) {
                textBlock.Text += d.Filename + "\n";
            }
            var firstData = data.First();
            for (int i = 0; i < firstData.Fields.Length; ++i)
            {
                var col = new DataGridTextColumn();
                col.Header = firstData.Fields[i];
                col.Binding = new Binding(string.Format("[{0}]", i));
                originalDataGrid.Columns.Add(col);
            }
            originalDataGrid.ItemsSource = firstData.Entries;
        }
    }
}
