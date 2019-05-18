using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SQLite;
using System.IO;

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
        public List<string[]> GetUserList()
        {
            string sql = "SELECT UserID, Name, Email FROM User ORDER BY UserID DESC";
            SQLiteCommand command = new SQLiteCommand(sql, DBConnection);
            SQLiteDataReader results = command.ExecuteReader();
            string[] user;
            List<string[]> userList = new List<string[]>();
            while (results.Read())
            {
                user = new string[] { results["UserID"].ToString(), results["Name"].ToString() };
                userList.Add(user);
            }

            return userList;
        }
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
        public void DeleteUser(User user)
        {
            string sql = "DELETE FROM User WHERE UserID=?";

            SQLiteParameter idParam = new SQLiteParameter();

            SQLiteCommand command = new SQLiteCommand(sql, DBConnection);

            command.Parameters.Add(idParam);

            idParam.Value = user.UserID;

            command.ExecuteNonQuery();
        }
        public void GetResults(User user)
        {
            string sql = "SELECT ResultsObject FROM Results WHERE UserID=?";

            SQLiteParameter idParam = new SQLiteParameter();

            SQLiteCommand command = new SQLiteCommand(sql, DBConnection);

            command.Parameters.Add(idParam);

            idParam.Value = user.UserID;

            SQLiteDataReader results = command.ExecuteReader();
        }
        private User GetUserFromDatabase(SQLiteDataReader results)
        {
            User user = new User();
            while(results.Read())
            {
                user.Email = results["Email"].ToString();
                user.Password = results["Password"].ToString();
                user.Name = results["Name"].ToString();
                user.UserID = results.GetInt32(0);
                user.Role = (UserRole)results.GetInt32(5);
            }
            return user;
        }
            
        public User AuthenticateUser(string email, string password)
        {
            string sql = "SELET * FROM User WHERE Email = ? AND Password = ?";

            SQLiteParameter emailParam = new SQLiteParameter();
            SQLiteParameter passwordParam = new SQLiteParameter();

            SQLiteCommand command = new SQLiteCommand(sql, DBConnection);

            command.Parameters.Add(emailParam);
            command.Parameters.Add(passwordParam);

            emailParam.Value = email;
            passwordParam.Value = password;

            SQLiteDataReader results = command.ExecuteReader();
            try
            {
                User user = GetUserFromDatabase(results);
                return user;
            }
            catch(Exception e)
            {
                User user = new User();
                return user;
            }
        }
    }
}
