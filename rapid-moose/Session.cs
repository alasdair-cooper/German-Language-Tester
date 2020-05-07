using System;

using System.Data.SqlClient;
using System.Text;

namespace rapid_moose
{
    public class Session
    {
        public static int CreateSession(int userID, string ip)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO sessions (session_start, session_end, session_expired, session_user_id, session_ip) ");
            sb.Append($"VALUES ('{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}', '{DateTime.Now.AddHours(3):yyyy-MM-dd HH:mm:ss.fff}','no', '{userID}', '{ip}') ");
            sb.Append("SELECT SCOPE_IDENTITY()");
            string sql = sb.ToString();

            SqlConnectionStringBuilder connectionStringBuilder = Program.BuildConnection();

            int sessionID = -1;

            using (SqlConnection connection = new SqlConnection(connectionStringBuilder.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        reader.Read();
                        sessionID = (int)reader.GetDecimal(0);
                    }
                }
            }
            return (sessionID);
        }

        public static bool CheckSession(int sessionID)
        {
            StringBuilder checkExists = new StringBuilder();
            checkExists.Append("SELECT COUNT(1) ");
            checkExists.Append("FROM sessions ");
            checkExists.Append($"WHERE session_ID = '{sessionID}' AND session_expired = 'no'");
            string checkExistSql = checkExists.ToString();

            SqlConnectionStringBuilder connectionStringBuilder = Program.BuildConnection();

            int exists = -1;

            using (SqlConnection connection = new SqlConnection(connectionStringBuilder.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(checkExistSql, connection))
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        reader.Read();
                        exists = reader.GetInt32(0);
                    }
                }
            }

            if (exists == 1)
            {
                return (true);
            }
            else
            {
                return (false);
            }
        }

        public static void ExpireSessions()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE sessions ");
            sb.Append("SET session_expired='yes' ");
            sb.Append($"WHERE session_expired = 'no' AND session_end < '{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}' AND session_id != 300");
            string sql = sb.ToString();

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

        public static int LookUpUserId(int sessionId)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT session_user_id ");
            sb.Append("FROM sessions ");
            sb.Append($"WHERE session_id='{sessionId}'");
            string sql = sb.ToString();

            SqlConnectionStringBuilder connectionStringBuilder = Program.BuildConnection();

            int userId = -1;
            using (SqlConnection connection = new SqlConnection(connectionStringBuilder.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        reader.Read();
                        userId = (int) reader["session_user_id"];
                    }
                }
            }
            return (userId);
        }
    }
}
