using Kimicu.YandexGames;
using KimicuUtility;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using WoodBlock;

public class SecondChane : MonoBehaviour
{
    [SerializeField] private UnityEvent _eventToDo;
    [SerializeField] private GamePause _gamePause;
    [SerializeField] private GridMap _gridMap;
    [SerializeField] private List<GameObject> _panelsToClose;
    [SerializeField] private List<KiCanvasGroup> _panelsWithCanvasGroupToClose;
    [SerializeField] private bool _isUseKiCanvasGroup = false;

    public void GetSecondChance() =>
        Advertisement.ShowVideoAd(onRewardedCallback: GetAwards, onErrorCallback: (string msg) => Debug.LogError(msg));
    private void GetAwards()
    {
        _eventToDo?.Invoke();

        _gridMap?.DestroyAllBlocks();
        _gamePause.StopPause();

        if (_isUseKiCanvasGroup)
        {
            foreach (var group in _panelsWithCanvasGroupToClose)
                group.TurnOff();
        }
        else
        {
            foreach (GameObject go in _panelsToClose)
            {
                go.SetActive(false);
            }
        }
    }
}
