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
            IRestaurantsProvider provider = new CachingRestaurantsProvider(new RestaurantsProvider());
            var restaurants = await provider.GetAllRestaurantsAsync();
            return View(restaurants);
        }
    }
}