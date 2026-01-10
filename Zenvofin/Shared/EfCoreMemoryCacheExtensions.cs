using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Zenvofin.Shared;

public static class EfCoreMemoryCacheExtensions
{
    extension<T>(IQueryable<T> query)
        where T : class
    {
        public async Task<List<TDestination>> FromCacheListAsync<TDestination>(
            IMemoryCache cache,
            string cacheKey,
            TimeSpan expiration,
            CancellationToken cancellationToken = default)
            where TDestination : class
        {
            if (cache.TryGetValue(cacheKey, out List<TDestination>? cachedData))
                return cachedData!;

            List<TDestination> data = await query
                .AsNoTracking()
                .ProjectToType<TDestination>()
                .ToListAsync(cancellationToken);

            cache.Set(cacheKey, data, expiration);

            return data;
        }

        public async Task<TDestination?> FromCacheItemAsync<TDestination>(
            IMemoryCache cache,
            string cacheKey,
            TimeSpan expiration,
            CancellationToken cancellationToken = default)
            where TDestination : class
        {
            if (cache.TryGetValue(cacheKey, out List<TDestination>? cachedData))
                return cachedData?.FirstOrDefault();

            List<TDestination> data = await query
                .AsNoTracking()
                .ProjectToType<TDestination>()
                .ToListAsync(cancellationToken);

            cache.Set(cacheKey, data, expiration);

            return data.FirstOrDefault();
        }
    }
}