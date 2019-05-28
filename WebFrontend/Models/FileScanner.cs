using System;
using System.ComponentModel.DataAnnotations;
using System.Data.SQLite;
using System.Threading.Tasks;
using Backend;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebFrontend.Models
{
	public class FileScanner : PageModel
	{
		[BindProperty]
		[Display(Name="File")]
		public IFormFile UploadFile { get; set; }

		public bool FileUpload { get; set; }
		public string ErrorMessage { get; protected set; }
		protected static FileManager fileManager;

		public async Task<IActionResult> OnPostUpload()
		{
			fileManager = new FileManager();

			using (var fileStream = fileManager.GetFileStream())
			{
				await UploadFile.CopyToAsync(fileStream);
			}

			FileUpload = fileManager.IdentifyFile();

			return Page();
		}

		public IActionResult OnPostScan()
		{
			return Redirect("/Dashboard/?Upload=Success");
		}

		/*
		 * getUserFromCookie()
		 * @purpose
		 * Create a user object from a given cookie
		 * @return
		 * User object if cookie is valid
		 * return null if cookie is not valid
		 */
		protected User GetUserFromCookie()
		{
			DatabaseManager databaseManager = new DatabaseManager();
			SQLiteConnection db = databaseManager.ConnectToDatabase();

			try
			{
				int userID = databaseManager.ValidateCookie(Request.Cookies["auth"], db);

				if (userID != -1)
				{
					return databaseManager.GetUserFromDatabase(userID, db);
				}

				return null;
			}
			catch (Exception)
			{
				return null;
			}
		}
	}
}
