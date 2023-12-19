using CW.Common;
using KimicuUtility;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace WoodBlock
{
    public class DisposableShopProduct : MonoBehaviour
    {
        [SerializeField] private Button _buyButton;
        [SerializeField] protected Image Picture;
        [SerializeField] protected TMP_Text TMPPriceJan;

        [field: SerializeField] public string ProductId { get; private set; } = "empty id";

        private int _priceJan = 1;

        public UnityEvent<string, int> OnClickBuyJan { get; private set; } = new();

        protected int PriceJan
        {
            get => _priceJan;
            set
            {
                _priceJan = value;
                TMPPriceJan.text = value.ToString();
            }
        }

        public DisposableShopProduct InitializeClick()
        {
            _buyButton.RemoveAllListener();
            _buyButton.AddListener(() => OnClickBuyJan.Invoke(ProductId, PriceJan));
            return this;
        }

        public DisposableShopProduct SetPicture(Sprite sprite)
        {
            Picture.sprite = sprite;
            if (sprite != Resources.Load("Sprites/loading", typeof(Sprite)))
            {
                Picture.GetComponent<CwRotate>().AngularVelocity = new Vector3(0, 0, 0);
                Picture.transform.rotation = Quaternion.identity;
            }
            else Picture.GetComponent<CwRotate>().AngularVelocity = new Vector3(0, 0, -360);
            return this;
        }

        public DisposableShopProduct SetPriceJan(int value)
        {
            PriceJan = value;
            return this;
        }
    }
}

