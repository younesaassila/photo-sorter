using System;
using System.IO;
using System.Text;

namespace PhotoSorterLibrary
{
	public class Logger
	{
		static StringBuilder logs = new();

		public static void WriteLine(string line)
		{
			logs.Append($"[{DateTime.Now}] {line}").Append(Environment.NewLine);
			Console.WriteLine(line);
		}

		public static void Clear()
		{
			logs.Clear();
		}

		public static void SaveLog(string directoryPath)
		{
			if (!Directory.Exists(directoryPath))
				throw new Exception($"'{directoryPath}' is not a directory.");

			string logsPath = Path.Join(directoryPath, "photo_sorter.log");
			if (logs.Length > 0)
			{
				using StreamWriter file = new(logsPath);
				file.Write(logs.ToString());
				file.Close();
				file.Dispose();
			}
			else if (File.Exists(logsPath))
			{
				File.Delete(logsPath);
			}
		}
	}
}
