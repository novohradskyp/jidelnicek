using Jidelnicek.Backend.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

namespace Jidelnicek.Backend.Provider
{
    public class CachingRestaurantsProvider : IRestaurantsProvider
    {
        private readonly IRestaurantsProvider restaurantProvider;
        private const string CacheItemNameBase = "Restaurants-";
        private const int MinutesToCache = 60;

        public CachingRestaurantsProvider(IRestaurantsProvider restaurantProvider)
        {
            this.restaurantProvider = restaurantProvider ?? throw new ArgumentNullException(nameof(restaurantProvider));
        }

        public async Task<IEnumerable<IRestaurant>> GetAllRestaurantsAsync()
        {
            IEnumerable<IRestaurant> Result;
            var Cache = MemoryCache.Default;
            var CacheItemName = CacheItemNameBase + restaurantProvider.GetType().FullName;
            Result = Cache[CacheItemName] as IEnumerable<IRestaurant>;
            if (Result == null)
            {
                Result = await restaurantProvider.GetAllRestaurantsAsync();
                Cache.Set(CacheItemName, Result, DateTimeOffset.Now.AddMinutes(MinutesToCache));
            }
            return Result;
        }
    }
}
