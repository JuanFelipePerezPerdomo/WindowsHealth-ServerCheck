using System.Management;

namespace WindowsHealth_ServerCheck.Diagnostic
{
    /// <summary>
    /// Herramienta temporal de diagnóstico.
    /// Llama a SmartDiagnostic.Run() desde un botón temporal en mainForm
    /// y vuelca el resultado en txt_healtInformation.
    /// ELIMINAR una vez confirmados los InstanceNames correctos.
    /// </summary>
    public static class SmartDiagnostic
    {
        public static List<string> Run()
        {
            var log = new List<string>();

            log.Add("══════════════════════════════════════════");
            log.Add("DIAGNÓSTICO WMI — FailurePredictStatus");
            log.Add("══════════════════════════════════════════");
            try
            {
                var col = new System.Management.ManagementObjectSearcher(
                    new ManagementScope(@"root\wmi"),
                    new ObjectQuery("SELECT * FROM MSStorageDriver_FailurePredictStatus")).Get();

                int count = 0;
                foreach (ManagementObject obj in col)
                {
                    count++;
                    string instance = obj["InstanceName"]?.ToString() ?? "(null)";
                    bool predict = false;
                    try { predict = (bool)obj["PredictFailure"]; } catch { }
                    log.Add($"[{count}] InstanceName : {instance}");
                    log.Add($"     PredictFailure: {predict}");
                    log.Add("");
                }
                if (count == 0)
                    log.Add("(sin resultados — WMI no devolvió ningún disco)");
            }
            catch (Exception ex)
            {
                log.Add("ERROR WMI FailurePredictStatus: " + ex.Message);
            }

            log.Add("══════════════════════════════════════════");
            log.Add("DIAGNÓSTICO LHM — Todos los sensores");
            log.Add("══════════════════════════════════════════");
            try
            {
                var computer = new LibreHardwareMonitor.Hardware.Computer
                { IsStorageEnabled = true };
                computer.Open();

                foreach (var hw in computer.Hardware
                    .Where(h => h.HardwareType == LibreHardwareMonitor.Hardware.HardwareType.Storage))
                {
                    hw.Update();
                    log.Add($"[ Disco ] {hw.Name}");
                    foreach (var sensor in hw.Sensors)
                    {
                        string val = sensor.Value.HasValue
                            ? sensor.Value.Value.ToString("F2")
                            : "(sin valor)";
                        log.Add($"  SensorType={sensor.SensorType,-14} Name=\"{sensor.Name}\"  Value={val}");
                    }
                    log.Add("");
                }
                computer.Close();
            }
            catch (Exception ex)
            {
                log.Add("ERROR LHM: " + ex.Message);
            }

            return log;
        }
    }
}