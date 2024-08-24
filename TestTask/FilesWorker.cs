using Microsoft.Win32;
using System.Windows;
using System.IO;
using TestTask.Entities;
using System.Data;

namespace TestTask
{
    internal class FilesWorker
    {
        private static Random random = new Random();
        public static string? folderPath;

        public static void GetFolderPath()
        {
            var folderDialog = new OpenFolderDialog
            {
                Multiselect = false,
                AddToRecent = true
            };

            if (folderDialog.ShowDialog() == true)
            {
                folderPath = folderDialog.FolderName;
            }
            else
            {
                throw new Exception();
            }
        }

        public static List<string> Create()
        {
            List<string> strings = new List<string>();
            string latinChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string russianChars = "абвгдеёжзийклмнопрстуфхцчшщъыьэюяАБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ";

            for (int i = 0; i < 100_000; i++)
            {
                //Random date
                DateTime startDate = new DateTime(2019, 1, 1);
                int range = (DateTime.Today - startDate).Days;
                string randomDate = startDate.AddDays(random.Next(range)).ToString("dd.MM.yyyy");

                //Random 10 latin chars
                string randomLatinChars = new string(Enumerable.Repeat(latinChars, 10).Select(s => s[random.Next(s.Length)]).ToArray());

                //Random 10 russian chars
                string randomRussianChars = new string(Enumerable.Repeat(russianChars, 10).Select(s => s[random.Next(s.Length)]).ToArray());

                //Random number
                int randomNumber = random.Next(1, 100_000_000);

                //Random float
                float randomFloat = (float)Math.Round(random.NextDouble() * 19 + 1, 8);

                strings.Add(new RandomString(randomDate, randomLatinChars, randomRussianChars, randomNumber, randomFloat).ToString());
            }

            return strings;
        }

        public static async void FilesCreator()
        {
            for (int i = 1; i <= 100; i++)
            {
                await File.WriteAllLinesAsync(@$"{folderPath}\{i.ToString("D3")}.txt", Create());

                UpdateProgressBar(i);
            }

            MessageBox.Show("Files created");
        }

        public static async void MergeFiles()
        {
            for (int i = 1; i <= 100; i++)
            {
                await File.AppendAllLinesAsync(@$"{folderPath}\all.txt", File.ReadAllLines(@$"{folderPath}\{i.ToString("D3")}.txt"));

                UpdateProgressBar(i);
            }

            MessageBox.Show("Files merged");
        }

        public static async void DeleteRows(string str)
        {
            string filePath = @$"{folderPath}\all.txt";
            string[] lines = File.ReadAllLines(filePath);
            int count = lines.Length;
            int deletedCount = 0;

            var remainingLines = lines.Where((line, index) =>
            {
                bool keepLine = !line.Contains(str);

                if (!keepLine)
                {
                    deletedCount++;
                }

                UpdateProgressBar(index * 100.0 / count);

                return keepLine;
            }).ToArray();

            await File.WriteAllLinesAsync(filePath, remainingLines);
            MessageBox.Show($"Deleted rows: {deletedCount}");
        }

        public static async Task Import()
        {
            string filePath = @$"{folderPath}\all.txt";
            string[] lines = File.ReadAllLines(filePath);

            await DatabaseWorker.ImportMergedFile(lines);
            MessageBox.Show("File imported");
        }

        public static void UpdateProgressBar(double progress)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                ((MainWindow)Application.Current.MainWindow).progressBar.Value = progress;
                ((MainWindow)Application.Current.MainWindow).showProcentTB.Text = $"{progress.ToString("F2")}%";
            });
        }
    }
}
