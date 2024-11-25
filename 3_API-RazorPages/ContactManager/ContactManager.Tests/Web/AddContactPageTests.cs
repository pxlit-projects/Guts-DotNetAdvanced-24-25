using AngleSharp.Html.Parser;
using Guts.Client.Core;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using ContactManager.Tests.Helpers;
using Guts.Client.Core.TestTools;

namespace ContactManager.Tests.Web
{
    [ExerciseTestFixture("dotnet2", "4-RAZORWEBAPI", "ContactManager",
   @"ContactManager\Pages\Contacts\AddContact.cshtml")]
    public class AddContactPageTests
    {
        private CustomWebApplicationFactory _factory = null!;
        private HttpClient _client = null!;
        private string _razorContentLowerCase = string.Empty;
        private string _setupError = string.Empty;

        [OneTimeSetUp]
        public void Setup()
        {
            try
            {
                _factory = new CustomWebApplicationFactory();
                _client = _factory.CreateClient();
                _razorContentLowerCase = Solution.Current.GetFileContent("ContactManager/Pages/Contacts/AddContact.cshtml").ToLower();
            }
            catch (Exception e)
            {
                _setupError = e.Message;
            }
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            _factory?.Dispose();
            _client?.Dispose();
        }

        [MonitoredTest]
        public async Task _01_AddContactPage_ReturnsSuccessStatusCode()
        {
            Assert.That(_setupError, Is.Empty, () => "application startup error: " + _setupError);

            // Act
            var response = await _client.GetAsync("Contacts/AddContact");

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "The AddContact page should return a status code 200.");
        }

        [MonitoredTest]
        public async Task _02_AddContactPage_ShouldContainExpectedHtmlContent()
        {
            Assert.That(_setupError, Is.Empty, () => "application startup error: " + _setupError);

            // Act
            HttpResponseMessage response = await _client.GetAsync("Contacts/AddContact");

            // Assert
            response.EnsureSuccessStatusCode();
            string content = await response.Content.ReadAsStringAsync();

            var parser = new HtmlParser();
            IHtmlDocument document = parser.ParseDocument(content);

            IElement? form = document.QuerySelector("form");

            Assert.That(form, Is.Not.Null, "The page has to contain a <form> element");

            IHtmlCollection<IElement> fields = form!.QuerySelectorAll("input");

            var selectElement = form.QuerySelector("select");
            Assert.That(selectElement, Is.Not.Null, "The page has to contain a select list");

            var selectOptions = selectElement!.QuerySelectorAll("option");

            List<string> expectedNames = ["Contact.FirstName", "Contact.Name", "Contact.Email", "Contact.Phone"];
            List<string?> actualNames = fields.Where(input => !string.IsNullOrEmpty(input.GetAttribute("id")))
            .Select(input => input.GetAttribute("name"))
            .ToList();

            Assert.That(actualNames.Count(), Is.EqualTo(4), "The form has to contain 4 input fields");
            Assert.That(actualNames, Is.EquivalentTo(expectedNames), "The form should contain the expected input fields");

            Assert.That(
                selectOptions.Any(option => option.GetAttribute("value") == ""),
                Is.True,
                "The 'Select a company' option should be present in the select element."
            );

            IHtmlButtonElement? button = form.QuerySelector("button") as IHtmlButtonElement;
            Assert.That(button, Is.Not.Null, "The form should have a button");
            Assert.That(button!.Type, Is.EqualTo("submit"), "The button should be a submit button");

            IElement? validationSummaryDiv = document.QuerySelector("div.validation-summary-valid");
            Assert.That(validationSummaryDiv, Is.Not.Null,
                "The page has to contain a div in which a summary of validation errors can be displayed");
        }

        [MonitoredTest]
        public void _03_AddContactPage_CompanyDropDown_ShouldBeRenderedUsingTagHelpers()
        {
            string message = "Do not use option tags, but use a tag helper. " +
                             "See https://learn.microsoft.com/en-us/aspnet/core/mvc/views/working-with-forms?view=aspnetcore-6.0#the-select-tag-helper";

            Assert.That(_razorContentLowerCase, Does.Not.Contain("<option"), message);
            Assert.That(_razorContentLowerCase, Does.Contain("asp-items"), message);
        }
    }
}

