using System;
using System.Collections.Generic;
using System.Linq;
using Tables;
using UnityEngine;

public class MonsterController : ObjectController, IAttackable, IMoveable, IHittable
{
    public Monster monsterTb;
    PlayerController targetObj;

    TagController m_TagController;
    MONSTER_TYPE monsterType;
    public Rigidbody rigi;

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


    List<Node> finalNodeList;
    Node targetNode;
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
        set => curHp = value;
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

    //Vector3 targetVec;

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
            ChangeState(OBJ_ANIMATION_STATE.READY);
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
        maxHp = curHp = monsterTb.HealthPoint * monLevel;
        hpRegen = monsterTb.HealthPointRegen;
        accuracy = monsterTb.Hit;
        dodge = monsterTb.Dodge;
        damage = monsterTb.Attack * monLevel;
        defense = monsterTb.DefencePoint * monLevel;
        targetObj = FindObjectOfType(typeof(PlayerController)) as PlayerController;
        aniCtrl.ChangeAnimation(OBJ_ANIMATION_STATE.IDLE);
        m_TagController = PoolManager.Instance.GetObj("HP_Guage", POOL_TYPE.TAG).GetComponent<TagController>();
        m_TagController.SetTag(this);

        attackSpd = monsterTb.AttackSpeed;
        attackRange = monsterTb.AttackRange;
        moveSpeed = monsterTb.MoveSpeed;
    }
    public void SetMonster(Vector3 _pos, MONSTER_TYPE _type, int _monLv)
    {
        monsterType = _type;
        Vector3 pos = UnityEngine.Random.insideUnitSphere;
        pos.y = 0;
        pos *= 2;
        transform.localPosition = pos;
        transform.localPosition += _pos;
        transform.localScale *= monsterTb.Scale;
        monLevel = _monLv;
        Init();

    }
    public void Move(Vector3 dir)
    {
        //캐릭터를 찾아서 따라감
        if (isDead)
            return;

        //노드에 도착하면 노드를 바꿔줘야함
        finalNodeList = Navigation.Instance.FindPath(new Vector2(transform.localPosition.x, transform.localPosition.z), new Vector2(targetObj.transform.localPosition.x, targetObj.transform.localPosition.z));

        for (int i = 0; i < finalNodeList.Count; i++)
        {
            if (targetNode == null)
            {
                targetNode = finalNodeList.LastOrDefault();
                break;
            }
            Vector3 tmpVec = new Vector3(transform.localPosition.x, transform.localPosition.z, 0);
            if (tmpVec == finalNodeList[i].Position)
            {
                targetNode = finalNodeList[i - 1];
                break;
            }
        }

        if (targetNode != null)
        {
            Vector3 targetVec = new Vector3(targetNode.Position.x, 0, targetNode.Position.y);
            if (GetTargetDistance(targetVec) < 1)
            {
                finalNodeList.RemoveAt(finalNodeList.Count - 1);
            }
            transform.LookAt(targetVec);
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, targetVec, MoveSpd * Time.deltaTime);
            SetMoveEvent();
        }
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
    public void SetDeadEvent()
    {
        IsDead = true;
        CurHP = 0;
        uint gold = 0;
        switch (monsterType)
        {
            case MONSTER_TYPE.COMMON:
                gold = MonsterManager.instance.CurrentStageTb.MonsterGold;
                break;
            case MONSTER_TYPE.ELETE:
                gold = MonsterManager.instance.CurrentStageTb.MonsterGold;
                break;
            case MONSTER_TYPE.BOSS:
                //스테이지 클리어
                GameManager.Instance.ChangeGameState(GAME_STATE.WIN);
                MonsterManager.instance.isChallengeableBoss = false;
                PlayerController.Instance.ChangeState(OBJ_ANIMATION_STATE.WIN);
                UIManager.Instance.OnClickOpenPopUp(UIStageClear.instance);
                break;
        }
        AccountManager.Instance.AddGoods(GOOD_TYPE.GOLD, gold);
        ChangeState(OBJ_ANIMATION_STATE.DIE);
        MonsterManager.instance.RemoveMonsterList(this);
    }

    public void SetMoveEvent()
    {
        ChangeState(OBJ_ANIMATION_STATE.MOVE);

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
        double finalDam = Math.Truncate(_damage - defense);
        curHp -= finalDam <= 0 ? 1 : finalDam;
        TagController.SetDamageFontText(finalDam);

        UpdateHPUI();
        if (curHp <= 0)
        {
            SetDeadEvent();
        }
    }
    public double CalculateAttDam()
    {
        return Math.Truncate(isCri ? Damage * 2 + CriDam : Damage); ;
    }
}
