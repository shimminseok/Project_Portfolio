using System.Collections.Generic;
using Tables;
using TMPro;
using UnityEngine;
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
    [SerializeField] Image loadingBox;
    [SerializeField] Image systemMessageObj;

    [Header("LeftAnchor")]
    [SerializeField] TextMeshProUGUI frameTxt;
    float deltaTime = 0f;

    [Header("TopAnchor")]
    [SerializeField] TextMeshProUGUI waveProcessText;
    [SerializeField] TextMeshProUGUI currentStageName;
    [SerializeField] Image processFillImg;
    [SerializeField] GameObject BossChallengeBtn;

    [Header("RightTopAnchor")]
    [SerializeField] List<Text> goodsTextList;
    [SerializeField] QuestObjerverSlot questObjerverSlot;


    [Header("BottomAnchor")]
    [SerializeField] Image hpBarImg;

    [Header("RightAnchor")]
    [SerializeField] Image autoButton;
    [SerializeField] List<Image> skillIconImg;
    [SerializeField] List<Image> skillCoolTimeImg;
    [SerializeField] List<Text> skillCoolTimeText;

    [Header("MainMenuNoti")]
    [SerializeField] GameObject questNoti;


    public Stack<UIPopUp> openedPopupList = new Stack<UIPopUp>();

    public FULL_POPUP_TYPE popupType = FULL_POPUP_TYPE.NONE;
    public int SkillSlotCount { get => skillIconImg.Count; }

    public QuestObjerverSlot ObjerverSlot => questObjerverSlot;

    void Start()
    {
        InitUI();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && openedPopupList.Count > 0)
        {
            OnClickClosePopUp(openedPopupList.Peek());
        }
        if (Input.GetKeyDown(KeyCode.F10))
        {
            Debug.LogWarning("Reset Account");
            PlayerPrefs.DeleteAll();
        }
        for (int i = 0; i < skillCoolTimeImg.Count; i++)
        {
            UpdateSkillCoolTime(i);
        }
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;

        float fps = 1f / deltaTime;
        frameTxt.text = string.Format("{0:0.}FPS", fps);

    }

    void InitUI()
    {
        UpdateGoodText(GOOD_TYPE.GOLD, AccountManager.Instance.Gold);
        UpdateGoodText(GOOD_TYPE.DIA, AccountManager.Instance.Dia);
        SetStageName(AccountManager.Instance.CurrentStageInfo.key);
        OnClickAuto();
    }
    public void UpdateGoodText(GOOD_TYPE _type, double _amount)
    {
        if (popupType != FULL_POPUP_TYPE.NONE)
            fullPopUp.UpdateFullPopUPGoodsBox(_type, _amount);


        goodsTextList[(int)(_type - 1)].text = Utility.ToCurrencyString(_amount);
    }

    public void EquipSkill(int _num, SkillItem _skill)
    {
        skillIconImg[_num].enabled = _skill.key != 0;
        skillIconImg[_num].sprite = _skill.GetSprite();
    }
    public string GetText(string _key)
    {
        string text = string.Empty;
        Tables.TextKey textTb = Tables.TextKey.Get(_key);
        if (textTb != null)
            text = Tables.TextKey.Get(_key).Description;

        return text;
    }
    public Sprite GetSprite(SPRITE_TYPE _type, string _name)
    {
        return spriteScripObjList[(int)_type].GetSprite(_name);
    }

    public void LoadingUISet()
    {
        loadingBox.raycastTarget = true;
        StartCoroutine(TweenManager.Instance.FadeIn(loadingBox.gameObject, 1, 0.1f, 0, () =>
        {
            StartCoroutine(TweenManager.Instance.FadeOut(loadingBox.gameObject, 0, 2f, 0.5f, () =>
            {
                loadingBox.raycastTarget = false;
                GameManager.instance.ChangeGameState(GAME_STATE.PLAYING);
                MonsterManager.instance.MonsterRegen();
            }));
        }));
    }
    public void SetStageName(int stageKey)
    {
        Stage stageTb = Stage.Get(stageKey);
        if (stageTb != null)
        {
            currentStageName.text = string.Format("{0}-{1} {2}", stageTb.Chapter, stageTb.Zone, GetText(stageTb.StageName));
        }
    }
    public string ReturnCurrentStageName()
    {
        return currentStageName.text;
    }
    public void SetWaveProsessUI()
    {
        float fillAmount = 0f;

        bool isChallengeableBoss = AccountManager.Instance.CurrentStageInfo.isChallengeableBoss;
        BossChallengeBtn.SetActive(isChallengeableBoss);
        if (isChallengeableBoss)
        {
            fillAmount = 1f;
            waveProcessText.text = "보스 도전";
        }
        else
        {
            int stageStep = MonsterManager.instance.StageStep;
            int genMonsterStep = MonsterManager.instance.GenMonsterStep;

            fillAmount = (float)stageStep / genMonsterStep;
            waveProcessText.text = $"웨이브 : {stageStep} / {genMonsterStep}";
        }

        processFillImg.fillAmount = fillAmount;
    }
    #region[Button Event]
    public void OnClickOpenPopUp(UIPopUp _popup)
    {
        foreach (var item in openedPopupList)
        {
            item.ClosePopUp();
        }
        if (!openedPopupList.Contains(_popup))
        {
            openedPopupList.Push(_popup);
        }
        _popup.OpenPopUp();
        if (popupType != FULL_POPUP_TYPE.NONE)
        {
            fullPopUp.ChildSetActive(true);
            fullPopUp.SettingUI();
            SoundManager.Instance.MuteEffectSount(true);
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
        MonsterManager.instance.ChallengeBoss();
        BossChallengeBtn.SetActive(false);
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
        var skillInfo = PlayerController.Instance.SkillInfoList[_num];
        bool hasSkill = skillInfo != null;
        skillCoolTimeImg[_num].gameObject.SetActive(hasSkill);

        if (hasSkill)
        {
            int skillKey = skillInfo.key;
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
