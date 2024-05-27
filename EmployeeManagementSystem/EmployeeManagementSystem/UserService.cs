using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarsManagementSystem
{
    public class UserService
    {
        private string connectionString = "Data Source=localhost;Initial Catalog=CarSearchSystem;Integrated Security=True";

        public User Login(string username, string password)
        {
            using (SqlConnection connect = new SqlConnection(connectionString))
            {
                string query = "SELECT Username, Role FROM Users WHERE Username = @username AND Password = @password";
                SqlCommand cmd = new SqlCommand(query, connect);
                cmd.Parameters.AddWithValue("@username", username);
                cmd.Parameters.AddWithValue("@password", password);

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable table = new DataTable();
                adapter.Fill(table);

                if (table.Rows.Count > 0)
                {
                    DataRow row = table.Rows[0];
                    return new User
                    {
                        Username = row["Username"].ToString(),
                        Role = row["Role"].ToString()
                    };
                }

                return null;
            }
        }

        public bool Register(string username, string password)
        {
            using (SqlConnection connect = new SqlConnection(connectionString))
            {
                connect.Open();

                string checkQuery = "SELECT COUNT(ID) FROM Users WHERE Username = @username";
                SqlCommand checkCmd = new SqlCommand(checkQuery, connect);
                checkCmd.Parameters.AddWithValue("@username", username);
                int count = (int)checkCmd.ExecuteScalar();

                if (count > 0)
                {
                    return false;
                }

                string insertQuery = "INSERT INTO Users (Username, Password, Role) VALUES (@username, @password, @role)";
                SqlCommand insertCmd = new SqlCommand(insertQuery, connect);
                insertCmd.Parameters.AddWithValue("@username", username);
                insertCmd.Parameters.AddWithValue("@password", password);
                insertCmd.Parameters.AddWithValue("@role", "user");
                insertCmd.ExecuteNonQuery();

                return true;
            }
        }
    }

}
