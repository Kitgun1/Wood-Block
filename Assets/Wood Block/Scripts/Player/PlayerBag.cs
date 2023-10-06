using KiYandexSDK;

namespace WoodBlock
{
    public static class PlayerBag
    {
        public static int MaxScore { get; private set; } = 0;
        public static int CoinAmount { get; private set; } = 100;
        public static int CancelMoveAmount { get; private set; } = 1;
        public static int ClearTableAmount { get; private set; } = 1;

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