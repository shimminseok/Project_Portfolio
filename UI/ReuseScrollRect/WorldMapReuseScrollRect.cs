using System.Collections.Generic;
using System.Linq;
using UI;

public class WorldMapReuseScrollRect : ReuseScrollview<WorldMapSlotCellData>
{
    protected override void Start()
    {
        base.Start();
    }

    public void CreateWorldMapListSlot(int _selectChapter)
    {
        tableData = new List<WorldMapSlotCellData>();
        int index = 0;
        foreach (var slot in Tables.Stage.data.Values.Where(x => x.Chapter == _selectChapter))
        {
            WorldMapSlotCellData cell = new WorldMapSlotCellData();
            cell.Index = index++;
            cell.m_StageTb = slot;
            tableData.Add(cell);
        }

        InitTableView();
    }
}

