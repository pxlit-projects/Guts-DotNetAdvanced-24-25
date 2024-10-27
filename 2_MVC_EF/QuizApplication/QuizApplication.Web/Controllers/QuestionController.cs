using Microsoft.AspNetCore.Mvc;
using QuizApplication.AppLogic;
using QuizApplication.AppLogic.Contracts;
using QuizApplication.Domain;
using QuizApplication.Web.Models;
using System.Collections.Generic;

namespace QuizApplication.Web.Controllers
{
    public class QuestionController : Controller
    {
        private readonly ILogger<QuestionController> _logger;

        public QuestionController(ILogger<QuestionController> logger, IQuizService quizService)
        {
            _logger = logger;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult QuestionsInCategory(int id)
        {
            return View();
        }

        public IActionResult QuestionWithAnswers(int id)
        {
            return View();
        }


    }
}
