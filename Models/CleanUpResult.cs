namespace WindowsHealth_ServerCheck.Models
{
    public class CleanUpResult
    {
        public DateTime Date { get; set; }
        public int DeleteFiles { get; set; }
        public int SkippedFiles { get; set; }
        public int DeleteDirs { get; set; }
        public int SkippedDirs { get; set; }
        public long FreedBytes { get; set; }
        public List<string> DeletedFilesNames { get; set; } = new List<string>();
    }
}
