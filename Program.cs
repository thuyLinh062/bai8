//Họ và Tên: Trần Thùy Linh
//MSSV: 202418935
using System;

namespace QuanLyThietBiPhongHoc
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding =
                System.Text.Encoding.UTF8;

            // ---------------------------------------------
            // BUOC 1: TAO CAC THIET BI
            // ---------------------------------------------

            ProjectorRemote projector = new ProjectorRemote(
                "PR01",
                "Dieu khien may chieu");

            AirConditionerRemote airConditioner =
                new AirConditionerRemote(
                    "AC01",
                    "Dieu khien dieu hoa",
                    "A101");

            WirelessMicrophone microphone =
                new WirelessMicrophone(
                    "MIC01",
                    "Micro khong day");

            List<TeachingDevice> devices = new()
            {
                projector,
                airConditioner,
                microphone
            };

            // ---------------------------------------------
            // BUOC 2: TAO CAC SUBSCRIBER
            // ---------------------------------------------

            ConsoleNotificationService notificationService =
                new ConsoleNotificationService();

            DeviceAuditLogger auditLogger =
                new DeviceAuditLogger();

            MaintenanceService maintenanceService =
                new MaintenanceService();

            // ---------------------------------------------
            // BUOC 3: TAO COORDINATOR VA DANG KY EVENT
            // ---------------------------------------------

            DeviceMonitoringCoordinator coordinator =
                new DeviceMonitoringCoordinator(
                    devices,
                    notificationService,
                    auditLogger,
                    maintenanceService);

            coordinator.Subscribe();

            // Goi Subscribe lan 2 de kiem tra khong dang ky trung
            coordinator.Subscribe();

            // ---------------------------------------------
            // BUOC 4: DANG KY DELEGATE LOC CANH BAO
            // ---------------------------------------------

            FilteredAlertSubscriber criticalOnly =
                new FilteredAlertSubscriber(
                    alert => alert.Severity == AlertSeverity.Critical,

                    alert => Console.WriteLine(
                        $"[LOC CRITICAL] {alert.DeviceId}: {alert.Message}")
                );

            foreach (TeachingDevice device in devices)
            {
                device.AlertRaised += criticalOnly.HandleAlert;
            }

            // ---------------------------------------------
            // BUOC 5: THAY DOI TRANG THAI THIET BI
            // ---------------------------------------------

            Console.WriteLine("\n========== THAY DOI TRANG THAI ==========");

            projector.ChangeStatus(DeviceStatus.Borrowed);

            projector.ChangeStatus(DeviceStatus.Borrowed);

            projector.ChangeStatus(DeviceStatus.UnderMaintenance);

            // ---------------------------------------------
            // BUOC 6: KIEM TRA PIN DIEU KHIEN MAY CHIEU
            // ---------------------------------------------

            Console.WriteLine("\n========== KIEM TRA MAY CHIEU ==========");

            projector.UpdateBatteryLevel(18);

            projector.UpdateBatteryLevel(15);

            projector.UpdateBatteryLevel(0);

            // ---------------------------------------------
            // BUOC 7: KIEM TRA DIEU KHIEN DIEU HOA
            // ---------------------------------------------

            Console.WriteLine("\n========== KIEM TRA DIEU HOA ==========");

            airConditioner.UpdateResponseState(false);

            airConditioner.UpdateResponseState(false);

            airConditioner.UpdateResponseState(true);

            airConditioner.UpdateResponseState(false);

            // ---------------------------------------------
            // BUOC 8: KIEM TRA MICRO
            // ---------------------------------------------

            Console.WriteLine("\n========== KIEM TRA MICRO ==========");

            microphone.UpdateBatteryLevel(25);

            microphone.UpdateBatteryLevel(20);

            microphone.UpdateBatteryLevel(0);

            microphone.UpdateSignalQuality(35);

            microphone.UpdateSignalQuality(0);

            // ---------------------------------------------
            // BUOC 9: KIEM TRA NGOAI LE
            // ---------------------------------------------

            Console.WriteLine("\n========== KIEM TRA NGOAI LE ==========");

            try
            {
                projector.UpdateBatteryLevel(120);
            }
            catch (InvalidDeviceReadingException ex)
            {
                Console.WriteLine(ex.Message);

                Console.WriteLine(
                    $"Ma thiet bi: {ex.DeviceId}");

                Console.WriteLine(
                    $"Gia tri khong hop le: {ex.InvalidValue}");
            }

            // ---------------------------------------------
            // BUOC 10: HIEN THI LICH SU THAY DOI
            // ---------------------------------------------

            Console.WriteLine("\n========== LICH SU THAY DOI ==========");

            foreach (string entry in auditLogger.Entries)
            {
                Console.WriteLine(entry);
            }

            // ---------------------------------------------
            // BUOC 11: HIEN THI YEU CAU BAO TRI
            // ---------------------------------------------

            Console.WriteLine("\n========== YEU CAU BAO TRI ==========");

            foreach (string request in maintenanceService.Requests)
            {
                Console.WriteLine(request);
            }

            // ---------------------------------------------
            // BUOC 12: HUY DANG KY
            // ---------------------------------------------

            foreach (TeachingDevice device in devices)
            {
                device.AlertRaised -= criticalOnly.HandleAlert;
            }

            coordinator.Dispose();

            Console.WriteLine("\nDa huy dang ky cac Event.");

            Console.ReadKey();
        }
    }
}