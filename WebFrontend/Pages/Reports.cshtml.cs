using Backend;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SQLite;

namespace WebFrontend
{
	public class DatabaseModel : Models.FileScanner
	{
        public int NumReports { get; set; }
        public User User { get; set; }
        private DatabaseManager databaseManager { get; set; }
        public IActionResult OnGet()
        {
            User = getUserFromCookie();
            if (User == null)
            {
                return RedirectToPage("/Login");
            }
            else
            {
                databaseManager = new DatabaseManager();
                SQLiteConnection db = databaseManager.ConnectToDatabase();
                var results = databaseManager.GetResultsList(User.UserID, db);
                NumReports = results.Count;

                return null;
            }
        }
    }
}
