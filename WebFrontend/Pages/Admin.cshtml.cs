using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Backend;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebFrontend.Pages
{
	public class AdminModel : Models.FileScanner
	{
        [BindProperty]
        [Display(Name = "Model")]
        public IFormFile UploadModel { get; set; }

        public List<string[]> LocalList { get; set; }
        DatabaseManager DatabaseManager { get; set; }

        public void OnGet()
        {
            DatabaseManager = new DatabaseManager();
            LocalList = DatabaseManager.GetUserList(DatabaseManager.ConnectToDatabase());
        }
        public string ModelName { get; set; }

        public void DeleteUser(int value)
        {
            DatabaseManager = new DatabaseManager();
            DatabaseManager.DeleteUser(value, DatabaseManager.ConnectToDatabase());
        }

        public string UserToDelete { get; set; }

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
