using System.Collections;
using System.Collections.Generic;
using Tables;
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
    void Update()
    {
        if (isHodding)
        {
            hoddingTime += Time.deltaTime;
            if(hoddingTime > 0.5f)
            {
                OnClickGrowthLevelUp();
                hoddingTime = 0.45f;
            }
        }
    }
    public void SetSlotInfo()
    {
    }
    public void OnClickGrowthLevelUp()
    {
        isHodding = true;
        count++;
        int levelUpMultiple = 1;
        switch (UIGrowth.instance.SelectMultipleNum)
        {
            case 0:
                levelUpMultiple *= 1;
                break;
            case 1:
                levelUpMultiple *= 10;
                break;
            case 2:
                levelUpMultiple *= 100;
                break;
            case 3:
                levelUpMultiple *= 1000;
                break;
            default:
                break;
        }
        AccountManager.Instance.GrowthLevelList[Index] += levelUpMultiple * count;
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
        PlayerPrefs.SetString("GrowthData",saveData);
        isHodding = false;
        hoddingTime = 0f;
        UpdateData();
    }

    public override void UpdateContent(GrowthSlotCellData _itemData)
    {
        Index = _itemData.Index;
        UpdateData();
    }

    void UpdateData()
    {
        Tables.Ability abilityTb = Tables.Ability.Get(Index + 1);
        if(abilityTb != null)
        {
            growthSlotName.text = string.Format("{0}  Lv.{1}", abilityTb.AbilityName, AccountManager.Instance.GrowthLevelList[Index]);
            growthLevel.text = string.Format("Lv.{0} ▶ Lv.{1}", AccountManager.Instance.GrowthLevelList[Index], AccountManager.Instance.GrowthLevelList[Index] + 1);
            growthCostText.text = "아직 안정함";
        }
    }
}
