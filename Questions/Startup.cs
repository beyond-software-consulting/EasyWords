using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using RabbitMQ.Client;
using EventBus;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using EventBus.Interfaces;
using EventBusRabbitMQProvider;
using Questions.Events;
using Questions.EventHandlers;
using Questions.Interfaces;
using Questions.Models;
using Questions.Providers;
using Questions.Managers;
using Questions.Adapters;
using Questions.Providers.Database;
using Questions.Providers.Cache;

namespace Questions
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddApplicationInsightsTelemetry(Configuration);

            services.AddLogging();


            services.AddMemoryCache();

            ///Question Microservice db entegration
            //services.AddQuestionDataBaseContext();

            services.AddTransient<IDatabaseRepository<Pair>>(p => new PairDatabaseRepository(Configuration["QuestionDbConnectionString"]));
            services.AddTransient<IDatabaseRepository<Question>>(p => new QuestionDatabaseRepository(Configuration["QuestionDbConnectionString"]));
            services.AddTransient<IDatabaseRepository<UserPairScore>>(p => new UserPairScoreDatabaseRepository(Configuration["QuestionDbConnectionString"]));

            services.AddTransient<ICacheProvider>(cache => new CouchbaseCacheProvider(Configuration["CouchbaseConnectionString"]));


           
            services.AddTransient<IQuestionManager, QuestionManager>();



            services.AddSingleton<IRabbitMQPersistentConnection>(sp =>
            {
                var logger = sp.GetRequiredService<ILogger<DefaultRabbitMQPersistentConnection>>();

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

                return new DefaultRabbitMQPersistentConnection(factory, logger, retryCount);
            });

            RegisterEventBus(services);


            var container = new ContainerBuilder();
            container.Populate(services);

            return new AutofacServiceProvider(container.Build());
        }



        private void RegisterEventBus(IServiceCollection services)
        {
            var subscriptionClientName = "Questions";


            services.AddSingleton<IEventBus, EventBusRabbitMQ>(sp =>
            {
                var rabbitMQPersistentConnection = sp.GetRequiredService<IRabbitMQPersistentConnection>();
                var iLifetimeScope = sp.GetRequiredService<ILifetimeScope>();
                var logger = sp.GetRequiredService<ILogger<EventBusRabbitMQ>>();
                var eventBusSubcriptionsManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();

                var retryCount = 5;
                if (!string.IsNullOrEmpty(Configuration["EventBusRetryCount"]))
                {
                    retryCount = int.Parse(Configuration["EventBusRetryCount"]);
                }

                return new EventBusRabbitMQ(rabbitMQPersistentConnection, logger, iLifetimeScope, eventBusSubcriptionsManager, subscriptionClientName, retryCount);
            });


            services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();

           services.AddTransient<UserRankChangedIntegrationEventHandler>();
            //services.AddTransient<OrderStartedIntegrationEventHandler>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();

            }
            app.UseGlobalExceptionHandling();
            app.UseHttpsRedirection();
            app.UseMvc();

            ConfigureEventBus(app);
            
        }

        private void ConfigureEventBus(IApplicationBuilder app)
        {
            var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();

            eventBus.Subscribe<UserRankChangedIntegrationEvent, UserRankChangedIntegrationEventHandler>();
            //eventBus.Subscribe<OrderStartedIntegrationEvent, OrderStartedIntegrationEventHandler>();
        }
    }
}
