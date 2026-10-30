//Họ và Tên: Trần Thùy Linh
//MSSV: 202418935
using System;

namespace QuanLyThietBiPhongHoc
{
    public delegate bool AlertFilter(DeviceAlertEventArgs alert);

    public class FilteredAlertSubscriber
    {
        private readonly AlertFilter filter;

        private readonly Action<DeviceAlertEventArgs> action;

        public FilteredAlertSubscriber(
            AlertFilter filter,
            Action<DeviceAlertEventArgs> action)
        {
            this.filter = filter;
            this.action = action;
        }

        public void HandleAlert(
            object? sender,
            DeviceAlertEventArgs e)
        {
            // Kiem tra dieu kien loc
            if (filter(e))
            {
                // Thuc hien hanh dong neu thoa man
                action(e);
            }
        }
    }
}