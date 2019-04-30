using System;
using System.Collections.Generic;
using Questions.Interfaces;
using Questions.Models;

namespace Questions.Providers.Database
{
    public class PairDatabaseRepository:BaseDatabaseRepository<Pair>
    {


        public PairDatabaseRepository(string connectionString):base(connectionString)
        { 
        }


        public override IList<Pair> GetAll()
        {
            return base.GetAll();

        }
        public override Pair GetById(int id)
        {
            return base.GetById(id);
        }
        public override IList<Pair> GetByFilter<TFilter>(TFilter filter)
        {
            return base.GetByFilter(filter);
        }
    }
}
