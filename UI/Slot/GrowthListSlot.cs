using UI;
using UnityEngine;
using UnityEngine.UI;

public class GrowthListSlot : ReuseCellData<GrowthSlotCellData>
{
    [SerializeField] Image growthImg;
    [SerializeField] Text growthSlotName;
    [SerializeField] Text growthLevel;
    [SerializeField] Image growthCostImg;
    [SerializeField] Text growthCostText;


    int count = 0;

    bool isHodding = false;
    float hoddingTime = 0f;

    Tables.StatReinforce m_StatReinforceTb;
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
        int upgradeMultiple = (AccountManager.Instance.GrowthLevelList[Index] * (AccountManager.Instance.GrowthLevelList[Index] / 3000 + 1));
        uint cost = (uint)(m_StatReinforceTb.Price * upgradeMultiple);
        AccountManager.Instance.UseGoods((GOOD_TYPE)m_StatReinforceTb.PriceType,cost,out bool isEnougt);
        if (!isEnougt)
        {
            ReleaseGrowthLevelUp();
            return;
        }

        isHodding = true;
        count++;
        AccountManager.Instance.GrowthLevelList[Index] += UIGrowth.instance.SelectMultipleNum * count;
        UpdateData();
    }
    public void ReleaseGrowthLevelUp()
    {
        count = 0;
        string saveData = string.Empty;
        for (int i = 0; i < AccountManager.Instance.GrowthLevelList.Count; i++)
        {
            saveData += AccountManager.Instance.GrowthLevelList[i];
            if (i < AccountManager.Instance.GrowthLevelList.Count - 1)
                saveData += ",";
        }
        PlayerPrefs.SetString("GrowthData", saveData);
        isHodding = false;
        hoddingTime = 0f;
        UpdateData();
    }

    public override void UpdateContent(GrowthSlotCellData _itemData)
    {
        Index = _itemData.Index;
        m_StatReinforceTb = Tables.StatReinforce.Get(Index + 1);
        UpdateData();
    }

    public void UpdateData()
    {
        growthImg.sprite = UIManager.Instance.GetSprite(SPRITE_TYPE.GROWTH_ICON, m_StatReinforceTb.Icon);
        growthSlotName.text = string.Format("{0}  Lv.{1}", UIManager.Instance.GetText(m_StatReinforceTb.NameText), AccountManager.Instance.GrowthLevelList[Index]);
        growthLevel.text = string.Format("Lv.{0} ¢º Lv.{1}", AccountManager.Instance.GrowthLevelList[Index], AccountManager.Instance.GrowthLevelList[Index] + UIGrowth.instance.SelectMultipleNum);
        int upgradePrice = (AccountManager.Instance.GrowthLevelList[Index] * (AccountManager.Instance.GrowthLevelList[Index] / 3000 + 1));
        growthCostText.text = string.Format("{0}", AccountManager.Instance.ToCurrencyString(m_StatReinforceTb.Price * upgradePrice));
    }
}
