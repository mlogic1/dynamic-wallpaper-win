// Console based frontend for Dynamic Wallpaper tool, used for development
using LibDynamicWallpaperWin;
using LibDynamicWallpaperWin.Utility;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

var serviceCollection = new ServiceCollection();


serviceCollection.AddLogging(config =>
{
	config.AddSimpleConsole(c =>
	{
		c.TimestampFormat = "[HH:mm:ss] ";
		c.IncludeScopes = false;
	});
	config.SetMinimumLevel(LogLevel.Information);
	config.AddProvider(new LoggerFileProviderLocalAppData());
});

serviceCollection.AddSingleton<DynWallpaperService>();
serviceCollection.AddSingleton<ConfigHandlerService>();
var serviceProvider = serviceCollection.BuildServiceProvider();

var wallpaperService = serviceProvider.GetRequiredService<DynWallpaperService>();
var configService = serviceProvider.GetRequiredService<ConfigHandlerService>();
configService.GetStoredActiveWallpaper();
var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

logger.LogInformation("Starting Dynamic Wallpaper Win [Console Version]");

try
{
	foreach (var arg in args)
	{
		wallpaperService.AddWallpaper(arg);
	}
	
	wallpaperService.SetActiveWallpaper("earth");
}catch(Exception ex)
{
	logger.LogError(ex.ToString());
}

Console.ReadLine();