using System;
using System.Collections.Generic;
using MongoDB.Bson;

namespace Questions.ApiModels
{

    public class QuestionBinding
    {

        public Question Question { get; set; }
        public IList<QuestionPair> WrongPairs { get; set; }
        public UserRank Rank { get; set; }
    }
}
