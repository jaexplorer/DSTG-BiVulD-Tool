using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;

namespace Backend
{
	public enum SourceType
	{
		Binary,
		Code
	}

	[DataContract]
	public class Results
	{
		public Results()
		{
			//
		}

		public Results(string directory, SourceType type)
		{
			Type = type;

			NumFunctions = 0;
			var probs = new StreamReader(directory + FileNames.ProbName);

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

			using (var hex = new StreamReader(directory + FileNames.HexName))
			{
				for (int i = 0; !hex.EndOfStream; ++i)
				{
					foreach (string value in hex.ReadLine().Split(","))
					{
						Functions[i].HexCode.Add(value);
					}
				}
			}

			using (var asm = new StreamReader(directory + FileNames.AsmName))
			{
				for (int i = 0; !asm.EndOfStream; ++i)
				{
					foreach (string value in asm.ReadLine().Split(","))
					{
						Functions[i].AsmCode.Add(value);
					}
				}
			}
		}

		[DataMember]
		public SourceType Type { get; set; }

		[DataMember]
		public int TimeTaken { get; private set; }

		[DataMember]
		public int NumFunctions { get; private set; }

		[DataMember]
		public List<Function> Functions { get; set; } = new List<Function>();
	}
}
