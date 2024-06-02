using System.Collections.Generic;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class GrowthListSlot : ReuseCellData<GrowthSlotCellData>
{
    [SerializeField] Image growthImg;
    [SerializeField] Image growthCostImg;
    [SerializeField] Text growthSlotName;
    [SerializeField] Text growthLevel;
    [SerializeField] Text growthCostText;

    [Header("Effect")]
    [SerializeField] Image levelUpEffect;
    [SerializeField] GameObject levelUpText;

    int count = 0;

    bool isHodding = false;
    float hoddingTime = 0f;

    Tables.StatReinforce m_StatReinforceTb;

    Coroutine effectCo;
    void Update()
    {
        if (isHodding)
        {
            hoddingTime += Time.deltaTime;
            if (hoddingTime > 0.5f)
            {
                OnClickGrowthLevelUp();
                hoddingTime = 0.45f;
            }
        }
    }
    public void OnClickGrowthLevelUp()
    {

        IAffectedGrowth Igrowth = null;
        switch ((STAT_TARGET_TYPE)m_StatReinforceTb.Target)
        {
            case STAT_TARGET_TYPE.PLAYER:
                Igrowth = PlayerController.Instance.GetComponent<IAffectedGrowth>();
                break;
            case STAT_TARGET_TYPE.COLLEAGUE:
                break;
            case STAT_TARGET_TYPE.ALL:
                break;
        }
        if(Igrowth != null)
        {
            Dictionary<STAT, int> levelList = Igrowth.GrowthLevelDic;

            int upgradeMultiple = (levelList[(STAT)m_StatReinforceTb.key]) * (levelList[(STAT)m_StatReinforceTb.key] / 3000 + 1);
            uint cost = (uint)(m_StatReinforceTb.Price * upgradeMultiple);
            AccountManager.Instance.UseGoods((GOOD_TYPE)m_StatReinforceTb.PriceType, cost, out bool isEnougt);
            if (!isEnougt)
            {
                ReleaseGrowthLevelUp();
                return;
            }

            isHodding = true;
            count++;
            levelList[(STAT)(Index + 1)] += UIGrowth.instance.SelectMultipleNum * count;
            levelUpEffect.gameObject.SetActive(true);
            if (effectCo != null)
                StopCoroutine(effectCo);

            effectCo = StartCoroutine(TweenManager.Instance.TweenAlpha(levelUpEffect.gameObject, 0.8f, 0f, 0.3f));
            UpdateData();
        }
       
    }
    public void ReleaseGrowthLevelUp()
    {
        count = 0;
        isHodding = false;
        hoddingTime = 0f;
        UpdateData();
    }

    public override void UpdateContent(GrowthSlotCellData _itemData)
    {
        Index = _itemData.Index;
        m_StatReinforceTb = Tables.StatReinforce.Get(_itemData.Index + 1);
        UpdateData();
    }

    public void UpdateData()
    {
        IAffectedGrowth Igrowth = null;
        switch ((STAT_TARGET_TYPE)m_StatReinforceTb.Target)
        {
            case STAT_TARGET_TYPE.PLAYER:
                Igrowth = PlayerController.Instance.GetComponent<IAffectedGrowth>();
                break;
            case STAT_TARGET_TYPE.COLLEAGUE:
                break;
            case STAT_TARGET_TYPE.ALL:
                break;
        }
        if(Igrowth != null)
        {
            Dictionary<STAT,int> levelList = Igrowth.GrowthLevelDic;
            growthImg.sprite = UIManager.Instance.GetSprite(SPRITE_TYPE.GROWTH_ICON, m_StatReinforceTb.Icon);
            growthSlotName.text = string.Format("{0}  Lv.{1}", UIManager.Instance.GetText(m_StatReinforceTb.NameText), levelList[(STAT)m_StatReinforceTb.key]);
            growthLevel.text = string.Format("Lv.{0} ¢º Lv.{1}", levelList[(STAT)m_StatReinforceTb.key], levelList[(STAT)m_StatReinforceTb.key] + UIGrowth.instance.SelectMultipleNum);
            int upgradePrice = (levelList[(STAT)m_StatReinforceTb.key] * (levelList[(STAT)m_StatReinforceTb.key] / 3000 + 1));
            growthCostText.text = string.Format("{0}", Utility.ToCurrencyString(m_StatReinforceTb.Price * upgradePrice));
        }

        
    }
}
