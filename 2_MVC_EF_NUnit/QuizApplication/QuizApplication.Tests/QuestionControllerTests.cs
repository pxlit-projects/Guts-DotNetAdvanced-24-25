using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using QuizApplication.AppLogic;
using QuizApplication.AppLogic.Contracts;
using QuizApplication.Domain;
using QuizApplication.Infrastructure;
using QuizApplication.Web.Controllers;
using QuizApplication.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace QuizApplication.Tests.Web
{
    [ExerciseTestFixture("dotnet2", "2-MVCEF", "QuizApplication",
        @"QuizApplication.Web\Controllers\QuestionController.cs")]
    public class QuestionControllerTests
    {
        private QuestionController _controller = null!;
        private Mock<IQuizService> _quizServiceMock = null!;

        [SetUp]
        public void Setup()
        {
            _quizServiceMock = new Mock<IQuizService>();
            var mockLogger = new Mock<ILogger<QuestionController>>();
            _controller = new QuestionController(mockLogger.Object, _quizServiceMock.Object);
        }

        [MonitoredTest("QuestionController - Constructor - Should have a constructor that accepts a IQuizService")]
        public void _01_ShouldHaveAConstructorThatAcceptsQuizService()
        {
            var controllerType = typeof(QuestionController);
            ConstructorInfo[] constructors = controllerType.GetConstructors(BindingFlags.Instance | BindingFlags.Public);
            Assert.That(constructors.Length, Is.EqualTo(1), "There should be exactly one public constructor.");

            ConstructorInfo constructor = constructors.First();
            ParameterInfo[] parameters = constructor.GetParameters();
            Assert.That(parameters.Length, Is.EqualTo(2), "The constructor should have 2 parameters.");
            Assert.That(parameters.Any(p => p.ParameterType == typeof(IQuizService)), Is.True,
                "One of the constructor parameters should be of type IQuizService, the other is an ILogger.");
        }

        [MonitoredTest("QuestionController - Should have a private readonly IQuizService field")]
        public void _02_ShouldHaveAPrivateReadonlyIQuizService()
        {
            FieldInfo? listField = typeof(QuestionController).GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
                        .SingleOrDefault(field => field.FieldType.IsAssignableTo(typeof(IQuizService)));
            Assert.That(listField, Is.Not.Null,
                "The class should have a private field that can store an IQuizService.");

            Assert.That(listField!.IsInitOnly, Is.True,
                "Make sure the field that holds IQuizService can only be set in the constructor");

        }

        [MonitoredTest("QuestionController - Index - Should use service and return a view with categories")]
        public void _03_Index_ShouldReturnCategories()
        {
            List<Category> categoryList = new List<Category>();
            categoryList.Add(new Category());
            categoryList.Add(new Category());
            _quizServiceMock.Setup(service => service.GetAllCategories()).Returns(categoryList);

            ViewResult? result = _controller.Index() as ViewResult;

            _quizServiceMock.Verify(service => service.GetAllCategories(), Times.Once,
                "The 'GetAllCategories' method of the service should be used once.");
            _quizServiceMock.VerifyNoOtherCalls();

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Model, Is.AssignableTo(typeof(IEnumerable<Category>)));
            Assert.That(((IEnumerable<Category>)result.Model).Count, Is.EqualTo(2), "The View should get a list of 2 categories to show");
        }

        [MonitoredTest("QuestionController - QuestionsInCategory - Should use service and return a view with questions of a category")]
        public void _04_QuestionsInCategory_ShouldReturnQuestions()
        {
            List<Question> questionList = new List<Question>();
            questionList.Add(new Question());
            questionList.Add(new Question());
            _quizServiceMock.Setup(service => service.GetQuestionsInCategory(1)).Returns(questionList);

            ViewResult? result = _controller.QuestionsInCategory(1) as ViewResult;

            _quizServiceMock.Verify(service => service.GetQuestionsInCategory(1), Times.Once,
                "The 'GetQuestionsInCategory' method of the service should be used once.");
            _quizServiceMock.VerifyNoOtherCalls();

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Model, Is.AssignableTo(typeof(IEnumerable<Question>)));
            Assert.That(((IEnumerable<Question>)result.Model).Count, Is.EqualTo(2), "The View should get a list of 2 questions to show");
        }

        [MonitoredTest("QuestionController - QuestionWithAnswers - Should use service and return a view using a QuestionViewModel")]
        public void _05_QuestionWithAnswers_ShouldReturnViewWithQuestionViewModel()
        {
            Question question = new Question();
            _quizServiceMock.Setup(service => service.GetQuestionByIdWithAnswersAndExtra(1)).Returns(question);

            ViewResult? result = _controller.QuestionWithAnswers(1) as ViewResult;

            _quizServiceMock.Verify(service => service.GetQuestionByIdWithAnswersAndExtra(1), Times.Once,
                "The 'GetQuestionByIdWithAnswersAndExtra' method of the service should be used once.");
            _quizServiceMock.VerifyNoOtherCalls();

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Model, Is.AssignableTo(typeof(QuestionViewModel)));
            Assert.That(((QuestionViewModel)result.Model).Question, Is.EqualTo(question), "The View should get an object of type QuestionViewModel, with the Question property set");
        }

        private void resetInvocationsRepositoryMocks()
        {
            _quizServiceMock.Invocations.Clear();
        }

        [TearDown]
        public void TearDown()
        {
            _controller?.Dispose();
        }
    }
}
