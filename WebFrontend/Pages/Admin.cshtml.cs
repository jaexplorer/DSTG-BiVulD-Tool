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
}
