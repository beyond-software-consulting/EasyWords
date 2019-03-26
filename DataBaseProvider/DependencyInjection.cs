using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore.SqlServer;
using Microsoft.EntityFrameworkCore;

namespace DataBaseProvider
{
    public static class IServiceCollectionExtension
    {
        public static IServiceCollection AddQuestionDataBaseContext(this IServiceCollection services)
        {

            services.AddDbContext<QuestionDBContext>(o => o.UseSqlServer(""));
            return services;
        }

        public static IServiceCollection AddProfileDataBaseContext(this IServiceCollection services)
        {

            services.AddDbContext<ProfileDBContext>(o => o.UseSqlServer(""));
            return services;
        }
    }
}
