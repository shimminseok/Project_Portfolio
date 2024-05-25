using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIInventory : UIPopUp
{
    public static UIInventory instance;

    [SerializeField] ScrollRect scrollRect;
    [SerializeField] InventoryReuseScrollRect invenitemScollRect;
    [SerializeField] List<Toggle> itemTypeTabToggle;
    [SerializeField] List<Text> itemTypeTabText;

    [Header("DetailItemPopup")]
    [SerializeField] PopupItemDetail itemDetailPopup;
    

    int currentItemTab;
    void Awake()
    {
        if(instance == null)
            instance = this;
    }
    private void Start()
    {
        ClosePopUp();
    }

    public override void OpenPopUp()
    {
        base.OpenPopUp();
        OnClickItemTabToggle(itemTypeTabToggle[0].transform);

    }
    public override void ClosePopUp()
    {
        base.ClosePopUp();
    }



    public void OnClickItemTabToggle(Transform _go)
    {
        currentItemTab = itemTypeTabToggle.FindIndex(x => x.name == _go.name);
        invenitemScollRect.CreateInventoryListSlot((ITEM_TYPE)currentItemTab);
        for (int i = 0; i < itemTypeTabText.Count; i++)
        {
            itemTypeTabText[i].color = currentItemTab == i ? Color.white : Color.gray;
        }
        if(UIManager.Instance.openedPopupList.Contains(itemDetailPopup))
        {
            UIManager.Instance.OnClickClosePopUp(itemDetailPopup);
            itemDetailPopup.ClosePopUp();
        }

        scrollRect.StopMovement();
        scrollRect.content.anchoredPosition = Vector2.zero;
    }


    public void OpenDetailItemInfoPopUp(InvenItemInfo _itemInfo)
    {
        itemDetailPopup.m_invenItem = _itemInfo;
        UIManager.Instance.OnClickOpenPopUp(itemDetailPopup);
    }

}
