using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    [Header("BottomAnchor")]
    [SerializeField] Image HpBarImg;
    void Start()
    {
        
    }

    void Update()
    {
        
    }


    public void UpdateHPBarUI(float _max, float _cur)
    {
        HpBarImg.fillAmount = _cur / _max;
    }
}
