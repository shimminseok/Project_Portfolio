using System;
using System.Collections.Generic;
using System.Linq;
using Tables;
using UnityEngine;


public class PlayerController : ObjectController, IMoveable, IAttackable, IHittable, IControlable, IUseSkill, IEquipableItem, IAffectedGrowth
{
    public static PlayerController Instance;
    [Header("PlayerController")]
    public CHARACTER_JOB job;
    Tables.Character characterTb;


    MonsterController targetObj;
    TagController m_TagController;
    public JoystickController JoystickController => GameManager.Instance.joystickController;

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
    float criRate;

    bool isCri;
    bool isLessHp;
    bool isDead;
    bool isMove;

    bool isFindEnemy;


    //Growth
    Dictionary<STAT, int> growthLevelDic = new Dictionary<STAT, int>();

    //Skill
    List<SkillInfo> skillInfoList = new List<SkillInfo>();
    Dictionary<int, float> skillCoolTime = new Dictionary<int, float>();
    int useSkillNum;

    int equipSkillIndex;
    SkillInfo equipSkillInfo;

    //Equipment
    InvenItemInfo[] equipItem = new InvenItemInfo[6] { new(), new(), new(), new(), new(), new() };


    List<Node> finalNodeList;

    Node targetNode;
    public double Damage
    {
        get
        {
            double dam = damage + CalculateGrowthStat(STAT.ATTACK) + GetEquipItemAbilityValue(STAT.ATTACK);
            return dam;
        }
    }
    public double Defense
    {
        get
        {
            double def = defense + CalculateGrowthStat(STAT.DEFENCE) + GetEquipItemAbilityValue(STAT.DEFENCE);
            return def;
        }
    }
    public float AttackSpd
    {
        get
        {
            float value = attackSpd + (float)CalculateGrowthStat(STAT.ATTACK_SPD) + (float)GetEquipItemAbilityValue(STAT.ATTACK_SPD);
            return value;
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
        get
        {
            float acc = accuracy + (float)CalculateGrowthStat(STAT.HIT) + (float)GetEquipItemAbilityValue(STAT.HIT);
            return accuracy;
        }
        set => accuracy = value;
    }
    public float MoveSpd
    {
        get
        {
            float moveSpd = moveSpeed + (float)CalculateGrowthStat(STAT.MOVE_SPD) + (float)GetEquipItemAbilityValue(STAT.MOVE_SPD);
            return moveSpd;
        }
    }
    public bool IsCri
    {
        get
        {
            float probability = criRate + (float)CalculateGrowthStat(STAT.CRI_RATE) + (float)GetEquipItemAbilityValue(STAT.CRI_RATE);
            isCri = probability >= UnityEngine.Random.Range(0, 1f);
            return isCri;
        }
    }

    public double CriDam
    {
        get => criDam + CalculateGrowthStat(STAT.CRI_DAM) + GetEquipItemAbilityValue(STAT.CRI_DAM);
    }

    public double MaxHP
    {
        get => maxHp + CalculateGrowthStat(STAT.HP) + GetEquipItemAbilityValue(STAT.HP);
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
        }
    }
    public Vector2 InputDirection
    {
        get => JoystickController.Direction;
    }
    public float Dodge
    {
        get
        {
            float value = dodge + (float)CalculateGrowthStat(STAT.DODGE) + (float)GetEquipItemAbilityValue(STAT.DODGE);
            return value;
        }
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
    }
    public Vector3 TargetDir => Target.transform.localPosition - transform.localPosition;

    public InvenItemInfo[] EquipmentItem
    {
        get => equipItem;
    }

