using System;
using Questions.Models;

namespace Questions.Providers.Database
{
    public class UserPairScoreDatabaseRepository:BaseDatabaseRepository<UserPairScore>
    {
        public UserPairScoreDatabaseRepository(string connectionString):base(connectionString)
        {
        }
    }
}
