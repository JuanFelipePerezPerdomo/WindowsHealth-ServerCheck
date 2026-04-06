namespace WindowsHealth_ServerCheck.Models
{
    public class AuditResult
    {
        public string TechnicianName { get; set; } = string.Empty;
        public bool IsDfServerTechnician { get; set; }

        public DateTime Date { get; set; }
        public CleanUpResult CleanUp { get; set; }
        public List<SmartResult> Disks { get; set; } = new List<SmartResult>();
        public UpdateResult Updates { get; set; }
        public DriversResult Drivers { get; set; }
        public DfServerData DfServer { get; set; }
        public bool CleanupExecuted { get; set; }
        public bool SmartExecuted { get; set; }
        public bool UpdatesExecuted { get; set; }
        public bool DriversExecuted { get; set; }
        public bool DfServerExecuted { get; set; }
    }
}
