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
var serviceProvider = serviceCollection.BuildServiceProvider();

var wallpaperService = serviceProvider.GetRequiredService<DynWallpaperService>();
var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

logger.LogInformation("Starting Dynamic Wallpaper Win [Console Version]");

try
{
	if (args.Length < 1)
	{
		logger.LogCritical("Console version requires at least 1 argument (path to meta file) to function.");
		Environment.Exit(1);
	}

	string lastAddedWallpaper = "";
	foreach (var arg in args)
	{
		lastAddedWallpaper = wallpaperService.AddWallpaper(arg);
	}
	
	wallpaperService.SetActiveWallpaper(lastAddedWallpaper);
}catch(Exception ex)
{
	logger.LogError(ex.ToString());
}

Console.ReadLine();