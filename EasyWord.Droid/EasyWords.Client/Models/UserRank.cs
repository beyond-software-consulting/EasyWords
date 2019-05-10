using System;
namespace EasyWords.Client.Models
{
    public class UserRank
    {
        public int TotalCorrect { get; set; }
        public int TotalWrong { get; set; }
        public int Score { get; set; }
        public int Rank { get; set; }
    }
}
