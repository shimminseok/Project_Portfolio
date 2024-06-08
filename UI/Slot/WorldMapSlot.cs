using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WorldMapSlot : ReuseCellData<WorldMapSlotCellData>
{
    [SerializeField] TextMeshProUGUI curStageNumTxt;
    [SerializeField] TextMeshProUGUI curStageName;
    [SerializeField] TextMeshProUGUI recommendAtk;
    [SerializeField] TextMeshProUGUI recommendDef;

    Tables.Stage m_StageTb;


    public override void UpdateContent(WorldMapSlotCellData _itemData)
    {
        m_StageTb = _itemData.m_StageTb;
        SetSlot();
    }

    void SetSlot()
    {
        curStageNumTxt.text = string.Format("{0}-{1}",m_StageTb.Chapter,m_StageTb.Zone);
        curStageName.text = string.Format("{0}", UIManager.Instance.GetText(m_StageTb.StageName));
        recommendAtk.text = Utility.ToCurrencyString(100);
        recommendDef.text = Utility.ToCurrencyString(100);
    }
}
