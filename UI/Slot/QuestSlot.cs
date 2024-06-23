using NPOI.SS.Formula.Functions;
using Tables;
using TMPro;
using UI;
using Unity.Burst.Intrinsics;
using UnityEngine;
using UnityEngine.UI;

public class QuestSlot : ReuseCellData<QuestSlotCellData>
{
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
        m_QuestTb = Tables.Quest.Get(_itemData.m_QuestInfo.key);

        Reward rewardTb = Reward.Get(m_QuestTb.QuestReward);
        double rewardQty = 0;

        ITEM_CATEGORY itemCategory = DetermineItemCategory(rewardTb);
        SetRewardDetails(rewardTb, itemCategory, ref rewardQty);

        itemQtyTxt.text = $"x{Utility.ToCurrencyString(rewardQty)}";

        progressFillImg.fillAmount = (float)(_itemData.m_QuestInfo.questCount / m_QuestTb.Value);
        progressTxt.text = $"{_itemData.m_QuestInfo.questCount}/{m_QuestTb.Value}";

        questName.text = UIManager.Instance.GetText(m_QuestTb.QuestName);
        questDesc.text = UIManager.Instance.GetText(m_QuestTb.QuestDescription);

        _itemData.m_QuestInfo.GetQuestProcess();
        SetReceivingButtonState(_itemData.m_QuestInfo.questState);
    }

    private ITEM_CATEGORY DetermineItemCategory(Reward rewardTb)
    {
        if (rewardTb.GoodsKey.Length > 0)
            return ITEM_CATEGORY.GOODS;
        if (rewardTb.MaterialKey.Length > 0)
            return ITEM_CATEGORY.MATERIAL;
        return ITEM_CATEGORY.ITEM;
    }

    private void SetRewardDetails(Reward rewardTb, ITEM_CATEGORY itemCategory, ref double rewardQty)
    {
        switch (itemCategory)
        {
            case ITEM_CATEGORY.GOODS:
                SetIconSprite(Tables.Goods.Get(rewardTb.GoodsKey[0])?.GoodsIcon);
                rewardQty = rewardTb.GoodsQty[0];
                break;
            case ITEM_CATEGORY.MATERIAL:
                SetIconSprite(Tables.Material.Get(rewardTb.MaterialKey[0])?.MaterialIcon);
                rewardQty = rewardTb.MaterialQty[0];
                break;
            case ITEM_CATEGORY.ITEM:
                SetIconSprite(Tables.Item.Get(rewardTb.ItemKey[0])?.ItemIcon);
                rewardQty = rewardTb.ItemQty[0];
                break;
        }
    }

    private void SetReceivingButtonState(int questState)
    {
        switch (questState)
        {
            case 1:
                recivingBtn.sprite = UIManager.Instance.GetSprite(SPRITE_TYPE.BTN_ICON, afterReceivingRewardImg);
                recivingTxt.text = afterReceivingRewardTxt;
                break;
            case 0:
                recivingBtn.sprite = UIManager.Instance.GetSprite(SPRITE_TYPE.BTN_ICON, beforeReceivingRewardImg);
                recivingTxt.text = beforeReceivingRewardTxt;
                break;
            default:
                recivingBtn.sprite = UIManager.Instance.GetSprite(SPRITE_TYPE.BTN_ICON, shortcutsImg);
                recivingTxt.text = shortcutsTxt;
                break;
        }
    }
    void SetIconSprite(string iconName)
    {
        if (!string.IsNullOrEmpty(iconName))
        {
            rewardItemIcon.sprite = UIManager.Instance.GetSprite(SPRITE_TYPE.ITEM_ICON, iconName);
        }
    }

    public void OnClickReciveBtn()
    {
        if (m_data.m_QuestInfo.questState == 1)
            return;

        else if (m_data.m_QuestInfo.questState == 0)
            CompleatedQuest();
        else
            ShortCutsPopup();
    }

    void CompleatedQuest()
    {
        //일일 미션 혹은 주간 미션이면
        //일일퀘스트완료 및 주간 퀘스트 완료를 증가시켜줘야함.
        m_data.m_QuestInfo.GetReward();
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
