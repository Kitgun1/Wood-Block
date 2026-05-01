using Playgama;
using Playgama.Modules.Advertisement;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace WoodBlock
{
    [RequireComponent(typeof(Button))]
    public sealed class CancelLastMoveButton : MonoBehaviour
    {
        private Button _button;
        private void Start() => _button = GetComponent<Button>();
        private void FixedUpdate()
        {
            if(GridMap.Instance.CalculateBlocksCount() == 0) _button.interactable = false;
            else _button.interactable = true;

        }
        public void Cancel()
        {
            if (GridMap.Instance.CanUndo())
            {
                Bridge.advertisement.rewardedStateChanged += Undo;
                Bridge.advertisement.ShowRewarded();
            }
        }

        private static void Undo(RewardedState state)
        {
            if(state == RewardedState.Rewarded)
            {
                GridMap.Instance.Undo();
                TableGenerator.Instance.Undo();
                Bridge.advertisement.rewardedStateChanged -= Undo;
            }
        }
    }
}