//Họ và Tên: Trần Thùy Linh
//MSSV: 202418935
using System;

namespace QuanLyThietBiPhongHoc
{
    class Program
    {
        // =================================================
        // BO DEM KET QUA KIEM THU
        // =================================================

        private static int passed = 0;
        private static int failed = 0;

        // Kiem tra mot dieu kien, in PASS/FAIL
        private static void Check(string testName, bool condition)
        {
            if (condition)
            {
                passed++;
                Console.WriteLine($"  [PASS] {testName}");
            }
            else
            {
                failed++;
                Console.WriteLine($"  [FAIL] {testName}");
            }
        }

        // So sanh gia tri thuc te voi gia tri mong doi
        private static void CheckEqual<T>(
            string testName, T expected, T actual)
        {
            bool ok = EqualityComparer<T>.Default.Equals(expected, actual);

            if (ok)
            {
                passed++;
                Console.WriteLine($"  [PASS] {testName}");
            }
            else
            {
                failed++;
                Console.WriteLine(
                    $"  [FAIL] {testName} (mong doi: {expected}, thuc te: {actual})");
            }
        }

        // Kiem tra mot hanh dong co nem InvalidDeviceReadingException khong
        private static InvalidDeviceReadingException? ExpectInvalidReading(
            Action action)
        {
            try
            {
                action();
            }
            catch (InvalidDeviceReadingException ex)
            {
                return ex;
            }

            return null;
        }

        private static void Section(string title)
        {
            Console.WriteLine($"\n========== {title} ==========");
        }

        // Bo ghi lai cac Event cua mot thiet bi de doi chieu ket qua
        private class Recorder
        {
            public List<DeviceAlertEventArgs> Alerts { get; } = new();
            public List<DeviceStatusChangedEventArgs> Changes { get; } = new();

            public Recorder(TeachingDevice device)
            {
                device.AlertRaised += (s, e) => Alerts.Add(e);
                device.StatusChanged += (s, e) => Changes.Add(e);
            }
        }

        // =================================================
        // MAIN
        // =================================================

        static int Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            TestStatusChange();
            TestProjectorBattery();
            TestAirConditioner();
            TestMicrophone();
            TestInvalidReadings();
            TestCoordinatorSubscribe();
            TestFilteredSubscriber();
            TestUnsubscribeAndDispose();

            Console.WriteLine("\n==========================================");
            Console.WriteLine($"TONG KET: {passed} dat, {failed} that bai.");
            Console.WriteLine("==========================================");

            // Tra ve ma loi khac 0 neu co test that bai
            return failed == 0 ? 0 : 1;
        }

        // =================================================
        // 1. THAY DOI TRANG THAI
        // =================================================

        static void TestStatusChange()
        {
            Section("1. THAY DOI TRANG THAI");

            ProjectorRemote projector =
                new ProjectorRemote("PR01", "Dieu khien may chieu");
            Recorder rec = new Recorder(projector);

            CheckEqual("Trang thai ban dau la Available",
                DeviceStatus.Available, projector.Status);

            projector.ChangeStatus(DeviceStatus.Borrowed);
            CheckEqual("Doi sang Borrowed: trang thai duoc cap nhat",
                DeviceStatus.Borrowed, projector.Status);
            CheckEqual("Doi sang Borrowed: phat 1 Event",
                1, rec.Changes.Count);
            CheckEqual("Event ghi dung OldStatus",
                DeviceStatus.Available, rec.Changes[0].OldStatus);
            CheckEqual("Event ghi dung NewStatus",
                DeviceStatus.Borrowed, rec.Changes[0].NewStatus);
            CheckEqual("Event ghi dung DeviceId",
                "PR01", rec.Changes[0].DeviceId);

            projector.ChangeStatus(DeviceStatus.Borrowed);
            CheckEqual("Doi sang cung trang thai: khong phat them Event",
                1, rec.Changes.Count);

            projector.ChangeStatus(DeviceStatus.UnderMaintenance);
            CheckEqual("Borrowed -> UnderMaintenance: phat Event thu 2",
                2, rec.Changes.Count);
            CheckEqual("Event thu 2 co OldStatus = Borrowed",
                DeviceStatus.Borrowed, rec.Changes[1].OldStatus);
            CheckEqual("Event thu 2 co NewStatus = UnderMaintenance",
                DeviceStatus.UnderMaintenance, rec.Changes[1].NewStatus);
        }

        // =================================================
        // 2. PIN DIEU KHIEN MAY CHIEU (Warning < 20, Critical = 0)
        // =================================================

