//Họ và Tên: Trần Thùy Linh
//MSSV: 202418935
using System;

namespace QuanLyThietBiPhongHoc
{
    //  DEVICE MONITORING COORDINATOR
    // =====================================================

    public class DeviceMonitoringCoordinator : IDisposable
    {
        private readonly List<TeachingDevice> devices;

        private readonly ConsoleNotificationService notificationService;

        private readonly DeviceAuditLogger auditLogger;

        private readonly MaintenanceService maintenanceService;

        private bool isSubscribed = false;

        public DeviceMonitoringCoordinator(
            IEnumerable<TeachingDevice> devices,
            ConsoleNotificationService notificationService,
            DeviceAuditLogger auditLogger,
            MaintenanceService maintenanceService)
        {
            this.devices = devices.ToList();

            this.notificationService = notificationService;

            this.auditLogger = auditLogger;

            this.maintenanceService = maintenanceService;
        }

        // Dang ky cac Event
        public void Subscribe()
        {
            // Tranh dang ky trung
            if (isSubscribed)
            {
                return;
            }

            foreach (TeachingDevice device in devices)
            {
                device.AlertRaised += notificationService.HandleAlert;

                device.AlertRaised += maintenanceService.HandleCriticalAlert;

                device.StatusChanged += auditLogger.HandleStatusChanged;
            }

            isSubscribed = true;
        }

        // Huy dang ky cac Event
        public void Unsubscribe()
        {
            if (!isSubscribed)
            {
                return;
            }

            foreach (TeachingDevice device in devices)
            {
                device.AlertRaised -= notificationService.HandleAlert;

                device.AlertRaised -= maintenanceService.HandleCriticalAlert;

                device.StatusChanged -= auditLogger.HandleStatusChanged;
            }

            isSubscribed = false;
        }

        // Giai phong dang ky
        public void Dispose()
        {
            Unsubscribe();
        }
    }
}