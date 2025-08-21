using System.Text.Json.Serialization;

namespace LibDynamicWallpaperWin
{
	public class MetaConfig
	{
		[JsonPropertyName("current_active_wallpaper")]
		public required string? CurrentActiveWallpaper { get; set; }

		[JsonPropertyName("wallpaper_meta_files")] 
		public required List<string> MetaFiles { get; set; }
	}
}
