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
        var walkDogAction = DatabaseHelper.UpsertDataWithReturn(new TodoCreationRecord()
        {
			Active = true,
			CreateTodoEvery = new TimeSpan(24, 0, 0),
			TodoText = "Walk dog",
            TaskUrgency = Urgency.High
        });
		var avesBirthdayAction = DatabaseHelper.UpsertDataWithReturn(new TodoCreationRecord()
        {
			Active = true,
			CreateTodoEvery = new TimeSpan(365, 0,0,0),
            TodoText = "Averys birthday",
            PrepWorkUrgency = Urgency.Low,
            TaskUrgency = Urgency.High,
            ShowPrepWorkDaysBefore = 14
        });
		var driveCarAction = DatabaseHelper.UpsertDataWithReturn(new TodoCreationRecord()
        {
            Active = true,
            CreateTodoEvery = new TimeSpan(31, 0, 0, 0),
            TodoText = "Drive car",
            TaskUrgency = Urgency.Medium
        });
        var paintFenceAction = DatabaseHelper.UpsertDataWithReturn(new TodoCreationRecord()
        {
            Active = true,
            TodoText = "Paint fence",
            TaskUrgency = Urgency.Medium,
        });
        DatabaseHelper.UpsertData(new TodoRecord()
        {
			Active = true,
			Deadline = DateTime.UtcNow.AddDays(0),
			TodoCreatorId = walkDogAction.First().Id,
        });
        DatabaseHelper.UpsertData(new TodoRecord()
        {
            Active = true,
            Deadline = new DateTime(DateTime.UtcNow.Year, 11, 1),
            TodoCreatorId = avesBirthdayAction.First().Id
        });
        DatabaseHelper.UpsertData(new TodoRecord()
        {
            Active = true,
            Deadline = DateTime.UtcNow.AddDays(31),
            TodoCreatorId = driveCarAction.First().Id
        });
        DatabaseHelper.UpsertData(new TodoRecord()
        {
            Active = true,
            Deadline = DateTime.UtcNow.AddDays(-20),
            TodoCreatorId = paintFenceAction.First().Id
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
