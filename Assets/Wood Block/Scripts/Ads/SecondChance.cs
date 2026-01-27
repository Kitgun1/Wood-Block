using Kimicu.YandexGames;
using UnityEngine;
using WoodBlock;

public class SecondChane : MonoBehaviour
{
    [SerializeField] private GameObject[] _panelsToClose;
    [SerializeField] private GridMap _gridMap;

    public void GetSecondChance() =>
        Advertisement.ShowVideoAd(onRewardedCallback: GetAwards, onErrorCallback: (string msg) => Debug.LogError(msg));
    private void GetAwards()
    {
        foreach(var panel in _panelsToClose)
            panel.SetActive(false);

        _gridMap?.DestroyAllBlocks();
    }
}
