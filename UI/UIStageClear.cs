using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIStageClear : UIPopUp
{
    public static UIStageClear instance;

    [SerializeField] TextMeshProUGUI nextStageCountTxt;
    [SerializeField] List<ItemSlot> rewardItemList = new List<ItemSlot>();
    float nextStageTime;

    Coroutine autoNextStage;
    void Awake()
    {
        if (instance == null)
            instance = this;
    }
    public override void OpenPopUp()
    {
        base.OpenPopUp();
        StageClear();
    }
    public override void ClosePopUp()
    {
        base.ClosePopUp();
    }
    public void StageClear()
    {
        autoNextStage = StartCoroutine(StartAutoNextStage());
        SetRewardItem();
    }
    public IEnumerator StartAutoNextStage()
    {
        nextStageTime = 3f;
        while (nextStageTime > 0)
        {
            nextStageCountTxt.text = $"{nextStageTime}초 후 자동 도전";
            yield return new WaitForSeconds(1f);
            nextStageTime -= 1f;
        }
        JoinNextStage();
    }
    public void OnClickChallengeBtn()
    {
        StopCoroutine(autoNextStage);
        JoinNextStage();
    }
    public void SetRewardItem()
    {
        Tables.Reward rewardTb = Tables.Reward.Get(MonsterManager.instance.currentStageTb.StageClearReward);
        int count = 0;
        if(rewardTb != null )
        {
            for (int i = 0; i < rewardTb.ItemKey.Length; i++)
            {
                rewardItemList[i].gameObject.SetActive(true);
                InvenItemInfo rewardInfo = new InvenItemInfo() { key = rewardTb.ItemKey[i], count = rewardTb.ItemQty[i] };
                rewardItemList[i].SetItemSlotInfo(SLOT_TYPE.REWARD, rewardInfo);
                rewardItemList[i].ActiveNotiImg(false);
                rewardItemList[i].ActiveNotGetImg(false);
                count++;
            }
            while (count < rewardItemList.Count)
            {
                rewardItemList[count++].gameObject.SetActive(false);
            }
        }
        GameManager.Instance.GetReward(rewardTb, out bool result);
        if (result)
        {

        }
    }

    void JoinNextStage()
    {
        AccountManager.Instance.CurStageKey = GameManager.Instance.GetNextStage();
        ClosePopUp();
        GameManager.Instance.ChangeGameState(GAME_STATE.LOADING);
    }
}
