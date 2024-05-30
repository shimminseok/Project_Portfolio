using System;
using System.Collections.Generic;
using Tables;
using UnityEngine;


public class PlayerController : ObjectController, IMoveable, IAttackable, IHittable, IControlable, IUseSkill, IEquipableItem
{
    public static PlayerController Instance;
    [Header("PlayerController")]
    public CHARACTER_JOB job;
    Tables.Character characterTb;


    MonsterController targetObj;
    TagController m_TagController;
    public JoystickController JoystickController => GameManager.Instance.joystickController;


    int attackCount;

    double maxHp;
    double curHp;
    double hpRegen;
    float genTime;

    float defense;
    double criDam;
    double damage;
    double finalDamage;
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

    //Equipment
    InvenItemInfo[] equipItem = new InvenItemInfo[6] { new(),new(), new(), new(), new(),new()};


    public double Damage
    {
        get => damage;
    }
    public double FinalDamage
    {
        get => finalDamage;
    }
    public float Defense
    {
        get => defense;
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

    public double CriDam
    {
        get => damage + criDam;
    }

    public double MaxHP
    {
        get => maxHp;
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
    public double CurHP
    {
        get => curHp;
        set => curHp = value;
    }

    public double HPRegen
    {
        get => hpRegen;
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

    public InvenItemInfo[] EquipmentItem
    {
        get => equipItem;
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
        if (isDead)
            return;



        HpGen();

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
            if (targetObj == null)
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

        m_TagController = PoolManager.Instance.GetObj("HP_Guage", POOL_TYPE.TAG).GetComponent<TagController>();
        m_TagController.SetTag(this);
    }
    public void InitHitData()
    {
        maxHp = CurHP = characterTb.HealthPoint;
        hpRegen = characterTb.HealthPointRegen;
        defense = characterTb.DefencePoint;
    }
    public void InitAttackData()
    {
        damage = characterTb.Attack;
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

    public double SetAttackPow(float _attackPow)
    {
        return Math.Truncate(Damage + _attackPow);
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
        foreach (var mon in MonsterManager.instance.monsterList)
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
        useSkillNum = _index;
        //스킬 사용
        ChangeState((OBJ_ANIMATION_STATE)_index + 101);
        Rotate(TargetDir);

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

    public void GetDamage(double _damage)
    {
        double finalDam = Math.Truncate(_damage - defense);

        curHp -= (finalDam <= 0 ? 1 : finalDam);
        //UpdateHPUI();
        m_TagController.UpdateHPUI();
        if (curHp <= 0)
        {
            SetDeadEvent();
        }
    }
    public double CalculateAttackDamage()
    {
        StatReinforce sr = StatReinforce.Get((int)STAT.ATTACK);

        double finalDamage = 0;

        //성장
        finalDamage += GetGrowthValue(STAT.ATTACK);

        //아이템 착용
        finalDamage += GetEquipItemAbilityValue(STAT.ATTACK);

        finalDamage += (isCri ? damage * 2 + CriDam : damage);

        return finalDamage;
    }
    public double CalculateSkillDamage(SkillInfo _skillInfo)
    {
        double finaldamage = 0;
        Tables.Skill skillTb = Tables.Skill.Get(_skillInfo.skillKey);
        double skillOffset = skillTb.DamageCoefficient + (_skillInfo.skillLevel * skillTb.AddDamageCoefficient);
        if (skillTb != null)
        {
            finaldamage = Math.Truncate(damage * skillOffset);
        }

        return finaldamage;
    }

    public void PlaySkillEffect(string[] _name)
    {
        for (int i = 0; i < _name.Length; i++)
        {
            GameObject go = EffectManager.instance.GetEffect(_name[i]);
            EffectManager.instance.PlayEffect(go);
            go.transform.parent = effectRoot;
            go.transform.localPosition = Vector3.zero;
        }

    }

    public void SkillAniEvent()
    {
        List<IHittable> gos = new List<IHittable>();
        Tables.Skill skillTb = Tables.Skill.Get(skillInfoList[useSkillNum].skillKey);
        switch ((SKILL_TYPE)skillTb.SkillType)
        {
            case SKILL_TYPE.CIRCLE:
                gos = GetInCircleObjects(transform, skillTb.SkillRange);
                break;
            case SKILL_TYPE.BAR:
                gos = GetInBarObjects(transform, skillTb.SkillRange, 5);
                break;
            case SKILL_TYPE.ANGLE:
                break;
        }
        foreach (var go in gos)
        {
            for (int i = 0; i < skillTb.AttackCount; i++)
            {
                go.GetDamage(CalculateSkillDamage(skillInfoList[useSkillNum]));
            }
        }
        PlaySkillEffect(skillTb.ActionFx); ;
    }
    public float GetGrowthValue(STAT _stat)
    {
        StatReinforce tb = StatReinforce.Get((int)_stat + 1);

        return AccountManager.Instance.GrowthLevelList[(int)_stat] * tb.StatValue;
    }

    public double GetEquipItemAbilityValue(STAT _stat)
    {
        double returnValue = 0;
        for (int i = 0; i < equipItem.Length; i++)
        {
            if (equipItem[i].IsEmpty)
                continue;

            Tables.Item itemTb = Item.Get(equipItem[i].key);
            if(itemTb != null)
            {
                for (int j = 0; j < itemTb.Ability.Length; j++)
                {
                    if (_stat == (STAT)itemTb.Ability[j])
                    {
                        returnValue += itemTb.AbilityValue[i];
                        break;
                    }
                }
            }

        }

        return returnValue;
    }

    public void EquipItem(InvenItemInfo _item)
    {
        Tables.Item itemTb = Item.Get(_item.key);
        if (!equipItem[itemTb.ItemType].IsEmpty)
            DequipItem(itemTb.ItemType);


        _item.isEquipped = true;
        equipItem[itemTb.ItemType] = _item;
        UICharactorInfo.instance.SetEquippengItemSlot(itemTb.ItemType);
    }

    public void DequipItem(int _index)
    {
        equipItem[_index].isEquipped = false;
        equipItem[_index] = new InvenItemInfo();
        UICharactorInfo.instance.SetEquippengItemSlot(_index);
    }
}