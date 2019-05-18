using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Backend;
using System.ComponentModel.DataAnnotations;


namespace WebFrontend.Pages
{
    public class TestUserModel
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
    public class DatabaseTestModel : PageModel
    {
        [BindProperty]
        public TestUserModel TestUserModel { get; set; }
        public string ErrorMessage { get; set; }
        public List<string[]> Users { get; set; }
        [TempData]
        public string Message { get; set; }
        public void OnGet()
        {
            DatabaseManager databaseManager = new DatabaseManager();
            Message = "";
            try
            {
                databaseManager.ConnectToDatabase();
                Users = databaseManager.GetUserList();
                databaseManager.DBConnection.Close();
            }
            catch(Exception e)
            {
                databaseManager.DBConnection.Close();
                ErrorMessage = "DATABASE ERROR";
            }  
            
        }
        public IActionResult OnPostSignUp()
        {
            try
            {
                DatabaseManager databaseManager = new DatabaseManager();
                
                User user = new User(TestUserModel.Email, TestUserModel.Username, TestUserModel.Password);
                if (ModelState.IsValid)
                {
                    databaseManager.ConnectToDatabase();

                    if (databaseManager.AddUser(user) == 0) 
                    {
                        databaseManager.DBConnection.Close();
                        Message = "An email has been sent to your email address. Follow the link to activate your account.";

                        return Page();
                    }
                    else
                    {
                        databaseManager.DBConnection.Close();
                        ModelState.AddModelError(string.Empty, "Account creation failed");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex);
            }

            return Page();
        }
        public IActionResult OnPostDelete()
        {

            return Page();
        }
    }
}