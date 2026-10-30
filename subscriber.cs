//Họ và Tên: Trần Thùy Linh
//MSSV: 202418935
using System;

namespace QuanLyThietBiPhongHoc
{
    // SUBSCRIBER: CONSOLE NOTIFICATION SERVICE
    
    public class ConsoleNotificationService
    {
        public void HandleAlert(
            object? sender,
            DeviceAlertEventArgs e)
        {
            Console.WriteLine(
                $"[{e.Severity}] {e.DeviceId}: {e.Message}");
        }
    }

    // SUBSCRIBER: DEVICE AUDIT LOGGER
    
    public class DeviceAuditLogger
    {
        private readonly List<string> entries = new();

        public IReadOnlyList<string> Entries =>
            entries.AsReadOnly();

        public void HandleStatusChanged(
            object? sender,
            DeviceStatusChangedEventArgs e)
        {
            entries.Add(
                $"{e.ChangedAt}: {e.DeviceId}, " +
                $"{e.OldStatus} -> {e.NewStatus}");
        }
    }

    // SUBSCRIBER: MAINTENANCE SERVICE

    public class MaintenanceService
    {
        private readonly List<string> requests = new();

        public IReadOnlyList<string> Requests =>
            requests.AsReadOnly();

        public void HandleCriticalAlert(
            object? sender,
            DeviceAlertEventArgs e)
        {
            // Chi tiep nhan canh bao Critical
            if (e.Severity != AlertSeverity.Critical)
            {
                return;
            }

            requests.Add(
                $"Maintenance request for {e.DeviceId}: {e.Message}");
        }
    }
}