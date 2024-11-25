using ContactManager.AppLogic.Contracts;
using ContactManager.Domain;
using ContactManager.Pages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using System.Reflection;
using ContactManager.Pages.Companies;
using ContactManager.Pages.Contacts;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Guts.Client.Core;

namespace ContactManager.Tests.Web
{
    [ExerciseTestFixture("dotnet2", "4-RAZORWEBAPI", "ContactManager",
    @"ContactManager\Pages\Companies\AddCompany.cshtml.cs")]
    public class AddCompanyPageModelTests
    {
        private Mock<ICompanyRepository> _companyRepositoryMock = null!;

        [SetUp]
        public void Setup()
        {
            _companyRepositoryMock = new Mock<ICompanyRepository>();

        }

        [MonitoredTest]
        public void _01_AddCompanyModel_ShouldInheritFromPageModel()
        {
            // Act
            var addCompanyModel = new AddCompanyModel(_companyRepositoryMock.Object);

            // Assert
            Assert.That(addCompanyModel, Is.InstanceOf<PageModel>());
        }

        [MonitoredTest]
        public void _02_CompanyProperty_HasBindPropertyAttribute()
        {
            // Arrange
            var companyProperty = typeof(AddCompanyModel).GetProperty("Company");

            // Act
            var hasBindPropertyAttribute = companyProperty!.GetCustomAttributes(typeof(BindPropertyAttribute), false).Any();

            // Assert
            Assert.That(hasBindPropertyAttribute, Is.True, "The Company property should have the [BindProperty] attribute.");
        }

        [MonitoredTest]
        public void _03_Constructor_ShouldInitializeAnEmptyContact()
        {
            //Act
            var model = new AddCompanyModel(_companyRepositoryMock.Object);

            //Assert
            Assert.That(model.Company, Is.Not.Null);
        }

        [MonitoredTest]
        public void _04_OnPost_WithValidModel_ShouldCallAddCompanyAndRedirect()
        {
            // Arrange
            var company = new Company { Name = Guid.NewGuid().ToString() };
            var addCompanyModel = new AddCompanyModel(_companyRepositoryMock.Object)
            {
                Company = company
            };

            // Act
            IActionResult result = addCompanyModel.OnPost();

            // Assert
            Assert.That(result, Is.InstanceOf<RedirectToPageResult>(), "The OnPost method should return a redirectToPage result");
            var redirectToPageResult = (RedirectToPageResult)result;
            Assert.That(redirectToPageResult.PageName == "/Index", Is.True, "The OnPost method should redirect to the Index Page");

            _companyRepositoryMock.Verify(repo => repo.AddCompany(company), Times.Once, "The repository is not used correctly to add the company");
        }

        [MonitoredTest]
        public void _05_OnPost_WithInvalidModel_ShouldStayOnPage()
        {
            // Arrange
            var company = new Company();
            var addCompanyModel = new AddCompanyModel(_companyRepositoryMock.Object)
            {
                Company = company
            };

            ModelStateDictionary? modelState = typeof(PageModel).GetProperty("ModelState", BindingFlags.Public | BindingFlags.Instance)?.GetValue(addCompanyModel) as ModelStateDictionary;
            modelState?.AddModelError("Company.Name", "The Name field is required.");

            // Act
            PageResult? result = addCompanyModel.OnPost() as PageResult;

            // Assert
            Assert.That(result, Is.Not.Null, "A 'PageResult' should be returned");
            Assert.That(result!.Page, Is.Null, "A 'PageResult' with 'Page' null should be returned");

            _companyRepositoryMock.Verify(repo => repo.AddCompany(It.IsAny<Company>()), Times.Never);
        }
    }
}
