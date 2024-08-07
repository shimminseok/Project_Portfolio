using UI;
using UnityEngine;
using UnityEngine.UI;

public class InvenItemSlot : ReuseCellData<InvenSlotCellData>
{

    [SerializeField] ItemSlot itemSlot;
    [SerializeField] Image itemCountFillAmount;
    [SerializeField] Text itemName;
    [SerializeField] Text itemCount;

    InvenItem m_ItemInfo;
    public void SetSlotInfo(Tables.Item _item)
    {
        m_ItemInfo = AccountManager.Instance.FindOrCreateInvenItem(_item);
        itemName.text = UIManager.Instance.GetText(_item.ItemName);


        itemSlot.UpdateSlotByType(m_ItemInfo);
        itemCount.text = $"{m_ItemInfo.count}/5";
        itemCountFillAmount.fillAmount = (float)m_ItemInfo.count / 5f;
    }


    public void OnClickItemSlot()
    {
        UIInventory.instance.OpenDetailItemInfoPopUp(AccountManager.Instance.FindOrCreateInvenItem(m_data.m_ItemTb));
    }

    public override void UpdateContent(InvenSlotCellData _itemData)
    {
        m_data = _itemData;
        SetSlotInfo(_itemData.m_ItemTb);
    }
}
