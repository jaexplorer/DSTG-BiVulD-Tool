using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebFrontend.Pages
{
	public class DashboardModel : PageModel
	{
        public bool FileUpload { get; set; }
        public bool Display { get; set; } = false;
        public string Probabilities { get; set; } = "[";     
        public Backend.Results Results { get; set; }       
        public string FunctionAxis { get; set; } = "[";      
        public int TotalIssues { get; set; } = 0;
        public float HighProb { get; set; }
        [BindProperty]
        public string HighlightedFunctionCode { get; set; }
        public void OnGet()
		{
            FileUpload = false;
            if (HttpContext.Request.Query["upload"] == "Success")
            {
                FileUpload = true; 
            }
		}
        public void OnPostScan()
        {
            FileUpload = true;
            if (Backend.FileManager.IdentifyFile())
            {
                FileUpload = true;
                Display = true;
                PrintResults();
            }
        }
        public void PrintResults()
        {
            Results = Backend.FileManager.AnalyseFile();
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
            HighProb = HighProb * 100;
            Probabilities += "]";
            for (int i = 1; i <= Results.NumFunctions; i++)
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
