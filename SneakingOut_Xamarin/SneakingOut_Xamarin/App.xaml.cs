using SneakingOut_Xamarin.View;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SneakingOut_Xamarin.Model;
using SneakingOut_Xamarin.Persistence;
using SneakingOut_Xamarin.ViewModel;
using System.Threading.Tasks;


namespace SneakingOut_Xamarin
{
    public enum GameLevel { Level1, Level2, Level3 }

    public partial class App : Application
    {
        #region Fields

        private SneakingOutDataAccess _SneakingOutDataAccess;
        private SneakingOutGameModel _SneakingOutGameModel;
        private SneakingOutViewModel _SneakingOutViewModel;
        private GamePage _gamePage;
        private SettingsPage _settingsPage;

        private IStore _store;
        private StoredGameBrowserModel _storedGameBrowserModel;
        private StoredGameBrowserViewModel _storedGameBrowserViewModel;
        private LoadGamePage _loadGamePage;
        private SaveGamePage _saveGamePage;

        private Boolean _advanceTimer;
        private NavigationPage _mainPage;
        private Boolean isPaused;
        private GameLevel _gameLevel;

        #endregion

        public App()
        {
            _SneakingOutDataAccess = DependencyService.Get<SneakingOutDataAccess>(); // az interfész megvalósítását automatikusan megkeresi a rendszer

            _SneakingOutGameModel = new SneakingOutGameModel(_SneakingOutDataAccess);
            _SneakingOutGameModel.GameOver += new EventHandler<SneakingOutEventArgs>(SneakingOutGameModel_GameOver);

            _SneakingOutViewModel = new SneakingOutViewModel(_SneakingOutGameModel);
            _SneakingOutViewModel.ExitGame += new EventHandler(SneakingOutViewModel_ExitGame);
            _SneakingOutViewModel.Level1 += new EventHandler(SneakingOutViewModel_Level1);
            _SneakingOutViewModel.Level2 += new EventHandler(SneakingOutViewModel_Level2);
            _SneakingOutViewModel.Level3 += new EventHandler(SneakingOutViewModel_Level3);
            _SneakingOutViewModel.SaveGame += new EventHandler(SneakingOutViewModel_SaveGame);
            _SneakingOutViewModel.PauseGame += new EventHandler(SneakingOutViewModel_PauseGame);
            _SneakingOutViewModel.RestartGame += new EventHandler(SneakingOutViewModel_RestartGame);

            _gamePage = new GamePage();
            _gamePage.BindingContext = _SneakingOutViewModel;

            _settingsPage = new SettingsPage();
            _settingsPage.BindingContext = _SneakingOutViewModel;

            // a játékmentések kezelésének összeállítása
            _store = DependencyService.Get<IStore>(); // a perzisztencia betöltése az adott platformon
            _storedGameBrowserModel = new StoredGameBrowserModel(_store);
            _storedGameBrowserViewModel = new StoredGameBrowserViewModel(_storedGameBrowserModel);
            _storedGameBrowserViewModel.GameLoading += new EventHandler<StoredGameEventArgs>(StoredGameBrowserViewModel_GameLoading);
            _storedGameBrowserViewModel.GameSaving += new EventHandler<StoredGameEventArgs>(StoredGameBrowserViewModel_GameSaving);

            _loadGamePage = new LoadGamePage();
            _loadGamePage.BindingContext = _storedGameBrowserViewModel;

            _saveGamePage = new SaveGamePage();
            _saveGamePage.BindingContext = _storedGameBrowserViewModel;

            // nézet beállítása
            _mainPage = new NavigationPage(_gamePage); // egy navigációs lapot használunk fel a három nézet kezelésére

            MainPage = _mainPage;

            isPaused = true;
        }

        

        protected override void OnStart()
        {
            _advanceTimer = false; // egy logikai értékkel szabályozzuk az időzítőt
         //   Device.StartTimer(TimeSpan.FromSeconds(1), () => { _SneakingOutGameModel.AdvanceTime(); return _advanceTimer; }); // elindítjuk az időzítőt
        }

        protected override void OnSleep()
        {
            _advanceTimer = false;

            // elmentjük a jelenleg folyó játékot
            try
            {
                Task.Run(async () => await _SneakingOutGameModel.SaveGameAsync("SuspendedGame"));
            }
            catch { }
        }

        protected override void OnResume()
        {
            // betöltjük a felfüggesztett játékot, amennyiben van
            try
            {
                Task.Run(async () =>
                {
                    await _SneakingOutGameModel.LoadGameAsync("SuspendedGame");
                    _SneakingOutViewModel.RefreshTable();

                    // csak akkor indul az időzítő, ha sikerült betölteni a játékot
                    _advanceTimer = true;
                    Device.StartTimer(TimeSpan.FromSeconds(1), () => { _SneakingOutGameModel.AdvanceTime(); return _advanceTimer; });
                });
            }
            catch { }
        }

        #region ViewModel event handlers

        /// <summary>
        /// Új játék indításának eseménykezelője.
        /// </summary>
        private async void SneakingOutViewModel_Level1(object sender, EventArgs e)
        {
            _SneakingOutGameModel.NewGame();
            try
            {
                await _SneakingOutGameModel.LoadGameAsync(@"C:\Digitalis oktatas\2021-2\eva\SneakingOut_Xamarin\SneakingOut_Xamarin\SneakingOut_Xamarin\level1.txt");
            }
            catch { }
            _gameLevel = GameLevel.Level1;
            _SneakingOutViewModel.RefreshTable();

            if (!_advanceTimer)
            {
                // ha nem fut az időzítő, akkor elindítjuk
                _advanceTimer = true;
                Device.StartTimer(TimeSpan.FromSeconds(1), () => { _SneakingOutGameModel.AdvanceTime(); return _advanceTimer; });
            }
        }

