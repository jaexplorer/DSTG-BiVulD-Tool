using System.ComponentModel.DataAnnotations;
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
	}
}
