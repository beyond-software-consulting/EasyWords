using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Questions.Interfaces
{
    public interface IDatabaseContext
    { 
        IMongoDatabase Database { get;  }

        #region Add Functions

        T AddOne<T>(T item);
        IList<T> AddRange<T>(IList<T> Items);

        #endregion

        #region Delete Functions

        bool DeleteOneByItem<T>(T item);
        bool DeleteById(object ID);
        //bool DumpAll();

        #endregion

        #region Find Functions

        T FindOneById<T>(object id);
        T FindOneByFilter<T,ST>(ST filter);
        IList<T> FindByFilter<T,ST>(ST filter);

        #endregion

        #region Get Functions
        IList<T> GetAll<T>();
        T GetLastInserted<T>();
        #endregion


    }
}
