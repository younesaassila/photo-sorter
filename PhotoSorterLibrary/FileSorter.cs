using System;
using System.Collections.Generic;
using System.IO;

namespace PhotoSorterLibrary
{
	public class FileSorter
	{
		public static void SortDirectory(string path, bool sortVideos)
		{
			Logger.Clear();
			Logger.WriteLine($"Sorting directory '{path}'... (images {(sortVideos ? "and videos" : "only")})");
			SortImages(path);
			if (sortVideos) SortVideos(path);
			Logger.SaveLog(path);
		}

		static void SortImages(string path)
		{
			List<string> images = DirectoryUtility.GetImages(path);
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
					Logger.WriteLine($"Couldn't find date taken for '{fileInfo.Name}'.");
					return;
				}
			}
		}

		static void SortVideos(string path)
		{
			List<string> videos = DirectoryUtility.GetVideos(path);
			foreach (string video in videos)
			{
				FileInfo fileInfo = new(video);
				DateTime? dateEncoded = DateTimeUtility.GetDateEncodedFromVideo(video);
				if (dateEncoded != null)
				{
					DateTime dateCreated = File.GetCreationTime(video);
					TimeSpan? timeSpan = dateEncoded - dateCreated;
					if (timeSpan > new TimeSpan(0, -1, 0) && timeSpan < new TimeSpan(0, 1, 0))
					{
						MoveFile(video, dateEncoded.Value);
					}
					else
					{
						Logger.WriteLine($"Date encoded for '{fileInfo.Name}' is likely incorrect.");
						return;
					}
				}
				else
				{
					Logger.WriteLine($"Couldn't find date encoded for '{fileInfo.Name}'.");
					return;
				}
			}
		}

		static void MoveFile(string filePath, DateTime date)
		{
			FileInfo fileInfo = new(filePath);
			string year = date.Year.ToString();
			string month = date.Month.ToString("00");

			string srcDirPath = Path.GetDirectoryName(filePath);
			string destDirPath = DirectoryUtility.GetDestinationDirectoryPath(srcDirPath, year, month);

			bool fileAlreadyExists =
				Directory.GetFiles(destDirPath, $"{Path.GetFileNameWithoutExtension(filePath)}.*").Length > 0;

			if (!fileAlreadyExists)
			{
				File.Move(filePath, Path.Join(destDirPath, fileInfo.Name));
			}
			else
			{
				string directoryName = Path.GetFileName(Path.GetDirectoryName(destDirPath));
				Logger.WriteLine($"'{fileInfo.Name}' already exists in directory '{directoryName}'.");
				return;
			}
		}
	}
}
