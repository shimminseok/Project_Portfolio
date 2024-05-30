using System.Collections;
using System.Collections.Generic;
using Tables;
using UnityEngine;
using UnityEngine.Jobs;

public class GameManager : Singleton<GameManager>
{
    public JoystickController joystickController;
    float gameSpeed = 1;

    public int stageStep;

    public bool isAuto = true;


    public float GameSpeed { get { return gameSpeed; } }
    public void BossChallenge()
    {

    }
    public void SetGameSpeed(float _speed)
    {
        gameSpeed = _speed;
    }
    public void GetReward(Tables.Reward _rewardTb, out bool _result)
    {
        if (_rewardTb == null)
        {
            _result = false;
            return;
        }
        switch((ITEM_CATEGORY)_rewardTb.RewardItemType)
        {
            case ITEM_CATEGORY.GOODS:
                for (int i = 0; i < _rewardTb.ItemKey.Length; i++)
                {
                    AccountManager.Instance.AddGoods((GOOD_TYPE)_rewardTb.ItemKey[i], _rewardTb.ItemQty[i]);
                }
                break;
            case ITEM_CATEGORY.ITEM:
                for (int i = 0; i < _rewardTb.ItemKey.Length; i++)
                {
                    Tables.Item itemTb = Tables.Item.Get(_rewardTb.ItemKey[i]);
                    if(itemTb != null)
                    {
                        InvenItemInfo iteminfo = new InvenItemInfo();
                        iteminfo.key = _rewardTb.ItemKey[i];
                        iteminfo.count = (uint)_rewardTb.ItemQty[i];
                        AccountManager.Instance.GetEquipItem((ITEM_TYPE)itemTb.ItemType,iteminfo);
                    }
                }
                break;
        }
        _result = true;
    }

}
