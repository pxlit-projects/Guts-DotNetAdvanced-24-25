using Guts.Client.Core.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using QuizApplication.AppLogic.Contracts;
using QuizApplication.Domain;
using QuizApplication.Tests.Web.Helpers;
using QuizApplication.Web.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


namespace QuizApplication.Tests.Web
{
    // More on Integration tests: https://learn.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-7.0
    
    [ExerciseTestFixture("dotnet2", "2-MVCEF", "QuizApplication",
        @"QuizApplication.Web\Views\Home\Index.cshtml;QuizApplication.Web\Views\Question\Index.cshtml;QuizApplication.Web\Views\Question\QuestionsInCategory.cshtml;QuizApplication.Web\Views\Question\QuestionWithAnswers.cshtml")]
    public class HtmlTests
    {
        private readonly WebApplicationFactory<Program> _factory;

        public HtmlTests()
        {
            _factory = new WebApplicationFactory<Program>();
        }

        [MonitoredTest("Html Integration Tests - All pages should return html in the body")]
        [TestCase("/")]
        [TestCase("/Home/About")]
        [TestCase("/Question")]
        [TestCase("/Question/QuestionsInCategory/1")]
        [TestCase("/Question/QuestionWithAnswers/1")]
        public async Task _01_Get_EndpointsReturnSuccessAndCorrectContentTypeAndPage(string url)
        {
            // Arrange
            await using var appFactory = new CustomWebApplicationFactory(services =>
            {
                services.Replace(ServiceDescriptor.Scoped(_ =>
                {
                    var providerMock = new Mock<IQuizService>();
                    providerMock.Setup(x => x.GetQuestionsInCategory(1)).Returns(
                       new List<Question>() { new Question() { Id = 1 } });
                    providerMock.Setup(x => x.GetQuestionByIdWithAnswersAndExtra(1)).Returns(
                        new Question() { Id = 1 });
                    providerMock.Setup(x => x.GetAllCategories()).Returns(new List<Category>());
                    return providerMock.Object;
                }));
            });

            var client = appFactory.CreateClient();

            // Act
            var response = await client.GetAsync(url);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.That(response.Content.Headers.ContentType!.ToString(), Is.EqualTo("text/html; charset=utf-8"));
            var content = await HtmlHelpers.GetDocumentAsync(response);
            Assert.That(content.Body, Is.Not.Null, "No body was returned in the html page");
        }

        [MonitoredTest("Html Integration Tests - HomeController - Index - Should return html containing a gif from the wwwroot")]
        public async Task _02_HomeControllerIndexShouldShowGif()
        {
            await using var appFactory = new CustomWebApplicationFactory(services =>
            {
                services.Replace(ServiceDescriptor.Scoped(_ =>
                {
                    var providerMock = new Mock<IQuizService>();
                    return providerMock.Object;
                }));
            });

            var client = appFactory.CreateClient();

            // Act
            var response = await client.GetAsync("/");
            var content = await HtmlHelpers.GetDocumentAsync(response);

            Assert.That(content.Body, Is.Not.Null, "No body was returned in the html page");
            Assert.That(Regex.Matches(content.Body.InnerHtml, "question-asking.gif").Count, Is.EqualTo(1), "The index page should show a gif-image that is available in the wwwroot");
        }

        [MonitoredTest("Html Integration Tests - QuestionController - Index - html should contain links to the questionsincategory/{id} route")]
        public async Task _03_QuestionIndexShouldContainHyperLinksToQuestionsInCategory()
        {
            // Arrange
            Random rnd = new Random();
            var randomCategoryTexts = new List<string>() { Guid.NewGuid().ToString(), Guid.NewGuid().ToString() };
            var randomCategoryIds = new List<int>() { rnd.Next(0, 255), rnd.Next(0, 255) };

            await using var appFactory = new CustomWebApplicationFactory(services =>
            {
                services.Replace(ServiceDescriptor.Scoped(_ =>
                {
                    var providerMock = new Mock<IQuizService>();
                    providerMock.Setup(x => x.GetAllCategories()).Returns(
                        new List<Category>() {
                            new Category() { Id = randomCategoryIds[0], Name = randomCategoryTexts[0]  },
                            new Category() { Id = randomCategoryIds[1], Name = randomCategoryTexts[1]  }
                            });
                    return providerMock.Object;
                }));
            });

            var client = appFactory.CreateClient();

            // Act
            var response = await client.GetAsync("/Question");

            // Assert
            var content = await HtmlHelpers.GetDocumentAsync(response);
            Assert.That(content.Body, Is.Not.Null, "No body was returned in the html page");
            Assert.That(Regex.Matches(content.Body.InnerHtml, "href=\"/Question/QuestionsInCategory/").Count, Is.EqualTo(2), "There should be 2 hyperlinks (<a> element) that point to the QuestionsInCategory route");
            Assert.That(Regex.Matches(content.Body.InnerHtml, "href=\"/Question/QuestionsInCategory/" + randomCategoryIds[0]).Count, Is.EqualTo(1), "There should be a link to the two categories that are returned in the test (<a> element) that point to the QuestionsInCategory route");
            Assert.That(Regex.Matches(content.Body.InnerHtml, "href=\"/Question/QuestionsInCategory/" + randomCategoryIds[1]).Count, Is.EqualTo(1), "There should be a link to the two categories that are returned in the test (<a> element) that point to the QuestionsInCategory route");

            Assert.That(Regex.Matches(content.Body.InnerHtml, randomCategoryTexts[0]).Count, Is.EqualTo(1), "The two returned category names in the test should be present in the generated HTML");
            Assert.That(Regex.Matches(content.Body.InnerHtml, randomCategoryTexts[1]).Count, Is.EqualTo(1), "The two returned category names in the test should be present in the generated HTML");
            
        }

