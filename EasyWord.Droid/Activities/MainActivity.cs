using System;
using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using EasyWord.Droid.Enums;
using EasyWords.Client;
using EasyWords.Client.Interfaces;
using EasyWords.Client.Models;
using Unity;
namespace EasyWord.Droid
{


    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        IQuestionConsumer questionConsumer;
        QuestionBinding currentQuestion; 
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            questionConsumer = App.Container.Resolve<QuestionConsumer>();




            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);
            GetNextQuestion();

            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);


           

        }

        private void GetNextQuestion()
        {
            var questionBindingTask = questionConsumer.GetQuestion(1, 1, 1, 1);
            questionBindingTask.Wait();
            var questionBinding = questionBindingTask.Result;
            TextView questionText = FindViewById<TextView>(Resource.Id.questionTitle);
            if (questionBinding.Question.QuestionWordNumber == 1)
            {
                questionText.SetText(questionBinding.Question.InLanguage1, TextView.BufferType.Normal);
            }
            else
            {
                questionText.SetText(questionBinding.Question.InLanguage2, TextView.BufferType.Normal);
            }
            GenerateAnswerButtons(questionBinding);
            currentQuestion = questionBinding;
        }

        private void GenerateAnswerButtons(QuestionBinding questionBinding)
        {
            LinearLayout buttonLayout = FindViewById<LinearLayout>(Resource.Id.questionContentLayout);
            var layoutParams = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, LinearLayout.LayoutParams.MatchParent);
            layoutParams.SetMargins(20, 10, 20, 10);
            var idGenerator = new Random(int.MaxValue);
            buttonLayout.RemoveViews(1, buttonLayout.ChildCount - 1);
            foreach (var item in questionBinding.WrongPairs)
            {
                EasyWordButton btn = new EasyWordButton(this);
                var id = idGenerator.Next();
                btn.Id = id;
                if (questionBinding.Question.QuestionWordNumber == 1)
                {
                    btn.SetText(item.InLanguage2, TextView.BufferType.Normal);
                }
                else
                {
                    btn.SetText(item.InLanguage1, TextView.BufferType.Normal);
                }

                if(item.PairID == questionBinding.Question.PairID)
                {
                    btn.IsQuestionAnswer = true;
                }
                else
                {
                    btn.IsQuestionAnswer = false;
                }
                btn.PairItem = item;
                btn.Click += AnswerButtonClicked;
                buttonLayout.AddView(btn);
            }


        }



        private void SendAnswer(AnswerModel answer)
        {
           // var answerResultTask = questionConsumer.SendAnswer(answer);
           // answerResultTask.Wait();
           // var answerResult = answerResultTask.Result;

            if (true)
            {
                MarkCorrectAnswer();
            }
            else
            {
                GetNextQuestion();
            }
        }
        private void MarkCorrectAnswer()
        {
            LinearLayout buttonLayout = FindViewById<LinearLayout>(Resource.Id.questionContentLayout);
            int buttonCount = buttonLayout.ChildCount;
            for (int i = 0; i < buttonCount; i++)
            {
                EasyWordButton button = buttonLayout.GetChildAt(i) as EasyWordButton;
                if (button == null) continue;
                if(currentQuestion.Question.PairID == button.PairItem.PairID)
                {
                    button.SetBackgroundColor(Android.Graphics.Color.GreenYellow);
                    break; 
                }

            }
        }


        #region Control Event Methods
        private void AnswerButtonClicked(object sender, EventArgs eventArgs)
        {
            Button btn = (Button)sender;

            var answer = new AnswerModel()
            {
                UserId = 1,
                UserClientId = 1,
                AnswerText = btn.Text,
                QuestionID = currentQuestion.Question.QuestionID,
                DictionaryId = currentQuestion.Question.DictionaryID
            };

            SendAnswer(answer);


        }
        #endregion

        #region View Builtin functions
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            if (id == Resource.Id.action_settings)
            {
                return true;
            }
            Console.WriteLine(item.ItemId);
            return base.OnOptionsItemSelected(item);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
        #endregion


    }
}

