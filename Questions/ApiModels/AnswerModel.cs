using System;
using MongoDB.Bson;
using Questions.Interfaces;

namespace Questions.ApiModels
{
    public class Answer
    {
 

        public int QuestionID { get; set; }
        public string AnswerText { get; set; }
        public int UserId { get; set; }
        public int DictionaryId { get; set; }


    }
}
