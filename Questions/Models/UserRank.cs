using System;
namespace Questions.Models
{
    public class UserRank
    {
        public int UserId { get; set; }
        public int DictionaryId { get; set; }
        public int Rank { get; set; }
        public int TotalCorrectAnswer { get; set; }
        public int TotalWrongAnswer { get; set; }
        public int TotalAnswer { get; set; }

    }
}
