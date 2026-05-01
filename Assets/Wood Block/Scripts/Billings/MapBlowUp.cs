using UnityEngine;
using WoodBlock;

public class MapBlowUp : MonoBehaviour
{
    [SerializeField] private GridMap _gridMap;

    public void BlowUp(string itemID)
    {
        Billings.PurchaseProduct(itemID, ConsumePayment, Debug.LogError);
    }
    private void ConsumePayment(string id)
    {
        Billings.ConsumeProduct(id,() => _gridMap?.DestroyAllBlocks());
    }
}
