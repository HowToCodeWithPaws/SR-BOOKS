using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Net.Http;
using System.IO;

/// Ср 4.2 (?)
/// Зубарева Наталия 
/// БПИ199.2
/// 30.05.2020

namespace SR
{
	/// <summary>
	/// Консольное приложение, резделенное на методы по задачам
	/// в условии.
	/// </summary>
	class Program
	{
		/// <summary>
		/// Метод для выполнения первой части первого задания,
		/// то есть синхронного изменения загруженных книг.
		/// Происходит изменение каждого файла из массива
		/// имен файлов в цикле.
		/// </summary>
		/// <param name="files"> Параметр массива
		/// имен обрабатываемых файлов. </param>
		static void Synchronous(string[] files)
		{
			/// Таймер и его запуск.
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();

			/// Настройка консоли и вывод сообщения о начале 
			/// выполнения.
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine("First part: synchronous execution started\n");
			Console.ForegroundColor = ConsoleColor.White;

			/// В цикле для каждого имени файла в массиве вызываем
			/// метод изменения этого файла.
			foreach (string file in files)
			{
				Transliterator.ChangeFile(file);
			}

			/// Остановка таймера, настройка внешнего вида консоли
			/// и вывод сообщения о выполнении части задания, вывод
			/// общего времени выполнения.
			stopwatch.Stop();

			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine($"\nFirst part: synchronous execution finished in " +
				$"{stopwatch.Elapsed} ({stopwatch.Elapsed:ss\\.fff} sec)\n");
			Console.ForegroundColor = ConsoleColor.White;
		}

		/// <summary>
		/// Асинхронный метод для обработки книг.
		/// То же самое, что в первом пункте, но немного
		/// быстрее. Что удивительно, не намного быстрее,
		/// но, как и с потоками (один из комментариев класса
		/// Transliterator (наболело)), не понятно, кто в этом
		/// виноват: реализация или технические средства.
		/// </summary>
		/// <param name="files"> Массив имен обрабатываемых
		/// файлов. </param>
		/// <returns></returns>
		static async Task Asynchronous(string[] files)
		{
			/// Таймер и его запуск.
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();

			/// Настройка консоли и вывод сообщения о начале 
			/// выполнения.
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine("First part: asynchronous execution started\n");
			Console.ForegroundColor = ConsoleColor.White;

			/// Запуск асинхронного выполнения изменения для
			/// каждого файла из масиива. Метод ожидает завершения
			/// всего процесса.
			await Task.WhenAll(files.Select(file => 
			Task.Run(() => Transliterator.ChangeFile(file))));

			/// Остановка таймера, настройка внешнего вида консоли
			/// и вывод сообщения о выполнении части задания, вывод
			/// общего времени выполнения. Из-за асинхронности тут 
			/// происходит какая-то магия, временные петли, небезызвестный
			/// парадокс близнецов и прочее. Просто смиримся, что время
			/// в асинхронности работает не так, как мы привыкли.
			stopwatch.Stop();

			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine($"\nFirst part: asynchronous execution finished in " +
				$"{stopwatch.Elapsed} ({stopwatch.Elapsed:ss\\.fff} sec)\n");
			Console.ForegroundColor = ConsoleColor.White;
		}

		/// <summary>
		/// Метод для выполнения второй части задания, то есть
		/// обработки книги, получаемой по веб-запросу.
		/// Мне не хотелось сохранять "Гордость и предубеждение",
		/// потому что это тяжело, поэтому происходит некоторая
		/// гимнастика с потоками.
		/// </summary>
		/// <param name="address"> Параметр адреса,
		/// по которому получается файл. </param>
		/// <returns></returns>
		static async Task SecondPart(string address)
		{
			/// Таймер и его запуск. Здесь он считает
			/// в том числе время веб-запроса, поэтому
			/// время отличается от времени, выводимого
			/// конкретно про обработку книги.
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();

			/// Настройка консоли и вывод сообщения о начале 
			/// выполнения.
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine("Second part: execution started\n");
			Console.ForegroundColor = ConsoleColor.White;

			HttpClient client = new HttpClient();

			try
			{
				/// Производится web запрос, что это, кто это, почему,
				/// откуда, ладно, самообразование, че уж там.
				HttpResponseMessage response = await client.GetAsync(address);

				if (response.IsSuccessStatusCode)
				{
					using (StreamReader reader = 
						new StreamReader(await response.Content.ReadAsStreamAsync()))
					{
						using (StreamWriter writer = new StreamWriter("new_book_from_web.txt"))
						{
							/// Вызов метода транслитерации по созданным потокам.
							Transliterator.Transliterate(reader, writer, "new_book_from_web");
						}
					}
				}
				else { throw new Exception("Http request failed(. Try again later."); }

				/// На всякий случай закроем руками...
				response.Dispose();

				/// Остановка таймера, настройка внешнего вида консоли
				/// и вывод сообщения о выполнении части задания, вывод
				/// общего времени выполнения.
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
			catch (FileNotFoundException)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("File not found(. Try again after fixing.");
				Console.ForegroundColor = ConsoleColor.White;
			}
			catch (IOException)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("Something is wrong with the file." +
					" Try again after fixing.");
				Console.ForegroundColor = ConsoleColor.White;
			}
			catch (UnauthorizedAccessException)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("The access is anauthorized(. Try again after fixing.");
				Console.ForegroundColor = ConsoleColor.White;
			}
			catch (System.Security.SecurityException)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("Security error(. Try again after fixing.");
				Console.ForegroundColor = ConsoleColor.White;
			}
			catch (Exception ex)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("Something went terribly wrong:\n " + ex.Message);
				Console.ForegroundColor = ConsoleColor.White;
			}
		}

		/// <summary>
		/// Точка входа, метод, последовательно запускающий методы для
		/// решения всех частей задания. Здесь также осуществляется цикл
		/// повтора решения.
		/// </summary>
		/// <returns></returns>
		static async Task Main()
		{
			/// Строковые переменные для хранения названий и адресов книг.
			string[] files = new string[] { "121-0", "1727-0", "4200-0", "58975-0",
				"pg972", "pg3207", "pg19942", "pg27827", "pg43936" };

			string fileFromWeb = "https://www.gutenberg.org/files/1342/1342-0.txt";

			/// Цикл повтора решения.
			do
			{
				try
				{
					/// Вызов метода синхронной обработки книг.
					Synchronous(files);

					/// Вызов метода асинхронной обработки книг.
					await Asynchronous(files);

					/// Вызов метода получения книги по web запросу.
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
