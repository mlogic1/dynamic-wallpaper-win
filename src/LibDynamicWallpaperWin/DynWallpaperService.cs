// some resources for icons perhaps: https://stackoverflow.com/questions/4142203/does-net-have-icon-collections

using Microsoft.Extensions.Logging;

namespace LibDynamicWallpaperWin
{
    public class DynWallpaperService
    {
        private List<DynamicWallpaper> _wallpaperList;
        private DynamicWallpaper? _activeWallpaper;
        private CancellationTokenSource _schedulerCtoken;   // todo implement shutdown

		private readonly ILogger<DynWallpaperService> _logger;
		private readonly ConfigHandlerService? _configHandler;

		public DynWallpaperService(ILogger<DynWallpaperService> logger, ConfigHandlerService? configHandler)
        {
            _logger = logger;
            _configHandler = configHandler;
            _wallpaperList = new List<DynamicWallpaper>();
            _activeWallpaper = null;
			_schedulerCtoken = new CancellationTokenSource();

            if (_configHandler != null) {
                InitializeFromConfig();
                _logger.LogInformation($"Dynamic wallpaper service initialized from configuration file.");
            }
		}

        private void InitializeFromConfig()
        {
            foreach (var metaFile in _configHandler!.GetMetaFiles())
            {
				try
				{
					var r = DynamicWallpaperLoader.LoadDynamicWallaper(metaFile);
					_wallpaperList.Add(r);
					_logger.LogInformation($"Loaded and added dynamic wallpaper {r.Name} from config meta file {metaFile}.");
				}
				catch (Exception ex)
				{
					_logger.LogError($"Failed to load dynamic wallpaper {metaFile} from config file. {ex.Message.ToString()}");
                    // config might be corrupted, clear it
                    _configHandler.ClearConfig();
				}
			}

            string? activeWallpaperFromConfig = _configHandler!.GetStoredActiveWallpaper();

			if (activeWallpaperFromConfig != null)
            {
                DynamicWallpaper? wallpaper = _wallpaperList.FirstOrDefault(c => c.Name == activeWallpaperFromConfig);
                if (wallpaper != null)
                {
					_activeWallpaper = wallpaper;
                    UpdateWallpaper(); // This will start the scheduler
				}
            }
        }

		public string? GetActiveWallpaper()
        {
            if (_activeWallpaper == null)
            {
				return null;
			}
            return _activeWallpaper.Name;
        }

		public List<string> GetWallpapers()
        {
            return _wallpaperList.Select(c => c.Name).ToList();
        }

        public string AddWallpaper(string wallpaperMetaFile)
        {
            try
            {
                var r = DynamicWallpaperLoader.LoadDynamicWallaper(wallpaperMetaFile);

                // check if a wallpaper with this ID (name) already exists
                if (_wallpaperList.Count(c => c.Name == r.Name) > 0)
                {
					_logger.LogWarning($"Wallaper with name '{r.Name}' is already added");
					throw new Exception($"Wallaper with name '{r.Name}' is already added");
                }

                _wallpaperList.Add(r);
				_logger.LogInformation($"Loaded and added dynamic wallpaper {r.Name} from meta file {wallpaperMetaFile}.");
                SaveConfig();
				return r.Name;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to load dynamic wallpaper {wallpaperMetaFile}. {ex.Message.ToString()}");
                throw new Exception(ex.Message);
            }
        }

        public bool RemoveWallpaper(string wallpaper)
        {
            DynamicWallpaper? wp = _wallpaperList.FirstOrDefault(c => c.Name.Equals(wallpaper));
            if (wp == null)
            {
                return false;
            }

            if (_activeWallpaper == wp)
            {
                ClearActiveWallpaper();
            }
            bool removeSuccess = _wallpaperList.Remove(wp);
			SaveConfig();
            return removeSuccess;
        }


