using NPOI.XWPF.UserModel;
using System.Collections;
using System.Collections.Generic;
using Tables;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class MonsterController : ObjectController, IAttackable, IMoveable, IHittable
{
    public Monster monsterTb;


    MONSTER_TYPE monsterType;
    [SerializeField] ObjectController targetObj;
    public Rigidbody rigi;

    float maxHp;
    float curHp;
    float hpRegen;
    float genTime;

    float defense;
    float criDam;
    float damage;
    float attackSpd;
    float attackRange;
    float accuracy;
    float dodge;
    float moveSpeed;

    bool isCri;
    bool isDead;


    public float MoveSpd 
    { 
        get => moveSpeed; 
        set => moveSpeed = value; 
    }
    public float GenTime 
    { 
        get => genTime; 
        set => genTime = value; 
    }
    public bool IsDead
    { 
        get =>  isDead; 
        set => isDead = value; 
    }
    public float MaxHP 
    { 
        get => maxHp; 
        set => maxHp = value; 
    }
    public float CurHP 
    { 
        get => curHp; 
        set => curHp = value; 
    }
    public float HPRegen 
    { 
        get => hpRegen; 
        set => hpRegen = value; 
    }
    public float Defense 
    { 
        get => defense; 
        set => defense = value;
    }
    public float Damage 
    { 
        get => damage; 
        set => damage = value; 
    }
    public float AttackSpd 
    { 
        get =>attackSpd; 
        set => attackSpd = value; 
    }
    public float AttackRange 
    { 
        get => attackRange; 
        set => attackRange = value; 
    }
    public float CriDam
    { 
        get => criDam;
        set => criDam = value; 
    }
    public float Accuracy
    {
        get => accuracy;
        set => accuracy = value;
    }
    public float Dodge
    {
        get => dodge;
        set => dodge = value;
    }
    public bool IsCri 
    { 
        get => isCri;
        set => isCri = value; 
    }
    public bool IsMove 
    { 
        get => throw new System.NotImplementedException(); 
        set => throw new System.NotImplementedException(); 
    }

    void Start()
    {
        objType = OBJ_TYPE.ENEMY;
        ObjectGetComponent();
        Init();
    }
    void Update()
    {

    }
    void FixedUpdate()
    {
        if (GetTargetDistance(targetObj.transform) > attackRange)
        {
            Move(targetObj.transform.localPosition);
        }
        else
        {
            aniCtrl.CurrentState = OBJ_ANIMATION_STATE.IDLE;
        }
    }

    public override void Init()
    {
        isDead = false;
        maxHp = curHp = monsterTb.HealthPoint;
        hpRegen = monsterTb.HealthPointRegen;
        accuracy = monsterTb.Hit;
        dodge = monsterTb.Dodge;
        targetObj = FindObjectOfType(typeof(PlayerController)) as ObjectController;

        InitMoveData();
        InitAttackData();
    }

    public void InitAttackData()
    {
        attackSpd = monsterTb.AttackSpeed;
        attackRange = 1;
    }

    public void InitHitData()
    {
    }

    public void InitMoveData()
    {
        moveSpeed = 3;
    }

    public void Move(Vector3 dir)
    {
        //캐릭터를 찾아서 따라감
        if (isDead)
            return;
        Rotate(dir);
        Vector3 direction = (dir - transform.localPosition).normalized;
        Vector3 movement = direction * MoveSpd * Time.fixedDeltaTime;
        SetMoveEvent();
        transform.Translate(movement,Space.World);
    }

    public override void ObjectGetComponent()
    {
        aniCtrl = GetComponent<AnimationController>();
        rigi = GetComponent<Rigidbody>();
        rigi.useGravity = false;
        rigi.mass = 2000;
        rigi.drag = 100;
        rigi.angularDrag = 0.05f;
        rigi.useGravity = true;
        rigi.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;

    }

    public void Rotate(Vector3 dir)
    {
        transform.LookAt(targetObj.transform);
    }

    public void SetAttackEvent()
    {
        throw new System.NotImplementedException();
    }

    public int SetAttackPow(float _attackPow)
    {
        throw new System.NotImplementedException();
    }

    public void SetDamage(IAttackable _attacker)
    {
        //TO DO
        //방어력 계산후 데미지 설정
        int finalDam = Mathf.RoundToInt(_attacker.IsCri ? _attacker.Damage * 2 + _attacker.CriDam :_attacker.Damage);
        CurHP -= (finalDam - defense);
        if (CurHP <= 0)
        {
            SetDeadEvent();
        }
    }

    public void SetDamageText()
    {
        throw new System.NotImplementedException();
    }

    public void SetDeadEvent()
    {
        isDead = true;
        CurHP = 0;
        MonsterManager.Instance.monsterList.Remove(this);
        Init();
    }

    public void SetMoveEvent()
    {
        aniCtrl.CurrentState = OBJ_ANIMATION_STATE.MOVE;
    }

    public void SetMoveSpeed(float _moveSpeed)
    {
        throw new System.NotImplementedException();
    }

    public void UpdateHPUI()
    {
        throw new System.NotImplementedException();
    }

    public float GetTargetDistance(Transform _target)
    {
        return Vector3.Distance(transform.localPosition, _target.transform.localPosition);
    }
    public override void FindEnemy(ObjectController _target)
    {
        targetObj = _target as PlayerController;
    }
}
