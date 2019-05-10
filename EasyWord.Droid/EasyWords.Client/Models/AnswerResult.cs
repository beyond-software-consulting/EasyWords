using System;
namespace EasyWords.Client.Models
{
    public class AnswerResult
    {
        public bool IsCorrect { get; set; }
        public UserRank Rank { get; set; }
    }
}
