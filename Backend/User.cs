using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;

namespace Backend
{
	public enum UserRole
	{
		Admin,
		User
	}
	public class User
	{
		public int UserID { get; set; }
		[DataType(DataType.EmailAddress)]
		public string Email { get; set; }
		public string Name { get; set; }
		public string Password { get; set; }
		public UserRole Role { get; set; }
		public List<Results> Results { get; set; }
		private string HashPassword(string password)
		{
			byte[] salt;
			//Generate a 16 byte random salt
			new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);
			//Generate a PBKDF2 hash using the salt and 10000 iterations
			Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);
			//Prepend salt to hash, hash is 20 bytes and salt is 16 bytes giving 36 byte hash
			byte[] hash = pbkdf2.GetBytes(20);
			byte[] hashBytes = new byte[36];
			Array.Copy(salt, 0, hashBytes, 0, 16);
			Array.Copy(hash, 0, hashBytes, 16, 20);
			string passwordHash = Convert.ToBase64String(hashBytes);
			return passwordHash;
		}
		public User(string email, string name, string password){
			Email = email;
			Password = HashPassword(password);
			Name = name;
			Role = UserRole.User;
		}
		public User()
		{

		}
	}
}
