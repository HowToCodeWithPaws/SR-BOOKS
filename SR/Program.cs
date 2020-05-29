using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace SR
{
	class Program
	{
		static void Synchronous(string[] files)
		{
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine("First part: synchronous execution started\n");
			Console.ForegroundColor = ConsoleColor.White;

			foreach (string file in files)
			{
				Transliterator.ChangeFile(file); 
			}

			stopwatch.Stop();
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine($"\nFirst part: synchronous execution finished in " +
				$"{stopwatch.Elapsed} ({stopwatch.Elapsed:ss\\.fff} sec)\n");
			Console.ForegroundColor = ConsoleColor.White;
		}

		static async Task Asynchronous(string[] files)
		{
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine("First part: asynchronous execution started\n");
			Console.ForegroundColor = ConsoleColor.White;

			await Task.WhenAll(files.Select(file => Task.Run(() => Transliterator.ChangeFile(file))));

			stopwatch.Stop();
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine($"\nFirst part: asynchronous execution finished in " +
				$"{stopwatch.Elapsed} ({stopwatch.Elapsed:ss\\.fff} sec)\n");
			Console.ForegroundColor = ConsoleColor.White;
		}

		static void SecondPart() { }

		static async Task Main()
		{
			string[] files = new string[] { "121-0", "1727-0", "4200-0", "58975-0",
				"pg972", "pg3207", "pg19942", "pg27827", "pg43936" };

			do
			{
				Synchronous(files);

				await Asynchronous(files);

				SecondPart();

				Console.WriteLine("\nДля повторного решения нажмите Enter, для выхода нажмите любой другой символ");

			} while (Console.ReadKey().Key == ConsoleKey.Enter);
		}
	}
}
