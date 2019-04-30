using System;
namespace Questions.Models
{
    public class CommunityScore
    {
 

        public int PairID { get; set; }
        public int DictionaryID { get; set; }
        public int TotalCorrect { get; set; }
        public int TotalWrong { get; set; }
        public int TotalCommunityScore { get; set; }
    }
}
