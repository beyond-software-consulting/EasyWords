using System;
namespace Questions.ApiModels
{
    public class Question
    {
        public int PairID { get; set; }
        public int DictionaryID { get; set; }
        public int QuestionID { get; set; }
        public string InLanguage1 { get; set; }
        public string InLanguage2 { get; set; }
        public string Conjugation { get; set; }
        public string SoundEx1 { get; set; }
        public string SoundEx2 { get; set; }
    }
}
