using System;

using System.Data.SqlClient;
using System.Text;

namespace rapid_moose
{
    public class User
    {
        public string email { get; set; }
        public string password { get; set; }

        public static void CreateUser(string email, string hashedPassword)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO users (user_email, user_password, user_time_created) ");
            sb.Append($"VALUES ('{email}', '{hashedPassword}','{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}') ");
            String sql = sb.ToString();

            StringBuilder result = new StringBuilder();
            SqlConnectionStringBuilder connectionStringBuilder = Program.BuildConnection();

            using (SqlConnection connection = new SqlConnection(connectionStringBuilder.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public static int CheckUser(string email, string hashedPassword)
        {
            StringBuilder checkExists = new StringBuilder();
            checkExists.Append("SELECT 1");
            checkExists.Append("FROM users ");
            checkExists.Append($"WHERE user_email = '{email}'");
            String checkExistSql = checkExists.ToString();

            StringBuilder getUser = new StringBuilder();
            getUser.Append("SELECT * ");
            getUser.Append("FROM users ");
            getUser.Append($"WHERE user_email = '{email}'");
            String getUserSql = getUser.ToString();

            string password = "";
            int userId = -1;

            SqlConnectionStringBuilder connectionStringBuilder = Program.BuildConnection();

            using (SqlConnection connection = new SqlConnection(connectionStringBuilder.ConnectionString))
            {
                int exists = -1;
                using (SqlCommand command = new SqlCommand(checkExistSql, connection))
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            exists = reader.GetInt32(0);
                        }
                    }
                }

                if (exists == 1)
                {
                    using (SqlCommand command = new SqlCommand(getUserSql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                password = reader["user_password"].ToString();
                                userId = (int)reader["user_id"];
                            }
                        }
                    }
                }
            }

            if (password == hashedPassword)
            {
                return (userId);
            }
            else
            {
                return (-1);
            }
        }

        public static string GetIconName(int userId)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT user_icon FROM users ");
            sb.Append($"WHERE user_id='{userId}' ");
            String sql = sb.ToString();

            StringBuilder result = new StringBuilder();
            SqlConnectionStringBuilder connectionStringBuilder = Program.BuildConnection();

            using (SqlConnection connection = new SqlConnection(connectionStringBuilder.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            return (reader.GetString(0));
                        }
                    }
                }
            }

            return ("error");
        }

        public static void SetIconName(int userId, string iconName)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE users ");
            sb.Append($"SET user_icon='{iconName}' ");
            sb.Append($"WHERE user_id='{userId}' ");
            String sql = sb.ToString();

            StringBuilder result = new StringBuilder();
            SqlConnectionStringBuilder connectionStringBuilder = Program.BuildConnection();

            using (SqlConnection connection = new SqlConnection(connectionStringBuilder.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public static bool CheckEmailUnused(string email)
        {
            StringBuilder checkExists = new StringBuilder();
            checkExists.Append("SELECT 1");
            checkExists.Append("FROM users ");
            checkExists.Append($"WHERE user_email = '{email}'");
            String checkExistSql = checkExists.ToString();

            SqlConnectionStringBuilder connectionStringBuilder = Program.BuildConnection();

            using (SqlConnection connection = new SqlConnection(connectionStringBuilder.ConnectionString))
            {
                int exists = -1;
                using (SqlCommand command = new SqlCommand(checkExistSql, connection))
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            exists = reader.GetInt32(0);
                        }
                    }
                }

                if (exists == 1)
                {
                    return (false);
                }
            }
            return (true);
        }
    }
}
