using System;
using QuestionModel;

namespace QuestionProvider
{
    public interface IQuestionProvider
    {
        float CalculateRank();
        Question GetNextQuestion();
    }
}
