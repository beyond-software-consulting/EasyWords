using System;
using System.Collections.Generic;
using System.Reflection;
using MongoDB.Driver;
using Questions.Helpers;
using Questions.Interfaces;
using System.Linq;
using Questions.Models;
using MongoDB.Bson;

namespace Questions.Providers.Database
{
    public class BaseDatabaseRepository<T>:IDatabaseRepository<T> where T: class
    {

        private IMongoClient client;
        private IMongoDatabase database;

        protected IMongoDatabase Database { get { return database; } }

        protected IMongoCollection<T> collection;

        public IMongoCollection<T> Collection { get { return collection; } }

        public BaseDatabaseRepository(string connectionString)
        {
            MongoUrl mongoUrl = new MongoUrl(connectionString);
            client = new MongoClient(mongoUrl);
            database = client.GetDatabase(mongoUrl.DatabaseName);

            collection = database.GetCollection<T>(GetCollectionName<T>());
        }

        public T Add(T item)
        {

            var prop = (from p in item.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                        where p.GetCustomAttribute(typeof(IdentityFieldAttribute)) != null
                        select p).FirstOrDefault();
            prop.SetValue(item, GetNextSequence());

            collection.InsertOne(item);
            return item;
        }

        public void Delete(T item)
        {

        }

        public virtual IList<T> GetAll()
        {
            var collectionName = GetCollectionName<T>();
            var collection = Database.GetCollection<T>(collectionName).Find(FilterDefinition<T>.Empty).ToList<T>();
            if (collection == null)
                collection = new List<T>();
            return collection;
        }

        public virtual IList<T> GetByFilter<TFilter>(TFilter filter)
        {
            throw new NotImplementedException();
        }

        public virtual T GetById(int id)
        {
            BsonDocument filter = new BsonDocument("Id", id);
            return (T)Collection.Find(filter);

        }

        public T Update(T item)
        {
            BsonDocument filter = new BsonDocument("Id", GetIDValue(item));
            Collection.ReplaceOne(filter, item);

            return item;
        }


        protected  string GetCollectionName<TT>()
        {
            string collectionName = "";
            var attr = typeof(TT).GetCustomAttributes(typeof(ModelAttribute), true).FirstOrDefault();
            if (attr != null)
            {
                collectionName = (attr as ModelAttribute).CollectionName;
            }
            return collectionName;
        }

        protected int GetIDValue(T item)
        {
            return (int)item.GetType().GetProperty("Id").GetValue(item);

        }

        protected int GetNextSequence()
        {
            var collectionName = GetCollectionName<Sequence>();

            var sequenceCollection = Database.GetCollection<Sequence>(collectionName);

            var filter = Builders<Sequence>.Filter.Eq(s => s.SequenceName, collectionName);
            var update = Builders<Sequence>.Update.Inc(s => s.SequenceValue, 1);

            var result = sequenceCollection.FindOneAndUpdate(filter, update, new FindOneAndUpdateOptions<Sequence, Sequence> { IsUpsert = true, ReturnDocument = ReturnDocument.After });

            return result.SequenceValue;


        }
    }
}
