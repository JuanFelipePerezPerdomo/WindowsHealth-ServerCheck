namespace WindowsHealth_ServerCheck.Helpers
{
    public static class HistoryLogger
    {
        public static readonly string HistoryPath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory, "Logs", "Healt_History.txt"
        );
        public static void Save(string header, List<string> log)
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(HistoryPath));

                using (StreamWriter sw = new StreamWriter(HistoryPath, append: true))
                {
                    sw.Write(header);
                    foreach (string line in log)
                    {
                        sw.WriteLine(line);
                    }
                    sw.WriteLine();
                }
            }
            catch (Exception ex)
            {
                {
                    MessageBox.Show("Error al guardar histórico: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
