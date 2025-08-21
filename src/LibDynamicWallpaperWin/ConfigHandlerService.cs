using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace LibDynamicWallpaperWin
{
	public class ConfigHandlerService
	{
		private const string APP_DATA_DIR_NAME = "DynamicWallpaperDyn";
		private const string CONFIG_FILE_NAME = "meta.json";
		private readonly string _configFile;

		private readonly ILogger _logger;
		private MetaConfig _metaConfig;

		public ConfigHandlerService(ILogger<ConfigHandlerService> logger)
		{
			_logger = logger;
			string appDataDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), APP_DATA_DIR_NAME);
			_configFile = Path.Combine(appDataDir, CONFIG_FILE_NAME);
			if (File.Exists(_configFile))
			{
				LoadConfig();
			}
			else
			{
				_metaConfig = new MetaConfig
				{
					CurrentActiveWallpaper = null,
					MetaFiles = new List<string>()
				};
			}
		}

		public string? GetStoredActiveWallpaper()
		{
			return _metaConfig.CurrentActiveWallpaper;
		}

		public List<string> GetMetaFiles()
		{
			return _metaConfig.MetaFiles.ToList();
		}

		private void LoadConfig()
		{
			try
			{
				_logger.LogInformation("Loading meta config from config file");
				string metaFileContent = File.ReadAllText(_configFile);
				MetaConfig? conf = JsonSerializer.Deserialize<MetaConfig>(metaFileContent);
				if (conf != null)
				{
					_metaConfig = conf;
				}
			}
			catch (Exception ex)
			{
				_logger.LogCritical($"Unable to load meta config file {_configFile}. Clearing the config and resetting it");
				_metaConfig = new MetaConfig
				{
					CurrentActiveWallpaper = null,
					MetaFiles = new List<string>()
				};
			}
			
		}

		internal void SaveConfig(string? activeWallpaperName, List<string> wallpaperMetaFiles)
		{
			_metaConfig.CurrentActiveWallpaper = activeWallpaperName;
			_metaConfig.MetaFiles = wallpaperMetaFiles;
			string serializedConfig = JsonSerializer.Serialize<MetaConfig>(_metaConfig);
			File.WriteAllText(_configFile, serializedConfig);
		}

		internal void ClearConfig()
		{
			_metaConfig.CurrentActiveWallpaper = null;
			_metaConfig.MetaFiles.Clear();
		}
	}
}
