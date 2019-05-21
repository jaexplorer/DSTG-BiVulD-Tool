using System.Threading.Tasks;
using Backend;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebFrontend.Models;

namespace WebFrontend.Pages
{
	public class SettingsModel : PageModel
	{
		[BindProperty]
		public FileUpload FileUpload { get; set; }

		public async Task<IActionResult> OnPostAsync()
		{
			if (!ModelState.IsValid)
			{
				return Page();
			}

			using (var fileStream = FileManager.GetModelStream())
			{
				await FileUpload.UploadFile.CopyToAsync(fileStream);
			}

			return RedirectToPage("./Settings");
		}
	}
}
