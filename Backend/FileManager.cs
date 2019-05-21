using System.Diagnostics;
using System.IO;

namespace Backend
{
	struct FileNames
	{
		public const string BiVulDDir = "../BiVulD";

		public const string UploadName = "sample/upload.bin";
		public const string UploadPath = BiVulDDir + "/" + UploadName;

		public const string ModelPath = "../BiVulD/model.h5";

		public const string ObjdumpPath = "objdump";
		public const string PythonPath = "/usr/bin/python3";
		
		public const string CreateCSVPath = "create_csv.py";
		public const string CreateTestFilesPath = "create_test_files.py";
		public const string GetProbabilitiesPath = "get_probabilities.py";

		public const string FuncName = "binary_good.csv";
		public const string ProbName = "prob_assembly.csv";
	}

	public static class FileManager
	{
		public static FileStream GetFileStream()
		{
			return new FileStream(FileNames.UploadPath, FileMode.Create);
		}

		public static FileStream GetModelStream()
		{
			return new FileStream(FileNames.ModelPath, FileMode.Create);
		}

		public static bool IdentifyFile()
		{
			if (Execute(FileNames.ObjdumpPath, "-d " + FileNames.UploadPath) == "")
			{
				return false;
			}

			return true;
		}

		public static Results AnalyseFile()
		{
			var stopwatch = new Stopwatch();

			stopwatch.Restart();

			Directory.SetCurrentDirectory(FileNames.BiVulDDir);

			string objdump = Execute(FileNames.ObjdumpPath, "-d " + FileNames.UploadName);
			string dumpName = FileNames.UploadName.Substring(0, FileNames.UploadName.Length - 3) + "objdump";

			using (var streamWriter = new StreamWriter(dumpName))
			{
				streamWriter.Write(objdump);
			}

			Execute(FileNames.PythonPath, FileNames.CreateCSVPath + " " + dumpName);
			Execute(FileNames.PythonPath, FileNames.CreateTestFilesPath);
			Execute(FileNames.PythonPath, FileNames.GetProbabilitiesPath);

			string probAssembly;

			using (var streamReader = new StreamReader(FileNames.ProbName))
			{
				probAssembly = streamReader.ReadToEnd();
			}

			stopwatch.Stop();

			using (var streamWriter = new StreamWriter(FileNames.ProbName))
			{
				streamWriter.Write(stopwatch.ElapsedMilliseconds + probAssembly.Substring(1));
			}

			return new Results(FileNames.BiVulDDir, SourceType.Binary);
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
