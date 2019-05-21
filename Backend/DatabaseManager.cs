using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Security.Cryptography;
using System.Text;

namespace Backend
{

	public class DatabaseManager
	{
		public SQLiteConnection DBConnection;
		public void ConnectToDatabase()
		{
			string database = "../Backend/Automatic_Cybersecurity.sqlite";
			DBConnection = new SQLiteConnection("Data Source="+database+";Version=3;");
			DBConnection.Open();
		}
		/*
		* GetUserList()
		* @purpose
		* Return a List of String Arrays
		* Each Array contains the User ID, Name and Email to allow an admin to identify
		* which user they will investigte further
		* @return
		* List<string[]> objects
		*/
		public List<string[]> GetUserList()
		{
			string sql = "SELECT UserID, Name, Email FROM User ORDER BY UserID DESC";
			SQLiteCommand command = new SQLiteCommand(sql, DBConnection);
			SQLiteDataReader results = command.ExecuteReader();
			string[] user;
			List<string[]> userList = new List<string[]>();
			while (results.Read())
			{
				user = new string[] { results["UserID"].ToString(),
					results["Name"].ToString(),
					results["Email"].ToString() };
				userList.Add(user);
			}

			return userList;
		}
		/*
		* AddUser(User user)
		* @purpose
		* Add record to User table
		* @param int userID
		* User object representing user to be added to User Table
		* @return
		* Integer
		* Case:
		*   0 on success
		*   1 on Exception
		*/
		public int AddUser(User user)
		{
			string sql = "INSERT INTO User (Email, Name, Password, Role) VALUES (?,?,?,?)";

			SQLiteParameter nameParam = new SQLiteParameter();
			SQLiteParameter emailParam = new SQLiteParameter();
			SQLiteParameter passwordParam = new SQLiteParameter();
			SQLiteParameter roleParam = new SQLiteParameter();

			SQLiteCommand command = new SQLiteCommand(sql, DBConnection);

			command.Parameters.Add(emailParam);
			command.Parameters.Add(nameParam);
			command.Parameters.Add(passwordParam);
			command.Parameters.Add(roleParam);

			nameParam.Value = user.Name;
			emailParam.Value = user.Email;
			passwordParam.Value = user.Password;
			roleParam.Value = user.Role;
			try
			{
				command.ExecuteNonQueryAsync();
				return 0;
			}
			catch (Exception e)
			{
				return 1;
			}

		}
		/*
		* DeleteUser(int userID)
		* @purpose
		* Remove record associated with a given UserID from User table
		* @param int userID
		* UserID of record to be deleted
		*/
		public void DeleteUser(int userID)
		{
			string sql = "DELETE FROM User WHERE UserID=?";

			SQLiteParameter idParam = new SQLiteParameter();

			SQLiteCommand command = new SQLiteCommand(sql, DBConnection);

			command.Parameters.Add(idParam);

			idParam.Value = userID;

			command.ExecuteNonQuery();
		}
		/*
		* SerializeResult(Results results)
		* @purpose
		* Serialize Results object to JSON object
		* @param Results results
		* Results object to be serialized
		*/
		private string SerializeResult(Results results)
		{
			MemoryStream ms = new MemoryStream();
			DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(Results));
			ser.WriteObject(ms, results);
			byte[] json = ms.ToArray();
			ms.Close();
			return Encoding.UTF8.GetString(json, 0, json.Length);
		}
		/*
		 * SaveResults(int userID, Results result)
		 * @purpose
		 * Write Results object associated wiht UserID to Results table
		 * @param int userID, Results result
		 * Integer value identifying UserID in User table
		 * Results object created from scan
		 */
		public void SaveResults(int userID, Results result)
		{
			string sql = "INSERT INTO Results (UserID, DateUploaded, FileName, ResultObject) VALUES (?,?,?,?)";

			SQLiteParameter idParam = new SQLiteParameter();
			SQLiteParameter dateParam = new SQLiteParameter();
			SQLiteParameter fileParam = new SQLiteParameter();
			SQLiteParameter resultParam = new SQLiteParameter();

			SQLiteCommand command = new SQLiteCommand(sql, DBConnection);

			command.Parameters.Add(idParam);
			command.Parameters.Add(dateParam);
			command.Parameters.Add(fileParam);
			command.Parameters.Add(resultParam);

			idParam.Value = userID;
			dateParam.Value = DateTime.Now.ToString("dd/MM/yyyy hh-mm-ss");
			//TO DO: Add filename to Results class
			fileParam.Value = "FileName";
			resultParam.Value = SerializeResult(result);

			command.ExecuteNonQuery();
		}
		/*
		 * GetResultsList(int userID)
		 * @purpose
		 * Return a List of String Arrays, each string represents a Result stored for a given user
		 * Each Array contains the Result ID, FileName and DateUploaded to allow a user to identify
		 * which function the will investigte further
		 * @param int userID
		 * Integer value identifying UserID in Results table
		 * @return
		 * List<string[]> Object
		 */
		public List<string[]> GetResultsList(int userID)
		{
			string sql = "SELECT ID, FileName, DateUploaded FROM Results WHERE UserID=?";

			SQLiteParameter idParam = new SQLiteParameter();

			SQLiteCommand command = new SQLiteCommand(sql, DBConnection);

			command.Parameters.Add(idParam);

			idParam.Value = userID;

			SQLiteDataReader results = command.ExecuteReader();

			string[] result;
			List<string[]> resultList = new List<string[]>();
			while (results.Read())
			{
				result = new string[] { results["ID"].ToString(),
					results["FileName"].ToString(),
					results["DateUploaded"].ToString() };
				resultList.Add(result);
			}
			return resultList;

		}
		/*
		 * GetResult(int id)
		 * @purpose
		 * Return a Results object used to populate fields in Reports and Dashboard screens
		 * @param int id
		 * Integer value identifying ID in Results table
		 * @return
		 * Results object
		 */
		public Results GetResult(int id)
		{
			string sql = "SELECT ResultObject FROM Results WHERE ID=?";

			SQLiteParameter idParam = new SQLiteParameter();

			SQLiteCommand command = new SQLiteCommand(sql, DBConnection);

			command.Parameters.Add(idParam);

			idParam.Value = id;

			SQLiteDataReader results = command.ExecuteReader();
			Results result = new Results();
			while (results.Read())
			{
				string resultJSON = results["ResultObject"].ToString();
				MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(resultJSON));
				DataContractJsonSerializer ser = new DataContractJsonSerializer(result.GetType());
				result = ser.ReadObject(ms) as Results;
				ms.Close();
			}
			return result;

		}
		private User QueryToUser(SQLiteDataReader query)
		{
			User user = new User();
			while (query.Read())
			{
				user.Email = query["Email"].ToString();
				user.Password = query["Password"].ToString();
				user.Name = query["Name"].ToString();
				user.UserID = query.GetInt32(0);
				user.Role = (UserRole)query.GetInt32(5);
			}
			return user;
		}
		/*
		 * GetUserFromDatabase(int userID)
		 * @purpose
		 * Return a User object used to populate fields on Profile screen
		 * @param int userID
		 * Integer value identifying UserID in User table
		 * @return
		 * User object
		 */
		public User GetUserFromDatabase(int userID)
		{
			User user = new User();
			string sql = "SELECT * FROM User WHERE UserID = ?";

			SQLiteParameter idParam = new SQLiteParameter();

			SQLiteCommand command = new SQLiteCommand(sql, DBConnection);

			command.Parameters.Add(idParam);

			idParam.Value = userID;

			SQLiteDataReader results = command.ExecuteReader();

			user = QueryToUser(results);
			return user;
		}
		private bool ValidatePassword(string password, string hashPassword)
		{
			byte[] saltedHash = Convert.FromBase64String(hashPassword);
			byte[] salt = new byte[16];
			Array.Copy(saltedHash, 0, salt, 0, 16);

			Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);

			byte[] hash = pbkdf2.GetBytes(20);

			bool match = true;
			for (int i = 0; i < 20; i++)
			{
				if (saltedHash[i + 16] != hash[i])
				{
					match = false;
				}
			}

			return match;
		}
		/*
		 * AuthenticateUser(string email, string password)
		 * @purpose
		 * Verify a provided password matches a stored hash
		 * @param string email, string password
		 * String value identifying Email in User table
		 * String value password submitted by user
		 * @return
		 * Boolean true or false
		 */
		public bool AuthenticateUser(string email, string password)
		{
			string sql = "SELECT Password FROM User WHERE Email = @email";

			SQLiteCommand command = new SQLiteCommand(sql, DBConnection);

			command.Parameters.AddWithValue("@email", email);

			User user = new User();

			SQLiteDataReader results = command.ExecuteReader();
			results.Read();
			string hash = results["Password"].ToString();
			if (ValidatePassword(password, results["Password"].ToString()))
			{
				return true;
			}
			else
			{
				return false;
			}

		}
	}
}
