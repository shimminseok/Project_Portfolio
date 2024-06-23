using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Tables;
using UnityEditorInternal.Profiling.Memory.Experimental;
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
    ACHIEBEMENT,
    LOOP,
}
public enum QUEST_CARTEGORY
{
    KILL_MONSTER = 1,//
    USE_GOLD,//
    USE_DIA,//
    UPGRADE_GROWTH,//
    SUMMON,//
    GET_EQUIPMENT,
    QUEST_CLEAR,
    CHAR_LEVEL_UP,
    CLEAR_STAGE,//
    MAX,
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
    public bool isSelected;
    public QuestInfo m_QuestInfo;

    public int Index { get => index; set { index = value; } }
}
#endregion[]
[Serializable]
public class InvenItemInfo
{
    public int key;
    public double count = 0;
    public int enhanceCount = 0;
    public bool isEquipped = false;
    public bool isGet = false;
    public bool IsEmpty => key == 0;

    static readonly Dictionary<STAT, string> statTextMapping = new Dictionary<STAT, string>
    {
        { STAT.ATTACK, "AttackPoint" },
        { STAT.HP, "HealthPoint" },
        { STAT.HP_REGEN, "HealthPointRegen" },
        { STAT.DEFENCE, "DefencePoint" },
        { STAT.ATTACK_SPD, "AttackSpeed" },
        { STAT.MOVE_SPD, "MoveSpeed" },
        { STAT.DODGE, "Dodge" },
        { STAT.HIT, "Hit" },
        { STAT.CRI_DAM, "CriticalDamage" },
        { STAT.CRI_RATE, "CriticalRate" }
    };

    public List<string> GetAbilityText()
    {
        var abilityTexts = new List<string>();
        var itemTb = Tables.Item.Get(key);

        if (itemTb != null)
        {
            foreach (var ability in itemTb.Ability)
            {
                if (statTextMapping.TryGetValue((STAT)ability, out string text))
                {
                    abilityTexts.Add(UIManager.Instance.GetText(text));
                }
            }
        }

        return abilityTexts;
    }

    public List<double> GetEquipEffectValues(int _enhanceCnt = 0)
    {
        return GetEffectValues(itemTb => itemTb.AbilityValue, _enhanceCnt);
    }

    public List<double> GetPassiveEffectValues(int _enhanceCnt = 0)
    {
        return GetEffectValues(itemTb => itemTb.PassiveEffectValue, _enhanceCnt);
    }

    List<double> GetEffectValues(Func<Tables.Item, double[]> valueSelector, int _enhanceCnt)
    {
        List<double> effectValues = new List<double>();
        Tables.Item itemTb = Tables.Item.Get(key);
        Tables.EnhancementData enhanceData = Tables.EnhancementData.Get((int)(itemTb?.Enhancement));

        if (itemTb != null && enhanceData != null)
        {
            double[] values = valueSelector(itemTb);
            int[] effects = itemTb.Ability;

            for (int i = 0; i < effects.Length; i++)
            {
                double value = values[i];
                value += GetEnhancementValue((STAT)effects[i], enhanceData, enhanceCount + _enhanceCnt);
                effectValues.Add(value);
            }
        }
        return effectValues;
    }

    double GetEnhancementValue(STAT stat, Tables.EnhancementData enhanceData, int enhanceCount)
    {
        return stat switch
        {
            STAT.ATTACK => enhanceData.atk * enhanceCount,
            STAT.HP => enhanceData.maxHp * enhanceCount,
            STAT.HP_REGEN => enhanceData.hpgen * enhanceCount,
            STAT.DEFENCE => enhanceData.def * enhanceCount,
            STAT.ATTACK_SPD => enhanceData.attackspeed * enhanceCount,
            STAT.DODGE => enhanceData.DodgePoint * enhanceCount,
            STAT.HIT => enhanceData.HitPoint * enhanceCount,
            STAT.CRI_DAM => enhanceData.CriticalDamagePoint * enhanceCount,
            STAT.CRI_RATE => enhanceData.CriticalChancePoint * enhanceCount,
            _ => 0
        };
    }

    public InvenItemInfo GetNextGradeItem()
    {
        var currentItem = Tables.Item.Get(key);
        bool foundCurrentItem = false;

        foreach (var item in Tables.Item.data.Values)
        {
            if (item.Job == currentItem.Job && item.ItemType == currentItem.ItemType)
            {
                if (foundCurrentItem)
                {
                    return new InvenItemInfo { key = item.key, count = 1 };
                }

                if (item.key == key)
                {
                    foundCurrentItem = true;
                }
            }
        }

        return new InvenItemInfo { key = key, count = 1 };
    }

    public void SynthesisItem()
    {
        var currentItem = Tables.Item.Get(key);
        var requiredCount = 5;
        int synthesCount = 0;
        while (count >= requiredCount)
        {
            count -= requiredCount;
            var nextItem = GetNextGradeItem();
            AccountManager.Instance.GetEquipItem((ITEM_TYPE)currentItem.ItemType, nextItem);
        }
        UIQuest.instance.IncreaseQuestCount(QUEST_CARTEGORY.GET_EQUIPMENT, synthesCount);
    }
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
public class QuestInfo
{
    public int key;
    public double questCount = 0;
    public int clearCount = 0;
    public int questState = -1;

    Quest m_QuestTb;

    public QuestInfo(int _key)
    {
        key = _key;
        m_QuestTb = Tables.Quest.Get(key);
    }

    public void IncrementQuestCount(double _count)
    {
        questCount += _count;
    }
    public void GetQuestProcess()
    {
        if (questState != -1)
            return;

        int goalCount = m_QuestTb.Value;

        if (questState != 1 && questCount >= m_QuestTb.Value)
        {
            questState = 0;
        }
    }


    public void GetReward()
    {
        while (questState == 0)
        {
            GameManager.Instance.GetReward(m_QuestTb.QuestReward, out bool result);
            if (result)
            {
                if (!m_QuestTb.Loop)
                {
                    questState = 1;
                    break;
                }
                if (questCount < m_QuestTb.Value)
                {
                    questState = -1;
                    break;
                }
                questCount -= m_QuestTb.Value;
                clearCount++;
            }
        }
        GetQuestProcess();
    }
    public void ShortcutsPopup()
    {

    }
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

    public string quest = DictionaryJsonUtility.ToJson(AccountManager.Instance.QuestInfoDictionary);


    public void LoadData()
    {
        AccountManager.Instance.PlayerLevel = level;

        AccountManager.Instance.Gold = gold;
        AccountManager.Instance.Dia = dia;

        AccountManager.Instance.CurrentStageInfo = currentStage;
        AccountManager.Instance.BestStageInfo = bestStageInfo;



        PlayerController.Instance.GrowthLevelDic = DictionaryJsonUtility.FromJson<STAT, int>(growhLevel);
        AccountManager.Instance.HasItemDictionary = DictionaryJsonUtility.FromJson<ITEM_TYPE, List<InvenItemInfo>>(inventory);
        AccountManager.Instance.SummonCount = summonCount;
        AccountManager.Instance.SummonRewardLevel = summonRewardLevel;

        PlayerController.Instance.SkillInfoList = equipSkillInfo;

        //AccountManager.Instance.QuestInfoDictionary = DictionaryJsonUtility.FromJson<QUEST_CARTEGORY, List<QuestInfo>>(quest);
    }

}
#endregion


