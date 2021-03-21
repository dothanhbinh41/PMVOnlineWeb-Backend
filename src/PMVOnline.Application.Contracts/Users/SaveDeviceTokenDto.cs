using System;
using System.Collections.Generic;
using System.Text;

namespace PMVOnline.Users
{
    public class SaveDeviceTokenDto
    {
        public string Token { get; set; }
        public DeviceType Device { get; set; }
    }
}
