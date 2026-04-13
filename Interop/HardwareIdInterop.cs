using System.Runtime.InteropServices;

namespace WindowsHealth_ServerCheck.Interop
{
    /// <summary>
    /// Espejo exacto de HardwareIdEntry en C.
    /// hardwareId[512] + compatibleId[512] = 1024 wchar_t = 2048 bytes.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct HardwareIdEntry
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 512)]
        public string HardwareId;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 512)]
        public string CompatibleId;
    }

    /// <summary>
    /// Espejo exacto de DeviceRecord en C.
    /// El layout debe coincidir byte a byte con la struct C.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct DeviceRecord
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 512)]
        public string InstanceId;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string DeviceDescription;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string Manufacturer;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string DriverVersion;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string DriverDate;          // formato "YYYY-MM-DD" desde C

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)] // MAX_PATH = 260
        public string InfPath;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string ClassGuid;

        public int HardwareIdCount;

        // Array inline de 8 entradas — debe declararse con MarshalAs ByValArray
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public HardwareIdEntry[] HardwareIds;

        public int HasDriver;
        public int IsProblem;
        public uint ProblemCode;
    }

    /// <summary>
    /// P/Invoke para las tres funciones exportadas por HardwareIdScanner.dll.
    /// Todas usan __cdecl, por eso CallingConvention.Cdecl.
    /// </summary>
    public static class HardwareIdNative
    {
        private const string DllName = "HardwareIdScanner.dll";

        /// <summary>
        /// Enumera todos los dispositivos presentes.
        /// Devuelve un puntero opaco a DeviceList (no lo marshaleamos directamente).
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr EnumerateAllDevices();

        /// <summary>
        /// Libera la memoria del DeviceList. SIEMPRE llamar al terminar.
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void FreeDeviceList(IntPtr list);

        /// <summary>
        /// Número de dispositivos en la lista.
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetDeviceCount(IntPtr list);

        /// <summary>
        /// Puntero al DeviceRecord en la posición index.
        /// Marshaleamos manualmente con PtrToStructure para evitar problemas
        /// con el array inline de HardwareIdEntry.
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr GetDevice(IntPtr list, int index);
    }

    /// <summary>
    /// Capa de alto nivel: enumera los dispositivos y devuelve una lista
    /// de DeviceRecord ya marshaleados, liberando la DLL correctamente.
    /// </summary>
    public static class HardwareIdScanner
    {
        public static List<DeviceRecord> GetAllDevices()
        {

            string dllPath = Path.Combine(AppContext.BaseDirectory, "HardwareIdScanner.dll");
            System.Diagnostics.Debug.WriteLine($"[DLL] Buscando en: {dllPath}");
            System.Diagnostics.Debug.WriteLine($"[DLL] Existe: {File.Exists(dllPath)}");

            var results = new List<DeviceRecord>();

            IntPtr list = HardwareIdNative.EnumerateAllDevices();
            if (list == IntPtr.Zero)
                return results;

            try
            {
                int count = HardwareIdNative.GetDeviceCount(list);

                for (int i = 0; i < count; i++)
                {
                    IntPtr recordPtr = HardwareIdNative.GetDevice(list, i);
                    if (recordPtr == IntPtr.Zero) continue;

                    // Marshal manual: convierte la memoria nativa a struct C#
                    DeviceRecord record = Marshal.PtrToStructure<DeviceRecord>(recordPtr);
                    results.Add(record);
                }
            }
            finally
            {
                // Garantiza que siempre se libere aunque haya excepción
                HardwareIdNative.FreeDeviceList(list);
            }

            return results;
        }

        /// <summary>
        /// Devuelve un diccionario: HardwareId → DeviceRecord.
        /// Útil para cruzar con Windows Update por ID exacto.
        /// </summary>
        public static Dictionary<string, DeviceRecord> GetDevicesByHardwareId()
        {
            var map = new Dictionary<string, DeviceRecord>(StringComparer.OrdinalIgnoreCase);

            foreach (DeviceRecord rec in GetAllDevices())
            {
                if (rec.HardwareIds == null) continue;

                for (int i = 0; i < rec.HardwareIdCount && i < rec.HardwareIds.Length; i++)
                {
                    string hwid = rec.HardwareIds[i].HardwareId;
                    if (!string.IsNullOrWhiteSpace(hwid) && !map.ContainsKey(hwid))
                        map[hwid] = rec;
                }
            }

            return map;
        }
    }
}
