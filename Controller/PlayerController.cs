using System.Collections;
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

    //Growth
    Dictionary<STAT, int> growthLevelDic = new Dictionary<STAT, int>();

    //Skill
    SkillItem[] equipSkillArr = new SkillItem[4];
    SkillItem equipSkillInfo;
    Dictionary<int, float> skillCoolTime = new Dictionary<int, float>();
    int useSkillNum;
    int equipSkillIndex;
    public int UseSkillSlotIndex => useSkillNum;
    //Equipment
    InvenItem[] equipItem = new InvenItem[6] { new(), new(), new(), new(), new(), new() };



    public List<Node> Path { get; set; }
    int targetIndex;


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
    public float CriRate
    {
        get
        {
            float probability = criRate + (float)CalculateGrowthStat(STAT.CRI_RATE) + (float)GetEquipItemAbilityValue(STAT.CRI_RATE);
            return probability;
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
        set
        {
            curHp = value;
            UpdateHPUI();
        }
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
    public SkillItem[] SkillInfoList
    {
        get
        {
            return equipSkillArr;
        }
        set
        {
            equipSkillArr = value;
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

    public InvenItem[] EquipmentItem
    {
        get => equipItem;
    }

    public Dictionary<STAT, int> GrowthLevelDic
    {
        get
        {
            return growthLevelDic;
        }
        set => growthLevelDic = value;
    }
    public bool IsRangeAttacker { get; set; }

    void Awake()
    {
        if (Instance == null)
            Instance = this;

    }
    void Start()
    {
        InitData((int)job);
        ObjectGetComponent();
    }

    private void OnDrawGizmos()
    {
        if (Path == null)
            return;

        for (int i = 0; i < Path.Count; i++)
        {
            Gizmos.color = Color.green;

            Gizmos.DrawCube(Path[i].worldPos, Vector3.one);
        }
    }

    void Update()
    {

        if (isDead || GameManager.Instance.GameState == GAME_STATE.WIN)
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

        if (targetObj == null)
            return;

        for (int i = 0; i < SkillInfoList.Length; i++)
        {
            if (GameManager.Instance.isAuto && !IsManualControl && IsUseableSkill(i)
                && GetTargetDistance(targetObj.transform.localPosition) <= attackRange)
            {
                if (aniCtrl.CurrentState < OBJ_ANIMATION_STATE.SKILL_1 && aniCtrl.CurrentState != OBJ_ANIMATION_STATE.DIE)
                {
                    UseSkill(i);
                    break;
                }
            }
        }
    }
    void FixedUpdate()
    {
        if (isDead || GameManager.Instance.GameState == GAME_STATE.WIN)
            return;


        if (GameManager.Instance.GameState == GAME_STATE.LOADING || GameManager.Instance.GameState == GAME_STATE.BOSS)
        {
            transform.localPosition = Navigation.Instance.start.worldPos;
            Idle();
        }
        else
        {
            if (IsManualControl || !GameManager.Instance.isAuto)
            {
                if (InputDirection != Vector2.zero)
                    Move();
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
                    if (aniCtrl.CurrentState < OBJ_ANIMATION_STATE.SKILL_1)
                    {
                        if (GetTargetDistance(targetObj.transform) >= AttackRange)
                        {
                            Move();
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
        Idle();
        isDead = false;
        targetObj = null;
        transform.localPosition = Navigation.Instance.start.worldPos;
        StopCoroutine("FollowPath");

        CurHP = MaxHP;
        foreach (var key in SkillCoolTime.Keys.ToList())
        {
            SkillCoolTime[key] = 0;
        }
    }
    public override void ObjectGetComponent()
    {
        aniCtrl = GetComponentInChildren<AnimationController>();
    }
    void InitData(int job)
    {
        characterTb = Tables.Character.Get(job);
        m_TagController = PoolManager.Instance.GetObj("HP_Guage", POOL_TYPE.TAG).GetComponent<TagController>();
        objType = OBJ_TYPE.PLAYER;
        growthLevelDic = DictionaryJsonUtility.FromJson<STAT, int>(AccountManager.Instance.SaveData.characterGrowhLevel); 
        SkillInfoList = AccountManager.Instance.SaveData.equipSkillInfo;

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


        for (int i = 0; i < SkillInfoList.Length; i++)
        {
            if (SkillInfoList[i] != null)
                UIManager.Instance.EquipSkill(i, SkillInfoList[i]);
            else
                UIManager.Instance.EquipSkill(i, new SkillItem());
        }

        TagController.SetTag(this);

    }
    public void Move()
    {
        if (IsManualControl)
        {
            Vector3 dir = new Vector3(InputDirection.x, 0, InputDirection.y).normalized;
            Vector3 targetPos = transform.localPosition + dir.normalized * MoveSpd * Time.deltaTime;
            transform.localPosition = Vector3.MoveTowards(transform.position, targetPos, MoveSpd * Time.deltaTime);
            transform.LookAt(dir + transform.localPosition);
        }
        else
        {
            if (targetObj != null && !targetObj.IsDead)
                Navigation.Instance.RequestPath(transform.localPosition, targetObj.transform.localPosition, OnPathFound);
        }
        if (aniCtrl.CurrentState != OBJ_ANIMATION_STATE.MOVE)
            ChangeState(OBJ_ANIMATION_STATE.MOVE);
    }
    public void OnPathFound(List<Node> newPath, bool pathSuccessful)
    {
        if (pathSuccessful)
        {
            Path = new List<Node>(newPath);
            targetIndex = 0;
            StopCoroutine("FollowPath");
            StartCoroutine("FollowPath");
        }
        else
        {
            Idle();
        }
    }
    IEnumerator FollowPath()
    {
        if (Path == null || Path.Count == 0)
        {
            yield break;
        }
        Node currentWaypoint = Path[0];


        while (true)
        {
            if (Navigation.Instance.NodeFromWorldPoint(transform.localPosition) == currentWaypoint)
            {
                targetIndex++;
                if (targetIndex >= Path.Count)
                {
                    yield break;
                }
                currentWaypoint = Path[targetIndex];
            }
            Rotate(currentWaypoint.worldPos - transform.localPosition);
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, currentWaypoint.worldPos, MoveSpd * Time.deltaTime);
            if (aniCtrl.CurrentState != OBJ_ANIMATION_STATE.MOVE)
            {
                StopCoroutine("FollowPath");
            }
            yield return null;
        }
    }
    public void Rotate(Vector3 dir)
    {
        //transform.localRotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 100 * Time.deltaTime);

        //방법 1
        //float distanceToTarget = dir.magnitude;

        //if (distanceToTarget < attackRange)
        //{
        //    // 타겟과 너무 가까우면 바로 바라보도록 설정
        //    transform.localRotation = Quaternion.LookRotation(dir);
        //}
        //else
        //{
        //    // 타겟과 거리가 충분하면 부드럽게 회전
        //    transform.localRotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 100 * Time.deltaTime);
        //}


        //방법 2
        float distanceToTarget = dir.magnitude;
        Debug.Log(distanceToTarget);
        float rotationSpeed = Mathf.Clamp((100 * distanceToTarget) * Time.deltaTime, 0, 1); // 거리에 따라 회전 속도 조정

        transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.LookRotation(dir), rotationSpeed);

        //방법 3
        //float rotationSpeed = 100 * Time.deltaTime;
        //Quaternion targetRotation = Quaternion.LookRotation(dir);
        //transform.localRotation = Quaternion.RotateTowards(transform.localRotation, targetRotation, rotationSpeed);
    }
    public void SetMoveEvent()
    {
    }

    void Attack()
    {
        if (aniCtrl.IsPlayingAnimation("SKILL"))
            return;

        if (aniCtrl.CurrentState != OBJ_ANIMATION_STATE.ATTACK)
        {
            ChangeState(OBJ_ANIMATION_STATE.ATTACK);
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
    }

    public IEnumerator SetDeadEvent()
    {
        isDead = true;
        CurHP = 0;
        GameManager.Instance.ChangeGameState(GAME_STATE.END);
        yield return null;
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
            if (GameManager.Instance.isTest)
            {
                if (!SkillCoolTime.ContainsKey(skillTb.key))
                    SkillCoolTime.Add(_index, 0);
                else
                    SkillCoolTime[_index] = 0;

                return;
            }

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
        Tables.Skill skill = Tables.Skill.Get(SkillInfoList[_index].key);
        if (skill != null)
        {
            ChangeState((OBJ_ANIMATION_STATE)skill.SkillAnimation);
            Rotate(TargetDir);
            SetSkillCoolDown(SkillInfoList[_index].key);
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
        return SkillInfoList[_num] != null && !SkillInfoList[_num].IsEmpty && SkillCoolTime[SkillInfoList[_num].key] <= 0 && GameManager.Instance.GameState == GAME_STATE.PLAYING;
    }

    public void GetDamage(double _damage)
    {
        double finalDam = System.Math.Truncate(_damage - Defense);

        if (finalDam <= 0)
            finalDam = 1;
        curHp -= finalDam;
        TagController.SetDamageFontText(finalDam);
        if (curHp <= 0)
        {
            ChangeState(OBJ_ANIMATION_STATE.DIE);
        }
    }
    public double CalculateAttDam()
    {
        return System.Math.Truncate(IsCri ? (Damage * 2) + CriDam : Damage); ;
    }
    public double CalculateSkillDamage(SkillItem _skillInfo)
    {
        double finaldamage = 0;
        Tables.Skill skillTb = Tables.Skill.Get(_skillInfo.key);
        double skillOffset = skillTb.DamageCoefficient + (_skillInfo.enhanceCount * skillTb.AddDamageCoefficient);
        if (skillTb != null)
        {
            finaldamage = System.Math.Truncate(Damage * skillOffset);
        }

        return finaldamage;
    }

    public void PlaySkillEffect(string _name)
    {
        GameObject go = EffectManager.instance.GetEffect(_name);
        go.transform.parent = effectRoot;
        go.transform.localPosition = Vector3.zero;
        go.transform.eulerAngles = transform.eulerAngles;
        EffectManager.instance.PlayEffect(go);
    }

    public void SkillAniEvent(int _type)
    {
        Tables.Skill skillTb = Tables.Skill.Get(SkillInfoList[useSkillNum].key);
        PlaySkillEffect(skillTb.ActionFx[_type]);
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

    public void EquipOrReplaceItem(InvenItem _item)
    {
        Tables.Item itemTb = Item.Get(_item.key);
        int itemType = itemTb.ItemType;

        // 이미 장착된 아이템이 있거나 동일한 아이템을 장착하려는 경우
        if (!equipItem[itemType].IsEmpty || _item.key == equipItem[itemType].key)
        {
            DequipItem(itemType);
        }
        else
        {
            _item.isEquipped = true;
            equipItem[itemType] = _item;
        }
        UICharactorInfo.instance.SetEquippingItemSlot(itemType);
    }

    public void DequipItem(int _index)
    {
        equipItem[_index].isEquipped = false;
        equipItem[_index] = new InvenItem();
        UICharactorInfo.instance.SetEquippingItemSlot(_index);
    }

    public double CalculateGrowthStat(STAT _stat)
    {
        double value = 0;
        Tables.StatReinforce st = StatReinforce.Get((int)_stat);
        if (st != null)
        {
            value = GrowthLevelDic[_stat] * st.StatValue;
        }
        return value;
    }

    public void AttackAniEvent(IHittable _target)
    {
        if (_target.IsDead)
            return;
        Vector3 targetPos = ((MonoBehaviour)_target).transform.localPosition;
        if (GetTargetDistance(targetPos) <= AttackRange)
        {
            Rotate(targetPos - transform.localPosition);
            _target.GetDamage(CalculateAttDam());
        }
    }

}