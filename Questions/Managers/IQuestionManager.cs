﻿using System;
using QuestionModel;
using AnswerModel;
using System.Threading.Tasks;

namespace QuestionManager
{
    public interface IQuestionManager
    {

        Question GetQuestion(string Dictionary);

        string SaveAnswer(AnswerModel.Answer answer);
         
    }
}