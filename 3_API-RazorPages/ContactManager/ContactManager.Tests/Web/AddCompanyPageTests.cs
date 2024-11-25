using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using ContactManager.Tests.Helpers;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using Guts.Client.Core;

namespace ContactManager.Tests.Web
{
    [ExerciseTestFixture("dotnet2", "4-RAZORWEBAPI", "ContactManager",
    @"ContactManager\Pages\Companies\AddCompany.cshtml")]
    public class AddCompanyPageTests
    {
        private CustomWebApplicationFactory _factory = null!;
        private HttpClient _client = null!;
        private string _setupError = string.Empty;

        [OneTimeSetUp]
        public void Setup()
        {
            try
            {
                _factory = new CustomWebApplicationFactory();
                _client = _factory.CreateClient();
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
        public async Task _01_AddCompany_ReturnsSuccessStatusCode()
        {
            Assert.That(_setupError, Is.Empty, () => "application startup error: " + _setupError);

            // Act
            var response = await _client.GetAsync("Companies/AddCompany");

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [MonitoredTest]
        public async Task _02_AddCompany_ShouldContainExpectedHtmlContent()
        {
            Assert.That(_setupError, Is.Empty, () => "application startup error: " + _setupError);

            var response = await _client.GetAsync("/Companies/AddCompany");
            var content = await response.Content.ReadAsStringAsync();

            // Parse the HTML content
            var parser = new HtmlParser();
            var document = parser.ParseDocument(content);

            // Find the form and count the fields
            var form = document.QuerySelector("form");

            Assert.That(form, Is.Not.Null, "The Page has to contain a <form> element");


            var fields = form.QuerySelectorAll("input");


            List<string> expectedNames = ["Company.Name", "Company.Address", "Company.Zip", "Company.City"];

            List<string?> actualNames = fields.Where(input => !string.IsNullOrEmpty(input.GetAttribute("id")))
            .Select(input => input.GetAttribute("name"))
            .ToList();

            Assert.That(actualNames.Count(), Is.EqualTo(4), "The form has to contain 4 input fields");
            Assert.That(actualNames, Is.EquivalentTo(expectedNames), "The form should contain the expected input fields");

            IHtmlButtonElement? button = form.QuerySelector("button") as IHtmlButtonElement;
            Assert.That(button, Is.Not.Null, "The form should have a button");
            Assert.That(button!.Type, Is.EqualTo("submit"), "The button should be a submit button");

            IElement? validationSummaryDiv = document.QuerySelector("div.validation-summary-valid");
            Assert.That(validationSummaryDiv, Is.Not.Null,
                "The page has to contain a div in which a summary of validation errors can be displayed");
        }
    }
}