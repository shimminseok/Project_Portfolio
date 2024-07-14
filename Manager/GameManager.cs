using NPOI.SS.Formula.Functions;
using System;
using System.Linq;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public JoystickController joystickController;
    public bool isTest;

    float gameSpeed = 2.9f;
    public bool isAuto = false;
    GAME_STATE gameState;

    public float GameSpeed { get { return gameSpeed; } }

    public GAME_STATE GameState { get { return gameState; } }


    void Start()
    {
        ChangeGameState(GAME_STATE.LOADING);
        SoundManager.Instance.PlayBGMSound((int)SOUND_BGM.NO_0);
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
    public void GetReward(string _rewardKey, out bool _result, bool _isOpen)
    {
        Tables.Reward rewardTb = Tables.Reward.Get(_rewardKey);
        if (rewardTb == null)
        {
            Debug.LogWarning("Get Reward Fail");
            _result = false;
            return;
        }

        ProcessRewardItems(rewardTb.GoodsKey, rewardTb.GoodsQty, _isOpen, ProcessGoods);
        ProcessRewardItems(rewardTb.MaterialKey, rewardTb.MaterialQty, _isOpen, ProcessMaterial);
        ProcessRewardItems(rewardTb.ItemKey, rewardTb.ItemQty, _isOpen, ProcessItem);

        _result = true;

        if (_isOpen)
            UISystem.instance.OpenRewardBox("º¸»ó");

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
    void ProcessRewardItems<T>(T[] _keys, double[] _quantities, bool _isOpen, Action<T, double, bool> _processAction)
    {
        for (int i = 0; i < _keys.Length; i++)
        {
            _processAction(_keys[i], _quantities[i], _isOpen);
        }
    }
    void ProcessGoods(int _key, double _quantity, bool _isOpen)
    {
        Tables.Goods goodsTb = Tables.Goods.Get(_key);
        if (goodsTb != null)
        {
            AccountManager.Instance.AddGoods((GOOD_TYPE)_key, _quantity);
            if (_isOpen)
                AddToUISystem(_key, _quantity);
        }
    }
    void ProcessMaterial(int _key, double _quantity, bool _isOpen)
    {
        Tables.Material materialTb = Tables.Material.Get(_key);
        if (materialTb != null)
        {
            AccountManager.Instance.GetMaterial(_key, _quantity);
            if (_isOpen)
                AddToUISystem(_key, _quantity);
        }
    }

    void ProcessItem(int _key, double _quantity, bool _isOpen)
    {
        Tables.Item itemTb = Tables.Item.Get(_key);
        if (itemTb != null)
        {
            InvenItem iteminfo = new InvenItem
            {
                key = _key,
                count = _quantity
            };
            AccountManager.Instance.AddorUpdateItem((ITEM_TYPE)itemTb.ItemType, iteminfo);
            if (_isOpen)
                AddToUISystem(_key, _quantity);
        }
    }
    private void AddToUISystem(int key, double quantity)
    {
        ItemSlotCell rewardItem = new ItemSlotCell
        {
            key = key,
            count = quantity
        };
        UISystem.instance.AddItem(rewardItem);
    }
    public void EnterStage(int _stageKey)
    {
        AccountManager.Instance.CurrentStageInfo = new StageInfo(_stageKey);
        StageChange();
    }

    public int GetKey<T>(T item) where T : class
    {
        switch (item)
        {
            case Tables.Item tableItem:
                return tableItem.key;
            case Tables.Skill tableSkill:
                return tableSkill.key;
            case Tables.Material tableMat:
                return tableMat.key;
            case Tables.Goods tableGoods:
                return tableGoods.key;
            default:
                return -1;
        }
    }
}
