using System;
using System.Collections;
using System.Collections.Generic;
using Couchbase;
using Couchbase.Authentication;
using Couchbase.Configuration.Client;
using Couchbase.Core;
using Questions.Interfaces;
using Questions.Models;

namespace Questions.Providers.Cache
{
    public class CouchbaseCacheProvider:ICacheProvider
    {
        Cluster cluster;
        IBucket bucket;
        public CouchbaseCacheProvider(string connectionString)
        {
            cluster = new Cluster(new ClientConfiguration() {

                Servers = new List<Uri>() { new Uri(connectionString) }
            });
            cluster.Authenticate("EasyWords", "Monk9562");
            bucket = cluster.OpenBucket("EasyWords");
        }

        public IList<Pair> Pairs {
            get {
                return Get<IList<Pair>>(CacheKeys.Pairs.ToString());
            }
            set {
                Set<IList<Pair>>(CacheKeys.Pairs.ToString(), value);
            }
        }

        public IList<Dictionary> Dictionaries { get {
                return Get<IList<Dictionary>>(CacheKeys.Dictionaries.ToString());
            } 
            set {
                Set<IList<Dictionary>>(CacheKeys.Dictionaries.ToString(), value);
            }
            }

        public IList<Question> Questions {
            get {
                return Get<IList<Question>>(CacheKeys.Dictionaries.ToString());
            }
            set
            {
                Set<IList<Question>>(CacheKeys.Questions.ToString(), value);
            }
        }

        public IList<UserPairScore> UserScores
        {
            get
            {
                return Get<IList<UserPairScore>>(CacheKeys.UserScores.ToString());
            }
            set
            {
                Set<IList<UserPairScore>>(CacheKeys.UserScores.ToString(), value); 
            }

        }

        public object Get(string key)
        {
            var retval = bucket.Get<dynamic>(key);
            return retval;
        }

        public TT Get<TT>(string key)
        {
            var retval = bucket.Get<TT>(key);

            return retval  == null ? default : retval.Value;
        }

        public bool IsExists(string key)
        {
            return bucket.Exists(key);
        }

        public void Remove(string key)
        {
            bucket.Remove(key);
        }

        public void Set<T>(string key, T data)
        {
            var doc = new Document<dynamic>();
            doc.Id = key;
            doc.Content = data;
            bucket.Upsert(doc);

           
        }

        public object Update<T>(string key, T data)
        {
            throw new NotImplementedException();
        }
    }
}
