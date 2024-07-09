using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICharactorInfo : UIPopUp
{
    public static UICharactorInfo instance;
    [SerializeField] List<ItemSlot> equippedItemSlot =new List<ItemSlot>();
    [SerializeField] CharInfo_AbilityScrollRect abilityScrollRect;



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

        for (int i = 0; i < equippedItemSlot.Count; i++)
        {
            SetEquippingItemSlot(i);
        }
        abilityScrollRect.CreateAbilitySlot();
    }
    public override void ClosePopUp()
    {
        base.ClosePopUp();

    }


    public void SetEquippingItemSlot(int _index)
    {
        equippedItemSlot[_index].UpdateSlotByType(PlayerController.Instance.EquipmentItem[_index],false);
        equippedItemSlot[_index].IsEquipping(false);
    }
}
