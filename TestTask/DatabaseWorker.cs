using static TestTask.FilesWorker;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using System.Windows;
using Aspose.Cells;
using TestTask.Entities;

namespace TestTask
{
    static class DatabaseWorker
    {
        private readonly static string connectionString = ConfigurationManager.ConnectionStrings["TaskConnection"].ConnectionString;

        public static async Task ImportMergedFile(string[] lines)
        {
            try
            {
                int batchSize = 1000;
                DataTable dataTable = new DataTable();
                dataTable.Columns.Add("date", typeof(string));
                dataTable.Columns.Add("latin_string", typeof(string));
                dataTable.Columns.Add("russian_string", typeof(string));
                dataTable.Columns.Add("number", typeof(int));
                dataTable.Columns.Add("float", typeof(float));

                for (int i = 0; i < lines.Length; i += batchSize)
                {
                    dataTable.Clear();

                    for (int j = i; j < i + batchSize && j < lines.Length; j++)
                    {
                        dataTable.Rows.Add(lines[j].Split("||", StringSplitOptions.RemoveEmptyEntries));
                    }

                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                        {
                            bulkCopy.DestinationTableName = "random_strings";
                            await Task.Run(() => bulkCopy.WriteToServer(dataTable));
                        }
                        connection.Close();
                    }
                    UpdateProgressBar((i + batchSize) / (double)lines.Length * 100);
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private static void InsertExcelFileInfo(string id, string fileName, string fileHash)
        {
            try
            {
                string query = "INSERT INTO imported_file (id, file_name, file_hash) VALUES (@id, @file_name, @file_hash)";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@id", id);
                    command.Parameters.AddWithValue("@file_name", fileName);
                    command.Parameters.AddWithValue("@file_hash", fileHash);

                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private static DataTable ReadExcelFile(string filePath)
        {
            Workbook workbook = new Workbook(filePath);
            Worksheet worksheet = workbook.Worksheets[0];
            Cells cells = worksheet.Cells;

            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("sheet_number", typeof(int));
            dataTable.Columns.Add("incoming_active", typeof(double));
            dataTable.Columns.Add("incoming_passive", typeof(double));
            dataTable.Columns.Add("circuit_debet", typeof(double));
            dataTable.Columns.Add("circuit_credit", typeof(double));
            dataTable.Columns.Add("outcoming_active", typeof(double));
            dataTable.Columns.Add("outcoming_passive", typeof(double));

            for (int i = 1; i < cells.MaxDataRow + 1; i++)
            {
                if (int.TryParse(cells[i, 0].StringValue, out _))
                {
                    DataRow row = dataTable.NewRow();
                    row["sheet_number"] = Convert.ToInt32(cells[i, 0].StringValue);
                    row["incoming_active"] = cells[i, 1].DoubleValue;
                    row["incoming_passive"] = cells[i, 2].DoubleValue;
                    row["circuit_debet"] = cells[i, 3].DoubleValue;
                    row["circuit_credit"] = cells[i, 4].DoubleValue;
                    row["outcoming_active"] = cells[i, 5].DoubleValue;
                    row["outcoming_passive"] = cells[i, 6].DoubleValue;

                    dataTable.Rows.Add(row);
                }
            }

            return dataTable;
        }

        public static void InsertExcelFileData(string id, string filePath, string fileHash)
        {
            InsertExcelFileInfo(id, filePath, fileHash);
            DataTable dataTable = ReadExcelFile(filePath);

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try 
                { 
                    connection.Open();
                    foreach (DataRow row in dataTable.Rows)
                    {
                        string query = "INSERT INTO balance_sheet (sheet_number, incoming_active, incoming_passive, circuit_debet, circuit_credit, outcoming_active, outcoming_passive, file_id) " +
                                       "VALUES (@sheet_number, @incoming_active, @incoming_passive, @circuit_debet, @circuit_credit, @outcoming_active, @outcoming_passive, @file_id)";

                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@sheet_number", row["sheet_number"]);
                            command.Parameters.AddWithValue("@incoming_active", row["incoming_active"]);
                            command.Parameters.AddWithValue("@incoming_passive", row["incoming_passive"]);
                            command.Parameters.AddWithValue("@circuit_debet", row["circuit_debet"]);
                            command.Parameters.AddWithValue("@circuit_credit", row["circuit_credit"]);
                            command.Parameters.AddWithValue("@outcoming_active", row["outcoming_active"]);
                            command.Parameters.AddWithValue("@outcoming_passive", row["outcoming_passive"]);
                            command.Parameters.AddWithValue("@file_id", id);

                            command.ExecuteNonQuery();
                        }
                    }
                }
                catch (SqlException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }
                }
            }
        }

        public static List<BalanceSheet> GetBalanceSheets(string fileHash)
        {
            List<BalanceSheet> balanceSheets = new List<BalanceSheet>();

            string query = "SELECT sheet_number, incoming_active, incoming_passive, circuit_debet, circuit_credit, outcoming_active, outcoming_passive, file_id FROM balance_sheet bs " +
                           "JOIN imported_file imf ON bs.file_id = imf.id WHERE file_hash = @file_hash";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@file_hash", fileHash);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    BalanceSheet sheet = new BalanceSheet
                    {
                        SheetNumber = reader.GetInt32(0),
                        IncomingActive = reader.GetDouble(1),
                        IncomingPassive = reader.GetDouble(2),
                        CircuitDebet = reader.GetDouble(3),
                        CircuitCredit = reader.GetDouble(4),
                        OutcomingActive = reader.GetDouble(5),
                        OutcomingPassive = reader.GetDouble(6)
                    };
                    balanceSheets.Add(sheet);
                }
            }

            return balanceSheets;
        }
    }
}
