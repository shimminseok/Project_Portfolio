using UI;
using UnityEngine;
using UnityEngine.UI;

public class InvenItemSlot : ReuseCellData<InvenSlotCellData>
{

    [SerializeField] ItemSlot itemSlot;
    [SerializeField] Image itemCountFillAmount;
    [SerializeField] Text itemName;
    [SerializeField] Text itemCount;


    public void SetSlotInfo(Tables.Item _item)
    {
        InvenItemInfo itemInfo = AccountManager.Instance.GetHasInvenItem(_item);
        itemName.text = UIManager.Instance.GetText(_item.ItemName);
        itemSlot.SetItemSlotInfo(SLOT_TYPE.INVENITEM, itemInfo);
        itemCount.text = string.Format("{0}/5 ", itemInfo.count);
        itemCountFillAmount.fillAmount = itemInfo.count / 5f;
    }


    public void OnClickItemSlot()
    {
        UIInventory.instance.OpenDetailItemInfoPopUp(AccountManager.Instance.GetHasInvenItem(m_data.m_ItemTb));
    }
    public void OnClickCraftingItem()
    {
        InvenItemInfo info = AccountManager.Instance.GetHasInvenItem(m_data.m_ItemTb);
        if (info != null && info.count >= 5)
        {

            //합성 가능
        }

    }
    public override void UpdateContent(InvenSlotCellData _itemData)
    {
        m_data = _itemData;
        SetSlotInfo(_itemData.m_ItemTb);
    }


}
