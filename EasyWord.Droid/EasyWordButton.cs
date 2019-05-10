using System;
using Android.Widget;
using EasyWords.Client.Models;

namespace EasyWord.Droid
{
    public class EasyWordButton:Button
    {

        public QuestionPair PairItem { get; set; }
        public bool IsQuestionAnswer { get; set; }
        public EasyWordButton(Android.Content.Context context) : base(context)
        {

        }
    }
}
