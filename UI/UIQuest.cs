using System;
using System.Collections.Generic;
using System.Linq;
using Tables;
using UnityEngine;
using UnityEngine.UI;

public class UIQuest : UIPopUp
{
    public static UIQuest instance;

    [Header("Main")]
    [SerializeField] GameObject mainQuestMenuNoti;


    [SerializeField] QuestReuseScrollRect questScrollRect;
    [SerializeField] List<GameObject> questCompletedNotiObj;
    [SerializeField] List<Toggle> questTypeTabList;


    int selectedQuestType = (int)QUEST_TYPE.LOOP;

    public event  Action<QUEST_TYPE> OnQuestCompleted;

    public Dictionary<QUEST_TYPE, List<QuestInfo>> questInfoDictionaryByType = new Dictionary<QUEST_TYPE, List<QuestInfo>>();

    public Dictionary<int, QuestInfo> questInfoDictionary = new Dictionary<int, QuestInfo>();
    void Awake()
    {
        if (instance == null)
            instance = this;
    }
    protected override void Start()
    {
        base.Start();

        questInfoDictionary = AccountManager.Instance.QuestInfoDictionary
            .SelectMany(kv => kv.Value)
            .ToDictionary(qi => qi.key, qi => qi);

        foreach (var item in questInfoDictionary.Values)
        {
            var questType = (QUEST_TYPE)item.m_QuestTb.QuestType;

            if (questInfoDictionaryByType.TryGetValue(questType, out var questList))
            {
                questList.Add(item);
            }
            else
            {
                questList = new List<QuestInfo> { item };
                questInfoDictionaryByType.Add(questType, questList);
            }
        }
        for (int i = 0; i < (int)QUEST_TYPE.MAX; i++)
        {
            UpdateNotiImage((QUEST_TYPE)i);
        }
        UpdateMainNotiImage();
        OnQuestCompleted += HandleQuestCompleted;

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
    public void OnQuestTypeNoti(QuestInfo _quest)
    {
        OnQuestCompleted?.Invoke((QUEST_TYPE)_quest.m_QuestTb.QuestType);
    }
    public void UpdateNotiImage(QUEST_TYPE _type)
    {
        bool hasReward = questInfoDictionaryByType[_type].Any(x => x.isCompleted && !x.isDone);
        questCompletedNotiObj[(int)_type].SetActive(hasReward);
        UpdateMainNotiImage();
    }
    void UpdateMainNotiImage()
    {
        bool hasReward = questCompletedNotiObj.Find(x => x.activeSelf);
        mainQuestMenuNoti.SetActive(hasReward);
    }
    void HandleQuestCompleted(QUEST_TYPE _type)
    {
        UpdateNotiImage(_type);
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
    #region[ButtonEvent]
    public void OnClickQuestTab(int _type)
    {
        selectedQuestType = _type;
        questScrollRect.CreateQuestSlot((QUEST_TYPE)_type);
    }
    public void OnClickAllReceiveRewardBtn()
    {
        var receivableQuestList = questInfoDictionary.Where(x => x.Value.isCompleted && !x.Value.isDone);
        foreach(var quest in receivableQuestList)
        {
            quest.Value.GetReward();
        }
    }
    #endregion
}
