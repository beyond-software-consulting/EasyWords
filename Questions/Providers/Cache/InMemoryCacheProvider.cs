using System;
using Microsoft.Extensions.Caching.Memory;

namespace Questions.Providers.Cache
{
    public class InMemoryCacheProvider:IMemoryCache
    {
        public InMemoryCacheProvider()
        {

        }

        public ICacheEntry CreateEntry(object key)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void Remove(object key)
        {
            throw new NotImplementedException();
        }

        public bool TryGetValue(object key, out object value)
        {
            throw new NotImplementedException();
        }
    }
}
