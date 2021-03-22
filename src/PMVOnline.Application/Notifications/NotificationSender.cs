using CorePush.Google;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using PMVOnline.Users;
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace PMVOnline.Notifications
{
    public class NotificationData
    {
        [AliasAs("body")]
        public string Body { get; set; }
        public string Title { get; set; }
        public string Sound { get; set; } = "default";
    }

    public class AndroidData
    {
        [AliasAs("notification")]
        [JsonProperty("notification")]
        public NotificationData Notification { get; set; }
        [AliasAs("registration_ids")]
        [JsonProperty("registration_ids")]
        public string[] RegistrationIds { get; set; }
    }

    public interface FCMApi
    {
        
        [Post("/fcm/send")]
        Task<ApiResponse<object>> SendNotification([Body] AndroidData request, [Header("Authorization")] string key= "key=AAAACAdSj6U:APA91bGt28YYcQoPvxNFwjbgN6W97U-rvHtYkefYBS6jKuLtda-L-phzutV4KKoJ5YEss8aETBmy62ID9XtxxEmvRpCD7HNkg0EQxgKS1li3bax00r14Ws8r0zJKooKYxiNl7fB-G5hl");
    }

    public class NotificationSender : ITransientDependency, INotificationSender
    {
        readonly IDeviceTokenManager deviceTokenManager;
        readonly IConfiguration configuration;

        public NotificationSender(IDeviceTokenManager deviceTokenManager, IConfiguration configuration)
        {
            this.deviceTokenManager = deviceTokenManager;
            this.configuration = configuration;
        }
        public static JsonSerializerSettings SnakeCaseSettings => new JsonSerializerSettings
        {
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new SnakeCaseNamingStrategy()
            }
        };

        public static RefitSettings SnakeCaseNaming => new RefitSettings
        {
            ContentSerializer = new NewtonsoftJsonContentSerializer(SnakeCaseSettings)
        };
        public async Task SendNotifications(Guid uid, string message)
        {
            var api = RestService.For<FCMApi>("https://fcm.googleapis.com", SnakeCaseNaming);
            var tokens = await deviceTokenManager.GetUserDevicesAsync(uid);
            await api.SendNotification(new AndroidData { RegistrationIds = tokens.Select(d => d.Token).ToArray(), Notification = new NotificationData { Title = "PMV Online", Body = message } });
        }

        public async Task SendNotifications(Guid[] uid, string message)
        {
            var api = RestService.For<FCMApi>("https://fcm.googleapis.com", SnakeCaseNaming);
            var tokens = await deviceTokenManager.GetUsersDevicesAsync(uid); 
            await api.SendNotification(new AndroidData { RegistrationIds = tokens.Select(d => d.Token).ToArray(), Notification = new NotificationData { Title = "PMV Online", Body = message } });
        }
    }
}
