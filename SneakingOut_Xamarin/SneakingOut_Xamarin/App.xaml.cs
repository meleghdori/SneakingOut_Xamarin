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
            _SneakingOutViewModel.Level1 += new EventHandler(SneakingOutViewModel_Level1);
            _SneakingOutViewModel.Level2 += new EventHandler(SneakingOutViewModel_Level2);
            _SneakingOutViewModel.Level3 += new EventHandler(SneakingOutViewModel_Level3);
            _SneakingOutViewModel.PauseGame += new EventHandler(SneakingOutViewModel_PauseGame);
            _SneakingOutViewModel.RestartGame += new EventHandler(SneakingOutViewModel_RestartGame);
            _SneakingOutViewModel.UpKeyDown += new EventHandler(ViewModel_UpKey);
            _SneakingOutViewModel.DownKeyDown += new EventHandler(ViewModel_DownKey);
            _SneakingOutViewModel.RightKeyDown += new EventHandler(ViewModel_RightKey);
            _SneakingOutViewModel.LeftKeyDown += new EventHandler(ViewModel_LeftKey);

            _gamePage = new GamePage();
            _gamePage.BindingContext = _SneakingOutViewModel;


            // nézet beállítása
            _mainPage = new NavigationPage(_gamePage); // egy navigációs lapot használunk fel a három nézet kezelésére

            MainPage = _mainPage;

            isPaused = false;
        }

        

        protected override void OnStart()
        {
            _advanceTimer = false; // egy logikai értékkel szabályozzuk az időzítőt
        }

        #region ViewModel event handlers

        private void ViewModel_UpKey(object sender, EventArgs e)
        {
            if (!isPaused)
            {
                _SneakingOutGameModel.PlayerMove(0);
            }
        }

        private void ViewModel_DownKey(object sender, EventArgs e)
        {
            if (!isPaused)
            {
                _SneakingOutGameModel.PlayerMove(1);
            }
        }

        private void ViewModel_RightKey(object sender, EventArgs e)
        {
            if (!isPaused)
            {
                _SneakingOutGameModel.PlayerMove(2);
            }
        }

        private void ViewModel_LeftKey(object sender, EventArgs e)
        {
            if (!isPaused)
            {
                _SneakingOutGameModel.PlayerMove(3);
            }
        }
        /// <summary>
        /// Új játék indításának eseménykezelője.
        /// </summary>
        private async void SneakingOutViewModel_Level1(object sender, EventArgs e)
        {
            _SneakingOutGameModel.NewGame();
            try
            {
                await _SneakingOutGameModel.LoadGameAsync("level1.txt");
            }
            catch { }
            _gameLevel = GameLevel.Level1;
            _SneakingOutViewModel.RefreshTable();

            if (!_advanceTimer || !isPaused)
            {
                // ha nem fut az időzítő, akkor elindítjuk
                _advanceTimer = true;
                isPaused = false;
                Device.StartTimer(TimeSpan.FromSeconds(0.5), () => { _SneakingOutGameModel.AdvanceTime(); return _advanceTimer; });
            }
        }

        /// <summary>
        /// Új játék indításának eseménykezelője.
        /// </summary>
        private async void SneakingOutViewModel_Level2(object sender, EventArgs e)
        {
            _SneakingOutGameModel.NewGame();
            try
            {
                await _SneakingOutGameModel.LoadGameAsync("level2.txt");
            }
            catch { }
            _SneakingOutViewModel.RefreshTable();
            _gameLevel = GameLevel.Level2;

            if (!_advanceTimer || !isPaused)
            {
                // ha nem fut az időzítő, akkor elindítjuk
                _advanceTimer = true;
                isPaused = false;
                Device.StartTimer(TimeSpan.FromSeconds(0.5), () => { _SneakingOutGameModel.AdvanceTime(); return _advanceTimer; });
            }
        }

        /// <summary>
        /// Új játék indításának eseménykezelője.
        /// </summary>
        private async void SneakingOutViewModel_Level3(object sender, EventArgs e)
        {
            _SneakingOutGameModel.NewGame();
            try
            {
                await _SneakingOutGameModel.LoadGameAsync("level3.txt");
            }
            catch { }
            _SneakingOutViewModel.RefreshTable();
            _gameLevel = GameLevel.Level3;
            if (!_advanceTimer || !isPaused)
            {
                // ha nem fut az időzítő, akkor elindítjuk
                _advanceTimer = true;
                isPaused = false;
                Device.StartTimer(TimeSpan.FromSeconds(0.5), () => { _SneakingOutGameModel.AdvanceTime(); return _advanceTimer; });
            }
        }

        private void SneakingOutViewModel_PauseGame(object sender, EventArgs e)
        {
            if (!isPaused)
            {
                _advanceTimer = false;
                isPaused = true;
                Device.StartTimer(TimeSpan.FromSeconds(0), () => { return _advanceTimer; });
            }
            else if (isPaused)
            {
                _advanceTimer = true;
                isPaused = false;
                Device.StartTimer(TimeSpan.FromSeconds(0.5), () => { _SneakingOutGameModel.AdvanceTime(); return _advanceTimer; });
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
