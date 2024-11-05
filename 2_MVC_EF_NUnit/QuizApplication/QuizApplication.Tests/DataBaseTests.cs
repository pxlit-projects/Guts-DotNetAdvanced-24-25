using Microsoft.AspNetCore.Authentication;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using QuizApplication.Domain;
using QuizApplication.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizApplication.Tests.Infrastructure
{
    public class DataBaseTests: IDisposable
    {
        private SqliteConnection _connection;
        private string? _migrationError;

        [OneTimeSetUp]
        public void CreateDatabase()
        {
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            using (QuizDbContext context = CreateDbContext(false))
            {
                //Check if migration succeeds
                try
                {
                    context.Database.Migrate();
                    Question firstQuestion = context.Set<Question>().First();
                }
                catch (Exception e)
                {
                    var messageBuilder = new StringBuilder();
                    messageBuilder.AppendLine("The migration (creation) of the database is not configured properly.");
                    messageBuilder.AppendLine("Remember to set the maximum length of the strings in Answer and Question (see README on github)");
                    messageBuilder.AppendLine(e.Message);
                    _migrationError = messageBuilder.ToString();
                }
            }
        }

        public void Dispose()
        {
            _connection?.Dispose();
        }

        [OneTimeTearDown]
        public void DropDatabase()
        {
            using (var context = CreateDbContext(false))
            {
                context.Database.EnsureDeleted();
            }
            _connection?.Close();
        }

        internal QuizDbContext CreateDbContext(bool assertMigration = true)
        {
            if (assertMigration)
            {
                AssertMigratedSuccessfully();
            }

            var options = new DbContextOptionsBuilder<QuizDbContext>()
                .UseSqlite(_connection)
                .Options;

            return new QuizDbContext(options);
        }

        private void AssertMigratedSuccessfully()
        {
            if (!string.IsNullOrEmpty(_migrationError))
            {
                Assert.Fail(_migrationError);
            }
        }
    }
}
