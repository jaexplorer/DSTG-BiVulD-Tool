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
		[DataMember]
		public SourceType Type { get; set; }
		[DataMember]
		public int TimeTaken { get; private set; }
		[DataMember]
		public int NumFunctions { get; private set; }
		[DataMember]
		public List<Function> Functions { get; set; } = new List<Function>();

		public Results()
		{

		}
		public Results(string directory, SourceType type)
		{
			Type = type;

			ReadProbs(directory);
			ReadHexCode(directory);
		}

		void ReadProbs(string directory)
		{
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
		}

		void ReadHexCode(string directory)
		{
			StreamReader functions = new StreamReader(directory + FileNames.FuncName);

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
