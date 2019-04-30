using System;
using Questions.Models;

namespace Questions.Providers.Database
{
    public class QuestionDatabaseRepository:BaseDatabaseRepository<Question>
    {
        public QuestionDatabaseRepository(string connectionString):base(connectionString)
        {
            
        }
    }
}
