using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestObjerverSlot : MonoBehaviour
{
    [SerializeField] GameObject notiImg;
    [SerializeField] ItemSlot rewardItemSlot;
    [SerializeField] TextMeshProUGUI questNameTxt;
    [SerializeField] TextMeshProUGUI questProcessPercentTxt;
    [SerializeField] TextMeshProUGUI questProcessTxt;
    [SerializeField] Slider questProcessSlider;





    List<QuestInfo> oederbyQuestCount = new List<QuestInfo>();

    public QuestInfo CurrentQuestInfo => currentQuestInfo;

    Tables.Quest currentQuestInfoTb;
    QuestInfo currentQuestInfo;
    ItemSlotCell rewardSlotCell;

    private void Start()
    {
        foreach (var item in AccountManager.Instance.QuestInfoDictionary)
        {
            oederbyQuestCount.AddRange(item.Value);
        }
        SortingQuest();
    }

    public void DisplayQuestInfo()
    {
        UpdateDisplayQuest(currentQuestInfo);
        DisplayQuestRewards(currentQuestInfo);
    }
    public void UpdateDisplayQuest(QuestInfo _quest)
    {
        Tables.Quest questTb = Tables.Quest.Get(_quest.key);
        if (questTb != null)
        {
            string questTypeName = GetQuestTypeName((QUEST_TYPE)questTb.QuestType);

            questNameTxt.text = $"{questTypeName}{UIManager.Instance.GetText(questTb.QuestName)}";
            questProcessTxt.text = $"{_quest.questCount}/{questTb.Value}";

            questProcessSlider.value = (float)_quest.questCount / questTb.Value;
            questProcessPercentTxt.text = $"{questProcessSlider.value * 100}%";

            notiImg.SetActive(_quest.isCompleted && !_quest.isDone);
        }

    }
    void ChangeDisplayQuest()
    {
        currentQuestInfoTb = Tables.Quest.Get(currentQuestInfo.key);
        DisplayQuestInfo();
    }
    public void SortingQuest()
    {

        var orderedQuests = oederbyQuestCount
                             .OrderByDescending(x => x.isCompleted)
                             .ThenBy(x => x.m_QuestTb.QuestType)
                             .ThenByDescending(x => x.ClearPercent)
                             .Where(x => !x.isDone);

        if (currentQuestInfo == null || currentQuestInfo.key != orderedQuests.First().key)
        {
            currentQuestInfo = orderedQuests.First();
            ChangeDisplayQuest();

        }
    }

    string GetQuestTypeName(QUEST_TYPE _questType)
    {
        switch (_questType)
        {
            case QUEST_TYPE.DAILY:
                return "[老老]";
            case QUEST_TYPE.WEEKLY:
                return "[林埃]";
            case QUEST_TYPE.ACHIEVEMENT:
                return "[诀利]";
            case QUEST_TYPE.LOOP:
                return "[馆汗]";
            default:
                return string.Empty;
        }
    }
    void DisplayQuestRewards(QuestInfo _quest)
    {
        Tables.Reward rewardTb = Tables.Reward.Get(_quest.m_QuestTb.QuestReward);

        if (rewardTb != null)
        {
            UIQuest.instance.SetRewardDetails(rewardItemSlot, rewardTb);
        }
    }


    public void OnClickSlot()
    {
        if (currentQuestInfo.isDone)
            return;

        else if (currentQuestInfo.isCompleted)
        {
            currentQuestInfo.GetReward();
            DisplayQuestInfo();
        }
    }
}
