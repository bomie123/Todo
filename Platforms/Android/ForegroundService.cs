using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;

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
            if (Build.VERSION.SdkInt < BuildVersionCodes.Tiramisu)
            {
                StartForeground(ServiceId, MainActivity.NotificationHandler.GetDefaultNotificationBuilder().Build());
            }
            else
            {
                StartForeground(ServiceId, MainActivity.NotificationHandler.GetDefaultNotificationBuilder().Build(), ForegroundService.TypeDataSync);
            }
            
            BackgroundServiceToRun = new Timer((obj) =>
            {
                count += 1;
                Console.WriteLine("Am running");
                if (count % 10 == 0)
                {
                    MainActivity.NotificationHandler.SendNotification(MainActivity.NotificationHandler.GetDefaultNotificationBuilder("sending warning ").Build(), TodoAppNotificationChannel.WarningNotificationId);
                }
                MainActivity.NotificationHandler.SendPersistentNotification($"Got to count {count}", TodoAppNotificationChannel.ForegroundServiceNotificationId);
            }, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
            return StartCommandResult.Sticky;
        }
        public static int count { get; set; }
     

    }
}
