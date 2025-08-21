using Microsoft.Extensions.Logging;

namespace LibDynamicWallpaperWin.Utility
{ 
	public class FileLogger : ILogger
	{
		private readonly string _logFile;
		private readonly string _category;

		public FileLogger(string logFile, string category)
		{
			_logFile = logFile;
			_category = category;
		}

		public IDisposable? BeginScope<TState>(TState state) where TState : notnull
		{
			return null;
		}

		public bool IsEnabled(LogLevel logLevel)
		{
			return true;
		}

		public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
		{
			string message = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{logLevel}] [{_category}]: {formatter(state, exception)}";
			File.AppendAllText(_logFile, message + Environment.NewLine);
		}
	}
}
