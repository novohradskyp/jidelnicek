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
    internal class ZomatoProvider : IMenuProvider
    {
        private readonly int restaurantId;

        public ZomatoProvider(int restaurantId)
        {
            this.restaurantId = restaurantId;
        }

        public async Task<IEnumerable<IMenuItem>> ProvideMenuAsync()
        {
            var Reader = new ZomatoReaderService();
            var Menu = await Reader.ReadMenuAsync(restaurantId);
            return Menu.SelectMany
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
                );
        }
    }
}
