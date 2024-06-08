using NPOI.HSSF.Record.Chart;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIWorldMap : UIPopUp
{
    public static UIWorldMap instance;

    [SerializeField] WorldMapReuseScrollRect worldMapSlotScroll;
    [SerializeField] WorldChapterReuseScrollRect worldMapChapterScroll;
    

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }
    void Start()
    {
        ChildSetActive(false);
    }

    public override void OpenPopUp()
    {
        base.OpenPopUp();
        ChangeSelectChapter(MonsterManager.instance.currentStageTb.Chapter);
    }
    public override void ClosePopUp()
    {
        base.ClosePopUp();
    }

    public void ChangeSelectChapter(int _selectChapter)
    {
        worldMapSlotScroll.CreateWorldMapListSlot(_selectChapter);
        worldMapChapterScroll.TableData.ForEach(x => x.isSelected = _selectChapter == x.chapter);
        foreach(var cell in worldMapChapterScroll.Cells)
        {
            cell.UpdateContent(cell.m_data);
        }

    }
}
