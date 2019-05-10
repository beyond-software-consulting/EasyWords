
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using EasyWords.Client;
using EasyWords.Client.Interfaces;
using Unity;

namespace EasyWord.Droid
{
    [Application]
    public class App : Application
    {
        public static UnityContainer Container { get; private set; }
        private static void Initialize()
        {
            App.Container = new UnityContainer();

            App.Container.RegisterType<IQuestionConsumer, QuestionConsumer>();

        }
        public App(IntPtr javaReference, JniHandleOwnership transfer)
        {


        }

        public override void OnCreate()
        {
            Initialize();

            base.OnCreate();
        }
    }
}
