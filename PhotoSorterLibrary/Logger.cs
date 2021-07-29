using System;
using System.Collections.Generic;
using System.IO;

namespace PhotoSorterLibrary
{
	public class Logger
	{
		static List<string> logs = new();

		public static void Write(string line)
		{
			logs.Add($"[{DateTime.Now}] {line}");
			Console.WriteLine(line);
		}

		public static void Clear()
		{
			logs = new List<string>();
		}

		public static async void SaveLogFile(string directoryPath)
		{
			if (!Directory.Exists(directoryPath))
				throw new Exception($"'{directoryPath}' is not a directory.");

			string logsPath = Path.Join(directoryPath, "photo_sorter.log");
			if (logs.Count > 0)
			{
				await File.WriteAllLinesAsync(logsPath, logs);
			}
			else if (File.Exists(logsPath))
			{
				File.Delete(logsPath);
			}
		}
	}
}
