using QuizApplication.Domain;
namespace QuizApplication.AppLogic.Contracts
{
    public interface IQuestionRepository
    {
        IReadOnlyList<Question> GetByCategoryId(int categoryId);
        Question GetByIdWithAnswers(int id);
    }
}
