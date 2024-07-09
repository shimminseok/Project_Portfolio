using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI;
using Tables;

public class CharInfo_AbilityScrollRect : ReuseScrollview<AbilitySlotCell>
{


    protected override void Start()
    {
        base.Start();
    }

    public void CreateAbilitySlot()
    {
        tableData = new List<AbilitySlotCell>();
        int index = 0;
        foreach (var  cell in Ability.data.Values)
        {
            AbilitySlotCell cellData = new AbilitySlotCell();
            cellData.abilityTb = cell;
            cellData.Index = index++;
            tableData.Add(cellData);
        }
        InitTableView();
    }
}
