using System;
using System.IO;
using Autofac;
using EventBus;
using EventBus.Interfaces;
using EventBusRabbitMQProvider;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Questions.Interfaces;
using Questions.Managers;
using RabbitMQ.Client;

namespace Tests
{
    [TestFixture]
    public class QuestionTests
    {
        IConfiguration configuration;
        IConfiguration Configuration { get; }
        ICacheProvider cacheProvider;
        IRabbitMQPersistentConnection rabbitMQ;
        IEventBus eventBus;
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
                                
            RegisterRabbitMQPersistentConnection();

            RegisterEventBus();

            RegisterQuestionController();
        

        }

        private void RegisterQuestionManager() {

            questionManager = new QuestionManager(questionDatabaseProvider,
                pairDatabaseProvider,
                userScoreDatabaseProvider,
                cacheProvider);

        }
        private void RegisterQuestionController() {

            questionController = new Questions.Controllers.QuestionsController(questionManager, eventBus);
        }

        private void RegisterDatabaseProviders() {

            questionDatabaseProvider = new Questions.Providers.Database.QuestionDatabaseRepository(configuration["QuestionDbConnectionString"]);
            pairDatabaseProvider = new Questions.Providers.Database.PairDatabaseRepository(configuration["QuestionDbConnectionString"]);
            userScoreDatabaseProvider = new Questions.Providers.Database.UserPairScoreDatabaseRepository(configuration["QuestionDbConnectionString"]);

            cacheProvider = new Questions.Providers.Cache.CouchbaseCacheProvider(configuration["CouchbaseConnectionString"]);

        }

        private void RegisterRabbitMQPersistentConnection() 
        {

            var eventB = new Mock<IEventBus>();

            var loggerMock = new Mock<ILogger<DefaultRabbitMQPersistentConnection>>();
            var factory = new ConnectionFactory()
            {
                HostName = Configuration["EventBusConnection"]
            };

            if (!string.IsNullOrEmpty(Configuration["EventBusUserName"]))
            {
                factory.UserName = Configuration["EventBusUserName"];
            }

            if (!string.IsNullOrEmpty(Configuration["EventBusPassword"]))
            {
                factory.Password = Configuration["EventBusPassword"];
            }

            var retryCount = 5;
            if (!string.IsNullOrEmpty(Configuration["EventBusRetryCount"]))
            {
                retryCount = int.Parse(Configuration["EventBusRetryCount"]);
            }

            rabbitMQ = new DefaultRabbitMQPersistentConnection(factory, loggerMock.Object, retryCount);

        }

        private void RegisterEventBus() {

            var eventBusLifetimeScope = new Mock<ILifetimeScope>();
            var eventBuslogger = new Mock<ILogger<EventBusRabbitMQ>>();
            var eventBusSubcriptionsManager = new Mock<IEventBusSubscriptionsManager>();

            eventBus = new EventBusRabbitMQ(rabbitMQ, eventBuslogger.Object, eventBusLifetimeScope.Object, eventBusSubcriptionsManager.Object);

        }

        private static IConfiguration GetConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();

            var config = builder.Build();


            return builder.Build();
        }

        [Test]
        public void GetQuestion()
        {

            var question = questionController.GetQuestion(1, 1, 12312, 1231);
            Assert.IsNull(question);

            question = questionController.GetQuestion(1, 1, 1, 1231);
            Assert.IsNull(question);

            question = questionController.GetQuestion(1, 1, 1, 1);
            Assert.IsNotNull(question);

            Assert.GreaterOrEqual(question.Value.WrongPairs.Count, 1);
        }

        [Test]
        public void SetAnswer()
        { 
        }
    }
}