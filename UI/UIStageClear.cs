using NPOI.SS.Formula.Functions;
using System.Collections;
using System.Collections.Generic;
using Tables;
using TMPro;
using UnityEngine;

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
        AccountManager.Instance.BestStageInfo = new StageInfo(GameManager.Instance.GetNextStage());
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
        GameManager.Instance.GetReward(MonsterManager.instance.CurrentStageTb.StageClearReward, out bool result, false);
        Tables.Reward rewardTb = Tables.Reward.Get(MonsterManager.instance.CurrentStageTb.StageClearReward);
        if (result)
        {
            int count = 0;
            if (rewardTb != null)
            {

                count = SetSlotInfo(rewardTb.GoodsKey, rewardTb.GoodsQty, count);
                count = SetSlotInfo(rewardTb.MaterialKey, rewardTb.MaterialQty, count);
                count = SetSlotInfo(rewardTb.ItemKey, rewardTb.ItemQty, count);
                while (count < rewardItemList.Count)
                {
                    rewardItemList[count++].gameObject.SetActive(false);
                }
            }
        }
    }

    int SetSlotInfo(int[] _keys, double[] _count,int _startIndex)
    {
        for (int i = 0; i < _keys.Length; i++)
        {
            rewardItemList[i].gameObject.SetActive(true);
            ItemSlotCell rewardInfo = new ItemSlotCell() { key = _keys[i], count = _count[i] };
            rewardItemList[i].UpdateSlot(rewardInfo);
            rewardItemList[i].ActiveNotiImg(false);
            rewardItemList[i].ActiveNotGetImg(false);
            _startIndex++;
        }
        return _startIndex;
    }

    void JoinNextStage()
    {
        GameManager.Instance.EnterStage(GameManager.Instance.GetNextStage());
        ClosePopUp();
    }
}
