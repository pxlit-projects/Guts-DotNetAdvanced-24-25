using Microsoft.EntityFrameworkCore;
using QuizApplication.AppLogic;
using QuizApplication.AppLogic.Contracts;
using QuizApplication.Domain;
using QuizApplication.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizApplication.Tests.Infrastructure
{
    [ExerciseTestFixture("dotnet2", "2-MVCEF", "QuizApplication", 
        @"QuizApplication.Infrastructure\QuestionRepository.cs")]
    public class QuestionRepositoryTests : DataBaseTests
    {
        [MonitoredTest("QuestionRepository - Interface should not have been changed")]
        public void _01_ShouldNotHaveChangedContracts()
        {
            var filePath = @"QuizApplication.AppLogic\Contracts\IQuestionRepository.cs";
            var fileHash = Solution.Current.GetFileHash(filePath);
            Assert.That(fileHash, Is.EqualTo("73-48-82-6E-A2-2D-2F-DA-FF-42-88-7B-78-07-75-5E"),
                $"The file '{filePath}' has changed. " +
                "Undo your changes on the file to make this test pass.");
        }

        [MonitoredTest("QuestionRepository - should implement IQuestionRepository")]
        public void _02_ShouldImplementIQuestionRepository()
        {
            Type quizRepositoryType = typeof(QuestionRepository);
            Assert.That(typeof(IQuestionRepository).IsAssignableFrom(quizRepositoryType), Is.True);
        }

        [MonitoredTest("QuestionRepository - GetByCategoryId  - should return questions in the category")]
        public void _03_GetByCategoryId_ShouldReturnQuestionsInCategory()
        {
            using (var context = CreateDbContext(true))
            {
                IQuestionRepository repository = new QuestionRepository(context);
                string guid = Guid.NewGuid().ToString();
                Question q1 = new Question() { Id = 1024, QuestionString = guid, CategoryId = 99 };
                string guid2 = Guid.NewGuid().ToString();
                Question q2 = new Question() { Id = 1025, QuestionString = guid2, CategoryId = 99 };
                context.Questions.Add(q1);
                context.Questions.Add(q2);
                context.SaveChanges();

                IReadOnlyList<Question> questionsInCategory = repository.GetByCategoryId(99);

                Assert.That(questionsInCategory, Has.Count.EqualTo(2), "Category 99 contains 2 questions in the seed data.");
                Assert.That(questionsInCategory.Any(q => q.QuestionString == guid), Is.True, "Category 99 contains a wrong question.");
                Assert.That(questionsInCategory.Any(q => q.QuestionString == guid2), Is.True, "Category 99 contains a wrong question.");
            }
        }

        [MonitoredTest("QuestionRepository - GetByIdWithAnswers  - should return a Question with Answers in it")]
        public void _04_GetById_ShouldReturnQuestionWithAnswers()
        {
            using (var context = CreateDbContext(true))
            {
                IQuestionRepository repository = new QuestionRepository(context);

                Question question = repository.GetByIdWithAnswers(7);

                Assert.That(question.QuestionString, Is.EqualTo("What is the largest mammal in the world?"));
                Assert.That(question.Answers, Has.Count.EqualTo(4), "There are 4 answers in the seed data of this question.");
            }
        }
    }
}
