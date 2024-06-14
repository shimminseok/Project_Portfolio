using System.Collections;
using System.Collections.Generic;
using Tables;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class WorldMapMonsterSlot : ReuseCellData<WorldMapGenMonsterCellData>
{
    [SerializeField] Image monsterImg;
    [SerializeField] GameObject bossImg;


    Monster m_MonsterTb;
    public override void UpdateContent(WorldMapGenMonsterCellData _itemData)
    {
        m_MonsterTb = _itemData.m_MonsterTb;
        SetSlotInfo();
    }

    void SetSlotInfo()
    {
        monsterImg.sprite = UIManager.Instance.GetSprite(SPRITE_TYPE.MONSTER, m_MonsterTb.Monster_Img);
        bossImg.SetActive(m_MonsterTb.key == UIWorldMap.instance.SelectStageTb.BossIndex);
    }
}
