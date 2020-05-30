using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace SR
{
	/// <summary>
	/// Статический класс для преобразования книг по правилам, описанным
	/// в условии. Здесь есть словарь, метод, создающий потоки,
	/// и метод, преобразующий файл, полученный из потока.
	/// Альтернативно вместо использования файловых потоков можно
	/// было использовать обычный File.ReadAllText, мне показалось,
	/// потоки должны быть эффективнее по времени, но моя программа 
	/// работает медленнее, чем у коллег, правда является ли причиной
	/// этого потоковая реализация или медленный ноутбук ¯\_(ツ)_/¯. 
	/// </summary>
	public static class Transliterator
	{
		/// <summary>
		/// Словарь с правилами перевода латинских букв в кириллицу.
		/// Было выбрано переводить Y в Й а не в Ы, потому что.
		/// Также здесь учтены прописные буквы, хотя можно было делать
		/// проверки на содержание symbol.ToUpper, находить соответствие
		/// и приводить в lowercase.
		/// </summary>
		static Dictionary<char, string> letterRules = new Dictionary<char, string>
		{
			['A'] = "А",
			['B'] = "Б",
			['C'] = "Ц",
			['D'] = "Д",
			['E'] = "Е",
			['F'] = "Ф",
			['G'] = "Г",
			['H'] = "Х",
			['I'] = "И",
			['J'] = "Ж",
			['K'] = "К",
			['L'] = "Л",
			['M'] = "М",
			['N'] = "Н",
			['O'] = "О",
			['P'] = "П",
			['Q'] = "КУ",
			['R'] = "Р",
			['S'] = "С",
			['T'] = "Т",
			['U'] = "У",
			['V'] = "В",
			['W'] = "У",
			['X'] = "КС",
			['Y'] = "Й",
			['Z'] = "З",
			['a'] = "а",
			['b'] = "б",
			['c'] = "ц",
			['d'] = "д",
			['e'] = "е",
			['f'] = "ф",
			['g'] = "г",
			['h'] = "х",
			['i'] = "и",
			['j'] = "ж",
			['k'] = "к",
			['l'] = "л",
			['m'] = "м",
			['n'] = "н",
			['o'] = "о",
			['p'] = "п",
			['q'] = "ку",
			['r'] = "р",
			['s'] = "с",
			['t'] = "т",
			['u'] = "у",
			['v'] = "в",
			['w'] = "у",
			['x'] = "кс",
			['y'] = "й",
			['z'] = "з",
		};

		/// <summary>
		/// Метод, реализующий запись нового файла, полученного
		/// транслитерацией исходного, также здесь засекается время
		/// и выводится сообщение о количестве символов и времени 
		/// обработки файла. Принимает потоки, в которых
		/// считывается старый и записывается новый файл, а также
		/// имя файла, чтобы вывести сообшение.
		/// </summary>
		/// <param name="reader"> Поток для считывания начального файла. </param>
		/// <param name="writer"> Поток для записи измененного файла. </param>
		/// <param name="filename"> Имя файла. </param>
		public static void Transliterate(StreamReader reader,
			StreamWriter writer, string filename)
		{
			/// Переменные для флага состояния считывания,
			/// количества символов до и после преобразования.
			bool read = true;
			int symBefore = 0;
			int symAfter = 0;

			/// Таймер и его запуск для измерения времени выполнения.
			/// Стоит заметить, что наверное более правильно было
			/// бы для измерения времени отдельно считывать файл,
			/// осуществлять преобразование и записывать новый, и 
			/// измерять каждый промежуток отдельно, но здесь эти
			/// этапы очень тесно связаны, так что таймер замеряет сразу все.
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();

			/// Считывание и запись осуществляются по одному символу
			/// до тех пор, пока файл не закончится.
			while (read)
			{
				int next = reader.Read();

				if (next != -1)
				{
					symBefore++;
					char sym = (char)next;

					/// Если словарь содержит символ, то символ заменяется, иначе,
					/// если это не буквенный символ, он записыавется без изменений,
					/// иначе пропускается.
					if (letterRules.ContainsKey(sym) && 
						letterRules.TryGetValue(sym, out string newSym))
					{
						writer.Write(newSym);
						symAfter += newSym.Length;
					}
					else
					{
						if (!char.IsLetter(sym))
						{
							writer.Write(sym);
							symAfter++;
						}
					}
				}
				else
				{
					/// Когда файл кончается, чтение завершается,
					/// таймер останавливается и выводится сообщение
					/// по обработке книги.
					read = false;
					stopwatch.Stop();

					Console.WriteLine("File " + filename + $".txt:          " +
						$"{symBefore} --> {symAfter}          " +
						$"in {stopwatch.Elapsed} ({stopwatch.Elapsed:ss\\.fff} sec)");
				}
			}
		}

		/// <summary>
		/// Метод для создания потоков, вызова метода транслитерации и 
		/// поимки файловых исключений.
		/// </summary>
		/// <param name="filename"></param>
		public static void ChangeFile(string filename)
		{
			try
			{
				using (var reader = new StreamReader(filename + ".txt"))
				{
					using (var writer = new StreamWriter("new_" + filename + ".txt"))
					{
						/// Вызов метода транслитерации по созданным потокам.
						Transliterate(reader, writer, filename);
					}
				}
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
	}
}
