using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Jidelnicek.Backend.Provider;
using System.Threading.Tasks;

namespace Jidelnicek.Web.Controllers
{
    public class HomeController : Controller
    {
        public async Task<ActionResult> Index()
        {
            var ZomatoProvider = new CachingProvider(new ZomatoProvider());
            var WebProvider = new CachingProvider(new WebPageProvider());
            var ZomatoRestaurants = await ZomatoProvider.ProvideRestaurantsAsync();
            var WebPageRestaurants = await WebProvider.ProvideRestaurantsAsync();
            return View(ZomatoRestaurants.Union(WebPageRestaurants));
        }
    }
}