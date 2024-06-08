using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WorldMapChapterSlot : ReuseCellData<WorldMapChapterCellData>
{
    [SerializeField] TextMeshProUGUI chapterNum;
    [SerializeField] Image selectImg;
    public override void UpdateContent(WorldMapChapterCellData _itemData)
    {
        m_data = _itemData;
        SetSlot(_itemData.chapter);
    }


    void SetSlot(int _chapter)
    {
        chapterNum.text = _chapter.ToString();
        selectImg.enabled = m_data.isSelected;
    }

    public void OnClickChapter()
    {
        if(!m_data.isSelected)
        {
            UIWorldMap.instance.ChangeSelectChapter(m_data.chapter);
            selectImg.enabled = m_data.isSelected;
        }
    }
}
