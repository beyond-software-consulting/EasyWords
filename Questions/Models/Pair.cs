using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using Questions.Helpers;
using Questions.Interfaces;
namespace Questions.Models
{
    [Model(CollectionName = "Pairs")]
    public class Pair : IModelBase
    {

        [IdentityField]
        public int Id { get; set; }

        [JsonIgnore]
        [BsonId]
        public ObjectId _Id { get; set; }
        public DateTime DateOfCreation { get; set; }

        public string InLanguage1 { get; set; }
        public string InLanguage2 { get; set; }
        public string Conjugation { get; set; }
        public int Active { get; set; }
        public int DictionaryId{ get; set; }
        public string SoundEx1 { get; set; }
        public string SoundEx2 { get; set; }

        public string ToJson()
        {
            throw new NotImplementedException();
        }
    }
}
