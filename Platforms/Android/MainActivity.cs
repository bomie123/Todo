using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using AndroidX.Core.App;
using TodoApp.Platforms.Android;

namespace TodoApp;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    protected override void OnCreate(Bundle savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        //create our notification channel 
        var notificationManager = (GetSystemService(NotificationService) as NotificationManager);
        if (notificationManager.GetNotificationChannel(NotificationChannelId) == null)
            notificationManager.CreateNotificationChannel(new NotificationChannel(NotificationChannelId, NotificationChannelName, NotificationImportance.Default)
            {
                Description = NotificationChannelDescription
            });

        //create our main app notification 
        if (!notificationManager.GetActiveNotifications().Any(x => x.Id == NotificationId))
        {

        }

        //create our foreground service 
        if (this.ApplicationContext.GetSystemService(nameof(TodoAppForegroundService)) == null)
            ApplicationContext.StartForegroundService(new Intent(this.ApplicationContext, typeof(TodoAppForegroundService)));

    }  

    #region Notification Channel 
    public const string NotificationChannelId = "TodoAppKeepAliveNotificationChannel";
    private const string NotificationChannelName = "Todo App";
    private string NotificationChannelDescription = "Your next Activity is X long away";

    #endregion

    #region Notification Details

    public const int NotificationId = 4550602;
    private const string NotificationContentTitle = "Todo App";
    public const int IconId = global::Android.Resource.Drawable.CheckboxOnBackground;

    private Notification GetNotification(string notificationText) =>
        new NotificationCompat.Builder(this, MainActivity.NotificationChannelId)
            .SetContentTitle(NotificationContentTitle)
            .SetContentText(notificationText)
            .SetSmallIcon(IconId)
            .SetOngoing(true)
            .SetContentIntent(GetNotificationIntent())
            .SetOnlyAlertOnce(true)
            .SetPriority(NotificationCompat.PriorityHigh)
            .SetCategory(NotificationCompat.CategoryMessage)
            .SetAutoCancel(true)
            .Build();

    private PendingIntent GetNotificationIntent()
        => PendingIntent.GetActivity(this, NotificationId,
            new Intent(this, typeof(MainActivity))
                .AddFlags(ActivityFlags.NoAnimation | ActivityFlags.SingleTop),
            PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Mutable | PendingIntentFlags.OneShot);

    #endregion
}
