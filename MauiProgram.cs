using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using TodoApp.Helpers;
using TodoApp.Models;

namespace TodoApp;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		//TODO fix this
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
		DatabaseHelper.UpsertData(new TodoRecord()
        {
			Description = "Test"
        });
        var test = DatabaseHelper.GetData<TodoRecord>("true");

		builder.Services.AddMauiBlazorWebView();

#if DEBUG
		builder.Services.AddBlazorWebViewDeveloperTools();
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
