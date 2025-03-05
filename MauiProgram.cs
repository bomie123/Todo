using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;

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

		builder.Services.AddMauiBlazorWebView();

#if DEBUG
		builder.Services.AddBlazorWebViewDeveloperTools();
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
