using Playgama;
using UnityEngine;

public class InterstitialAd : MonoBehaviour
{
    public void ShowAdd() => Bridge.advertisement.ShowInterstitial();
}
