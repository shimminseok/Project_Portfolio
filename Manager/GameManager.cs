using UnityEngine;
using System.Linq;

public class GameManager : Singleton<GameManager>
{
    public JoystickController joystickController;


    float gameSpeed = 1;
    public bool isAuto = true;
    GAME_STATE gameState;

    public float GameSpeed { get { return gameSpeed; } }

    public GAME_STATE GameState { get { return gameState; } }

    void Start()
    {
        ChangeGameState(GAME_STATE.LOADING);
    }

    public void ChangeGameState(GAME_STATE _state)
    {
        gameState = _state;

        switch(gameState)
        {
            case GAME_STATE.PLAYING:
                {
                }
                break;
            case GAME_STATE.WIN:
                UIStageClear.instance.StageClear();
                break;
            case GAME_STATE.LOADING:
                UIManager.Instance.LoadingUISet();
                break;
        }
    }
    public void SetGameSpeed(float _speed)
    {
        gameSpeed = _speed;
    }
    public void GetReward(Tables.Reward _rewardTb, out bool _result)
    {
        if (_rewardTb == null)
        {
            Debug.LogWarning("Get Reward Fail");
            _result = false;
            return;
        }
        for (int i = 0; i < _rewardTb.RewardItemType.Length; i++)
        {
            switch ((ITEM_CATEGORY)_rewardTb.RewardItemType[i])
            {
                case ITEM_CATEGORY.GOODS:
                    for (int j = 0; j < _rewardTb.ItemKey.Length; j++)
                    {
                        AccountManager.Instance.AddGoods((GOOD_TYPE)_rewardTb.ItemKey[j], _rewardTb.ItemQty[j]);
                    }
                    break;
                case ITEM_CATEGORY.ITEM:
                    for (int j = 0; j < _rewardTb.ItemKey.Length; j++)
                    {
                        Tables.Item itemTb = Tables.Item.Get(_rewardTb.ItemKey[j]);
                        if (itemTb != null)
                        {
                            InvenItemInfo iteminfo = new InvenItemInfo();
                            iteminfo.key = _rewardTb.ItemKey[j];
                            iteminfo.count = (uint)_rewardTb.ItemQty[j];
                            AccountManager.Instance.GetEquipItem((ITEM_TYPE)itemTb.ItemType, iteminfo);
                        }
                    }
                    break;
            }
        }
        _result = true;
    }

    public int GetNextStage()
    {
        bool check = false;
        foreach(var key in Tables.Stage.data.Keys)
        {
            if (AccountManager.Instance.CurStageKey == Tables.Stage.data.Last().Key)
                return Tables.Stage.data.Last().Key;

            if (check)
                return key;

            if(key == AccountManager.Instance.CurStageKey)
                check = true;
        }
        return AccountManager.Instance.CurStageKey;
    }


}
