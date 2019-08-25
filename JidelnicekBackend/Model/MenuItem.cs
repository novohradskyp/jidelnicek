using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jidelnicek.Backend.Model
{
    /// <summary>
    /// Model položky menu. Označuje jedno jídlo.
    /// </summary>
    internal class MenuItem : IMenuItem
    {
        /// <summary>
        /// Den, pro který je položka menu platná.
        /// </summary>
        public DateTime Day { get; set; }

        /// <summary>
        /// Název jídla na menu.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Cena menu.
        /// </summary>
        public string Price { get; set; }
    }
}
