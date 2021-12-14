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
    /// Tic-Tac-Toe adatel�r�s megval�s�t�sa Android platformra.
    /// </summary>
    public class AndroidDataAccess : SneakingOutDataAccess
    {
        /// <summary>
        /// F�jl bet�lt�se.
        /// </summary>
        /// <param name="path">El�r�si �tvonal.</param>
        /// <returns>A beolvasott mez��rt�kek.</returns>
        public async Task<SneakingOutTable> LoadAsync(String path)
        {
            // a bet�lt�s a szem�lyen k�nyvt�rb�l t�rt�nik
            String filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), path);

            AssetManager assets = Android.App.Application.Context.Assets;

            String value;

            // a f�jlm�veletet taszk seg�ts�g�vel v�gezz�k (aszinkron m�don)
            String[] values;
            using (StreamReader sr = new StreamReader(assets.Open(path)))
            {
                value = sr.ReadToEnd();

            }

            values = value.Split(' ');

            Int32 tableSize = Int32.Parse(values[0]);
            SneakingOutTable table = new SneakingOutTable(tableSize); // l�trehozzuk a t�bl�t

            Int32 valueIndex = 1;
            for (Int32 rowIndex = 0; rowIndex < tableSize; rowIndex++)
            {              
                for (Int32 columnIndex = 0; columnIndex < tableSize; columnIndex++)
                {
                    table.SetValue(rowIndex, columnIndex, Int32.Parse(values[valueIndex])); // �rt�kek bet�lt�se
                    valueIndex++;
                }
            }

            return table;
        }

        /// <summary>
        /// F�jl ment�se.
        /// </summary>
        /// <param name="path">El�r�si �tvonal.</param>
        /// <param name="table">A f�jlba ki�rand� j�t�kt�bla.</param>
        public async Task SaveAsync(String path, SneakingOutTable table)
        {
            String text = table.Size.ToString(); // m�ret

            for (Int32 i = 0; i < table.Size; i++)
            {
                for (Int32 j = 0; j < table.Size; j++)
                {
                    text += table[i, j] + " "; // mez��rt�kek
                }
            }

            // f�jl l�trehoz�sa
            String filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), path);

            // ki�r�s (aszinkron m�don)
            await Task.Run(() => File.WriteAllText(filePath, text));
        }
    }
}