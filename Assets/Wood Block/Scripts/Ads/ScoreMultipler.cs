using Playgama;
using Playgama.Modules.Advertisement;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using WoodBlock;

[RequireComponent(typeof(Button))]
public class ScoreMultipler : MonoBehaviour
{
    [SerializeField] private GridMap _score;
    private Button _scoreButton;

    private void Start() => _scoreButton = GetComponent<Button>();

    public void GetMultiplier()
    {
        Advertisement.ShowAwardedAdd(() =>
        {
            StartCoroutine(GetAwards());
        });
    }

    private IEnumerator GetAwards()
    {
        _score.IsMultiplierEnabled = true;
        _scoreButton.interactable = false;
        yield return new WaitForSeconds(30);
        _score.IsMultiplierEnabled = false;
        _scoreButton.interactable = true;
    }
}
