using Kimicu.YandexGames;
using UnityEngine.Events;

namespace WoodBlock
{
    public static class PlayerBag
    {
        private static int _maxScore = 0;
        private static int _coinAmount = 0;
        private static int _cancelMoveAmount = 0;
        private static int _clearTableAmount = 0;

        public static int MaxScore
        {
            get => _maxScore;
            set
            {
                _maxScore = value;
                MaxScoreChanged?.Invoke(value);
            }
        }

        public static int CoinAmount
        {
            get => _coinAmount;
            set
            {
                _coinAmount = value;
                CoinAmountChanged?.Invoke(value);
            }
        }

        public static int CancelMoveAmount
        {
            get => _cancelMoveAmount;
            set
            {
                _cancelMoveAmount = value;
                CancelMoveAmountChanged?.Invoke(value);
            }
        }

        public static int ClearTableAmount
        {
            get => _clearTableAmount;
            set
            {
                _clearTableAmount = value;
                ClearTableAmountChanged?.Invoke(value);
            }
        }

        public static readonly UnityEvent<int> MaxScoreChanged = new();
        public static readonly UnityEvent<int> CoinAmountChanged = new();
        public static readonly UnityEvent<int> CancelMoveAmountChanged = new();
        public static readonly UnityEvent<int> ClearTableAmountChanged = new();

        public static void LoadOrCreate()
        {
            MaxScore = (int)YandexData.Load(nameof(MaxScore), MaxScore);
            CoinAmount = (int)YandexData.Load(nameof(CoinAmount), CoinAmount);
            CancelMoveAmount = (int)YandexData.Load(nameof(CancelMoveAmount), CancelMoveAmount);
            ClearTableAmount = (int)YandexData.Load(nameof(ClearTableAmount), ClearTableAmount);
        }

        public static void Save()
        {
            YandexData.Save(nameof(MaxScore), MaxScore);
            YandexData.Save(nameof(CoinAmount), CoinAmount);
            YandexData.Save(nameof(CancelMoveAmount), CancelMoveAmount);
            YandexData.Save(nameof(ClearTableAmount), ClearTableAmount);
        }
    }
}