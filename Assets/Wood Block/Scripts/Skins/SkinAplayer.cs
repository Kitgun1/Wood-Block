using System;
using UnityEngine;
using UnityEngine.UI;

public class SkinAplayer : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenederer;
    [SerializeField] private Image _buttonRender;
    [SerializeField] private SkinsData _skinsData;
    [SerializeField] private SkinType _type;

    public void Start() => Applay();

    private void OnEnable() => ShopSystem.OnBackgroundSkinChanged += Applay;
    private void OnDisable() => ShopSystem.OnBackgroundSkinChanged -= Applay;

    private void Applay()
    {
        switch (_type)
        {
            case SkinType.Cell:
                if (DataSaver.HasSaves(SaveKeys.SelectedSkinId))
                {
                    Skin skin = _skinsData.GetSkin(DataSaver.Load<string>(SaveKeys.SelectedSkinId));
                    ShoesSkin(skin);
                }
                else
                {
                    Skin skin = _skinsData.GetSkin("base_skin");
                    ShoesSkin(skin);
                }

                break;
            default:
                if (DataSaver.HasSaves(SaveKeys.SelectedBackgroundId))
                {
                    SkinBackground skin = _skinsData.GetBackgroundSkin(DataSaver.Load<string>(SaveKeys.SelectedBackgroundId));
                    ShoesSkin(skin);
                }
                else
                {
                    SkinBackground skin = _skinsData.GetBackgroundSkin("base_bg");
                    ShoesSkin(skin);
                }
                break;
        }

    }

    private void ShoesSkin(SkinBackground skin)
    {
        switch (_type)
        {
            case SkinType.Background:
                _spriteRenederer.sprite = skin.Background;
                break;
            case SkinType.FrameForBlockContaiern:
                _spriteRenederer.sprite = skin.ShoesBlockFrame;
                break;
            case SkinType.FrameForCell:
                _spriteRenederer.sprite = skin.FrameForCell;
                break;
            case SkinType.GameFrame:
                _spriteRenederer.sprite = skin.GameFrame;
                break;
            case SkinType.Button:
                _buttonRender.sprite = skin.Button;
                break;
            case SkinType.BuyButton:
                _buttonRender.sprite = skin.BuyButton;
                break;
            case SkinType.ShopButtonLine:
                _buttonRender.sprite = skin.ShoesBlockFrame;
                break;
            case SkinType.LosePanel:
                _buttonRender.sprite = skin.LosePanel;
                break;
        }
    }
    private void ShoesSkin(Skin skin)
    {
        if (skin == null)
        {
            Debug.Log("Skin is null");
            return;
        }
        _spriteRenederer.sprite = skin.Cell;
        _spriteRenederer.sortingOrder = 10;
        SetSpriteSize(_spriteRenederer, 102, 104);
    }

    private void SetSpriteSize(SpriteRenderer spriteRenderer, int targetWidthPixels, int targetHeightPixels)
    {
        if (spriteRenderer.sprite == null) return;

        float originalPixelWidth = spriteRenderer.sprite.rect.width;
        float originalPixelHeight = spriteRenderer.sprite.rect.height;

        float ppu = spriteRenderer.sprite.pixelsPerUnit;

        float originalWorldWidth = originalPixelWidth / ppu;
        float originalWorldHeight = originalPixelHeight / ppu;

        float targetWorldWidth = targetWidthPixels / ppu;
        float targetWorldHeight = targetHeightPixels / ppu;

        float scaleX = targetWorldWidth / originalWorldWidth;
        float scaleY = targetWorldHeight / originalWorldHeight;

        spriteRenderer.transform.localScale = new Vector3(scaleX, scaleY, 1f);


        var collider = GetComponent<BoxCollider2D>();
        Vector2 spriteSize = spriteRenderer.sprite.bounds.size;
        collider.size = spriteSize;
    }
}
