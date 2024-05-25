using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UI;
using UnityEngine;

public class InventoryReuseScrollRect : ReuseScrollview<InvenSlotCellData>
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }
    public void CreateInventoryListSlot(ITEM_TYPE _type)
    {
        tableData = new List<InvenSlotCellData>();
        int i = 0;
        foreach(var item in Tables.Item.data.Where(x => x.Value.ItemType == (int)_type))
        {
            InvenSlotCellData cell = new InvenSlotCellData();
            cell.Index = i++;
            cell.m_ItemTb = item.Value;
            tableData.Add(cell);
        }
        InitTableView();
    }
}
