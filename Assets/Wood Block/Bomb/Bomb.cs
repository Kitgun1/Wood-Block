using KimicuUtility;
using Playgama;
using Playgama.Modules.Advertisement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

namespace WoodBlock
{
    public sealed class Bomb : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public Image image;
        public float speed;
        public float z;

        private bool grabbed = false;
        private Cell target;

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
                target = GridMap.Instance.PointerCell;
                Bridge.advertisement.rewardedStateChanged += GetAward;
                Bridge.advertisement.ShowRewarded();
            }
        }

        private void GetAward(RewardedState state)
        {
            if(state == RewardedState.Rewarded)
            {
                GridMap.Instance.UseBomb(target);
                TableGenerator.Instance.PushBombInHistory();
                Bridge.advertisement.rewardedStateChanged -= GetAward;
            }
        }
    }
}
