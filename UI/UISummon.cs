using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Tables;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UISummon : UIPopUp
{
    public static UISummon instance;

    [SerializeField] List<Toggle> summonTypeToggles = new List<Toggle>();
    [SerializeField] Image summonLevelFillImg;
    [SerializeField] TextMeshProUGUI summonLevelText;
    [SerializeField] TextMeshProUGUI summonExpText;
    [SerializeField] ItemSlot summonLevelRewardItemSlot;


    [Header("ResultItemPopup")]
    [SerializeField] GameObject resultItemPopup;
    [SerializeField] ItemSlot resultSlotBase;
    [SerializeField] Transform resultSlotRoot;
    [SerializeField] TextMeshProUGUI screenTouchText;
    SUMMON_TYPE selectSummonType;


    [SerializeField] float delayTime = 0.2f;


    Coroutine summonResultCo;
    bool isPlayingSummonResultCo;
    Queue<InvenItemInfo> summonResultItemQueue = new Queue<InvenItemInfo>();
    void Awake()
    {
        if (instance == null)
            instance = this;
    }
    void Start()
    {
        selectSummonType = SUMMON_TYPE.WEAPONE;
        ChildSetActive(false);
    }
    public override void OpenPopUp()
    {
        base.OpenPopUp();
        OnClickSummonTab(summonTypeToggles[0].gameObject);
        UIManager.Instance.popupType = FULL_POPUP_TYPE.SUMMON;
    }

    public override void ClosePopUp()
    {
        base.ClosePopUp();
        CloseSummonResultPopup();
    }

    public void OnClickSummonBtn(int _count)
    {
        AccountManager.Instance.UseGoods(GOOD_TYPE.DIA, (ulong)(_count * 100), out bool isEnough);
        if (isEnough)
            Summon(_count);
    }
    public void OnClickSummonTab(GameObject _go)
    {
        int onToggleIndex = summonTypeToggles.FindIndex(x => x.name == _go.name);
        selectSummonType = (SUMMON_TYPE)onToggleIndex;
        UpdateSummonLevel();
    }

    void Summon(int _count)
    {

        if (isPlayingSummonResultCo)
        {
            SkipSummonResult();
        }
        else
        {
            for (int i = 0; i < resultSlotRoot.childCount; i++)
            {
                Destroy(resultSlotRoot.GetChild(i).gameObject);
            }
            summonResultItemQueue.Clear();
            for (int i = 0; i < _count; i++)
            {
                List<Tables.Item> itemList = GetSummonItemList();
                AccountManager.Instance.SummonCountUp(selectSummonType);
                InvenItemInfo summonedItem = SummonRandomItem(itemList);

                InvenItemInfo resultItem = summonResultItemQueue.FirstOrDefault(x => x.key == summonedItem.key);
                if (resultItem == null)
                {
                    summonResultItemQueue.Enqueue(summonedItem);
                }
                else
                {
                    resultItem.count++;
                }
            }

            summonResultCo = StartCoroutine(SummonResult(summonResultItemQueue));
        }
        
    }

    List<Tables.Item> GetSummonItemList()
    {
        int key = 10000 + (1000 * (int)selectSummonType) + AccountManager.Instance.GetSummonLevel(selectSummonType);
        Tables.Summon summonTb = Tables.Summon.Get(key);
        float randomValue = Random.Range(0, 100);
        float rate = 0;

        for (int j = 0; j < summonTb.ItemRate.Length; j++)
        {
            rate += summonTb.ItemRate[j];
            if (randomValue < rate)
            {
                switch (selectSummonType)
                {
                    case SUMMON_TYPE.WEAPONE:
                        return Tables.Item.data.Values.Where(x => x.ItemGrade == j + 1 && x.isSummon == 1 && x.ItemType == (int)ITEM_TYPE.WEAPONE).ToList();
                    case SUMMON_TYPE.ARMOR:
                        return Tables.Item.data.Values.Where(x => x.ItemGrade == j + 1 && x.isSummon == 1 && x.ItemType >= (int)ITEM_TYPE.ARMOR && x.ItemType <= (int)ITEM_TYPE.SHOES).ToList();
                    case SUMMON_TYPE.ACC:
                        return Tables.Item.data.Values.Where(x => x.ItemGrade == j + 1 && x.isSummon == 1 && x.ItemType >= (int)ITEM_TYPE.ACC_1 && x.ItemType <= (int)ITEM_TYPE.ACC_2).ToList();
                }
            }
        }
        return new List<Tables.Item>();
    }

    InvenItemInfo SummonRandomItem(List<Tables.Item> itemList)
    {
        while (true)
        {
            int randomIndex = Random.Range(0, itemList.Count);
            string qualityKey = $"Summons_Quality_Grade_{itemList[randomIndex].Quality_Grade}";
            Tables.Define defineTb = Tables.Define.Get(qualityKey);
            if (defineTb == null)
            {
                Debug.LogWarningFormat("DefineTb is Null Key : {0}", qualityKey);
            }
            if (Random.Range(0f, 100f) < defineTb.value)
            {
                var selectedItem = itemList[randomIndex];
                InvenItemInfo itemInfo = new InvenItemInfo() { key = selectedItem.key, count = 1 };
                AccountManager.Instance.GetEquipItem((ITEM_TYPE)selectedItem.ItemType, itemInfo);
                return itemInfo;
            }
        }
    }
    IEnumerator SummonResult(Queue<InvenItemInfo> resultItemList)
    {
        resultItemPopup.SetActive(true);
        StartCoroutine(TweenManager.Instance.TweenAlpha(screenTouchText, 1, 0.2f, 0.5f, TweenType.PINGPONG));
        isPlayingSummonResultCo = true;
        yield return new WaitForSeconds(0.2f);
        while (resultItemList.Count > 0)
        {
            GameObject go = Instantiate(resultSlotBase.gameObject, resultSlotRoot);
            ItemSlot tmp = go.GetComponent<ItemSlot>();
            if (tmp != null)
            {
                tmp.SetItemSlotInfo(SLOT_TYPE.SUMMON_RESULT, resultItemList.Dequeue());
            }
            yield return new WaitForSeconds(delayTime);
        }
        isPlayingSummonResultCo = false;
    }
    void SkipSummonResult()
    {
        StopCoroutine(summonResultCo);
        while (summonResultItemQueue.Count > 0)
        {
            GameObject go = Instantiate(resultSlotBase.gameObject, resultSlotRoot);
            ItemSlot tmp = go.GetComponent<ItemSlot>();
            if (tmp != null)
            {
                tmp.SetItemSlotInfo(SLOT_TYPE.SUMMON_RESULT, summonResultItemQueue.Dequeue());
            }
        }
        isPlayingSummonResultCo = false;
    }
    public void CloseSummonResultPopup()
    {
        if (isPlayingSummonResultCo)
        {
            SkipSummonResult();
        }
        else
        {
            resultItemPopup.SetActive(false);
            for (int i = 0; i < resultSlotRoot.childCount; i++)
            {
                Destroy(resultSlotRoot.GetChild(i).gameObject);
            }
        }
        UpdateSummonLevel();
    }
    void UpdateSummonLevel()
    {
        string tbKey = string.Empty;
        double count = AccountManager.Instance.SummonCount[(int)selectSummonType];
        uint demandCnt = 0;
        int level = 1;
        while (count >= 0)
        {
            switch (selectSummonType)
            {
                case SUMMON_TYPE.WEAPONE:
                    tbKey = string.Format("ItemGachaLvCost_{0}", level);
                    break;
                case SUMMON_TYPE.ARMOR:
                    tbKey = string.Format("DefensiveGachaLvCost_{0}", level);
                    break;
                case SUMMON_TYPE.ACC:
                    tbKey = string.Format("AccessoryGachaLvCost_{0}", level);
                    break;
            }
            Tables.InGamePrice ingameTb = Tables.InGamePrice.Get(tbKey);

            if (count >= ingameTb.demandGoodsQty)
            {
                count -= ingameTb.demandGoodsQty;
                level++;
            }
            else
            {
                demandCnt = ingameTb.demandGoodsQty;
                break;
            }
        }
        summonLevelText.text = $"Lv. {AccountManager.Instance.GetSummonLevel(selectSummonType)}";
        summonLevelFillImg.fillAmount = (float)count / demandCnt;
        summonExpText.text = string.Format("{0} / {1}", count, demandCnt);
        CheckSummonLevelUpReward();
    }
    void CheckSummonLevelUpReward()
    {
        string rewardTbKey = "";
        switch (selectSummonType)
        {
            case SUMMON_TYPE.WEAPONE:
                rewardTbKey = "GachaLv_Equip_";
                break;
            case SUMMON_TYPE.ARMOR:
                rewardTbKey = "GachaLv_Defensive_";
                break;
            case SUMMON_TYPE.ACC:
                rewardTbKey = "GachaLv_Accessory_";
                break;
            default:
                break;
        }
        Tables.Reward rewardTb = Reward.Get($"{rewardTbKey}{AccountManager.Instance.SummonRewardLevel[(int)selectSummonType]}");
        InvenItemInfo info = new InvenItemInfo() { key = rewardTb.ItemKey[0], count = (uint)rewardTb.ItemQty[0] };
        summonLevelRewardItemSlot.SetItemSlotInfo(SLOT_TYPE.REWARD, info);
        if (AccountManager.Instance.SummonRewardLevel[(int)selectSummonType] <= AccountManager.Instance.GetSummonLevel(selectSummonType))
        {
            summonLevelRewardItemSlot.ActiveNotiImg(true);
            summonLevelRewardItemSlot.ActiveNotGetImg(false);

        }
        else
        {
            summonLevelRewardItemSlot.ActiveNotiImg(false);
            summonLevelRewardItemSlot.ActiveNotGetImg(true);
        }

    }
    public void OnClickSummonLevelReward()
    {
        if (AccountManager.Instance.SummonRewardLevel[(int)selectSummonType] <= AccountManager.Instance.GetSummonLevel(selectSummonType))
        {
            string rewardTbKey = "";
            switch (selectSummonType)
            {
                case SUMMON_TYPE.WEAPONE:
                    rewardTbKey = "GachaLv_Equip_";
                    break;
                case SUMMON_TYPE.ARMOR:
                    rewardTbKey = "GachaLv_Defensive_";
                    break;
                case SUMMON_TYPE.ACC:
                    rewardTbKey = "GachaLv_Accessory_";
                    break;
                default:
                    break;
            }
            rewardTbKey += AccountManager.Instance.SummonRewardLevel[(int)selectSummonType];
            GameManager.Instance.GetReward(rewardTbKey, out bool result);
            if (result)
            {
                AccountManager.Instance.SummonRewardLevel[(int)selectSummonType]++;
                CheckSummonLevelUpReward();
            }
        }

    }
}
