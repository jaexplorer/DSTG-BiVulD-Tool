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
		private string database = "../Backend/Automatic_Cybersecurity.sqlite";
		/*
		 * IsTableExists(string tableName, SQLiteConnection db)
		 * @purpose
		 * Check if a table exists in database
		 * @param string tableName, SQLiteConnection db
		 * string tableName Table to be checked
		 * SQLiteConnection db connection to database
		 * @return
		 * bool true if exists otherwise false
		 */
		private bool IsTableExists(string tableName, SQLiteConnection db)
		{
			string sql = "pragma table_info(\"" + tableName + "\")";
			SQLiteCommand command = new SQLiteCommand(sql, db);

			if (command.ExecuteScalar() != null)
			{
				return true;
			}
			else
			{
				return false;
			}
		}
		/*
		 * IsDbExists(string fileName)
		 * @purpose
		 * Check if database exists
		 * @param string fileName
		 * string fileName directory path of database
		 * @return
		 * bool true if exists otherwise false
		 */
		private bool IsDbExists(string fileName)
		{

			bool exists = File.Exists(fileName);
			if (exists)
			{
				SQLiteConnection db = new SQLiteConnection("Data Source=" + fileName + ";Version=3;");
				db.Open();
				try
				{
					var userTable = IsTableExists("User", db);
					var resultTable = IsTableExists("Results", db);
					db.Close();
					if (db == null || !userTable || !resultTable)
					{
						return false;
					}
					else
					{
						return true;
					}
				}
				catch(Exception e)
				{
					db.Close();
					return false;
				}
			}
			else
			{
				return false;
			}
		}
		/*
		 * CreateDatabase()
		 * @purpose
		 * Create database and populate admin user
		 * @return
		 * SQLiteConnection object reresenting connnection to database file
		 */
		private SQLiteConnection CreateDatabase()
		{
			SQLiteConnection.CreateFile(database);

			SQLiteConnection dbConnection = new SQLiteConnection("Data Source=" + database + ";Version=3;");
			dbConnection.Open();

			string sql = "CREATE TABLE \"Results\" ( \"ID\" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, " +
				"\"UserID\" INTEGER NOT NULL, \"FileName\" TEXT NOT NULL," +
				"\"DateUploaded\" TEXT NOT NULL, " +
				"\"ResultObject\" TEXT NOT NULL );" +
				"CREATE TABLE \"User\" (\"UserID\" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT," +
				"\"Email\" TEXT NOT NULL UNIQUE," +
				"\"Name\" TEXT NOT NULL, " +
				"\"Password\" TEXT NOT NULL, " +
				"\"LastLogin\" TEXT, " +
				"\"Role\" INTEGER NOT NULL DEFAULT 0," +
				"\"Cookie\" TEXT UNIQUE);" +
				"INSERT INTO \"User\" (\"UserID\", \"Email\", \"Name\"," +
				"\"Password\", \"LastLogin\", \"Role\") VALUES ('1', 'email@email.com'," +
				" 'admin', 'jR1qKO9qXceNTj2/RxRjzd4FgMB6WWcBd6Y6+B5Ts8rwE/Tw', '', '1');";
			SQLiteCommand command = new SQLiteCommand(sql, dbConnection);
			try
			{
				command.ExecuteNonQuery();
			}
			catch (Exception e)
			{
				dbConnection.Close();
			}
			dbConnection.Close();
			return dbConnection;

		}
		/*
		 * NOTE: All calls to method must have error handling
		 * @purpose
		 * Creates an open connection to database
		 * @return
		 * SQLiteConnection object reresenting connnection to database file
		 */
		public SQLiteConnection ConnectToDatabase()
		{
			SQLiteConnection dbConnection;
			if (IsDbExists(database))
			{
				dbConnection = new SQLiteConnection("Data Source=" + database + ";Version=3;");
				return dbConnection;
			}
			else
			{
				dbConnection = CreateDatabase();
				return dbConnection;
			}
		}
		/*
		* GetUserList()
		* @purpose
		* Return a List of String Arrays
		* Each Array contains the User ID, Name and Email to allow an admin to identify
		* which user they will investigte further
		* @param
		* SQLiteConnection dbConnection
		* @return
		* List<string[]> objects
		*
		* NOTE: All calls to method must have error handling
		*/
		public List<string[]> GetUserList(SQLiteConnection dbConnection)
		{
			dbConnection.Open();
			string sql = "SELECT UserID, Name, Email FROM User ORDER BY UserID DESC";
			SQLiteCommand command = new SQLiteCommand(sql, dbConnection);
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
			dbConnection.Close();
			return userList;
		}

		/*
		* AddUser(User user)
		* @purpose
		* Add record to User table
		* @param int userID
		* User object representing user to be added to User Table
		* SQLiteConnection dbConnection database connection
		* NOTE: All calls to method must have error handling
		*/
		public void AddUser(User user, SQLiteConnection dbConnection)
		{
			dbConnection.Open();
			string sql = "INSERT INTO User (Email, Name, Password, Role) VALUES (?,?,?,?)";

			SQLiteParameter nameParam = new SQLiteParameter();
			SQLiteParameter emailParam = new SQLiteParameter();
			SQLiteParameter passwordParam = new SQLiteParameter();
			SQLiteParameter roleParam = new SQLiteParameter();

			using (SQLiteTransaction sqlTransaction = dbConnection.BeginTransaction())
			{
				SQLiteCommand command = new SQLiteCommand(sql, dbConnection);
				command.Parameters.Add(emailParam);
				command.Parameters.Add(nameParam);
				command.Parameters.Add(passwordParam);
				command.Parameters.Add(roleParam);

				nameParam.Value = user.Name;
				emailParam.Value = user.Email;
				passwordParam.Value = user.Password;
				roleParam.Value = user.Role;

				command.ExecuteNonQuery();
				sqlTransaction.Commit();
			}
			dbConnection.Close();
		}
		/*
		* UpdateUser(User user)
		* @purpose
		* Update record to User table
		* @param int userID, SQLiteConnection dbConnection
		* User object representing user to be updated in User Table
		* SQLiteConnection dbConnection database connection
		*
		* NOTE: All calls to method must have error handling
		*/
		public void UpdateUser(User user, SQLiteConnection dbConnection)
		{
			dbConnection.Open();
			string sql = "UPDATE User SET Email = ?, Name = ?, Password = ?, Role = ? WHERE UserID = ?";

			SQLiteParameter nameParam = new SQLiteParameter();
			SQLiteParameter emailParam = new SQLiteParameter();
			SQLiteParameter passwordParam = new SQLiteParameter();
			SQLiteParameter roleParam = new SQLiteParameter();
			SQLiteParameter idParam = new SQLiteParameter();

			using (SQLiteTransaction sqlTransaction = dbConnection.BeginTransaction())
			{
				SQLiteCommand command = new SQLiteCommand(sql, dbConnection);

				command.Parameters.Add(emailParam);
				command.Parameters.Add(nameParam);
				command.Parameters.Add(passwordParam);
				command.Parameters.Add(roleParam);
				command.Parameters.Add(idParam);

				nameParam.Value = user.Name;
				emailParam.Value = user.Email;
				passwordParam.Value = user.Password;
				roleParam.Value = user.Role;
				idParam.Value = user.UserID;

				command.ExecuteNonQuery();
				sqlTransaction.Commit();
			}
			dbConnection.Close();
		}
		/*
		* DeleteUser(int userID)
		* @purpose
		* Remove record associated with a given UserID from User table
		* @param int userID, SQLiteConnection dbConnection
		* UserID of record to be deleted
		* SQLiteConnection dbConnection database connection
		* NOTE: All calls to method must have error handling
		*/

		public void DeleteUser(int userID, SQLiteConnection dbConnection)
		{
			dbConnection.Open();
			string sql = "DELETE FROM User WHERE UserID=?";
			SQLiteParameter idParam = new SQLiteParameter();
			using (SQLiteTransaction sqlTransaction = dbConnection.BeginTransaction())
			{
				SQLiteCommand command = new SQLiteCommand(sql, dbConnection);

				command.Parameters.Add(idParam);

				idParam.Value = userID;

				command.ExecuteNonQuery();
				sqlTransaction.Commit();
			}
			dbConnection.Close();
		}

		/*
		 * SaveResults(int userID, Results result, SQLiteConnection dbConnection)
		 * @purpose
		 * Write Results object associated wiht UserID to Results table
		 * @param int userID, Results result
		 * Integer value identifying UserID in User table
		 * Results object created from scan
		 * SQLiteConnection dbConnection
		 * NOTE: All calls to method must have error handling
		 */
		public void SaveResults(int userID, Results result, SQLiteConnection dbConnection)
		{
			dbConnection.Open();
			string sql = "INSERT INTO Results (UserID, DateUploaded, FileName, ResultObject) VALUES (?,?,?,?)";
			SQLiteParameter idParam = new SQLiteParameter();
			SQLiteParameter dateParam = new SQLiteParameter();
			SQLiteParameter fileParam = new SQLiteParameter();
			SQLiteParameter resultParam = new SQLiteParameter();
			using (SQLiteTransaction sqlTransaction = dbConnection.BeginTransaction())
			{
				SQLiteCommand command = new SQLiteCommand(sql, dbConnection);

				command.Parameters.Add(idParam);
				command.Parameters.Add(dateParam);
				command.Parameters.Add(fileParam);
				command.Parameters.Add(resultParam);

				idParam.Value = userID;
				dateParam.Value = DateTime.Now.ToString("dd/MM/yyyy hh-mm-ss");

				// TODO: Add filename to Results class
				fileParam.Value = "Scan"+ DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss");
				resultParam.Value = SerializeResult(result);

				command.ExecuteNonQuery();
				sqlTransaction.Commit();
			}
			dbConnection.Close();
		}

		/*
		 * GetResultsList(int userID, SQLiteConnection dbConnection)
		 * @purpose
		 * Return a List of String Arrays, each string represents a Result stored for a given user
		 * Each Array contains the Result ID, FileName and DateUploaded to allow a user to identify
		 * which function the will investigte further
		 * @param int userID
		 * Integer value identifying UserID in Results table
		 * SQLiteConnection dbConnection database connection
		 * @return
		 * List<string[]> Object
		 *
		 * NOTE: All calls to method must have error handling
		 */
		public List<string[]> GetResultsList(int userID, SQLiteConnection dbConnection)
		{
			dbConnection.Open();
			string sql = "SELECT ID, FileName, DateUploaded FROM Results WHERE UserID=?";
			SQLiteParameter idParam = new SQLiteParameter();
			SQLiteCommand command = new SQLiteCommand(sql, dbConnection);

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
			dbConnection.Close();
			return resultList;
		}

		/*
		 * GetResult(int id, SQLiteConnection dbConnection)
		 * @purpose
		 * Return a Results object used to populate fields in Reports and Dashboard screens
		 * @param int id, SQLiteConnection dbConnection
		 * Integer value identifying ID in Results table
		 * SQLiteConnection dbConnection database connection
		 * @return
		 * Results object
		 *
		 * NOTE: All calls to method must have error handling
		 */
		public Results GetResult(int id, SQLiteConnection dbConnection)
		{
			dbConnection.Open();
			string sql = "SELECT ResultObject FROM Results WHERE ID=?";
			SQLiteParameter idParam = new SQLiteParameter();
			SQLiteCommand command = new SQLiteCommand(sql, dbConnection);

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
			dbConnection.Close();
			return result;
		}

		/*
		 * GetUserFromDatabase(int userID, SQLiteConnection dbConnection)
		 * @purpose
		 * Return a User object used to populate fields on Profile screen
		 * @param int userID
		 * Integer value identifying UserID in User table
		 * SQLiteConnection dbConnection dtabase connection
		 * @return
		 * User object
		 *
		 * NOTE: All calls to method must have error handling
		 */
		public User GetUserFromDatabase(int userID, SQLiteConnection dbConnection)
		{
			dbConnection.Open();
			User user = new User();
			string sql = "SELECT * FROM User WHERE UserID = ?";
			SQLiteParameter idParam = new SQLiteParameter();
			SQLiteCommand command = new SQLiteCommand(sql, dbConnection);

			command.Parameters.Add(idParam);

			idParam.Value = userID;
			SQLiteDataReader results = command.ExecuteReader();
			user = QueryToUser(results);
			dbConnection.Close();
			return user;
		}
		/*
		 * [Overload]
		 * GetUserFromDatabase(string email, SQLiteConnection dbConnection)
		 * @purpose
		 * Return a User object used to populate fields on Profile screen
		 * @param string email
		 * String value identifying Email in User table
		 * SQLiteConnection dbConnection dtabase connection
		 * @return
		 * User object
		 *
		 * NOTE: All calls to method must have error handling
		 */
		public User GetUserFromDatabase(string email, SQLiteConnection dbConnection)
		{
			dbConnection.Open();
			User user = new User();
			string sql = "SELECT * FROM User WHERE Email = ?";
			SQLiteParameter emailParam = new SQLiteParameter();
			SQLiteCommand command = new SQLiteCommand(sql, dbConnection);

			command.Parameters.Add(emailParam);

			emailParam.Value = email;
			SQLiteDataReader results = command.ExecuteReader();
			user = QueryToUser(results);
			dbConnection.Close();
			return user;
		}
		/*
		 * ValidatePassword(string password, string hashPassword)
		 * @purpose
		 * Validate a password provided by user to a stored password
		 * @param string password, string hashPassword
		 * string password password provided by user
		 * string hashPassword password stored in database
		 * @return
		 * bool true is match otherwise false
		 */
		private bool ValidatePassword(string password, string hashPassword)
		{
			byte[] saltedHash = Convert.FromBase64String(hashPassword);
			byte[] salt = new byte[16];

			Array.Copy(saltedHash, 0, salt, 0, 16);

			Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);
			byte[] hash = pbkdf2.GetBytes(20);
			bool match = true;

			for (int i = 0; i < 20; ++i)
			{
				match &= saltedHash[i + 16] == hash[i];
			}

			return match;
		}
		/*
		 * AuthenticateUser(string email, string password, SQLiteConnection dbConnection)
		 * @purpose
		 * Verify a provided password matches a stored hash
		 * @param string email, string password, SQLiteConnection dbConnection
		 * String value identifying Email in User table
		 * String value password submitted by user
		 * SQLiteConnection dbConnection database connection
		 * @return
		 * Boolean true or false
		 *
		 * NOTE: All calls to method must have error handling
		 */
		public bool AuthenticateUser(string email, string password, SQLiteConnection dbConnection)
		{
			dbConnection.Open();
			string sql = "SELECT Password FROM User WHERE Email = @email";
			SQLiteCommand command = new SQLiteCommand(sql, dbConnection);

			command.Parameters.AddWithValue("@email", email);

			User user = new User();
			SQLiteDataReader results = command.ExecuteReader();

			results.Read();

			string hash = results["Password"].ToString();

			if (ValidatePassword(password, results["Password"].ToString()))
			{
				results.Close();
				dbConnection.Close();
				return true;
			}
			results.Close();
			dbConnection.Close();
			return false;
		}
		/*
		 * GetLastLogin(int userID, SQLiteConnection dbConnection)
		 * @purpose
		 * Retreive the last login from User table
		 * @param int userID, SQLiteConnection dbConnection
		 * Integer value identifying ID in Results table
		 * SQLiteConnection dbConnection database connection
		 * @return
		 * String date of last login
		 */
		public string GetLastLogin(int userID, SQLiteConnection dbConnection)
		{
			dbConnection.Open();
			string sql = "SELECT LastLogin FROM User WHERE UserID = @id";
			SQLiteCommand command = new SQLiteCommand(sql, dbConnection);

			command.Parameters.AddWithValue("@id", userID);

			SQLiteDataReader results = command.ExecuteReader();

			results.Read();
			string lastLogin = results["LastLogin"].ToString();
			results.Close();
			dbConnection.Close();

			return lastLogin;
		}
		/*
		 * GetTotalScans(int userID, SQLiteConnection dbConnection)
		 * @purpose
		 * Retreive a count of all scans performed by the user from
		 * Results table
		 * @param int userID, SQLiteConnection dbConnection
		 * Integer value identifying ID in Results table
		 * SQLiteConnection dbConnection database connection
		 * @return
		 * Integer number of scans
		 */
		public int GetTotalScans(int userID, SQLiteConnection dbConnection)
		{
			dbConnection.Open();
			string sql = "SELECT COUNT(ID) FROM REsults WHERE UserID = @id";
			SQLiteCommand command = new SQLiteCommand(sql, dbConnection);

			command.Parameters.AddWithValue("@id", userID);

			SQLiteDataReader results = command.ExecuteReader();

			results.Read();
			int totalScans = results.GetInt32(0);
			results.Close();
			dbConnection.Close();
			return totalScans;
		}

		/*
		 * SerializeResult(Results results)
		 * @purpose
		 * Serialize Results object to JSON object
		 * @param Results results
		 * Results object to be serialized
		 */
		string SerializeResult(Results results)
		{
			MemoryStream ms = new MemoryStream();
			DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(Results));
			ser.WriteObject(ms, results);
			byte[] json = ms.ToArray();

			ms.Close();

			return Encoding.UTF8.GetString(json, 0, json.Length);
		}
		/*
		 * QueryToUser(SQLiteDataReader query)
		 * @purpose
		 * Create a user from a retrived database row from the User table
		 * @param SQLiteDataReader query
		 * SQLiteDataReader object to containing all retrieved rows
		 */
		User QueryToUser(SQLiteDataReader query)
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
		 * RecordLogin(int userID,  SQLiteConnection dbConnection)
		 * @purpose
		 * Update the LastLogin field when a user signs in
		 * @param int userID, SQLiteConnection dbConnection
		 * Integer value identifying ID in Results table
		 * SQLiteConnection dbConnection database connection
		 */
		public void RecordLogin(string email, SQLiteConnection dbConnection)
		{
			dbConnection.Open();
			string sql = "UPDATE User SET LastLogin = @today WHERE Email = @email";
			using (SQLiteTransaction sqlTransaction = dbConnection.BeginTransaction())
			{
				SQLiteCommand command = new SQLiteCommand(sql, dbConnection);

				command.Parameters.AddWithValue("@today", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"));
				command.Parameters.AddWithValue("@email", email);

				command.ExecuteNonQuery();
				sqlTransaction.Commit();
			}
			dbConnection.Close();
		}
		/*
		 * StoreCookie(int userID, string cookie, SQLiteConnection dbConnection)
		 * @purpose
		 * Store a generate cookie into the User database
		 * @param int userID, string cookie, SQLiteConnection dbConnection
		 * Integer value identifying ID in Results table
		 * string cookie cookie value to be recorded
		 * SQLiteConnection dbConnection database connection
		 */
		public void StoreCookie(int userID, string cookie, SQLiteConnection dbConnection)
		{
			dbConnection.Open();
			string sql = "UPDATE  User SET Cookie = @cookie WHERE UserID = @id";
			using (SQLiteTransaction sqlTransaction = dbConnection.BeginTransaction())
			{
				SQLiteCommand command = new SQLiteCommand(sql, dbConnection);

				command.Parameters.AddWithValue("@cookie", cookie);
				command.Parameters.AddWithValue("@id", userID);

				command.ExecuteNonQuery();
				sqlTransaction.Commit();
			}
			dbConnection.Close();
		}
		/*
		 * ValidateCookie(string cookie, SQLiteConnection dbConnection)
		 * @purpose
		 * Identify the UserID of the user with given cookie
		 * @paramstring cookie, SQLiteConnection dbConnection
		 * string cookie value of the given cookie
		 * SQLiteConnection dbConnection connection to database
		 * @return
		 * Integer identifying the UserID
		 * if no user found return -1
		 */
		public int ValidateCookie(string cookie, SQLiteConnection dbConnection)
		{
			int userID;
			dbConnection.Open();
			string sql = "SELECT UserID From User WHERE Cookie = @cookie";
			SQLiteCommand command = new SQLiteCommand(sql, dbConnection);

			command.Parameters.AddWithValue("@cookie", cookie);

			SQLiteDataReader results = command.ExecuteReader();
			try
			{
				results.Read();
				userID = results.GetInt32(0);
				results.Close();
				dbConnection.Close();
			}
			catch (Exception e)
			{
				dbConnection.Close();
				userID = -1;
			}

			return userID;
		}
	}
}
