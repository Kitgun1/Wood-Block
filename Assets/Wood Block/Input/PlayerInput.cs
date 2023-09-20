using UnityEngine;

namespace WoodBlock
{
    public static class PlayerInput
    {
        private static bool _initialized;

        private static PlayerInputAction _input;
        public static PlayerInputAction.PlayerActions PlayerActions => _input.Player;

        public static void Initialize()
        {
            if (_initialized)
            {
                Debug.LogWarning($"{nameof(PlayerInput)} is initialized!");
                return;
            }

            _input = new PlayerInputAction();
            _input.Enable();

            _initialized = true;
        }
    }
}