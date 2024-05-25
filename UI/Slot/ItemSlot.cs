using Newtonsoft.Json.Bson;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemSlot : MonoBehaviour
{
    [SerializeField] Image itemGradeBG;
    [SerializeField] Image itemIconImg;
    [SerializeField] Image qualityImg;
    [SerializeField] TextMeshProUGUI enhanceCountTxt;
    [SerializeField] TextMeshProUGUI hasCountTxt;
    [SerializeField] GameObject notGetImg;
    [SerializeField] GameObject equippingTxt;


   public void SetItemSlotInfo(SLOT_TYPE _type, int _key)
    {
        switch (_type)
        {
            case SLOT_TYPE.INVENITEM:
                SetInvenItemSlot(Tables.Item.Get(_key));
                break;
            case SLOT_TYPE.SUMMON_RESULT:
                SetSummonResultItemSlot(Tables.Item.Get(_key));
                break;
            default:
                break;
        }

    }

    void SetInvenItemSlot(Tables.Item _itemTb)
    {
        if (EmptySlot(_itemTb == null))
            return;
        SetItemSlot(_itemTb);

        InvenItemInfo infoSlot = AccountManager.Instance.GetHasInvenItem(_itemTb);
        notGetImg.SetActive(infoSlot.count == 0);
        enhanceCountTxt.text = string.Format("Lv.{0}", infoSlot.enhanceCount);
        enhanceCountTxt.gameObject.SetActive(infoSlot.enhanceCount > 0);
        equippingTxt.gameObject.SetActive(infoSlot.isEquipped);
        hasCountTxt.gameObject.SetActive(false);
    }
    void SetSummonResultItemSlot(Tables.Item _itemTb)
    {
        if (EmptySlot(_itemTb == null))
            return;

        SetItemSlot(_itemTb);
        InvenItemInfo infoSlot = AccountManager.Instance.GetHasInvenItem(_itemTb);
        notGetImg.SetActive(false);
        hasCountTxt.text = $"x{ infoSlot.count}";
        equippingTxt.gameObject.SetActive(false);
        enhanceCountTxt.gameObject.SetActive(false);
    }
    void SetItemSlot(Tables.Item _itemTb)
    {
        itemGradeBG.sprite = UIManager.Instance.GetSprite(SPRITE_TYPE.ITEM_GRADE, string.Format("item_bg_00{0}", _itemTb.ItemGrade));
        qualityImg.sprite = UIManager.Instance.GetSprite(SPRITE_TYPE.ITEM_GRADE, string.Format("icon_class_a00{0}", _itemTb.Quality_Grade));
        itemIconImg.sprite = UIManager.Instance.GetSprite(SPRITE_TYPE.ITEM_ICON, _itemTb.ItemIcon);
    }
    bool EmptySlot(bool _isEmpty)
    {

        itemIconImg.gameObject.SetActive(!_isEmpty);
        qualityImg.gameObject.SetActive(!_isEmpty);
        notGetImg.gameObject.SetActive(!_isEmpty);
        enhanceCountTxt.gameObject.SetActive(!_isEmpty);
        equippingTxt.gameObject.SetActive(!_isEmpty);
        enhanceCountTxt.gameObject.SetActive(!_isEmpty);

        return _isEmpty;
    }
}
