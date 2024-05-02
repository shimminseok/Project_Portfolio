using UnityEngine;

public class PlayerController : ObjectController, IMoveable, IAttackable, IHittable, IControlable
{
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

    void Awake()
    {

    }
    void Start()
    {
        InitData((int)job);
        ObjectGetComponent();
    }
    void Update()
    {
        HpGen();
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
    void FixedUpdate()
    {
        if (IsManualControl)
        {
            Move(new Vector3(InputDirection.x, 0, InputDirection.y));
        }
        //수동 조작이 아니면 적을 찾음
        else if (!IsManualControl)
        {
            if (targetObj != null && !targetObj.IsDead)
            {
                if (GetTargetDistance(targetObj.transform) > attackRange)
                {
                    Move(targetObj.transform.localPosition - transform.localPosition);
                }
                else
                {
                    Attack();
                }
            }
            else
            {
                aniCtrl.CurrentState = OBJ_ANIMATION_STATE.IDLE;
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
        objType = OBJ_TYPE.COLLEAGUE;
        //GameManager.Instance.CreateCharacterPrefab(characterTb.Prefab, transform);
        InitHitData();
        InitAttackData();
        InitMoveData();
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
        Vector3 movement = dir * MoveSpd * Time.fixedDeltaTime;
        transform.Translate(movement, Space.World);
        Rotate(movement);
        SetMoveEvent();
    }
    public void Rotate(Vector3 dir)
    {
        Quaternion rotation = Quaternion.LookRotation(dir, Vector3.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.fixedDeltaTime * 10f);
    }
    public void SetMoveEvent()
    {
        aniCtrl.CurrentState = OBJ_ANIMATION_STATE.MOVE;
    }
    public void SetMoveSpeed(float _moveSpeed)
    {
        MoveSpd = _moveSpeed;
    }
    void Attack()
    {
        targetObj.SetDamage(this);
        SetAttackEvent();
    }
    public void SetAttackEvent()
    {
        aniCtrl.CurrentState = OBJ_ANIMATION_STATE.ATTACK;
    }
    public void SetDamage(IAttackable _attacker)
    {
        //TO DO
        //방어력 계산후 데미지 설정

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
        aniCtrl.CurrentState = OBJ_ANIMATION_STATE.DIE;
    }
    public float GetTargetDistance(Transform _target)
    {
        return Vector3.Distance(transform.localPosition, _target.localPosition);
    }
}