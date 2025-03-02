using Android.App;
using Android.Content;
using Android.OS;
using AndroidX.Core.App;
using Application = Android.App.Application;

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
                manager.CreateNotificationChannel(new NotificationChannel(NotificationChannelId, NotificationChannelName, NotificationImportance.Default));
        }


        public void SendPersistentNotification(string notificationText, TodoAppNotificationChannel channel) =>
            SendNotification(GetDefaultPersistentNotificationBuilder(notificationText).Build(), channel);

        public void SendNotification(Notification notification, TodoAppNotificationChannel channel) =>
        AndroidManager.Notify((int)channel, notification);


        private NotificationManager AndroidManager { get; set; }
        private MainActivity Activity { get; set; }

        public NotificationCompat.Builder GetDefaultPersistentNotificationBuilder(string notificationText) =>
            new NotificationCompat.Builder(Activity, NotificationChannelId)
                .SetContentTitle(NotificationChannelName)
                .SetContentText(notificationText)
                .SetContentIntent(OpenMainActivityPageIntent())
                .SetSmallIcon(IconId)
                .SetOnlyAlertOnce(true)
                .SetDeleteIntent(OpenMainActivityPageIntent())
                .SetOngoing(true)
                .SetSilent(true)
                .SetPriority(NotificationCompat.PriorityMin)
                .SetCategory(NotificationCompat.CategoryStatus);

        public NotificationCompat.Builder GetDefaultNotificationBuilder(string notificationText) =>
            new NotificationCompat.Builder(Activity, NotificationChannelId)
                .SetContentTitle(NotificationChannelName)
                .SetContentText(notificationText)
                .SetContentIntent(OpenMainActivityPageIntent())
                .SetSmallIcon(IconId)
                .SetPriority(NotificationCompat.PriorityDefault)
                .SetCategory(NotificationCompat.CategoryStatus);


        public NotificationCompat.Builder GetDefaultNotificationBuilder() =>
            GetDefaultPersistentNotificationBuilder("");

        private PendingIntent OpenMainActivityPageIntent()
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
