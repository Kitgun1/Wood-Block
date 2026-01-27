using Kimicu.YandexGames;
using UnityEngine;

public class GameShortcuts : MonoBehaviour
{
    public void AddGameToDesktop()
    {
        Shortcut.CanSuggest((bool isCan) => 
        {
            if (isCan) Shortcut.Suggest(); 
            else Debug.Log("Cant to add game icon to desktop now"); 
        });
    }
}
