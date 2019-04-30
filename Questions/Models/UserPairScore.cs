using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using Questions.Helpers;
using Questions.Interfaces;

namespace Questions.Models
{
    [Model(CollectionName =  "UserPairScores")]
    public class UserPairScore
    {

        public int PairId { get; set; }
        public int UserId { get; set; }
        public int DictionaryId { get; set; }
        [BsonElement("NumberOfCorrectAnswers")]
        [BsonIgnoreIfNull]
        public int TotalCorrect { get; set; }
        [BsonElement("NumberOfWrongAnswers")]
        [BsonIgnoreIfNull]
        public int TotalWrong { get; set; }
      
        public string Score { get; set; }

        [JsonIgnore]
        [BsonId]
        public ObjectId _Id { get; set; }

      
    }
}