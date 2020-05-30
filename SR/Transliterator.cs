﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace SR
{
	/// <summary>
	/// 
	/// </summary>
	public static class Transliterator
	{
		/// <summary>
		/// 
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
		/// 
		/// </summary>
		/// <param name="reader"></param>
		/// <param name="writer"></param>
		/// <param name="filename"></param>
		public static void Transliterate(StreamReader reader, StreamWriter writer, string filename) {
			bool read = true;
			int symBefore = 0;
			int symAfter = 0;
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();

			while (read)
			{
				int next = reader.Read();

				if (next != -1)
				{
					symBefore++;
					char sym = (char)next;

					if (letterRules.ContainsKey(sym) && letterRules.TryGetValue(sym, out string newSym))
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
					read = false;
					stopwatch.Stop();

					Console.WriteLine("File " + filename + $".txt:          " +
						$"{symBefore} --> {symAfter}          " +
						$"in {stopwatch.Elapsed} ({stopwatch.Elapsed:ss\\.fff} sec)");
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="filename"></param>
		public static void ChangeFile(string filename)
		{
			Console.ForegroundColor = ConsoleColor.White;

			try
			{
				using (var reader = new StreamReader(filename + ".txt"))
				{
					using (var writer = new StreamWriter("new_" + filename + ".txt"))
					{
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
