using System;
using System.Data.SQLite;
using Backend;
using Microsoft.AspNetCore.Mvc;

namespace WebFrontend
{
	public class DashboardModel : Models.FileScanner
	{
		public bool Display { get; set; } = false;
		public string Probabilities { get; set; } = "[";
		public Results Results { get; set; }
		public string FunctionAxis { get; set; } = "[";
		public int TotalIssues { get; set; } = 0;
		public float HighProb { get; set; }
		public new User User { get; set; }
		DatabaseManager DatabaseManager { get; set; }

		[BindProperty]
		public string HighlightedFunctionHexCode { get; set; }

		[BindProperty]
		public string HighlightedFunctionAsmCode { get; set; }

		public IActionResult OnGet()
		{
			User = GetUserFromCookie();

			if (User == null)
			{
				return RedirectToPage("/Login");
			}

			Display = HttpContext.Request.Query["Upload"] == "Success";

			if (Display)
			{
				Results = fileManager.AnalyseFile();
				DatabaseManager = new DatabaseManager();
				SQLiteConnection db = DatabaseManager.ConnectToDatabase();

				try
				{
					DatabaseManager.SaveResults(User.UserID, Results, db);
				}
				catch (Exception)
				{
					ErrorMessage = "Result could not be saved";
				}

				HighProb = 0;

				foreach (Function function in Results.Functions)
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

				for (int i = 1; i <= Results.Functions.Count; ++i)
				{
					FunctionAxis += i.ToString();
					FunctionAxis += ",";
				}

				FunctionAxis += "]";

				fileManager.Cleanup();
			}

			return null;
		}

		public string FormatTime(int timeTaken)
		{
			int minutes = (timeTaken / 1000) / 60;
			int seconds = (timeTaken / 1000) % 60;

			return minutes.ToString() + " mins " + seconds.ToString() + " secs";
		}
	}
}
