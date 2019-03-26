using System;
using AnswerModel;
using DataBaseProvider;
using QuestionModel;
using QuestionProvider;

namespace QuestionManager
{
    public class QuestionManager:IQuestionManager
    {
        IQuestionProvider provider;
        QuestionDBContext dbContext;
        public QuestionManager(IQuestionProvider Provider,QuestionDBContext db)
        {
            provider = Provider;
            dbContext = db;
        }

        public Question GetQuestion(string Dictionary)
        {
            return provider.GetNextQuestion();
        }

        public string SaveAnswer(AnswerModel.Answer answer)
        {
            
            return "";
        }
    }
}