        static void TestProjectorBattery()
        {
            Section("2. PIN DIEU KHIEN MAY CHIEU");

            ProjectorRemote projector =
                new ProjectorRemote("PR01", "Dieu khien may chieu");
            Recorder rec = new Recorder(projector);

            CheckEqual("Pin ban dau la 100", 100, projector.BatteryLevel);

            projector.UpdateBatteryLevel(50);
            CheckEqual("Pin 50: khong canh bao", 0, rec.Alerts.Count);

            projector.UpdateBatteryLevel(20);
            CheckEqual("Pin 20 (bien): khong canh bao", 0, rec.Alerts.Count);

            projector.UpdateBatteryLevel(19);
            CheckEqual("Pin 19 (bien): co 1 canh bao", 1, rec.Alerts.Count);
            CheckEqual("Pin 19: muc Warning",
                AlertSeverity.Warning, rec.Alerts[0].Severity);

            projector.UpdateBatteryLevel(15);
            CheckEqual("Pin 15 (van Warning): khong canh bao lap",
                1, rec.Alerts.Count);

            projector.UpdateBatteryLevel(0);
            CheckEqual("Pin 0: co canh bao thu 2", 2, rec.Alerts.Count);
            CheckEqual("Pin 0: muc Critical",
                AlertSeverity.Critical, rec.Alerts[1].Severity);

            projector.UpdateBatteryLevel(0);
            CheckEqual("Pin van 0: khong canh bao lap",
                2, rec.Alerts.Count);

            projector.UpdateBatteryLevel(80);
            CheckEqual("Sac lai len 80: khong canh bao",
                2, rec.Alerts.Count);

            projector.UpdateBatteryLevel(10);
            CheckEqual("Pin tut xuong 10 lan nua: canh bao lai",
                3, rec.Alerts.Count);
            CheckEqual("Canh bao lai la Warning",
                AlertSeverity.Warning, rec.Alerts[2].Severity);
        }

        // =================================================
        // 3. DIEU KHIEN DIEU HOA
        // =================================================

        static void TestAirConditioner()
        {
            Section("3. DIEU KHIEN DIEU HOA");

            AirConditionerRemote ac =
                new AirConditionerRemote("AC01", "Dieu khien dieu hoa", "A101");
            Recorder rec = new Recorder(ac);

            Check("Ban dau dang phan hoi", ac.IsResponsive);

            ac.UpdateResponseState(true);
            CheckEqual("Dat true khi dang true: khong canh bao",
                0, rec.Alerts.Count);

            ac.UpdateResponseState(false);
            CheckEqual("Mat phan hoi: co 1 canh bao", 1, rec.Alerts.Count);
            CheckEqual("Canh bao la Critical",
                AlertSeverity.Critical, rec.Alerts[0].Severity);
            Check("Canh bao co ten phong A101",
                rec.Alerts[0].Message.Contains("A101"));
            Check("IsResponsive = false", !ac.IsResponsive);

            ac.UpdateResponseState(false);
            CheckEqual("Mat phan hoi lien tiep: khong canh bao lap",
                1, rec.Alerts.Count);

            ac.UpdateResponseState(true);
            CheckEqual("Phan hoi lai: khong canh bao", 1, rec.Alerts.Count);
            Check("IsResponsive = true", ac.IsResponsive);

            ac.UpdateResponseState(false);
            CheckEqual("Mat phan hoi lan 2: canh bao lai",
                2, rec.Alerts.Count);
        }

        // =================================================
        // 4. MICRO KHONG DAY (Pin: Warning < 30; Tin hieu: Warning < 40)
        // =================================================

