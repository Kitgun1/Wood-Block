using System;

namespace WoodBlock
{
    [Flags]
    public enum WindowType
    {
        None = 0,
        StartMenu = 1 << 0,
        SettingsPopup = 1 << 1,
        Leaderboard = 1 << 2,
        GameView = 1 << 3,
        Market = 1 << 4,
    }
}