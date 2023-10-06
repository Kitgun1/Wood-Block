using TMPro;
using UnityEngine;

namespace WoodBlock
{
    public class QuantitativeShopProduct : DisposableShopProduct
    {
        [SerializeField] protected TMP_Text TMPAmount;

        private int _amountReward;

        protected int AmountReward
        {
            get => _amountReward;
            set
            {
                _amountReward = value;
                TMPAmount.text = value.ToString();
            }
        }

        public DisposableShopProduct SetAmountReward(int value)
        {
            AmountReward = value;
            return this;
        }

        public override void GetReward()
        {
            
        }
    }
}