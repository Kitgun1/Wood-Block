using AYellowpaper.SerializedCollections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Schema;
using UnityEngine;

[RequireComponent(typeof(GUIDrawer))]
public class ShopSystem : MonoBehaviour
{
    [Header("Default Items Settings")]
    [SerializeField] private string _defaultSkinId = "base_skin";
    [SerializeField] private string _defaultBackgroundId = "base_bg";
    [SerializeField] private bool _autoSelectDefaultItems = true;
    [SerializeField] private List<string> _skinIdThatGetForAdd;
    [SerializeField] private SerializedDictionary<string, int> _skinIdThatGetForLevels;

    [Header("Debug")]
    [SerializeField] private bool _debugMode = false;


    private List<ShopItem> _items = new();
    private string _currentSelectedSkin = string.Empty;
    private string _currentSelectedBackground = string.Empty;

    private HashSet<string> _defaultItemsIds = new();

    public event Action<List<ShopItem>> OnShopUpdated;
    public event Action<List<ShopItem>> OnInitialized;
    public static event Action OnBackgroundSkinChanged;
    public static event Action OnSkinSelected;

    private void Start()
    {
        Initialize();
        GetItemsForLevelComplete();
    }

    public void BuyItem(string itemID)
    {
        var shopItem = GetShopItemByID(itemID);
        if (shopItem == null)
        {
            Debug.LogError($"ShopItem with ID {itemID} not found");
            return;
        }

        if (shopItem.IsBought)
        {
            SelectSkin(itemID);
        }
        else
        {
            switch (shopItem.WaysToUnlockSkin)
            {
                case WaysToUnlockSkin.BuyForCurrencie:
                    Billings.PurchaseProduct(itemID, OnPurchaseSuccess, OnPurchaseError);
                    break;
                case WaysToUnlockSkin.BuyForAdd:
                    GetSkinForAdd(shopItem);
                    break;
                default:
                    break;
            }
        }
    }
    private void GetSkinForAdd(ShopItem shopItem)
    {
        Advertisement.ShowAwardedAdd(() =>
        {
            shopItem.IsBought = true;
            DataSaver.Save(SaveKeys.Products, _items);
            OnProductConsumed(shopItem.CatalogProduct.ID);
        });
    }

    public ShopItem GetShopItemByID(string itemID)
        => _items.FirstOrDefault(x => x.CatalogProduct.ID == itemID);

    public void ConsumeProduct(string productID)
    {
        var shopItem = GetShopItemByID(productID);

        if (shopItem == null)
        {
            Debug.LogError($"Product {productID} not found in shop items");
            return;
        }

        shopItem.IsBought = true;
        DataSaver.Save(SaveKeys.Products, _items);
        OnProductConsumed(productID);
    }

    private void SelectSkin(string itemID)
    {
        var shopItem = GetShopItemByID(itemID);
        if (shopItem == null) return;

        if (!shopItem.IsBought)
        {
            Debug.LogWarning($"Trying to select not bought item: {itemID}");
            return;
        }

        if (shopItem.ProductType == ProductType.Skin)
        {
            _currentSelectedSkin = itemID;
            DataSaver.Save(SaveKeys.SelectedSkinId, _currentSelectedSkin);
            OnSkinSelected?.Invoke();

            if (_debugMode)
                Debug.Log($"Skin selected: {itemID}");
        }
        else
        {
            _currentSelectedBackground = itemID;
            DataSaver.Save(SaveKeys.SelectedBackgroundId, _currentSelectedBackground);
            OnBackgroundSkinChanged?.Invoke();

            if (_debugMode)
                Debug.Log($"Background selected: {itemID}");
        }

        OnShopUpdated?.Invoke(_items);
    }

    private void OnPurchaseSuccess(string response)
    {
        string productID = response;
        var shopItem = GetShopItemByID(productID);

        if (shopItem == null)
        {
            Debug.LogError($"Product {productID} not found in shop items");
            return;
        }

        shopItem.IsBought = true;
        DataSaver.Save(SaveKeys.Products, _items);

        Billings.ConsumeProduct(productID,
            () => OnProductConsumed(productID),
            error => Debug.LogError($"Consume error: {error}"));
    }

    private void OnProductConsumed(string productID)
    {
        if (_debugMode)
            Debug.Log($"Product {productID} consumed successfully");

        OnShopUpdated?.Invoke(_items);
        SelectSkin(productID);
    }

