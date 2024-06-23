using System;
using System.Collections;
using System.Collections.Generic;
using Tables;
using UnityEngine;

public class MonsterController : ObjectController, IAttackable, IMoveable, IHittable
{
    public Monster monsterTb;
    PlayerController targetObj;

    TagController m_TagController;
    MONSTER_TYPE monsterType;
    [SerializeField] Rigidbody rigi;

    double maxHp;
    double curHp;
    double hpRegen;
    float genTime;

    double defense;
    double criDam;
    double damage;
    float attackSpd;
    float attackRange;
    float accuracy;
    float dodge;
    float moveSpeed;

    bool isCri;
    bool isDead;

    int targetIndex;

    public float MoveSpd
    {
        get => moveSpeed;
    }
    public float GenTime
    {
        get => genTime;
        set => genTime = value;
    }
    public bool IsDead
    {
        get => isDead;
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
        set
        {
            curHp = value;
            UpdateHPUI();
        }
    }
    public double HPRegen
    {
        get => hpRegen;
        set => hpRegen = value;
    }
    public double Defense
    {
        get
        {
            return defense;
        }
        set => defense = value;
    }
    public double Damage
    {
        get => damage;
        set => damage = value;
    }
    public float AttackSpd
    {
        get => attackSpd;
    }
    public float AttackRange
    {
        get => attackRange;
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
    }
    public ObjectController Target => targetObj;


    public Vector3 TargetDir => Target.transform.localPosition - transform.localPosition;

    public List<Node> Path { get; set; }
    public bool IsRangeAttacker { get; set; }

    int monLevel = 1;
    private void Awake()
    {
        ObjectGetComponent();
        objType = OBJ_TYPE.MONSTER;
    }

    void FixedUpdate()
    {
        if (isDead)
            return;

        if (GameManager.Instance.GameState == GAME_STATE.END || GameManager.Instance.GameState == GAME_STATE.READY)
        {
            ChangeState(OBJ_ANIMATION_STATE.IDLE);
        }
        else if (GameManager.Instance.GameState == GAME_STATE.BOSS)
        {
            ChangeState(OBJ_ANIMATION_STATE.IDLE);
        }
        else
        {
            if (targetObj != null)
            {
                if (targetObj.IsDead)
                {
                    FindEnemy();
                }
                else
                {
                    if (GetTargetDistance(targetObj.transform) > attackRange)
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

    public override void Init()
    {
        isDead = false;
        maxHp = curHp = monsterTb.HealthPoint * monLevel;
        hpRegen = monsterTb.HealthPointRegen;
        accuracy = monsterTb.Hit;
        dodge = monsterTb.Dodge;
        damage = monsterTb.Attack * monLevel;
        defense = monsterTb.DefencePoint * monLevel;
        targetObj = FindObjectOfType(typeof(PlayerController)) as PlayerController;
        m_TagController = PoolManager.Instance.GetObj("HP_Guage", POOL_TYPE.TAG).GetComponent<TagController>();
        m_TagController.SetTag(this);

        attackSpd = monsterTb.AttackSpeed;
        attackRange = monsterTb.AttackRange;
        moveSpeed = monsterTb.MoveSpeed;

        IsRangeAttacker = monsterTb.AttackType != 1;

        aniCtrl.ChangeAnimation(OBJ_ANIMATION_STATE.IDLE);
    }
    public void SetMonster(Vector3 _pos, MONSTER_TYPE _type, int _monLv)
    {
        monsterType = _type;
        transform.localPosition = _pos;
        transform.localScale *= monsterTb.Scale;
        monLevel = _monLv;
        Init();
    }
    public void Move()
    {
        if (isDead)
            return;

        ChangeState(OBJ_ANIMATION_STATE.MOVE);
        if (targetObj != null && !targetObj.IsDead)
        {
            Navigation.Instance.RequestPath(transform.localPosition, targetObj.transform.localPosition, OnPathFound);
        }
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
            if (transform.localPosition == currentWaypoint.worldPos)
            {
                targetIndex++;
                if (targetIndex >= Path.Count)
                {
                    yield break;
                }
                currentWaypoint = Path[targetIndex];
            }

            Vector3 dir = currentWaypoint.worldPos - transform.localPosition;
            transform.localRotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 10 * Time.deltaTime);
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, currentWaypoint.worldPos, MoveSpd * Time.deltaTime);
            if (aniCtrl.CurrentState != OBJ_ANIMATION_STATE.MOVE)
            {
                StopCoroutine("FollowPath");
            }
            yield return null;
        }
    }
    void Attack()
    {
        ChangeState(OBJ_ANIMATION_STATE.ATTACK);
        Rotate(TargetDir);
    }
    public override void ObjectGetComponent()
    {
        aniCtrl = GetComponent<AnimationController>();
        rigi = GetComponent<Rigidbody>();
        if (rigi == null)
            rigi = gameObject.AddComponent<Rigidbody>();
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
    public IEnumerator SetDeadEvent()
    {
        yield return new WaitUntil(() => aniCtrl.m_Animator.GetCurrentAnimatorClipInfo(0)[0].clip.name == "dead");
        yield return new WaitUntil(() => !aniCtrl.IsPlayingAnimation("DEAD"));
        PushObj();

    }
    public void PushObj()
    {
        PoolManager.Instance.PushObj(gameObject.name, POOL_TYPE.MONSTER, gameObject);
        PoolManager.Instance.PushObj(TagController.name, POOL_TYPE.TAG, TagController.gameObject);
    }
    void Dead()
    {
        IsDead = true;
        CurHP = 0;
        uint gold = 0;
        switch (monsterType)
        {
            case MONSTER_TYPE.COMMON:
                {
                    GameManager.Instance.GetReward(MonsterManager.instance.CurrentStageTb.StageIdleReward, out bool result);
                    if (result)
                    {
                        UIQuest.instance.IncreaseQuestCount(QUEST_CARTEGORY.KILL_MONSTER,1);
                    }
                }
                break;

            case MONSTER_TYPE.BOSS:
                //스테이지 클리어
                GameManager.Instance.ChangeGameState(GAME_STATE.WIN);
                UIQuest.instance.IncreaseQuestCount(QUEST_CARTEGORY.CLEAR_STAGE,1);
                break;
        }
        AccountManager.Instance.AddGoods(GOOD_TYPE.GOLD, gold);
        ChangeState(OBJ_ANIMATION_STATE.DIE);
        MonsterManager.instance.RemoveMonsterList(this);
    }
    public void SetMoveEvent()
    {
        //ChangeState(OBJ_ANIMATION_STATE.MOVE);
    }

    public void UpdateHPUI()
    {
        TagController.UpdateHPUI(MaxHP, CurHP);
    }

    public float GetTargetDistance(Transform _target)
    {
        return Vector3.Distance(transform.localPosition, _target.transform.localPosition);
    }
    public float GetTargetDistance(Vector3 _target)
    {
        return Vector3.Distance(transform.localPosition, _target);
    }
    public override void FindEnemy()
    {
        targetObj = PlayerController.Instance;
    }

    public void GetDamage(double _damage)
    {
        double finalDam = Math.Truncate(_damage - Defense);
        CurHP -= finalDam <= 0 ? 1 : finalDam;

        TagController.SetDamageFontText(finalDam);
        if (CurHP <= 0)
        {
            Dead();
        }
    }
    public double CalculateAttDam()
    {
        return Math.Truncate(isCri ? Damage * 2 + CriDam : Damage); ;
    }

    public void AttackAniEvent(IHittable _target)
    {
        if (_target.IsDead)
            return;

        _target.GetDamage(CalculateAttDam());
    }
}
