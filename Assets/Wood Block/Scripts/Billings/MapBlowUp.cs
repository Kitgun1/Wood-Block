using Agava.YandexGames;
using Kimicu.YandexGames;
using UnityEngine;
using WoodBlock;
using Billing = Kimicu.YandexGames.Billing;

public class MapBlowUp : MonoBehaviour
{
    [SerializeField] private GridMap _gridMap;

    public void BlowUp(string itemID)
    {
        if (Billing.Initialized)
            Billing.PurchaseProduct(itemID, ConsumePayment, Debug.LogError);
        else
            Debug.LogError("Billing not initialized");
    }
    private void ConsumePayment(PurchaseProductResponse response)
    {
        Billing.ConsumeProduct(response.purchaseData.purchaseToken, () => _gridMap?.DestroyAllBlocks());
    }
}
