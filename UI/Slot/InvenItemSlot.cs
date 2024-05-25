using UI;
using UnityEngine;
using UnityEngine.UI;

public class InvenItemSlot : ReuseCellData<InvenSlotCellData>
{

    [SerializeField] ItemSlot itemSlot;
    [SerializeField] Image itemCountFillAmount;
    [SerializeField] Text itemName;
    [SerializeField] Text itemCount;


    InvenSlotCellData cellData;
    public void SetSlotInfo(Tables.Item _item)
    {
        InvenItemInfo itemInfo = AccountManager.Instance.GetHasInvenItem(_item);
        itemName.text = UIManager.Instance.GetText(_item.ItemName);
        itemSlot.SetItemSlotInfo(SLOT_TYPE.INVENITEM, _item.key);
        itemCount.text = string.Format("{0}/5 ", itemInfo.count);
        itemCountFillAmount.fillAmount = itemInfo.count / 5f;
    }


    public void OnClickItemSlot()
    {
        UIInventory.instance.OpenDetailItemInfoPopUp(AccountManager.Instance.GetHasInvenItem(cellData.m_ItemTb));
    }
    public void OnClickCraftingItem()
    {
        InvenItemInfo info = AccountManager.Instance.GetHasInvenItem(cellData.m_ItemTb);
        if (info != null && info.count >= 5)
        {

            //�ռ� ����
        }

    }
    public override void UpdateContent(InvenSlotCellData _itemData)
    {
        cellData = _itemData;
        SetSlotInfo(_itemData.m_ItemTb);
    }


}
