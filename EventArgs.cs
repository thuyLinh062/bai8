//Họ và Tên: Trần Thùy Linh
//MSSV: 202418935
using System;

namespace QuanLyThietBiPhongHoc
{
    // Dữ liệu khi trạng thái thiết bị thay đổi
    public class DeviceStatusChangedEventArgs : EventArgs
    {
        public string DeviceId { get; }
        public DeviceStatus OldStatus { get; }
        public DeviceStatus NewStatus { get; }
        public DateTime ChangedAt { get; }

        public DeviceStatusChangedEventArgs(
            string deviceId,
            DeviceStatus oldStatus,
            DeviceStatus newStatus,
            DateTime changedAt)
        {
            DeviceId = deviceId;
            OldStatus = oldStatus;
            NewStatus = newStatus;
            ChangedAt = changedAt;
        }
    }

    // Dữ liệu khi thiết bị phát sinh cảnh báo
    public class DeviceAlertEventArgs : EventArgs
    {
        public string DeviceId { get; }
        public string Message { get; }
        public AlertSeverity Severity { get; }
        public DateTime OccurredAt { get; }

        public DeviceAlertEventArgs(
            string deviceId,
            string message,
            AlertSeverity severity,
            DateTime occurredAt)
        {
            DeviceId = deviceId;
            Message = message;
            Severity = severity;
            OccurredAt = occurredAt;
        }
    }
}