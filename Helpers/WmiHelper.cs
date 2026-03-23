using System.Management;

namespace WindowsHealth_ServerCheck.Helpers
{
    public class WmiHelper
    {
        public static ManagementObjectCollection Query(string scope, string query)
        {
            ManagementScope ms = new ManagementScope(scope);
            ObjectQuery oq = new ObjectQuery(query);
            return new ManagementObjectSearcher(ms, oq).Get();
        }
        public static string EscapeWmiString(string value)
        {
            return value.Replace("\\", "\\\\");
        }
        public static string ParseWmiDate(string wmiDate)
        {
            if (string.IsNullOrWhiteSpace(wmiDate) || wmiDate == "-")
                return "-";
            try
            {
                // ManagementDateTimeConverter es la forma canónica de parsear fechas WMI
                DateTime dt = ManagementDateTimeConverter.ToDateTime(wmiDate);
                return dt.ToString("dd/MM/yyyy");
            }
            catch
            {
                // Fallback: los primeros 8 caracteres son YYYYMMDD
                if (wmiDate.Length >= 8)
                {
                    string y = wmiDate.Substring(0, 4);
                    string m = wmiDate.Substring(4, 2);
                    string d = wmiDate.Substring(6, 2);
                    return $"{d}/{m}/{y}";
                }
                return wmiDate;
            }
        }
    }
}
