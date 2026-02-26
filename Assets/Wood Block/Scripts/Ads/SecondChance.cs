using Kimicu.YandexGames;
using UnityEngine;
using UnityEngine.Events;
using WoodBlock;

public class SecondChane : MonoBehaviour
{
    [SerializeField] private UnityEvent _eventToDo;
    [SerializeField] private GamePause _gamePause;
    [SerializeField] private GridMap _gridMap;

    public void GetSecondChance() =>
        Advertisement.ShowVideoAd(onRewardedCallback: GetAwards, onErrorCallback: (string msg) => Debug.LogError(msg));
    private void GetAwards()
    {
        _eventToDo?.Invoke();

        _gridMap?.DestroyAllBlocks();
        _gamePause.StopPause();
    }
}
