using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using UI;

public class SkillListSlot : ReuseCellData<SkillListCellData>
{
    [SerializeField] GameObject selectedSlot;
    [SerializeField] GameObject getSkillBG;
    [SerializeField] Image skillIconFrame;
    [SerializeField] Image skillIconImg;
    [SerializeField] TextMeshProUGUI skillName;
    [SerializeField] TextMeshProUGUI skillCoolTime;

    Tables.Skill m_skillTb;
    public override void UpdateContent(SkillListCellData _itemData)
    {
        m_data = _itemData;
        SetSkillListSlotInfo(_itemData.m_skill);
        selectedSlot.SetActive(_itemData.isSelected);

    }

    public void SetSkillListSlotInfo(Tables.Skill skillTb)
    {
        m_skillTb = skillTb;
        skillName.text = UIManager.Instance.GetText(m_skillTb.SkillName);
        skillIconImg.sprite = UIManager.Instance.GetSprite(SPRITE_TYPE.SKILL_ICON,skillTb.SkillListIcon);
        skillCoolTime.text = $"{skillTb.CoolTime}√ ";
        getSkillBG.SetActive(AccountManager.Instance.IsGetSkill(m_skillTb.key));
        skillIconFrame.sprite = UIManager.Instance.GetSprite(SPRITE_TYPE.SKILL_ICON, $"skilluse_a_bg00{skillTb.SkillGrade}");
    }


    public void OnClickSlot()
    {
        UISkill.Instance.SelectedSkill(Index);
        UISkill.Instance.GetClickedSkillInfo(m_skillTb);
    }


}
