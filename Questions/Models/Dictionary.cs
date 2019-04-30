using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using Questions.Helpers;
using Questions.Interfaces;

namespace Questions.Models
{
    [Model(CollectionName ="Dictionaries")]
    public class Dictionary:IModelBase
    {


        [JsonIgnore]
        [BsonId]
        public ObjectId _Id { get; set; }

        [IdentityField]
        public int Id { get; set; }

        public string Name { get; set; }
        public string Language1 { get; set; }
        public string Language2 { get; set; }
        public bool Active { get; set; }
        public string Description { get; set; }
        public int VisualOrderId { get; set; }
        public bool IsOfficial { get; set; }

        public DateTime DateOfCreation { get; set; }

        public string ToJson()
        {
            throw new NotImplementedException();
        }
    }
}
