using System;
using QuestionModel;

namespace QuestionManager
{
    public interface IQuestionManager
    {

        Question GetQuestion(string Dictionary);
         
    }
}