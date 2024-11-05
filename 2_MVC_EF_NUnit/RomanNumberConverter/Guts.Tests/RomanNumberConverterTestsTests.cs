using System.Reflection;
using System.Text;
using Guts.Client.Core;
using Guts.Client.Core.TestTools;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using RomanNumberConverterApp.Ui.Tests.Models;

namespace Guts.Tests
{
    [ExerciseTestFixture("dotnet2", "3-NUnit", "RomanNumberConverter",
        @"RomanNumberConverterApp.Ui\Models\RomanNumberConverter.cs;RomanNumberConverterApp.Ui.Tests\Models\RomanNumberConverterTests.cs")]
    public class RomanNumberConverterTestsTests
    {
        private const string ValueShouldBeInRangeMethodName = "Convert_ValueIsNotBetweenOneAnd3999_ShouldThrowArgumentException";
        private const string ValidValueShouldConvertCorrectlyMethodName = "Convert_ValidValue_ShouldReturnRomanNumberEquivalent";

        private MethodInfo? _setupMethod;
        private MethodInfo? _valueShouldBeInRangeMethod;
        private MethodInfo? _validValueShouldConvertCorrectlyMethod;
        private string _testClassContent = string.Empty;
        private RomanNumberConverterTests _romanNumberConverterTestsInstance = null!;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            var testClassType = typeof(RomanNumberConverterTests);

            _setupMethod = testClassType.GetMethods()
                .FirstOrDefault(m => m.GetCustomAttribute<SetUpAttribute>() != null);

            _valueShouldBeInRangeMethod =
                testClassType.GetMethod(ValueShouldBeInRangeMethodName);

            _validValueShouldConvertCorrectlyMethod =
                testClassType.GetMethod(ValidValueShouldConvertCorrectlyMethodName);

            _testClassContent = Solution.Current.GetFileContent(@"RomanNumberConverterApp.Ui.Tests\Models\RomanNumberConverterTests.cs");
        }

        [SetUp]
        public void SetUp()
        {
            _romanNumberConverterTestsInstance = new RomanNumberConverterTests();

            if (_setupMethod != null)
            {
                _setupMethod.Invoke(_romanNumberConverterTestsInstance, Array.Empty<object>());
            }
        }

        #region In range test

        [MonitoredTest]
        public void _01_ShouldHaveATestThatChecksIfTheValueIsInRange()
        {
            AssertHasValueIsInRangeTestMethod();

            List<TestCaseAttribute> testCaseAttributes = _valueShouldBeInRangeMethod!.GetCustomAttributes()
                .OfType<TestCaseAttribute>().ToList();

            Assert.That(testCaseAttributes, Has.Count.GreaterThanOrEqualTo(2), () => "The method should have at least 2 test cases.");

            Assert.That(testCaseAttributes,
                Has.All.Matches((TestCaseAttribute testCase) => testCase.Arguments?.Length == 1),
                () => "All test cases must have 1 argument (value)");

            var hasInvalidTestCases = testCaseAttributes.Any(a =>
            {
                if (!(a.Arguments[0] is int value)) return true;
                if (value >= 1 && value <= 3999) return true;
                return false;
            });
            Assert.That(hasInvalidTestCases, Is.False,
                "The method should have only 'TestCases' for values that can be parsed to an integer outside the 1-3999 range.");

            Assert.That(testCaseAttributes,
                Has.One.Matches((TestCaseAttribute testCase) => ((int)testCase.Arguments[0]!) == 0),
                "One of the test cases should be '0'");

            Assert.That(testCaseAttributes,
                Has.One.Matches((TestCaseAttribute testCase) => ((int)testCase.Arguments[0]!) == 4000),
                "One of the test cases should be '4000'");

            var methodBody = GetMethodBodyWithoutComments(ValueShouldBeInRangeMethodName);

            AssertCallsSutMethodAndUsesAssertThatSyntax(methodBody);

            Assert.That(methodBody, Contains.Substring("Assert.That("), "The test should use the Assert.That syntax.");
            Assert.That(methodBody, Contains.Substring("Throws."), "The test should check if the tested method throws an ArgumentException");
            Assert.That(methodBody, Contains.Substring("ArgumentException"), "The test should check if the tested method throws an ArgumentException");
            Assert.That(methodBody, Contains.Substring(".Message."), "The test should check if message of the thrown exception contains the text '1-3999'.");
            Assert.That(methodBody, Contains.Substring("1-3999"), "The test should check if message of the thrown exception contains the text '1-3999'.");
        }

