namespace WoodBlock
{
    public class CoinDisplay : ValueDisplay
    {
        protected override void Start()
        {
            PlayerBag.CoinAmountChanged.AddListener(UpdateText);
            UpdateText(PlayerBag.CoinAmount);
        }

        protected override void UpdateText(int value)
        {
            ValueTMP.text = value.ToString();
        }
    }
}