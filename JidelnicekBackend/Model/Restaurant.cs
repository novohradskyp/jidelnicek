using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jidelnicek.Backend.Model
{
    /// <summary>
    /// Model restaurace.
    /// </summary>
    internal class Restaurant : IRestaurant
    {
        /// <summary>
        /// Název restaurace.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Položky nabízeného menu.
        /// </summary>
        public IEnumerable<IMenuItem> Menu { get; set; }
    }
}
