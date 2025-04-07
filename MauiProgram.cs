using Blazored.Modal;
using Microsoft.Extensions.Logging;
using Serilog;
using TodoApp.Helpers;

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
        DatabaseHelper.DeleteDatabase();
        builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
			});

        builder.Services.AddMauiBlazorWebView();
        builder.Services.AddBlazoredModal();

#if DEBUG
		builder.Services.AddBlazorWebViewDeveloperTools();
		builder.Logging.AddDebug();
#endif

        return builder.Build();


        //TODO 

        //need to implement background handler to create the todorecords & update the notification
        //Need to implement the frontend to show the elements 

        //Further expansions
        //timer - for exercize
        //implement logging 
        //calendar integration?
        //mechanism to back up tasks in google drive
        //homescreen widget 
        //combined todos (a todo made up of many sub todos)
    }
}