        static void TestMicrophone()
        {
            Section("4. MICRO KHONG DAY");

            WirelessMicrophone mic =
                new WirelessMicrophone("MIC01", "Micro khong day");
            Recorder rec = new Recorder(mic);

            CheckEqual("Pin ban dau 100", 100, mic.BatteryLevel);
            CheckEqual("Tin hieu ban dau 100", 100, mic.SignalQuality);

            // --- Pin ---
            mic.UpdateBatteryLevel(30);
            CheckEqual("Pin 30 (bien): khong canh bao", 0, rec.Alerts.Count);

            mic.UpdateBatteryLevel(25);
            CheckEqual("Pin 25: co 1 canh bao", 1, rec.Alerts.Count);
            CheckEqual("Pin 25: muc Warning",
                AlertSeverity.Warning, rec.Alerts[0].Severity);

            mic.UpdateBatteryLevel(20);
            CheckEqual("Pin 20 (van Warning): khong canh bao lap",
                1, rec.Alerts.Count);

            mic.UpdateBatteryLevel(0);
            CheckEqual("Pin 0: co canh bao thu 2", 2, rec.Alerts.Count);
            CheckEqual("Pin 0: muc Critical",
                AlertSeverity.Critical, rec.Alerts[1].Severity);

            // --- Tin hieu ---
            mic.UpdateSignalQuality(40);
            CheckEqual("Tin hieu 40 (bien): khong canh bao",
                2, rec.Alerts.Count);

            mic.UpdateSignalQuality(35);
            CheckEqual("Tin hieu 35: co canh bao thu 3", 3, rec.Alerts.Count);
            CheckEqual("Tin hieu 35: muc Warning",
                AlertSeverity.Warning, rec.Alerts[2].Severity);

            mic.UpdateSignalQuality(0);
            CheckEqual("Tin hieu 0: co canh bao thu 4", 4, rec.Alerts.Count);
            CheckEqual("Tin hieu 0: muc Critical",
                AlertSeverity.Critical, rec.Alerts[3].Severity);

            CheckEqual("Pin va tin hieu cap nhat doc lap (pin = 0)",
                0, mic.BatteryLevel);
            CheckEqual("Pin va tin hieu cap nhat doc lap (tin hieu = 0)",
                0, mic.SignalQuality);
        }

        // =================================================
        // 5. DU LIEU KHONG HOP LE
        // =================================================

        static void TestInvalidReadings()
        {
            Section("5. DU LIEU KHONG HOP LE (NGOAI LE)");

            ProjectorRemote projector =
                new ProjectorRemote("PR01", "Dieu khien may chieu");
            WirelessMicrophone mic =
                new WirelessMicrophone("MIC01", "Micro khong day");
            Recorder projRec = new Recorder(projector);
            Recorder micRec = new Recorder(mic);

            projector.UpdateBatteryLevel(60);

            InvalidDeviceReadingException? ex =
                ExpectInvalidReading(() => projector.UpdateBatteryLevel(120));
            Check("Pin may chieu = 120: nem ngoai le", ex != null);
            CheckEqual("Ngoai le ghi dung DeviceId", "PR01", ex?.DeviceId);
            CheckEqual("Ngoai le ghi dung InvalidValue", 120, ex?.InvalidValue);
            CheckEqual("Sau ngoai le: pin giu nguyen 60",
                60, projector.BatteryLevel);
            CheckEqual("Sau ngoai le: khong phat canh bao",
                0, projRec.Alerts.Count);

            Check("Pin may chieu = -1: nem ngoai le",
                ExpectInvalidReading(() => projector.UpdateBatteryLevel(-1)) != null);
            Check("Pin may chieu = 101: nem ngoai le",
                ExpectInvalidReading(() => projector.UpdateBatteryLevel(101)) != null);
            Check("Pin micro = 150: nem ngoai le",
                ExpectInvalidReading(() => mic.UpdateBatteryLevel(150)) != null);
            Check("Tin hieu micro = -5: nem ngoai le",
                ExpectInvalidReading(() => mic.UpdateSignalQuality(-5)) != null);

            CheckEqual("Micro: pin giu nguyen 100 sau ngoai le",
                100, mic.BatteryLevel);
            CheckEqual("Micro: tin hieu giu nguyen 100 sau ngoai le",
                100, mic.SignalQuality);
            CheckEqual("Micro: khong phat canh bao khi du lieu sai",
                0, micRec.Alerts.Count);

            Check("Gia tri bien 0 la hop le",
                ExpectInvalidReading(() => projector.UpdateBatteryLevel(0)) == null);
            Check("Gia tri bien 100 la hop le",
                ExpectInvalidReading(() => projector.UpdateBatteryLevel(100)) == null);
        }

        // =================================================
        // 6. COORDINATOR VA CAC SUBSCRIBER
        // =================================================

