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

        Task<ApiModels.QuestionBinding> GetQuestion(int userId,int userClientId,int dictionaryId,int questionTypeId);

        Task<IActionResult> SaveAnswer(ApiModels.Answer answer);


        Task<IActionResult> GenerateTestData();

         
    }
}