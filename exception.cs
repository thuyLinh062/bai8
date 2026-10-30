//Họ và Tên: Trần Thùy Linh
//MSSV: 202418935
using System;

namespace QuanLyThietBiPhongHoc
{
    public class InvalidDeviceReadingException : Exception
    {
        public string DeviceId { get; }
        public int InvalidValue { get; }

        public InvalidDeviceReadingException(
            string deviceId,
            int invalidValue)
            : base($"Gia tri {invalidValue} cua thiet bi {deviceId} khong hop le. Phai tu 0 den 100.")
        {
            DeviceId = deviceId;
            InvalidValue = invalidValue;
        }
    }
}