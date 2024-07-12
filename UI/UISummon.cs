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
    Queue<ItemSlotCell> summonResultItemQueue = new Queue<ItemSlotCell>();
    void Awake()
    {
        if (instance == null)
            instance = this;
    }
    protected override void Start()
    {
        base.Start();
        selectSummonType = SUMMON_TYPE.WEAPONE;
    }
    public override void OpenPopUp()
    {
        UIManager.Instance.popupType = FULL_POPUP_TYPE.SUMMON;

        base.OpenPopUp();
        OnClickSummonTab(summonTypeToggles[0].gameObject);
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
            List<ItemSlotCell> summonedItems = new List<ItemSlotCell>();

            for (int i = 0; i < resultSlotRoot.childCount; i++)
            {
                Destroy(resultSlotRoot.GetChild(i).gameObject);
            }
            summonResultItemQueue.Clear();

            for (int i = 0; i < _count; i++)
            {
                switch (selectSummonType)
                {
                    case SUMMON_TYPE.WEAPONE:
                    case SUMMON_TYPE.ARMOR:
                    case SUMMON_TYPE.ACC:
                        List<Tables.Item> itemList = GetSummonItemList<Tables.Item>();
                        ItemSlotCell summonedItem = SummonRandomItem(itemList);
                        summonedItems.Add(summonedItem); break;
                    case SUMMON_TYPE.SKILL:
                        List<Tables.Skill> skillList = GetSummonItemList<Tables.Skill>();
                        ItemSlotCell summonedSkill = SummonRandomItem(skillList);
                        summonedItems.Add(summonedSkill);
                        break;
                }
                AccountManager.Instance.SummonCountUp(selectSummonType);
            }

            foreach (var summonedItem in summonedItems)
            {
                ItemSlotCell resultItem = summonResultItemQueue.FirstOrDefault(x => x.key == summonedItem.key);
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
        UIQuest.instance.IncreaseQuestCount(QUEST_CARTEGORY.SUMMON, _count);
    }
    List<T> GetSummonItemList<T>() where T : class
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
                return FilterItemsBySummonType<T>(j + 1);
            }
        }
        return new List<T>();
    }

    List<T> FilterItemsBySummonType<T>(int grade) where T : class
    {
        switch (selectSummonType)
        {
            case SUMMON_TYPE.WEAPONE:
                if (typeof(T) == typeof(Tables.Item))
                {
                    return Tables.Item.data.Values
                        .Where(x => x.ItemGrade == grade && x.isSummon == 1 && x.ItemType == (int)ITEM_TYPE.WEAPONE)
                        .Cast<T>()
                        .ToList();
                }
                break;

            case SUMMON_TYPE.ARMOR:
                if (typeof(T) == typeof(Tables.Item))
                {
                    return Tables.Item.data.Values
                        .Where(x => x.ItemGrade == grade && x.isSummon == 1 && x.ItemType >= (int)ITEM_TYPE.ARMOR && x.ItemType <= (int)ITEM_TYPE.SHOES)
                        .Cast<T>()
                        .ToList();
                }
                break;

            case SUMMON_TYPE.ACC:
                if (typeof(T) == typeof(Tables.Item))
                {
                    return Tables.Item.data.Values
                        .Where(x => x.ItemGrade == grade && x.isSummon == 1 && x.ItemType >= (int)ITEM_TYPE.ACC_1 && x.ItemType <= (int)ITEM_TYPE.ACC_2)
                        .Cast<T>()
                        .ToList();
                }
                break;

            case SUMMON_TYPE.SKILL:
                if (typeof(T) == typeof(Tables.Skill))
                {
                    return Tables.Skill.data.Values
                        .Where(x => x.SkillGrade == grade)
                        .Cast<T>()
                        .ToList();
                }
                break;
        }
        return new List<T>();
    }
    ItemSlotCell SummonRandomItem<T>(List<T> list) where T : class
    {
        while (true)
        {
            int randomIndex = Random.Range(0, list.Count);
            var selectedItem = list[randomIndex];

            string qualityKey;
            Tables.Define defineTb = null;

            if (typeof(T) == typeof(Tables.Item))
            {
                var item = selectedItem as Tables.Item;
                qualityKey = $"Summons_Quality_Grade_{item.Quality_Grade}";
                defineTb = Tables.Define.Get(qualityKey);
            }

            if (defineTb != null && Random.Range(0f, 100f) >= defineTb.value)
            {
                continue;
            }
            switch (selectSummonType)
            {
                case SUMMON_TYPE.WEAPONE:
                case SUMMON_TYPE.ARMOR:
                case SUMMON_TYPE.ACC:
                    if (selectedItem is Tables.Item item)
                    {
                        InvenItem itemInfo = new InvenItem { key = item.key, count = 1 };
                        AccountManager.Instance.AddorUpdateItem((ITEM_TYPE)item.ItemType, itemInfo);
                    }
                    break;
                case SUMMON_TYPE.SKILL:
                    if (selectedItem is Tables.Skill skill)
                    {
                        SkillItem skillItem = new SkillItem { key = skill.key, count = 1 };
                        AccountManager.Instance.AddorUpdateSkill(skillItem);
                    }
                    break;
            }
            return new ItemSlotCell { key = GameManager.Instance.GetKey(selectedItem), count = 1 };
        }
    }



    IEnumerator SummonResult(Queue<ItemSlotCell> resultItemList)
    {
        resultItemPopup.SetActive(true);
        StartCoroutine(TweenManager.Instance.TweenAlpha(screenTouchText, 1, 0.2f, 0.5f, TweenType.PINGPONG));
        isPlayingSummonResultCo = true;
        yield return new WaitForSeconds(0.2f);
        while (resultItemList.Count > 0)
        {
            ItemSlotCell slotcell = resultItemList.Dequeue();
            object itemInfo = CreateItemInfo(slotcell);
            InstantiateAndSetupItemSlot(itemInfo);
            yield return new WaitForSeconds(delayTime);
        }
        isPlayingSummonResultCo = false;
    }
    object CreateItemInfo(ItemSlotCell slotcell)
    {
        switch (selectSummonType)
        {
            case SUMMON_TYPE.WEAPONE:
            case SUMMON_TYPE.ARMOR:
            case SUMMON_TYPE.ACC:
                return new InvenItem { key = slotcell.key, count = slotcell.count, isGet = true };
            case SUMMON_TYPE.SKILL:
                return new SkillItem { key = slotcell.key, count = slotcell.count, isGet = true };
            default:
                return null;
        }
    }

    void InstantiateAndSetupItemSlot(object itemInfo)
    {
        var itemSlot = Instantiate(resultSlotBase.gameObject, resultSlotRoot).GetComponent<ItemSlot>();
        if (itemSlot != null)
        {
            itemSlot.UpdateSlotByType(itemInfo);
        }
    }
    void SkipSummonResult()
    {
        StopCoroutine(summonResultCo);
        while (summonResultItemQueue.Count > 0)
        {
            Instantiate(resultSlotBase.gameObject, resultSlotRoot).GetComponent<ItemSlot>()?.UpdateSlot(summonResultItemQueue.Dequeue());
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
        double count = AccountManager.Instance.SummonCount[(int)selectSummonType];
        uint demandCnt = 0;
        int level = 1;
        string tbKey = string.Empty;
        while (true)
        {
            switch (selectSummonType)
            {
                case SUMMON_TYPE.WEAPONE:
                    tbKey = $"ItemGachaLvCost_{level}";
                    break;
                case SUMMON_TYPE.ARMOR:
                    tbKey = $"DefensiveGachaLvCost_{level}";
                    break;
                case SUMMON_TYPE.ACC:
                    tbKey = $"AccessoryGachaLvCost_{level}";
                    break;
                case SUMMON_TYPE.SKILL:
                    tbKey = $"SkillGachaLvCost_{level}";
                    break;
                default:
                    Debug.Log($"SummonType Not Define");
                    return;
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
        summonLevelText.text = $"Lv. {level}";
        summonLevelFillImg.fillAmount = (float)count / demandCnt;
        summonExpText.text = $"{count} / {demandCnt}";
        CheckSummonLevelUpReward();
    }
    void CheckSummonLevelUpReward()
    {
        string rewardTbKey = GetRewardTableKey(selectSummonType);
        if (string.IsNullOrEmpty(rewardTbKey))
        {
            return;
        }

        Tables.Reward rewardTb = Reward.Get($"{rewardTbKey}{AccountManager.Instance.SummonRewardLevel[(int)selectSummonType]}");
        ItemSlotCell info = new ItemSlotCell { key = rewardTb.GoodsKey[0], count = (uint)rewardTb.GoodsQty[0] };
        summonLevelRewardItemSlot.UpdateSlot(info);

        bool isEligibleForReward = AccountManager.Instance.SummonRewardLevel[(int)selectSummonType] <= AccountManager.Instance.GetSummonLevel(selectSummonType);
        summonLevelRewardItemSlot.ActiveNotiImg(isEligibleForReward);
        summonLevelRewardItemSlot.ActiveNotGetImg(!isEligibleForReward);
    }
    public void OnClickSummonLevelReward()
    {
        if (AccountManager.Instance.SummonRewardLevel[(int)selectSummonType] <= AccountManager.Instance.GetSummonLevel(selectSummonType))
        {
            string rewardTbKey = GetRewardTableKey(selectSummonType);
            if (string.IsNullOrEmpty(rewardTbKey))
            {
                return;
            }

            rewardTbKey += AccountManager.Instance.SummonRewardLevel[(int)selectSummonType];
            GameManager.Instance.GetReward(rewardTbKey, out bool result, true);
            if (result)
            {
                AccountManager.Instance.SummonRewardLevel[(int)selectSummonType]++;
                CheckSummonLevelUpReward();
            }
        }
    }
    string GetRewardTableKey(SUMMON_TYPE summonType)
    {
        return summonType switch
        {
            SUMMON_TYPE.WEAPONE => "GachaLv_Equip_",
            SUMMON_TYPE.ARMOR => "GachaLv_Defensive_",
            SUMMON_TYPE.ACC => "GachaLv_Accessory_",
            SUMMON_TYPE.SKILL => "GachaLv_skill_",
            _ => string.Empty,
        };
    }
}
