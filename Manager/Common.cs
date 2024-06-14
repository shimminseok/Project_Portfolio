using NPOI.POIFS.Storage;
using System;
using System.Collections.Generic;
using UnityEngine;

#region [Enum]
public enum CHARACTER_JOB
{
    KNIGHT = 1,
    RANGER,
    MAGITION
}
public enum OBJ_TYPE
{
    PLAYER,
    COLLEAGUE,
    MONSTER,
}
public enum MONSTER_TYPE
{
    COMMON,
    ELETE,
    BOSS
}
public enum POOL_TYPE
{
    MONSTER,
    MAP,
    TAG,
    EFFECT
}
public enum ITEM_TYPE
{
    WEAPONE,
    ARMOR,
    GLOVES,
    SHOES,
    ACC_1,
    ACC_2
}
public enum OBJ_ANIMATION_STATE
{
    IDLE,
    DIE,
    MOVE,
    ATTACK,
    WIN,
    READY,

    SKILL_1 = 101,
    SKILL_3,
    SKILL_4,
    SKILL_5,
    SKILL_6,
    SKILL_7,
    SKILL_8,
    SKILL_9,
}
public enum GOOD_TYPE
{
    GOLD = 1,
    DIA,
}
public enum SKILL_TYPE
{
    CIRCLE = 1,
    BAR,
    ANGLE
}
public enum SPRITE_TYPE
{
    SKILL_ICON,
    SKILL_LISTICON,
    BTN_ICON,
    GROWTH_ICON,
    ITEM_GRADE,
    ITEM_ICON,
    MONSTER
}
public enum ITEM_CATEGORY
{
    GOODS = 1,
    ITEM = 2
}
public enum STAT
{
    ATTACK = 1,
    HP,
    HP_REGEN,
    DEFENCE,
    ATTACK_SPD,
    MOVE_SPD,
    CRI_DAM,
    CRI_RATE,
    HIT,
    DODGE,
}
public enum STAT_TARGET_TYPE
{
    PLAYER = 1,
    COLLEAGUE,
    ALL
}
public enum SUMMON_TYPE
{
    WEAPONE,
    ARMOR,
    ACC,
}
public enum SLOT_TYPE
{
    INVENITEM,
    SUMMON_RESULT,
    REWARD
}
public enum FULL_POPUP_TYPE
{
    NONE,
    SUMMON,
}
public enum GAME_STATE
{
    READY,
    PLAYING,
    WIN,
    END,
    BOSS,
    LOADING
}
#endregion
#region [Interface]
public interface IAttackable
{
    ObjectController Target { get; }
    double Damage { get; }
    float AttackSpd { get; }
    float AttackRange { get; }
    double CriDam { get; }
    float Accuracy { get; set; }

    bool IsCri { get; }
    double CalculateAttDam();
}
public interface IAffectedGrowth
{
    Dictionary<STAT, int> GrowthLevelDic { get; }
    double CalculateGrowthStat(STAT _stat);
}
public interface IHittable
{

    float GenTime { get; set; }
    bool IsDead { get; }
    double MaxHP { get; }
    double CurHP { get; set; }
    double HPRegen { get; }
    double Defense { get; }
    float Dodge { get; }
    TagController TagController { get; }
    void UpdateHPUI();
    void SetDeadEvent();
    void GetDamage(double _damage);
}

public interface IMoveable
{
    bool IsMove { get; set; }
    float MoveSpd { get; }
    void SetMoveEvent();
    void Move(Vector3 dir);
    void Rotate(Vector3 dir);
    float GetTargetDistance(Transform _target);
}
public interface IControlable
{
    JoystickController JoystickController { get; }
    bool IsManualControl { get; }
    Vector2 InputDirection { get; }
}

public interface IUseSkill
{
    List<SkillInfo> SkillInfoList { get; }
    int UseSkillNum { get; }
    Dictionary<int, float> SkillCoolTime { get; set; }

    float UpdateSkillCoolTime(int _index, bool _isFill);
    double CalculateSkillDamage(SkillInfo _skillInfo);
    bool IsTargetAngle(GameObject _target, float _angle);
    List<IHittable> GetInCircleObjects(Transform _start, float _radius);
    List<IHittable> GetInBarObjects(Transform _start, float _width, float _range);
    void UseSkill(int _index);
    void SetSkillCoolDown(int _index);
    void PlaySkillEffect(string[] _name);
    void SkillAniEvent();
}
public interface IEquipableItem
{
    public InvenItemInfo[] EquipmentItem { get; }
    void EquipItem(InvenItemInfo _item);
    void DequipItem(int _index);
    double GetEquipItemAbilityValue(STAT _stat);
}

#endregion

#region[Class]

#region[CellData]
public class GrowthSlotCellData
{
    int index;

    public int Index { get => index; set { index = value; } }
}
public class InvenSlotCellData
{
    int index;
    public Tables.Item m_ItemTb;

    public int Index { get => index; set { index = value; } }
}
public class WorldMapGenMonsterCellData
{
    int index;
    public Tables.Monster m_MonsterTb;

