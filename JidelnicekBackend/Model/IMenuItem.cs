using System;

namespace Jidelnicek.Backend.Model
{
    /// <summary>
    /// Rozhraní popisující jednu položku menu.
    /// </summary>
    public interface IMenuItem
    {
        /// <summary>
        /// Název jídla na menu.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Cena menu.
        /// </summary>
        string Price { get; }
    }
}