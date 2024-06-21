using Tables;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class QuestSlot : ReuseCellData<QuestSlotCellData>
{
    [SerializeField] Image rewardItemIcon;
    [SerializeField] TextMeshProUGUI itemQtyTxt;
    [SerializeField] TextMeshProUGUI questName;
    [SerializeField] TextMeshProUGUI questDesc;
    [SerializeField] Image progressFillImg;
    [SerializeField] TextMeshPro progressTxt;



    public override void UpdateContent(QuestSlotCellData _itemData)
    {

        Reward rewardTb = Reward.Get(_itemData.m_QuestTb.QuestReward);
        double rewardQty = 0;
        switch (_itemData.rewardItemCartegory)
        {
            case ITEM_CATEGORY.GOODS:
                SetIconSprite(Tables.Goods.Get(rewardTb.GoodsKey[0])?.GoodsIcon);
                rewardQty = rewardTb.GoodsQty[0];
                break;
            case ITEM_CATEGORY.MATERIAL:
                SetIconSprite(Tables.Material.Get(rewardTb.GoodsKey[0])?.MaterialIcon);
                rewardQty = rewardTb.MaterialQty[0];
                break;
            case ITEM_CATEGORY.ITEM:
                SetIconSprite(Tables.Item.Get(rewardTb.GoodsKey[0])?.ItemIcon);
                rewardQty = rewardTb.ItemQty[0];
                break;
        }
        itemQtyTxt.text = string.Format("x{0}", Utility.ToCurrencyString(rewardQty));
    }
    void SetIconSprite(string iconName)
    {
        if (!string.IsNullOrEmpty(iconName))
        {
            rewardItemIcon.sprite = UIManager.Instance.GetSprite(SPRITE_TYPE.ITEM_ICON, iconName);
        }
    }


    public void OnClickGetRewardBtn()
    {
        GameManager.Instance.GetReward(m_data.m_QuestTb.QuestReward, out bool result);
        if (result)
        {

        }
    }
    public void OnClickShortCutBtn()
    {

    }
}
