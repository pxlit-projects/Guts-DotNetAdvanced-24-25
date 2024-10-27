using Moq;
using QuizApplication.AppLogic.Contracts;
using QuizApplication.AppLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using QuizApplication.Domain;

namespace QuizApplication.Tests.AppLogic
{
    [ExerciseTestFixture("dotnet2", "2-MVCEF", "QuizApplication",
    @"QuizApplication.AppLogic\QuizService.cs")]
    public class QuizServiceTests
    {
        private Type _quizServiceType = null!;
        private Mock<IQuestionRepository> _questionRepositoryMock = null!;
        private Mock<ICategoryRepository> _categoryRepositoryMock = null!;
        private QuizService? _service = null!;

        [OneTimeSetUp]
        public void BeforeAllTests()
        {
            _quizServiceType = typeof(QuizService);
            _questionRepositoryMock = new Mock<IQuestionRepository>();
            _categoryRepositoryMock = new Mock<ICategoryRepository>();

            ConstructQuizService(_questionRepositoryMock.Object, _categoryRepositoryMock.Object);
        }

        [MonitoredTest("QuizService - Should not have changed contract files")]
        public void _01_ShouldNotHaveChangedContracts()
        {
            var filePath = @"QuizApplication.AppLogic\Contracts\IQuizService.cs";
            var fileHash = Solution.Current.GetFileHash(filePath);
            Assert.That(fileHash, Is.EqualTo("A5-2C-1D-41-C5-37-FD-A4-F9-FB-27-99-5B-0E-1E-32"),
                $"The file '{filePath}' has changed. " +
                "Undo your changes on the file to make this test pass.");
        }

        [MonitoredTest("QuizService - Should implement IQuizService")]
        public void _02_ShouldImplementIQuizService()
        {
            Assert.That(typeof(IQuizService).IsAssignableFrom(_quizServiceType), Is.True, "The IQuizService should be implemented in QuizService.");
        }

        [MonitoredTest("QuizService - Should only be visible to the AppLogic layer")]
        public void _03_ShouldOnlyBeVisibleToTheAppLogicLayer()
        {
            Assert.That(_quizServiceType.IsNotPublic,
                "Only IQuizService should be visible to the other layers. The QuizService class itself can be encapsulated in the application logic layer.");
        }

        [MonitoredTest("QuizService - Constructor - should accept two repositories")]
        public void _04_ShouldHaveAConstructorThatAcceptsTwoRepositories()
        {
            ConstructorInfo[] constructors = _quizServiceType.GetConstructors(BindingFlags.Instance | BindingFlags.Public);
            Assert.That(constructors.Length, Is.EqualTo(1), "There should be exactly one public constructor.");

            ConstructorInfo constructor = constructors.First();
            ParameterInfo[] parameters = constructor.GetParameters();
            Assert.That(parameters.Length, Is.EqualTo(2), "The constructor should have 2 parameters.");
            Assert.That(parameters.Any(p => p.ParameterType == typeof(ICategoryRepository)), Is.True,
                "One of the constructor parameters should be of type ICategoryRepository.");
            Assert.That(parameters.Any(p => p.ParameterType == typeof(IQuestionRepository)), Is.True,
                "One of the constructor parameters should be of type IQuestionRepository.");
        }

        [MonitoredTest("QuizService - GetAllCategories - should use repository and return categories")]
        public void _05_GetAllCategoriesShouldInvokeRepositoryAndReturnCategories()
        {
            List<Category> categoryList = new List<Category>();
            categoryList.Add(new Category());
            categoryList.Add(new Category());

            IReadOnlyList<Category>? returnedList = null;

            _categoryRepositoryMock.Setup(repo => repo.GetAll()).Returns(categoryList);
            resetInvocationsRepositoryMocks();

            returnedList = _service!.GetAllCategories();

            _categoryRepositoryMock.Verify(repo => repo.GetAll(), Times.Once,
                            "The 'GetAll' method of the category repository should be used once.");
            _categoryRepositoryMock.VerifyNoOtherCalls();
            _questionRepositoryMock.VerifyNoOtherCalls();

            Assert.That(returnedList, Is.Not.Null, "The service should return a list of categories.");
            Assert.That(returnedList.Count, Is.EqualTo(2), "The list should contain two items.");
        }

