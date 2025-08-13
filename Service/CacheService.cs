
using System.Collections.Concurrent;
using System.Text.Json;
using Lorecraft_API.Resources;
using Microsoft.Extensions.Caching.Distributed;
namespace Lorecraft_API.Service
{
    public interface ICacheService
    {
        Task<bool> CheckAsync<T>(string key, CancellationToken cancellationToken = default)
where T : class;
        Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
                where T : class;
        Task<T> GetAsync<T>(string key, Func<Task<T>> factory, CancellationToken cancellationToken = default)
where T : class;
        Task<T> GetAsync<T>(string key, Func<T> factory, CancellationToken cancellationToken = default)
                where T : class;
        Task<T> GetAsync<T>(string key, Func<long, Task<T>> factory, long id, CancellationToken cancellationToken = default)
        where T : class;
        Task<T> GetAsync<T>(string key, Func<long[], Task<T>> factory, long[] ids, CancellationToken cancellationToken = default)
        where T : class;
        Task<T> GetAsync<T>(string key, Func<long, bool, Task<T>> factory, long id, bool permission, CancellationToken cancellationToken = default)
        where T : class;
        Task<T?> GetNullableAsync<T>(string key, Func<long, Task<T?>> factory, long id, CancellationToken cancellationToken = default)
        where T : class;
        Task<T> GetAsync<T>(string key, Func<int, Task<T>> factory, int id, CancellationToken cancellationToken = default)
        where T : class;
        Task SetAsync<T>(string key, T value, CancellationToken cancellationToken = default)
        where T : class;
        Task RemoveAsync(string key, CancellationToken cancellationToken = default);
        Task RemoveAllAsync(CancellationToken cancellationToken = default);
        Task RemoveByPrefixAsync(string prefixKey, CancellationToken cancellationToken = default);

    }
    public class CacheService(IDistributedCache distributedCache) : ICacheService
    {
        private static readonly ConcurrentDictionary<string, bool> CacheKeys = new();
        private readonly IDistributedCache _distributedCache = distributedCache;

        public async Task<bool> CheckAsync<T>(string key, CancellationToken cancellationToken = default)
where T : class
        {
            string? cachedValue = await _distributedCache.GetStringAsync(
                key,
                cancellationToken
            );

            return cachedValue is not null;
        }
        public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
        where T : class
        {
            string? cachedValue = await _distributedCache.GetStringAsync(
                key,
                cancellationToken
            );

            if (cachedValue is null) return null;

            T? value = JsonSerializer.Deserialize<T>(cachedValue);

            return value;
        }

        public async Task<T> GetAsync<T>(string key, Func<Task<T>> factory, CancellationToken cancellationToken = default)
        where T : class
        {

            T? cachedValue = await GetAsync<T>(key, cancellationToken);

            if (cachedValue is not null) return cachedValue;

            cachedValue = await factory();

            await SetAsync(key, cachedValue, cancellationToken);

            return cachedValue;
        }
        public async Task<T> GetAsync<T>(string key, Func<T> factory, CancellationToken cancellationToken = default)
        where T : class
        {

            T? cachedValue = await GetAsync<T>(key, cancellationToken);

            if (cachedValue is not null) return cachedValue;

            cachedValue = factory();

            await SetAsync(key, cachedValue, cancellationToken);

            return cachedValue;
        }

        public async Task<T> GetAsync<T>(string key, Func<long, Task<T>> factory, long id, CancellationToken cancellationToken = default)
        where T : class
        {

            T? cachedValue = await GetAsync<T>(key, cancellationToken);

            if (cachedValue is not null) return cachedValue;

            cachedValue = await factory(id);

            await SetAsync(key, cachedValue, cancellationToken);

            return cachedValue;
        }
        public async Task<T> GetAsync<T>(string key, Func<long, bool, Task<T>> factory, long id, bool permission, CancellationToken cancellationToken = default)
        where T : class
        {

            T? cachedValue = await GetAsync<T>(key, cancellationToken);

            if (cachedValue is not null) return cachedValue;

            cachedValue = await factory(id, permission);

            await SetAsync(key, cachedValue, cancellationToken);

            return cachedValue;
        }
        public async Task<T> GetAsync<T>(string key, Func<long[], Task<T>> factory, long[] ids, CancellationToken cancellationToken = default)
        where T : class
        {

            T? cachedValue = await GetAsync<T>(key, cancellationToken);

            if (cachedValue is not null) return cachedValue;

            cachedValue = await factory(ids);

            await SetAsync(key, cachedValue, cancellationToken);

            return cachedValue;
        }
        public async Task<T?> GetNullableAsync<T>(string key, Func<long, Task<T?>> factory, long id, CancellationToken cancellationToken = default)
        where T : class
        {

            T? cachedValue = await GetAsync<T>(key, cancellationToken);

            if (cachedValue is not null) return cachedValue;

            cachedValue = await factory(id);

            if (cachedValue is null)
                return null;

            await SetAsync(key, cachedValue, cancellationToken);

            return cachedValue;
        }
        public async Task<T> GetAsync<T>(string key, Func<int, Task<T>> factory, int id, CancellationToken cancellationToken = default)
        where T : class
        {

            T? cachedValue = await GetAsync<T>(key, cancellationToken);

            if (cachedValue is not null) return cachedValue;

            cachedValue = await factory(id);

            await SetAsync(key, cachedValue, cancellationToken);

            return cachedValue;
        }

        public async Task SetAsync<T>(string key, T value, CancellationToken cancellationToken = default)
        where T : class
        {
            string cacheValue = JsonSerializer.Serialize(value);

            await _distributedCache.SetStringAsync(key, cacheValue, cancellationToken);

            CacheKeys.TryAdd(key, false);
        }

        public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
        {
            await _distributedCache.RemoveAsync(key, cancellationToken);

            CacheKeys.TryRemove(key, out bool _);
        }
        public async Task RemoveAllAsync(CancellationToken cancellationToken = default)
        {
            IEnumerable<Task> tasks = CacheKeys.Keys
            .Select(key => RemoveAsync(key, cancellationToken));

            await Task.WhenAll(tasks);
        }
        public async Task RemoveByPrefixAsync(string prefixKey, CancellationToken cancellationToken = default)
        {
            IEnumerable<Task> tasks = CacheKeys.Keys
            .Where(key => key
            .Split(':')[0].Equals(prefixKey))
            .Select(key => RemoveAsync(key, cancellationToken));

            await Task.WhenAll(tasks);
        }
    }

}