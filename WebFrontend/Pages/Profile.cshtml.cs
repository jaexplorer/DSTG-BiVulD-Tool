using System;
using System.ComponentModel.DataAnnotations;
using System.Data.SQLite;
using Backend;
using Microsoft.AspNetCore.Mvc;

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

	public class PasswordModel
	{
		[Required(ErrorMessage = "Password is required")]
		[RegularExpression(@"^(?=.*[0-9])(?=.*[a-z])(?=.*[A-Z])(?=.*?[#?!@$%^&*-]).{6,20}$",
			ErrorMessage = "Password must contain at least one lowercase, UPPERCASE, number and symbol")]
		[MinLength(8, ErrorMessage = "Password must be at least 8 characters long")]
		[DataType(DataType.Password)]
		[Display(Name = "Password")]
		public string Password { get; set; }

		[Required(ErrorMessage = "Retype Password")]
		[Compare("Password", ErrorMessage = "Passwords must match.")]
		[Display(Name = "Retype Password")]
		public string ConfirmPassword { get; set; }
	}

	public class ProfileModel : Models.FileScanner
	{
		[BindProperty]
		public UserModel UserModel { get; set; }

		[BindProperty]
		public PasswordModel PasswordModel { get; set; }

		[BindProperty]
		public string LastLogin { get; set; }

		public int TotalScans { get; set; }
		public new User User { get; set; }
		public string ErrorMessage { get; set; }
		DatabaseManager DatabaseManager { get; set; }

		public IActionResult OnGet()
		{
			User = GetUserFromCookie();

			if (User == null)
			{
				return RedirectToPage("/Login");
			}

			GetUserInfo(User.UserID);
			UserModel = new UserModel();

			PasswordModel = new PasswordModel
			{
				Password = "",
				ConfirmPassword = ""
			};

			UserModel.Email = User.Email;
			UserModel.Username = User.Name;

			return null;
		}

		public IActionResult OnPostSave()
		{
			User = GetUserFromCookie();

			if (User == null)
			{
				return RedirectToPage("/Login");
			}

			DatabaseManager = new DatabaseManager();
			SQLiteConnection db = DatabaseManager.ConnectToDatabase();
			User.Name = UserModel.Username;
			User.Email = UserModel.Email;

			try
			{
				DatabaseManager.UpdateUser(User, db);

				return RedirectToPage("/Profile");
			}
			catch (Exception e)
			{
				ErrorMessage = "User Update Failed. Error Message: " + e.Message;
			}

			return RedirectToPage("/Profile");
		}

		public IActionResult OnPostSavePassword()
		{
			User = GetUserFromCookie();

			if (User == null)
			{
				return RedirectToPage("/Login");
			}

			DatabaseManager = new DatabaseManager();
			SQLiteConnection db = DatabaseManager.ConnectToDatabase();
			User updateUser = new User(User.UserID, User.Email, User.Name, PasswordModel.Password);

			try
			{
				DatabaseManager.UpdateUser(updateUser, db);

				return RedirectToPage("/Profile");
			}
			catch (Exception e)
			{
				ErrorMessage = "User Update Failed. Error Message: " + e.Message;
			}

			return RedirectToPage("/Profile");
		}

		/*
		 * getUserInfo(int userID)
		 * @purpose
		 * Retrieve User info from  database
		 * @param
		 * int userID represesnting user
		*/
		void GetUserInfo(int userID)
		{
			DatabaseManager = new DatabaseManager();
			SQLiteConnection db = DatabaseManager.ConnectToDatabase();

			try
			{
				LastLogin = DatabaseManager.GetLastLogin(userID, db);
				TotalScans = DatabaseManager.GetTotalScans(userID, db);
			}
			catch (Exception e)
			{
				ErrorMessage = "Failed to find user. Error Message: " + e.Message;
			}
		}
	}
}