    private void OnPurchaseError(string error)
    {
        Debug.LogError($"Purchase failed: {error}");
    }

    private void GetItemsForLevelComplete()
    {
        List<ShopItem> items = _items.Where(x => x.WaysToUnlockSkin == WaysToUnlockSkin.BuyForLevels).ToList();
        int currentLevel = DataSaver.Load<int>(SaveKeys.CurrentLevel);

        List<ShopItem> resutlItems = items.Where(x => x.IsBought == false && x.LevelToGetSkin == currentLevel).ToList();

        if (resutlItems.Count != 0)
        {
            foreach (ShopItem item in resutlItems)
                item.IsBought = true;

            DataSaver.Save(SaveKeys.Products, _items);

            foreach (ShopItem item in resutlItems)
                OnProductConsumed(item.CatalogProduct.ID);
        }
    }

    private bool CheckIsItemBought(string id)
        => GetShopItemByID(id)?.IsBought ?? false;

    private void Initialize()
    {
        if (!Billings.IsInitialized)
        {
            Debug.LogError("Billing not initialized");
            return;
        }

        InitializeDefaultItemsIds();
        LoadOrCreateShopItems();
        SetupDefaultItems();
        LoadSelectedItems();

        OnInitialized?.Invoke(_items);

        if (_debugMode)
            Debug.Log($"Shop initialized. Total items: {_items.Count}, Default items: {_defaultItemsIds.Count}");
    }

    private void InitializeDefaultItemsIds()
    {
        _defaultItemsIds.Clear();

        if (!string.IsNullOrEmpty(_defaultSkinId))
            _defaultItemsIds.Add(_defaultSkinId);

        if (!string.IsNullOrEmpty(_defaultBackgroundId))
            _defaultItemsIds.Add(_defaultBackgroundId);
    }

    private void LoadOrCreateShopItems()
    {
        if (DataSaver.HasSaves(SaveKeys.Products))
        {
            _items = DataSaver.Load<List<ShopItem>>(SaveKeys.Products);
            EnsureDefaultItemsAreBought();
            SyncWithCurrentCatalog();
            SetWayToGet();
        }
        else
        {
            CreateItemsFromCatalog();
            MarkDefaultItemsAsBought();
        }
    }

    private void EnsureDefaultItemsAreBought()
    {
        bool changed = false;

        foreach (var defaultId in _defaultItemsIds)
        {
            var item = GetShopItemByID(defaultId);
            if (item != null && !item.IsBought)
            {
                item.IsBought = true;
                changed = true;

                if (_debugMode)
                    Debug.Log($"Marked default item as bought: {defaultId}");
            }
        }

        if (changed)
            DataSaver.Save(SaveKeys.Products, _items);
    }

    private void MarkDefaultItemsAsBought()
    {
        foreach (var item in _items)
        {
            if (_defaultItemsIds.Contains(item.CatalogProduct.ID))
            {
                item.IsBought = true;

                if (_debugMode)
                    Debug.Log($"Default item marked as bought: {item.CatalogProduct.ID}");
            }
        }
    }

    private void SetupDefaultItems()
    {
        if (_autoSelectDefaultItems)
        {
            if (!DataSaver.HasSaves(SaveKeys.SelectedSkinId) && !string.IsNullOrEmpty(_defaultSkinId))
            {
                var defaultSkin = GetShopItemByID(_defaultSkinId);
                if (defaultSkin != null && defaultSkin.IsBought)
                {
                    _currentSelectedSkin = _defaultSkinId;
                    DataSaver.Save(SaveKeys.SelectedSkinId, _currentSelectedSkin);

                    if (_debugMode)
                        Debug.Log($"Auto-selected default skin: {_defaultSkinId}");
                }
            }

            if (!DataSaver.HasSaves(SaveKeys.SelectedBackgroundId) && !string.IsNullOrEmpty(_defaultBackgroundId))
            {
                var defaultBackground = GetShopItemByID(_defaultBackgroundId);
                if (defaultBackground != null && defaultBackground.IsBought)
                {
                    _currentSelectedBackground = _defaultBackgroundId;
                    DataSaver.Save(SaveKeys.SelectedBackgroundId, _currentSelectedBackground);

                    if (_debugMode)
                        Debug.Log($"Auto-selected default background: {_defaultBackgroundId}");
                }
            }
        }
    }

