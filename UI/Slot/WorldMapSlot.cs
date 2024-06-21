using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WorldMapSlot : ReuseCellData<WorldMapSlotCellData>
{
    [SerializeField] GameObject lockImg;
    [SerializeField] GameObject selectedStageImg;
    [SerializeField] TextMeshProUGUI curStageNumTxt;
    [SerializeField] TextMeshProUGUI curStageName;
    [SerializeField] TextMeshProUGUI recommendAtk;
    [SerializeField] TextMeshProUGUI recommendDef;

    Tables.Stage m_StageTb;


    public override void UpdateContent(WorldMapSlotCellData _itemData)
    {
        m_data = _itemData;
        m_StageTb = _itemData.m_StageTb;

        SetSlot();
    }

    void SetSlot()
    {
        lockImg.gameObject.SetActive(AccountManager.Instance.BestStageInfo.key < m_StageTb.key);
        curStageNumTxt.text = string.Format("{0}-{1}",m_StageTb.Chapter,m_StageTb.Zone);
        curStageName.text = string.Format("{0} {1}", UIManager.Instance.GetText(m_StageTb.StageName),m_StageTb.Zone);
        recommendAtk.text = Utility.ToCurrencyString(m_StageTb.Stage_Recommend_Atk);
        recommendDef.text = Utility.ToCurrencyString(m_StageTb.Stage_Recommend_Def);

        recommendAtk.color = m_StageTb.Stage_Recommend_Atk > AccountManager.Instance.Total_Atk ? Color.red : Color.green;
        recommendDef.color = m_StageTb.Stage_Recommend_Def > AccountManager.Instance.Total_Def ? Color.red : Color.green;

        selectedStageImg.SetActive(m_data.isSelected);
    }

    public void OnClickWorldMapSlot()
    {
        UIWorldMap.instance.ChangeSelectStageInfo(m_StageTb);
    }
}
