using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace JudoApp
{
    public enum BeltColor
    {
        White,
        Red
    }

    public class TatamiMatchState : INotifyPropertyChanged
    {
        private int _tatamiNumber;
        private FightDisplayModel _currentFight;
        private FightDisplayModel _nextFight;
        private int _mainTimerSeconds = DefaultMainDurationSeconds;
        private int _holdTimerSeconds;
        private bool _isMainTimerRunning;
        private bool _isHoldTimerRunning;
        private bool _highlightWhite;
        private bool _highlightRed;
        private string _statusMessage = "Введите номер татами, чтобы начать.";

        public const int DefaultMainDurationSeconds = 120;
        public const int DefaultHoldDurationSeconds = 20;

        public ObservableCollection<FightDisplayModel> UpcomingFights { get; } = new ObservableCollection<FightDisplayModel>();

        public ScoreCard WhiteScore { get; } = new ScoreCard();
        public ScoreCard RedScore { get; } = new ScoreCard();

        public event PropertyChangedEventHandler PropertyChanged;

        public int TatamiNumber
        {
            get => _tatamiNumber;
            set => SetField(ref _tatamiNumber, value);
        }

        public FightDisplayModel CurrentFight
        {
            get => _currentFight;
            set => SetField(ref _currentFight, value);
        }

        public FightDisplayModel NextFight
        {
            get => _nextFight;
            set => SetField(ref _nextFight, value);
        }

        public int MainTimerSeconds
        {
            get => _mainTimerSeconds;
            set => SetField(ref _mainTimerSeconds, value);
        }

        public int HoldTimerSeconds
        {
            get => _holdTimerSeconds;
            set => SetField(ref _holdTimerSeconds, value);
        }

        public bool IsMainTimerRunning
        {
            get => _isMainTimerRunning;
            set => SetField(ref _isMainTimerRunning, value);
        }

        public bool IsHoldTimerRunning
        {
            get => _isHoldTimerRunning;
            set => SetField(ref _isHoldTimerRunning, value);
        }

        public bool HighlightWhite
        {
            get => _highlightWhite;
            set => SetField(ref _highlightWhite, value);
        }

        public bool HighlightRed
        {
            get => _highlightRed;
            set => SetField(ref _highlightRed, value);
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set => SetField(ref _statusMessage, value);
        }

        public void ResetScores()
        {
            WhiteScore.Reset();
            RedScore.Reset();
            HighlightWhite = false;
            HighlightRed = false;
            HoldTimerSeconds = 0;
            IsHoldTimerRunning = false;
            MainTimerSeconds = DefaultMainDurationSeconds;
            IsMainTimerRunning = false;
        }

        protected void SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(field, value))
            {
                return;
            }

            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class ScoreCard : INotifyPropertyChanged
    {
        private int _ippon;
        private int _wazaAri;
        private int _penalties;
        private bool _isDisqualified;

        public event PropertyChangedEventHandler PropertyChanged;

        public int Ippon
        {
            get => _ippon;
            set => SetField(ref _ippon, value);
        }

        public int WazaAri
        {
            get => _wazaAri;
            set => SetField(ref _wazaAri, value);
        }

        public int Penalties
        {
            get => _penalties;
            set => SetField(ref _penalties, value);
        }

        public bool IsDisqualified
        {
            get => _isDisqualified;
            set => SetField(ref _isDisqualified, value);
        }

        public void Reset()
        {
            Ippon = 0;
            WazaAri = 0;
            Penalties = 0;
            IsDisqualified = false;
        }

        protected void SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(field, value))
            {
                return;
            }

            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

