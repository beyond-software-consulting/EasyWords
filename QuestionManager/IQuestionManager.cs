using System;
using QuestionModel;
using AnswerModel;
namespace QuestionManager
{
    public interface IQuestionManager
    {

        Question GetQuestion(string Dictionary);

        string SaveAnswer(AnswerModel.Answer answer);
         
    }
}