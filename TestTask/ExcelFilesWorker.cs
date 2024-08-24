using Microsoft.Win32;
using static TestTask.DatabaseWorker;
using System.IO;
using System.Security.Cryptography;

namespace TestTask
{
    static class ExcelFilesWorker
    {
        private static string filePath;
        public static string fileHashCode;

        private static void GetFileInfo()
        {
            var fileDialog = new OpenFileDialog
            {
                Multiselect = false,
                AddToRecent = true
            };

            if (fileDialog.ShowDialog() == true)
            {
                filePath = fileDialog.FileName;
                fileHashCode = GetFileHash(filePath);
            }
            else
            {
                throw new Exception();
            }
        }

        public static string GetFileHash(string filePath)
        {
            using (var sha256 = SHA256.Create())
            {
                using (var stream = File.OpenRead(filePath))
                {
                    var hash = sha256.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
        }

        public static Task ImportExcelFile()
        {
            GetFileInfo();

            InsertExcelFileData(Guid.NewGuid().ToString(), filePath, fileHashCode);

            return Task.CompletedTask;
        }
    }
}
