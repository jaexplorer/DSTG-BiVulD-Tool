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
		public const string PythonPath = "/usr/local/bin/python3";
		
		public const string CreateCSVPath = "create_csv.py";
		public const string CreateTestFilesPath = "create_test_files.py";
		public const string GetProbabilitiesPath = "get_probabilities.py";

		public const string FuncName = "binary_good.csv";
		public const string ProbName = "prob_assembly.csv";
	}

	public class FileManager
	{
		public FileManager()
		{
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
			if (Execute(FileNames.ObjdumpPath, "-d " + FileNames.BiVulDDir + TempPath + FileNames.UploadName) == "")
			{
				return false;
			}

			return true;
		}

		public Results AnalyseFile()
		{
			var stopwatch = new Stopwatch();

			stopwatch.Restart();

			Directory.SetCurrentDirectory(FileNames.BiVulDDir + TempPath);

			string objdump = Execute(FileNames.ObjdumpPath, "-d " + FileNames.UploadName);
			string dumpName = FileNames.UploadName.Substring(0, FileNames.UploadName.Length - 3) + "objdump";

			using (var streamWriter = new StreamWriter(dumpName))
			{
				streamWriter.Write(objdump);
			}

			Directory.SetCurrentDirectory("..");

			Execute(FileNames.PythonPath, FileNames.CreateCSVPath + " " + TempPath + " " + dumpName);
			Execute(FileNames.PythonPath, FileNames.CreateTestFilesPath + " " + TempPath);
			Execute(FileNames.PythonPath, FileNames.GetProbabilitiesPath + " " + TempPath);

			string probAssembly;

			using (var streamReader = new StreamReader(TempPath + FileNames.ProbName))
			{
				probAssembly = streamReader.ReadToEnd();
			}

			stopwatch.Stop();

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
