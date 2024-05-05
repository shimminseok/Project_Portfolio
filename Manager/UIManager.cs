using NPOI.Util.Collections;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
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
    void Start()
    {
        
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

    public void EquipSkill(int _skillkey)
    {
        Tables.Skill skillTb = Tables.Skill.Get(_skillkey);
        if(skillTb != null)
            skillIconImg[UISkill.Instance.selectSkillNumber].sprite = skillIconScripObj.GetSprite(skillTb.SkillIcon);
    }
    #region[Button Event]
    public void OnClickClosePopUp()
    {

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


    public void OnPointerClickDelegate(PointerEventData data)
    {

    }
    #endregion
}
