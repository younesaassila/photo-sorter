﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace PhotoSorterLibrary
{
	public class Sorter
	{
		public static void SortDirectory(string path)
		{
			Logs.Clear();
			Logs.Write($"Sorting directory '{path}'...");
			SortImages(path);
			SortVideos(path);
			Logs.SaveFile(path);
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
					SortFile(image, (DateTime)dateTaken);
				}
				else
				{
					FileInfo fileInfo = new(image);
					Logs.Write($"Couldn't find date taken for '{fileInfo.Name}'.");
				}
			}
		}

		static void SortVideos(string path)
		{
			List<string> videos = GetVideos(path);
			foreach (string video in videos)
			{
				DateTime? dateEncoded = DateTimeUtility.GetDateEncodedFromVideo(video);
				if (dateEncoded != null)
				{
					SortFile(video, (DateTime)dateEncoded);
				}
				else
				{
					FileInfo fileInfo = new(video);
					Logs.Write($"Couldn't find date encoded for '{fileInfo.Name}'.");
				}
			}
		}

		static void SortFile(string filePath, DateTime date)
		{
			FileInfo fileInfo = new(filePath);
			string sourcePath = Path.GetDirectoryName(filePath);
			string year = date.Year.ToString();
			string month = date.Month.ToString("00");

			Regex dirRegex = new(@$"{year}-[\d,]*{month}[^\\/]*$");

			string dirPath = Directory.GetDirectories(sourcePath)
				.Where(dir => dirRegex.IsMatch(dir))
				.OrderBy(dir => dir.Length)
				.ToArray()
				.FirstOrDefault();
			if (dirPath == null)
				dirPath = Directory.CreateDirectory(Path.Join(sourcePath, $"{year}-{month}")).FullName;

			try
			{
				File.Move(filePath, Path.Join(dirPath, fileInfo.Name));
			}
			catch
			{
				string directoryName = Path.GetFileName(Path.GetDirectoryName(dirPath));
				Logs.Write($"'{fileInfo.Name}' already exists in directory '{directoryName}'.");
			}
		}
	}
}