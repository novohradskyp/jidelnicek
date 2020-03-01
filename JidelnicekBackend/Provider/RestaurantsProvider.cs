using Jidelnicek.Backend.Model;
using Jidelnicek.Backend.Util;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jidelnicek.Backend.Provider
{
    public class RestaurantsProvider : IRestaurantsProvider
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
                MenuProvider = new ZomatoProvider(18774679)
            },
            new RestaurantDefinition()
            {
                Name = "Na růžku",
                MenuProvider = new WebPageMenuProvider("https://www.naruzkubrno.cz/tydenni-menu/", "//div[@class='section-inner']//img", new RuzekUrlTranslator())
            },
            new RestaurantDefinition()
            {
                Name = "Al Capone",
                MenuProvider = new WebPageMenuProvider("https://www.pizzaalcapone.cz/cz/poledni-menu", "//div[@class='container']//table")
            }
        };
#pragma warning restore S1075 // URIs should not be hardcoded

        public async Task<IEnumerable<IRestaurant>> GetAllRestaurantsAsync()
        {//Metoda umožní překrytí tasků pro získání menu.
            return await Task.WhenAll(restaurants.Select(MakeRestaurantAsync));
        }

        private async Task<IRestaurant> MakeRestaurantAsync(RestaurantDefinition definition)
        {
            using (var operation = TelemetrySetting.TelemetryClientInstance.StartOperation<DependencyTelemetry>("Load - " + definition.Name))
            {
                var restaurant = new Restaurant()
                {
                    Name = definition.Name
                };
                try
                {
                    restaurant.Menu = await definition.MenuProvider.ProvideMenuAsync();
                }
                catch (Exception e)
                {//Chyby při stahování jídelníčku ignorovat. V případě chyby dát prázdné menu.
                    TelemetrySetting.TelemetryClientInstance.TrackException(e, new Dictionary<string, string>() { { "Restaurant", definition.Name } });
                    operation.Telemetry.Success = false;
                    restaurant.Menu = new List<IMenuItem>();
                }
                return restaurant;
            }
        }
    }
}
