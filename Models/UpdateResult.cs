namespace WindowsHealth_ServerCheck.Models
{
    public class UpdateResult
    {
        public DateTime Date { get; set; }
        public int UpdatesFound { get; set; }
        public int UpdatesInstalled { get; set; }
        public List<string> UpdateTitles { get; set; } = new List<string>();
        public bool Success { get; set; }
    }
}
