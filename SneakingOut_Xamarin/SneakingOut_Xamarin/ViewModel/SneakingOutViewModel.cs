using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using SneakingOut_Xamarin.Model;
using SneakingOut_Xamarin.Persistence;

namespace SneakingOut_Xamarin.ViewModel
{
    public class SneakingOutViewModel : ViewModelBase
    {
        #region Fields

        private SneakingOutGameModel _model; // modell

        #endregion

        #region Properties

        /// <summary>
        /// Új játék kezdése parancs lekérdezése.
        /// </summary>
        public DelegateCommand NewGameCommand { get; private set; }

        /// <summary>
        /// Játék mentése parancs lekérdezése.
        /// </summary>
        public DelegateCommand SaveGameCommand { get; private set; }

        /// <summary>
        /// Kilépés parancs lekérdezése.
        /// </summary>
        public DelegateCommand ExitCommand { get; private set; }

        /// <summary>
        /// Kilépés parancs lekérdezése.
        /// </summary>
        public DelegateCommand RestartCommand { get; private set; }

        /// <summary>
        /// Kilépés parancs lekérdezése.
        /// </summary>
        public DelegateCommand PauseCommand { get; private set; }

        /// <summary>
        /// Kilépés parancs lekérdezése.
        /// </summary>
        public DelegateCommand Level1Command { get; private set; }

        public DelegateCommand Level2Command { get; private set; }

        public DelegateCommand Level3Command { get; private set; }

        public DelegateCommand LeftKeyDownCommand { get;private set; }

        public DelegateCommand RightKeyDownCommand { get;private set; }

        public DelegateCommand DownKeyDownCommand { get;private set; }

        public DelegateCommand UpKeyDownCommand { get;private set; }


        /// <summary>
        /// Játékmező gyűjtemény lekérdezése.
        /// </summary>
        public ObservableCollection<SneakingOutField> Fields { get; set; }

        /// <summary>
        /// Lépések számának lekérdezése.
        /// </summary>
        public Int32 GameStepCount { get { return _model.GameStepCount; } }

        /// <summary>
        /// Fennmaradt játékidő lekérdezése.
        /// </summary>
        public String GameTime { get { return TimeSpan.FromSeconds(_model.GameTime).ToString("g"); } }

        #endregion

        #region Events

        /// <summary>
        /// Játék mentésének eseménye.
        /// </summary>
        public event EventHandler SaveGame;

        /// <summary>
        /// Játékból való kilépés eseménye.
        /// </summary>
        public event EventHandler ExitGame;

        /// <summary>
        /// Level1
        /// </summary>
        public event EventHandler Level1;
        /// <summary>
        /// Level2
        /// </summary>
        public event EventHandler Level2;
        /// <summary>
        /// Level3
        /// </summary>
        public event EventHandler Level3;

        /// <summary>
        /// Játékból megállítása
        /// </summary>
        public event EventHandler PauseGame;

        /// <summary>
        /// Játékból ujrainditasa
        /// </summary>
        public event EventHandler RestartGame;

        /// <summary>
        /// jatekos iranyatasanak esemenyei
        /// </summary>
        public event EventHandler UpKeyDown;
        public event EventHandler DownKeyDown;
        public event EventHandler RightKeyDown;
        public event EventHandler LeftKeyDown;



        #endregion

        #region Constructors

        /// <summary>
        /// Sudoku nézetmodell példányosítása.
        /// </summary>
        /// <param name="model">A modell típusa.</param>
        public SneakingOutViewModel(SneakingOutGameModel model)
        {
            // játék csatlakoztatása
            _model = model;
            _model.GameAdvanced += new EventHandler<SneakingOutEventArgs>(Model_GameAdvanced);
            _model.GameOver += new EventHandler<SneakingOutEventArgs>(Model_GameOver);
            _model.GameCreated += new EventHandler<SneakingOutEventArgs>(Model_GameCreated);

            // parancsok kezelése
            RestartCommand = new DelegateCommand(param => OnRestartGame());
            PauseCommand = new DelegateCommand(param => OnPauseGame());
            Level1Command = new DelegateCommand(param => OnLevel1());
            Level2Command = new DelegateCommand(param => OnLevel2());
            Level3Command = new DelegateCommand(param => OnLevel3());
            SaveGameCommand = new DelegateCommand(param => OnSaveGame());
            ExitCommand = new DelegateCommand(param => OnExitGame());
            UpKeyDownCommand = new DelegateCommand(param => OnUpKeyDown());
            DownKeyDownCommand = new DelegateCommand(param => OnDownKeyDown());
            LeftKeyDownCommand = new DelegateCommand(param => OnLeftKeyDown());
            RightKeyDownCommand = new DelegateCommand(param => OnRightKeyDown());



            // játéktábla létrehozása
            Fields = new ObservableCollection<SneakingOutField>();
            for (Int32 i = 0; i < _model.Table.Size; i++) // inicializáljuk a mezőket
            {
                for (Int32 j = 0; j < _model.Table.Size; j++)
                {
                    Fields.Add(new SneakingOutField
                    {
                        IsEmpty = false,
                        IsExit = false,
                        IsPlayer = false,
                        IsSecurity = false,
                        IsWall = false,
                        X = i,
                        Y = j,
                    }) ;
                   
                    OnPropertyChanged();
                }
            }

            RefreshTable();
            
        }

