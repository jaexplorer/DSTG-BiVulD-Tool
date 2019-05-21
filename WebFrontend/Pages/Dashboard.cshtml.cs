using Backend;
using Microsoft.AspNetCore.Mvc;

namespace WebFrontend
{
	public class DashboardModel : Models.FileScanner
	{
		public bool FileUpload { get; set; }
		public bool Display { get; set; } = false;
		public string Probabilities { get; set; } = "[";
		public Results Results { get; set; }
		public string FunctionAxis { get; set; } = "[";
		public int TotalIssues { get; set; } = 0;
		public float HighProb { get; set; }

		[BindProperty]
		public string HighlightedFunctionCode { get; set; }

		public void OnGet()
		{
			FileUpload = HttpContext.Request.Query["upload"] == "Success";
		}

		public void OnPostScan()
		{
			FileUpload = true;

			if (fileManager.IdentifyFile())
			{
				FileUpload = true;
				Display = true;

				PrintResults();
			}
		}

		public void PrintResults()
		{
			Results = fileManager.AnalyseFile();
			HighProb = 0;

			foreach (Backend.Function function in Results.Functions)
			{
				if (function.Prob > HighProb)
				{
					HighProb = (float)function.Prob;
				}

				if (function.Prob > 0.5)
				{
					TotalIssues += 1;
				}

				Probabilities += ((float)function.Prob * 100).ToString();
				Probabilities += ",";
			}

			HighProb *= 100;
			Probabilities += "]";

			for (int i = 1; i <= Results.NumFunctions; ++i)
			{
				FunctionAxis += i.ToString();
				FunctionAxis += ",";
			}

			FunctionAxis += "]";
		}

		public string FormatTime(int timeTaken)
		{
			int minutes = (timeTaken / 1000) / 60;
			int seconds = (timeTaken / 1000) % 60;

			return minutes.ToString() + " mins " + seconds.ToString() + " secs";
		}
	}
}
