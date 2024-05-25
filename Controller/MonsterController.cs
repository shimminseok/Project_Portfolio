using NPOI.XWPF.UserModel;
using System;
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
    public Face face;
    [SerializeField] Material faceMaterial;
    PlayerController targetObj;

    TagController m_TagController;
    MONSTER_TYPE monsterType;
    public Rigidbody rigi;

    double finalDamage;

    double maxHp;
    double curHp;
    double hpRegen;
    float genTime;

    float defense;
    double criDam;
    double damage;
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
    public double MaxHP 
    { 
        get => maxHp; 
        set => maxHp = value; 
    }
    public double CurHP 
    { 
        get => curHp; 
        set => curHp = value; 
    }
    public double HPRegen 
    { 
        get => hpRegen; 
        set => hpRegen = value; 
    }
    public float Defense 
    { 
        get => defense; 
        set => defense = value;
    }
    public double Damage 
    { 
        get => damage; 
        set => damage = value; 
    }
    public double FinalDamage
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
    public double CriDam
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
        Vector3 pos = UnityEngine.Random.insideUnitSphere;
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
            SetFace(OBJ_ANIMATION_STATE.ATTACK);
            Rotate(TargetDir);
        }

    }
    public override void ObjectGetComponent()
    {
        aniCtrl = GetComponent<AnimationController>();
        faceMaterial = transform.GetChild(1).GetComponent<Renderer>().materials[1];
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

    public double SetAttackPow(float _attackPow)
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
        uint gold = 0;
        switch(monsterType)
        {
            case MONSTER_TYPE.COMMON:
                gold = MonsterManager.instance.currentStageTb.MonsterGold;
                break;
            case MONSTER_TYPE.ELETE:
                gold = MonsterManager.instance.currentStageTb.MonsterGold;
                break;
            case MONSTER_TYPE.BOSS:
                //스테이지 클리어
                break;
        }
        PoolManager.Instance.PushObj(m_TagController.name, POOL_TYPE.TAG, m_TagController.gameObject);
        AccountManager.Instance.AddGoods(GOOD_TYPE.GOLD,gold);
        ChangeState(OBJ_ANIMATION_STATE.DIE);
        SetFace(OBJ_ANIMATION_STATE.DIE);
        MonsterManager.instance.monsterList.Remove(this);

        
    }

    public void SetMoveEvent()
    {
        ChangeState(OBJ_ANIMATION_STATE.MOVE);
        SetFace(OBJ_ANIMATION_STATE.MOVE);

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

    public void GetDamage(double _damage)
    {
        double finalDam = Math.Truncate(_damage - defense);
        curHp -= finalDam <= 0 ? 1 : finalDam;
        
        Debug.LogFormat("Monster Name : {0}, Cur HP : {1}", gameObject.name, curHp);
        if (curHp <= 0)
        {
            SetDeadEvent();
        }
    }
    public double CalculateAttackDamage()
    {
        double finalDamage = Math.Truncate(isCri ? damage * 2 + CriDam : damage);

        return finalDamage;
    }
    public void SetFace(OBJ_ANIMATION_STATE _state)
    {
        switch(_state)
        {
            case OBJ_ANIMATION_STATE.IDLE:
                faceMaterial.SetTexture("_MainTex", face.Idleface);
                break;
            case OBJ_ANIMATION_STATE.MOVE:
                faceMaterial.SetTexture("_MainTex", face.WalkFace);
                break;
            case OBJ_ANIMATION_STATE.ATTACK:
                faceMaterial.SetTexture("_MainTex", face.attackFace);
                break;
            case OBJ_ANIMATION_STATE.DIE:
                faceMaterial.SetTexture("_MainTex", face.damageFace);
                break;
        }
    }
}
