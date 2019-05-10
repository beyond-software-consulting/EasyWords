using System;
using Microsoft.AspNetCore.Mvc;

namespace Questions.ApiModels
{
    public class AnswerResult
    {
        public bool IsCorrect { get; set; }
        public UserRank Rank { get; set; }


    }
}
