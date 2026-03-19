namespace WindowsHealth_ServerCheck.Models
{
    public class SmartResult
    {
        public DateTime Date { get; set; }
        public string? DiskName { get; set; }
        public string? InterfaceType { get; set; } // "SATA", "NVMe", "UBS", etc
        public int Temperature { get; set; }
        public int HealthPercent { get; set; }
        public int HoursUsed { get; set; }
        public bool PredictFailure { get; set; }
        public bool PredictFailureResolved { get; set; } // false → no hay fuente para determinarlo
        public bool HasHealthData { get; set; } // SSD: tiene atributo de vida útil
        public bool HasSmartData { get; set; } // false = disco detectado pero sin SMART (USB, etc.)
    }
}
