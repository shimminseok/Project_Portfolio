using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UISummon : UIFullPopUp
{

    [SerializeField] List<Toggle> summonTypeToggles = new List<Toggle>();


    [Header("ResultItemPopup")]
    [SerializeField] GameObject resultItemPopup;
    [SerializeField] ItemSlot resultSlotBase;
    [SerializeField] Transform resultSlotRoot;
    [SerializeField] TextMeshProUGUI screenTouchText;
    SUMMON_TYPE selectSummonType;


    [SerializeField] float delayTime = 0.2f;

    private void Start()
    {
        selectSummonType = SUMMON_TYPE.WEAPONE;
        //Summon(30);
    }
    public override void OpenPopUp()
    {
        base.OpenPopUp();
        summonTypeToggles[0].isOn = true;
    }

    public override void ClosePopUp()
    {
        base.ClosePopUp();
    }

    public void OnClickSummonBtn(int _count)
    {
        Summon(_count);
    }
    public void OnClickSummonTab(GameObject _go)
    {
        int onToggleIndex = summonTypeToggles.FindIndex(x => x.name == _go.name);
        selectSummonType = (SUMMON_TYPE)onToggleIndex;
    }

    void Summon(int _count)
    {
        List<InvenItemInfo> resultItemList = new List<InvenItemInfo>();

        for (int i = 0; i < _count; i++)
        {
            List<Tables.Item> itemList = GetSummonItemList();
            AccountManager.Instance.SummonCountUp(selectSummonType);
            InvenItemInfo summonedItem = SummonRandomItem(itemList);

            InvenItemInfo resultItem = resultItemList.Find(x => x.key == summonedItem.key);
            if (resultItem == null)
            {
                resultItemList.Add(summonedItem);
            }
        }

        StartCoroutine(SummonResult(resultItemList));
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
            string qualityKey = $"Summons_Quality_Grade_{randomIndex + 1}";
            Tables.Define defineTb = Tables.Define.Get(qualityKey);

            if (Random.Range(0f, 100f) < defineTb.value)
            {
                var selectedItem = itemList[randomIndex];
                InvenItemInfo itemInfo = new InvenItemInfo() { key = selectedItem.key};
                AccountManager.Instance.GetEquipItem((ITEM_TYPE)selectedItem.ItemType, itemInfo);
                return itemInfo;
            }
        }
    }

    IEnumerator SummonResult(List<InvenItemInfo> resultItemList)
    {
        yield return new WaitForSeconds(0.5f);
        resultItemPopup.SetActive(true);
        for (int i = 0; i < resultItemList.Count; i++)
        {
            yield return new WaitForSeconds(delayTime);
            GameObject go = Instantiate(resultSlotBase.gameObject, resultSlotRoot);
            ItemSlot tmp = go.GetComponent<ItemSlot>();
            if(tmp != null) 
            {
                tmp.SetItemSlotInfo(SLOT_TYPE.SUMMON_RESULT, resultItemList[i].key);
            }
        }

        StartCoroutine(TweenManager.Instance.TweenAlpha(screenTouchText,1,0.2f,0.5f,TweenType.PINGPONG));
    }
    public void CloseSummonResultPopup()
    {
        resultItemPopup.SetActive(false);
    }
}
