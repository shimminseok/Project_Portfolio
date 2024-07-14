using System.Collections.Generic;
using System.Linq;
using Tables;
using UnityEngine;
using UnityEngine.UI;

public class UIQuest : UIPopUp
{
    public static UIQuest instance;

    [SerializeField] QuestReuseScrollRect questScrollRect;
    [SerializeField] List<GameObject> questCompletedNotiObj;
    [SerializeField] List<Toggle> questTypeTabList;
    int selectedQuestType = (int)QUEST_TYPE.LOOP;
    void Awake()
    {
        if (instance == null)
            instance = this;
    }
    protected override void Start()
    {
        base.Start();
    }

    public override void OpenPopUp()
    {
        base.OpenPopUp();
        OnClickQuestTab(selectedQuestType);
    }
    public override void ClosePopUp()
    {
        base.ClosePopUp();
    }


    public void OnClickQuestTab(int _type)
    {
        selectedQuestType = _type;
        questScrollRect.CreateQuestSlot((QUEST_TYPE)_type);
    }
    public void IncreaseQuestCount(QUEST_CARTEGORY _cartegory, double _value)
    {
        if (UIManager.Instance.openedPopupList.Contains(this))
        {
            foreach (var cell in questScrollRect.Cells)
            {
                if (cell.Index < questScrollRect.TableData.Count && cell.Index >= 0)
                    cell.UpdateContent(questScrollRect.TableData[cell.Index]);
            }
        }

        if (AccountManager.Instance.QuestInfoDictionary.TryGetValue(_cartegory, out var list))
        {
            list.ForEach(x => x.IncrementQuestCount(_value));
        }
    }
    public void IncreaseQuestCount(int _key, double _value)
    {
        if (UIManager.Instance.openedPopupList.Contains(this))
        {
            foreach (var cell in questScrollRect.Cells)
            {
                if (cell.Index < questScrollRect.TableData.Count && cell.Index >= 0)
                    cell.UpdateContent(questScrollRect.TableData[cell.Index]);
            }
        }
        Tables.Quest questTb = Quest.Get(_key);
        if (questTb != null)
        {
            AccountManager.Instance.QuestInfoDictionary[(QUEST_CARTEGORY)questTb.QuestGroupType].Find(x => x.key == _key)?.IncrementQuestCount(_value);
        }
    }
    /// <summary>
    /// 메인화면에서의 Noti이미지 활성/비활성을 위한 함수
    /// </summary>
    /// <returns></returns>
    public bool ActiveNotiState()
    {
        return questCompletedNotiObj.Any(x => x.activeSelf);
    }

    ItemSlotCell SetRewardDetails(Reward rewardTb, ITEM_CATEGORY itemCategory)
    {
        ItemSlotCell cell = new ItemSlotCell();
        switch (itemCategory)
        {
            case ITEM_CATEGORY.GOODS:
                cell.key = rewardTb.GoodsKey[0];
                cell.count = rewardTb.GoodsQty[0];
                cell.itemGrade = Tables.Goods.Get(rewardTb.GoodsKey[0]).Grade;
                break;
            case ITEM_CATEGORY.MATERIAL:
                cell.key = rewardTb.MaterialKey[0];
                cell.count = rewardTb.MaterialQty[0];
                cell.itemGrade = Tables.Material.Get(rewardTb.MaterialKey[0]).Grade;

                break;
            case ITEM_CATEGORY.ITEM:
                cell.key = rewardTb.ItemKey[0];
                cell.count = rewardTb.ItemQty[0];
                cell.itemGrade = Tables.Item.Get(rewardTb.ItemKey[0]).ItemGrade;
                break;
        }

        return cell;
    }
    ITEM_CATEGORY DetermineItemCategory(Reward rewardTb)
    {
        if (rewardTb.GoodsKey.Length > 0)
            return ITEM_CATEGORY.GOODS;
        if (rewardTb.MaterialKey.Length > 0)
            return ITEM_CATEGORY.MATERIAL;
        if (rewardTb.ItemKey.Length > 0)
            return ITEM_CATEGORY.ITEM;

        return ITEM_CATEGORY.NONE;
    }

    public void SetRewardDetails(ItemSlot _targetSlot, Tables.Reward _reward)
    {

        ITEM_CATEGORY category = DetermineItemCategory(_reward);
        ItemSlotCell cell = SetRewardDetails(_reward, category);
        _targetSlot.UpdateSlot(cell);
    }
}
