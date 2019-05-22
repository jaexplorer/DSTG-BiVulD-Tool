using Backend;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace WebFrontend
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
    }
    public class ProfileModel : Models.FileScanner
    {
        [BindProperty]
        public UserModel UserModel { get; set; }

        [BindProperty]
        public User User { get; set; }
        private DatabaseManager databaseManager { get; set; }
        public string ErrorMessage { get; set; }
        private User getUserFromCookie()
        {
            databaseManager = new DatabaseManager();
            databaseManager.ConnectToDatabase();
            User user = new User();
            try
            {
                //TO DO: Add userID identifer from cookie as parameter
                user = databaseManager.GetUserFromDatabase(1);
                databaseManager.DBConnection.Close();
            }
            catch (Exception e)
            {
                ErrorMessage = "Failed to find user";
                databaseManager.DBConnection.Close();
            }
            return user;
        }
        public void OnGet()
        {
            User = getUserFromCookie();
            UserModel = new UserModel();
            UserModel.Email = User.Email;
            UserModel.Username = User.Name;
        }
        public IActionResult OnPostSave()
        {          
            User = getUserFromCookie();
            databaseManager = new DatabaseManager();
            databaseManager.ConnectToDatabase();
            User.Name = UserModel.Username;
            User.Email = UserModel.Email;
            try
            {
                databaseManager.UpdateUser(User);
                databaseManager.DBConnection.Close();
                return Page();
            }
            catch (Exception e)
            {
                ErrorMessage = "User Update Failed";
                databaseManager.DBConnection.Close();
            }
            
            return Page();
        }
    }
}
