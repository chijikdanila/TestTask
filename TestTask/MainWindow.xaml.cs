using System.Windows;
using static TestTask.FilesWorker;
using static TestTask.ExcelFilesWorker;
using static TestTask.DatabaseWorker;
using Microsoft.VisualBasic;
using System.Windows.Controls;

namespace TestTask
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void showTask1_Click(object sender, RoutedEventArgs e)
        {
            Task2.Visibility = Visibility.Hidden;
            Task1.Visibility = Visibility.Visible;
        }

        private void showTask2_Click(object sender, RoutedEventArgs e)
        {
            Task1.Visibility = Visibility.Hidden;
            Task2.Visibility = Visibility.Visible;
        }

        private void chooseFolderPath_Click(object sender, RoutedEventArgs e)
        {
            GetFolderPath();

            showPathTB.Text = folderPath;
        }

        private async void createFiles_Click(object sender, RoutedEventArgs e)
        {
            await Task.Run(() => FilesCreator());
        }

        private async void mergeFiles_Click(object sender, RoutedEventArgs e)
        {
            await Task.Run(() => MergeFiles());
        }

        private async void deleteRows_Click(object sender, RoutedEventArgs e)
        {
            string temp = Interaction.InputBox("Input row criteria", "Criteria");

            if (!temp.Equals(string.Empty))
            {
                await Task.Run(() => DeleteRows(temp));
            }
            else
            {
                MessageBox.Show("Please input correct row delete criteria for a file");
            }
        }

        private async void import_Click(object sender, RoutedEventArgs e)
        {
            await Task.Run(() => Import());
        }

        private async void chooseFilePath_Click(object sender, RoutedEventArgs e)
        {
            await Task.Run(() => ImportExcelFile());
            filesListBox.Items.Add(fileHashCode);
        }

        private void filesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (filesListBox.SelectedItem != null)
                {
                    dataGrid.ItemsSource = GetBalanceSheets(filesListBox.SelectedItem.ToString());
                }
            });
        }
    }
}