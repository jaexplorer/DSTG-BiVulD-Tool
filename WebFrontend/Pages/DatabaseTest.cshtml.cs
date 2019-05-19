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
    public class TestLoginModel
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
    }
    public class TestDeleteUserModel
    {
        [Required(ErrorMessage = "ID is required")]
        [Range(0, int.MaxValue, ErrorMessage = "ID must be a positive number")]
        public int UserID { get; set; }
    }
    public class ShowResultModel
    {
        [Required(ErrorMessage = "ID is required")]
        [Range(0, int.MaxValue, ErrorMessage = "ID must be a positive number")]
        public int ID { get; set; }
    }
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
        [BindProperty]
        public TestLoginModel TestLoginModel { get; set; }
        [BindProperty]
        public TestDeleteUserModel TestDeleteUserModel { get; set; }
        [BindProperty]
        public ShowResultModel ShowResultModel { get; set; }
        public string ErrorMessage { get; set; }
        public List<string[]> Users { get; set; }
        public List<string[]> Results { get; set; }
        [TempData]
        public string Message { get; set; }
        public Results ShowResult { get; set; }
        public string Probabilities { get; set; } = "[";
        public string FunctionAxis { get; set; } = "[";
        public int TotalIssues { get; set; } = 0;
        public float HighProb { get; set; }
        public bool Show { get; set; }
        public User TestUser { get; set; }
        [BindProperty]
        public string HighlightedFunctionCode { get; set; }
        public void OnGet()
        {
            Show = false;
            DatabaseManager databaseManager = new DatabaseManager();
            Message = "";
            try
            {
                databaseManager.ConnectToDatabase();
                Users = databaseManager.GetUserList();
                Results = databaseManager.GetResultsList(1);
                TestUser = databaseManager.GetUserFromDatabase(1);
                databaseManager.DBConnection.Close();
            }
            catch (Exception e)
            {
                databaseManager.DBConnection.Close();
                ErrorMessage = "DATABASE ERROR";
            }

        }
        public IActionResult OnPostSignUp()
        {
           
                DatabaseManager databaseManager = new DatabaseManager();

                User user = new User(TestUserModel.Email, TestUserModel.Username, TestUserModel.Password);
                
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
                
           

            return Page();
        }
        public IActionResult OnPostDelete()
        {

            DatabaseManager databaseManager = new DatabaseManager();
            databaseManager.ConnectToDatabase();
            databaseManager.DeleteUser(TestDeleteUserModel.UserID);
            databaseManager.DBConnection.Close();
            return Page();
        }

        public IActionResult OnPostSave()
        {
            Results result = FileManager.AnalyseFile();
            DatabaseManager databaseManager = new DatabaseManager();
            databaseManager.ConnectToDatabase();
            try
            {
                databaseManager.SaveResults(1, result);
                databaseManager.DBConnection.Close();
            }
            catch (Exception e)
            {
                ErrorMessage = e.ToString();
                databaseManager.DBConnection.Close();
            }

            return Page();
        }
        public void PrintResults()
        {
            HighProb = 0;

            foreach (Backend.Function function in ShowResult.Functions)
            {
                if (function.Prob > HighProb)
                {
                    HighProb = (float)function.Prob;
                }

                if (function.Prob > 0.5)
                {
                    TotalIssues += 1;
                }

                Probabilities += ((float)function.Prob * 100).ToString();
                Probabilities += ",";
            }

            HighProb *= 100;
            Probabilities += "]";

            for (int i = 1; i <= ShowResult.NumFunctions; ++i)
            {
                FunctionAxis += i.ToString();
                FunctionAxis += ",";
            }

            FunctionAxis += "]";
        }

        public string FormatTime(int timeTaken)
        {
            int minutes = (timeTaken / 1000) / 60;
            int seconds = (timeTaken / 1000) % 60;

            return minutes.ToString() + " mins " + seconds.ToString() + " secs";
        }
        public IActionResult OnPostShow()
        {
            Show = true;
            DatabaseManager databaseManager = new DatabaseManager();
            databaseManager.ConnectToDatabase();
            ShowResult = databaseManager.GetResult(ShowResultModel.ID);
            databaseManager.DBConnection.Close();
            PrintResults();
            /*
            try
            {
                databaseManager.SaveResults(1, result);
                databaseManager.DBConnection.Close();
            }
            catch (Exception e)
            {
                ErrorMessage = e.ToString();
                databaseManager.DBConnection.Close();
            }
            */

            return Page();
        }

        public IActionResult OnPostLogIn()
        {
            DatabaseManager databaseManager = new DatabaseManager();
            databaseManager.ConnectToDatabase();
            
                
                    if (databaseManager.AuthenticateUser(TestLoginModel.Username, TestLoginModel.Password))
                    {
                        databaseManager.DBConnection.Close();
                        return RedirectToPage("/Index");
                    }

                    
                
            

            return Page();
        }
    }
}
