using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UI;
using Tables;
using Unity.Burst.CompilerServices;
using System;

public class CharInfo_AbilitySlot : ReuseCellData<AbilitySlotCell>
{
    [SerializeField] Image boxImg;
    [SerializeField] TextMeshProUGUI abilityName;
    [SerializeField] TextMeshProUGUI abilityValue;
    public override void UpdateContent(AbilitySlotCell _itemData)
    {
        m_data = _itemData;
        abilityName.text = UIManager.Instance.GetText(m_data.abilityTb.AbilityName);


        var statValueMap = new Dictionary<STAT, Func<double>>
    {
        { STAT.ATTACK, () => PlayerController.Instance.Damage },
        { STAT.HP, () => PlayerController.Instance.MaxHP },
        { STAT.HP_REGEN, () => PlayerController.Instance.HPRegen },
        { STAT.DEFENCE, () => PlayerController.Instance.Defense },
        { STAT.ATTACK_SPD, () => PlayerController.Instance.AttackSpd },
        { STAT.MOVE_SPD, () => PlayerController.Instance.MoveSpd },
        { STAT.CRI_DAM, () => PlayerController.Instance.CriDam },
        { STAT.CRI_RATE, () => PlayerController.Instance.CriRate },
        { STAT.HIT, () => PlayerController.Instance.Accuracy },
        { STAT.DODGE, () => PlayerController.Instance.Dodge }
    };

        double value = statValueMap.TryGetValue((STAT)m_data.abilityTb.key, out var getValue) ? getValue() : 0;
        abilityValue.text = Utility.ToCurrencyString(value);
        ColorUtility.TryParseHtmlString("#808080", out Color color);
        boxImg.color = m_data.Index % 2 == 0 ? Color.white : color;
    }


}
