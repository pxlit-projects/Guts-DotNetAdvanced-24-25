using QuizApplication.AppLogic.Contracts;
using QuizApplication.Domain;
using QuizApplication.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace QuizApplication.Tests.Infrastructure
{
    [ExerciseTestFixture("dotnet2", "2-MVCEF", "QuizApplication",
    @"QuizApplication.Infrastructure\InMemoryCategoryRepository.cs")]
    public class InMemoryCategoryRepositoryTests
    {
        [MonitoredTest("InMemoryCategoryRepository - Interfaces should not have been changed")]
        public void _01_ShouldNotHaveChangedContracts()
        {
            var filePath = @"QuizApplication.AppLogic\Contracts\ICategoryRepository.cs";
            var fileHash = Solution.Current.GetFileHash(filePath);
            Assert.That(fileHash, Is.EqualTo("F3-DD-22-EA-E1-FE-A8-F1-3D-21-A1-2A-45-FD-6A-06"),
                $"The file '{filePath}' has changed. " +
                "Undo your changes on the file to make this test pass.");
        }

        [MonitoredTest("InMemoryCategoryRepository - Should implement ICategoryRepository interface")]
        public void _02_ShouldImplementICategoryRepository()
        {
            Type categoryRepositoryType = typeof(InMemoryCategoryRepository);
            Assert.That(typeof(ICategoryRepository).IsAssignableFrom(categoryRepositoryType), Is.True);
        }

        [MonitoredTest("InMemoryCategoryRepository - Should have a ReadOnlyList private field")]
        public void _03_ShouldHaveAPrivateReadonlyIListField()
        {
            FieldInfo? listField = typeof(InMemoryCategoryRepository).GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
                        .SingleOrDefault(field => field.FieldType.IsAssignableTo(typeof(IReadOnlyList<Category>)));
            Assert.That(listField, Is.Not.Null,
                "The class should have a private field that can store an IReadOnlyList.");

            Assert.That(listField!.IsInitOnly, Is.True,
                "Make sure the field that holds the collection of categories can only be set in the constructor");

        }

        [MonitoredTest("InMemoryCategoryRepository - Should have no public properties")]
        public void _04_ShouldNotHavePublicProperties()
        {
            var props = typeof(InMemoryCategoryRepository).GetProperties();
            Assert.That(props.Count, Is.Zero, "The repository should not have any public properties");
        }
    }
}
