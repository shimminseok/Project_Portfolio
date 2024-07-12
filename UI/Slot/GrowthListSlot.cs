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
        IAffectedGrowth iGrowth = GetAffectedGrowth((STAT_TARGET_TYPE)m_StatReinforceTb.Target);
        if (iGrowth == null) 
            return;

        Dictionary<STAT, int> levelList = iGrowth.GrowthLevelDic;
        double upgradePrice = CalculateUpgradePrice(levelList);

        AccountManager.Instance.UseGoods((GOOD_TYPE)m_StatReinforceTb.PriceType, upgradePrice, out bool isEnough);
        if (!isEnough)
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
    public void ReleaseGrowthLevelUp()
    {
        UIQuest.instance.IncreaseQuestCount(QUEST_CARTEGORY.UPGRADE_GROWTH, count);
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

        IAffectedGrowth iGrowth = GetAffectedGrowth((STAT_TARGET_TYPE) m_StatReinforceTb.Target);
        if (iGrowth == null) 
            return;

        Dictionary<STAT, int> levelList = iGrowth.GrowthLevelDic;
        int currentLevel = levelList[(STAT)m_StatReinforceTb.key];
        int nextLevel = currentLevel + UIGrowth.instance.SelectMultipleNum;

        growthImg.sprite = UIManager.Instance.GetSprite(SPRITE_TYPE.GROWTH_ICON, m_StatReinforceTb.Icon);
        growthSlotName.text = $"{UIManager.Instance.GetText(m_StatReinforceTb.NameText)}  Lv.{currentLevel}";
        growthLevel.text = $"Lv.{currentLevel} ¢º Lv.{nextLevel}";

        double upgradePrice = CalculateUpgradePrice(levelList);
        growthCostText.color = IsEnoughCost(upgradePrice) ? Color.white : Color.red;
        growthCostText.text = Utility.ToCurrencyString(upgradePrice);
    }

    IAffectedGrowth GetAffectedGrowth(STAT_TARGET_TYPE targetType)
    {
        return targetType switch
        {
            STAT_TARGET_TYPE.PLAYER => PlayerController.Instance.GetComponent<IAffectedGrowth>(),
            STAT_TARGET_TYPE.COLLEAGUE => null,
            STAT_TARGET_TYPE.ALL => null,
            _ => null
        };
    }

    double CalculateUpgradePrice(Dictionary<STAT, int> levelList)
    {
        int currentLevel = levelList[(STAT)m_StatReinforceTb.key];
        int nextLevel = currentLevel + UIGrowth.instance.SelectMultipleNum;
        int levelDivisionFactor = nextLevel / 3000 + 1;
        return m_StatReinforceTb.Price * nextLevel * levelDivisionFactor;
    }

    private bool IsEnoughCost(double cost)
    {
        return AccountManager.Instance.Gold >= cost;
    }
}
