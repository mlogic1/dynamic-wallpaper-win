using LibDynamicWallpaperWin;
using LibDynamicWallpaperWin.Utility;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DynamicWallpaperWin
{
	internal class DynamicWallpaperApp : ApplicationContext
	{
		NotifyIcon _trayIcon = new NotifyIcon();
		private readonly ILogger _logger;
		private readonly DynWallpaperService _wallpaperService;
		
		internal DynamicWallpaperApp()
		{
			ServiceProvider serviceProvider = InitServices();
			_logger = serviceProvider.GetRequiredService<ILogger<DynamicWallpaperApp>>();
			_wallpaperService = serviceProvider.GetRequiredService<DynWallpaperService>();
			InitTrayIcon();
		}

		private ServiceProvider InitServices()
		{
			var serviceCollection = new ServiceCollection();
			serviceCollection.AddLogging( config =>
			{
				config.SetMinimumLevel(LogLevel.Information);
				config.AddProvider(new LoggerFileProviderLocalAppData());
			});

			serviceCollection.AddSingleton<DynWallpaperService>();
			serviceCollection.AddSingleton<ConfigHandlerService>();
			var serviceProvider = serviceCollection.BuildServiceProvider();
			return serviceProvider;
		}

		private void InitTrayIcon()
		{
			ContextMenuStrip notifyIconOptions = new ContextMenuStrip();

			_trayIcon.Icon = Properties.Resources.app_icon;
			_trayIcon.Visible = true;
			_trayIcon.Text = "Dynamic Wallpaper";
			_trayIcon.ContextMenuStrip = notifyIconOptions;
			_trayIcon.ContextMenuStrip.Opening += RefreshNotifyTrayContextItems;

			Application.ApplicationExit += (sender, e) =>   // properly hide icon when application exits so it doesnt end up stuck in system tray
			{
				_trayIcon.Visible = false;
				_trayIcon.Dispose();
			};
		}

		private void RefreshNotifyTrayContextItems(object? sender, System.ComponentModel.CancelEventArgs e)
		{
			ContextMenuStrip? notifyIconOptions = _trayIcon.ContextMenuStrip;

			if (notifyIconOptions == null)
			{
				return;
			}

			e.Cancel = false;
			notifyIconOptions.Items.Clear();

			List<ToolStripItem> items = new List<ToolStripItem>();
			ToolStripItem itemExit = new ToolStripMenuItem("Exit");
			ToolStripItem itemAbout = new ToolStripMenuItem("About");

			ToolStripItem itemAdd = new ToolStripMenuItem("Add new wallpaper...");
			ToolStripMenuItem itemMenuRemoveWallpaper = new ToolStripMenuItem("Remove wallpaper");
			ToolStripMenuItem itemMenuSetActiveWallpaper = new ToolStripMenuItem("Set active wallpaper");

			List<ToolStripMenuItem> activateItems = new List<ToolStripMenuItem>();
			List<ToolStripMenuItem> removeItems = new List<ToolStripMenuItem>();

			string? activeWallpaper = _wallpaperService.GetActiveWallpaper();

			foreach (string wallpaper in _wallpaperService.GetWallpapers())
			{
				var wallpaperItem = new ToolStripMenuItem(wallpaper, null, MenuClickActivateWallpaper);

				if (activeWallpaper != null || activeWallpaper == wallpaper)
				{
					wallpaperItem.Checked = true;
				}
				activateItems.Add(wallpaperItem);
				removeItems.Add(new ToolStripMenuItem(wallpaper, null, MenuClickRemoveWallpaper));
			}

			itemMenuSetActiveWallpaper.DropDownItems.AddRange(activateItems.ToArray());
			itemMenuRemoveWallpaper.DropDownItems.AddRange(removeItems.ToArray());

			// Events
			itemExit.Click += MenuClickExit;
			itemAbout.Click += MenuClickAbout;
			itemAdd.Click += MenuClickAddWallpaper;

			items.Add(itemMenuSetActiveWallpaper);
			items.Add(itemAdd);
			items.Add(itemMenuRemoveWallpaper);
			items.Add(itemAbout);
			items.Add(itemExit);

			notifyIconOptions.Items.AddRange(items.ToArray());
		}

		private void MenuClickAddWallpaper(object? sender, EventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog()
			{
				Title = "Select a wallpaper meta file",
				Filter = "Json Files (*.json)|*.json",
				Multiselect = false
			};

			DialogResult dialogResult = openFileDialog.ShowDialog();

			if (dialogResult != DialogResult.OK)
			{
				return;
			}

			string selectedFile = openFileDialog.FileName;
			try
			{
				_wallpaperService.AddWallpaper(selectedFile);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Unable to add wallpaper", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void MenuClickActivateWallpaper(object? sender, EventArgs e)
		{
			var menuItem = sender as ToolStripMenuItem;
			if (menuItem == null) { return; }

			string? wallpaper = menuItem.Text;

			if (wallpaper != null)
			{
				_wallpaperService.SetActiveWallpaper(wallpaper);
			}
		}

		private void MenuClickRemoveWallpaper(object? sender, EventArgs e)
		{
			var menuItem = sender as ToolStripMenuItem;
			if (menuItem == null) { return; }

			string? wallpaper = menuItem.Text;

			if (wallpaper != null)
			{
				_wallpaperService.RemoveWallpaper(wallpaper);
			}
		}

		private void MenuClickAbout(object? sender, EventArgs e)
		{
			using (Form f = new AboutForm())
			{
				f.ShowDialog();
			}
		}

		private void MenuClickExit(object? sender, EventArgs e)
		{
			Application.Exit();
		}
	}
}
