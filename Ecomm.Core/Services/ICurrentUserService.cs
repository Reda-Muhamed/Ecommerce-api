using System;
using System.Collections.Generic;
using System.Text;

namespace Ecomm.Core.Services
{
    public interface ICurrentUserService
    {
        Guid? UserId { get; }
        string? Email { get; }
        bool IsAuthenticated { get; }
    }

}
