using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UI;

public class WorldMapFirstClearRewardSlot : ReuseCellData<WorldMapFirstClearRewardItemCellData>
{
    [SerializeField] Image iconImg;
    [SerializeField] TextMeshProUGUI itemQtyTxt;

    public override void UpdateContent(WorldMapFirstClearRewardItemCellData _itemData)
    {
        switch (_itemData.category)
        {
            case ITEM_CATEGORY.GOODS:
                SetIconSprite(Tables.Goods.Get(_itemData.rewardKey)?.GoodsIcon);
                break;
            case ITEM_CATEGORY.ITEM:
                SetIconSprite(Tables.Item.Get(_itemData.rewardKey)?.ItemIcon);
                break;
            case ITEM_CATEGORY.MATERIAL:
                SetIconSprite(Tables.Material.Get(_itemData.rewardKey)?.MaterialIcon);
                break;
        }
        itemQtyTxt.text = string.Format("x{0}", Utility.ToCurrencyString(_itemData.count));
    }
    void SetIconSprite(string iconName)
    {
        if (!string.IsNullOrEmpty(iconName))
        {
            iconImg.sprite = UIManager.Instance.GetSprite(SPRITE_TYPE.ITEM_ICON, iconName);
        }
    }
}
