using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebFrontend.Pages
{
	public class UserModel
	{
		[Required(ErrorMessage = "Username is required")]
		[MinLength(6, ErrorMessage = "Username must be at least 6 characters")]
		[Display(Name = "Username")]
		public string Username { get; set; }

		[Required(ErrorMessage = "Email is required")]
		[DataType(DataType.EmailAddress)]
		[Display(Name = "Email")]
		public string Email { get; set; }

		[Required(ErrorMessage = "Password is required")]
		[RegularExpression(@"^(?=.*[0-9])(?=.*[a-z])(?=.*[A-Z])(?=.*?[#?!@$%^&*-]).{6,20}$", ErrorMessage = "Password must contain at least one lowercase, UPPERCASE, number and symbol")]
		[MinLength(8, ErrorMessage = "Password must be at least 8 characters long")]
		[DataType(DataType.Password)]
		[Display(Name = "Password")]
		public string Password { get; set; }

		[Required(ErrorMessage = "Retype Password")]
		[Compare("Password", ErrorMessage = "Passwords must match.")]
		[Display(Name = "Retype Password")]
		public string ConfirmPassword { get; set; }
	}

	public class SignupModel : PageModel
	{
		[BindProperty]
		public UserModel UserModel { get; set; }

		[TempData]
		public string Message { get; set; }

		public void OnGet()
		{
			//
		}

		public async Task<IActionResult> OnPostSignUp()
		{
			try
			{
				if (ModelState.IsValid)
				{
					// Await response from database for confirmation of email sent
					if (UserModel.Username == "Testing" && UserModel.Password == "Password1!" && UserModel.ConfirmPassword == UserModel.Password)
					{
						Message = "An email has been sent to your email address. Follow the link to activate your account.";

						return Page();
					}

					ModelState.AddModelError(string.Empty, "Account creation failed");
				}
			}
			catch (Exception ex)
			{
				Console.Write(ex);
			}

			return Page();
		}
	}
}
