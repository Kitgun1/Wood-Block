using Kimicu.YandexGames;
using KimicuUtility;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace WoodBlock
{
    public sealed class Bomb : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public Image image;
        public float speed;
        public float z;

        private bool grabbed = false;

        private void Update()
        {
            if (grabbed)
            {
                var newPos = Vector3.Lerp
                (
                    transform.position,
                    PlayerInput.PlayerActions.MousePosition.ReadValue<Vector2>().GetWorldSpace(0),
                    Time.deltaTime * speed
                );
                newPos.z = z;
                transform.position = newPos;
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            grabbed = true;
            image.raycastTarget = false;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            grabbed = false;
            image.raycastTarget = true;
            (transform as RectTransform).anchoredPosition = Vector3.zero;

            if (GridMap.Instance.PointerCell != null)
            {
                Advertisement.ShowVideoAd
                (
                    onRewardedCallback: () =>
                    {
                        GridMap.Instance.UseBomb(GridMap.Instance.PointerCell);
                        TableGenerator.Instance.PushBombInHistory();
                    }
                );
            }
        }
    }
}
