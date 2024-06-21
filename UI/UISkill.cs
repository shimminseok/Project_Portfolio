using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UISkill : UIPopUp
{
    public static UISkill Instance;

    [Header("SkillList")]
    [SerializeField] SkillListReuseScrollRect skillListReuseScrollRect;
    List<SkillListSlot> skillSlotList = new List<SkillListSlot>();

    [Header("SkillSetting")]
    public List<Image> skillSettingIconList;
    public List<Image> skillSettingLockImgList;
    public List<Image> skillSettingSelectedImgList;
    public Image skillDescSkillIcon;
    public Text skillDescSkillName;
    public Text skillDescSkillLimitLevel;
    public Text skillDescSkillDesc;
    public Text skillSettingSelectedSlotNumTxt;


    int selectSkillNumber = 0;
    int selectEquipSlotNumber;
    bool isOnClickEquipSkill;
    bool isOnClickUnEquipSkill;


    void Awake()
    {
        if (Instance == null)
            Instance = this;
    }
    protected override void Start()
    {
        base.Start();
    }

    public override void ClosePopUp()
    {
        base.ClosePopUp();
    }

    public override void OpenPopUp()
    {
        base.OpenPopUp();

        SelectedSkill(0);
        UnLockSkillIcon();

        isOnClickEquipSkill = false;
        skillListReuseScrollRect.CreateSkillListSlot();
    }



    public void SelectedSkill(int _num)
    {
        selectSkillNumber = _num;
        skillListReuseScrollRect.TableData.ForEach(x => x.isSelected = x.Index == _num);

        foreach (var cell in skillListReuseScrollRect.Cells)
        {
            cell.UpdateContent(skillListReuseScrollRect.TableData[cell.Index]);
        }

    }
    public void GetClickedSkillInfo(Tables.Skill _skillTb)
    {
        SetSkillInfo(_skillTb);
    }
    void SetSkillInfo(Tables.Skill _skillTb)
    {
        skillDescSkillIcon.sprite = UIManager.Instance.GetSprite(SPRITE_TYPE.SKILL_ICON, _skillTb.SkillListIcon);
        skillDescSkillName.text = UIManager.Instance.GetText(_skillTb.SkillName);

        //TO DO
        //스킬 설명
        skillDescSkillDesc.text = UIManager.Instance.GetText(_skillTb.SkillDescription);
    }
    void UnLockSkillIcon()
    {
        for (int i = 0; i < skillSettingLockImgList.Count; i++)
        {
            skillSettingLockImgList[i].gameObject.SetActive(AccountManager.Instance.PlayerLevel < Tables.Define.Get(string.Format("SkillSlotOpen_{0:D2}", i + 1)).value);
        }

        for (int i = 0; i < PlayerController.Instance.SkillInfoList.Length; i++)
        {
            SetSkillSettingIconList(i);
        }
    }
    void SetSkillSettingIconList(int _num)
    {
        if (PlayerController.Instance.SkillInfoList[_num] != null && PlayerController.Instance.SkillInfoList[_num].skillKey != 0)
        {
            skillSettingIconList[_num].enabled = true;
            skillSettingIconList[_num].sprite = UIManager.Instance.GetSprite(SPRITE_TYPE.SKILL_ICON, Tables.Skill.Get(PlayerController.Instance.SkillInfoList[_num].skillKey).SkillListIcon);
        }
        else
        {
            skillSettingIconList[_num].enabled = false;
        }
    }
    #region[Button Action]
    public void OnClickEquipSkill()
    {
        isOnClickEquipSkill = true;
        UIManager.Instance.SetSystemMessage("장착할 스킬의 슬롯을 선택해주세요.", 1f, 0.5f);
    }
    public void OnClickUnEquipSkill()
    {
        if (PlayerController.Instance.SkillInfoList[selectEquipSlotNumber] != null && !PlayerController.Instance.SkillInfoList[selectEquipSlotNumber].IsEmpty)
        {
            PlayerController.Instance.SkillInfoList[selectEquipSlotNumber].UnEquipSkill();
            PlayerController.Instance.SkillCoolTime.Remove(PlayerController.Instance.SkillInfoList[selectEquipSlotNumber].skillKey);
            SetSkillSettingIconList(selectEquipSlotNumber);
            UIManager.Instance.EquipSkill(selectEquipSlotNumber, 0);
        }
    }
    public void OnClickSkillSettingSkillIcon(int _num)
    {
        selectEquipSlotNumber = _num;
        skillSettingSelectedSlotNumTxt.text = string.Format("선택 슬롯 : {0}번", _num + 1);
        if (isOnClickEquipSkill)
        {
            SkillInfo skillInfo = new SkillInfo();
            int key = skillListReuseScrollRect.TableData[selectSkillNumber].m_skill.key;
            int level = 1;
            skillInfo.EquipSkill(key, level);
            PlayerController.Instance.SkillInfoList[_num] = skillInfo;
            PlayerController.Instance.SetSkillCoolDown(key);
            UIManager.Instance.EquipSkill(_num, key);
            isOnClickEquipSkill = false;
            SetSkillSettingIconList(_num);
        }
        else
        {
            isOnClickUnEquipSkill = true;
        }
        for (int i = 0; i < skillSettingSelectedImgList.Count; i++)
        {
            skillSettingSelectedImgList[i].gameObject.SetActive(i == _num);
        }
    }
    #endregion
}