        [MonitoredTest]
        public void _02_TheTestThatChecksIfTheValueIsInRangeShouldPass()
        {
            AssertHasValueIsInRangeTestMethod();
            AssertTestMethodPasses(_valueShouldBeInRangeMethod!);
        }

        [MonitoredTest]
        public void _03_TheTestThatChecksIfTheValueIsInRangeShouldPassForCorrectExpectations()
        {
            AssertHasValueIsInRangeTestMethod();
            AssertTestMethodPasses(_valueShouldBeInRangeMethod!, 0);
            AssertTestMethodPasses(_valueShouldBeInRangeMethod!, 4000);
            int tooBig = Random.Shared.Next(4001, int.MaxValue);
            AssertTestMethodPasses(_valueShouldBeInRangeMethod!, tooBig);
            int tooSmall = -1 * Random.Shared.Next(1, int.MaxValue);
            AssertTestMethodPasses(_valueShouldBeInRangeMethod!, tooSmall);
        }

        [MonitoredTest]
        public void _04_TheTestThatChecksIfTheValueIsInRangeShouldFailForWrongExpectation()
        {
            AssertHasValueIsInRangeTestMethod();

            foreach (int validValue in GetSomeValidValues(5))
            {
                AssertTestMethodFails(_valueShouldBeInRangeMethod!, validValue);
            }
        }

        #endregion

        #region Correctly convert test

        [MonitoredTest]
        public void _05_ShouldHaveATestThatChecksConversionOfValidNumbers()
        {
            AssertHasCorrectlyConvertTestMethod();

            var testCaseAttributes = _validValueShouldConvertCorrectlyMethod!.GetCustomAttributes()
                .OfType<TestCaseAttribute>().ToList();

            Assert.That(testCaseAttributes, Has.Count.GreaterThanOrEqualTo(4), "The method should have at least 4 test cases.");

            Assert.That(testCaseAttributes,
                Has.All.Matches((TestCaseAttribute testCase) => testCase.Arguments?.Length == 2),
                () => "All test cases must have 2 arguments (value, expected)");

            var allTestCaseValuesAreValid = testCaseAttributes.All(a =>
            {
                if (!(a.Arguments[0] is int value)) return false;

                if (!(a.Arguments[1] is string expected)) return false;
                if (string.IsNullOrEmpty(expected)) return false;

                if (value >= 1 && value <= 3999) return true;

                return false;
            });
            Assert.That(allTestCaseValuesAreValid, Is.True,
                "The method should have only 'TestCases' with the first argument an integer inside the 1-3999 range. " +
                "The second argument should be a non-empty string");

            string validRomanCharacters = "IVXLCDM";
            var allTestCaseExpectationsAreValid = testCaseAttributes.All(a =>
            {
                if (!(a.Arguments[1] is string expected)) return false;
                if (string.IsNullOrEmpty(expected)) return false;

                var hasOnlyRomanCharacters = expected.ToCharArray().All(c => validRomanCharacters.Contains(c));
                return hasOnlyRomanCharacters;
            });
            Assert.That(allTestCaseExpectationsAreValid, Is.True,
                "The method should have only 'TestCases' with the second argument being a valid Roman number. " +
                "A Roman number has no whitespaces and is build using one or more of the following characters: " +
                "I, V, X, L, C, D and M");


            var methodBody = GetMethodBodyWithoutComments(ValidValueShouldConvertCorrectlyMethodName);

            AssertCallsSutMethodAndUsesAssertThatSyntax(methodBody);

            Assert.That(methodBody, Contains.Substring("Is.EqualTo("),
                "The method should compare the converted result with the expected result'.");
        }

        [MonitoredTest]
        public void _06_TheTestThatChecksConversionOfValidNumbersShouldPass()
        {
            AssertHasCorrectlyConvertTestMethod();
            AssertTestMethodPasses(_validValueShouldConvertCorrectlyMethod!);
        }

