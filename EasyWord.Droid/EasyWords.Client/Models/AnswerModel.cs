using System;
namespace EasyWords.Client.Models
{
    public class AnswerModel
    {
        public int QuestionID { get; set; }
        public string AnswerText { get; set; }
        public int UserId { get; set; }
        public int UserClientId { get; set; }
        public int DictionaryId { get; set; }
    }
}