        /// <summary>
        /// Új játék indításának eseménykezelője.
        /// </summary>
        private void SneakingOutViewModel_Level2(object sender, EventArgs e)
        {
            _SneakingOutGameModel.NewGame();
            _SneakingOutViewModel.RefreshTable();
            _gameLevel = GameLevel.Level2;

            if (!_advanceTimer)
            {
                // ha nem fut az időzítő, akkor elindítjuk
                _advanceTimer = true;
                Device.StartTimer(TimeSpan.FromSeconds(1), () => { _SneakingOutGameModel.AdvanceTime(); return _advanceTimer; });
            }
        }

        /// <summary>
        /// Új játék indításának eseménykezelője.
        /// </summary>
        private void SneakingOutViewModel_Level3(object sender, EventArgs e)
        {
            _SneakingOutGameModel.NewGame();
            _SneakingOutViewModel.RefreshTable();
            _gameLevel = GameLevel.Level3;
            if (!_advanceTimer)
            {
                // ha nem fut az időzítő, akkor elindítjuk
                _advanceTimer = true;
                Device.StartTimer(TimeSpan.FromSeconds(1), () => { _SneakingOutGameModel.AdvanceTime(); return _advanceTimer; });
            }
        }

        /// <summary>
        /// Játék betöltésének eseménykezelője.
        /// </summary>
        private async void SneakingOutViewModel_LoadGame(object sender, System.EventArgs e)
        {
            await _storedGameBrowserModel.UpdateAsync(); // frissítjük a tárolt játékok listáját
            await _mainPage.PushAsync(_loadGamePage); // átnavigálunk a lapra
        }

        /// <summary>
        /// Játék mentésének eseménykezelője.
        /// </summary>
        private async void SneakingOutViewModel_SaveGame(object sender, EventArgs e)
        {
            await _storedGameBrowserModel.UpdateAsync(); // frissítjük a tárolt játékok listáját
            await _mainPage.PushAsync(_saveGamePage); // átnavigálunk a lapra
        }

        private async void SneakingOutViewModel_ExitGame(object sender, EventArgs e)
        {
            await _mainPage.PushAsync(_settingsPage); // átnavigálunk a beállítások lapra
        }

        private void SneakingOutViewModel_PauseGame(object sender, EventArgs e)
        {
            if (!isPaused)
            {
                isPaused = true;
                
            }
            else if (isPaused)
            {
                isPaused = false;
            }
        }

        private void SneakingOutViewModel_RestartGame(object sender, EventArgs e)
        {
            if (_gameLevel == GameLevel.Level1)
            {
                SneakingOutViewModel_Level1(sender, e);
            }
            if (_gameLevel == GameLevel.Level2)
            {
                SneakingOutViewModel_Level2(sender, e);
            }
            if (_gameLevel == GameLevel.Level3)
            {
                SneakingOutViewModel_Level3(sender, e);
            }
        }

        /// <summary>
        /// Betöltés végrehajtásának eseménykezelője.
        /// </summary>
        private async void StoredGameBrowserViewModel_GameLoading(object sender, StoredGameEventArgs e)
        {
            await _mainPage.PopAsync(); // visszanavigálunk

            // betöltjük az elmentett játékot, amennyiben van
            try
            {
                await _SneakingOutGameModel.LoadGameAsync(e.Name);
                _SneakingOutViewModel.RefreshTable();

                // csak akkor indul az időzítő, ha sikerült betölteni a játékot
                _advanceTimer = true;
                Device.StartTimer(TimeSpan.FromSeconds(1), () => { _SneakingOutGameModel.AdvanceTime(); return _advanceTimer; });
            }
            catch
            {
                await MainPage.DisplayAlert("Sneaking Out the game", "Failed to load!", "OK");
            }
        }

        /// <summary>
        /// Mentés végrehajtásának eseménykezelője.
        /// </summary>
        private async void StoredGameBrowserViewModel_GameSaving(object sender, StoredGameEventArgs e)
        {
            await _mainPage.PopAsync(); // visszanavigálunk
            _advanceTimer = false;

            try
            {
                // elmentjük a játékot
                await _SneakingOutGameModel.SaveGameAsync(e.Name);
            }
            catch { }

            await MainPage.DisplayAlert("Sneaking Out the game", "Game successfully saved!", "OK");
        }

        #endregion

        #region Model event handlers

        /// <summary>
        /// Játék végének eseménykezelője.
        /// </summary>
        private async void SneakingOutGameModel_GameOver(object sender, SneakingOutEventArgs e)
        {
            _advanceTimer = false;

            if (e.IsWon) // győzelemtől függő üzenet megjelenítése
            {
                await MainPage.DisplayAlert("Sneaking Out the Game", "Congratulations! You won!" + Environment.NewLine +
                                            "You did " + e.GameStepCount + " steps and your time was " +
                                            TimeSpan.FromSeconds(e.GameTime).ToString("g") ,
                                            "OK");
            }
            else
            {
                await MainPage.DisplayAlert("Sneaking Out the Game", "You did your best! Maybe next time. ", "OK");
            }
        }

        #endregion
    }
}
