using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace WoodBlock
{
    public class ButtonLink : MonoBehaviour, IPointerUpHandler
    {
        [SerializeField] private string _url;

        public UnityEvent OnOpen;

        public void OnPointerUp(PointerEventData eventData)
        {
            Application.OpenURL(_url);
            OnOpen?.Invoke();
        }
    }
}