using Spine.Unity;
using System.Collections;
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

    [SerializeField] SkeletonGraphic itemGradeEffect;

    int targetGrade = 1;
    string[] itemGradeTeduriEffectName = new string[5] { "gold","pink","purple","red","white"};

    private void Start()
    {
        itemGradeEffect.Initialize(true);
    }
    public void UpdateSlotByType(object _itemObj, bool _isActiveUI = true)
    {
        switch (_itemObj)
        {
            case InvenItem equipmentItem:
                UpdateSlot(equipmentItem, _isActiveUI);
                break;
            case MaterialItem materialItem:
                UpdateSlot(materialItem);
                break;
            case SkillItem skillItem:
                UpdateSlot(skillItem);
                break;
            case MonsterItem monsterItem:
                UpdateSlot(monsterItem);
                break;
        }
    }
    public void UpdateSlot(ItemSlotCell _item)
    {
        itemIconImg.sprite = _item.GetSprite();
        itemGradeBG.sprite = UIManager.Instance.GetSprite(SPRITE_TYPE.ITEM_GRADE, $"item_bg_00{_item.itemGrade}");

        hasCountTxt.text = $"x{Utility.ToCurrencyString(_item.count)}";

        qualityImg.gameObject.SetActive(false);
        enhanceCountTxt.gameObject.SetActive(false);
        IsEquipping(false);
        ActiveNotiImg(false);
        ActiveNotGetImg(false);
    }
    void UpdateSlot(InvenItem _item, bool _isActiveUI)
    {
        if (_item.key == 0)
        {
            EmptySlot();
            return;
        }
        qualityImg.gameObject.SetActive(true);
        itemIconImg.gameObject.SetActive(true);


        itemIconImg.sprite = _item.GetSprite();
        targetGrade = _item.m_Table.ItemGrade;
        itemGradeBG.sprite = UIManager.Instance.GetSprite(SPRITE_TYPE.ITEM_GRADE, $"item_bg_00{targetGrade}");
        qualityImg.sprite = UIManager.Instance.GetSprite(SPRITE_TYPE.ITEM_GRADE, $"icon_class_a00{_item.m_Table?.Quality_Grade}");

        hasCountTxt.text = _isActiveUI ? $"x{Utility.ToCurrencyString(_item.count)}" : string.Empty;
        enhanceCountTxt.text = _item.enhanceCount > 0 ? $"Lv.{_item.enhanceCount}" : string.Empty;
        ActiveNotGetImg(!_item.isGet && _isActiveUI);
        IsEquipping(_item.isEquipped && _isActiveUI);
        ActiveNotiImg(_item.count >= 5 && _isActiveUI);
    }
    void UpdateSlot(MaterialItem _item)
    {
        itemIconImg.sprite = _item.GetSprite();
        qualityImg.gameObject.SetActive(false);
        hasCountTxt.text = _item.count > 1 ? $"x{Utility.ToCurrencyString(_item.count)}" : string.Empty;
        enhanceCountTxt.text = _item.enhanceCount > 0 ? $"Lv.{_item.enhanceCount}" : string.Empty;
    }
    void UpdateSlot(SkillItem _item)
    {
        itemIconImg.sprite = _item.GetSprite();
        qualityImg.gameObject.SetActive(false);
        hasCountTxt.text = string.Empty;
        enhanceCountTxt.text = _item.enhanceCount > 0 ? $"Lv.{_item.enhanceCount}" : string.Empty;
        targetGrade = _item.m_Table.SkillGrade;
        itemGradeBG.sprite = UIManager.Instance.GetSprite(SPRITE_TYPE.ITEM_GRADE, $"item_bg_00{targetGrade}");
        IsEquipping(false);
        ActiveNotiImg(false);
        ActiveNotGetImg(false);
    }
    void UpdateSlot(MonsterItem _item)
    {
        itemIconImg.sprite = _item.GetSprite();
        qualityImg.gameObject.SetActive(false);
        hasCountTxt.text = string.Empty;
        enhanceCountTxt.text = _item.enhanceCount > 0 ? $"Lv.{_item.enhanceCount}" : string.Empty;
    }
    void EmptySlot()
    {
        targetGrade = 1;
        itemGradeBG.sprite = UIManager.Instance.GetSprite(SPRITE_TYPE.ITEM_GRADE, "item_bg_001");
        itemIconImg.gameObject.SetActive(false);
        qualityImg.gameObject.SetActive(false);
        enhanceCountTxt.text = string.Empty;
        hasCountTxt.text = string.Empty;
        IsEquipping(false);
        ActiveNotiImg(false);
        ActiveNotGetImg(false);
        itemGradeEffect.gameObject.SetActive(false);
    }
    public void IsEquipping(bool _isEquipping)
    {
        equippingTxt.SetActive(_isEquipping);
    }
    public void ActiveNotiImg(bool _isActive)
    {
        notiImg.SetActive(_isActive);
    }
    public void ActiveNotGetImg(bool _isActive)
    {
        notGetImg.SetActive(_isActive);
    }
    public void SetGradeEffectOn(float _delay = 0f, bool _isOpenEffect = true)
    {
        if (gameObject.activeInHierarchy)
            StartCoroutine(EffectAniActive(_delay, _isOpenEffect));

    }
    IEnumerator EffectAniActive(float _delay, bool _isOpenEffect)
    {
        yield return new WaitForSeconds(_delay);

        if (targetGrade >= 3)
        {
            if (_isOpenEffect)
            {
                UIManager.Instance.SetSkeletonAnimation(itemGradeEffect, itemGradeTeduriEffectName[targetGrade - 3], true);
            }
        }
        else
            itemGradeEffect.gameObject.SetActive(false);
    }
}
