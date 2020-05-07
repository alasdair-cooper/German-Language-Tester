using System;

using System.Data.SqlClient;
using System.Text;

namespace rapid_moose
{
    public class Data
    {
        public static string GetWords(string book, int chapter)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT * FROM German ");
            sb.Append("WHERE german_book='" + book.Substring(0, 2) + "' ");
            sb.Append("AND german_chapter='" + chapter + "' ");
            sb.Append("FOR JSON AUTO");
            String sql = sb.ToString();

            return (GetSqlJson(sql));
        }

        public static string GetChapters(string book)
        {
            if(book == null)
            {
                return (null);
            }

            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT german_chapter AS chapter, COUNT(german_chapter) AS word_count FROM german ");
            sb.Append("WHERE german_book='" + book.Substring(0, 2) + "' ");
            sb.Append("GROUP BY german_chapter ");
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
