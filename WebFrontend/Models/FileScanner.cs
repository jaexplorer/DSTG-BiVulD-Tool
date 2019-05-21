using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebFrontend.Models;
using static Backend.FileManager;

namespace WebFrontend.Models
{
	public class FileScanner : PageModel
	{
		[BindProperty]
        [Required]
		[Display(Name="File")]
		public IFormFile UploadFile { get; set; }

		public async Task<IActionResult> OnPostUpload() {
            if (UploadFile.Length == 0)
			{
				ModelState.AddModelError(UploadFile.Name, "The uploaded file must be valid");
			}

			if (UploadFile.ContentType != "application/octet-stream" && UploadFile.ContentType != "text/plain")
			{
				ModelState.AddModelError(UploadFile.Name, "The uploaded file must be a binary file or source code");
			}

			if (!ModelState.IsValid)
			{
				return Page();
			}

			using (var fileStream = GetFileStream())
			{
				await UploadFile.CopyToAsync(fileStream);
			}

			// TODO: Add checking for success
			return Redirect("/Dashboard?upload=Success");
		}
	}
}
