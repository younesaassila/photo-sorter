﻿using PhotoSorterLibrary;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace PhotoSorter
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.OutputEncoding = System.Text.Encoding.UTF8;
			PrintProductInfo();
			Console.WriteLine();

			string path = args.Length > 0 ? args[0] : "";

			while (!Directory.Exists(path))
			{
				if (!string.IsNullOrWhiteSpace(path))
				{
					Console.WriteLine($"'{path}' is not a directory.");
				}
				Console.Write("Enter the path of the directory to sort: ");
				path = Console.ReadLine().Trim();
			}

			Sorter.SortDirectory(path);
		}

		static void PrintProductInfo()
		{
			FileVersionInfo assemblyVersionInfo = GetAssemblyVersionInfo();
			string name = assemblyVersionInfo.ProductName;
			string version = assemblyVersionInfo.ProductVersion;
			string copyright = assemblyVersionInfo.LegalCopyright;
			Console.WriteLine($"{name} [Version {version}]");
			Console.WriteLine(copyright);
		}

		static FileVersionInfo GetAssemblyVersionInfo()
		{
			Assembly assembly = Assembly.GetExecutingAssembly();
			return FileVersionInfo.GetVersionInfo(assembly.Location);
		}
	}
}
