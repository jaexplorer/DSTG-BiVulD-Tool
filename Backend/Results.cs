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

			using (var probs = new StreamReader(directory + FileNames.ProbName))
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
					bool ignoringComma = false;
					string temp = "";

					foreach (char c in asm.ReadLine())
					{
						if (c == '"')
						{
							ignoringComma = !ignoringComma;
						}
						else
						{
							if ((c != ',') || ignoringComma)
							{
								temp += c;
							}
							else
							{
								Functions[i].AsmCode.Add(temp);

								temp = "";
							}
						}
					}

					Functions[i].AsmCode.Add(temp);
				}
			}
		}

		[DataMember]
		public SourceType Type { get; set; }

		[DataMember]
		public int TimeTaken { get; private set; }

		[DataMember]
		public int NumFunctions
		{
			get
			{
				return Functions.Count;
			}
		}

		[DataMember]
		public List<Function> Functions { get; set; } = new List<Function>();
	}
}
