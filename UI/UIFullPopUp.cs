using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIFullPopUp : UIPopUp
{
    [SerializeField] List<GameObject> fullPopupGoodsBoxList;
    [SerializeField] List<Text> fullPopUpGoodsTextList;

    void Start()
    {
        ChildSetActive(false);
    }

    public override void OpenPopUp()
    {
        UIManager.Instance.CloseAllPopUp();
        base.OpenPopUp();
        UIManager.Instance.isFullPopUp = true;
    }
    public override void ClosePopUp()
    {
        base.ClosePopUp();
        UIManager.Instance.isFullPopUp = false;
    }


    public void UpdateFullPopUPGoodsBox(GOOD_TYPE _type, uint _amount)
    {
        fullPopUpGoodsTextList[(int)_type].text = AccountManager.Instance.ToCurrencyString(_amount);
    }

    public void OnEnableGoodsBox()
    {
        
    }
}
