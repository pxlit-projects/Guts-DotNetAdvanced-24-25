using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Moq;
using QuizApplication.Web.Controllers;

namespace QuizApplication.Tests.Web;

[ExerciseTestFixture("dotnet2", "2-MVCEF", "QuizApplication",
        @"QuizApplication.Web\Controllers\HomeController.cs")]
public class HomeControllerTests
{
    private HomeController _controller = null!;

    [SetUp]
    public void Setup()
    {
        var mockLogger = new Mock<ILogger<HomeController>>();
        _controller = new HomeController(mockLogger.Object);
    }


    [MonitoredTest("HomeController - Index should return something")]
    public void _01_Index_ShouldReturnDefaultView()
    {
        ViewResult? result = _controller.Index() as ViewResult;
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.ViewName, Is.Null);
    }

    [MonitoredTest("HomeController - About should return something")]
    public void _02_About_ReturnsContent()
    {
        ViewResult? result = _controller.About() as ViewResult;
        Assert.That(result, Is.Not.Null);
    }

    [TearDown]
    public void TearDown()
    {
        _controller?.Dispose();
    }

}