//Họ và Tên: Trần Thùy Linh
//MSSV: 202418935
using System;

namespace QuanLyThietBiPhongHoc
{
    // PROJECTOR REMOTE

    public class ProjectorRemote : TeachingDevice
    {
        public int BatteryLevel { get; private set; }

        public ProjectorRemote(string id, string name)
            : base(id, name)
        {
            BatteryLevel = 100;
        }

        public void UpdateBatteryLevel(int newLevel)
        {
            // Kiem tra du lieu
            if (newLevel < 0 || newLevel > 100)
            {
                throw new InvalidDeviceReadingException(
                    Id, newLevel);
            }

            // Xac dinh muc canh bao cu va moi
            AlertSeverity? oldSeverity = GetSeverity(BatteryLevel);
            AlertSeverity? newSeverity = GetSeverity(newLevel);

            BatteryLevel = newLevel;

            // Chi canh bao khi chuyen sang mot muc moi
            if (newSeverity.HasValue &&
                newSeverity != oldSeverity)
            {
                if (newSeverity == AlertSeverity.Critical)
                {
                    RaiseAlert(
                        "Pin dieu khien may chieu da het!",
                        AlertSeverity.Critical);
                }
                else
                {
                    RaiseAlert(
                        "Pin dieu khien may chieu duoi 20%.",
                        AlertSeverity.Warning);
                }
            }
        }

        private AlertSeverity? GetSeverity(int level)
        {
            if (level == 0)
            {
                return AlertSeverity.Critical;
            }

            if (level < 20)
            {
                return AlertSeverity.Warning;
            }

            return null;
        }
    }

    // AIR CONDITIONER REMOTE

    public class AirConditionerRemote : TeachingDevice
    {
        public string AssignedRoom { get; }

        public bool IsResponsive { get; private set; }

        public AirConditionerRemote(
            string id,
            string name,
            string assignedRoom)
            : base(id, name)
        {
            AssignedRoom = assignedRoom;
            IsResponsive = true;
        }

        public void UpdateResponseState(bool isResponsive)
        {
            // Khong co thay doi thi khong lam gi
            if (IsResponsive == isResponsive)
            {
                return;
            }

            IsResponsive = isResponsive;

            // Chi canh bao khi mat phan hoi
            if (!IsResponsive)
            {
                RaiseAlert(
                    $"Dieu khien dieu hoa tai phong {AssignedRoom} khong phan hoi.",
                    AlertSeverity.Critical);
            }
        }
    }

    // WIRELESS MICROPHONE

    public class WirelessMicrophone : TeachingDevice
    {
        public int BatteryLevel { get; private set; }

        public int SignalQuality { get; private set; }

        public WirelessMicrophone(string id, string name)
            : base(id, name)
        {
            BatteryLevel = 100;
            SignalQuality = 100;
        }

        // Cap nhat pin
        public void UpdateBatteryLevel(int newLevel)
        {
            if (newLevel < 0 || newLevel > 100)
            {
                throw new InvalidDeviceReadingException(
                    Id, newLevel);
            }

            AlertSeverity? oldSeverity = GetBatterySeverity(BatteryLevel);
            AlertSeverity? newSeverity = GetBatterySeverity(newLevel);

            BatteryLevel = newLevel;

            if (newSeverity.HasValue &&
                newSeverity != oldSeverity)
            {
                if (newSeverity == AlertSeverity.Critical)
                {
                    RaiseAlert(
                        "Pin micro da het!",
                        AlertSeverity.Critical);
                }
                else
                {
                    RaiseAlert(
                        "Pin micro duoi 30%.",
                        AlertSeverity.Warning);
                }
            }
        }

        // Cap nhat chat luong tin hieu
        public void UpdateSignalQuality(int newQuality)
        {
            if (newQuality < 0 || newQuality > 100)
            {
                throw new InvalidDeviceReadingException(
                    Id, newQuality);
            }

            AlertSeverity? oldSeverity = GetSignalSeverity(SignalQuality);
            AlertSeverity? newSeverity = GetSignalSeverity(newQuality);

            SignalQuality = newQuality;

            if (newSeverity.HasValue &&
                newSeverity != oldSeverity)
            {
                if (newSeverity == AlertSeverity.Critical)
                {
                    RaiseAlert(
                        "Tin hieu micro da mat hoan toan!",
                        AlertSeverity.Critical);
                }
                else
                {
                    RaiseAlert(
                        "Chat luong tin hieu micro duoi 40%.",
                        AlertSeverity.Warning);
                }
            }
        }

        private AlertSeverity? GetBatterySeverity(int level)
        {
            if (level == 0)
            {
                return AlertSeverity.Critical;
            }

            if (level < 30)
            {
                return AlertSeverity.Warning;
            }

            return null;
        }

        private AlertSeverity? GetSignalSeverity(int quality)
        {
            if (quality == 0)
            {
                return AlertSeverity.Critical;
            }

            if (quality < 40)
            {
                return AlertSeverity.Warning;
            }

            return null;
        }
    }

}