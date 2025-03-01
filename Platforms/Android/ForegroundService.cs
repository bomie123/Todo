using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using AndroidX.Core.App;

namespace TodoApp.Platforms.Android
{
    [Service(Name="todoApp.bomie.TodoAppForegroundService")]
    class TodoAppForegroundService : Service

    {
        private static Timer BackgroundServiceToRun { get; set; }
        public override IBinder? OnBind(Intent? intent) => null;
        public const int ServiceId = 15340201;
        public override StartCommandResult OnStartCommand(Intent? intent, StartCommandFlags flags, int startId)
        {
            var notification = GetNotification("TestIntro");
            if (Build.VERSION.SdkInt < BuildVersionCodes.Tiramisu)
            {
                StartForeground(ServiceId, notification);
            }
            else
            {
                StartForeground(ServiceId, notification, ForegroundService.TypeDataSync);
            }
            NotificationManagerCompat.From(this).Notify(NotificationId, notification);
            BackgroundServiceToRun = new Timer((obj) =>
            {
                Console.WriteLine("Am running");
            }, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
            return StartCommandResult.Sticky;
        }
     

    }
}