        [MonitoredTest]
        public void _07_TheTestThatChecksConversionOfValidNumbersShouldPassForCorrectExpectations()
        {
            AssertHasCorrectlyConvertTestMethod();

            foreach (int validValue in GetSomeValidValues(100))
            {
                AssertTestMethodPasses(_validValueShouldConvertCorrectlyMethod!, validValue, R(Convert.ToInt32(validValue)));
            }
        }

        [MonitoredTest]
        public void _08_TheTestThatChecksConversionOfValidNumbersShouldFailForWrongExpectation()
        {
            AssertHasCorrectlyConvertTestMethod();

            AssertTestMethodFails(_validValueShouldConvertCorrectlyMethod!, "2", "2");
            AssertTestMethodFails(_validValueShouldConvertCorrectlyMethod!, "2", "ii");
            AssertTestMethodFails(_validValueShouldConvertCorrectlyMethod!, "3", "II");
            AssertTestMethodFails(_validValueShouldConvertCorrectlyMethod!, "4", "IIII");
            AssertTestMethodFails(_validValueShouldConvertCorrectlyMethod!, "4", "iv");
            AssertTestMethodFails(_validValueShouldConvertCorrectlyMethod!, "9", "VIIII");
            AssertTestMethodFails(_validValueShouldConvertCorrectlyMethod!, "10", "VV");
            AssertTestMethodFails(_validValueShouldConvertCorrectlyMethod!, "30", "XXXX");
            AssertTestMethodFails(_validValueShouldConvertCorrectlyMethod!, "40", "XXXX");
            AssertTestMethodFails(_validValueShouldConvertCorrectlyMethod!, "60", "XL");
            AssertTestMethodFails(_validValueShouldConvertCorrectlyMethod!, "90", "LXXXX");
            AssertTestMethodFails(_validValueShouldConvertCorrectlyMethod!, "400", "CCCC");
            AssertTestMethodFails(_validValueShouldConvertCorrectlyMethod!, "500", "CCCCC");
            AssertTestMethodFails(_validValueShouldConvertCorrectlyMethod!, "900", "DCCCC");
            AssertTestMethodFails(_validValueShouldConvertCorrectlyMethod!, "1000", "DD");

            foreach (var validValue in GetSomeValidValues(10))
            {
                AssertTestMethodFails(_validValueShouldConvertCorrectlyMethod!, validValue, Guid.NewGuid().ToString());
                AssertTestMethodFails(_validValueShouldConvertCorrectlyMethod!, validValue, "");
            }
        }

        #endregion

        #region Helpers
        private IList<int> GetSomeValidValues(int numberOfValues)
        {
            var values = new List<int>();
            for (int i = 0; i < numberOfValues; i++)
            {
                values.Add(Random.Shared.Next(1, 4000));
            }
            return values;
        }

        private void AssertCallsSutMethodAndUsesAssertThatSyntax(string methodBody)
        {
            Assert.That(methodBody, Contains.Substring(".Convert("),
                "The method should call the 'Convert' method of a 'RomanNumberConverter' instance.");
            Assert.That(methodBody, Contains.Substring("Assert.That("),
                "The method should use the 'Assert.That' method of NUnit.");
        }

        private string R(int a)
        {
            if (a >= 1000) return "M" + R(a - 1000);
            if (a >= 900) return "CM" + R(a - 900);
            if (a >= 500) return "D" + R(a - 500);
            if (a >= 400) return "CD" + R(a - 400);
            if (a >= 100) return "C" + R(a - 100);
            if (a >= 90) return "XC" + R(a - 90);
            if (a >= 50) return "L" + R(a - 50);
            if (a >= 40) return "XL" + R(a - 40);
            if (a >= 10) return "X" + R(a - 10);
            if (a >= 9) return "IX" + R(a - 9);
            if (a >= 5) return "V" + R(a - 5);
            if (a >= 4) return "IV" + R(a - 4);
            if (a >= 1) return "I" + R(a - 1);
            return string.Empty;
        }

