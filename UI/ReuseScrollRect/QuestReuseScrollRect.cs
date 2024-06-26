using System.Collections.Generic;
using System.Linq;
using UI;
using UnityEngine;

public class QuestReuseScrollRect : ReuseScrollview<QuestSlotCellData>
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }


    public void CreateQuestSlot(QUEST_TYPE _type)
    {
        tableData = new List<QuestSlotCellData>();

        // 필요한 QuestInfo들을 캐싱
        var questInfoDictionary = AccountManager.Instance.QuestInfoDictionary
            .SelectMany(kv => kv.Value)
            .ToDictionary(qi => qi.key, qi => qi);

        // 필터링된 퀘스트 데이터를 가져옴
        var filteredQuests = Tables.Quest.data.Values
            .Where(quest => quest.QuestType == (int)_type);

        foreach (var tb in filteredQuests)
        {
            int index = 0;
            if (questInfoDictionary.TryGetValue(tb.key, out var questInfo))
            {
                QuestSlotCellData cell = new QuestSlotCellData();
                cell.Index = index++;
                cell.m_QuestInfo = questInfo;
                cell.m_QuestInfo.GetQuestProcess();
                tableData.Add(cell);
            }
            else
            {
                Debug.Log($"Quest Mapping Fail for key: {tb.key}");
            }
        }

        InitTableView();
    }
}
