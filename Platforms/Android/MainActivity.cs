using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using TodoApp.Platforms.Android;

namespace TodoApp;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    protected override void OnCreate(Bundle savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        //check our permissions
        GetRequiredPermission<Permissions.PostNotifications>().Wait();
        NotificationHandler =
            new NotificationHandler(this, this.GetSystemService(NotificationService) as NotificationManager);

        //create our foreground service 
        if (this.ApplicationContext.GetSystemService(nameof(TodoAppForegroundService)) == null)
            ApplicationContext.StartForegroundService(new Intent(this.ApplicationContext, typeof(TodoAppForegroundService)));

        //TODO
        //Setup nosql database and cache
    }

    protected override void OnNewIntent(Intent? intent)
    {
        base.OnNewIntent(intent);
    }

    #region Permissions

    private async Task GetRequiredPermission<T>() where T : Permissions.BasePermission, new()
    {
        while (await Permissions.CheckStatusAsync<T>() != PermissionStatus.Granted)
        {
            try
            {
                Permissions.RequestAsync<T>().Wait(TimeSpan.FromSeconds(1));
            }
            catch (Exception ex)
            {
                //permission request never returns, so just wait a second
                Thread.Sleep(1000);
            }
        }
    }
    #endregion


    public static NotificationHandler? NotificationHandler { get; set; } = null;

}