        private void AssertHasValueIsInRangeTestMethod()
        {
            AssertHasTestMethod(_valueShouldBeInRangeMethod, ValueShouldBeInRangeMethodName, 1);

            var parameter = _valueShouldBeInRangeMethod!.GetParameters().First();
            Assert.That(parameter.ParameterType, Is.EqualTo(typeof(int)),
                $"The parameter of the '{ValueShouldBeInRangeMethodName}' method should be of type 'string'.");
        }

        private void AssertHasCorrectlyConvertTestMethod()
        {
            AssertHasTestMethod(
                _validValueShouldConvertCorrectlyMethod, ValidValueShouldConvertCorrectlyMethodName, 2);

            var parameters = _validValueShouldConvertCorrectlyMethod!.GetParameters().ToList();
            Assert.That(parameters, Has.One.Matches((ParameterInfo p) => p.ParameterType == typeof(int)),
                $"One parameter of the '{ValidValueShouldConvertCorrectlyMethodName}' method should be of type 'int'.");
            Assert.That(parameters, Has.One.Matches((ParameterInfo p) => p.ParameterType == typeof(string)),
                $"One parameter of the '{ValidValueShouldConvertCorrectlyMethodName}' method should be of type 'string'.");
        }

        private void AssertHasTestMethod(MethodInfo? testMethod, string methodName, int expectedParameterCount)
        {
            Assert.That(testMethod, Is.Not.Null,
                $"Could not find test method with name '{methodName}'.");

            Assert.That(testMethod!.GetParameters().Length, Is.EqualTo(expectedParameterCount),
                 $"The method '{methodName}' should have {expectedParameterCount} parameter(s).");

            if (expectedParameterCount == 0)
            {
                var testAttribute = testMethod.GetCustomAttributes()
                    .OfType<TestAttribute>().FirstOrDefault();
                Assert.That(testAttribute, Is.Not.Null,
                    "No 'Test' attribute is defined for the method.");
            }
        }

        private string GetMethodBodyWithoutComments(string methodName)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(_testClassContent);
            var root = syntaxTree.GetRoot();
            var method = root
                .DescendantNodes()
                .OfType<MethodDeclarationSyntax>()
                .FirstOrDefault(md => md.Identifier.ValueText.Equals(methodName));

            if (method == null) return string.Empty;

            var bodyBuilder = new StringBuilder(); //no pun intended :)
            foreach (var statement in method!.Body!.Statements)
            {
                bodyBuilder.AppendLine(statement.ToString());
            }
            return bodyBuilder.ToString();
        }

        private void AssertTestMethodFails(MethodInfo testMethod, params object[] parameters)
        {
            Assert.That(() => testMethod.Invoke(_romanNumberConverterTestsInstance, parameters), Throws.InstanceOf<Exception>(),
                () => $"{testMethod.Name}({StringyfyParameters(parameters)}) should fail, but doesn't.");
        }

        private void AssertTestMethodPasses(MethodInfo testMethod)
        {
            var testCaseAttributes = testMethod.GetCustomAttributes()
                .OfType<TestCaseAttribute>().ToList();

            if (testCaseAttributes.Any())
            {
                foreach (var testCaseAttribute in testCaseAttributes)
                {
                    AssertTestMethodPasses(testMethod, testCaseAttribute.Arguments!);
                }
            }
            else
            {
                AssertTestMethodPasses(testMethod, Array.Empty<object>());
            }
        }

        private void AssertTestMethodPasses(MethodInfo testMethod, params object[] parameters)
        {
            Assert.That(() => testMethod.Invoke(_romanNumberConverterTestsInstance, parameters), Throws.Nothing,
                () => $"{testMethod.Name}({StringyfyParameters(parameters)}) should pass, but doesn't.");
        }

        private string StringyfyParameters(params object[] parameters)
        {
            if (parameters == null || parameters.Length == 0) return string.Empty;

            var builder = new StringBuilder();
            foreach (var parameter in parameters)
            {
                if (parameter is string)
                {
                    builder.Append($"\"{parameter}\"");
                }
                else
                {
                    builder.Append(parameter);
                }

                builder.Append(", ");
            }

            builder.Remove(builder.Length - 2, 2);
            return builder.ToString();
        }
        #endregion
    }
}