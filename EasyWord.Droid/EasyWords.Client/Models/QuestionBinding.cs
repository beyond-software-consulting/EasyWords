using System;
using System.Collections.Generic;

namespace EasyWords.Client.Models
{
    public class QuestionBinding
    {
        public Question Question { get; set; }
        public IList<QuestionPair> WrongPairs { get; set; }
        public UserRank Rank { get; set; }
    }
}
