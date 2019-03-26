using System;
using QuestionModel;

namespace QuestionProvider
{
    public class QuestionProvider : IQuestionProvider
    {
        public QuestionProvider()
        {
        }

        public float CalculateRank()
        {
            throw new NotImplementedException();
        }

        public Question GetNextQuestion()
        {
            return new Question() { ID = 1, QuestionText = "What is this?", Answer = "this is a pen", Dictionary = "EN", DiffcultLevel = 1 };
        }
    }
}