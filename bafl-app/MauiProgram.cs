namespace bafl_app;

using bafl.library;

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
		builder.Services.AddSingleton<CheerCompViewModel>();
		builder.Services.AddSingleton<CheerModel>();
		builder.Services.AddSingleton<MainPage>();
		builder.Services.AddSingleton<CashAppPayView>();
        builder.Services.AddSingleton<ZellePayView>();
		builder.Services.AddSingleton<ScheduleView>();

		// build the app
        return builder.Build();
	}
}

