using System;
using System.Collections.Generic;
using System.Text;

namespace SneakingOut_Xamarin.ViewModel
{
    public class SneakingOutField : ViewModelBase
    {
        private Boolean _isWall;
        private Boolean _isPlayer;
        private Boolean _isSecurity;
        private Boolean _isEmpty;
        private Boolean _isExit;

        public Boolean IsSecurity
        {
            get { return _isSecurity; }
            set
            {
                if (_isSecurity != value)
                {
                    _isSecurity = value;
                    OnPropertyChanged();
                }
            }
        }

        public Boolean IsPlayer
        {
            get { return _isPlayer; }
            set
            {
                if (_isPlayer != value)
                {
                    _isPlayer = value;
                    OnPropertyChanged();
                }
            }
        }

        public Boolean IsWall
        {
            get { return _isWall; }
            set
            {
                if (_isWall != value)
                {
                    _isWall = value;
                    OnPropertyChanged();
                }
            }
        }

        public Boolean IsEmpty
        {
            get { return _isEmpty; }
            set
            {
                if (_isEmpty != value)
                {
                    _isEmpty = value;
                    OnPropertyChanged();
                }
            }
        }

        public Boolean IsExit
        {
            get { return _isExit; }
            set
            {
                if (_isExit != value)
                {
                    _isExit = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Függőleges koordináta lekérdezése, vagy beállítása.
        /// </summary>
        public Int32 X { get; set; }

        /// <summary>
        /// Vízszintes koordináta lekérdezése, vagy beállítása.
        /// </summary>
        public Int32 Y { get; set; }

      
        /// <summary>
        /// Lépés parancs lekérdezése, vagy beállítása.
        /// </summary>
        public DelegateCommand StepCommand { get; set; }
    }
}
