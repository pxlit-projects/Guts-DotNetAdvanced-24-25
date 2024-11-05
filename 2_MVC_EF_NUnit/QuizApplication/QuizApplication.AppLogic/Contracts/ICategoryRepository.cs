using QuizApplication.Domain;

namespace QuizApplication.AppLogic.Contracts
{
    public interface ICategoryRepository
    {
        IReadOnlyList<Category> GetAll();
        Category? GetById(int id);
    }
}
