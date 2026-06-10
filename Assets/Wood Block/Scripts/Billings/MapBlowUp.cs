using System;
using System.Linq;
using TMPro;
using UnityEngine;
using WoodBlock;

public class MapBlowUp : MonoBehaviour
{
    [SerializeField] private GridMap _gridMap;
    [SerializeField] private string _productId = "mapBlowUp";
    [SerializeField] private TMP_Text _priceText;

    private void Start()
    {
        UpdatePriceText();
    }

    private void OnEnable()
    {
        UpdatePriceText();
    }

    public void UpdatePriceText()
    {
        if (_priceText == null) return;

        if (Billings.IsInitialized && Billings.CatalogProducts != null)
        {
            var product = Billings.CatalogProducts.FirstOrDefault(x => x.ID == _productId);
            if (product != null)
            {
                _priceText.text = product.Price;
                return;
            }
        }
        _priceText.text = "";
    }

    public void BlowUp(string itemID)
    {
        Billings.PurchaseProduct(itemID, ConsumePayment, Debug.LogError);
    }
    private void ConsumePayment(string id)
    {
        Billings.ConsumeProduct(id,() => _gridMap?.DestroyAllBlocks());
    }
}
