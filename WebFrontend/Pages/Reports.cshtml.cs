using System;
using System.Collections.Generic;
using System.Data.SQLite;
using Backend;
using Microsoft.AspNetCore.Mvc;

namespace WebFrontend
{
	public class DatabaseModel : Models.FileScanner
	{
		public int NumReports { get; set; }
		public new User User { get; set; }
		DatabaseManager DatabaseManager { get; set; }
        public Results Result { get; set; }
        [BindProperty]
        public int ResultID { get; set; }
        public bool ShowResult { get; set; }
        public List<string[]> ResultsList { get; set; }
        public string Probabilities { get; set; } = "[";
        public string FunctionAxis { get; set; } = "[";
        public int TotalIssues { get; set; } = 0;
        public float HighProb { get; set; }
        [BindProperty]
        public string HighlightedFunctionHexCode { get; set; }
        [BindProperty]
        public string HighlightedFunctionAsmCode { get; set; }
        public IActionResult OnGet()
		{
            
            ShowResult = false;
            User = GetUserFromCookie();

			if (User == null)
			{
				return RedirectToPage("/Login");
			}
			DatabaseManager = new DatabaseManager();
			SQLiteConnection db = DatabaseManager.ConnectToDatabase();
            ResultsList = DatabaseManager.GetResultsList(User.UserID, db);
			NumReports = ResultsList.Count;

			return null;
		}
        private void ShowResults()
        {
            HighProb = 0;

            foreach (Function function in Result.Functions)
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

            for (int i = 1; i <= Result.Functions.Count; ++i)
            {
                FunctionAxis += i.ToString();
                FunctionAxis += ",";
            }

            FunctionAxis += "]";
        }
        public IActionResult OnPostView()
        {
            ShowResult = true;
            User = GetUserFromCookie();

            if (User == null)
            {
                return RedirectToPage("/Login");
            }

            DatabaseManager = new DatabaseManager();
            SQLiteConnection db = DatabaseManager.ConnectToDatabase();
            try
            {
                Result = DatabaseManager.GetResult(ResultID, db);
                ShowResults();
            }
            catch(Exception e)
            {
                ErrorMessage = "Unable to print result. Error: " + e.Message;
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