        [MonitoredTest("QuizService - GetQuestionsInCategory - should use repository and return questions of a category")]
        public void _06_GetQuestionsInCategoryShouldInvokeRepositoryAndReturnQuestions()
        {
            var mockQuestion1 = new Question();
            var mockQuestion2 = new Question();
            var rnd = new Random();
            var randomCategoryId = rnd.Next();
            mockQuestion1.CategoryId = randomCategoryId;
            mockQuestion2.CategoryId = randomCategoryId;

            List<Question> questionList = new List<Question>();
            questionList.Add(mockQuestion1);
            questionList.Add(mockQuestion2);

            _questionRepositoryMock.Setup(repo => repo.GetByCategoryId(randomCategoryId)).Returns(questionList);
            resetInvocationsRepositoryMocks();

            var returnedQuestions = _service!.GetQuestionsInCategory(randomCategoryId);

            _questionRepositoryMock.Verify(repo => repo.GetByCategoryId(randomCategoryId), Times.Once,
                            "The 'GetByCategoryId' method of the question repository should be used once.");
            _questionRepositoryMock.VerifyNoOtherCalls();
            _categoryRepositoryMock.VerifyNoOtherCalls();

            Assert.That(returnedQuestions, Is.Not.Null, "The service should return a Question.");
            Assert.That(returnedQuestions.Count, Is.EqualTo(2), "There should be 2 questions.");
            Assert.That(returnedQuestions[0].CategoryId, Is.EqualTo(randomCategoryId), "The questions in the list should belong to the generated category.");
        }

        [MonitoredTest("QuizService - GetQuestionByIdWithAnswersAndExtra - should use repository and return question with an extra answer")]
        public void _07_GetQuestionByIdWithAnswersShouldInvokeRepositoryAndReturnQuestion()
        {
            var mockQuestion = new Question();
            mockQuestion.Answers = new List<Answer> { new Answer(), new Answer() };

            _questionRepositoryMock.Setup(repo => repo.GetByIdWithAnswers(1)).Returns(mockQuestion);
            resetInvocationsRepositoryMocks();

            var returnedQuestion = _service!.GetQuestionByIdWithAnswersAndExtra(1);

            _questionRepositoryMock.Verify(repo => repo.GetByIdWithAnswers(1), Times.Once,
                            "The 'GetById' method of the question repository should be used once (with id 1).");
            _questionRepositoryMock.VerifyNoOtherCalls();
            _categoryRepositoryMock.VerifyNoOtherCalls();

            Assert.That(returnedQuestion, Is.Not.Null, "The service should return a Question.");
            Assert.That(returnedQuestion.Answers.Count, Is.EqualTo(3), "The question should have 3 answer items (one is generated extra see test 08).");
        }

        [MonitoredTest("QuizService - GetQuestionByIdWithAnswersAndExtra - should generate an answer that is correct if no other answer in the question is correct")]
        [TestCase(true)]
        [TestCase(false)]
        public void _08_GetQuestionByIdShouldAddAnExtraAnswer(bool correctAnswerPresent)
        {
            var mockQuestion = new Question();
            mockQuestion.Answers = new List<Answer>() { new Answer() { IsCorrect = correctAnswerPresent }, new Answer() };
            _questionRepositoryMock.Setup(repo => repo.GetByIdWithAnswers(1)).Returns(mockQuestion);
            resetInvocationsRepositoryMocks();

            var returnedQuestion = _service!.GetQuestionByIdWithAnswersAndExtra(1);

            _questionRepositoryMock.Verify(repo => repo.GetByIdWithAnswers(1), Times.Once,
                            "The 'GetByIdWithAnswers' method of the question repository should be used once (with id 1).");
            _questionRepositoryMock.VerifyNoOtherCalls();
            _categoryRepositoryMock.VerifyNoOtherCalls();

            Assert.That(returnedQuestion, Is.Not.Null, "The service should return a Question.");
            Assert.That(returnedQuestion.Answers.Count, Is.EqualTo(3), "The question should have 3 answer items after processing by the service.");
            Assert.That(returnedQuestion.Answers.Last().AnswerText, Contains.Substring("the answers"), "The generated question should contain the string 'None of the answers is correct.' and should be placed last in the list.");
            if (correctAnswerPresent)
            {
                Assert.That(returnedQuestion.Answers.Last().IsCorrect, Is.False, "If one or more correct answers are present, the extra answer should be false");
            }
            else
            {
                Assert.That(returnedQuestion.Answers.Last().IsCorrect, Is.True, "If no correct answers are present, the extra answer should be true");
            }
        }

        private void resetInvocationsRepositoryMocks()
        {
            _questionRepositoryMock.Invocations.Clear();
            _questionRepositoryMock.Invocations.Clear();
        }


        private void ConstructQuizService(IQuestionRepository questionRepository, ICategoryRepository categoryRepository)
        {
            try
            {
                _service = Activator.CreateInstance(typeof(QuizService),
                    BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public, null,
                    new object[] { questionRepository, categoryRepository },
                    null) as QuizService;
            }
            catch (Exception)
            {
                try
                {
                    _service = Activator.CreateInstance(typeof(QuizService),
                        BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public, null,
                        new object[] { categoryRepository, questionRepository },
                        null) as QuizService;
                }
                catch (Exception)
                {
                    _service = Activator.CreateInstance(typeof(QuizService),
                        BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public, null,
                        new object[] { },
                        null) as QuizService;
                }
            }
            Assert.That(_service, Is.Not.Null, "Failed to instantiate a QuizService. There should be a constructor accepting a IQuestionRepository and a ICategoryRepository");
        }
    }
}