    private void SetWayToGet()
    {
        foreach(var item in _items)
        {
            var way = GetWayToUnlockSkin(item.CatalogProduct.ID);
            item.WaysToUnlockSkin = way;
            item.LevelToGetSkin = _skinIdThatGetForLevels.GetValueOrDefault(item.CatalogProduct.ID);
        }
        DataSaver.Save(SaveKeys.Products, _items);
    }
    private void SyncWithCurrentCatalog()
    {
        var existingIds = _items.Select(x => x.CatalogProduct.ID).ToHashSet();
        var newProducts = Billings.CatalogProducts.Where(x => !existingIds.Contains(x.ID));

        foreach (var product in newProducts)
        {
            var productType = GetProductType(product.ID);
            WaysToUnlockSkin way = GetWayToUnlockSkin(product.ID);
            var isDefault = _defaultItemsIds.Contains(product.ID);

            _items.Add(new ShopItem(isDefault, product, productType, way, _skinIdThatGetForLevels.GetValueOrDefault(product.ID)));

            if (_debugMode && isDefault)
                Debug.Log($"Added new default item from catalog: {product.ID}");
        }

        if (newProducts.Any())
        {
            DataSaver.Save(SaveKeys.Products, _items);
        }
    }

    private void CreateItemsFromCatalog()
    {
        _items.Clear();
        var Items = Billings.CatalogProducts.ToList();
        AddItemsToList(Items);
    }

    private void AddItemsToList(List<CatalogProduct> catalogProduts)
    {
        foreach (var product in catalogProduts)
        {
            ProductType productType = GetProductType(product.ID);
            WaysToUnlockSkin way = GetWayToUnlockSkin(product.ID);
            bool isDefault = _defaultItemsIds.Contains(product.ID);

            if (productType != ProductType.None)
            {
                _items.Add(new ShopItem(isDefault, product, productType, way, _skinIdThatGetForLevels.GetValueOrDefault(product.ID)));
            }

            if (_debugMode)
                Debug.Log($"Created item: {product.ID}, Type: {productType}, IsDefault: {isDefault}, Way: {way}");
        }
    }

    private ProductType GetProductType(string productId)
    {
        if (productId.Contains("skin"))
            return ProductType.Skin;
        else if (productId.Contains("bg"))
            return ProductType.Background;
        else
        {
            Debug.LogWarning($"Unknown product type for {productId}, defaulting to Skin");
            return ProductType.None;
        }
    }
    private WaysToUnlockSkin GetWayToUnlockSkin(string productId)
    {
        if (_skinIdThatGetForAdd.Contains(productId))
            return WaysToUnlockSkin.BuyForAdd;
        else if (_skinIdThatGetForLevels.ContainsKey(productId))
            return WaysToUnlockSkin.BuyForLevels;
        else
            return WaysToUnlockSkin.BuyForCurrencie;
    }

    private void LoadSelectedItems()
    {
        if (DataSaver.HasSaves(SaveKeys.SelectedSkinId))
        {
            _currentSelectedSkin = DataSaver.Load<string>(SaveKeys.SelectedSkinId);

            var selectedSkin = GetShopItemByID(_currentSelectedSkin);
            if (selectedSkin == null || !selectedSkin.IsBought)
            {
                if (_debugMode)
                    Debug.LogWarning($"Selected skin {_currentSelectedSkin} is invalid, resetting to default");

                ResetToDefaultSkin();
            }
        }
        else
        {
            ResetToDefaultSkin();
        }

        if (DataSaver.HasSaves(SaveKeys.SelectedBackgroundId))
        {
            _currentSelectedBackground = DataSaver.Load<string>(SaveKeys.SelectedBackgroundId);

            var selectedBackground = GetShopItemByID(_currentSelectedBackground);
            if (selectedBackground == null || !selectedBackground.IsBought)
            {
                if (_debugMode)
                    Debug.LogWarning($"Selected background {_currentSelectedBackground} is invalid, resetting to default");

                ResetToDefaultBackground();
            }
        }
        else
        {
            ResetToDefaultBackground();
        }
    }

    private void ResetToDefaultSkin()
    {
        if (!string.IsNullOrEmpty(_defaultSkinId))
        {
            var defaultSkin = GetShopItemByID(_defaultSkinId);
            if (defaultSkin != null && defaultSkin.IsBought)
            {
                _currentSelectedSkin = _defaultSkinId;
                DataSaver.Save(SaveKeys.SelectedSkinId, _currentSelectedSkin);
                OnSkinSelected?.Invoke();

                if (_debugMode)
                    Debug.Log($"Reset to default skin: {_defaultSkinId}");
            }
        }
    }

