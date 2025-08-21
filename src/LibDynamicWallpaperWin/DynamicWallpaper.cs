using System.Text.Json.Serialization;

namespace LibDynamicWallpaperWin
{
	internal class DynamicWallpaperImage
	{
		[JsonPropertyName("file_name")]
		public required string FileName { get; set; }

		[JsonPropertyName("time")]
		public required TimeOnly Time { get; set; }
	}

	internal class DynamicWallpaper
	{
		[JsonPropertyName("name")]
		public required string Name { get; set; }
		[JsonPropertyName("description")]
		public required string Description { get; set; }

		[JsonPropertyName("images")]
		public required List<DynamicWallpaperImage> Images { get; set; }

		public string MetaFile { get; set; } = string.Empty;
	}
}
