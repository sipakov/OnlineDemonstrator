using System;

namespace OnlineDemonstrator.Libraries.Domain.Dto
{
    public class DeviceIn
    {
        public string DeviceId { get; set; }

        public bool IsLicenseActivated { get; set; }

        public int OsId { get; set; }
    }
}