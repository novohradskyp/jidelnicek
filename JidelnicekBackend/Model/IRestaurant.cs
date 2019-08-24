using System.Collections.Generic;

namespace Jidelnicek.Backend.Model
{
    /// <summary>
    /// Rozhraní popisující restauraci.
    /// </summary>
    public interface IRestaurant
    {
        /// <summary>
        /// Název restaurace.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Interní id restaurace.
        /// </summary>
        int Id { get; }

        /// <summary>
        /// Položky nabízeného menu.
        /// </summary>
        IEnumerable<IMenuItem> Menu { get; }
    }
}