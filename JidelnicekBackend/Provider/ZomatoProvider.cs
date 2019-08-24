using Jidelnicek.Backend.Model;
using Jidelnicek.Backend.Model.Zomato;
using Jidelnicek.Backend.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jidelnicek.Backend.Provider
{
    public class ZomatoProvider : IMenuProvider
    {
        private static readonly List<Restaurant> Restaurants = new List<Restaurant>()
        {
            new Restaurant()
            {
                Name = "U Dřeváka",
                Id = 16505458
            },
            new Restaurant()
            {
                Name = "Divá Bára",
                Id = 16514047
            },
            new Restaurant()
            {
                Name = "U Bílého beránka",
                Id = 16506737
            },
            new Restaurant()
            {
                Name = "Himalaya",
                Id = 18020959
            },
            new Restaurant()
            {
                Name = "Golden Nepal",
                Id = 18346442
            },
            new Restaurant()
            {
                Name = "Restaurace Leonardo",
                Id = 16506573
            }
        };

        public async Task<IEnumerable<IRestaurant>> ProvideRestaurantsAsync()
        {
            var Result = new List<Restaurant>();
            var Reader = new ZomatoReaderService();
            foreach (var Rest in Restaurants)
            {
                var Menu = await Reader.ReadMenuAsync(Rest.Id);
                var Output = new Restaurant()
                {
                    Name = Rest.Name,
                    Id = Rest.Id,
                    Menu = Menu.SelectMany
                    (
                        ZomatoMenu => ZomatoMenu.Dishes.Select
                        (
                            Dish => new MenuItem()
                            {
                                Day = ZomatoMenu.StartDate,
                                Name = Dish.Name,
                                Price = Dish.Price
                            }
                        )
                    )
                };
                Result.Add(Output);
            }
            return Result;
        }
    }
}
