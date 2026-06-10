using AYellowpaper.SerializedCollections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WoodBlock;

[CreateAssetMenu(fileName = "New Skin Data")]
public class SkinsData : ScriptableObject
{
    [SerializedDictionary("Key","Skin")]
    [field: SerializeField] private SerializedDictionary<string,SkinBackground> _backgroundSkins = new();
    [SerializedDictionary("Key", "Skin")]
    [field: SerializeField] private SerializedDictionary<string, Skin> _skins = new();

    public SkinBackground GetBackgroundSkin(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            name = "base_bg";
        }

        if (_backgroundSkins.TryGetValue(name, out var skin) && skin != null)
            return skin;

        if (name != "base_bg" && _backgroundSkins.TryGetValue("base_bg", out var defaultSkin) && defaultSkin != null)
            return defaultSkin;

        foreach (var pair in _backgroundSkins)
        {
            if (pair.Value != null)
                return pair.Value;
        }

        return null;
    }

    public Skin GetSkin(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            name = "base_skin";
        }

        if (_skins.TryGetValue(name, out var skin) && skin != null)
            return skin;

        if (name != "base_skin" && _skins.TryGetValue("base_skin", out var defaultSkin) && defaultSkin != null)
            return defaultSkin;

        foreach (var pair in _skins)
        {
            if (pair.Value != null)
                return pair.Value;
        }

        return null;
    }
}
[Serializable]
public class SkinBackground
{
    [SerializeField] private Sprite _background;
    [SerializeField] private Sprite _frameForCell;
    [SerializeField] private Sprite _shoesBlockFrame;
    [SerializeField] private Sprite _gameFrame;
    [SerializeField] private Sprite _button;
    [SerializeField] private Sprite _buyButton;
    [SerializeField] private Sprite _losePanel;

    public Sprite Background { get =>  _background; }
    public Sprite FrameForCell { get => _frameForCell; }
    public Sprite ShoesBlockFrame { get => _shoesBlockFrame; }
    public Sprite GameFrame { get => _gameFrame; }
    public Sprite Button { get => _button; }
    public Sprite BuyButton { get => _buyButton; }
    public Sprite LosePanel { get => _losePanel; }
}

[Serializable]
public class Skin
{
    [SerializeField] private Sprite _cell;
    public Sprite Cell { get => _cell; }
}
