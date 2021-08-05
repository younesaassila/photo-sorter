using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace PhotoSorterLibrary
{
	public class DirectoryUtility
	{
		public static List<string> GetFilesWithExtensions(string path, string[] extensions)
		{
			return Directory
				.GetFiles(path, "*.*", SearchOption.TopDirectoryOnly)
				.Where(file => extensions.Any(file.ToLower().EndsWith))
				.ToList();
		}

		public static List<string> GetImages(string path)
		{
			return GetFilesWithExtensions(path, new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".tif", ".tiff", ".raw" });
		}

		public static List<string> GetVideos(string path)
		{
			return GetFilesWithExtensions(path, new[] { ".mkv", ".flv", ".avi", ".mov", ".wmv", ".mp4", ".m4v", ".mpeg" });
		}

		public static string GetDestinationDirectoryPath(string sourceDirectoryPath, string year, string month)
		{
			Regex regex = new(@$"{year}-[\d,]*{month}[^\\/‒]*$");

			string path = Directory.GetDirectories(sourceDirectoryPath)
				.Where(dir => regex.IsMatch(dir))
				.OrderBy(dir => dir.Length)
				.ToArray()
				.FirstOrDefault();
			if (path == null)
				path = Directory.CreateDirectory(Path.Join(sourceDirectoryPath, $"{year}-{month}")).FullName;
			if (!path.EndsWith(Path.DirectorySeparatorChar))
				path = $"{path}{Path.DirectorySeparatorChar}";

			return path;
		}
	}
}