    public int Index { get => index; set { index = value; } }
}
public class WorldMapIdleRewardItemCellData
{

}
public class WorldMapFirstClearRewardItemCellData
{

}
public class WorldMapSlotCellData
{
    int index;
    public Tables.Stage m_StageTb;
    public bool isSelected;
    public int Index { get => index; set { index = value; } }
}
public class WorldMapChapterCellData
{
    int index;
    public int chapter;
    public bool isSelected;
    public int Index { get => index; set { index = value;} }
}
#endregion[]
[Serializable]
public class InvenItemInfo
{
    public int key;
    public double count = 0;
    public int enhanceCount = 0;

    public bool isEquipped = false;

    public List<string> GetAbilityText()
    {
        List<string> str = new List<string>();
        Tables.Item itemTb = Tables.Item.Get(key);
        if (itemTb != null)
        {
            for (int i = 0; i < itemTb.Ability.Length; i++)
            {
                switch ((STAT)itemTb.Ability[i])
                {
                    case STAT.ATTACK:
                        str.Add(UIManager.Instance.GetText("AttackPoint"));
                        break;
                    case STAT.HP:
                        str.Add(UIManager.Instance.GetText("HealthPoint"));
                        break;
                    case STAT.HP_REGEN:
                        str.Add(UIManager.Instance.GetText("HealthPointRegen"));
                        break;
                    case STAT.DEFENCE:
                        str.Add(UIManager.Instance.GetText("DefencePoint"));
                        break;
                    case STAT.ATTACK_SPD:
                        str.Add(UIManager.Instance.GetText("AttackSpeed"));
                        break;
                    case STAT.MOVE_SPD:
                        str.Add(UIManager.Instance.GetText("MoveSpeed"));
                        break;
                    case STAT.DODGE:
                        str.Add(UIManager.Instance.GetText("Dodge"));
                        break;
                    case STAT.HIT:
                        str.Add(UIManager.Instance.GetText("Hit"));
                        break;
                    case STAT.CRI_DAM:
                        str.Add(UIManager.Instance.GetText("CriticalDamage"));
                        break;
                    case STAT.CRI_RATE:
                        str.Add(UIManager.Instance.GetText("CriticalRate"));
                        break;
                }
            }

        }
        return str;
    }
    public List<double> GetEquipEffectValues(int _enhanceCnt = 0)
    {
        List<double> str = new List<double>();
        Tables.Item itemTb = Tables.Item.Get(key);
        if (itemTb != null)
        {
            Tables.EnhancementData enhanceData = Tables.EnhancementData.Get(itemTb.Enhancement);
            if (enhanceData != null)
            {
                for (int i = 0; i < itemTb.Ability.Length; i++)
                {
                    switch ((STAT)itemTb.Ability[i])
                    {
                        case STAT.ATTACK:
                            str.Add(itemTb.AbilityValue[i] + enhanceData.atk * (enhanceCount + _enhanceCnt));
                            break;
                        case STAT.HP:
                            str.Add(itemTb.AbilityValue[i] + enhanceData.maxHp * (enhanceCount + _enhanceCnt));
                            break;
                        case STAT.HP_REGEN:
                            str.Add(itemTb.AbilityValue[i] + enhanceData.hpgen * (enhanceCount + _enhanceCnt));
                            break;
                        case STAT.DEFENCE:
                            str.Add(itemTb.AbilityValue[i] + enhanceData.def * (enhanceCount + _enhanceCnt));
                            break;
                        case STAT.ATTACK_SPD:
                            str.Add(itemTb.AbilityValue[i] + enhanceData.attackspeed * (enhanceCount + _enhanceCnt));
                            break;
                        case STAT.DODGE:
                            str.Add(itemTb.AbilityValue[i] + enhanceData.DodgePoint * (enhanceCount + _enhanceCnt));
                            break;
                        case STAT.HIT:
                            str.Add(itemTb.AbilityValue[i] + enhanceData.HitPoint * (enhanceCount + _enhanceCnt));
                            break;
                        case STAT.CRI_DAM:
                            str.Add(itemTb.AbilityValue[i] + enhanceData.CriticalDamagePoint * (enhanceCount + _enhanceCnt));
                            break;
                        case STAT.CRI_RATE:
                            str.Add(itemTb.AbilityValue[i] + enhanceData.CriticalChancePoint * (enhanceCount + _enhanceCnt));
                            break;
                    }
                }
            }
        }
        return str;
    }
    public List<double> GetPassiveEffectValues(int _enhanceCnt = 0)
    {
        List<double> str = new List<double>();
        Tables.Item itemTb = Tables.Item.Get(key);
        if (itemTb != null)
        {
            Tables.EnhancementData enhanceData = Tables.EnhancementData.Get(itemTb.Enhancement);
            if (enhanceData != null)
            {
                for (int i = 0; i < itemTb.PassiveEffect.Length; i++)
                {
                    switch ((STAT)itemTb.PassiveEffect[i])
                    {
                        case STAT.ATTACK:
                            str.Add(itemTb.PassiveEffectValue[i] + enhanceData.atk * (enhanceCount + _enhanceCnt));
                            break;
                        case STAT.HP:
                            str.Add(itemTb.PassiveEffectValue[i] + enhanceData.maxHp * (enhanceCount + _enhanceCnt));
                            break;
                        case STAT.HP_REGEN:
                            str.Add(itemTb.PassiveEffectValue[i] + enhanceData.hpgen * (enhanceCount + _enhanceCnt));
                            break;
                        case STAT.DEFENCE:
                            str.Add(itemTb.PassiveEffectValue[i] + enhanceData.def * (enhanceCount + _enhanceCnt));
                            break;
                        case STAT.ATTACK_SPD:
                            str.Add(itemTb.PassiveEffectValue[i] + enhanceData.attackspeed * (enhanceCount + _enhanceCnt));
                            break;
                        case STAT.DODGE:
                            str.Add(itemTb.PassiveEffectValue[i] + enhanceData.DodgePoint * (enhanceCount + _enhanceCnt));
                            break;
                        case STAT.HIT:
                            str.Add(itemTb.PassiveEffectValue[i] + enhanceData.HitPoint * (enhanceCount + _enhanceCnt));
                            break;
                        case STAT.CRI_DAM:
                            str.Add(itemTb.PassiveEffectValue[i] + enhanceData.CriticalDamagePoint * (enhanceCount + _enhanceCnt));
                            break;
                        case STAT.CRI_RATE:
                            str.Add(itemTb.PassiveEffectValue[i] + enhanceData.CriticalChancePoint * (enhanceCount + _enhanceCnt));
                            break;
                    }
                }
            }
        }
        return str;
    }

