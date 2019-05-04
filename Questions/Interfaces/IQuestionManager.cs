using System;
using System.Threading.Tasks;
using Questions.Models;

using MongoDB.Bson;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace Questions.Interfaces
{
    public interface IQuestionManager
    {

        Task<ActionResult<ApiModels.QuestionBinding>> GetQuestion(int userId,int userClientId,int dictionaryId,int questionTypeId);

        Task<ActionResult> SaveAnswer(ApiModels.Answer answer);
        string AddQuestion(Question question);

        Task<ActionResult> GenerateTestData();

         
    }
}