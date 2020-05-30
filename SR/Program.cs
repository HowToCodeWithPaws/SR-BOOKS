using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Net.Http;
using System.IO;

namespace SR
{
	class Program
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="files"></param>
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

		/// <summary>
		/// 
		/// </summary>
		/// <param name="files"></param>
		/// <returns></returns>
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

		/// <summary>
		/// 
		/// </summary>
		/// <param name="address"></param>
		/// <returns></returns>
		static async Task SecondPart(string address)
		{
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine("Second part: execution started\n");
			Console.ForegroundColor = ConsoleColor.White;

			HttpClient client = new HttpClient();

			try
			{
				HttpResponseMessage response = await client.GetAsync(address);
				if (response.IsSuccessStatusCode)
				{
					using (StreamReader reader = 
						new StreamReader(await response.Content.ReadAsStreamAsync()))
					{
						using (StreamWriter writer = new StreamWriter("new_book_from_web.txt"))
						{
							Transliterator.Transliterate(reader, writer, "new_book_from_web");
						}
					}
				}
				else { throw new Exception("Http request failed(. Try again later."); }

				stopwatch.Stop();
				Console.ForegroundColor = ConsoleColor.Green;
				Console.WriteLine($"\nSecond part: execution finished in " +
					$"{stopwatch.Elapsed} ({stopwatch.Elapsed:ss\\.fff} sec)\n");
				Console.ForegroundColor = ConsoleColor.White;
			}
			catch (HttpRequestException)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("\nError in http request. Try again later.\n");
				Console.ForegroundColor = ConsoleColor.White;
			}
			catch (ArgumentNullException)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("\nError in http request: null argument. Try again later.\n");
				Console.ForegroundColor = ConsoleColor.White;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		static async Task Main()
		{
			string[] files = new string[] { "121-0", "1727-0", "4200-0", "58975-0",
				"pg972", "pg3207", "pg19942", "pg27827", "pg43936" };

			string fileFromWeb = "https://www.gutenberg.org/files/1342/1342-0.txt";

			// 
			do
			{
				try
				{
					Synchronous(files);

					await Asynchronous(files);

					await SecondPart(fileFromWeb);
				}
				catch (Exception e)
				{
					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine("Something went terribly wrong:\n" + e.Message);
					Console.ForegroundColor = ConsoleColor.White;
				}

				Console.WriteLine("\nPress Enter to execute again, press (F) anything else to exit");

			} while (Console.ReadKey().Key == ConsoleKey.Enter);
		}
	}
}
