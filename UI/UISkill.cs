using System;
using UnityEngine;

public class UISkill : UIPopUp
{
    public static UISkill Instance;

    [SerializeField] GameObject skillListSlotPrefab;
    [SerializeField] Transform skillListGrid;
    public int selectSkillNumber;


    void Awake()
    {
        if (Instance == null)
            Instance = this;
    }
    void Start()
    {
        foreach(Tables.Skill skillTb in Tables.Skill.data.Values)
        {
            GameObject go = Instantiate(skillListSlotPrefab, skillListGrid);
            SkillListSlot skillListSlot = go.GetComponent<SkillListSlot>();
            if(skillListSlot != null)
            {
                skillListSlot.SetSkillListSlotInfo(skillTb);
            }
        }
    }

    public void EquipSkill(Tables.Skill _skillTb, bool _isEquip)
    {
        SkillInfo skillInfo = new SkillInfo();
        if (_isEquip)
        {
            int key = _skillTb.key;
            int level = 1;
            skillInfo.EquipSkill(key, level);
            PlayerController.Instance.SkillInfoList[selectSkillNumber] = skillInfo;
            PlayerController.Instance.SetSkillCoolDown(key);
            UIManager.Instance.EquipSkill(key);
        }
        else
        {
            PlayerController.Instance.SkillCoolTime.Remove(PlayerController.Instance.SkillInfoList[selectSkillNumber].skillKey);
            PlayerController.Instance.SkillInfoList[selectSkillNumber].UnEquipSkill();
        }
    }

    public override void ClosePopUp()
    {
        if (UIManager.Instance.openedPopupList.Count > 0)
            UIManager.Instance.openedPopupList.Pop().OpenPopUp();

        ChildSetActive(false);
    }

    public override void OpenPopUp()
    {
        UIManager.Instance.openedPopupList.Push(this);
        ChildSetActive(true);
    }


    public void GetClickedSkillInfo(Tables.Skill _skillTb)
    {
        EquipSkill(_skillTb,true);
    }
}
