using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    [Header("BottomAnchor")]
    [SerializeField] Image HpBarImg;

    [Header("RightAnchor")]
    [SerializeField] Image AutoButton;
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
    #region[Button Event]
    public void OnClickBossChallenge()
    {

    }
    public void OnClickAuto()
    {
        GameManager.Instance.isAuto = !GameManager.Instance.isAuto;
        AutoButton.color = GameManager.Instance.isAuto ? Color.red : Color.white;
    }
    #endregion
}
