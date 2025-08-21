using System.IO;
using System.Runtime.InteropServices;

namespace LibDynamicWallpaperWin
{
	static internal class WinAPIFunctions
	{
		[DllImport("user32.dll", SetLastError = true)]
		public static extern bool SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

		private const int SPI_SETDESKWALLPAPER = 20;
		private const int SPIF_UPDATEINIFILE = 0x01;
		private const int SPIF_SENDWININICHANGE = 0x02;

		static internal bool UpdateSystemWallpaper(string pathToImage)
		{
			bool result = SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, pathToImage, SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
			return result;
		}
	}
}
