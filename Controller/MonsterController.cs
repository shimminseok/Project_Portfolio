using NPOI.XWPF.UserModel;
using System.Collections;
using System.Collections.Generic;
using Tables;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class MonsterController : ObjectController, IAttackable, IMoveable, IHittable
{
    public Monster monsterTb;
    PlayerController targetObj;

    TagController m_TagController;
    MONSTER_TYPE monsterType;
    public Rigidbody rigi;

    int finalDamage;

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
    public int FinalDamage
    {
        get => finalDamage;
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
    public TagController TagController
    {
        get => m_TagController;
        set => m_TagController = value;
    }
    public ObjectController Target => targetObj;


    public Vector3 TargetDir => Target.transform.localPosition - transform.localPosition;

    private void Awake()
    {
        ObjectGetComponent();
        objType = OBJ_TYPE.MONSTER;
    }
    void Start()
    {
    }
    void Update()
    {

    }
    void FixedUpdate()
    {
        if(isDead) 
            return;

        if(targetObj != null)
        {
            if (targetObj.IsDead)
            {
                FindEnemy();
            }
            else
            {
                if (GetTargetDistance(targetObj.transform) > attackRange)
                {
                    Move(Target.transform.localPosition - transform.localPosition);
                }
                else
                {
                    Attack();
                }
            }
        }


    }

    public override void Init()
    {
        isDead = false;
        maxHp = curHp = monsterTb.HealthPoint;
        hpRegen = monsterTb.HealthPointRegen;
        accuracy = monsterTb.Hit;
        dodge = monsterTb.Dodge;
        damage = monsterTb.Attack;
        targetObj = FindObjectOfType(typeof(PlayerController)) as PlayerController;
        aniCtrl.ChangeAnimation(OBJ_ANIMATION_STATE.IDLE);

        m_TagController = PoolManager.Instance.GetObj("HP_Guage", POOL_TYPE.TAG).GetComponent<TagController>();
        m_TagController.SetTag(this);
        InitMoveData();
        InitAttackData();
    }
    public void SetMonster(Vector3 _pos, MONSTER_TYPE _type)
    {
        monsterType = _type;
        Init();
        Vector3 pos = Random.insideUnitSphere;
        pos.y = 0;
        pos *= 2;

        transform.localPosition = pos;
        transform.localPosition += _pos;

        transform.localScale *= monsterTb.Scale;

    }
    public void InitAttackData()
    {
        attackSpd = monsterTb.AttackSpeed;
        attackRange = monsterTb.AttackRange;
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

        Vector3 monveVec = dir.normalized * MoveSpd;
        Rotate(monveVec);
        Vector3 tmpVec = Vector3.Lerp(transform.localPosition, transform.localPosition + monveVec, 0.01f);
        transform.localPosition = tmpVec;
        SetMoveEvent();
    }
    void Attack()
    {
        if (aniCtrl.GetAniState != OBJ_ANIMATION_STATE.ATTACK)
        {
            ChangeState(OBJ_ANIMATION_STATE.ATTACK);
            Rotate(TargetDir);
        }

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

    public void SetDamageText()
    {
        throw new System.NotImplementedException();
    }

    public void SetDeadEvent()
    {
        IsDead = true;
        CurHP = 0;
        aniCtrl.ChangeAnimation(OBJ_ANIMATION_STATE.DIE);
        MonsterManager.Instance.monsterList.Remove(this);
        
        //Init();
    }

    public void SetMoveEvent()
    {
        aniCtrl.ChangeAnimation(OBJ_ANIMATION_STATE.MOVE);
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
    public override void FindEnemy()
    {
        targetObj = PlayerController.Instance;
    }

    public void GetDamage(int _damage)
    {
        int finalDam = Mathf.RoundToInt(_damage - defense);
        curHp -= finalDam <= 0 ? 1 : finalDam;
        
        Debug.LogFormat("Monster Name : {0}, Cur HP : {1}", gameObject.name, curHp);
        if (curHp <= 0)
        {
            SetDeadEvent();
        }
    }
    public int CalculateAttackDamage()
    {
        int finalDamage = Mathf.RoundToInt(isCri ? damage * 2 + CriDam : damage);

        return finalDamage;
    }
}
