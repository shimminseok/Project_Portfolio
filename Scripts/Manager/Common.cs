using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;


#region [Enum]
public enum EObjType
{
    COLLEAGUE,
    NPC,
    ENEMY,
}
#endregion
#region [Interface]
public interface IAttackable
{
    int AttackCount { get; set; }
    float Damage { get; set; }
    float AttackSpd {  get; set; }
    float AttackRange {  get; set; }
    float CriDam { get; set; }
    bool IsCri {  get; set; }

    void InitAttackData(Tables.Character characterTb);
    int SetAttackPow(float _attackPow);
    int SetDamage(IAttackable _attacker);
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
    void InitHitData(Tables.Character characterTb);
    void UpdateHPUI();
    void GetDamage(IAttackable _attacker);
    void SetDeadEvent();
}

    public interface IMoveable
{
    float MoveSpd {  get; set; }
    void InitMoveData(Tables.Character characterTb);
    void SetMoveSpeed(float _moveSpeed);
    void SetMoveEvent();
}
#endregion

#region [Enum]
public enum CHARACTER_JOB
{
    KNIGHT = 1,
    RANGER,
    MAGITION
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


