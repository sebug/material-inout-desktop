using material_inout_desktop.Excel;
using Microsoft.Extensions.DependencyInjection;

namespace material_inout_desktop;

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

		builder.Services.AddSingleton<ImportArticlesPage>();
		builder.Services.AddTransient<IArticlesListReader, ArticlesListReader>();

		return builder.Build();
	}
}
