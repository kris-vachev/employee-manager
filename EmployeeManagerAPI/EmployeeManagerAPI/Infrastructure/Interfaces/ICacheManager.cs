using Microsoft.Extensions.Caching.Memory;

namespace EmployeeManagerAPI.Infrastructure.Interfaces
{
    public interface ICacheManager
    {
        void Set<TItem>(object key, TItem item, MemoryCacheEntryOptions options);
        TItem? Get<TItem>(object key);
        void Remove(object key);
        void ClearAll();
    }
}
