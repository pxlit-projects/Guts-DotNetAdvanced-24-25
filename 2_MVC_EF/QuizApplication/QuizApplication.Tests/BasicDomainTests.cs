using QuizApplication.AppLogic.Contracts;
using QuizApplication.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace QuizApplication.Tests.Domain
{
    [ExerciseTestFixture("dotnet2", "2-MVCEF", "QuizApplication",
    @"QuizApplication.Domain\Answer.cs;QuizApplication.Domain\Category.cs;QuizApplication.Domain\Question.cs")]
    public class BasicDomainTests
    {
        [MonitoredTest("Domain - Question - should contain the correct properties")]
        public void _01_QuestionShouldContainCorrectProperties()
        {
            Type questionType = typeof(Question);

            PropertyInfo[] properties = questionType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty);
            Assert.That(properties.Count, Is.EqualTo(4), "Class Question should have 4 properties (with getter and setter)");
            Assert.That(properties.Any(p => p.Name == "Id"), Is.True);
            Assert.That(properties.Any(p => p.Name == "QuestionString"), Is.True);
            Assert.That(properties.Any(p => p.Name == "CategoryId"), Is.True);
            Assert.That(properties.Any(p => p.Name == "Answers"), Is.True);

            ConstructorInfo[] constructors = questionType.GetConstructors(BindingFlags.Instance | BindingFlags.Public);
            ConstructorInfo constructor = constructors.First();
            ParameterInfo[] parameters = constructor.GetParameters();
            Assert.That(constructors.Length, Is.EqualTo(1), "There should be exactly one public constructor.");
            Assert.That(parameters.Length, Is.EqualTo(0), "The constructor should have 0 parameters.");
        }

        [MonitoredTest("Domain - Answer - should contain the correct properties")]
        public void _02_AnswerShouldContainCorrectProperties()
        {
            Type answerType = typeof(Answer);

            PropertyInfo[] properties = answerType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty);
            Assert.That(properties.Count, Is.EqualTo(4), "Class Question should have 4 properties (with getter and setter)");
            Assert.That(properties.Any(p => p.Name == "Id"), Is.True);
            Assert.That(properties.Any(p => p.Name == "AnswerText"), Is.True);
            Assert.That(properties.Any(p => p.Name == "IsCorrect"), Is.True);
            Assert.That(properties.Any(p => p.Name == "QuestionId"), Is.True);

            ConstructorInfo[] constructors = answerType.GetConstructors(BindingFlags.Instance | BindingFlags.Public);
            ConstructorInfo constructor = constructors.First();
            ParameterInfo[] parameters = constructor.GetParameters();
            Assert.That(constructors.Length, Is.EqualTo(1), "There should be exactly one public constructor.");
            Assert.That(parameters.Length, Is.EqualTo(0), "The constructor should have 0 parameters.");
        }

        [MonitoredTest("Domain - Question and Answer - should do the necessary null checks and initializations")]
        public void _03_NullChecks()
        {
            Question question = new Question();
            Assert.That(question.Answers, Is.Not.Null, "The answers list should be initialised after constructing a Question.");
            Assert.That(question.QuestionString, Is.Not.Null, "The question itself can not be null after construction of a Question.");

            Answer answer = new Answer();
            Assert.That(answer.AnswerText, Is.Not.Null, "The answer text in an Answer can not be null after construction of a Answer.");
        }
    }
}
