using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebFrontend.Models;
using static Backend.FileManager;

namespace WebFrontend.Pages
{
    public class IndexModel : PageModel
    {
        [BindProperty]
        public FileUpload FileUpload { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (FileUpload.UploadFile.Length == 0)
            {
                ModelState.AddModelError(FileUpload.UploadFile.Name, "The uploaded file must be valid");
            }

            if (FileUpload.UploadFile.ContentType != "application/octet-stream" && FileUpload.UploadFile.ContentType != "text/plain")
            {
                ModelState.AddModelError(FileUpload.UploadFile.Name, "The uploaded file must be a binary file or source code");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            using (var fileStream = GetFileStream())
            {
                await FileUpload.UploadFile.CopyToAsync(fileStream);
            }
            // Add checking for success
            return Redirect("/Dashboard?upload=Success");
        }
    }
}
