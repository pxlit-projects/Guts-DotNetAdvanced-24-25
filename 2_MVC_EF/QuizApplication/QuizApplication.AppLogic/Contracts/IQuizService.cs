using QuizApplication.Domain;

namespace QuizApplication.AppLogic.Contracts
{
    public interface IQuizService
    {
        IReadOnlyList<Category> GetAllCategories();

        /* This function should add an extra Answer to the list of answers in the question.
            This answer is 'None of the answers is correct.' and is the correct answer if none
            of the returned answers is correct.
         */
        Question GetQuestionByIdWithAnswersAndExtra(int id);
        IReadOnlyList<Question> GetQuestionsInCategory(int id);
    }
}