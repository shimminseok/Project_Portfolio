using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class GrowthReuseScrollRect : ReuseScrollview<GrowthSlotCellData>
{
    protected override void Start()
    {
        base.Start();
        CreateGrowthListSlot();
    }

    public void CreateGrowthListSlot()
    {
        tableData = new List<GrowthSlotCellData>();
        for (int i = 0; i < Tables.StatReinforce.data.Count; i++) 
        {
            GrowthSlotCellData cell = new GrowthSlotCellData();
            cell.Index = i;
            tableData.Add(cell);

        }
        InitTableView();
    }


}
