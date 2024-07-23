using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class UISkill : UIPopUp
{
    public static UISkill Instance;

    [Header("SkillList")]
    [SerializeField] SkillListReuseScrollRect skillListReuseScrollRect;
    List<SkillListSlot> skillSlotList = new List<SkillListSlot>();

    [Header("SkillSetting")]
    [SerializeField] List<Image> skillSettingSkillIconFrameImgs;
    [SerializeField] List<Image> skillSettingIconList;
    [SerializeField] List<Image> skillSettingLockImgList;
    [SerializeField] List<Image> skillSettingSelectedImgList;
    [SerializeField] Image skillDescSkillIcon;
    [SerializeField] Text skillDescSkillName;
    [SerializeField] Text skillDescSkillLimitLevel;
    [SerializeField] Text skillDescSkillDesc;
    [SerializeField] Text skillSettingSelectedSlotNumTxt;

    [Header("SkillDetailInfo")]
    [SerializeField] GameObject skillDetailPopupObj;
    [SerializeField] ItemSlot skillDetail_Skill_Slot;
    [SerializeField] Image skillDetail_SkillRemainCnt_Fill;
    [SerializeField] TextMeshProUGUI skillDetail_Skill_Name;
    [SerializeField] TextMeshProUGUI skillDetail_Skill_Level;
    [SerializeField] TextMeshProUGUI skillDetail_DemainPiece_Cnt;
    [SerializeField] TextMeshProUGUI skillDetail_CurrentLv_SkillDesc;
    [SerializeField] TextMeshProUGUI skillDetail_NextLv_SkillDesc;



    int selectSkillNumber = 0;
    int selectEquipSlotNumber;
    bool isOnClickEquipSkill;
    bool isOnClickUnEquipSkill;


    bool isOpenSkillDetailPopUp;

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
        if (isOpenSkillDetailPopUp)
        {
            CloseSkillDetailUI();
            UIManager.Instance.OnClickOpenPopUp(this);
            return;
        }

    }

    public override void OpenPopUp()
    {
        base.OpenPopUp();
        UnLockSkillIcon();
        CloseSkillDetailUI();
        isOnClickEquipSkill = false;
        skillListReuseScrollRect.CreateSkillListSlot();
    }



    public void SelectedSkill(int _num)
    {
        selectSkillNumber = _num;
        skillListReuseScrollRect.TableData.ForEach(x => x.isSelected = x.Index == _num);


        SetSkillDetailInfo(skillListReuseScrollRect.TableData[_num].m_skill);
        skillListReuseScrollRect.UpdateAllCells();

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
        skillDescSkillDesc.text = GetSkillDesc(_skillTb.key);
    }
    string GetSkillDesc(int _key, bool _isNextLv = false)
    {
        if (!AccountManager.Instance.HasSkillDictionary.TryGetValue(_key, out var skillDictionary))
        {
            skillDictionary = new SkillItem()
            {
                key = _key,
                m_Table = Tables.Skill.Get(_key)
            };
        }
        return skillDictionary.GetSkillDesc(skillDictionary, _isNextLv);
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
        SkillItem skillInfo = PlayerController.Instance.SkillInfoList[_num];
        bool hasSkill = skillInfo != null && skillInfo.key != 0;

        if (hasSkill)
        {
            Tables.Skill skill = Tables.Skill.Get(skillInfo.key);
            skillSettingIconList[_num].enabled = true;
            skillSettingIconList[_num].sprite = UIManager.Instance.GetSprite(SPRITE_TYPE.SKILL_ICON, skill.SkillListIcon);
            skillSettingSkillIconFrameImgs[_num].sprite = UIManager.Instance.GetSprite(SPRITE_TYPE.SKILL_ICON, $"skilluse_a_bg00{skill.SkillGrade}");
        }
        else
        {
            skillSettingIconList[_num].enabled = false;
            skillSettingSkillIconFrameImgs[_num].sprite = UIManager.Instance.GetSprite(SPRITE_TYPE.SKILL_ICON, "skilluse_a_bg001");
        }
    }
    #region[SkillDetailUI]

    public void SetSkillDetailInfo(Tables.Skill _skillTb)
    {
        if(!AccountManager.Instance.HasSkillDictionary.TryGetValue(_skillTb.key, out SkillItem skillInfo))
        {
            skillInfo = new SkillItem
            {
                key = _skillTb.key,
                m_Table = _skillTb
            };
        }
        skillDetail_Skill_Slot.UpdateSlot(skillInfo);

        //각성에 요구되는 갯수는 각성 * 10;
        skillDetail_Skill_Name.text = UIManager.Instance.GetText(skillInfo.m_Table.SkillName);
        skillDetail_Skill_Level.text = $"Lv.{skillInfo.enhanceCount}";
        skillDetail_DemainPiece_Cnt.text = $"{skillInfo.count} / {(skillInfo.skillAwake + 1) * 10}";
        skillDetail_SkillRemainCnt_Fill.fillAmount = (float)skillInfo.count / ((skillInfo.skillAwake + 1) * 10);



        skillDetail_CurrentLv_SkillDesc.text = skillInfo.GetSkillDesc(skillInfo);
        skillDetail_NextLv_SkillDesc.text = skillInfo.GetSkillDesc(skillInfo, true);


        OpenSkillDetailUI();
    }
    void OpenSkillDetailUI()
    {
        isOpenSkillDetailPopUp = true;
        skillDetailPopupObj.SetActive(true);
    }

    void CloseSkillDetailUI()
    {
        isOpenSkillDetailPopUp = false;
        skillDetailPopupObj.SetActive(false);
    }
    #endregion


    #region[Button Action]
    public void OnClickEquipSkill()
    {
        isOnClickEquipSkill = true;
        UISystem.instance.SetSystemMessage("장착할 스킬의 슬롯을 선택해주세요.");
    }
    public void OnClickUnEquipSkill()
    {
        if (PlayerController.Instance.SkillInfoList[selectEquipSlotNumber] != null && !PlayerController.Instance.SkillInfoList[selectEquipSlotNumber].IsEmpty)
        {
            PlayerController.Instance.SkillInfoList[selectEquipSlotNumber].UnEquipSkill();
            PlayerController.Instance.SkillCoolTime.Remove(PlayerController.Instance.SkillInfoList[selectEquipSlotNumber].key);
            SetSkillSettingIconList(selectEquipSlotNumber);
            UIManager.Instance.EquipSkill(selectEquipSlotNumber, PlayerController.Instance.SkillInfoList[selectEquipSlotNumber]);
        }
    }
    public void OnClickSkillSettingSkillIcon(int _num)
    {
        selectEquipSlotNumber = _num;
        skillSettingSelectedSlotNumTxt.text = string.Format("선택 슬롯 : {0}번", _num + 1);
        if (isOnClickEquipSkill)
        {
            SkillItem skillInfo = new SkillItem();
            int key = skillListReuseScrollRect.TableData[selectSkillNumber].m_skill.key;
            int level = 1;
            skillInfo.EquipSkill(key, level);
            PlayerController.Instance.SkillInfoList[_num] = skillInfo;
            PlayerController.Instance.SetSkillCoolDown(key);
            UIManager.Instance.EquipSkill(_num, skillInfo);
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

    public void OnClickOpenDetailSkillInfoUI()
    {
        isOpenSkillDetailPopUp = true;
    }

    public void OnClickSkillLevelUpBtn()
    {

    }
    public void OnClickSkillAwakeBtn()
    {

    }
    #endregion
}
