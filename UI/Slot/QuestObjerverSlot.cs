using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestObjerverSlot : MonoBehaviour
{
    [SerializeField] ItemSlot rewardItemSlot;
    [SerializeField] TextMeshProUGUI questNameTxt;
    [SerializeField] TextMeshProUGUI questProcessPercentTxt;
    [SerializeField] TextMeshProUGUI questProcessTxt;
    [SerializeField] Slider questProcessSlider;





    List<QuestInfo> oederbyQuestCount = new List<QuestInfo>();
    ItemSlotCell rewardSlotCell;

    private void Start()
    {
        foreach (var item in AccountManager.Instance.QuestInfoDictionary)
        {
            oederbyQuestCount.AddRange(item.Value);
        }

        DisplayQuestInfo(oederbyQuestCount.First());
    }

    public void DisplayQuestInfo(QuestInfo _quest)
    {
        Tables.Quest questTb = Tables.Quest.Get(_quest.key);

        if (questTb != null)
        {
            string questTypeName = GetQuestTypeName((QUEST_TYPE)questTb.QuestType);

            questNameTxt.text = $"{questTypeName}{UIManager.Instance.GetText(questTb.QuestName)}";
            questProcessPercentTxt.text = $"{(_quest.questCount / questTb.Value) * 100}%";
            questProcessTxt.text = $"{_quest.questCount}/{questTb.Value}";
            questProcessSlider.value = (float)_quest.questCount / questTb.Value;

            DisplayQuestRewards(_quest);
        }
    }

    public void ChangeDisplayQuest()
    {
        oederbyQuestCount = oederbyQuestCount
                                     .OrderByDescending(x => x.isCompleted)
                                     .ThenByDescending(x => x.isDone)
                                     .ThenByDescending(x => x.questCount)
                                     .ThenBy(x => x.m_QuestTb.QuestType).ToList();
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
}
