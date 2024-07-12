using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Xml.Serialization;

public class UIDeath : UIPopUp
{
    public static UIDeath instance;

    [SerializeField] TextMeshProUGUI stageNameTxt;

    void Awake()
    {
        if (instance == null)
            instance = this;
    }
    protected override void Start()
    {
        base.Start();
    }
    public override void OpenPopUp()
    {
        base.OpenPopUp();
        stageNameTxt.text = UIManager.Instance.ReturnCurrentStageName();

        //5몇초후에 스테이지 리셋
        StartCoroutine("EnterStage");

    }
    public override void ClosePopUp()
    {
        StopCoroutine("EnterStage");
        base.ClosePopUp();
    }
    public IEnumerator EnterStage()
    {
        yield return new WaitForSeconds(5);
        UIManager.Instance.OnClickClosePopUp(this);
        GameManager.Instance.EnterStage(AccountManager.Instance.CurrentStageInfo.key);

    }

    public void OnClickShortcutBtn(UIPopUp _popup)
    {
        UIManager.Instance.OnClickOpenPopUp(_popup);
    }

}
