using Android.App;
using Android.Content;
using AndroidX.Core.App;

namespace TodoApp.Platforms.Android
{
    public class NotificationHandler
    {
        public const string NotificationChannelId = "TodoAppKeepAliveNotificationChannel";
        private const string NotificationChannelName = "Todo App";

        private const int IconId = global::Android.Resource.Drawable.CheckboxOnBackground;

        public NotificationHandler(MainActivity activity, NotificationManager manager)
        {
            AndroidManager = manager;
            Activity = activity;
            if (manager.GetNotificationChannel(NotificationChannelId) == null)
                manager.CreateNotificationChannel(new NotificationChannel(NotificationChannelId, NotificationChannelName, NotificationImportance.Min));
        }


        public void SendNotification(string notificationText, TodoAppNotificationChannel channel) =>
            SendNotification(GetDefaultNotificationBuilder(notificationText).Build(), channel);

        public void SendNotification(Notification notification, TodoAppNotificationChannel channel) =>
            AndroidManager.Notify((int)channel, notification);


        private NotificationManager AndroidManager { get; set; }
        private MainActivity Activity { get; set; }

        public NotificationCompat.Builder GetDefaultNotificationBuilder(string notificationText) =>
            new NotificationCompat.Builder(Activity, NotificationChannelId)
                .SetContentTitle(NotificationChannelName)
                .SetContentText(notificationText)
                .SetContentIntent(GetNotificationIntent())
                .SetSmallIcon(IconId)
                .SetOnlyAlertOnce(true)
                .SetOngoing(true)
                .SetSilent(true)
                .SetPriority(NotificationCompat.PriorityMin)
                .SetCategory(NotificationCompat.CategoryStatus);

        public NotificationCompat.Builder GetDefaultNotificationBuilder() =>
            GetDefaultNotificationBuilder("");

        private PendingIntent GetNotificationIntent()
            => PendingIntent.GetActivity(Activity, 0,
                new Intent(Activity, typeof(MainActivity))
                    .AddFlags(ActivityFlags.NoAnimation),
                PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Mutable);
    }

    public enum TodoAppNotificationChannel
    {
        ForegroundServiceNotificationId = TodoAppForegroundService.ServiceId,
        WarningNotificationId = 5090101
    }
}
