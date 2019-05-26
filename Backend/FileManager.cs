using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;

namespace Backend
{
	struct FileNames
	{
		public const string BiVulDDir = "../BiVulD/";

		public const string UploadName = "upload.bin";

		public const string ModelPath = "../BiVulD/model.h5";

		public const string ObjdumpPath = "objdump";
		// public const string PythonPath = "/usr/bin/python3";

		// Andrew's Python
		// public const string PythonPath = "/Users/AndrewSabato/anaconda3/bin/python";

		// Kai's Python
		public const string PythonPath = "/usr/local/bin/python3";

		public const string CreateCSVPath = "create_csv.py";
		public const string CreateTestFilesPath = "create_test_files.py";
		public const string GetProbabilitiesPath = "get_probabilities.py";

		public const string HexName = "binary_good.csv";
		public const string AsmName = "asm_good.csv";
		public const string ProbName = "prob_assembly.csv";
	}

	public class FileManager
	{
		public FileManager()
		{
			// Generate a random name for the temporary directory
			byte[] salt;
			new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);
			TempPath = BitConverter.ToUInt32(salt, 0).ToString() + "/";

			Directory.CreateDirectory(FileNames.BiVulDDir + TempPath);
		}

		public string TempPath { get; private set; } = "";

		public FileStream GetFileStream()
		{
			return new FileStream(FileNames.BiVulDDir + TempPath + FileNames.UploadName, FileMode.Create);
		}

		public static FileStream GetModelStream()
		{
			return new FileStream(FileNames.ModelPath, FileMode.Create);
		}

		public bool IdentifyFile()
		{
			// If objdump returns nothing, it can't read the file
			if (Execute(FileNames.ObjdumpPath, "-d " + FileNames.BiVulDDir + TempPath + FileNames.UploadName) == "")
			{
				return false;
			}

			return true;
		}

		public Results AnalyseFile()
		{
			var stopwatch = new Stopwatch();

			stopwatch.Start();
			Directory.SetCurrentDirectory(FileNames.BiVulDDir + TempPath);

			// Disassemble the file for the Python scripts
			string objdump = Execute(FileNames.ObjdumpPath, "-d " + FileNames.UploadName);
			string dumpName = FileNames.UploadName.Substring(0, FileNames.UploadName.Length - 3) + "objdump";

			using (var streamWriter = new StreamWriter(dumpName))
			{
				streamWriter.Write(objdump);
			}

			Directory.SetCurrentDirectory("..");

			// Run the Python scripts on the diassembled file
			Execute(FileNames.PythonPath, FileNames.CreateCSVPath + " " + TempPath + " " + dumpName);
			Execute(FileNames.PythonPath, FileNames.CreateTestFilesPath + " " + TempPath);
			Execute(FileNames.PythonPath, FileNames.GetProbabilitiesPath + " " + TempPath);

			string probAssembly;

			// Read the results of the Python scripts
			using (var streamReader = new StreamReader(TempPath + FileNames.ProbName))
			{
				probAssembly = streamReader.ReadToEnd();
			}

			stopwatch.Stop();

			// Insert the time taken to run the scripts into the results file
			using (var streamWriter = new StreamWriter(TempPath + FileNames.ProbName))
			{
				streamWriter.Write(stopwatch.ElapsedMilliseconds + probAssembly.Substring(1));
			}

			return new Results(FileNames.BiVulDDir + TempPath, SourceType.Binary);
		}

		static string Execute(string fileName, string arguments)
		{
			ProcessStartInfo psi = new ProcessStartInfo
			{
				FileName = fileName,
				Arguments = arguments,
				UseShellExecute = false,
				RedirectStandardOutput = true
			};

			using (Process process = Process.Start(psi))
			{
				using (StreamReader sr = process.StandardOutput)
				{
					return sr.ReadToEnd();
				}
			}
		}
	}
}
