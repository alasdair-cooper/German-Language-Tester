using System;

using System.Data.SqlClient;
using System.Text;

namespace rapid_moose
{
    public class Admin
    {
        public static string GetSessions()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT * FROM sessions ");
            sb.Append("FOR JSON AUTO");
            String sql = sb.ToString();

            return (GetSqlJson(sql));
        }

        public static string GetUsers()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT * FROM users ");
            sb.Append("FOR JSON AUTO");
            String sql = sb.ToString();

            return (GetSqlJson(sql));
        }

        public static string GetSqlJson(string sql)
        {
            try
            {
                StringBuilder result = new StringBuilder();
                SqlConnectionStringBuilder connectionStringBuilder = Program.BuildConnection();

                using (SqlConnection connection = new SqlConnection(connectionStringBuilder.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (!reader.HasRows)
                            {
                                result.Append("[]");
                            }
                            else
                            {
                                while (reader.Read())
                                {
                                    result.Append(reader.GetValue(0).ToString().Trim());
                                }
                            }
                        }
                    }
                }
                return (result.ToString().Replace("\\", ""));
            }
            catch (SqlException e)
            {
                return (e.ToString());
            }
        }
    }
}
