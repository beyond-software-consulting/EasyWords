using System;
using Newtonsoft.Json;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Questions.Interfaces;
using Questions.Helpers;

namespace Questions.Models
{
    [Model(CollectionName ="Questions")]
    public class Question:IModelBase
    {


        [JsonIgnore]
        [BsonId]
        public ObjectId _Id { get; set; }

        [IdentityField]
        public int Id { get; set; }

        public int PairID { get; set; } 

        public int Type { get; set; }

        public int QuestionWordNumber { get; set; }

        [BsonElement("Answer")]
        public string Answer { get; set; }

        public bool WasAnswerCorrect { get; set; }

        public DateTime? DateOfAnswer { get; set; }

        public int UserClientID { get; set;}

        public int DiffcultLevel { get; set; }

        public DateTime DateOfCreation { get; set; }

  
    }
}
