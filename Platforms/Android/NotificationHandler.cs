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
                manager.CreateNotificationChannel(new NotificationChannel(NotificationChannelId, NotificationChannelName, NotificationImportance.Default));
        }

        private string PersistentNotificationText { get; set; }
        public void UpdatePersistentNotification(string notificationText)
        {
            PersistentNotificationText = notificationText;
            SendNotification(GetDefaultPersistentNotificationBuilder(PersistentNotificationText).Build(), TodoAppNotificationChannel.ForegroundServiceNotificationId);
        }

        public void RestorePersistentNotification() => UpdatePersistentNotification(PersistentNotificationText);

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
                .SetDeleteIntent(GetPendingIntentForBroadcastType<OnDismissNotificationRecieved>(OnDismissNotificationRecieved.Action))
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

        private PendingIntent GetPendingIntentForBroadcastType<T>(string action) where T : BroadcastReceiver
        {
            var actionIntent = new Intent(Activity, typeof(T));
            actionIntent.SetAction(action);
            var pendingActionIntent = PendingIntent.GetBroadcast(Activity, 0, actionIntent,
                PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Immutable);
            return pendingActionIntent;
        }

    }

    [BroadcastReceiver(Enabled = true, Exported = false)]
    [IntentFilter(new[] {Action})]
    public class OnDismissNotificationRecieved : BroadcastReceiver
    {
        public const string Action = $"TodoApp.Action.DismissNotification";
        public override void OnReceive(Context? context, Intent? intent)
        {
            MainActivity.NotificationHandler.RestorePersistentNotification();
        }
    }

    public enum TodoAppNotificationChannel
    {
        ForegroundServiceNotificationId = TodoAppForegroundService.ServiceId,
        WarningNotificationId = 5090101
    }
}
