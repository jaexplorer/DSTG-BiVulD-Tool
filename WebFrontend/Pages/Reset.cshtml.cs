using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebFrontend.Pages
{
	public class ResetModel : PageModel
	{
		[BindProperty]
		[Required(ErrorMessage = "Email is required")]
		[DataType(DataType.EmailAddress)]
		[Display(Name = "Email")]
		[EmailAddress(ErrorMessage = "Email is invalid")]
		[RegularExpression("^([a-zA-Z0-9_\\-\\.]+)@([a-zA-Z0-9_\\-\\.]+)\\.([a-zA-Z]{2,5})$", ErrorMessage = "Invalid Email")]
		public string Email { get; set; }

		[TempData]
		public string Message { get; set; }

		public void OnGet()
		{
			Message = null;
		}

		public async Task<IActionResult> OnPostReset()
		{
			// If email exists, send password reset email to address
			Message = "An email has been sent to your email address. Follow the link to reset your password.";
			return Page();
		}
	}
}
