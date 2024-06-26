using System.Collections.Generic;
using System.Linq;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class UIGrowth : UIPopUp
{
    public static UIGrowth instance;

    [SerializeField] List<Toggle> buttons = new List<Toggle>();
    [SerializeField] GrowthReuseScrollRect reuseScrollRect;
    int selectMultipleNum;

    public int SelectMultipleNum => selectMultipleNum;
    private void Awake()
    {
        if (instance == null)
            instance = this;
    }
    protected override void Start()
    {
        base.Start();
    }

    public override void ClosePopUp()
    {
        base.ClosePopUp();
    }

    public override void OpenPopUp()
    {
        base.OpenPopUp();
        ChildSetActive(true);
        buttons[PlayerPrefs.GetInt("GrowthMultiple", 0)].isOn = true;
        switch (PlayerPrefs.GetInt("GrowthMultiple", 0))
        {
            case 0:
                selectMultipleNum = 1;
                break;
            case 1:
                selectMultipleNum = 10;
                break;
            case 2:
                selectMultipleNum = 100;
                break;
            case 3:
                selectMultipleNum = 1000;
                break;
            default:
                break;
        }
    }

    public void SetSelectMultiple(GameObject go)
    {
        int index = go.transform.GetSiblingIndex();
        PlayerPrefs.SetInt("GrowthMultiple", index);
        switch (index)
        {
            case 0:
                selectMultipleNum = 1;
                break;
            case 1:
                selectMultipleNum = 10;
                break;
            case 2:
                selectMultipleNum = 100;
                break;
            case 3:
                selectMultipleNum = 1000;
                break;
            default:
                break;
        }
        foreach(var slot in reuseScrollRect.Cells)
        {
            GrowthListSlot gslot = slot as GrowthListSlot;
            gslot.UpdateData();
        }
        
    }



}
