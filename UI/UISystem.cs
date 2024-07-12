using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class UISystem : MonoBehaviour
{
    public static UISystem instance;

    [Header("SystemMsg")]
    [SerializeField] TextMeshProUGUI systemMsg;
    [Header("RewardItem")]
    [SerializeField] GameObject rewardBoxObj;
    [SerializeField] List<ItemSlot> rewardSlots;
    [SerializeField] TextMeshProUGUI getRewardTxt;


    void Awake()
    {
        if (instance == null)
            instance = this;
    }
    int rewardCnt = 0;
    public void SetSystemMessage(string _message, UnityAction _action = null)
    {
        systemMsg.text = _message;
        StartCoroutine(TweenManager.Instance.FadeIn(systemMsg.transform.parent.gameObject, 1, 0, 0, () =>
        {
            StartCoroutine(TweenManager.Instance.FadeOut(systemMsg.transform.parent.gameObject, 0, 1f, 0.5f, _action));
        }));
    }


    public void AddItem(ItemSlotCell _cell)
    {
        rewardSlots[rewardCnt++].UpdateSlot(_cell);
    }
    public void OpenRewardBox(string _desc)
    {
        if (rewardCnt == 0)
            return;

        StopAllCoroutines();
        rewardBoxObj.SetActive(true);
        getRewardTxt.text = _desc;
        for (int i = 0; i < rewardSlots.Count; i++)
        {
            rewardSlots[i].gameObject.SetActive(i < rewardCnt);
        }
        rewardCnt = 0;

        StartCoroutine(TweenManager.Instance.FadeIn(rewardBoxObj, 1, 0, 0, () =>
        {
            StartCoroutine(TweenManager.Instance.FadeOut(rewardBoxObj, 0, 3f, 0.5f, () =>
            {
            }));
        }));
    }
}
