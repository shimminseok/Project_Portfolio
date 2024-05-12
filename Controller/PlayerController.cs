using System.Collections.Generic;
using UnityEngine;

public class PlayerController : ObjectController, IMoveable, IAttackable, IHittable, IControlable, IUseSkill
{
    public static PlayerController Instance;
    [Header("PlayerController")]
    public CHARACTER_JOB job;
    Tables.Character characterTb;


    MonsterController targetObj;
    TagController m_TagController;
    public JoystickController JoystickController => GameManager.Instance.joystickController;


    int attackCount;

    float maxHp;
    float curHp;
    float hpRegen;
    float genTime;

    float defense;
    float criDam;
    float damage;
    int finalDamage;
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
    public int FinalDamage
    {
        get => finalDamage;
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
    public TagController TagController
    {
        get => m_TagController;
        set => m_TagController = value;
    }
    public Vector3 TargetDir => Target.transform.localPosition - transform.localPosition;
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
        if (isDead)
            return;



        //HpGen();

        if (Input.GetKeyDown(KeyCode.Alpha1) && IsUseableSkill(0))
        {
            UseSkill(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) && IsUseableSkill(1))
        {
            UseSkill(1);
        }

        for (int i = 0; i < skillInfoList.Count; i++)
        {
            if (GameManager.Instance.isAuto && !IsManualControl && IsUseableSkill(i)
                && GetTargetDistance(targetObj.transform) <= attackRange && !aniCtrl.IsPlayingAnimation("SKILL"))
            {
                UseSkill(i);
                return;
            }
        }
    }
    void FixedUpdate()
    {
        if (isDead)
            return;

        if (IsManualControl || !GameManager.Instance.isAuto)
        {
            if (InputDirection != Vector2.zero)
                Move(new Vector3(InputDirection.x, 0, InputDirection.y));
            else
                Idle();
        }


        //수동 조작이 아니면 적을 찾음
        else if (!IsManualControl)
        {
            if(targetObj == null)
            {
                Idle();
            }
            else if (targetObj.IsDead)
            {
                FindEnemy();
            }
            else
            {
                if (aniCtrl.GetAniState < OBJ_ANIMATION_STATE.SKILL_1)
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
        
        m_TagController = PoolManager.Instance.GetObj("HP_Guage",POOL_TYPE.TAG).GetComponent<TagController>();
        m_TagController.SetTag(this);
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
        if (aniCtrl.IsPlayingAnimation("ATTACK") || aniCtrl.IsPlayingAnimation("SKILL"))
            return;

        Vector3 monveVec = dir.normalized * MoveSpd;
        Rotate(monveVec);
        Vector3 tmpVec = Vector3.Lerp(transform.localPosition, transform.localPosition + monveVec, 0.01f);
        transform.localPosition = tmpVec;
        SetMoveEvent();
    }
    public void Rotate(Vector3 dir)
    {
        Quaternion rotation = Quaternion.LookRotation(dir, Vector3.up);
        transform.rotation = rotation;
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
        if (aniCtrl.IsPlayingAnimation("SKILL"))
            return;

        if (aniCtrl.GetAniState != OBJ_ANIMATION_STATE.ATTACK)
        {
            ChangeState(OBJ_ANIMATION_STATE.ATTACK);
            Rotate(TargetDir);
        }

    }
    void Idle()
    {
        FindEnemy();
        ChangeState(OBJ_ANIMATION_STATE.IDLE);
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

    public void UpdateHPUI()
    {
        m_TagController.UpdateHPUI();
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

    public void SetDeadEvent()
    {
        isDead = true;
        AttackCount = 0;
        CurHP = 0;
        ChangeState(OBJ_ANIMATION_STATE.DIE);
    }
    public float GetTargetDistance(Transform _target)
    {
        return Vector3.Distance(transform.localPosition, _target.localPosition);
    }
    /// <summary>
    /// 적을 찾는 함수
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
            }
        }
    }
    //스킬장착 및 교체시 스킬 쿨타임을 초기화 시켜주는 함수
    public void SetSkillCoolDown(int _index)
    {
        Tables.Skill skillTb = Tables.Skill.Get(_index);
        if (skillTb != null)
        {
            if (!SkillCoolTime.ContainsKey(skillTb.key))
                SkillCoolTime.Add(_index, skillTb.CoolTime);
            else
                SkillCoolTime[_index] = skillTb.CoolTime;
        }

    }
    public void UseSkill(int _index)
    {
        UseSkillNum = _index;
        //스킬 사용
        ChangeState((OBJ_ANIMATION_STATE)_index + 101);
        Rotate(TargetDir);
        //
        List<GameObject> gos = GetInBarObjects(transform, 100, 5);
        Tables.Skill skillTb = Tables.Skill.Get(SkillInfoList[_index].skillKey);
        foreach (var g in gos)
        {
            IHittable hitObj = g.GetComponent<IHittable>();
            if (hitObj != null)
            {
                for (int i = 0; i < skillTb.AttackCount; i++)
                {
                    hitObj.GetDamage(CalculateSkillDamage(skillInfoList[_index]));
                }
            }
        }
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
            else
            {
                Tables.Skill skillTb = Tables.Skill.Get(_index);
                return SkillCoolTime[_index] / skillTb.CoolTime;
            }
        }
        else
            return 0f;
    }
    public bool IsUseableSkill(int _num)
    {
        return aniCtrl.GetAniState == OBJ_ANIMATION_STATE.IDLE && !skillInfoList[_num].IsEmpty && SkillCoolTime[SkillInfoList[_num].skillKey] <= 0;
    }

    public void GetDamage(int _damage)
    {
        int finalDam =  Mathf.RoundToInt(_damage - defense);
        
        curHp -= (finalDam <= 0 ? 1 : finalDam);
        //UpdateHPUI();
        m_TagController.UpdateHPUI();
        if (curHp <= 0)
        {
            SetDeadEvent();
        }
    }
    public int CalculateAttackDamage()
    {
        int finaldamage = Mathf.RoundToInt(isCri ? damage * 2 + CriDam : damage);

        return finaldamage;
    }
    public int CalculateSkillDamage(SkillInfo _skillInfo)
    {
        int finaldamage = 0;
        Tables.Skill skillTb = Tables.Skill.Get(_skillInfo.skillKey);
        float skillOffset = skillTb.DamageCoefficient + (_skillInfo.skillLevel * skillTb.AddDamageCoefficient);
        if (skillTb != null)
        {
            finaldamage = Mathf.RoundToInt(damage * skillOffset);
        }

        return finaldamage;
    }
}