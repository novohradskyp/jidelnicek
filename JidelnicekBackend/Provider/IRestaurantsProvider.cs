using System.Collections.Generic;
using System.Threading.Tasks;
using Jidelnicek.Backend.Model;

namespace Jidelnicek.Backend.Provider
{
    public interface IRestaurantsProvider
    {
        Task<IEnumerable<IRestaurant>> GetAllRestaurantsAsync();
    }
}