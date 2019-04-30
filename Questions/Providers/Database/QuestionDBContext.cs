using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Questions.Models;
using Questions.Interfaces;
using MongoDB.Driver;
using MongoDB.Bson;
using Questions.Helpers;
using System.Reflection;

namespace Questions.Providers.Database
{
    public class QuestionDBContext : IDatabaseContext
    {
        MongoClient client;
        public QuestionDBContext()
        {
        }

        public QuestionDBContext(string connectionUrl)
        {

            MongoUrl mongoUrl = new MongoUrl(connectionUrl);
            client = new MongoClient(mongoUrl);
            Database = client.GetDatabase(mongoUrl.DatabaseName);

        }

        public IMongoDatabase Database { get; set; }

        public T AddOne<T>(T item)
        {
            string collectionName = GetCollectionName<T>();

            var prop = item.GetType().GetProperty("Id", BindingFlags.Public | BindingFlags.Instance);
            if (prop != null && prop.CanWrite)
                prop.SetValue(item, GetNextSequence<T>(), null);

            Database.GetCollection<T>(collectionName).InsertOne(item);

            return item;
        }

        public IList<T> AddRange<T>(IList<T> items)
        {
            string collectionName = GetCollectionName<T>();

            Database.GetCollection<T>(collectionName).InsertMany(items);

            return items;

        }

        public bool DeleteById(object id)
        {
            throw new System.NotImplementedException();
        }

        public bool DeleteOneByItem<T>(T item)
        {
            throw new System.NotImplementedException();
        }

        public IList<T> FindByFilter<T, ST>(ST filter)
        {
            throw new System.NotImplementedException();
        }

        public T FindOneByFilter<T, ST>(ST filter)
        {
            throw new System.NotImplementedException();
        }

        public T FindOneById<T>(object id)
        {


            var collectionName = GetCollectionName<T>();
            //Builders<T>.Filter.Eq()

            //return Database.GetCollection<T>(collectionName).Find()

            return default(T);
        }

        public IList<T> GetAll<T>()
        {
            var collectionName = GetCollectionName<T>();
            var collection = Database.GetCollection<T>(collectionName).Find(FilterDefinition<T>.Empty).ToList<T>();

            return collection;
        }

        public T GetLastInserted<T>()
        {
            string collectionName = GetCollectionName<T>();
            var collection = Database.GetCollection<T>(collectionName);
            T result = (T)collection.Find(FilterDefinition<T>.Empty).Sort("{Id: -1}").Limit(1);
            return result;

        }

        private string GetCollectionName<T>()
        {
            string collectionName = "";
            var attr = typeof(T).GetCustomAttributes(typeof(ModelAttribute), true).FirstOrDefault();
            if (attr != null)
            {
                collectionName = (attr as ModelAttribute).CollectionName;
            }
            return collectionName;
        }

        private int GetNextSequence<T>()
        {
            var collectionName = GetCollectionName<Sequence>();

            var collection = Database.GetCollection<Sequence>(collectionName);

            var filter = Builders<Sequence>.Filter.Eq(s => s.SequenceName, collectionName);
            var update = Builders<Sequence>.Update.Inc(s => s.SequenceValue, 1);

            var result = collection.FindOneAndUpdate(filter, update, new FindOneAndUpdateOptions<Sequence, Sequence> { IsUpsert = true, ReturnDocument = ReturnDocument.After });

            return result.SequenceValue;

           
        }

    }
}
