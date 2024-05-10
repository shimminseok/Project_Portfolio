using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    [Header("ScriptableObj")]
    [SerializeField] SkillIcon skillIconScripObj;

    [Header("BottomAnchor")]
    [SerializeField] Image hpBarImg;

    [Header("RightAnchor")]
    [SerializeField] Image autoButton;
    [SerializeField] List<Image> skillIconImg;
    [SerializeField] List<Image> skillCoolTimeImg;
    [SerializeField] List<Text> skillCoolTimeText;


    public Stack<UIPopUp> openedPopupList = new Stack<UIPopUp>();

    public int SkillSlotCount { get => skillIconImg.Count; }
    void Start()
    {
        for (int i = 0; i < SkillSlotCount; i++)
        {
            EquipSkill(i, PlayerController.Instance.SkillInfoList[i].skillKey);
        }
    }

    void Update()
    {
        for (int i = 0; i < skillCoolTimeImg.Count; i++)
        {
            UpdateSkillCoolTime(i);
        }
    }

    public string GetText(string _key)
    {
        string text = string.Empty;
        return text;
    }


    public void UpdateHPBarUI(float _max, float _cur)
    {
        hpBarImg.fillAmount = _cur / _max;
    }

    public void EquipSkill(int _num, int _skillkey)
    {
        Tables.Skill skillTb = Tables.Skill.Get(_skillkey);
        skillIconImg[_num].enabled = skillTb != null;
        if (skillTb != null)
        {
            skillIconImg[_num].sprite = GetSkillIconImg(skillTb.SkillIcon);
        }
    }

    public Sprite GetSkillIconImg(string _name)
    {
        return skillIconScripObj.GetSprite(_name);
    }
    #region[Button Event]
    public void OnClickOpenPopUp(UIPopUp _popup)
    {
        if(!openedPopupList.Contains(_popup))
        {
            openedPopupList.Push(_popup);
        }
        _popup.OpenPopUp();
    }
    public void OnClickClosePopUp(UIPopUp _popup)
    {
        UIPopUp closePopup = openedPopupList.Pop();
        if (closePopup == _popup)
            closePopup.ClosePopUp();
        else
            _popup.ClosePopUp();

    }


    public void OnClickBossChallenge()
    {

    }
    public void OnClickAuto()
    {
        GameManager.Instance.isAuto = !GameManager.Instance.isAuto;
        autoButton.color = GameManager.Instance.isAuto ? Color.red : Color.white;
    }
    public void OnClickUseSkill(int _num)
    {
        PlayerController.Instance.UseSkill(_num);
    }
    void UpdateSkillCoolTime(int _num)
    {
        if (PlayerController.Instance.SkillInfoList[_num] != null)
        {
            int skillKey = PlayerController.Instance.SkillInfoList[_num].skillKey;
            skillCoolTimeImg[_num].fillAmount = PlayerController.Instance.UpdateSkillCoolTime(skillKey, true);
            skillCoolTimeText[_num].text = string.Format("{0:#.#}", PlayerController.Instance.UpdateSkillCoolTime(skillKey, false));
        }
    }
    #endregion
}
