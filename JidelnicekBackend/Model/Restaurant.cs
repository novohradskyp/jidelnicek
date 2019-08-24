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
    public class Restaurant : IRestaurant
    {
        /// <summary>
        /// Název restaurace.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Interní id restaurace.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Položky nabízeného menu.
        /// </summary>
        public IEnumerable<IMenuItem> Menu { get; set; }
    }
}
