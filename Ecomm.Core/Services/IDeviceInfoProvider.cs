using Ecomm.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ecomm.Core.Services
{
    public interface IDeviceInfoProvider
    {
        DeviceInfoDto GetDeviceInfo();

    }
}
