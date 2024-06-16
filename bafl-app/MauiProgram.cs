namespace bafl_app;

using bafl_app.library;

/// <summary>
/// Core program.
/// </summary>
public static class MauiProgram
{
	/// <summary>
	/// Create the MAUI application.
	/// </summary>
	/// <returns>The MAUI app instance.</returns>
	public static MauiApp CreateMauiApp()
	{
		// construct the app
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

        // instantiate the views
        builder.Services.AddSingleton<CheerCompView>();
        builder.Services.AddSingleton<DrillCompView>();
        builder.Services.AddSingleton<MainPage>();
		builder.Services.AddSingleton<SquareView>();
        builder.Services.AddSingleton<ZellePayView>();
		builder.Services.AddSingleton<BylawsView>();
        builder.Services.AddSingleton<ScheduleViewRegular>();
        builder.Services.AddSingleton<ScheduleView9v9>();

        // build the app
        return builder.Build();
	}
}

