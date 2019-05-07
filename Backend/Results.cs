using System;
using System.Collections.Generic;
using System.IO;

namespace Backend
{
	public enum SourceType
	{
		Binary,
		Code
	}

	public class Results
	{
		public SourceType Type { get; }
		public int TimeTaken { get; private set; }
		public int NumFunctions { get; private set; }
		public List<Function> Functions { get; set; } = new List<Function>();

		public Results(string directory, SourceType type)
		{
			Type = type;

			ReadProbs(directory);
			ReadHexCode(directory);
		}

		void ReadProbs(string directory)
		{
			NumFunctions = 0;
			var probs = new StreamReader(directory + "/" + FileNames.ProbName);

			using (probs)
			{
				try
				{
					TimeTaken = int.Parse(probs.ReadLine());
				}
				catch (ArgumentNullException)
				{
					TimeTaken = -1;
				}

				while (!probs.EndOfStream)
				{
					Function function = new Function
					{
						Prob = double.Parse(probs.ReadLine())
					};

					Functions.Add(function);

					NumFunctions += 1;
				}
			}
		}

		void ReadHexCode(string directory)
		{
			StreamReader functions = new StreamReader(directory + "/" + FileNames.FuncName);

			using (functions)
			{
				for (int i = 0; !functions.EndOfStream; ++i)
				{
					foreach (string value in functions.ReadLine().Split(","))
					{
						Functions[i].HexCode.Add(value);
					}
				}
			}
		}
	}
}
