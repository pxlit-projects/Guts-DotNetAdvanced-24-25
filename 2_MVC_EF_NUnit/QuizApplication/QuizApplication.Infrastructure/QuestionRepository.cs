using QuizApplication.AppLogic.Contracts;
using QuizApplication.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizApplication.Infrastructure
{
    internal class QuestionRepository : IQuestionRepository
    {
        public QuestionRepository(QuizDbContext dbContext)
        {

        }

        public IReadOnlyList<Question> GetByCategoryId(int categoryId)
        {
            throw new NotImplementedException();
        }

        public Question GetByIdWithAnswers(int id)
        {
            throw new NotImplementedException();
        }
    }
}
