using Microsoft.WindowsAPICodePack.Shell;
using System;

namespace PhotoSorterLibrary
{
	public class DateTimeUtility
	{
		public static DateTime? GetDateTakenFromImage(string path)
		{
			ShellObject shell = ShellObject.FromParsingName(path);
			return shell.Properties.System.Photo.DateTaken.Value;
		}

		public static DateTime? GetDateEncodedFromVideo(string path)
		{
			ShellObject shell = ShellObject.FromParsingName(path);
			return shell.Properties.System.Media.DateEncoded.Value;
		}
	}
}
