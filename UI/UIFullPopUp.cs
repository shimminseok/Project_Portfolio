using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIFullPopUp : MonoBehaviour
{
    [SerializeField] List<GameObject> fullPopupGoodsBoxList;
    [SerializeField] List<Text> fullPopUpGoodsTextList;

    [SerializeField] TextMeshProUGUI titleText;

    void Start()
    {
        ChildSetActive(false);
    }
    public void ClosePopup()
    {
        ChildSetActive(false);
        switch (UIManager.Instance.popupType)
        {
            case FULL_POPUP_TYPE.SUMMON:
                UIManager.Instance.OnClickClosePopUp(UISummon.instance);
                break;
        }
    }
    public void ChildSetActive(bool _isActive)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(_isActive);
        }
    }
    public void UpdateFullPopUPGoodsBox(GOOD_TYPE _type, double _amount)
    {
        fullPopUpGoodsTextList[(int)_type - 1].text = Utility.ToCurrencyString(_amount);
    }
    public void SettingUI()
    {
        FULL_POPUP_TYPE type = UIManager.Instance.popupType;
        fullPopupGoodsBoxList.ForEach(x => x.SetActive(type != FULL_POPUP_TYPE.NONE));
        UpdateFullPopUPGoodsBox(GOOD_TYPE.GOLD,AccountManager.Instance.Gold);
        UpdateFullPopUPGoodsBox(GOOD_TYPE.DIA, AccountManager.Instance.Dia);

        switch (UIManager.Instance.popupType)
        {
            case FULL_POPUP_TYPE.NONE:
                titleText.text = string.Empty;
                break;
            case FULL_POPUP_TYPE.SUMMON:
                titleText.text = "º“»Ø";
                break;
        }
    }
}
