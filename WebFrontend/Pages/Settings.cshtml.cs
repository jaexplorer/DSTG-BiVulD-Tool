using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebFrontend.Models;
using static Backend.FileManager;

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

			using (var fileStream = GetModelStream())
			{
				await FileUpload.UploadFile.CopyToAsync(fileStream);
			}

			return RedirectToPage("./Settings");
		}
	}
}
