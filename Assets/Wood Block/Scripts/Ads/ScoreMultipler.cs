using Kimicu.YandexGames;
using System.Collections;
using UnityEngine;
using WoodBlock;

public class ScoreMultipler : MonoBehaviour
{
    [SerializeField] private GridMap _score;

    public void GetMultiplier() =>
    Advertisement.ShowVideoAd(onRewardedCallback: StartGetAwardsCorutine, onErrorCallback: (string msg) => Debug.LogError(msg));
    private void StartGetAwardsCorutine() => StartCoroutine(GetAwards());
    private IEnumerator GetAwards()
    {
        _score.IsMultiplierEnabled = true;
        yield return new WaitForSeconds(30);
        _score.IsMultiplierEnabled = false;
    }
}
