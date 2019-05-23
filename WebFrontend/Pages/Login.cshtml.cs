using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.RegularExpressions;

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
                    // Await response from database for valid login and redirect to index page if successful
                    if (Username == "Testing" && this.Password == "Password1!")
                    {
                        return RedirectToPage("/Index");
                    }
                    ModelState.AddModelError(string.Empty, "Invalid username or password.");
                }
                catch (Exception ex)
                {
                    Console.Write(ex);
                }
                return Page();
		}
        [BindProperty]
        public UserModel UserModel { get; set; }

        [TempData]
        public string Message { get; set; }

        public async Task<IActionResult> OnPostSignUp()
        {
            try
            {
                // Await response from database for confirmation of email sent
                if (UserModel.Username == "Testing" && UserModel.CheckPassword == "Password1!" && UserModel.ConfirmPassword == UserModel.CheckPassword)
                {
                    Message = "An email has been sent to your email address. Follow the link to activate your account.";

                    return Page(); 
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex);
            }
            return Page();
        }
    }
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

        [Required(ErrorMessage = "Password is required")]
        [CheckPass]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters long")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string CheckPassword { get; set; }
        public class CheckPassAttribute : ValidationAttribute
        {
            protected override ValidationResult IsValid(object value, ValidationContext validationContext)
            {
                string pass = "";
                if(value != null)
                {
                    pass = value.ToString();
                }
                else if(!(Regex.Match(pass, @"/[a-z]+/")).Success)
                {
                    return new ValidationResult("Password must contain atleast 1 lowercase");
                }
                else if (!(Regex.Match(pass, @"/[A-Z]+/")).Success)
                {
                    return new ValidationResult("Password must contain atleast 1 uppercase");
                }
                else if (!(Regex.Match(pass, @"/[0-9]+/")).Success)
                {
                    return new ValidationResult("Password must contain atleast 1 number");
                }
                else if (!(Regex.Match(pass, @"/?=.*?[#?!@$%^&*-]")).Success)
                {
                    return new ValidationResult("Password must contain atleast 1 symbol");
                }
                return ValidationResult.Success;
            }
        }
    }
}
