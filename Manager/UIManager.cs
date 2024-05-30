using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    [Header("ScriptableObj")]
    [SerializeField] List<SpriteScriptableObject> spriteScripObjList = new List<SpriteScriptableObject>();

    [Header("FullPopUp")]
    public UIFullPopUp fullPopUp;

    [Header("Tag")]
    public Canvas tagCanvas;

    [Header("SystemMessage")]
    [SerializeField] Image systemMessageObj;
    [SerializeField] Text systemMessageTxt;

    [Header("RightTopAnchor")]
    [SerializeField] List<Text> goodsTextList;
    [Header("BottomAnchor")]
    [SerializeField] Image hpBarImg;

    [Header("RightAnchor")]
    [SerializeField] Image autoButton;
    [SerializeField] List<Image> skillIconImg;
    [SerializeField] List<Image> skillCoolTimeImg;
    [SerializeField] List<Text> skillCoolTimeText;


    public Stack<UIPopUp> openedPopupList = new Stack<UIPopUp>();

    public FULL_POPUP_TYPE popupType = FULL_POPUP_TYPE.NONE;
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

        if (Input.GetKeyDown(KeyCode.Escape) && openedPopupList.Count > 0)
        {
            OnClickClosePopUp(openedPopupList.Peek());
        }

        for (int i = 0; i < skillCoolTimeImg.Count; i++)
        {
            UpdateSkillCoolTime(i);
        }
    }

    public void UpdateGoodText(GOOD_TYPE _type, double _amount)
    {
        if(popupType != FULL_POPUP_TYPE.NONE)
        {
            fullPopUp.UpdateFullPopUPGoodsBox(_type, _amount);
        }
        else
            goodsTextList[(int)(_type - 1)].text = AccountManager.Instance.ToCurrencyString(_amount);
    }

    public void UpdateHPBarUI(double _max, double _cur)
    {
        float pevAmount = hpBarImg.fillAmount;
        TweenManager.Instance.TweenFill(hpBarImg, pevAmount, (float)(_cur / _max));
    }

    public void EquipSkill(int _num, int _skillkey)
    {
        Tables.Skill skillTb = Tables.Skill.Get(_skillkey);
        skillIconImg[_num].enabled = skillTb != null;
        if (skillTb != null)
        {
            skillIconImg[_num].sprite = GetSprite(SPRITE_TYPE.SKILL_ICON, skillTb.SkillIcon);
        }
    }
    public string GetText(string _key)
    {
        string text = Tables.TextKey.Get(_key).Description;
        return text;
    }
    public Sprite GetSprite(SPRITE_TYPE _type, string _name)
    {
        return spriteScripObjList[(int)_type].GetSprite(_name);
    }
    public void SetSystemMessage(string _message, float _time, float _delay = 0, UnityAction _action = null)
    {
        systemMessageTxt.text = _message;
        StartCoroutine(TweenManager.Instance.FadeIn(systemMessageObj.gameObject, 1, 0, 0, () =>
        {
            StartCoroutine(TweenManager.Instance.FadeOut(systemMessageObj.gameObject, 0, _time, _delay, _action));
        }));
    }
    #region[Button Event]
    public void OnClickOpenPopUp(UIPopUp _popup)
    {
        if (!openedPopupList.Contains(_popup))
        {
            openedPopupList.Push(_popup);
        }
        _popup.OpenPopUp();
        if(popupType != FULL_POPUP_TYPE.NONE)
        {
            fullPopUp.ChildSetActive(true);
            fullPopUp.SettingUI();
        }
    }
    public void CloseAllPopUp()
    {
        while (openedPopupList.Count > 0)
        {
            openedPopupList.TryPop(out UIPopUp tmp);
            tmp.ClosePopUp();
        }
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
        if (PlayerController.Instance.IsUseableSkill(_num))
            PlayerController.Instance.UseSkill(_num);
    }
    void UpdateSkillCoolTime(int _num)
    {
        if (PlayerController.Instance.SkillInfoList[_num] != null)
        {
            int skillKey = PlayerController.Instance.SkillInfoList[_num].skillKey;
            skillCoolTimeImg[_num].fillAmount = PlayerController.Instance.UpdateSkillCoolTime(skillKey, true);
            float skillCoolTime = PlayerController.Instance.UpdateSkillCoolTime(skillKey, false);
            if (skillCoolTimeText[_num].enabled)
            {
                skillCoolTimeText[_num].text = string.Format("{0:0.#}", skillCoolTime);
            }
            skillCoolTimeText[_num].enabled = skillCoolTime > 0;
        }
    }


    public void CheatAddGold()
    {
        AccountManager.Instance.AddGoods(GOOD_TYPE.GOLD, 100000000);

    }
    public void CheatAddDia()
    {
        AccountManager.Instance.AddGoods(GOOD_TYPE.DIA, 100000);
    }
    #endregion
}
