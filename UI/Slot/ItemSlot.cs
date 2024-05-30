using Tables;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    [SerializeField] Image itemGradeBG;
    [SerializeField] Image itemIconImg;
    [SerializeField] Image qualityImg;
    [SerializeField] TextMeshProUGUI enhanceCountTxt;
    [SerializeField] TextMeshProUGUI hasCountTxt;
    [SerializeField] GameObject equippingTxt;
    [SerializeField] GameObject notGetImg;
    [SerializeField] GameObject notiImg;


    public void SetItemSlotInfo(SLOT_TYPE _type, InvenItemInfo _info)
    {
        switch (_type)
        {
            case SLOT_TYPE.INVENITEM:
                SetInvenItemSlot(_info);
                break;
            case SLOT_TYPE.SUMMON_RESULT:
                SetSummonResultItemSlot(_info);
                break;
            case SLOT_TYPE.REWARD:
                SetRewardItemSlot(_info.key,_info.count);
                break;
            default:
                break;
        }
    }

    void SetInvenItemSlot(InvenItemInfo _info)
    {
        Tables.Item itemTb = Item.Get(_info.key);
        if (EmptySlot(itemTb == null))
            return;
        SetItemSlot(itemTb);

        enhanceCountTxt.text = string.Format("Lv.{0}", _info.enhanceCount);
        enhanceCountTxt.gameObject.SetActive(_info.enhanceCount > 0);
        equippingTxt.gameObject.SetActive(_info.isEquipped);
        hasCountTxt.gameObject.SetActive(false);
        ActiveNotGetImg(_info.count == 0);
        ActiveNotiImg(false);
    }
    void SetSummonResultItemSlot(InvenItemInfo _info)
    {
        Tables.Item itemTb = Item.Get(_info.key);
        if (EmptySlot(itemTb == null))
            return;

        SetItemSlot(itemTb);

        hasCountTxt.text = $"x{_info.count}";
        equippingTxt.gameObject.SetActive(false);
        enhanceCountTxt.gameObject.SetActive(false);
        ActiveNotGetImg(false);
        ActiveNotiImg(false);
    }
    void SetGoodsSlot(Tables.Goods _goods, uint _count)
    {
        if (_goods != null)
        {
            itemIconImg.sprite = UIManager.Instance.GetSprite(SPRITE_TYPE.ITEM_ICON, _goods.GoodsIcon);
            itemGradeBG.sprite = UIManager.Instance.GetSprite(SPRITE_TYPE.ITEM_GRADE, "item_bg_001");
            hasCountTxt.text = $"x{_count}";

            qualityImg.gameObject.SetActive(false);
            enhanceCountTxt.gameObject.SetActive(false);
            equippingTxt.SetActive(false);
        }
    }
    void SetRewardItemSlot(int _key, uint _count)
    {
        if (_key < 100)
        {
            Tables.Goods goods = Tables.Goods.Get(_key);
            if(goods != null)
                SetGoodsSlot(goods, _count);
        }
        else if (_key < 1000)
        {

        }
        else if (_key < 10000)
        {

        }
        else if (_key < 100000)
        {
            Tables.Item itemTb = Tables.Item.Get(_key);
            if (itemTb != null)
                SetItemSlot(itemTb);
        }
    }
    void SetItemSlot(Tables.Item _itemTb)
    {
        qualityImg.gameObject.SetActive(true);
        enhanceCountTxt.gameObject.SetActive(true);
        equippingTxt.SetActive(true);

        itemGradeBG.sprite = UIManager.Instance.GetSprite(SPRITE_TYPE.ITEM_GRADE, string.Format("item_bg_00{0}", _itemTb.ItemGrade));
        itemIconImg.sprite = UIManager.Instance.GetSprite(SPRITE_TYPE.ITEM_ICON, _itemTb.ItemIcon);


        qualityImg.sprite = UIManager.Instance.GetSprite(SPRITE_TYPE.ITEM_GRADE, string.Format("icon_class_a00{0}", _itemTb.Quality_Grade));
    }
    bool EmptySlot(bool _isEmpty)
    {

        itemIconImg.gameObject.SetActive(!_isEmpty);
        qualityImg.gameObject.SetActive(!_isEmpty);
        enhanceCountTxt.gameObject.SetActive(!_isEmpty);
        equippingTxt.gameObject.SetActive(!_isEmpty);
        ActiveNotiImg(!_isEmpty);
        ActiveNotGetImg(!_isEmpty);
        return _isEmpty;
    }

    public void ActiveNotiImg(bool _isActive)
    {
        notiImg.SetActive(_isActive);
    }
    public void ActiveNotGetImg(bool _isActive)
    {
        notGetImg.SetActive(_isActive);
    }
}
