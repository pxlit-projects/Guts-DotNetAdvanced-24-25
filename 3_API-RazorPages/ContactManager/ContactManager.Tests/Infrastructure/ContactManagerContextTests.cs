using ContactManager.Domain;
using ContactManager.Infrastructure;
using Guts.Client.Core;
using Guts.Client.Core.TestTools;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;

namespace ContactManager.Tests.Infrastructure
{
    [ExerciseTestFixture("dotnet2", "4-RAZORWEBAPI", "ContactManager",
    @"ContactManager.Infrastructure\ContactManagerDbContext.cs")]
    internal class ContactManagerContextTests:DatabaseTestBase
    {
        private string _contactManagerContextClassContent = null!;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _contactManagerContextClassContent = Solution.Current.GetFileContent(@"ContactManager.Infrastructure\ContactManagerDbContext.cs");
        }

        [MonitoredTest]
        public void _01_ShouldHave2DbSets()
        {
            var properties = GetDbSetProperties();

            Assert.That(properties, Has.Count.EqualTo(2), "There should be exactly 2 'DbSet' properties.");
            Assert.That(properties,
                Has.One.Matches((PropertyDeclarationSyntax p) => p.Type.ToString() == "DbSet<Contact>"),
                "There should be one 'DbSet' for contacts.");
            Assert.That(properties,
                Has.One.Matches((PropertyDeclarationSyntax p) => p.Type.ToString() == "DbSet<Company>"),
                "There should be one 'DbSet' for companies.");
        }

        [MonitoredTest]
        public void _02_OnModelCreating_ShouldSeedCompaniesAndContacts()
        {
            using var context = CreateDbContext();
            var seededCompanies = context.Set<Company>().ToList();
            var seededContacts = context.Set<Contact>().ToList();
            Assert.That(seededCompanies, Has.Count.GreaterThanOrEqualTo(2), "The database must be seeded wit at least 2 companies. " +
                                                                            $"Now the database contains {seededCompanies.Count} companies after creation.");
            AssertContact(seededContacts, "Johnson");
            AssertContact(seededContacts, "Doe");
            AssertCompany(seededCompanies, "TechCo");
            AssertCompany(seededCompanies, "Widgets Inc.");
        }

        [MonitoredTest]
        public void _03_OnModelCreating_ShouldConfigureTheRelationBetweenCompaniesAndContacts()
        {
            using var context = CreateDbContext();
            var companyEntityType = context.Model.FindEntityType(typeof(Contact).FullName!);
            var contactFK = companyEntityType?.GetForeignKeys().FirstOrDefault(fk => fk.PrincipalEntityType.ClrType == typeof(Company));
            Assert.That(contactFK, Is.Not.Null,
                "No foreign key relation found between Company and Contact.");
        }

        private void AssertCompany(IList<Company> companies, string companyName)
        {
            Assert.That(companies.Any(c => c.Name == companyName), Is.True,
                $"Company '{companyName}' is not seeded.");
        }

        private void AssertContact(IList<Contact> contacts, string contactName)
        {
            Assert.That(contacts.Any(c => c.Name == contactName), Is.True,
                $"Contact '{contactName}' is not seeded.");
        }

        private IList<PropertyDeclarationSyntax> GetDbSetProperties()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(_contactManagerContextClassContent);
            var root = syntaxTree.GetRoot();
            var properties = root
                .DescendantNodes()
                .OfType<PropertyDeclarationSyntax>()
                .Where(p =>
                {
                    if (!(p.Type is GenericNameSyntax genericName)) return false;
                    return genericName.Identifier.ValueText == "DbSet";
                });

            return properties.ToList();
        }

    }
}
