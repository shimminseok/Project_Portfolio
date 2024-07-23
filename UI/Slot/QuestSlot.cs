using NPOI.SS.Formula.Functions;
using Tables;
using TMPro;
using UI;
using Unity.Burst.Intrinsics;
using UnityEngine;
using UnityEngine.UI;

public class QuestSlot : ReuseCellData<QuestSlotCellData>
{
    [SerializeField] ItemSlot rewardItem;
    [SerializeField] Image rewardItemIcon;
    [SerializeField] TextMeshProUGUI itemQtyTxt;
    [SerializeField] TextMeshProUGUI questName;
    [SerializeField] TextMeshProUGUI questDesc;
    [SerializeField] Image progressFillImg;
    [SerializeField] TextMeshProUGUI progressTxt;

    [SerializeField] Image recivingBtn;
    [SerializeField] TextMeshProUGUI recivingTxt;


    Tables.Quest m_QuestTb;

    string shortcutsImg = "com_btn_014";
    string beforeReceivingRewardImg = "com_btn_009";
    string afterReceivingRewardImg = "com_btn_012";

    string shortcutsTxt = "바로가기";
    string beforeReceivingRewardTxt = "받기";
    string afterReceivingRewardTxt = "완료";
    public override void UpdateContent(QuestSlotCellData _itemData)
    {
        m_data = _itemData;
        m_QuestTb = Tables.Quest.Get(_itemData.m_QuestInfo.key);

        SetRewardItemSlot(Reward.Get(m_QuestTb.QuestReward));

        UpdateQuestSlot();

        SetReceivingButtonState();
    }
    public void SetRewardItemSlot(Reward _rewardTb )
    {
        RewardManager.Instance.SetRewardDetails(rewardItem,_rewardTb);
    }
    void UpdateQuestSlot()
    {
        progressFillImg.fillAmount = (float)(m_data.m_QuestInfo.questCount / m_QuestTb.Value);
        progressTxt.text = $"{m_data.m_QuestInfo.questCount}/{m_QuestTb.Value}";

        questName.text = UIManager.Instance.GetText(m_QuestTb.QuestName);
        questDesc.text = UIManager.Instance.GetText(m_QuestTb.QuestDescription);

        m_data.m_QuestInfo.GetQuestProcess();
    }



    void SetReceivingButtonState()
    {
        if(m_data.m_QuestInfo.isDone)
        {
            recivingBtn.sprite = UIManager.Instance.GetSprite(SPRITE_TYPE.BTN_ICON, afterReceivingRewardImg);
            recivingTxt.text = afterReceivingRewardTxt;
        }
        else if (m_data.m_QuestInfo.isCompleted)
        {
            recivingBtn.sprite = UIManager.Instance.GetSprite(SPRITE_TYPE.BTN_ICON, beforeReceivingRewardImg);
            recivingTxt.text = beforeReceivingRewardTxt;
        }
        else
        {
            recivingBtn.sprite = UIManager.Instance.GetSprite(SPRITE_TYPE.BTN_ICON, shortcutsImg);
            recivingTxt.text = shortcutsTxt;
        }
        UpdateQuestSlot();
    }

    public void OnClickReciveBtn()
    {
        if (m_data.m_QuestInfo.isDone)
            return;

        else if (m_data.m_QuestInfo.isCompleted)
            CompleatedQuest();
        else
            ShortCutsPopup();
    }

    void CompleatedQuest()
    {
        m_data.m_QuestInfo.GetReward();
        SetReceivingButtonState();
    }
    void ShortCutsPopup()
    {
        switch ((QUEST_CARTEGORY)m_QuestTb.QuestGroupType)
        {
            case QUEST_CARTEGORY.KILL_MONSTER:
            case QUEST_CARTEGORY.CHAR_LEVEL_UP:
            case QUEST_CARTEGORY.QUEST_CLEAR:
                UIManager.Instance.OnClickClosePopUp(UIQuest.instance);
                break;
            case QUEST_CARTEGORY.USE_GOLD:
            case QUEST_CARTEGORY.UPGRADE_GROWTH:
                UIManager.Instance.OnClickOpenPopUp(UIGrowth.instance);
                break;
            case QUEST_CARTEGORY.USE_DIA:
            case QUEST_CARTEGORY.SUMMON:
                UIManager.Instance.OnClickOpenPopUp(UISummon.instance);
                break;
            case QUEST_CARTEGORY.GET_EQUIPMENT:
                UIManager.Instance.OnClickOpenPopUp(UIInventory.instance);
                break;
            case QUEST_CARTEGORY.CLEAR_STAGE:
                UIManager.Instance.OnClickOpenPopUp(UIWorldMap.instance);
                break;
        }
    }
}
