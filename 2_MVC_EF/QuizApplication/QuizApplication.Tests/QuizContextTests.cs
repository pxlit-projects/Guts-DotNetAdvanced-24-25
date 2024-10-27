using Castle.Core.Resource;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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
        @"QuizApplication.Infrastructure\QuizDbContext.cs")]
    public class QuizContextTests : DataBaseTests
    {
        private string _quizContextClassContent;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _quizContextClassContent = Solution.Current.GetFileContent(@"QuizApplication.Infrastructure\QuizDbContext.cs");
        }

        [MonitoredTest("QuizContext - should be derived from DbContext")]
        public void _01_ShouldBeDerivedFromDbContext()
        {
            var dbContext = typeof(QuizDbContext);
            Assert.That(typeof(DbContext).IsAssignableFrom(dbContext), Is.True);
        }

        [MonitoredTest("QuizContext - should have 2 DbSets")]
        public void _02_ShouldHave2DbSets()
        {
            var properties = GetDbSetProperties();

            Assert.That(properties, Has.Count.EqualTo(2), "There should be exactly 2 'DbSet' properties.");
            Assert.That(properties,
                Has.One.Matches((PropertyDeclarationSyntax p) => p.Type.ToString() == "DbSet<Question>"),
                "There should be one 'DbSet' for questions.");
            Assert.That(properties,
                Has.One.Matches((PropertyDeclarationSyntax p) => p.Type.ToString() == "DbSet<Answer>"),
                "There should be one 'DbSet' for answers.");
        }

        [MonitoredTest("QuizContext - OnModelCreating - should seed the Questions and Answers")]
        public void _03_OnModelCreating_ShouldSeedQuestionsAndAnswers()
        {
            using (var context = CreateDbContext())
            {
                var seededQuestions = context.Set<Question>().ToList();
                var seededAnswers = context.Set<Answer>().ToList();
                Assert.That(seededQuestions, Has.Count.GreaterThanOrEqualTo(5), "The database must be seeded wit at least 5 questions. " +
                                                                             $"Now the database contains {seededQuestions.Count} questions after creation.");
                AssertQuestion(seededQuestions, "What is the capital of France?");
                AssertQuestion(seededQuestions, "What is the largest organ in the human body?");
                AssertAnswer(seededAnswers, "Rome");
                AssertAnswer(seededAnswers, "Liver");
            }
        }

        [MonitoredTest("QuizContext - There must be a relation between Questions and Answers")]
        public void _04_OnModelCreating_ShouldConfigureTheRelationBetweenQuestionAndAnswers()
        {
            using (var context = CreateDbContext())
            {
                var answerEntityType = context.Model.FindEntityType(typeof(Answer).FullName!);
                var anserFK = answerEntityType?.GetForeignKeys().FirstOrDefault(fk => fk.PrincipalEntityType.ClrType == typeof(Question));
                Assert.That(anserFK, Is.Not.Null,
                    "No foreign key relation found between Question and Answer.");
            }
        }

        private BlockSyntax GetMethodBody(string methodName)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(_quizContextClassContent);
            var root = syntaxTree.GetRoot();
            var method = root
                .DescendantNodes()
                .OfType<MethodDeclarationSyntax>()
                .FirstOrDefault(md => md.Identifier.ValueText.Equals(methodName));
            Assert.That(method, Is.Not.Null,
                () => $"Could not find the '{methodName}' method. You may have accidentally deleted or renamed it?");
            return method.Body!;
        }

        private IList<PropertyDeclarationSyntax> GetDbSetProperties()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(_quizContextClassContent);
            var root = syntaxTree.GetRoot();
            var properties = root
                .DescendantNodes()
                .OfType<PropertyDeclarationSyntax>()
                .Where(p =>
                {
                    if (!(p.Type is GenericNameSyntax genericName)) return false;
                    return genericName.Identifier.ValueText == "DbSet";
                });

            return properties.ToList();
        }

        private void AssertQuestion(IList<Question> questions, string questionString)
        {
            Assert.That(questions.Any(q => q.QuestionString == questionString), Is.True,
                $"Question '{questionString}' is not seeded.");
        }
        private void AssertAnswer(IList<Answer> answer, string answerText)
        {
            Assert.That(answer.Any(a => a.AnswerText == answerText), Is.True,
                $"Question '{answerText}' is not seeded.");
        }

    }
}
