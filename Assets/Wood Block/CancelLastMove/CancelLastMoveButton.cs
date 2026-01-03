using Kimicu.YandexGames;
using System;
using UnityEngine;

namespace WoodBlock
{
    public sealed class CancelLastMoveButton : MonoBehaviour
    {
        public void Cancel()
        {
            if (GridMap.Instance.CanUndo() && Advertisement.AdvertisementIsAvailable)
            {
                Advertisement.ShowVideoAd(onRewardedCallback: Undo);
            }
        }

        private static void Undo()
        {
            GridMap.Instance.Undo();
            TableGenerator.Instance.Undo();
        }
    }
}