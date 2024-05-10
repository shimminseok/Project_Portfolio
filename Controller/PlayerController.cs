using System.Collections.Generic;
using UnityEngine;

public class PlayerController : ObjectController, IMoveable, IAttackable, IHittable, IControlable, IUseSkill
{
    public static PlayerController Instance;
    [Header("PlayerController")]
    public CHARACTER_JOB job;
    Tables.Character characterTb;


    MonsterController targetObj;
    public JoystickController JoystickController => GameManager.Instance.joystickController;


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
    float accuracy;
    float dodge;
    float moveSpeed;

    bool isCri;
    bool isLessHp;
    bool isDead;
    bool isMove;

    bool isFindEnemy;


    //Skill
    List<SkillInfo> skillInfoList = new List<SkillInfo>();
    Dictionary<int, float> skillCoolTime = new Dictionary<int, float>();
    int useSkillNum;

    int equipSkillIndex;
    SkillInfo equipSkillInfo;


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
    public float Accuracy
    {
        get => accuracy;
        set => accuracy = value;
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
            if (isDead)
            {
                isMove = false;
            }
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

    public float HPRegen
    {
        get => hpRegen;
        set => hpRegen = value;
    }
    public float GenTime
    {
        get => genTime;
        set => genTime = value;
    }

    public bool IsMove
    {
        get => isMove;
        set
        {
            isMove = value;
            if (isMove)
            {
            }
        }
    }
    public Vector2 InputDirection
    {
        get => JoystickController.Direction;
    }
    public float Dodge
    {
        get => dodge;
        set => dodge = value;
    }

    public bool IsManualControl
    {
        get
        {
            IsMove = JoystickController.Direction != Vector2.zero;
            return JoystickController.Direction != Vector2.zero;
        }
    }
    public ObjectController Target => targetObj;

    public bool IsFindEnemy
    {
        get
        {
            return isFindEnemy;
        }
        set
        {
            isFindEnemy = value;
        }
    }

    public List<SkillInfo> SkillInfoList
    {
        get
        {
            return skillInfoList;
        }
        set
        {
            skillInfoList = value;
        }
    }
    public int UseSkillNum
    {
        get => useSkillNum;
        set => useSkillNum = value;
    }
    public Dictionary<int, float> SkillCoolTime
    {
        get => skillCoolTime;
        set
        {
            skillCoolTime = value;
        }
    }

    void Awake()
    {
        if (Instance == null)
            Instance = this;
    }
    void Start()
    {
        for (int i = 0; i < UIManager.Instance.SkillSlotCount; i++)
        {
            skillInfoList.Add(new SkillInfo());
        }
        InitData((int)job);
        ObjectGetComponent();
    }
    void Update()
    {
        if (IsDead)
            return;


        HpGen();

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ChangeState(OBJ_ANIMATION_STATE.SKILL_1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ChangeState(OBJ_ANIMATION_STATE.SKILL_2);
        }
    }
    void FixedUpdate()
    {
        if (IsDead)
            return;

        if (IsManualControl || !GameManager.Instance.isAuto)
        {
            if (InputDirection != Vector2.zero)
                Move(new Vector3(InputDirection.x, 0, InputDirection.y));
            else
                Idle();
        }


        //���� ������ �ƴϸ� ���� ã��
        else if (!IsManualControl)
        {
            if (!IsFindEnemy)
            {
                FindEnemy();
            }
            if (targetObj != null && !targetObj.IsDead)
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
            else
            {
                ChangeState(OBJ_ANIMATION_STATE.IDLE);
            }
        }
    }
    public override void Init()
    {
        isDead = false;
    }
    public override void ObjectGetComponent()
    {
        aniCtrl = GetComponentInChildren<AnimationController>();
    }
    void InitData(int job)
    {
        characterTb = Tables.Character.Get(job);
        objType = OBJ_TYPE.PLAYER;
        InitHitData();
        InitAttackData();
        InitMoveData();
        for (int i = 0; i < skillInfoList.Count; i++)
        {
            UIManager.Instance.EquipSkill(i, skillInfoList[i].skillKey);
        }
    }
    public void InitHitData()
    {
        MaxHP = CurHP = characterTb.HealthPoint;
        HPRegen = characterTb.HealthPointRegen;
        Defense = characterTb.DefencePoint;
    }
    public void InitAttackData()
    {
        Damage = characterTb.Attack;
        AttackSpd = characterTb.AttackSpeed;
        AttackRange = characterTb.AttackRange;
    }
    public void InitMoveData()
    {
        SetMoveSpeed(characterTb.MoveSpeed);
    }
    public void Move(Vector3 dir)
    {
        Vector3 monveVec = dir.normalized * MoveSpd;
        Rotate(monveVec);
        Vector3 tmpVec = Vector3.Lerp(transform.localPosition, transform.localPosition + monveVec, 0.01f);
        transform.localPosition = tmpVec;
        SetMoveEvent();
    }
    public void Rotate(Vector3 dir)
    {
        Quaternion rotation = Quaternion.LookRotation(dir, Vector3.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.fixedDeltaTime * 10f);
    }
    public void SetMoveEvent()
    {
        ChangeState(OBJ_ANIMATION_STATE.MOVE);
    }
    public void SetMoveSpeed(float _moveSpeed)
    {
        MoveSpd = _moveSpeed;
    }
    void Attack()
    {
        ChangeState(OBJ_ANIMATION_STATE.ATTACK);
        Rotate(targetObj.transform.localPosition - transform.localPosition);
    }
    void Idle()
    {
        aniCtrl.CurrentState = OBJ_ANIMATION_STATE.IDLE;
    }
    void HpGen()
    {
        if (CurHP < MaxHP)
        {
            GenTime += Time.deltaTime;
            if (GenTime >= 1f)
            {
                CurHP += HPRegen;

                if (CurHP >= MaxHP)
                    CurHP = MaxHP;

                GenTime = 0f;
            }

        }
    }
    public void SetDamage(IAttackable _attacker)
    {
        //TO DO
        //���� ����� ������ ����

        int finalDam = Mathf.RoundToInt(_attacker.IsCri ? _attacker.Damage * 2 + _attacker.CriDam : _attacker.Damage);
        CurHP -= (finalDam - defense);
        if (CurHP <= 0)
        {
            SetDeadEvent();
        }
    }

    public void UpdateHPUI()
    {
        UIManager.Instance.UpdateHPBarUI(maxHp, curHp);
    }

    public void SetDamageText()
    {
        //������ ��Ʈ�� ����ִ� �Լ�
        //�ڽ��� Ʈ�������� �Ѱ��ְ� UI�� �ѷ��ش�?
    }

    public int SetAttackPow(float _attackPow)
    {
        return Mathf.RoundToInt(Damage + _attackPow);
    }

    public void SetDeadEvent()
    {
        isDead = true;
        AttackCount = 0;
        CurHP = 0;
        aniCtrl.CurrentState = OBJ_ANIMATION_STATE.DIE;
    }
    public float GetTargetDistance(Transform _target)
    {
        return Vector3.Distance(transform.localPosition, _target.localPosition);
    }
    /// <summary>
    /// ���� ã�� �Լ�
    /// </summary>
    /// <param name="_target"></param>
    public override void FindEnemy()
    {
        foreach (var mon in MonsterManager.Instance.monsterList)
        {
            float dis = mon.GetTargetDistance(transform);
            if (targetObj == null || targetObj.IsDead || dis < targetObj.GetTargetDistance(transform))
            {
                targetObj = mon;
                IsFindEnemy = true;
            }
        }
    }
    //��ų���� �� ��ü�� ��ų ��Ÿ���� �ʱ�ȭ �����ִ� �Լ�
    public void SetSkillCoolDown(int _index)
    {
        Tables.Skill skillTb = Tables.Skill.Get(_index);
        if (skillTb != null)
        {
            if (!SkillCoolTime.ContainsKey(skillTb.key))
                SkillCoolTime.Add(_index, skillTb.CoolTime);
        }
    }
    public void UseSkill(int _index)
    {
        if (skillInfoList[_index] == null || skillInfoList[_index].IsEmpty)
            return;

        UseSkillNum = _index;
        //��ų ���
        ChangeState((OBJ_ANIMATION_STATE)_index + 100);

        //
        SetSkillCoolDown(skillInfoList[_index].skillKey);
    }
    public float UpdateSkillCoolTime(int _index, bool _isFill)
    {
        if (skillCoolTime.ContainsKey(_index))
        {
            SkillCoolTime[_index] -= Time.deltaTime;
            if (SkillCoolTime[_index] < 0)
            {
                SkillCoolTime[_index] = 0;
            }
            if (!_isFill)
            {
                return SkillCoolTime[_index];
            }

            Tables.Skill skillTb = Tables.Skill.Get(_index);
            return SkillCoolTime[_index] / skillTb.CoolTime;
        }
        else
            return 0f;
    }

}