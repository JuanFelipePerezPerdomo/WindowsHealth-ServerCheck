using WindowsHealth_ServerCheck.Models;
using WindowsHealth_ServerCheck.Modules;

namespace WindowsHealth_ServerCheck.Services
{
    public class AuditService
    {
        public static async Task<(AuditResult result, Dictionary<string, List<string>> logs)> RunAll(
            Action<string> onModuleStart)
        {
            AuditResult audit = new AuditResult { Date = DateTime.Now };
            Dictionary<string, List<string>> logs = new Dictionary<string, List<string>>();

            onModuleStart("Ejecutando limpieza de archivos temporales...");
            var (cleanLog, cleanResult) = await Task.Run(() => TempCleaner.Clean());
            audit.CleanUp = cleanResult;
            audit.CleanupExecuted = true;
            logs["cleanup"] = cleanLog;

            onModuleStart("Ejecutando protocolo S.M.A.R.T...");
            var (smartLog, smartResults) = await Task.Run(() => SmartProtocol.smart());
            audit.Disks = smartResults;
            audit.SmartExecuted = true;
            logs["smart"] = smartLog;

            onModuleStart("Escaneando drivers del sistema...");
            var (driverLog, driverResult) = await Task.Run(() => DriverScanner.Scan());
            audit.Drivers = driverResult;
            audit.DriversExecuted = true;
            logs["drivers"] = driverLog;

            return (audit, logs);
        }
        public static void FinalizeAudit(
            AuditResult audit,
            Dictionary<string, List<string>> logs,
            UpdateResult updateResult,
            List<string> updateLog)
        {
            if (updateResult != null)
            {
                audit.Updates = updateResult;
                audit.UpdatesExecuted = true;
                logs["updates"] = updateLog;
            }
        }
    }
}
