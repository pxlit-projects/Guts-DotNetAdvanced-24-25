using ContactManager.AppLogic.Contracts;
using ContactManager.Pages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using System.Reflection;
using ContactManager.Domain;
using ContactManager.Pages.Contacts;
using Microsoft.AspNetCore.Mvc.Rendering;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using ContactManager.Pages.Companies;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Guts.Client.Core;

namespace ContactManager.Tests.Web
{
    [ExerciseTestFixture("dotnet2", "4-RAZORWEBAPI", "ContactManager",
        @"ContactManager\Pages\Contacts\AddContact.cshtml.cs")]
    public class AddContactPageModelTests
    {
        private Mock<IContactRepository> _contactRepositoryMock = null!;
        private Mock<ICompanyRepository> _companyRepositoryMock = null!;
        private List<Company> _allCompanies = null!;

        [SetUp]
        public void Setup()
        {
            _contactRepositoryMock = new Mock<IContactRepository>();

            _allCompanies =
            [
                new Company { Id = Random.Shared.Next(1, int.MaxValue), Name = Guid.NewGuid().ToString() },
                new Company { Id = Random.Shared.Next(1, int.MaxValue), Name = Guid.NewGuid().ToString() },
                new Company { Id = Random.Shared.Next(1, int.MaxValue), Name = Guid.NewGuid().ToString() }
            ];

            _companyRepositoryMock = new Mock<ICompanyRepository>();
            _companyRepositoryMock.Setup(repo => repo.GetAllCompanies()).Returns(_allCompanies);
        }

        [MonitoredTest]
        public void _01_ContactProperty_ShouldHaveBindPropertyAttribute()
        {
            var contactProperty = typeof(AddContactModel).GetProperty("Contact");
            Assert.That(contactProperty, Is.Not.Null, "The AddContactModel class must have a 'Contact' property");
            bool contactHasBindPropertyAttribute = contactProperty!.GetCustomAttributes(typeof(BindPropertyAttribute), false).Any();
            Assert.That(contactHasBindPropertyAttribute, Is.True, "The Contact property should have the [BindProperty] attribute.");
        }

        [MonitoredTest]
        public void _02_Constructor_ShouldInitializeAListOfCompanySelectListItems()
        {
            //Act
            var model = new AddContactModel(_companyRepositoryMock.Object, _contactRepositoryMock.Object);

            //Assert
            _companyRepositoryMock.Verify(repo => repo.GetAllCompanies(), Times.Once,
                "The injected company repository should be used to retrieve all companies");

            IList<SelectListItem> selectListItems = GetCompanySelectListItems(model);
            Assert.That(selectListItems.Count, Is.EqualTo(_allCompanies.Count + 1),
                "The number of SelectListItems should be the number of companies plus one (a 'Select a company' item)");

            foreach (Company company in _allCompanies)
            {
                SelectListItem? matchingSelectListItem = selectListItems.FirstOrDefault(li => li.Value == company.Id.ToString());
                Assert.That(matchingSelectListItem, Is.Not.Null,
                    $"Cannot find a SelectListItem with value {company.Id.ToString()}, the Id of one of the companies.");
                Assert.That(matchingSelectListItem!.Text, Is.EqualTo(company.Name),
                    $"The Text of the SelectListItems should be the name of a company.");
            }

            SelectListItem noCompanySelectListItem = selectListItems.First();
            Assert.That(noCompanySelectListItem.Value, Is.Empty, "The 'Value' of the first SelectListItem should be empty");
            Assert.That(noCompanySelectListItem.Text, Is.EqualTo("Select a company"),
                "The 'Text' of the first SelectListItem should be 'Select a company'");
        }

        [MonitoredTest]
        public void _03_Constructor_ShouldInitializeAnEmptyContact()
        {
            //Act
            var model = new AddContactModel(_companyRepositoryMock.Object, _contactRepositoryMock.Object);

            //Assert
            Assert.That(GetContact(model), Is.Not.Null);
        }

        [MonitoredTest]
        public void _04_OnGet_ShouldDoNothing()
        {
            var model = new AddContactModel(_companyRepositoryMock.Object, _contactRepositoryMock.Object);
            model.OnGet();
        }

        [MonitoredTest]
        public void _05_OnPost_ValidModelState_ShouldAddContactAndRedirectToIndexPage()
        {
            //Arrange
            var model = new AddContactModel(_companyRepositoryMock.Object, _contactRepositoryMock.Object);

            //Act
            RedirectToPageResult? result = model.OnPost() as RedirectToPageResult;

            //Assert
            Assert.That(result, Is.Not.Null, "A 'RedirectToPageResult' should be returned");
            Assert.That(result!.PageName == "/Index", Is.True, "Redirect is not correct");
            Contact contact = GetContact(model);
            _contactRepositoryMock.Verify(repo => repo.AddContact(contact), Times.Once,
                "The contact is not added correctly using the contact repository");
        }

        [MonitoredTest]
        public void _06_OnPost_InValidModelState_ShouldStayOnPage()
        {
            //Arrange
            var model = new AddContactModel(_companyRepositoryMock.Object, _contactRepositoryMock.Object);

            ModelStateDictionary? modelState = typeof(PageModel).GetProperty("ModelState", BindingFlags.Public | BindingFlags.Instance)?.GetValue(model) as ModelStateDictionary;
            modelState?.AddModelError("someError", "The Contact property is not valid");

            //Act
            PageResult? result = model.OnPost() as PageResult;

            //Assert
            Assert.That(result, Is.Not.Null, "A 'PageResult' should be returned");
            Assert.That(result!.Page, Is.Null, "A 'PageResult' with 'Page' null should be returned");

            _contactRepositoryMock.Verify(repo => repo.AddContact(It.IsAny<Contact>()), Times.Never,
                "The contact should not be added using the contact repository");
        }

        private IList<SelectListItem> GetCompanySelectListItems(AddContactModel pageModel)
        {
            PropertyInfo? companySelectListItemsProperty = typeof(AddContactModel).GetProperty("CompanySelectListItems");
            Assert.That(companySelectListItemsProperty, Is.Not.Null,
                "The AddContactModel class must have a 'CompanySelectListItems' property. " +
                "See https://learn.microsoft.com/en-us/aspnet/core/mvc/views/working-with-forms?view=aspnetcore-8.0#the-select-tag-helper");
            
            IList<SelectListItem>? value = companySelectListItemsProperty!.GetValue(pageModel) as IList<SelectListItem>;
            Assert.That(value, Is.Not.Null,
                "The 'CompanySelectListItems' property should be of type 'IList<SelectListItem>'. " +
                "See https://learn.microsoft.com/en-us/aspnet/core/mvc/views/working-with-forms?view=aspnetcore-8.0#the-select-tag-helper");

            return value!;
        }

        private Contact GetContact(AddContactModel pageModel)
        {
            PropertyInfo? contactProperty = typeof(AddContactModel).GetProperty("Contact");
            Assert.That(contactProperty, Is.Not.Null, "The AddContactModel class must have a 'Contact' property");

            Contact? value = contactProperty!.GetValue(pageModel) as Contact;
            Assert.That(value, Is.Not.Null, "The 'Contact' property should be of type 'Contact'");

            return value!;
        }
    }
}
