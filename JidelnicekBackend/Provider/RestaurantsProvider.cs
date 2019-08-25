using Jidelnicek.Backend.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jidelnicek.Backend.Provider
{
    public class RestaurantsProvider
    {
#pragma warning disable S1075 // URIs should not be hardcoded
        private readonly List<RestaurantDefinition> restaurants = new List<RestaurantDefinition>()
        {
            new RestaurantDefinition()
            {
                Name = "U Dřeváka",
                MenuProvider = new ZomatoProvider(16505458)
            },
            new RestaurantDefinition()
            {
                Name = "Divá Bára",
                MenuProvider = new ZomatoProvider(16514047)
            },
            new RestaurantDefinition()
            {
                Name = "U Bílého beránka",
                MenuProvider = new ZomatoProvider(16506737)
            },
            new RestaurantDefinition()
            {
                Name = "Leonardo",
                MenuProvider = new WebPageMenuProvider("http://www.penzion-luna.cz/?q=node/26559", "//div[@id='node-26559']/div[1]")
            },
            new RestaurantDefinition()
            {
                Name = "Padagali",
                MenuProvider = new WebPageMenuProvider("http://padagali.cz/denni-menu/", "//div[@class='row']//li[@class='screen-reader-text']|//div[@class='row']//li//h4")
            }
        };
#pragma warning restore S1075 // URIs should not be hardcoded

        public async Task<IEnumerable<IRestaurant>> GetAllRestaurantsAsync()
        {
            List<Restaurant> restaurantList = new List<Restaurant>(restaurants.Count);
            foreach(var definition in restaurants)
            {
                var restaurant = new Restaurant();
                restaurant.Name = definition.Name;
                restaurant.Menu = await definition.MenuProvider.ProvideMenuAsync();
                restaurantList.Add(restaurant);
            }
            return restaurantList;
        }
    }
}
