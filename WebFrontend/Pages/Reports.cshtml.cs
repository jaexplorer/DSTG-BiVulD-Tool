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

		public IActionResult OnGet()
		{
			User = GetUserFromCookie();

			if (User == null)
			{
				return RedirectToPage("/Login");
			}

			DatabaseManager = new DatabaseManager();
			SQLiteConnection db = DatabaseManager.ConnectToDatabase();
			var results = DatabaseManager.GetResultsList(User.UserID, db);
			NumReports = results.Count;

			return null;
		}
	}
}
