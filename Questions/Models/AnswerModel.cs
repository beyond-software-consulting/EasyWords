using System;
namespace AnswerModel
{
    public class Answer:IAnswerModel
    {
        public Answer()
        {
        }

        public int QuestionID { get; set; }
        public string AnswerText { get; set; }
        public int ID { get; set; }
    }
}