        // Setting the active wallpaper will start the scheduler, from that point on, the scheduler keeps re-scheduling itself until cancelled or application exit
        public bool SetActiveWallpaper(string wallpaper)
        {
            DynamicWallpaper? wp = _wallpaperList.FirstOrDefault(c => c.Name.Equals(wallpaper));
			if (wp == null)
			{
				_logger.LogInformation($"Setting active wallpaper to '{wallpaper}' failed. Wallpaper not found.");
				return false;
			}
			ClearActiveWallpaper();
			_activeWallpaper = wp;
			_logger.LogInformation($"Setting active wallpaper to '{wallpaper}'");
			UpdateWallpaper();
            SaveConfig();
			return true;
		}

        public bool ClearActiveWallpaper()
        {
            if (_activeWallpaper == null)
            {
                return false;
            }
            _logger.LogInformation("Clearing active wallpaper.");
			_schedulerCtoken.Cancel();
			_activeWallpaper = null;
            SaveConfig();

			return true;
        }

        // This function immediately updates the wallpaper, it can be called by the application layer,
        // it also cancels any active timer and immediately sets up the next schedule
        public bool UpdateWallpaper()
        {
            if (_activeWallpaper == null)
            {
                return false;
            }

            _schedulerCtoken.Cancel(); // cancel any previous timers
            string wallpaperFile = GetClosestWallpaperImageToSystemTime(_activeWallpaper).FileName;

            _logger.LogInformation($"Updating system wallpaper to {wallpaperFile}");
			WinAPIFunctions.UpdateSystemWallpaper(wallpaperFile);

			_schedulerCtoken = new CancellationTokenSource();
			_ = RunSchedulerAsync(_schedulerCtoken.Token);

			return true;
        }

        private async Task RunSchedulerAsync(CancellationToken ctoken)
        {
			while (!ctoken.IsCancellationRequested)
            {
				if (_activeWallpaper == null)
					return;

				TimeOnly now = TimeOnly.FromDateTime(DateTime.Now);
				DynamicWallpaperImage nextImage = GetFirstWallpaperImageAfterTime(_activeWallpaper, now);
				// time until next image
				TimeSpan timeUntilNext = nextImage.Time > now
					? nextImage.Time.ToTimeSpan() - now.ToTimeSpan()
					: (TimeSpan.FromHours(24) - now.ToTimeSpan()) + nextImage.Time.ToTimeSpan();

				_logger.LogInformation($"Scheduler setting up, next image: {nextImage.FileName}\n\tat {nextImage.Time}, in {(int)timeUntilNext.TotalMinutes} min.");
				await Task.Delay(timeUntilNext, ctoken);

				if (WinAPIFunctions.UpdateSystemWallpaper(GetClosestWallpaperImageToSystemTime(_activeWallpaper).FileName))
                {
					_logger.LogInformation($"Scheduler successfully updated wallaper to {nextImage.FileName}.");
				}
                else
                {
                    _logger.LogError($"Scheduler failed to update wallpaper to {nextImage.FileName}. Check if the file exists");
                }
                await Task.Delay(500);  // wait just a tiny bit
			}
		}

        private DynamicWallpaperImage GetFirstWallpaperImageAfterTime(DynamicWallpaper wallpaper, TimeOnly time)
        {
            DynamicWallpaperImage? next = wallpaper.Images
                .Where(t => t.Time > time)
                .OrderBy(t => t.Time)
                .FirstOrDefault();

			if (next == null)
            {
                // take first, the system clock is beyond any other image
                DynamicWallpaperImage first = wallpaper.Images
                .OrderBy(t => t.Time)
                .First();

                return first;
			}
            else
            {
                return next;
            }
		}

        private DynamicWallpaperImage GetClosestWallpaperImageToSystemTime(DynamicWallpaper wallpaper)
        {
            TimeOnly now = TimeOnly.FromDateTime(DateTime.Now);

            DynamicWallpaperImage closest = wallpaper.Images
                .OrderBy(t => Math.Abs((t.Time.ToTimeSpan() - now.ToTimeSpan()).TotalMinutes))
                .First();

            return closest;
        }

        private void SaveConfig()
        {
			if (_configHandler != null)
			{
				_configHandler.SaveConfig(_activeWallpaper?.Name, _wallpaperList.Select(c => c.MetaFile).ToList());
			}
		}
    }
}