        static void TestCoordinatorSubscribe()
        {
            Section("6. COORDINATOR VA SUBSCRIBER");

            ProjectorRemote projector =
                new ProjectorRemote("PR01", "Dieu khien may chieu");
            AirConditionerRemote ac =
                new AirConditionerRemote("AC01", "Dieu khien dieu hoa", "A101");

            ConsoleNotificationService notification = new();
            DeviceAuditLogger audit = new();
            MaintenanceService maintenance = new();

            DeviceMonitoringCoordinator coordinator =
                new DeviceMonitoringCoordinator(
                    new TeachingDevice[] { projector, ac },
                    notification, audit, maintenance);

            // Subscribe 2 lan: khong duoc dang ky trung
            coordinator.Subscribe();
            coordinator.Subscribe();

            // Bat output cua Console de dem so dong thong bao
            TextWriter original = Console.Out;
            StringWriter captured = new StringWriter();
            Console.SetOut(captured);

            try
            {
                projector.ChangeStatus(DeviceStatus.Borrowed);
                projector.UpdateBatteryLevel(15); // Warning
                ac.UpdateResponseState(false);    // Critical
            }
            finally
            {
                Console.SetOut(original);
            }

            string[] lines = captured.ToString()
                .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            CheckEqual("Subscribe 2 lan: chi in 2 dong thong bao (khong lap)",
                2, lines.Length);
            Check("Co thong bao Warning cua PR01",
                lines.Any(l => l.Contains("Warning") && l.Contains("PR01")));
            Check("Co thong bao Critical cua AC01",
                lines.Any(l => l.Contains("Critical") && l.Contains("AC01")));

            CheckEqual("AuditLogger: ghi dung 1 muc (khong lap)",
                1, audit.Entries.Count);
            Check("AuditLogger: noi dung co PR01 va Borrowed",
                audit.Entries[0].Contains("PR01") &&
                audit.Entries[0].Contains("Borrowed"));

            CheckEqual("MaintenanceService: chi nhan canh bao Critical (1 yeu cau)",
                1, maintenance.Requests.Count);
            Check("MaintenanceService: yeu cau thuoc ve AC01",
                maintenance.Requests[0].Contains("AC01"));

            coordinator.Dispose();
        }

        // =================================================
        // 7. DELEGATE LOC CANH BAO
        // =================================================

        static void TestFilteredSubscriber()
        {
            Section("7. DELEGATE LOC CANH BAO");

            WirelessMicrophone mic =
                new WirelessMicrophone("MIC01", "Micro khong day");

            List<DeviceAlertEventArgs> received = new();

            FilteredAlertSubscriber criticalOnly =
                new FilteredAlertSubscriber(
                    alert => alert.Severity == AlertSeverity.Critical,
                    alert => received.Add(alert));

            mic.AlertRaised += criticalOnly.HandleAlert;

            mic.UpdateBatteryLevel(25); // Warning -> bi loc
            CheckEqual("Warning bi loc, khong goi action", 0, received.Count);

            mic.UpdateBatteryLevel(0);  // Critical -> di qua
            CheckEqual("Critical di qua bo loc", 1, received.Count);
            CheckEqual("Action nhan dung canh bao Critical",
                AlertSeverity.Critical, received[0].Severity);

            mic.AlertRaised -= criticalOnly.HandleAlert;
            mic.UpdateSignalQuality(0);
            CheckEqual("Sau khi huy dang ky: khong nhan them canh bao",
                1, received.Count);
        }

        // =================================================
        // 8. HUY DANG KY VA DISPOSE
        // =================================================

        static void TestUnsubscribeAndDispose()
        {
            Section("8. HUY DANG KY / DISPOSE");

            AirConditionerRemote ac =
                new AirConditionerRemote("AC01", "Dieu khien dieu hoa", "A101");

            ConsoleNotificationService notification = new();
            DeviceAuditLogger audit = new();
            MaintenanceService maintenance = new();

            DeviceMonitoringCoordinator coordinator =
                new DeviceMonitoringCoordinator(
                    new TeachingDevice[] { ac },
                    notification, audit, maintenance);

            coordinator.Subscribe();
            coordinator.Unsubscribe();
            coordinator.Unsubscribe(); // goi lan 2 khong duoc loi

            TextWriter original = Console.Out;
            StringWriter captured = new StringWriter();
            Console.SetOut(captured);

            try
            {
                ac.ChangeStatus(DeviceStatus.Borrowed);
                ac.UpdateResponseState(false);
            }
            finally
            {
                Console.SetOut(original);
            }

            CheckEqual("Sau Unsubscribe: khong in thong bao",
                0, captured.ToString().Length);
            CheckEqual("Sau Unsubscribe: AuditLogger khong ghi them",
                0, audit.Entries.Count);
            CheckEqual("Sau Unsubscribe: MaintenanceService khong nhan them",
                0, maintenance.Requests.Count);

            // Dang ky lai van hoat dong binh thuong
            coordinator.Subscribe();
            ac.UpdateResponseState(true);
            ac.UpdateResponseState(false);
            CheckEqual("Dang ky lai: MaintenanceService nhan canh bao",
                1, maintenance.Requests.Count);

            // Dispose phai huy dang ky
            coordinator.Dispose();
            ac.UpdateResponseState(true);
            ac.UpdateResponseState(false);
            CheckEqual("Sau Dispose: khong nhan them yeu cau bao tri",
                1, maintenance.Requests.Count);
        }
    }
}