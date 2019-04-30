using System;
using System.Collections;
using System.Collections.Generic;
using Questions.Models;

namespace Questions.Interfaces
{
    public enum CacheKeys : int
    {
        Questions = 1,
        Pairs = 2,
        Dictionaries = 3,
        UserScores = 4
    }

    public interface ICacheProvider
    {
        object Get(string key);
        T Get<T>(string key);
        void Set<T>(string key, T data);
        void Remove(string key);
        object Update<T>(string key, T data);

        bool IsExists(string key);

        IList<Pair> Pairs { get; set; }
        IList<Dictionary> Dictionaries { get; set; }
        IList<Question> Questions { get; set; }
        IList<UserPairScore> UserScores { get; set; }


    }
}
