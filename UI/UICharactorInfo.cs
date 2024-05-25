using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICharactorInfo : UIPopUp
{
    [SerializeField] List<ItemSlot> equippedItemSlot =new List<ItemSlot>();




    private void Start()
    {
        ChildSetActive(false);
    }

    public override void OpenPopUp()
    {
        base.OpenPopUp();

        for (int i = 0; i < equippedItemSlot.Count; i++)
        {
            SetEquippengItemSlot(i);
        }


    }
    public override void ClosePopUp()
    {
        base.ClosePopUp();

    }


    public void SetEquippengItemSlot(int _index)
    {
        equippedItemSlot[_index].SetItemSlotInfo(SLOT_TYPE.INVENITEM,PlayerController.Instance.EquipmentItem[_index].key);
    }
}
