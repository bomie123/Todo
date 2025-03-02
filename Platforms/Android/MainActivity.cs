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
    #region Permissions

    private async Task GetRequiredPermission<T>() where T : Permissions.BasePermission, new()
    {

        if (await Permissions.CheckStatusAsync<T>() != PermissionStatus.Granted)
        {
            while (await Permissions.RequestAsync<T>() != PermissionStatus.Granted) { }
        }
    }
    #endregion


    public static NotificationHandler? NotificationHandler { get; set; } = null;

}
