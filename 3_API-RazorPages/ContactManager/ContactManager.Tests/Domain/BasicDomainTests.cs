using System.ComponentModel.DataAnnotations;
using ContactManager.Domain;
using System.Reflection;
using Guts.Client.Core;

namespace ContactManager.Tests.Domain
{
    [ExerciseTestFixture("dotnet2", "4-RAZORWEBAPI", "ContactManager",
    @"ContactManager.Domain\Company.cs;ContactManager.Domain\Contact.cs")]
    internal class BasicDomainTests
    {
        [MonitoredTest]
        public void _01_Company_ShouldUseDataAnnotationsThatAreUsedDuringModelBinding()
        {
            Type companyType = typeof(Company);

            PropertyInfo nameProperty = companyType.GetProperty(nameof(Company.Name))!;
            AssertMaxLength(nameProperty, 100);
            PropertyInfo addressProperty = companyType.GetProperty(nameof(Company.Address))!;
            AssertMaxLength(addressProperty, 100);
            PropertyInfo zipProperty = companyType.GetProperty(nameof(Company.Zip))!;
            AssertMaxLength(zipProperty, 10);
            PropertyInfo cityProperty = companyType.GetProperty(nameof(Company.City))!;
            AssertMaxLength(cityProperty, 50);
        }

        [MonitoredTest]
        public void _02_Contact_ShouldUseDataAnnotationsThatAreUsedDuringModelBinding()
        {
            Type contactType = typeof(Contact);

            PropertyInfo nameProperty = contactType.GetProperty(nameof(Contact.Name))!;
            AssertMaxLength(nameProperty, 100);

            PropertyInfo firstNameProperty = contactType.GetProperty(nameof(Contact.FirstName))!;
            AssertMaxLength(firstNameProperty, 100);

            PropertyInfo emailProperty = contactType.GetProperty(nameof(Contact.Email))!;
            AssertMaxLength(emailProperty, 100);
            EmailAddressAttribute? emailAddressAttribute = emailProperty.GetCustomAttribute<EmailAddressAttribute>();
            Assert.That(emailAddressAttribute, Is.Not.Null,
                $"The {emailProperty.Name} property should be validated as an email address during ModelBinding");

            PropertyInfo phoneProperty = contactType.GetProperty(nameof(Contact.Phone))!;
            AssertMaxLength(phoneProperty, 20);
        }

        [MonitoredTest]
        public void _03_CompanyAndContact_ShouldDoNullChecksAndInitializations()
        {
            Company company = new Company();
            Assert.That(company.Name, Is.Not.Null, "The name of the company can not be null after constructing a Company.");
            Assert.That(company.Address, Is.Not.Null, "The name of the company can not be null after construction of a Company.");
            Assert.That(company.Zip, Is.Not.Null, "The zip of the company can not be null after constructing a Company.");
            Assert.That(company.City, Is.Not.Null, "The city of the company can not be null after constructing a Company.");
            Assert.That(company.Contacts, Is.Not.Null, "The contacts list should be initialised after constructing a Company.");


            Contact contact = new Contact();
            Assert.That(contact.Name, Is.Not.Null, "The name of the contact can not be null after constructing a Contact.");
            Assert.That(contact.FirstName, Is.Not.Null, "The firstname of the contact can not be null after constructing a Contact.");
            Assert.That(contact.Email, Is.Not.Null, "The email of the contact can not be null after constructing a Contact.");
            Assert.That(contact.Phone, Is.Not.Null, "The phone of the contact can not be null after constructing a Contact.");
        }

        private void AssertMaxLength(PropertyInfo property, int maxLength)
        {
            MaxLengthAttribute? maxLengthAttribute = property.GetCustomAttribute<MaxLengthAttribute>();

            Assert.That(maxLengthAttribute, Is.Not.Null,
                $"The {property.Name} property should have a maximum length that is used during ModelBinding");
            Assert.That(maxLengthAttribute!.Length, Is.EqualTo(maxLength), $"The maximum length of {property.Name} should be {maxLength}");
        }
    }
}
