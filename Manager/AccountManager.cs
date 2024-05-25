using Newtonsoft.Json;
using NPOI.OpenXmlFormats.Shared;
using NPOI.Util;
using System.Collections;
using System.Collections.Generic;
using Tables;
using UnityEngine;
using System.Linq;
using Unity.VisualScripting;
using Newtonsoft.Json.Linq;



public class AccountManager : Singleton<AccountManager>
{
    int curStageKey = 101001;
    int playerLevel = 100;
    List<int> growthLevelList = new List<int>();
    Dictionary<ITEM_TYPE, List<InvenItemInfo>> hasItemDictionary = new Dictionary<ITEM_TYPE, List<InvenItemInfo>>();
    uint gold = 0;
    uint dia = 0;
    public int PlayerLevel { get { return playerLevel; } set => playerLevel = value; }
    public int CurStageKey { get => curStageKey;    set => curStageKey = value; }

    string[] CurrencyUnits = new string[] { "", "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z" };
    
    public Dictionary<ITEM_TYPE, List<InvenItemInfo>> HasItemDictionary => hasItemDictionary;

    int[] summonCount = Enumerable.Repeat(0, 4).ToArray();
    

    public int[] SummonLevel => summonCount;
    public List<int> GrowthLevelList
    {
        get => growthLevelList;
        set => growthLevelList = value;
    }
    public ulong Gold => gold;
    public ulong Dia => dia;

    void Start()
    {
        dia = (uint)PlayerPrefs.GetInt("Dia", 0);
        gold = (uint)PlayerPrefs.GetInt("Gold", 0);
        string invenData = PlayerPrefs.GetString("Inventory", string.Empty);
        if(!string.IsNullOrEmpty(invenData))
        {
            hasItemDictionary = DictionaryJsonUtility.FromJson<ITEM_TYPE,List<InvenItemInfo>>(invenData);
        }
        AddGoods(GOOD_TYPE.GOLD, 1000000000);


    }

    public void UseGoods(GOOD_TYPE _type, uint _amount, out bool _isEnough)
    {
        uint goods = 0;
        switch (_type)
        {
            case GOOD_TYPE.DIA:
                if (dia < _amount)
                {
                    _isEnough = false;
                    return;
                }
                dia -= _amount; // 다이아 사용
                goods = dia;
                PlayerPrefs.SetInt("Dia", (int)dia);
                break;
            case GOOD_TYPE.GOLD:
                if (gold < _amount)
                {
                    _isEnough = false;
                    return;
                }
                gold -= _amount; // 골드 사용
                goods = gold;
                PlayerPrefs.SetInt("Gold", (int)gold);
                break;
        }
        _isEnough = true;
        UIManager.Instance.UpdateGoodText(_type, goods);

    }
    public void AddGoods(GOOD_TYPE _type, uint _amount)
    {
        uint goods = 0;
        switch (_type)
        {
            case GOOD_TYPE.DIA:
                dia += _amount;
                goods = dia;
                PlayerPrefs.SetInt("Dia", (int)dia);
                break;
            case GOOD_TYPE.GOLD:
                gold += _amount;
                goods = gold;
                PlayerPrefs.SetInt("Gold", (int)gold);
                break;
        }
        UIManager.Instance.UpdateGoodText(_type, goods);
    }

    public string ToCurrencyString(double number)
    {
        string zero = "0";

        if (-1d < number && number < 1d)  // 0일 때
        {
            return zero;
        }

        if (double.IsInfinity(number))   // 무한 일 때 
        {
            return "Infinity";
        }

        //  부호 출력 문자열
        string significant = (number < 0) ? "-" : string.Empty;

        //  보여줄 숫자
        string showNumber = string.Empty;

        //  단위 문자열
        string unityString = string.Empty;

        //  패턴을 단순화 시키기 위해 무조건 지수 표현식으로 변경한 후 처리
        string[] partsSplit = number.ToString("E").Split('+');

        //  예외
        if (partsSplit.Length < 2)
        {
            return zero;
        }

        //  지수 (자릿수 표현)
        if (!int.TryParse(partsSplit[1], out int exponent))  // 지수가 존재하는지
        {
            Debug.LogWarningFormat("Failed - ToCurrentString({0}) : partSplit[1] = {1}", number, partsSplit[1]);
            return zero;
        }

        //  몫은 문자열 인덱스
        int quotient = exponent / 3;   // 000이 3개 , 1000 단위로 값이 변경된다

        //  나머지는 정수부 자릿수 계산에 사용(10의 거듭제곱을 사용)
        int remainder = exponent % 3;

        //  1A 미만은 그냥 표현
        if (exponent < 3)
        {
            showNumber = System.Math.Truncate(number).ToString();
        }
        else
        {
            //  10의 거듭제곱을 구해서 자릿수 표현값을 만들어 준다.
            double temp = double.Parse(partsSplit[0].Replace("E", "")) * System.Math.Pow(10, remainder);

            //  소수 둘째자리까지만 출력한다.
            showNumber = temp.ToString("F").Replace(".00", "");
        }

        unityString = CurrencyUnits[quotient];

        return string.Format("{0}{1}{2}", significant, showNumber, unityString);
    }


    public InvenItemInfo GetHasInvenItem(Tables.Item _item)
    {
        hasItemDictionary.TryGetValue((ITEM_TYPE)_item.ItemType,out List<InvenItemInfo> list);
        InvenItemInfo itemInfo = new InvenItemInfo();
        if (list.Count > 0)
        {
            itemInfo = list.Find(x => x.key == _item.key);
            if (itemInfo == null)
                itemInfo = new InvenItemInfo();
        }
        itemInfo.key = _item.key;
        return itemInfo;
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
        while(count >= demandCnt)
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
            count -= ingameTb.demandGoodsQty;
            demandCnt = ingameTb.demandGoodsQty;
            if (count >= 0)
                level++;
        }
        return level;
    }
    public void GetEquipItem(ITEM_TYPE _type, InvenItemInfo _info)
    {
        _info.count = 1;
        if(hasItemDictionary.ContainsKey(_type))
        {
            InvenItemInfo invenItem = hasItemDictionary[_type].Find(x => x.key == _info.key);

            if (invenItem != null)
            {
                invenItem.count++;
            }
            else
                hasItemDictionary[_type].Add(_info);
        }
        else
        {
            hasItemDictionary.Add(_type,new List<InvenItemInfo> { _info });
        }
    }
    private void OnApplicationQuit()
    {
        string data = DictionaryJsonUtility.ToJson(hasItemDictionary);
        PlayerPrefs.SetString("Inventory", data);
    }
}
