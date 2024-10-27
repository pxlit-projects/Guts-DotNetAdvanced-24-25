using Microsoft.EntityFrameworkCore;
using QuizApplication.Domain;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace QuizApplication.Infrastructure
{
    internal class QuizDbContext : DbContext
    {
        public DbSet<Question> Questions { get; set; } = null!; //with '!' we are telling the compiler that we are sure that this property will never be null (the base class 'DbContext' takes care of this)

        public QuizDbContext(DbContextOptions<QuizDbContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        private IList<Answer> GetSeedAnswers()
        {
            return new List<Answer>()
                    {
            new Answer() { Id = 1, QuestionId = 1, AnswerText = "Rome", IsCorrect = false },
            new Answer() { Id = 2, QuestionId = 1, AnswerText = "Madrid", IsCorrect = false },
            new Answer() { Id = 3, QuestionId = 1, AnswerText = "Berlin", IsCorrect = false },
            new Answer() { Id = 4, QuestionId = 1, AnswerText = "Brussels", IsCorrect = false },
            new Answer() { Id = 5, QuestionId = 2, AnswerText = "Jupiter", IsCorrect = false },
            new Answer() { Id = 6, QuestionId = 2, AnswerText = "Venus", IsCorrect = false },
            new Answer() { Id = 7, QuestionId = 2, AnswerText = "Mars", IsCorrect = true },
            new Answer() { Id = 8, QuestionId = 2, AnswerText = "Saturn", IsCorrect = false },
            new Answer() { Id = 9, QuestionId = 3, AnswerText = "Charles Dickens", IsCorrect = false },
            new Answer() { Id = 10, QuestionId = 3, AnswerText = "William Shakespeare", IsCorrect = true },
            new Answer() { Id = 11, QuestionId = 3, AnswerText = "Jane Austen", IsCorrect = false },
            new Answer() { Id = 12, QuestionId = 3, AnswerText = "Mark Twain", IsCorrect = false },
            new Answer() { Id = 13, QuestionId = 4, AnswerText = "Mercury", IsCorrect = true },
            new Answer() { Id = 14, QuestionId = 4, AnswerText = "Uranus", IsCorrect = false },
            new Answer() { Id = 15, QuestionId = 4, AnswerText = "Neptunus", IsCorrect = false },
            new Answer() { Id = 16, QuestionId = 4, AnswerText = "Earth", IsCorrect = false },
            new Answer() { Id = 17, QuestionId = 5, AnswerText = "Oxygen", IsCorrect = false },
            new Answer() { Id = 18, QuestionId = 5, AnswerText = "Nitrogen", IsCorrect = false },
            new Answer() { Id = 19, QuestionId = 5, AnswerText = "Carbon dioxide", IsCorrect = true },
            new Answer() { Id = 20, QuestionId = 5, AnswerText = "Hydrogen", IsCorrect = false },
            new Answer() { Id = 21, QuestionId = 6, AnswerText = "Vincent van Gogh", IsCorrect = false },
            new Answer() { Id = 22, QuestionId = 6, AnswerText = "Pablo Picasso", IsCorrect = false },
            new Answer() { Id = 23, QuestionId = 6, AnswerText = "Leonardo da Vinci", IsCorrect = true },
            new Answer() { Id = 24, QuestionId = 6, AnswerText = "Michelangelo", IsCorrect = false },
            new Answer() { Id = 25, QuestionId = 7, AnswerText = "Elephant", IsCorrect = false },
            new Answer() { Id = 26, QuestionId = 7, AnswerText = "Giraffe", IsCorrect = false },
            new Answer() { Id = 27, QuestionId = 7, AnswerText = "Blue whale", IsCorrect = true },
            new Answer() { Id = 28, QuestionId = 7, AnswerText = "Lion", IsCorrect = false },
            new Answer() { Id = 29, QuestionId = 8, AnswerText = "Beijing", IsCorrect = false },
            new Answer() { Id = 30, QuestionId = 8, AnswerText = "Seoul", IsCorrect = false },
            new Answer() { Id = 31, QuestionId = 8, AnswerText = "Shanghai", IsCorrect = false },
            new Answer() { Id = 32, QuestionId = 8, AnswerText = "Tokyo", IsCorrect = true },
            new Answer() { Id = 33, QuestionId = 9, AnswerText = "Oxygen", IsCorrect = false },
            new Answer() { Id = 34, QuestionId = 9, AnswerText = "Carbon dioxide", IsCorrect = true },
            new Answer() { Id = 35, QuestionId = 9, AnswerText = "Hydrogen", IsCorrect = false },
            new Answer() { Id = 36, QuestionId = 9, AnswerText = "Nitrogen", IsCorrect = false },
            new Answer() { Id = 37, QuestionId = 10, AnswerText = "Brain", IsCorrect = false },
            new Answer() { Id = 38, QuestionId = 10, AnswerText = "Skin", IsCorrect = true },
            new Answer() { Id = 39, QuestionId = 10, AnswerText = "Heart", IsCorrect = false },
            new Answer() { Id = 40, QuestionId = 10, AnswerText = "Liver", IsCorrect = false },
        };
        }

        private IList<Question> GetSeedQuestions()
        {
            return new List<Question>
        {
            new Question() { Id = 1, QuestionString = "What is the capital of France?", CategoryId = 4 },
            new Question() { Id = 2, QuestionString = "Which planet is known as the 'Red Planet'?", CategoryId = 3 },
            new Question() { Id = 3, QuestionString = "Who wrote the play 'Romeo and Juliet'?", CategoryId = 2 },
            new Question() { Id = 4, QuestionString = "What is the closest planet to the sun?", CategoryId = 3 },
            new Question() { Id = 5, QuestionString = "Which gas do plants absorb from the atmosphere during photosynthesis?", CategoryId = 1 },
            new Question() { Id = 6, QuestionString = "Who painted the Mona Lisa?", CategoryId = 2 },
            new Question() { Id = 7, QuestionString = "What is the largest mammal in the world?", CategoryId = 1 },
            new Question() { Id = 8, QuestionString = "What is the capital of Japan?", CategoryId = 4 },
            new Question() { Id = 9, QuestionString = "Which gas do humans exhale when they breathe?", CategoryId = 1 },
            new Question() { Id = 10, QuestionString = "What is the largest organ in the human body?", CategoryId = 1 }
        };

        }
    }
}
