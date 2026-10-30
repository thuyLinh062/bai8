//Họ và Tên: Trần Thùy Linh
//MSSV: 202418935
using System;

namespace QuanLyThietBiPhongHoc
{
    public abstract class TeachingDevice
    {
        public string Id { get; }
        public string Name { get; }

        public DeviceStatus Status { get; private set; }

        // Khai bao Event
        public event EventHandler<DeviceStatusChangedEventArgs>?
            StatusChanged;

        public event EventHandler<DeviceAlertEventArgs>?
            AlertRaised;

        protected TeachingDevice(string id, string name)
        {
            Id = id;
            Name = name;
            Status = DeviceStatus.Available;
        }

        // Thay doi trang thai
        public void ChangeStatus(DeviceStatus newStatus)
        {
            // Neu trang thai khong thay doi thi khong phat Event
            if (newStatus == Status)
            {
                return;
            }

            DeviceStatus oldStatus = Status;

            // Cap nhat trang thai truoc
            Status = newStatus;

            // Phat sinh Event
            OnStatusChanged(
                new DeviceStatusChangedEventArgs(
                    Id,
                    oldStatus,
                    newStatus,
                    DateTime.Now
                )
            );
        }

        // Phat sinh canh bao
        protected void RaiseAlert(
            string message,
            AlertSeverity severity)
        {
            OnAlertRaised(
                new DeviceAlertEventArgs(
                    Id,
                    message,
                    severity,
                    DateTime.Now
                )
            );
        }

        // Phat sinh Event thay doi trang thai
        protected virtual void OnStatusChanged(
            DeviceStatusChangedEventArgs e)
        {
            StatusChanged?.Invoke(this, e);
        }

        // Phat sinh Event canh bao
        protected virtual void OnAlertRaised(
            DeviceAlertEventArgs e)
        {
            AlertRaised?.Invoke(this, e);
        }
    }
}