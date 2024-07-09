using System.Linq;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public JoystickController joystickController;
    public bool isTest;

    float gameSpeed = 2.9f;
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

        switch (gameState)
        {
            case GAME_STATE.PLAYING:
                {
                }
                break;
            case GAME_STATE.WIN:
                PlayerController.Instance.ChangeState(OBJ_ANIMATION_STATE.WIN);
                UIManager.Instance.OnClickOpenPopUp(UIStageClear.instance);
                break;
            case GAME_STATE.LOADING:
                UIManager.Instance.LoadingUISet();
                break;
            case GAME_STATE.END:
                UIManager.Instance.OnClickOpenPopUp(UIDeath.instance);
                break;
        }
    }
    public void StageChange()
    {
        ChangeGameState(GAME_STATE.LOADING);
        UIManager.Instance.SetStageName(AccountManager.Instance.CurrentStageInfo.key);
        MapManager.Instance.Init();
        MonsterManager.instance.Init();
        PlayerController.Instance.Init();
    }
    public void SetGameSpeed(float _speed)
    {
        gameSpeed = _speed;
    }
    public void GetReward(string _rewardKey, out bool _result)
    {
        Tables.Reward rewardTb = Tables.Reward.Get(_rewardKey);
        if (rewardTb == null)
        {
            Debug.LogWarning("Get Reward Fail");
            _result = false;
            return;
        }
        for (int i = 0; i < rewardTb.GoodsKey.Length; i++)
        {
            Tables.Goods goodsTb = Tables.Goods.Get(rewardTb.GoodsKey[i]);
            if (goodsTb != null)
            {
                AccountManager.Instance.AddGoods((GOOD_TYPE)rewardTb.GoodsKey[i], rewardTb.GoodsQty[i]);
            }
        }
        for (int i = 0; i < rewardTb.MaterialKey.Length; i++)
        {
            Tables.Material materialTb = Tables.Material.Get(rewardTb.MaterialKey[i]);
            if (materialTb != null)
            {
                AccountManager.Instance.AddMaterial(rewardTb.MaterialKey[i], rewardTb.MaterialQty[i]);
            }
        }
        for (int i = 0; i < rewardTb.ItemKey.Length; i++)
        {
            Tables.Item itemTb = Tables.Item.Get(rewardTb.ItemKey[i]);
            if (itemTb != null)
            {
                InvenItem iteminfo = new InvenItem();
                iteminfo.key = rewardTb.ItemKey[i];
                iteminfo.count = (uint)rewardTb.ItemQty[i];
                AccountManager.Instance.GetEquipItem((ITEM_TYPE)itemTb.ItemType, iteminfo);
            }
        }
        _result = true;
    }

    public int GetNextStage()
    {
        bool check = false;
        foreach (var key in Tables.Stage.data.Keys)
        {
            if (AccountManager.Instance.CurrentStageInfo.key == Tables.Stage.data.Last().Key)
                return Tables.Stage.data.Last().Key;

            if (check)
                return key;

            if (key == AccountManager.Instance.CurrentStageInfo.key)
                check = true;
        }
        return AccountManager.Instance.CurrentStageInfo.key;
    }
    public void EnterStage(int _stageKey)
    {
        AccountManager.Instance.CurrentStageInfo = new StageInfo(_stageKey);
    }
}
