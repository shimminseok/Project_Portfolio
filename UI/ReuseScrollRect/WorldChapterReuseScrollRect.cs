using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class WorldChapterReuseScrollRect : ReuseScrollview<WorldMapChapterCellData>
{

    void Awake()
    {
        CreateWorldMapChapterListSlot();
    }
    protected override void Start()
    {
        base.Start();
    }

    public void CreateWorldMapChapterListSlot()
    {
        int index = 0;
        int prevChapter = 0;
        foreach(var data in Tables.Stage.data)
        {
            if (prevChapter != data.Value.Chapter)
            {
                prevChapter = data.Value.Chapter;
                WorldMapChapterCellData cell = new WorldMapChapterCellData();
                cell.Index = index++;
                cell.chapter = index;
                tableData.Add(cell);
            }
        }
        InitTableView();
    }
}
