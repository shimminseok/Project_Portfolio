using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGrowth : UIPopUp
{
    public static UIGrowth instance;

    [SerializeField] ScrollRect growthSlotScollRect;
    [SerializeField] List<Toggle> buttons = new List<Toggle>();
    [SerializeField] ToggleGroup toggleGroup;
    int selectMultipleNum;

    public int SelectMultipleNum => selectMultipleNum;
    private void Awake()
    {
        if (instance == null)
            instance = this;
    }
    void Start()
    {
        ClosePopUp();
        string defalutdata = string.Empty;

        for (int i = 0; i < Tables.Ability.data.Count; i++)
        {
            AccountManager.Instance.GrowthLevelList.Add(0);
            defalutdata += 1;
            if (i < Tables.Ability.data.Count - 1)
                defalutdata += ",";
        }
        string[] dataLoad = PlayerPrefs.GetString("GrowthData", defalutdata).Split(',');
        for (int i = 0; i < AccountManager.Instance.GrowthLevelList.Count; i++)
        {
            AccountManager.Instance.GrowthLevelList[i] = int.Parse(dataLoad[i]);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
    public override void ClosePopUp()
    {
        base.ClosePopUp();
    }

    public override void OpenPopUp()
    {
        base.OpenPopUp();
        growthSlotScollRect.verticalNormalizedPosition = 0;
        ChildSetActive(true);
        selectMultipleNum = PlayerPrefs.GetInt("GrowthMultiple", 0);
        buttons[selectMultipleNum].isOn = true;
    }

    public void SetSelectMultiple(GameObject go)
    {
        int index = go.transform.GetSiblingIndex();
        PlayerPrefs.SetInt("GrowthMultiple", index);
        selectMultipleNum = index;
    }



}
