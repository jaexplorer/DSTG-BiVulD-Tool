using System;
using System.ComponentModel.DataAnnotations;
using System.Data.SQLite;
using Backend;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebFrontend.Pages
{
	public class LoginModel : PageModel
	{
		[BindProperty]
		[Required(ErrorMessage = "Email is required")]
		[DataType(DataType.EmailAddress)]
		[Display(Name = "Email")]
		public string Email { get; set; }

		[BindProperty]
		[Required(ErrorMessage = "Password is required")]
		[RegularExpression(@"^(?=.*[0-9])(?=.*[a-z])(?=.*[A-Z])(?=.*?[#?!@$%^&*-]).{6,20}$", ErrorMessage = "Invalid Password")]
		[MinLength(8)]
		[DataType(DataType.Password)]
		[Display(Name = "Password")]
		public string Password { get; set; }

		[BindProperty]
		public SignUpModel SignUpModel { get; set; }

		[TempData]
		public string ErrorMessage { get; set; }

		DatabaseManager DatabaseManager { get; set; }

		public IActionResult OnPostLogIn()
		{
			DatabaseManager = new DatabaseManager();
			SQLiteConnection db = DatabaseManager.ConnectToDatabase();
			User user;

			try
			{
				if (DatabaseManager.AuthenticateUser(Email, Password, db))
				{
					user = DatabaseManager.GetUserFromDatabase(Email, db);

					DatabaseManager.RecordLogin(Email, db);

					string cookie = user.GenerateCookie();

					DatabaseManager.StoreCookie(user.UserID, cookie, db);

					if (Request.Cookies["auth"] != null)
					{
						Response.Cookies.Delete("auth");
					}

					var CookieOptions = new CookieOptions
					{
						Expires = DateTime.Now.AddDays(30),
						IsEssential = true
					};

					Response.Cookies.Append("auth", cookie, CookieOptions);

					return RedirectToPage("/Profile");
				}

				ModelState.AddModelError(string.Empty, "Invalid username or password.");
			}
			catch (Exception e)
			{
				ErrorMessage = "Login failed. Error Message: " + e.Message;
			}

			return Page();
		}

		public IActionResult OnPostSignUp()
		{
			DatabaseManager = new DatabaseManager();
			SQLiteConnection db = DatabaseManager.ConnectToDatabase();

			User user = new User
			{
				Email = SignUpModel.Email,
				Name = SignUpModel.Username
			};

			user.Password = user.HashPassword(SignUpModel.Password);
			user.Role = UserRole.User;

			try
			{
				DatabaseManager.AddUser(user, db);

				ErrorMessage = "Account creation successful. Sign in.";

				return Page();
			}
			catch (Exception e)
			{
				ErrorMessage = "Failed to create User. Error Message: " + e.Message;
			}

			return Page();
		}
	}

	public class SignUpModel
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
}
