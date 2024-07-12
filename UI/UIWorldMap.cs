using NPOI.HSSF.Record.Chart;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Tables;
using Unity.VisualScripting;

public class UIWorldMap : UIPopUp
{
    public static UIWorldMap instance;

    [SerializeField] WorldMapReuseScrollRect worldMapSlotScroll;
    [SerializeField] WorldChapterReuseScrollRect worldMapChapterScroll;
    [SerializeField] WorldMapGenMonsterReuseScrollRect worldMapGenMonsterScroll;
    [SerializeField] WorldMapIdleRewardScrollRect worldMapIdleRewardScroll;
    [SerializeField] WorldMapFirstClearScrollRect worldMapFirstRewardScroll;

    [SerializeField] TextMeshProUGUI totalAtk_Txt;
    [SerializeField] TextMeshProUGUI totalDef_Txt;


    Stage selectStageTb;

    public Stage SelectStageTb => selectStageTb;
    private void Awake()
    {
        if (instance == null)
            instance = this;
    }
    protected override void  Start()
    {
        base.Start();
    }

    public override void OpenPopUp()
    {
        base.OpenPopUp();
        ChangeSelectChapter(MonsterManager.instance.CurrentStageTb.Chapter);
        ChangeSelectStageInfo(MonsterManager.instance.CurrentStageTb);
        totalAtk_Txt.text = Utility.ToCurrencyString(AccountManager.Instance.Total_Atk);
        totalDef_Txt.text = Utility.ToCurrencyString(AccountManager.Instance.Total_Def);
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
            cell.UpdateContent(worldMapChapterScroll.TableData[cell.Index]);
        }
    }
    public void ChangeSelectStageInfo(Stage _stageTb)
    {
        selectStageTb = _stageTb;
        worldMapSlotScroll.TableData.ForEach(x => x.isSelected = x.m_StageTb.key == selectStageTb.key);
        foreach (var cell in worldMapSlotScroll.Cells)
        {
            cell.UpdateContent(worldMapSlotScroll.TableData[cell.Index]);
        }

        worldMapGenMonsterScroll.CreateSlot(selectStageTb);
        worldMapIdleRewardScroll.CreateSlot(selectStageTb);
        worldMapFirstRewardScroll.CreateSlot(selectStageTb);

    }

    public void OnClickStageEnterBtn()
    {
        if (MonsterManager.instance.CurrentStageTb.key == selectStageTb.key)
            return;

        GameManager.Instance.EnterStage(selectStageTb.key);

    }
}
