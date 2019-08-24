using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;
using Jidelnicek.Backend.Model;

namespace Jidelnicek.Backend.Provider
{
    public class CachingProvider : IMenuProvider
    {
        private readonly IMenuProvider MenuProvider;
        private const string CacheItemNameBase = "Menu-";
        private const int MinutesToCache = 60;

        public CachingProvider(IMenuProvider MenuProvider)
        {
            this.MenuProvider = MenuProvider ?? throw new ArgumentNullException(nameof(MenuProvider));
        }

        public async Task<IEnumerable<IRestaurant>> ProvideRestaurantsAsync()
        {
            IEnumerable<IRestaurant> Result;
            var Cache = MemoryCache.Default;
            var CacheItemName = CacheItemNameBase + MenuProvider.GetType().FullName;
            Result = Cache[CacheItemName] as IEnumerable<IRestaurant>;
            if(Result == null)
            {
                Result = await MenuProvider.ProvideRestaurantsAsync();
                Cache.Set(CacheItemName, Result, DateTimeOffset.Now.AddMinutes(MinutesToCache));
            }
            return Result;
        }
    }
}
