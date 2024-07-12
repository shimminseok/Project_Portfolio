using System.Collections;
using System.Collections.Generic;
using UI;
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
    

    int currentItemTab = 0;



    void Awake()
    {
        if(instance == null)
            instance = this;
    }
    protected override void Start()
    {
        base.Start();
    }

    public override void OpenPopUp()
    {
        base.OpenPopUp();
        OnClickItemTabToggle(itemTypeTabToggle[currentItemTab].transform);

    }
    public override void ClosePopUp()
    {
        base.ClosePopUp();
        if (itemDetailPopup.IsOpenPopup)
        {
            itemDetailPopup.ClosePopUp();
            UIManager.Instance.OnClickOpenPopUp(this);
        }
    }



    public void OnClickItemTabToggle(Transform _go)
    {
        currentItemTab = itemTypeTabToggle.FindIndex(x => x.name == _go.name);
        invenitemScollRect.CreateInventoryListSlot((ITEM_TYPE)currentItemTab);
        for (int i = 0; i < itemTypeTabText.Count; i++)
        {
            itemTypeTabText[i].color = currentItemTab == i ? Color.white : Color.gray;
        }
        if(itemDetailPopup.IsOpenPopup)
        {
            itemDetailPopup.ClosePopUp();

        }

        scrollRect.StopMovement();
    }
    public void UpdateInvenSlot()
    {
        invenitemScollRect.UpdateAllCells();
    }


    public void OpenDetailItemInfoPopUp(InvenItem _itemInfo)
    {
        itemDetailPopup.SetInvenItem(_itemInfo);
        itemDetailPopup.OpenPopUp();
    }
    public void OnClickAllSynthesisItem()
    {
        for (int i = 0; i < AccountManager.Instance.HasItemDictionary[(ITEM_TYPE)currentItemTab].Count; i++)
        {
            AccountManager.Instance.HasItemDictionary[(ITEM_TYPE)currentItemTab][i].SynthesisItem();
        }
        UpdateInvenSlot();
        UISystem.instance.OpenRewardBox("아이템 합성");
    }
}
