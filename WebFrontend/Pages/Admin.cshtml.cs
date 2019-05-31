using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SQLite;
using System.Threading.Tasks;
using Backend;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebFrontend.Pages
{
	public class AdminModel : Models.FileScanner
	{

		// private DatabaseManager _databaseManager = new DatabaseManager(); 
		// private List<string[]> _localList = _databaseManager.GetUserList(_databaseManager.ConnectToDatabase()); 

		[BindProperty]
		[Display(Name = "Model")]
		public IFormFile UploadModel { get; set; }
		public bool ModelUpload { get; set; }
        public List<string[]> LocalList { get; set; }
        public new User User { get; set; }
        public string ModelName { get; set; }
		DatabaseManager DatabaseManager { get; set; }
        [BindProperty]
        public int UserToDelete { get; set; }


		public AdminModel() {
			DatabaseManager = new DatabaseManager();
			LocalList = DatabaseManager.GetUserList(DatabaseManager.ConnectToDatabase());
		}

        public void OnPostDelete()
        {
            DatabaseManager = new DatabaseManager();
            SQLiteConnection db = DatabaseManager.ConnectToDatabase();
            try
            {
                DatabaseManager.DeleteUser(UserToDelete, db);
                LocalList = DatabaseManager.GetUserList(DatabaseManager.ConnectToDatabase());
            }
            catch(Exception e)
            {
                //s
            }
        }
        public IActionResult OnGet()
		{
            User = GetUserFromCookie();
            if (User == null)
            {
                return RedirectToPage("/Login");
            }

            DatabaseManager = new DatabaseManager();
			LocalList = DatabaseManager.GetUserList(DatabaseManager.ConnectToDatabase());
            return null;
		}

		public async Task<IActionResult> OnPostUploadModel()
		{
			if (UploadModel.Length == 0)
			{
				ModelState.AddModelError(UploadModel.Name, "The uploaded model must be valid");
			}

			if (!ModelState.IsValid)
			{
				return Page();
			}

			using (var fileStream = FileManager.GetModelStream())
			{
				await UploadModel.CopyToAsync(fileStream);
			}

			return Redirect("/Admin");
		}
	}

	public class ScanModel
	{
		public string CurrentModel { get; set; }
	}
}
