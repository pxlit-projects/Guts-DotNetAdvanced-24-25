using QuizApplication.Domain;

namespace QuizApplication.Web.Models
{
    public class QuestionViewModel
    {
        public QuestionViewModel(Question question)
        {
            Question = question;
        }

        public Question Question { get; set; }
    }
}
