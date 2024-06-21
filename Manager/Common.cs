using System;
using System.Collections;
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
    SKILL_2,
    SKILL_3,
    SKILL_4,
    SKILL_5,
    SKILL_6,
    SKILL_7,
    SKILL_8,
    SKILL_9,
    SKILL_10,
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
    BTN_ICON,
    GROWTH_ICON,
    ITEM_GRADE,
    ITEM_ICON,
    MONSTER
}
public enum EFFECT_TYPE
{
    ATTACK,
    HIT,
    PROJECTILE,
    SKILL1,
    SKILL1_HIT,
    SKILL1_MISSILE_FX,
    SKILL1_TARGET_FX,
    DEAD,
    HIT_BOSS,
    MAX
}
public enum ITEM_CATEGORY
{
    GOODS = 1,
    ITEM,
    MATERIAL,
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
public enum SOUND_BGM
{
    NO_0,   // 기본 bgm
    NO_1,   // 소환 bgm

}
public enum SOUND_EFFECT
{
    NO_0,           // 공격 단타 1
    NO_1,           // 공격 단타 2
    NO_2,           // 공격 단타 3
    NO_3,           // 공격 단타 4
    NO_4,           // 골드 소모
    NO_5,           // 클릭
    NO_6,           // 무기 / 방어구 착용
    NO_7,           // 장신구 착용
    NO_8,           // 장착 해제
    NO_9,           // 평타Slash 1
    NO_10,         // 평타Slash 2
    NO_11,          //팝업 오픈


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
public enum QUEST_TYPE
{
    DAYILY,
    WEEKLY,
    ACHIEBEMENT
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
    bool IsRangeAttacker { get; set; }
    bool IsCri { get; }
    void AttackAniEvent(IHittable _target);
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
    IEnumerator SetDeadEvent();
    void GetDamage(double _damage);
}

public interface IMoveable
{
    bool IsMove { get; set; }
    float MoveSpd { get; }
    List<Node> Path { get; set; }
    void SetMoveEvent();
    void Move();
    void Rotate(Vector3 dir);
    float GetTargetDistance(Vector3 _target);
}
public interface IControlable
{
    JoystickController JoystickController { get; }
    bool IsManualControl { get; }
    Vector2 InputDirection { get; }
}

public interface IUseSkill
{
    SkillInfo[] SkillInfoList { get; }
    int UseSkillNum { get; }
    Dictionary<int, float> SkillCoolTime { get; set; }

    float UpdateSkillCoolTime(int _index, bool _isFill);
    double CalculateSkillDamage(SkillInfo _skillInfo);
    bool IsTargetAngle(GameObject _target, float _angle);
    List<IHittable> GetInCircleObjects(Transform _start, float _radius);
    List<IHittable> GetInBarObjects(Transform _start, float _width, float _range);
    void UseSkill(int _index);
    void SetSkillCoolDown(int _index);
    void PlaySkillEffect(string _name);
    void SkillAniEvent(int _type);
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
public class SkillListCellData
{
    int index;
    public Tables.Skill m_skill;
    public bool isSelected;
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
    int index;
    public ITEM_CATEGORY category;
    public int rewardKey;
    public double count;


    public int Index { get => index; set { index = value; } }
}
public class WorldMapFirstClearRewardItemCellData
{
    int index;
    public ITEM_CATEGORY category;
    public int rewardKey;
    public double count;

    public int Index { get => index; set { index = value; } }
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
    public int Index { get => index; set { index = value; } }
}
public class QuestSlotCellData
{
    int index;
    public Tables.Quest m_QuestTb;
    public bool isSelected;
    public int questState;

    public ITEM_CATEGORY rewardItemCartegory;


    public int questCount;
    public int clearCount;
    public int Index { get => index; set { index = value; } }

    public int GetQuestProcess()
    {
        int questState = -1;
        int goalCount = m_QuestTb.Value;
        while (questCount >= goalCount)
        {
            questCount -= goalCount;
            clearCount++;
            if (m_QuestTb.NextValue == 0)
                break;

            goalCount = m_QuestTb.NextValue;
        }

        return questState;
    }
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
public class MaterialInfo
{
    public int key;
    public double count;

    public MaterialInfo(int _key, double _count)
    {
        key = _key;
        count = _count;
    }
}
[System.Serializable]
public class StageInfo
{
    public int key;
    public bool isChallengeableBoss;

    public StageInfo(int _key)
    {
        key = _key;
        isChallengeableBoss = AccountManager.Instance.BestStageInfo.key > AccountManager.Instance.CurrentStageInfo.key;
    }
    public StageInfo()
    {
        key = 101001;
        isChallengeableBoss = false;
    }
}

[System.Serializable]
public class DictionaryWrapper<TKey, TValue>
{
    public TKey key;
    public TValue value;
}
[System.Serializable]
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
    public Node boss;
    public string name;
    public string memo;
    public TextAsset text;
    public Node[,] mapNode;
    public Vector2 mapSize;
    public List<Vector3> monsterSpawnPoint = new List<Vector3>();
    public List<GameObject> mapList = new List<GameObject>();

}
public class Node
{
    public bool walkable;
    public Vector3 worldPos;
    public int gridX;
    public int gridY;
    public int gCost;
    public int hCost;
    public Node parent;
    public int fCost { get { return gCost + hCost; } }

    public Node(bool _walkable, Vector3 _worldPos, int _gridX, int _gridY)
    {
        walkable = _walkable;
        worldPos = _worldPos;
        gridX = _gridX;
        gridY = _gridY;
    }
}
[System.Serializable]
public class PlayerSaveData
{
    public int level = AccountManager.Instance.PlayerLevel;
    public double gold = AccountManager.Instance.Gold;
    public double dia = AccountManager.Instance.Dia;

    public StageInfo currentStage = AccountManager.Instance.CurrentStageInfo;
    public StageInfo bestStageInfo = AccountManager.Instance.BestStageInfo;

    public string growhLevel = DictionaryJsonUtility.ToJson(PlayerController.Instance.GrowthLevelDic);
    public string inventory = DictionaryJsonUtility.ToJson(AccountManager.Instance.HasItemDictionary);

    public int[] summonCount = AccountManager.Instance.SummonCount;
    public int[] summonRewardLevel = AccountManager.Instance.SummonRewardLevel;

    public SkillInfo[] equipSkillInfo = PlayerController.Instance.SkillInfoList;


    public void LoadData()
    {
        AccountManager.Instance.PlayerLevel = level;

        AccountManager.Instance.Gold = gold;
        AccountManager.Instance.Dia = dia;

        AccountManager.Instance.CurrentStageInfo = currentStage;
        AccountManager.Instance.BestStageInfo = bestStageInfo;



        PlayerController.Instance.GrowthLevelDic = DictionaryJsonUtility.FromJson<STAT, int>(growhLevel);
        AccountManager.Instance.HasItemDictionary = DictionaryJsonUtility.FromJson<ITEM_TYPE, List<InvenItemInfo>>(inventory);
        summonCount = AccountManager.Instance.SummonCount;
        summonRewardLevel = AccountManager.Instance.SummonRewardLevel;

        equipSkillInfo = PlayerController.Instance.SkillInfoList;
    }

}
#endregion


