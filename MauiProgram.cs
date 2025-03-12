using Microsoft.Extensions.Logging;
using Serilog;
using TodoApp.Helpers;
using TodoApp.Models.DataModels;

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
        var walkDogAction = DatabaseHelper.UpsertDataWithReturn(new TodoCreationRecord()
        {
			Active = true,
			CreateTodoEvery = new TimeSpan(24, 0, 0),
			DaysBeforeHighUrgency = 1,
			DaysBeforeLowUrgency = 0,
			DaysBeforeMediumUrgency = 0,
			TodoText = "Walk dog",
        });
		var avesBirthdayAction = DatabaseHelper.UpsertDataWithReturn(new TodoCreationRecord()
        {
			Active = true,
			CreateTodoEvery = new TimeSpan(365, 0,0,0),
			DaysBeforeHighUrgency = 14,
			DaysBeforeLowUrgency = 0,
			DaysBeforeMediumUrgency = 0,
			TodoText = "Averys birthday"
        });
		DatabaseHelper.UpsertData(new TodoCreationRecord()
        {
            Active = true,
            CreateTodoEvery = new TimeSpan(31, 0, 0, 0),
            DaysBeforeHighUrgency = 0,
            DaysBeforeLowUrgency = 4,
            DaysBeforeMediumUrgency = 1,
            TodoText = "Drive car"
        });
		DatabaseHelper.UpsertData(new TodoRecord()
        {
			Active = true,
			Completed = false,
			Deadline = DateTime.UtcNow.AddDays(0),
			TodoCreatorId = walkDogAction.First().Id
        });
        DatabaseHelper.UpsertData(new TodoRecord()
        {
            Active = true,
            Completed = false,
            Deadline = new DateTime(DateTime.UtcNow.Year, 11, 1),
            TodoCreatorId = walkDogAction.First().Id
        });

        #endregion
#endif

        return builder.Build();
	}
}
