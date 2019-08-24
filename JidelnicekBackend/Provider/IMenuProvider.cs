using System.Collections.Generic;
using System.Threading.Tasks;
using Jidelnicek.Backend.Model;

namespace Jidelnicek.Backend.Provider
{
    public interface IMenuProvider
    {
        Task<IEnumerable<IRestaurant>> ProvideRestaurantsAsync();
    }
}