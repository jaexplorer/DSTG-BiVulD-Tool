using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebFrontend.Pages
{
	public class LoginModel : PageModel
	{
		[BindProperty]
		[Required(ErrorMessage = "Username is required")]
		[MinLength(6, ErrorMessage = "Invalid username")]
		[Display(Name = "Username")]
		public string Username { get; set; }

		[BindProperty]
		[Required(ErrorMessage = "Password is required")]
		[RegularExpression(@"^(?=.*[0-9])(?=.*[a-z])(?=.*[A-Z])(?=.*?[#?!@$%^&*-]).{6,20}$", ErrorMessage = "Invalid Password")]
		[MinLength(8)]
		[DataType(DataType.Password)]
		[Display(Name = "Password")]
		public string Password { get; set; }

		[BindProperty]
		[Display(Name = "Status")]
		public string Status { get; set; }

		public async Task<IActionResult> OnPostLogIn()
		{
			try
			{
				if (ModelState.IsValid)
				{
					// Await response from database for valid login and redirect to index page if successful
					if (Username == "Testing" && this.Password == "Password1!")
					{
						return RedirectToPage("/Index");
					}

					ModelState.AddModelError(string.Empty, "Invalid username or password.");
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
