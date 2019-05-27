using System;
using Backend;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.RegularExpressions;
using System.Data.SQLite;

namespace WebFrontend.Pages
{
    public class LoginModel : PageModel
    {
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
        [Display(Name = "Status")]
        public string Status { get; set; }
        private DatabaseManager databaseManager { get; set; }
        public async Task<IActionResult> OnPostLogIn()
        {
            databaseManager = new DatabaseManager();
            SQLiteConnection db = databaseManager.ConnectToDatabase();
            try
            {
                if(databaseManager.AuthenticateUser(UserModel.Email,UserModel.Password, db))
                {
                    //To Add set current webpage to be "Logged In"
                    return RedirectToPage("/Profile");
                }
                ModelState.AddModelError(string.Empty, "Invalid username or password.");
            }
            catch (Exception e)
            {
                Console.Write(e);
            }
            return Page();
        }
        [BindProperty]
        public UserModel UserModel { get; set; }

        [TempData]
        public string Message { get; set; }

        public async Task<IActionResult> OnPostSignUp()
        {
            databaseManager = new DatabaseManager();
            SQLiteConnection db = databaseManager.ConnectToDatabase();
            try
            {
                Message = "An email has been sent to your email address. Follow the link to activate your account.";
                return Page();
            }
            catch (Exception e)
            {
                Message = "Signup Failed. Error Message: " + e.Message;
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
    }
}
