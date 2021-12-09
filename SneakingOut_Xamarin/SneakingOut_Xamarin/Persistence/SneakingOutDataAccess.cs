using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SneakingOut_Xamarin.Persistence
{
    public interface SneakingOutDataAccess
    {
        /// <summary>
        /// Fájl betöltése.
        /// </summary>
        /// <param name="path">Elérési útvonal.</param>
        /// <returns>A fájlból beolvasott játéktábla.</returns>
        Task<SneakingOutTable> LoadAsync(String path);

        /// <summary>
        /// Fájl mentése.
        /// </summary>
        /// <param name="path">Elérési útvonal.</param>
        /// <param name="table">A fájlba kiírandó játéktábla.</param>
        Task SaveAsync(String path, SneakingOutTable table);
    }
}