    private void ResetToDefaultBackground()
    {
        if (!string.IsNullOrEmpty(_defaultBackgroundId))
        {
            var defaultBackground = GetShopItemByID(_defaultBackgroundId);
            if (defaultBackground != null && defaultBackground.IsBought)
            {
                _currentSelectedBackground = _defaultBackgroundId;
                DataSaver.Save(SaveKeys.SelectedBackgroundId, _currentSelectedBackground);
                OnBackgroundSkinChanged?.Invoke();

                if (_debugMode)
                    Debug.Log($"Reset to default background: {_defaultBackgroundId}");
            }
        }
    }

    // Публичные методы для управления дефолтными предметами
    public void SetDefaultSkin(string skinId)
    {
        if (string.IsNullOrEmpty(skinId))
        {
            Debug.LogError("Cannot set empty default skin ID");
            return;
        }

        _defaultSkinId = skinId;
        InitializeDefaultItemsIds();

        // Убеждаемся, что новый дефолтный скин отмечен как купленный
        var skinItem = GetShopItemByID(skinId);
        if (skinItem != null && !skinItem.IsBought)
        {
            skinItem.IsBought = true;
            DataSaver.Save(SaveKeys.Products, _items);
        }

        if (_debugMode)
            Debug.Log($"Default skin changed to: {skinId}");
    }

    public void SetDefaultBackground(string backgroundId)
    {
        if (string.IsNullOrEmpty(backgroundId))
        {
            Debug.LogError("Cannot set empty default background ID");
            return;
        }

        _defaultBackgroundId = backgroundId;
        InitializeDefaultItemsIds();

        var backgroundItem = GetShopItemByID(backgroundId);
        if (backgroundItem != null && !backgroundItem.IsBought)
        {
            backgroundItem.IsBought = true;
            DataSaver.Save(SaveKeys.Products, _items);
        }

        if (_debugMode)
            Debug.Log($"Default background changed to: {backgroundId}");
    }

    public bool IsDefaultItem(string itemId) => _defaultItemsIds.Contains(itemId);

    public string GetDefaultSkinId() => _defaultSkinId;

    public string GetDefaultBackgroundId() => _defaultBackgroundId;

    // Вспомогательные методы
    public List<ShopItem> GetItemsByType(ProductType type)
        => _items.Where(x => x.ProductType == type).ToList();

    public ShopItem GetSelectedSkin()
        => GetShopItemByID(_currentSelectedSkin);

    public ShopItem GetSelectedBackground()
        => GetShopItemByID(_currentSelectedBackground);

    public void ResetAllPurchases() // Для отладки
    {
        foreach (var item in _items)
        {
            // Не сбрасываем дефолтные предметы
            if (!_defaultItemsIds.Contains(item.CatalogProduct.ID))
            {
                item.IsBought = false;
            }
        }

        DataSaver.Save(SaveKeys.Products, _items);
        ResetToDefaultSkin();
        ResetToDefaultBackground();
        OnShopUpdated?.Invoke(_items);

        if (_debugMode)
            Debug.Log("All non-default purchases reset");
    }
}
[Serializable]
public class ShopItem
{
    private bool _isBought;
    private CatalogProduct _catalogProduct;
    private ProductType _productType;
    private WaysToUnlockSkin _waysToUnlockSkin;
    private int _levelToGetSkin = 0;

    public bool IsBought
    {
        get => _isBought;
        set => _isBought = value;
    }

    public CatalogProduct CatalogProduct => _catalogProduct;
    public ProductType ProductType => _productType;
    public WaysToUnlockSkin WaysToUnlockSkin{get => _waysToUnlockSkin; set => _waysToUnlockSkin = value; }
    public int LevelToGetSkin { get => _levelToGetSkin; set => _levelToGetSkin = value; }

    public ShopItem(bool isBought, CatalogProduct catalogProduct, ProductType productType, WaysToUnlockSkin wayToUnlockSkin, int levelToGetSkin = 0)
    {
        _isBought = isBought;
        _catalogProduct = catalogProduct;
        _productType = productType;
        _waysToUnlockSkin = wayToUnlockSkin;
        _levelToGetSkin = levelToGetSkin;
    }
}