    public bool IsEmpty => key == 0;
}
[System.Serializable]
public class DictionaryWrapper<TKey, TValue>
{
    public TKey key;
    public TValue value;
}
public class SkillInfo
{
    public int skillKey;
    public int skillLevel;

    public bool IsEmpty { get => skillKey == 0; }

    public void EquipSkill(int _key, int _level)
    {
        skillKey = _key;
        skillLevel = _level;
    }
    public void UnEquipSkill()
    {
        skillKey = 0;
        skillLevel = 0;
    }
}
public class GrowthInfo
{
    public int key;
    public int level;
}
[System.Serializable]
public class Map
{
    public Node start;
    public string name;
    public string memo;
    public TextAsset Text;
    public Node[,] MapNode;
    public Vector2 GroundSize;
    public List<Vector3> monsterSpawnPoint = new List<Vector3>();
    public List<GameObject> MapList = new List<GameObject>();
}
public class node
{
    public bool walkable;
    public Vector3 worldPos;
    public int gridX;
    public int gridY;
    public int gCost;
    public int hCost;
    public node parent;
    public int fCost { get { return gCost + hCost; } }

    public node(bool _walkable, Vector3 _worldPos, int _gridX, int _gridY)
    {
        walkable = _walkable;
        worldPos = _worldPos;
        gridX = _gridX;
        gridY = _gridY;
    }
    public node(node _node)
    {
        walkable = _node.walkable;
        worldPos = _node.worldPos;
        gridX = _node.gridX;
        gridY = _node.gridY;
        gCost = _node.gCost;
        hCost = _node.hCost;
        parent = _node.parent;

    }
}
public class Node
{
    public Node Parent;
    public bool Moveable;
    public float F;
    public float G;
    public float H;

    public int X;
    public int Y;

    public Vector3 Position;

    public float fCost
    {
        get { return G + H; }
    }

    public Node(float x, float y, bool moveable = true)
    {
        X = (int)x;
        Y = (int)y;
        Moveable = moveable;
    }

    public Node(Node node)
    {
        Parent = node.Parent;
        Moveable = node.Moveable;

        F = node.F;
        G = node.G;
        H = node.H;

        X = node.X;
        Y = node.Y;

        Position = node.Position;
    }
}

[System.Serializable]
public class PlayerSaveData
{
    public int level = AccountManager.Instance.PlayerLevel;

    public double gold = AccountManager.Instance.Gold;
    public double dia = AccountManager.Instance.Dia;

    public int stageKey = AccountManager.Instance.CurStageKey;

    public string growhLevel = DictionaryJsonUtility.ToJson(PlayerController.Instance.GrowthLevelDic);
    public string inventory = DictionaryJsonUtility.ToJson(AccountManager.Instance.HasItemDictionary);

    public int[] summonCount = AccountManager.Instance.SummonCount;
    public int[] summonRewardLevel = AccountManager.Instance.SummonRewardLevel;


    public void LoadData()
    {
        AccountManager.Instance.PlayerLevel = level;

        AccountManager.Instance.Gold = gold;
        AccountManager.Instance.Dia = dia;

        AccountManager.Instance.CurStageKey = stageKey;

        PlayerController.Instance.GrowthLevelDic = DictionaryJsonUtility.FromJson<STAT,int>(growhLevel);
        AccountManager.Instance.HasItemDictionary = DictionaryJsonUtility.FromJson<ITEM_TYPE, List<InvenItemInfo>>(inventory);
        summonCount = AccountManager.Instance.SummonCount;
        summonRewardLevel =AccountManager.Instance.SummonRewardLevel;
    }

}
#endregion


