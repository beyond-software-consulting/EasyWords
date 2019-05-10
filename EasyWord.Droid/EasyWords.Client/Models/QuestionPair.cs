using System;
namespace EasyWords.Client.Models
{
    public class QuestionPair
    {
        public int PairID { get; set; }
        public string InLanguage1 { get; set; }
        public string InLanguage2 { get; set; }
        public string Conjugation { get; set; }
        public string SoundEx1 { get; set; }
        public string SoundEx2 { get; set; }
    }
}
