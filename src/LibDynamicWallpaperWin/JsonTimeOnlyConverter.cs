using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LibDynamicWallpaperWin
{
	internal class JsonTimeOnlyConverter : JsonConverter<TimeOnly>
	{
		public JsonTimeOnlyConverter()
		{
		}

		public override TimeOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			var value = reader.GetString();
			if (TimeOnly.TryParseExact(value, "HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out var time))
			{
				return time;
			}
			throw new JsonException($"Unable to read time {value}");
		}

		public override void Write(Utf8JsonWriter writer, TimeOnly value, JsonSerializerOptions options)
		{
			throw new NotImplementedException();
		}
	}
}
