using System;
using MongoDB.Bson;
using Questions.Interfaces;

namespace Questions.Models
{
    public class Answer:IModelBase
    {
 

        public int QuestionID { get; set; }
        public string AnswerText { get; set; }

        public DateTime CreateDate { get; set; }
        public int Id { get; set; }
       
        public ObjectId InternalId { get; set; }
        public ObjectId _Id { get; set; }
        public DateTime DateOfCreation { get; set; }

        public string ToJson()
        {
            throw new NotImplementedException();
        }
    }
}
