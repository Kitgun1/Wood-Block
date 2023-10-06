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

        public UnityEvent<string, int> OnClickBuyJan { get; private set; }

        protected int PriceJan
        {
            get => _priceJan;
            set
            {
                _priceJan = value;
                TMPPriceJan.text = value.ToString();
            }
        }

        public virtual void GetReward()
        {
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
            return this;
        }

        public DisposableShopProduct SetPriceJan(int value)
        {
            PriceJan = value;
            return this;
        }
    }
}