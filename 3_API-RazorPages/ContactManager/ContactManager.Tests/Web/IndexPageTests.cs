using AngleSharp.Html.Parser;
using ContactManager.AppLogic.Contracts;
using ContactManager.Pages;
using ContactManager.Tests.Helpers;
using Guts.Client.Core;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Testing;
using Moq;
using System.Net;
using AngleSharp.Html.Dom;

namespace ContactManager.Tests.Web
{
    [ExerciseTestFixture("dotnet2", "4-RAZORWEBAPI", "ContactManager",
   @"ContactManager\Pages\Index.cshtml")]
    public class IndexPageTests
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
        public async Task _01_ShouldReturnsSuccessStatusCode()
        {
            Assert.That(_setupError, Is.Empty, () => "application startup error: " + _setupError);

            // Act
            HttpResponseMessage response = await _client.GetAsync("/Index");

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [MonitoredTest]
        public async Task _02_ShouldContainExpectedHtmlContent()
        {
            Assert.That(_setupError, Is.Empty, () => "application startup error: " + _setupError);

            // Act
            HttpResponseMessage response = await _client.GetAsync("/Index");
            string content = await response.Content.ReadAsStringAsync();

            var parser = new HtmlParser();
            IHtmlDocument document = parser.ParseDocument(content);

            var table = document.QuerySelector("table");
            Assert.That(table, Is.Not.Null, "The Page has to contain a <table> element");
            var headerRow = table!.QuerySelector("thead tr");
            Assert.That(headerRow, Is.Not.Null, "The table has to contain a <thead> element and <tr> elements in it");
            var columns = headerRow!.QuerySelectorAll("th");


            string[] expectedPropertyNames = { "Name", "First Name", "Email", "Phone", "Company" };

            for (int i = 0; i < columns.Length; i++)
            {
                string columnName = columns[i].TextContent.Trim();
                Assert.That(columnName, Is.EqualTo(expectedPropertyNames[i]), $"Column {i} should match property name {expectedPropertyNames[i]}");
            }

            // Assert
            Assert.That(content.Contains("<h1>Contacts</h1>"), Is.True, "The page should have a title with the text \"Contacts\"");
            Assert.That(columns.Length, Is.EqualTo(5), "The table should contain 5 columns.");

            var dataRows = document.QuerySelectorAll("table tbody tr");
            Assert.That(dataRows.Length, Is.GreaterThan(0), "The table should contain at least one data row.");
            foreach (var row in dataRows)
            {
                var cells = row.QuerySelectorAll("td");
                Assert.That(cells.Length, Is.EqualTo(5), "The data row should contain 5 cells.");
            }
        }

    }

}