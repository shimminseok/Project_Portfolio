using System.Collections.Generic;
using UI;

public class WorldMapIdleRewardScrollRect : ReuseScrollview<WorldMapIdleRewardItemCellData>
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    public void CreateSlot(Tables.Stage _stageTb)
    {
        tableData = new List<WorldMapIdleRewardItemCellData>();
        Tables.Reward rewardTb = Tables.Reward.Get(_stageTb.StageIdleReward);
        if (rewardTb != null)
        {
            int index = 0;
            index = AddRewardItems(rewardTb.GoodsKey, rewardTb.GoodsQty, ITEM_CATEGORY.GOODS, index);
            index = AddRewardItems(rewardTb.MaterialKey, rewardTb.MaterialQty, ITEM_CATEGORY.MATERIAL, index);
            index = AddRewardItems(rewardTb.ItemKey, rewardTb.ItemQty, ITEM_CATEGORY.ITEM, index);
        }
        InitTableView();
    }

    int AddRewardItems(int[] _rewardKeys, double[] _rewardQty,ITEM_CATEGORY _category, int _index)
    {
        for (int i = 0; i < _rewardKeys.Length; i++)
        {
            WorldMapIdleRewardItemCellData cellData = new WorldMapIdleRewardItemCellData
            {
                Index = _index++,
                category = _category,
                rewardKey = _rewardKeys[i],
                count = _rewardQty[i]
            };
            tableData.Add(cellData);

        }
        return _index;
    }
}
