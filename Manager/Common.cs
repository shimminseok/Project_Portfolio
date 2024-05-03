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
    COLLEAGUE,
    ENEMY,
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
    MAP
}
public enum OBJ_ANIMATION_STATE
{
    IDLE,
    DIE,
    MOVE,
    ATTACK,
    LESS_HP
}
#endregion
#region [Interface]
public interface IAttackable
{    
    float Damage { get; set; }
    float AttackSpd {  get; set; }
    float AttackRange {  get; set; }
    float CriDam { get; set; }
    float Accuracy { get; set; }
    bool IsCri {  get; set; }

    void InitAttackData();
    int SetAttackPow(float _attackPow);
    void SetDamage(IAttackable _attacker);
    void SetDamageText();
    void SetAttackEvent();
}
public interface IHittable
{
    float GenTime { get; set; }
    bool IsDead { get; set; }
    float MaxHP { get; set; }
    float CurHP { get; set; }
    float HPRegen {  get; set; }
    float Defense { get; set; }
    float Dodge { get; set; }
    void InitHitData();
    void UpdateHPUI();
    void SetDeadEvent();
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
    void ManualMove(Vector3 dir);


}
#endregion

#region[Class]

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


