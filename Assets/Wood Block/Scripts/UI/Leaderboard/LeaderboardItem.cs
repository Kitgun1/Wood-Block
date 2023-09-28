using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace WoodBlock
{
    public class LeaderboardItem : MonoBehaviour
    {
        [field: SerializeField] public Image Picture { get; private set; }
        [field: SerializeField] public TMP_Text NameProfile { get; private set; }
        [field: SerializeField] public TMP_Text Score { get; private set; }
    }
}