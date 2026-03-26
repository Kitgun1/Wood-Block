using UnityEngine;
using UnityEngine.UI;

public class ScrollBarValueSetter : MonoBehaviour
{
    private Scrollbar _scrollBar;
    private void OnEnable()
    {
        _scrollBar = GetComponent<Scrollbar>();

        _scrollBar.value = 1f;
    }

}
