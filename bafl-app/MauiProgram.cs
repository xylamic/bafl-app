namespace bafl_app;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

        builder.Services.AddSingleton<CheerCompView>();
		builder.Services.AddSingleton<CheerCompViewModel>();
		builder.Services.AddSingleton<CheerModel>();
		builder.Services.AddSingleton<MainPage>();

        return builder.Build();
	}
}

