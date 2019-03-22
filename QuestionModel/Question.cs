using System;
namespace QuestionModel
{
    public class Question:IQuestionModel
    {
        public Question()
        {
        }

        public int ID { get; set; }

        public string QuestionText { get; set; }
        public string Answer { get; set; }
        public string Dictionary { get; set; }
        public int DiffcultLevel { get; set; }
    }
}
