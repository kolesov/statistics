using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kolesov.Domain.Interfaces
{
    public interface INotificationService
    {
        void SendNotification(string message);
    }
}