        [MonitoredTest("Html Integration Tests - QuestionController - QuestionsInCategory - html should contain links to the QuestionWithAnswers/{id} route")]
        public async Task _04_QuestionsInCategoryShouldContainLinksToQuestionsWithAnswers()
        {
            // Arrange
            Random rnd = new Random();
            var randomQuestionTexts = new List<string>() { Guid.NewGuid().ToString(), Guid.NewGuid().ToString() };
            var randomQuestionIds = new List<int>() { rnd.Next(0, 255), rnd.Next(0, 255) };

            await using var appFactory = new CustomWebApplicationFactory(services =>
            {
                services.Replace(ServiceDescriptor.Scoped(_ =>
                {
                    var providerMock = new Mock<IQuizService>();
                    providerMock.Setup(x => x.GetQuestionsInCategory(1)).Returns(
                        new List<Question>() {
                            new Question() { Id = randomQuestionIds[0], QuestionString = randomQuestionTexts[0]  },
                            new Question() { Id = randomQuestionIds[1], QuestionString = randomQuestionTexts[1]  }
                            });
                    return providerMock.Object;
                }));
            });

            var client = appFactory.CreateClient();

            // Act
            var response = await client.GetAsync("/Question/QuestionsInCategory/1");

            // Assert
            var content = await HtmlHelpers.GetDocumentAsync(response);
            Assert.That(content.Body, Is.Not.Null, "No body was returned in the html page");
            Assert.That(Regex.Matches(content.Body.InnerHtml, "href=\"/Question/QuestionWithAnswers/").Count, Is.EqualTo(2), "There should be 2 hyperlinks (<a> element) that point to the QuestionWithAnswers route");
            Assert.That(Regex.Matches(content.Body.InnerHtml, "href=\"/Question/QuestionWithAnswers/" + randomQuestionIds[0]).Count, Is.EqualTo(1), "There should be a link to the two questions that are returned in the test (<a> element) that point to the QuestionWithAnswers route");
            Assert.That(Regex.Matches(content.Body.InnerHtml, "href=\"/Question/QuestionWithAnswers/" + randomQuestionIds[1]).Count, Is.EqualTo(1), "There should be a link to the two questions that are returned in the test (<a> element) that point to the QuestionWithAnswers route");

            Assert.That(Regex.Matches(content.Body.InnerHtml, randomQuestionTexts[0]).Count, Is.EqualTo(1), "The two returned question text in the test should be present in the generated HTML");
            Assert.That(Regex.Matches(content.Body.InnerHtml, randomQuestionTexts[1]).Count, Is.EqualTo(1), "The two returned question text in the test should be present in the generated HTML");
            Assert.That(Regex.Matches(content.Body.InnerHtml, "href=\"/Question\"").Count, Is.GreaterThanOrEqualTo(1), "There should be a way to navigate back to \"/Question\"");

        }

        [MonitoredTest("Html Integration Tests - QuestionController - QuestionWithAnswers - html should contain the questions and all the answers")]
        public async Task _05_QuestionsWithAnswerShouldShowAllAnswers()
        {
            // Arrange
            Random rnd = new Random();
            var randomAnswerTexts = new List<string>() { Guid.NewGuid().ToString(), Guid.NewGuid().ToString() };
            var randomQuestionText = Guid.NewGuid().ToString();
            var randomCategoryId = rnd.Next(0, 255);
            await using var appFactory = new CustomWebApplicationFactory(services =>
            {
                services.Replace(ServiceDescriptor.Scoped(_ =>
                {
                    var providerMock = new Mock<IQuizService>();
                    providerMock.Setup(x => x.GetQuestionByIdWithAnswersAndExtra(1)).Returns(
                        new Question()
                        {
                            Id = 1,
                            CategoryId = randomCategoryId,
                            QuestionString = randomQuestionText,
                            Answers = new List<Answer>()
                            {
                            new Answer() { AnswerText = randomAnswerTexts[0] },
                            new Answer() { AnswerText = randomAnswerTexts[1] }
                            }
                        });
                    return providerMock.Object;
                }));
            });

            var client = appFactory.CreateClient();

            // Act
            var response = await client.GetAsync("/Question/QuestionWithAnswers/1");

            // Assert
            var content = await HtmlHelpers.GetDocumentAsync(response);
            Assert.That(content.Body, Is.Not.Null, "No body was returned in the html page");
            Assert.That(Regex.Matches(content.Body.InnerHtml, randomQuestionText).Count, Is.EqualTo(1), "The two question text generated in this test should be present in the generated HTML");
            Assert.That(Regex.Matches(content.Body.InnerHtml, randomAnswerTexts[0]).Count, Is.EqualTo(1), "The two returned answers in the test should be present in the generated HTML");
            Assert.That(Regex.Matches(content.Body.InnerHtml, randomAnswerTexts[1]).Count, Is.EqualTo(1), "The two returned answers in the test should be present in the generated HTML");
            Assert.That(Regex.Matches(content.Body.InnerHtml, "href=\"/Question/QuestionsInCategory/" + randomCategoryId).Count, Is.GreaterThanOrEqualTo(1), "There should be a way to navigate back to categoryId \"/Question/QuestionsInCategory/" + randomCategoryId);

        }

        [OneTimeTearDown]
        public void TearDown()
        {
            _factory?.Dispose();
        }
    }
}
