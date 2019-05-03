using System;
using System.IO;
using System.Threading.Tasks;
using Autofac;
using EventBus;
using EventBus.Interfaces;
using EventBusRabbitMQProvider;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Questions.Interfaces;
using Questions.Managers;
using RabbitMQ.Client;

namespace Tests
{

    public class QuestionTests
    {
        IConfiguration configuration;
        ICacheProvider cacheProvider;
        Questions.Controllers.QuestionsController questionController;
        IQuestionManager questionManager;
        IDatabaseRepository<Questions.Models.Question> questionDatabaseProvider;
        IDatabaseRepository<Questions.Models.Pair> pairDatabaseProvider;
        IDatabaseRepository<Questions.Models.UserPairScore> userScoreDatabaseProvider;

        [SetUp]
        public void Setup()
        {
            configuration = GetConfiguration();

            RegisterDatabaseProviders();

            RegisterQuestionManager();

            RegisterQuestionController();

        }

        private void RegisterQuestionManager()
        {

            questionManager = new QuestionManager(questionDatabaseProvider,
                pairDatabaseProvider,
                userScoreDatabaseProvider,
                cacheProvider);

        }
        private void RegisterQuestionController()
        {

            var mockEventBus = new Mock<IEventBus>();
            questionController = new Questions.Controllers.QuestionsController(questionManager, mockEventBus.Object);
        }

        private void RegisterDatabaseProviders()
        {

            questionDatabaseProvider = new Questions.Providers.Database.QuestionDatabaseRepository(configuration["QuestionDbConnectionString"]);
            pairDatabaseProvider = new Questions.Providers.Database.PairDatabaseRepository(configuration["QuestionDbConnectionString"]);
            userScoreDatabaseProvider = new Questions.Providers.Database.UserPairScoreDatabaseRepository(configuration["QuestionDbConnectionString"]);

            cacheProvider = new Questions.Providers.Cache.CouchbaseCacheProvider(configuration["CouchbaseConnectionString"]);

        }

        private static IConfiguration GetConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();

            var config = builder.Build();

            return builder.Build();
        }

        [Test]
        public void GetQuestionWithNormalValues()
        {

            var questionOp = questionController.GetQuestion(1, 1, 1, 1);
            questionOp.Wait();
            var question = questionOp.Result;
            Assert.IsNotNull(question);

        }

        [Test]
        public void GetQuestionWithInvalidDictioary()
        {

            Assert.Catch(() =>
            {
                var questionOp = questionController.GetQuestion(1, 1, 1234, 1);
                questionOp.Wait();
            });

        }

        [Test]
        public void GetQuestionWithInvalidQuestionTypeID()
        {

            Assert.Catch(() =>
            {
                var questionOp = questionController.GetQuestion(1, 1, 1, 1111);
                questionOp.Wait();
            });
        }

        [Test]
        public async Task GetQuestionAndCheckWrongPairs()
        {

            var question = await questionController.GetQuestion(1, 1, 1, 1);
            Assert.GreaterOrEqual(question.Value.WrongPairs.Count, 1);

        }
        [Test]
        public async Task SetAnswerReturnOkResult()
        {
            var answer  = new Questions.Models.Answer();
            answer.QuestionID = 11794;
            answer.AnswerText = "yaratmak";
            var result = await questionController.SaveAnswer(answer);
            Assert.AreEqual(result, new OkResult());
            
        }

        [Test]
        public async Task SetAnswerReturnBadRequestResult()
        {
            var answer = new Questions.Models.Answer();
            answer.QuestionID = 11794;
            answer.AnswerText = "yaratmakaaa";
            var result = await questionController.SaveAnswer(answer);
            Assert.AreEqual(result, new BadRequestResult());

        }
    }
}