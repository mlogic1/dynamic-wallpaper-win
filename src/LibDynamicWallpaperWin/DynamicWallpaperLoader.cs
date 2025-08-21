using System.Text.Json;

namespace LibDynamicWallpaperWin
{
	internal static class DynamicWallpaperLoader
	{
		private static JsonSerializerOptions _jsonOptions;

		static DynamicWallpaperLoader()
		{
			_jsonOptions = new JsonSerializerOptions(JsonSerializerDefaults.General);
			_jsonOptions.Converters.Add(new JsonTimeOnlyConverter());
		}

		internal static DynamicWallpaper LoadDynamicWallaper(string jsonMetaFile)
		{
			if (!File.Exists(jsonMetaFile))
			{
				throw new FileNotFoundException($"File {jsonMetaFile} does not exist.");
			}
			string metaFileDirectory = Path.GetDirectoryName(jsonMetaFile) ?? throw new Exception($"No directory found in path: {jsonMetaFile}");
			string metaFileContent = "";
			try
			{
				metaFileContent = File.ReadAllText(jsonMetaFile);
			}
			catch (Exception ex) 
			{
				throw new Exception($"Unable to read meta file {jsonMetaFile}. {ex.Message.ToString()}");
			}
			
			try
			{
				DynamicWallpaper? dto = JsonSerializer.Deserialize<DynamicWallpaper>(metaFileContent, _jsonOptions);
				
				if (dto == null)
				{
					throw new Exception($"Unable to deserialize meta file {jsonMetaFile}.");
				}

				if (dto.Images.Count == 0)
				{
					throw new Exception($"There are no images specified in the meta file {jsonMetaFile}");
				}

				foreach(DynamicWallpaperImage imageDTO in dto.Images)
				{
					string fullImagePath = Path.Combine(metaFileDirectory, imageDTO.FileName);
					if (!File.Exists(fullImagePath))
					{
						throw new FileNotFoundException($"Image file {imageDTO.FileName} does not exist");
					}

					imageDTO.FileName = fullImagePath;
				}

				// TODO: validate that all times are unique, there should not be 2 wallpapers at the same time

				return dto;
			}
			catch(Exception ex)
			{
				throw new Exception(ex.Message);
			}
		}
	}
}
