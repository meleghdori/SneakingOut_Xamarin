using System;
using System.IO;
using System.Threading.Tasks;
using Android.Content.Res;
using SneakingOut_Xamarin.Droid.Persistence;
using SneakingOut_Xamarin.Persistence;
using Xamarin.Forms;

[assembly: Dependency(typeof(AndroidDataAccess))]
namespace SneakingOut_Xamarin.Droid.Persistence
{
    /// <summary>
    /// Tic-Tac-Toe adatelérés megvalósítása Android platformra.
    /// </summary>
    public class AndroidDataAccess : SneakingOutDataAccess
    {
        /// <summary>
        /// Fájl betöltése.
        /// </summary>
        /// <param name="path">Elérési útvonal.</param>
        /// <returns>A beolvasott mezõértékek.</returns>
        public async Task<SneakingOutTable> LoadAsync(String path)
        {
            // a betöltés a személyen könyvtárból történik
            String filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), path);

            AssetManager assets = Android.App.Application.Context.Assets;

            String value;

            // a fájlmûveletet taszk segítségével végezzük (aszinkron módon)
            String[] values;
            using (StreamReader sr = new StreamReader(assets.Open(path)))
            {
                value = sr.ReadToEnd();

            }

            values = value.Split(' ');

            Int32 tableSize = Int32.Parse(values[0]);
            SneakingOutTable table = new SneakingOutTable(tableSize); // létrehozzuk a táblát

            Int32 valueIndex = 1;
            for (Int32 rowIndex = 0; rowIndex < tableSize; rowIndex++)
            {              
                for (Int32 columnIndex = 0; columnIndex < tableSize; columnIndex++)
                {
                    table.SetValue(rowIndex, columnIndex, Int32.Parse(values[valueIndex])); // értékek betöltése
                    valueIndex++;
                }
            }

            return table;
        }

        /// <summary>
        /// Fájl mentése.
        /// </summary>
        /// <param name="path">Elérési útvonal.</param>
        /// <param name="table">A fájlba kiírandó játéktábla.</param>
        public async Task SaveAsync(String path, SneakingOutTable table)
        {
            String text = table.Size.ToString(); // méret

            for (Int32 i = 0; i < table.Size; i++)
            {
                for (Int32 j = 0; j < table.Size; j++)
                {
                    text += table[i, j] + " "; // mezõértékek
                }
            }

            // fájl létrehozása
            String filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), path);

            // kiírás (aszinkron módon)
            await Task.Run(() => File.WriteAllText(filePath, text));
        }
    }
}