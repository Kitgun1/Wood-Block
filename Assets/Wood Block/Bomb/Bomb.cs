using Kimicu.YandexGames;
using KimicuUtility;
using UnityEngine;
using UnityEngine.EventSystems;

namespace WoodBlock
{
    public sealed class Bomb : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        private void Update()
        {
            transform.position = Vector3.Lerp
            (   
                transform.position, 
                PlayerInput.PlayerActions.MousePosition.ReadValue<Vector2>().GetWorldSpace(0), 
                Time.deltaTime
            );
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            enabled = true;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            enabled = false;
            (transform as RectTransform).anchoredPosition = Vector3.zero;

            if (GridMap.Instance.PointerCell != null)
            {
                Advertisement.ShowVideoAd(onRewardedCallback: () => GridMap.Instance.UseBomb(GridMap.Instance.PointerCell));
            }
        }
    }
}
