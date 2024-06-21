using System.Collections.Generic;
using System.Linq;
using UnityEngine;



public class AccountManager : Singleton<AccountManager>
{
    int playerLevel = 100;


    StageInfo currentStageInfo = new StageInfo();
    StageInfo bestStageInfo = new StageInfo();



    Dictionary<ITEM_TYPE, List<InvenItemInfo>> hasItemDictionary = new Dictionary<ITEM_TYPE, List<InvenItemInfo>>();
    Dictionary<int, MaterialInfo> hasMaterialDictionary = new Dictionary<int, MaterialInfo>();
    double gold = 0;
    double dia = 0;
    public int PlayerLevel { get { return playerLevel; } set => playerLevel = value; }
    public StageInfo BestStageInfo
    {
        get => bestStageInfo;
        set
        {
            bestStageInfo = value;
        }
    }
    public StageInfo CurrentStageInfo
    {
        get => currentStageInfo;


        set
        {
            currentStageInfo = value;
            GameManager.Instance.StageChange();
        }
    }

    public double Total_Atk
    {
        //Player의 모든 공격력 관련을 종합 계산
        get
        {
            //공격력
            double total = 0;
            total += CalculateTotalAbility(STAT.ATTACK);
            //크리확률
            total += CalculateTotalAbility(STAT.CRI_RATE);
            //크리데미지
            total += CalculateTotalAbility(STAT.CRI_DAM);
            //공격속도
            total += CalculateTotalAbility(STAT.ATTACK_SPD);

            return total;
        }
    }
    public double Total_Def
    {
        //Player의 모든 방어력 관련된 항목을 종합 계산
        get
        {
            //체력
            double total = 0;
            total += CalculateTotalAbility(STAT.HP);
            //방어력
            total += CalculateTotalAbility(STAT.DEFENCE);
            //명중률
            total += CalculateTotalAbility(STAT.HIT);
            //회피
            total += CalculateTotalAbility(STAT.DODGE);

            return total;
        }
    }

    public Dictionary<ITEM_TYPE, List<InvenItemInfo>> HasItemDictionary
    {
        get { return hasItemDictionary; }
        set { hasItemDictionary = value; }
    }

    int[] summonCount = Enumerable.Repeat(0, 4).ToArray();
    int[] summonRewardLevel = Enumerable.Repeat(1, 4).ToArray();

    public int[] SummonCount => summonCount;
    public int[] SummonRewardLevel => summonRewardLevel;
    public double Gold
    {
        get { return gold; }
        set
        {
            gold = value;
            UIManager.instance.UpdateGoodText(GOOD_TYPE.GOLD, gold);
        }
    }
    public double Dia
    {
        get { return dia; }
        set
        {
            dia = value;
            UIManager.instance.UpdateGoodText(GOOD_TYPE.DIA, dia);
        }
    }
    void Start()
    {
        LoadData();
    }

    public void UseGoods(GOOD_TYPE _type, ulong _amount, out bool _isEnough)
    {
        switch (_type)
        {
            case GOOD_TYPE.DIA:
                if (dia < _amount)
                {
                    _isEnough = false;
                    return;
                }
                Dia -= _amount; // 다이아 사용
                break;
            case GOOD_TYPE.GOLD:
                if (gold < _amount)
                {
                    _isEnough = false;
                    return;
                }
                Gold -= _amount; // 골드 사용
                break;
        }
        _isEnough = true;

    }
    public void AddGoods(GOOD_TYPE _type, double _amount)
    {
        switch (_type)
        {
            case GOOD_TYPE.DIA:
                Dia += _amount;
                break;
            case GOOD_TYPE.GOLD:
                Gold += _amount;
                break;
        }
    }
    public void AddMaterial(int _matKey, double _amount)
    {
        if (hasMaterialDictionary.TryGetValue(_matKey, out var material))
        {
            material.count += _amount;
        }
        else
        {
            MaterialInfo matinfo = new MaterialInfo(_matKey, _amount);
            hasMaterialDictionary.Add(_matKey, matinfo);
        }
    }



