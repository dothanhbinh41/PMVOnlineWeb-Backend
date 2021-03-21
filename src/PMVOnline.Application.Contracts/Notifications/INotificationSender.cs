using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace PMVOnline.Notifications
{
    public interface INotificationSender
    {
        Task SendNotifications(Guid uid, string message);
        Task SendNotifications(Guid[] uid, string message);
    } 
}
