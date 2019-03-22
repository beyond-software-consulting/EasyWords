using System;
using QuestionModel;

namespace QuestionManager
{
    public class QuestionManager:IQuestionManager
    {
        public QuestionManager()
        {
        }

        public Question GetQuestion(string Dictionary)
        {
            return new Question() { ID = 1, QuestionText = "Test", Answer = "Test", Dictionary = "EN", DiffcultLevel = 1 };
        }
    }
}
