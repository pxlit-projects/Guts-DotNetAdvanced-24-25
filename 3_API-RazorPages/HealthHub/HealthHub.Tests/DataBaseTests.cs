using HealthHub.Domain;
using HealthHub.Infrastructure;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthHub.Tests
{
    internal class DataBaseTests:IDisposable
    {
        private SqliteConnection _connection;
        private string? _migrationError;

        [OneTimeSetUp]
        public void CreateDatabase()
        {
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            using (HealthHubDbContext context = CreateDbContext(false))
            {
                //Check if migration succeeds
                try
                {
                    context.Database.Migrate();
                    Specialty firstSpecialty = context.Set<Specialty>().First();
                }
                catch (Exception e)
                {
                    var messageBuilder = new StringBuilder();
                    messageBuilder.AppendLine("The migration (creation) of the database is not configured properly.");
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

        internal HealthHubDbContext CreateDbContext(bool assertMigration = true)
        {
            if (assertMigration)
            {
                AssertMigratedSuccessfully();
            }

            var options = new DbContextOptionsBuilder<HealthHubDbContext>()
                .UseSqlite(_connection)
                .Options;

            return new HealthHubDbContext(options);
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