    public Dictionary<STAT, int> GrowthLevelDic
    {
        get => growthLevelDic;
        set => growthLevelDic = value;
    }

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        foreach (var item in StatReinforce.data.Values.Where(x => x.Target == (int)STAT_TARGET_TYPE.PLAYER || x.Target == (int)STAT_TARGET_TYPE.ALL))
        {
            growthLevelDic.Add((STAT)item.key, 0);
        }
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
                break;
            }
        }
    }
    void FixedUpdate()
    {
        if (isDead || GameManager.Instance.GameState == GAME_STATE.WIN)
            return;


        if (GameManager.Instance.GameState == GAME_STATE.LOADING || GameManager.Instance.GameState == GAME_STATE.BOSS)
        {
            transform.localPosition = Vector3.zero;
            Idle();
        }
        else
        {
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
                if (targetObj == null || GameManager.Instance.GameState == GAME_STATE.READY)
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

        damage = characterTb.Attack;
        attackSpd = characterTb.AttackSpeed;
        attackRange = characterTb.AttackRange;
        maxHp = CurHP = characterTb.HealthPoint;
        hpRegen = characterTb.HealthPointRegen;
        defense = characterTb.DefencePoint;
        criDam = characterTb.CriticalDamage;
        defense = characterTb.DefencePoint;
        moveSpeed = characterTb.MoveSpeed;
        criRate = characterTb.CriticalRate;

        for (int i = 0; i < skillInfoList.Count; i++)
        {
            UIManager.Instance.EquipSkill(i, skillInfoList[i].skillKey);
        }

        m_TagController = PoolManager.Instance.GetObj("HP_Guage", POOL_TYPE.TAG).GetComponent<TagController>();
        TagController.SetTag(this);

    }
    public void Move(Vector3 dir)
    {
        if (IsManualControl)
        {
            Vector3 targetPos = transform.localPosition + dir.normalized * MoveSpd * Time.deltaTime;
            transform.localPosition = Vector3.MoveTowards(transform.position, targetPos, MoveSpd * Time.deltaTime);
            transform.LookAt(dir.normalized + transform.localPosition);
        }
        else
        {
            targetNode = finalNodeList.LastOrDefault();
            if (targetNode != null)
            {
                dir = new Vector3(targetNode.Position.x, 0, targetNode.Position.y);
                if (GetTargetDistance(dir) < 1)
                {
                    finalNodeList.RemoveAt(finalNodeList.Count - 1);
                }
            }
            transform.LookAt(dir);
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, dir, MoveSpd * Time.deltaTime);
        }
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
        TagController.UpdateHPUI(MaxHP, CurHP);
        UIManager.Instance.UpdateHPBarUI(MaxHP, CurHP);
    }

    public void SetDeadEvent()
    {
        isDead = true;
        CurHP = 0;
        ChangeState(OBJ_ANIMATION_STATE.DIE);
        GameManager.Instance.ChangeGameState(GAME_STATE.END);
    }
    public float GetTargetDistance(Transform _target)
    {
        return Vector3.Distance(transform.localPosition, _target.localPosition);
    }
    public float GetTargetDistance(Vector3 _target)
    {
        return Vector3.Distance(transform.localPosition, _target);
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
            if (targetObj == null || targetObj.IsDead ||    dis < targetObj.GetTargetDistance(transform) )
            {
                targetObj = mon;
            }
        }
        if (targetObj != null)
        {
            finalNodeList = Navigation.Instance.FindPath(new Vector2(transform.localPosition.x, transform.localPosition.z), new Vector2(targetObj.transform.localPosition.x, targetObj.transform.localPosition.z));
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
        Tables.Skill skill = Tables.Skill.Get(skillInfoList[_index].skillKey);
        if (skill != null)
        {
            ChangeState((OBJ_ANIMATION_STATE)skill.SkillAnimation);
            Rotate(TargetDir);
            SetSkillCoolDown(skillInfoList[_index].skillKey);
        }

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
        return !skillInfoList[_num].IsEmpty && SkillCoolTime[SkillInfoList[_num].skillKey] <= 0 && GameManager.Instance.GameState == GAME_STATE.PLAYING;
    }

    public void GetDamage(double _damage)
    {
        double finalDam = Math.Truncate(_damage - Defense);

        if (finalDam <= 0)
            finalDam = 1;
        curHp -= finalDam;
        UpdateHPUI();
        TagController.SetDamageFontText(finalDam);
        if (curHp <= 0)
        {
            SetDeadEvent();
        }
    }
    public double CalculateAttDam()
    {
        return Math.Truncate(IsCri ? (Damage * 2) + CriDam : Damage); ;
    }
    public double CalculateSkillDamage(SkillInfo _skillInfo)
    {
        double finaldamage = 0;
        Tables.Skill skillTb = Tables.Skill.Get(_skillInfo.skillKey);
        double skillOffset = skillTb.DamageCoefficient + (_skillInfo.skillLevel * skillTb.AddDamageCoefficient);
        if (skillTb != null)
        {
            finaldamage = Math.Truncate(Damage * skillOffset);
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
        PlaySkillEffect(skillTb.ActionFx);
    }

    public double GetEquipItemAbilityValue(STAT _stat)
    {
        double returnValue = 0;
        for (int i = 0; i < equipItem.Length; i++)
        {
            if (equipItem[i].IsEmpty)
                continue;

            Tables.Item itemTb = Item.Get(equipItem[i].key);
            if (itemTb != null)
            {
                for (int j = 0; j < itemTb.Ability.Length; j++)
                {
                    if (_stat == (STAT)itemTb.Ability[j])
                    {
                        returnValue += itemTb.AbilityValue[j];
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

    public double CalculateGrowthStat(STAT _stat)
    {
        double value = 0;
        Tables.StatReinforce st = StatReinforce.Get((int)_stat);
        if (st != null)
        {
            value = growthLevelDic[_stat] * st.StatValue;
        }
        return value;
    }
}