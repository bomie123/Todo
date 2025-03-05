using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

namespace TodoApp.Helpers
{
    class CacheHelper
    {
        public string Origin { get; set; }
        public CacheHelper(string origin)
        {
            Origin = origin;
        }

        private MemoryCache _actualMemoryCache { get; set; }
        private  MemoryCache MemoryCache
        {
            get
            {
                _actualMemoryCache ??= new MemoryCache(Origin);
                return _actualMemoryCache;
            }
        }
        private static CacheItemPolicy _policy { get; set; } = new CacheItemPolicy()
        {
            SlidingExpiration = TimeSpan.FromMinutes(5),

        };

        public T GetOrCreate<T>(string key,Func<T> GenerationMethod)
        {
            key = Origin + key;
            if (!MemoryCache.Contains(key))
            {
                MemoryCache.Add(key, GenerationMethod.Invoke(), _policy);
            }

            return (T)MemoryCache.Get(key);
        }

    }
}
