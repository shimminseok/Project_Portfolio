using System.Collections;
using System.Collections.Generic;
using Tables;
using UnityEngine;

public class RewardManager : Singleton<RewardManager>
{
    public ITEM_CATEGORY DetermineItemCategory(Reward rewardTb)
    {
        if (rewardTb.GoodsKey.Length > 0)
            return ITEM_CATEGORY.GOODS;
        if (rewardTb.MaterialKey.Length > 0)
            return ITEM_CATEGORY.MATERIAL;
        if (rewardTb.ItemKey.Length > 0)
            return ITEM_CATEGORY.ITEM;

        return ITEM_CATEGORY.NONE;
    }
    ItemSlotCell SetRewardDetails(Reward rewardTb, ITEM_CATEGORY itemCategory)
    {
        ItemSlotCell cell = new ItemSlotCell();
        switch (itemCategory)
        {
            case ITEM_CATEGORY.GOODS:
                cell.key = rewardTb.GoodsKey[0];
                cell.count = rewardTb.GoodsQty[0];
                cell.itemGrade = Tables.Goods.Get(rewardTb.GoodsKey[0]).Grade;
                break;
            case ITEM_CATEGORY.MATERIAL:
                cell.key = rewardTb.MaterialKey[0];
                cell.count = rewardTb.MaterialQty[0];
                cell.itemGrade = Tables.Material.Get(rewardTb.MaterialKey[0]).Grade;

                break;
            case ITEM_CATEGORY.ITEM:
                cell.key = rewardTb.ItemKey[0];
                cell.count = rewardTb.ItemQty[0];
                cell.itemGrade = Tables.Item.Get(rewardTb.ItemKey[0]).ItemGrade;
                break;
        }

        return cell;
    }
    public void SetRewardDetails(ItemSlot _targetSlot, Reward _reward)
    {

        ITEM_CATEGORY category = DetermineItemCategory(_reward);
        ItemSlotCell cell = SetRewardDetails(_reward, category);
        _targetSlot.UpdateSlot(cell);
    }
}
