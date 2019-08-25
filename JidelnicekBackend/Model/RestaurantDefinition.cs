using Jidelnicek.Backend.Provider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jidelnicek.Backend.Model
{
    internal class RestaurantDefinition
    {
        public string Name { get; set; }
        public IMenuProvider MenuProvider { get; set; }
    }
}
