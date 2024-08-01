using ExamCardSeller.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Npgsql;
using System.Globalization;
namespace ExamCardSeller.Extensions
{
   

    public static class DatabaseSetupExtensions
    {
        public static IHost EnsureDatabaseSetup(this IHost host, bool ensureDbCreated = false)
        {
            using var scope = host.Services.CreateScope();
            var serviceProvider = scope.ServiceProvider;
            try
            {
                var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
                if (ensureDbCreated)
                {
                    EnsureDbCreation(context);
                }
                context.Database.Migrate();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while migrating or seeding the database -{ex.Message}- {ex.StackTrace}");
                throw;
            }

            return host;
        }

        private static void EnsureDbCreation(ApplicationDbContext context)
        {
            try
            {
                if (context.Database.CanConnect())
                {
                    return;
                }

                var databaseName = context.Database.GetDbConnection().Database;
                var connectionString =
                    context.Database.GetConnectionString()!.Replace($"Database={databaseName};", "", true,
                        CultureInfo.InvariantCulture);

                using var connection = new NpgsqlConnection(connectionString);
                connection.Open();
                var cmd = connection.CreateCommand();
                cmd.CommandText = $"CREATE DATABASE \"{databaseName}\"";
                cmd.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception ex)
            {
                // Handle or log the exception as needed
                throw new Exception($"An error occurred while creating the database: {ex.Message}", ex);
            }
        }
    }

}