    public InvenItemInfo GetHasInvenItem(Tables.Item _item)
    {
        InvenItemInfo invenItem = new InvenItemInfo();
        if (hasItemDictionary.TryGetValue((ITEM_TYPE)_item.ItemType, out List<InvenItemInfo> list))
        {
            invenItem = list.Find(x => x.key == _item.key);
            if (invenItem != null)
            {
                return invenItem;
            }
            else
            {
                invenItem.key = _item.key;
            }
        }
        else
        {
            invenItem.key = _item.key;
        }
        return invenItem;
    }

    public void SummonCountUp(SUMMON_TYPE _type)
    {
        summonCount[(int)_type]++;
    }
    public int GetSummonLevel(SUMMON_TYPE _type)
    {
        string tbKey = string.Empty;
        double count = summonCount[(int)_type];
        uint demandCnt = 0;
        int level = 1;
        while (count >= demandCnt)
        {
            switch (_type)
            {
                case SUMMON_TYPE.WEAPONE:
                    tbKey = string.Format("ItemGachaLvCost_{0}", level);
                    break;
                case SUMMON_TYPE.ARMOR:
                    tbKey = string.Format("DefensiveGachaLvCost_{0}", level);
                    break;
                case SUMMON_TYPE.ACC:
                    tbKey = string.Format("AccessoryGachaLvCost_{0}", level);
                    break;
            }
            Tables.InGamePrice ingameTb = Tables.InGamePrice.Get(tbKey);
            if (ingameTb != null)
            {
                count -= ingameTb.demandGoodsQty;
                demandCnt = ingameTb.demandGoodsQty;
                if (count >= 0)
                    level++;
            }
        }
        return level;
    }
    public void GetEquipItem(ITEM_TYPE _type, InvenItemInfo _info)
    {
        if (hasItemDictionary.TryGetValue(_type, out var list))
        {
            InvenItemInfo invenItem = list.Find(x => x.key == _info.key);
            if (invenItem != null)
            {
                invenItem.count += _info.count;
            }
            else
            {
                list.Add(_info);
            }
        }
        else
        {
            hasItemDictionary.Add(_type, new List<InvenItemInfo> { _info });
        }
    }
    double CalculateTotalAbility(STAT _st)
    {
        double total = 0;
        switch (_st)
        {
            case STAT.ATTACK:
                total = PlayerController.Instance.Damage;
                break;
            case STAT.HP:
                total = PlayerController.Instance.MaxHP;
                break;
            case STAT.HP_REGEN:
                total = PlayerController.Instance.HPRegen;
                break;
            case STAT.DEFENCE:
                total = PlayerController.Instance.Defense;
                break;
            case STAT.ATTACK_SPD:
                total = PlayerController.Instance.AttackSpd;
                break;
            case STAT.MOVE_SPD:
                total = PlayerController.Instance.MoveSpd;
                break;
            case STAT.CRI_DAM:
                total = PlayerController.Instance.CriDam;
                break;
            case STAT.CRI_RATE:
                total = PlayerController.Instance.CriDam;
                break;
            case STAT.HIT:
                total = PlayerController.Instance.Accuracy;
                break;
            case STAT.DODGE:
                total = PlayerController.Instance.Dodge;
                break;
        }
        total += PlayerController.Instance.CalculateGrowthStat(_st);
        total += PlayerController.Instance.GetEquipItemAbilityValue(_st);
        return total;
    }
    private void OnApplicationQuit()
    {
        if (GameManager.instance.isTest)
            return;

        PlayerSaveData saveData = new PlayerSaveData();
        string data = JsonUtility.ToJson(saveData, true);

        PlayerPrefs.SetString("PlayerData", data);
    }

    void LoadData()
    {
        if(GameManager.instance.isTest)
        {
            PlayerPrefs.DeleteAll();
        }
        string playerData = PlayerPrefs.GetString("PlayerData", string.Empty);
        if (!string.IsNullOrEmpty(playerData))
        {
            PlayerSaveData saveData = JsonUtility.FromJson<PlayerSaveData>(playerData);
            saveData.LoadData();
        }
        else
        {
            new PlayerSaveData();
        }
    }
}
