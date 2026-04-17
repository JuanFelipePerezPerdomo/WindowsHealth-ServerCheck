    namespace WindowsHealth_ServerCheck.Models
{
    public class OtherData
    {
        // Java
        public string JavaVersion { get; set; }
        public string OnlineJavaVersion { get; set; }
        public bool JavaUpToDate { get; set; }
        // Memoria RAM
        public string RAM { get; set; } = string.Empty;
        // BackUps
        public string BackUpState { get; set; }
        public string LastBackUp { get; set; }
        // AntiVirus
        public string AntiVirusName { get; set; }
        public string AntiVirusState{ get; set; }
        public string AntiVirusDir { get; set; }
        // Network Units
        public List<RedInfo> RedInterface { get; set; } = new();
        public List<UnitRedInfo> RedUnits { get; set; } = new();
    }

    public class UnitRedInfo
    {
        public string Letter { get; set; } = "";
        public string Path { get; set; } = "";
        public double TotalGB { get; set; } = 0;
        public double FreeGB { get; set; } = 0;
        public double FreePercentage { get; set; } = 0;
        public string VisualUsage { get; set; } = "";
    }

    public class RedInfo
    {
        public string RedName { get; set; }
        public string RedState { get; set; }
        public string Mbps { get; set; }
        public string RedType { get; set; } 

    }
}
