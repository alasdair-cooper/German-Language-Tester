using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

using System.Data.SqlClient;

namespace rapid_moose
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        public static SqlConnectionStringBuilder BuildConnection()
        {
            SqlConnectionStringBuilder connectionStringBuilder = new SqlConnectionStringBuilder(Environment.GetEnvironmentVariable("SQLAZURECONNSTR_AZURE_DATABASE_CONNECTION_STRING"));

            return (connectionStringBuilder);
        }
    }
}
