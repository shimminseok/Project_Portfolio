using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

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
public enum OBJ_ANIMATION_STATE
{
    IDLE,
    DIE,
    MOVE,
    ATTACK,
    LESS_HP,


    SKILL_1 = 101,
    SKILL_2
}
public enum GOOD_TYPE
{
    DIA = 1,
    GOLD,
}
public enum SKILL_TYPE
{
    CIRCLE,
    BAR,
    ANGLE
}
public enum SPRITE_TYPE
{
    SKILL_ICON,
    SKILL_LISTICON,
    BTN_ICON,
}
#endregion
#region [Interface]
public interface IAttackable
{    
    ObjectController Target { get; }
    int FinalDamage { get;}
    float Damage { get; }
    float AttackSpd {  get; set; }
    float AttackRange {  get; set; }
    float CriDam { get; }
    float Accuracy { get; set; }

    bool IsCri {  get; set; }

    void InitAttackData();
    int SetAttackPow(float _attackPow);
    void SetDamageText();
    int CalculateAttackDamage();

}
public interface IHittable
{

    float GenTime { get; set; }
    bool IsDead { get; }
    float MaxHP { get; }
    float CurHP { get; set; }
    float HPRegen {  get; }
    float Defense { get; }
    float Dodge { get; }
    TagController TagController { get; set; }
    void InitHitData();
    void UpdateHPUI();
    void SetDeadEvent();
    void GetDamage(int _damage);
}

    public interface IMoveable
{
    bool IsMove { get; set; }
    float MoveSpd {  get; set; }
    void InitMoveData();
    void SetMoveSpeed(float _moveSpeed);
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
    int UseSkillNum {  get; }
    Dictionary<int,float> SkillCoolTime { get; set; }

    float UpdateSkillCoolTime(int _index,bool _isFill);
    int CalculateSkillDamage(SkillInfo _skillInfo);
    bool IsTargetAngle(GameObject _target, float _angle);
    List<IHittable> GetInCircleObjects(Transform _start, float _radius);
    List<IHittable> GetInBarObjects(Transform _start, float _width, float _range);
    void UseSkill(int _index);
    void SetSkillCoolDown(int _index);
    void PlaySkillEffect(string[] _name);
    void SkillAniEvent();
}
public interface IContent : IEventSystemHandler
{
    bool Update(int index);
}
public interface IReuseCellData
{
    int Index { get; set; }
    public void Clear();
}
#endregion

#region[Class]

#region[CellData]
public class GrowthSlotCellData
{
    int index;
    public int Index { get => index; set => index = value; }

    public void Clear()
    {

    }
}
#endregion[]
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
[System.Serializable]
public class Map
{
    public Node start;
    public string name;
    public string memo;
    public Node[,] MapNode;
    public Vector2 GroundSize;
    public Dictionary<Vector3, int> MonsterSpawnPoints = new Dictionary<Vector3, int>();
    public List<GameObject> MapList = new List<GameObject>();
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

    public void CalcCost(Node destination, int g)
    {
        GetH(destination);
        G = g;
        F = G + H;
    }

    void GetH(Node destination)
    {
        int diffX = Mathf.Abs(destination.X - X);
        int diffY = Mathf.Abs(destination.Y - Y);
        H = (diffX + diffY) * 10;
    }
}

#endregion


