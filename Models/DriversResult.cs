namespace WindowsHealth_ServerCheck.Models
{
    public class DriversResult
    {
        public DateTime Date { get; set; }
        public int TotalDrivers { get; set; }
        public int OutdatedDrivers { get; set; }
        public List<DriverInfo> Drivers { get; set; } = new List<DriverInfo>();
    }
    public class DriverInfo
    {
        public string DeviceName { get; set; }
        public string DriverVersion { get; set; }
        public string DriverDate { get; set; }
        public string Manufacturer { get; set; }
        public bool IsOutdated { get; set; }
        public string UpdatedTitle { get; set; }
    }
}
