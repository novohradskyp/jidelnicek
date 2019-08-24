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
            var Provider = new CachingProvider(new ZomatoProvider());
            var Menu = await Provider.ProvideRestaurantsAsync();
            return View(Menu);
        }
    }
}