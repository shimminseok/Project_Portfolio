using Newtonsoft.Json.Serialization;
using System;
using Tables;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : ObjectController, IMoveable, IAttackable, IHittable
{
    public CHARACTER_JOB job;

    int attackCount;

    float maxHp;
    float curHp;
    float hpRegen;
    float genTime;

    float defense;
    float criDam;
    float damage;
    float attackSpd;
    float attackRange;
    float moveSpeed;
    
    bool isCri;
    bool isLessHp;
    bool isDead;



    public float Damage 
    {
        get => damage;
        set
        {
            damage = value;
        }
    }
    public float Defense 
    { 
        get => defense;
        set
        {
            defense = value;
        }
    }
    public float AttackSpd 
    { 
        get => attackSpd;
        set
        {
            attackSpd = value;
        }
    }
    public float AttackRange 
    { 
        get => attackRange;
        set
        {
            attackRange = value;
        }
    }
    public float MoveSpd
    { 
        get => moveSpeed;
        set
        {
            moveSpeed = value;
        }
    }
    public bool IsCri 
    {
        get
        {
            return isCri;
        }
        set
        {
            isCri = Tables.Character.Get((int)job).CriticalRate <= UnityEngine.Random.Range(0, 1f);
            isCri = value;
        }
    }

    public float CriDam 
    {
        get => criDam; 
        set 
        {
            criDam = Damage * 2 + value;
        } 
    }

    public float MaxHP 
    { 
        get => maxHp;
        set
        {
            maxHp = value;
        }
    }

    public bool IsDead
    {
        get => isDead;
        set
        {
            isDead = value;
        }
    }

    public int AttackCount 
    {
        get => attackCount;
        set => attackCount = value;
    }
    public float CurHP 
    {
        get => curHp;
        set => curHp = value; 
    }

    public float HPRegen { 
        get => hpRegen; 
        set => hpRegen = value;
    }
    public float GenTime 
    { 
        get => genTime; 
        set => genTime = value; 
    }
    void Awake()
    {
        InitData(Tables.Character.Get((int)job));
        ObjectGetComponent();
    }
    void Start()
    {

    }
    void Update()
    {
        HpGen();
    }
    void HpGen()
    {
        if(CurHP < MaxHP)
        {
            GenTime += Time.deltaTime;
            if(GenTime >= 1f)
            {
                CurHP += HPRegen;

                if (CurHP >= MaxHP)
                    CurHP = MaxHP;

                GenTime = 0f;
            }

        }
    }
    void FixedUpdate()
    {
    }
    public override void Init()
    {
        isDead = false;
    }
    public override void ObjectGetComponent()
    {
    }
    void InitData(Character _tb)
    {
        GameManager.Instance.CreateCharacterPrefab(_tb.Prefab, transform);
        InitHitData(_tb);
        InitAttackData(_tb);
        InitMoveData(_tb);
    }
    public void InitHitData(Tables.Character _tb)
    {
        MaxHP = CurHP =_tb.HealthPoint;
        HPRegen = _tb.HealthPointRegen;
        Defense = _tb.DefencePoint;
    }
    public void InitAttackData(Character _tb)
    {
        Damage = _tb.Attack;
        AttackSpd = _tb.AttackSpeed;
        AttackRange = _tb.AttackRange;
    }
    public void InitMoveData(Character _tb)
    {
        SetMoveSpeed(_tb.MoveSpeed);
    }
    public void SetMoveEvent()
    {
        //TO DO
        //걷을때 발생되는 이벤트 함수
        throw new System.NotImplementedException();
    }
    public void SetMoveSpeed(float _moveSpeed)
    {
        MoveSpd = _moveSpeed;
    }
    public int SetDamage(IAttackable _attacker)
    {
        //TO DO
        //방어력 계산후 데미지 설정
        int finalDam = Mathf.RoundToInt(_attacker.IsCri ? _attacker.Damage : _attacker.CriDam);
        return finalDam;
    }

    public void UpdateHPUI()
    {
        UIManager.Instance.UpdateHPBarUI(maxHp, curHp);
    }

    public void SetDamageText()
    {
        //데미지 폰트를 띄워주는 함수
        //자신의 트랜스폼을 넘겨주고 UI를 뿌려준다?
    }

    public int SetAttackPow(float _attackPow)
    {
        return Mathf.RoundToInt(Damage + _attackPow);
    }

    public void GetDamage(IAttackable _attacker)
    {
        CurHP -= _attacker.SetDamage(_attacker);
        if(CurHP <= 0)
        {
            SetDeadEvent();
        }
    }

    public void SetDeadEvent()
    {
        IsDead = true;
        AttackCount = 0;
        CurHP = 0;
    }

    public void SetAttackEvent()
    {
    }
}