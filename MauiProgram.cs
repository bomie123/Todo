using Microsoft.Extensions.Logging;
using Serilog;
using TodoApp.Helpers;
using TodoApp.Models.DataModels;
using TodoApp.Models.DataModels.Enums;

namespace TodoApp;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateLogger();
		Log.Fatal($"Started up application");
        builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
			});

        builder.Services.AddMauiBlazorWebView();

#if DEBUG
		builder.Services.AddBlazorWebViewDeveloperTools();
		builder.Logging.AddDebug();
        #region Easy test data
        //clear old data 
        File.Delete(Path.Combine(FileSystem.Current.CacheDirectory, "todo.db"));
        DatabaseHelper.UpsertData(new TodoRecord()
        {
			Active = true,
            ActionDate = DateTime.UtcNow,
            ShowReminderBeforeDays = 0,
            Label = "Walk dog",
            MaxImportance = Importance.High,
            RepeatEvery = TimeSpan.FromDays(1)
        });
        DatabaseHelper.UpsertData(new TodoRecord()
        {
            Active = true,
            ActionDate = new DateTime(DateTime.UtcNow.Year, 11, 1),
            ShowReminderBeforeDays = 20,
            Label = "Averys birthday",
            MaxImportance = Importance.High,
            RepeatEvery = TimeSpan.FromDays(365)
        });
        DatabaseHelper.UpsertData(new TodoRecord()
        {
            Active = true,
            ActionDate = DateTime.UtcNow.AddDays(3),
            ShowReminderBeforeDays = 5,
            Label = "Drive car",
            MaxImportance = Importance.Medium,
            RepeatEvery = TimeSpan.FromDays(30)
        });
        DatabaseHelper.UpsertData(new TodoRecord()
        {
            Active = true,
            ActionDate = DateTime.UtcNow.AddDays(-50),
            Label = "Paint fence",
            MaxImportance = Importance.Medium
        });
        #endregion
#endif

        return builder.Build();


        //TODO 

        //need to implement background handler to create the todorecords & update the notification
        //Need to implement the frontend to show the elements 

        //Further expansions
        //timer - for exercize
        //implement logging 
        //mechanism to back up tasks in google drive
        //homescreen widget 
        //combined todos (a todo made up of many sub todos)
    }
}
