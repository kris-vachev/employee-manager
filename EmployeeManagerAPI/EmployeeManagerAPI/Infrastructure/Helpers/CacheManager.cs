using EmployeeManagerAPI.Infrastructure.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace EmployeeManagerAPI.Infrastructure.Helpers
{
    /// <summary>
    /// Provide management for caching data using an in-memory collection.
    /// Additionally keep track of the stored keys.
    /// </summary>
    public class CacheManager : ICacheManager
    {
        private readonly IMemoryCache _cache;
        private readonly List<object> _cacheKeys;

        public CacheManager(IMemoryCache cache)
        {
            _cache = cache;
            _cacheKeys = new List<object>();
        }

        /// <summary>
        /// Add an item to cache.
        /// </summary>
        /// <param name="key">Key to be used for the item.</param>
        /// <param name="item">Item to be added.</param>
        /// <param name="options">Cache options.</param>
        public void Set<TItem>(object key, TItem item, MemoryCacheEntryOptions options)
        {
            _cacheKeys.Add(key);
            _cache.Set(key, item, options);
        }

        /// <summary>
        /// Retrieve an item from cache.
        /// </summary>
        /// <param name="key">Key of the item to be rerieved.</param>
        /// <returns>Returns the cached data of the given model if found, otherwise null.</returns>
        public TItem? Get<TItem>(object key)
        {
            return _cache.Get<TItem>(key);
        }

        /// <summary>
        /// Remove an item from cache.
        /// </summary>
        /// <param name="key">Key of item to be removed.</param>
        public void Remove(object key)
        {
            _cache.Remove(key);
            _cacheKeys.Remove(key);
        }

        /// <summary>
        /// Clear all data stored in the cache.
        /// </summary>
        public void ClearAll()
        {
            foreach (var key in _cacheKeys)
            {
                _cache.Remove(key);
            }
            _cacheKeys.Clear();
        }
    }
}
