using NPOI.Util;
using System.Collections;
using System.Collections.Generic;
using Tables;
using UnityEngine;



public class AccountManager : Singleton<AccountManager>
{
    // Start is called before the first frame update
    int curStageKey = 101001;
    int playerLevel = 100;
    List<int> growthLevelList = new List<int>();
    ulong gold = 0;
    ulong dia = 0;
    public int PlayerLevel { get { return playerLevel; } set => playerLevel = value; }
    public int CurStageKey { get => curStageKey;    set => curStageKey = value; }

    string[] CurrencyUnits = new string[] { "", "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z" };
    
    
    public List<int> GrowthLevelList
    {
        get => growthLevelList;
        set => growthLevelList = value;
    }
    public ulong Gold => gold;
    public ulong Dia => dia;

    void Start()
    {
    }

    public void UseGoods(GOOD_TYPE _type, ulong _amount)
    {
        ulong goods = 0;
        switch (_type)
        {
            case GOOD_TYPE.DIA:
                if (dia < _amount)
                {
                    return;
                }
                dia -= _amount; // ���̾� ���
                goods = dia;
                break;
            case GOOD_TYPE.GOLD:
                if (gold < _amount)
                {
                    return;
                }
                gold -= _amount; // ��� ���
                goods = gold;
                break;
        }
        UIManager.Instance.UpdateGoodText(_type, goods);
    }
    public void AddGoods(GOOD_TYPE _type, ulong _amount)
    {
        ulong goods = 0;
        switch (_type)
        {
            case GOOD_TYPE.DIA:
                dia += _amount;
                goods = dia;
                break;
            case GOOD_TYPE.GOLD:
                gold += _amount;
                goods = gold;
                break;
        }

        UIManager.Instance.UpdateGoodText(_type, goods);
    }

    public string ToCurrencyString(double number)
    {
        string zero = "0";

        if (-1d < number && number < 1d)  // 0�� ��
        {
            return zero;
        }

        if (double.IsInfinity(number))   // ���� �� �� 
        {
            return "Infinity";
        }

        //  ��ȣ ��� ���ڿ�
        string significant = (number < 0) ? "-" : string.Empty;

        //  ������ ����
        string showNumber = string.Empty;

        //  ���� ���ڿ�
        string unityString = string.Empty;

        //  ������ �ܼ�ȭ ��Ű�� ���� ������ ���� ǥ�������� ������ �� ó��
        string[] partsSplit = number.ToString("E").Split('+');

        //  ����
        if (partsSplit.Length < 2)
        {
            return zero;
        }

        //  ���� (�ڸ��� ǥ��)
        if (!int.TryParse(partsSplit[1], out int exponent))  // ������ �����ϴ���
        {
            Debug.LogWarningFormat("Failed - ToCurrentString({0}) : partSplit[1] = {1}", number, partsSplit[1]);
            return zero;
        }

        //  ���� ���ڿ� �ε���
        int quotient = exponent / 3;   // 000�� 3�� , 1000 ������ ���� ����ȴ�

        //  �������� ������ �ڸ��� ��꿡 ���(10�� �ŵ������� ���)
        int remainder = exponent % 3;

        //  1A �̸��� �׳� ǥ��
        if (exponent < 3)
        {
            showNumber = System.Math.Truncate(number).ToString();
        }
        else
        {
            //  10�� �ŵ������� ���ؼ� �ڸ��� ǥ������ ����� �ش�.
            double temp = double.Parse(partsSplit[0].Replace("E", "")) * System.Math.Pow(10, remainder);

            //  �Ҽ� ��°�ڸ������� ����Ѵ�.
            showNumber = temp.ToString("F").Replace(".00", "");
        }

        unityString = CurrencyUnits[quotient];

        return string.Format("{0}{1}{2}", significant, showNumber, unityString);
    }


}
