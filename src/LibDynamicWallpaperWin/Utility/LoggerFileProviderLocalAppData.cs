using Microsoft.Extensions.Logging;

namespace LibDynamicWallpaperWin.Utility
{
	public class LoggerFileProviderLocalAppData : ILoggerProvider
	{
		private const string APP_DATA_DIR_NAME = "DynamicWallpaperDyn";
		private const string LOGGER_FILE_NAME = "DynamicWallpaperDyn.log";
		private readonly string appDataDir;

		public LoggerFileProviderLocalAppData()
		{
			appDataDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), APP_DATA_DIR_NAME);
			try
			{
				var dirInfo = Directory.CreateDirectory(appDataDir);
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message);
			}

			ClearOldLogFile();
		}

		private void ClearOldLogFile()
		{
			string logFile = Path.Combine(appDataDir, LOGGER_FILE_NAME);
			if (!File.Exists(logFile)) { return; }

			DateTime now = DateTime.Now;
			if ((now - File.GetCreationTime(logFile)).TotalDays > 7)
			{
				try
				{
					File.Delete(logFile);
				}
				catch (Exception) { }
			}
		}

		public ILogger CreateLogger(string categoryName)
		{
			string logFile = Path.Combine(appDataDir, LOGGER_FILE_NAME);
			return new FileLogger(logFile, categoryName);
		}

		public void Dispose()
		{
			
		}
	}
}
