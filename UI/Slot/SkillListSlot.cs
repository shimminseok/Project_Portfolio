using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillListSlot : MonoBehaviour
{
    int number;
    public EventTrigger eventTrigger;

    public SkillListIcon spriteIcon;

    public Image selectedSlot;
    public Image skillIconImg;
    public Text skillName;
    public Text skillUnLockLevel;

    Tables.Skill m_skillTb;


    public int Number { get => number; set => number = value; }
    public Tables.Skill SkillTb { get => m_skillTb; }

    void Start()
    {
        EventTrigger.TriggerEvent trigger = new EventTrigger.TriggerEvent();
        trigger.AddListener((data) => OnClickSlot());
        eventTrigger.triggers.Add(new EventTrigger.Entry { eventID = EventTriggerType.PointerClick ,callback = trigger});
    }
    public void SetSkillListSlotInfo(Tables.Skill skillTb)
    {
        m_skillTb = skillTb;
        skillIconImg.sprite = spriteIcon.GetSprite(skillTb.SkillListIcon);
        skillName.text = UIManager.Instance.GetText(m_skillTb.SkillName);
        skillUnLockLevel.text = string.Format("해금 레벨 : {0}", m_skillTb.UnLockLevel);
    }


    public void OnClickSlot()
    {
        UISkill.Instance.SelectedSkill(Number);
        UISkill.Instance.GetClickedSkillInfo(m_skillTb);
    }
}
