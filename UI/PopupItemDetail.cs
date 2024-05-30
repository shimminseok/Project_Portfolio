using System.Collections.Generic;
using Tables;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class PopupItemDetail : UIPopUp
{
    [SerializeField] InvenItemSlot itemSlot;
    [SerializeField] List<TextMeshProUGUI> passiveEffect_Name;
    [SerializeField] List<TextMeshProUGUI> passiveEffect_Value;

    [SerializeField] List<TextMeshProUGUI> equippedEffect_Name;
    [SerializeField] List<TextMeshProUGUI> equippedEffect_Value;

    [SerializeField] Image enhanceCostIcon;
    [SerializeField] TextMeshProUGUI enhanceCostText;

    [SerializeField] TextMeshProUGUI equipText;

    public InvenItemInfo m_invenItem;

    float holddingTime = 0f;
    bool isHoldding = false;

    int enhanceCount;

    void Update()
    {
        if (isHoldding)
        {
            holddingTime += Time.deltaTime;
            if (holddingTime > 0.5f)
            {
                OnClickEnhanceBtn();
                holddingTime = 0.45f;
            }
        }

    }
    public override void OpenPopUp()
    {
        base.OpenPopUp();
        SetItemDetailInfo(m_invenItem);
    }
    public override void ClosePopUp()
    {
        base.ClosePopUp();
    }

    public void SetItemDetailInfo(InvenItemInfo _item)
    {
        Tables.Item itemTb = Tables.Item.Get(_item.key);
        if (itemTb != null)
        {
            itemSlot.SetSlotInfo(itemTb);
            for (int i = 0; i < passiveEffect_Name.Count; i++)
            {
                passiveEffect_Name[i].text = _item.GetAbilityText()[i];
                passiveEffect_Value[i].text = AccountManager.Instance.ToCurrencyString(_item.GetPassiveEffectValues()[i]);
            }

            for (int i = 0; i < equippedEffect_Name.Count; i++)
            {
                equippedEffect_Name[i].text = _item.GetAbilityText()[i];
                string str = string.Format("{0} -> {1}", AccountManager.Instance.ToCurrencyString(_item.GetEquipEffectValues()[i]), AccountManager.Instance.ToCurrencyString(_item.GetEquipEffectValues(1)[i]));
                equippedEffect_Value[i].text = str;
            }



            enhanceCostText.text = AccountManager.Instance.ToCurrencyString(10000);

            equipText.text = _item.isEquipped ? "¿Â¬¯¡ﬂ" : "¿Â¬¯«œ±‚";

            UIInventory.instance.UpdateInvenSlot();
        }
    }

    public void OnClickEquipBtn()
    {
        PlayerController.Instance.EquipItem(m_invenItem);
        SetItemDetailInfo(m_invenItem);

    }
    public void OnClickEnhanceBtn()
    {

        Tables.Item itemTb = Tables.Item.Get(m_invenItem.key);
        if (itemTb != null)
        {
            EnhancementData enhanceData = EnhancementData.Get(itemTb.Enhancement + m_invenItem.enhanceCount);
            if (enhanceData != null)
            {
                InGamePrice ingamePriceTb = InGamePrice.Get(enhanceData.inGamePriceKey);
                if (ingamePriceTb != null)
                {
                    AccountManager.Instance.UseGoods((GOOD_TYPE)ingamePriceTb.demandGoodsType, ingamePriceTb.demandGoodsQty, out bool isEnougt);
                    if (!isEnougt)
                    {
                        ReleaseEnhanceBtn();
                        return;
                    }
                    isHoldding = true;
                    enhanceCount++;
                    m_invenItem.enhanceCount++;
                    SetItemDetailInfo(m_invenItem);
                }
            }
        }
    }
    public void ReleaseEnhanceBtn()
    {
        isHoldding = false;
        enhanceCount = 0;
        SetItemDetailInfo(m_invenItem);

    }
}
