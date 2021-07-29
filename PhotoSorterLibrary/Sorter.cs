using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace PhotoSorterLibrary
{
	public class Sorter
	{
		public static void SortDirectory(string path, bool sortVideos)
		{
			Logger.Clear();
			Logger.Write($"Sorting directory '{path}'... (images {(sortVideos ? "and videos" : "only")})");
			SortImages(path);
			if (sortVideos) SortVideos(path);
			Logger.SaveLogFile(path);
		}

		static List<string> GetFilesWithExtensions(string path, string[] extensions)
		{
			return Directory
				.GetFiles(path, "*.*", SearchOption.TopDirectoryOnly)
				.Where(file => extensions.Any(file.ToLower().EndsWith))
				.ToList();
		}

		static List<string> GetImages(string path)
		{
			return GetFilesWithExtensions(path, new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".tif", ".tiff", ".raw" });
		}

		static List<string> GetVideos(string path)
		{
			return GetFilesWithExtensions(path, new[] { ".mkv", ".flv", ".avi", ".mov", ".wmv", ".mp4", ".m4v", ".mpeg" });
		}

		static void SortImages(string path)
		{
			List<string> images = GetImages(path);
			foreach (string image in images)
			{
				DateTime? dateTaken = DateTimeUtility.GetDateTakenFromImage(image);
				if (dateTaken != null)
				{
					MoveFile(image, dateTaken.Value);
				}
				else
				{
					FileInfo fileInfo = new(image);
					Logger.Write($"Couldn't find date taken for '{fileInfo.Name}'.");
				}
			}
		}

		static void SortVideos(string path)
		{
			List<string> videos = GetVideos(path);
			foreach (string video in videos)
			{
				FileInfo fileInfo = new(video);
				DateTime? dateEncoded = DateTimeUtility.GetDateEncodedFromVideo(video);
				if (dateEncoded != null)
				{
					DateTime dateCreated = File.GetCreationTime(video);
					if (dateEncoded - dateCreated > new TimeSpan(0, 1, 0))
					{
						Logger.Write($"Date encoded for '{fileInfo.Name}' is likely incorrect.");
						return;
					}
					MoveFile(video, dateEncoded.Value);
				}
				else
				{
					Logger.Write($"Couldn't find date encoded for '{fileInfo.Name}'.");
				}
			}
		}

		static void MoveFile(string filePath, DateTime date)
		{
			FileInfo fileInfo = new(filePath);
			string year = date.Year.ToString();
			string month = date.Month.ToString("00");

			string sourceDirPath = Path.GetDirectoryName(filePath);

			Regex destDirRegex = new(@$"{year}-[\d,]*{month}[^\\/‒]*$");

			string destDirPath = Directory.GetDirectories(sourceDirPath)
				.Where(dir => destDirRegex.IsMatch(dir))
				.OrderBy(dir => dir.Length)
				.ToArray()
				.FirstOrDefault();
			if (destDirPath == null)
				destDirPath = Directory.CreateDirectory(Path.Join(sourceDirPath, $"{year}-{month}")).FullName;
			if (!destDirPath.EndsWith(Path.DirectorySeparatorChar))
				destDirPath = $"{destDirPath}{Path.DirectorySeparatorChar}";

			bool fileAlreadyExists = Directory.GetFiles(destDirPath, $"{Path.GetFileNameWithoutExtension(filePath)}.*").Length > 0;

			if (!fileAlreadyExists)
			{
				File.Move(filePath, Path.Join(destDirPath, fileInfo.Name));
			}
			else
			{
				string directoryName = Path.GetFileName(Path.GetDirectoryName(destDirPath));
				Logger.Write($"'{fileInfo.Name}' already exists in directory '{directoryName}'.");
			}
		}
	}
}