        #endregion

        #region Private methods
        
        // itt probalgatom h megy e az esemeny kivaltasa
        public void PlayerChangedEventHandler(object sender, Player player)
        {
            Fields[player.getPositionX() + player.getPositionY()].X = player.getPositionX();
            OnPropertyChanged();
        }

        public void SyncTable(ObservableCollection<SneakingOutField> Fields )
        {
            for(int i = 0; i < _model.Table.Size; i++)
            {
                for(int j = 0; j < _model.Table.Size; j++)
                {

                    Fields[i * _model.Table.Size + j].X = i;
                    Fields[i * _model.Table.Size + j].Y = j;
    
                    if (_model.Table.GetValue(i, j) == 0)
                    {
                       
                        Fields[i * _model.Table.Size + j].IsEmpty = true;
                    }
                    else if (_model.Table.GetValue(i, j) == 1)
                    {
                        
                        Fields[i * _model.Table.Size + j].IsSecurity = true;
                    }
                    else if (_model.Table.GetValue(i, j) == 2)
                    {
                       
                        Fields[i * _model.Table.Size + j].IsSecurity = true;
                    }
                    else if (_model.Table.GetValue(i, j) == 3)
                    {
                       
                        Fields[i * _model.Table.Size + j].IsPlayer = true;
                    }
                    else if (_model.Table.GetValue(i, j) == 4)
                    {
                        
                        Fields[i * _model.Table.Size + j].IsWall = true;
                    }
                    else if (_model.Table.GetValue(i, j) == 5)
                    {
                        
                        Fields[i * _model.Table.Size + j].IsExit = true;
                    }

                    OnPropertyChanged("Fields");
                }
            }
        }

        /// <summary>
        /// Tábla frissítése.
        /// </summary>
        public void RefreshTable()
        {
            foreach (SneakingOutField field in Fields) // inicializálni kell a mezőket is
            {

                field.IsExit = false;
                field.IsPlayer = false;
                field.IsSecurity = false;
                field.IsWall = false;
                field.IsEmpty = false;
                SyncTable(Fields);
            }

            OnPropertyChanged("GameTime");
            OnPropertyChanged("GameStepCount");
        }

        #endregion

        #region Game event handlers

        /// <summary>
        /// Játék végének eseménykezelője.
        /// </summary>
        private void Model_GameOver(object sender, SneakingOutEventArgs e)
        {}

        /// <summary>
        /// Játék előrehaladásának eseménykezelője.
        /// </summary>
        private void Model_GameAdvanced(object sender, SneakingOutEventArgs e)
        {
            OnPropertyChanged("GameTime");
            OnPropertyChanged("Fields");
            RefreshTable();
        }

        /// <summary>
        /// Játék létrehozásának eseménykezelője.
        /// </summary>
        private void Model_GameCreated(object sender, SneakingOutEventArgs e)
        {
            RefreshTable();
        }

        #endregion

        #region Event methods

        /// <summary>
        /// jatek megallitasa
        /// </summary>
        private void OnPauseGame()
        {
            if (PauseGame != null)
                PauseGame(this, EventArgs.Empty);
        }

        /// <summary>
        /// jatek ujrakezdese
        /// </summary>
        private void OnRestartGame()
        {
            if (RestartGame != null)
                RestartGame(this, EventArgs.Empty);
        }

        /// <summary>
        /// egyes szint
        /// </summary>
        private void OnLevel1()
        {
            if (Level1 != null)
                Level1(this, EventArgs.Empty);
        }

        /// <summary>
        /// kettes szint
        /// </summary>
        private void OnLevel2()
        {
            if (Level2 != null)
                Level2(this, EventArgs.Empty);
        }

        /// <summary>
        /// harmas szint
        /// </summary>
        private void OnLevel3()
        {
            if (Level3 != null)
                Level3(this, EventArgs.Empty);
        }

        /// <summary>
        /// Játék mentése eseménykiváltása.
        /// </summary>
        private void OnSaveGame()
        {
            if (SaveGame != null)
                SaveGame(this, EventArgs.Empty);
        }

        /// <summary>
        /// Játékból való kilépés eseménykiváltása.
        /// </summary>
        private void OnExitGame()
        {
            if (ExitGame != null)
                ExitGame(this, EventArgs.Empty);
        }

        private void OnUpKeyDown()
        {
            if (UpKeyDown != null)
                UpKeyDown(this, EventArgs.Empty);
        }

        private void OnDownKeyDown()
        {
            if (DownKeyDown != null)
                DownKeyDown(this, EventArgs.Empty);
        }

        private void OnRightKeyDown()
        {
            if (RightKeyDown != null)
                RightKeyDown(this, EventArgs.Empty);
        }

        private void OnLeftKeyDown()
        {
            if(LeftKeyDown != null)
                LeftKeyDown(this, EventArgs.Empty);
        }

        #endregion


    }
}
