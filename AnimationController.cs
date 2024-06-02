using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    OBJ_ANIMATION_STATE currentState;

    OBJ_ANIMATION_STATE prevState;
    ObjectController m_Controller;
    Animator animator;


    public OBJ_ANIMATION_STATE GetAniState { get => currentState; }
    public Animator m_Animator => animator;
    void Start()
    {
        m_Controller = GetComponentInParent<ObjectController>();
        animator = GetComponent<Animator>();
        EnterAnimationState(OBJ_ANIMATION_STATE.IDLE);
    }
    public void ChangeAnimation(OBJ_ANIMATION_STATE _state)
    {

        currentState = _state;
        if (prevState != currentState)
        {
            ExitAnimationState(prevState);
            EnterAnimationState(_state);
        }
    }
    void EnterAnimationState(OBJ_ANIMATION_STATE _state)
    {
        switch(_state)
        {
            case OBJ_ANIMATION_STATE.ATTACK:
                {
                    IAttackable attackable = m_Controller.GetComponent<IAttackable>();
                    if(attackable != null)
                    {
                        animator.speed = GameManager.Instance.GameSpeed * attackable.AttackSpd;
                    }
                }
                break;
            case OBJ_ANIMATION_STATE.MOVE:
                {
                    IMoveable moveable = m_Controller.GetComponent<IMoveable>();
                    if (moveable != null)
                    {
                        animator.speed = GameManager.Instance.GameSpeed * moveable.MoveSpd;
                    }
                }
                break;
            case OBJ_ANIMATION_STATE.WIN:
                {
                }
                break;
            default:
                animator.speed = GameManager.Instance.GameSpeed;
                break;
        }
        animator.SetInteger("State", (int)_state);
    }
    void ExitAnimationState(OBJ_ANIMATION_STATE _state)
    {
        animator.SetInteger("State", (int)_state);
        prevState = currentState;
    }
    public void AttackEvent()
    {

        switch (m_Controller.objType)
        {
            case OBJ_TYPE.PLAYER:
                PlayerController characterCon = m_Controller as PlayerController;
                MonsterController targetMon = characterCon.Target as MonsterController;
                if (characterCon.GetTargetDistance(targetMon.transform) <= characterCon.AttackRange)
                    targetMon.GetDamage(characterCon.CalculateAttackDamage());
                break;

            case OBJ_TYPE.MONSTER:
                MonsterController monsterCon = m_Controller as MonsterController;
                switch (monsterCon.Target.objType)
                {
                    case OBJ_TYPE.PLAYER:
                        PlayerController.Instance.GetDamage(monsterCon.CalculateAttackDamage());
                        break;
                    case OBJ_TYPE.COLLEAGUE:
                        break;
                }
                break;
        }
    }
    public void EndAttackAni()
    {
        ChangeAnimation(OBJ_ANIMATION_STATE.IDLE);
    }
    public void EndSkillAni()
    {
        ChangeAnimation(OBJ_ANIMATION_STATE.IDLE);
    }
    public void MoveEvent()
    {

    }
    public void SkillEvent()
    {
        IUseSkill skillEvent = m_Controller.GetComponent<IUseSkill>();
        if(skillEvent != null)
        {
            skillEvent.SkillAniEvent();
        }

    }
    public void DeadEvent()
    {
        switch (m_Controller.objType)
        {
            case OBJ_TYPE.PLAYER:
                PlayerController characterCon = m_Controller as PlayerController;
                break;

            case OBJ_TYPE.MONSTER:
                MonsterController monsterCon = m_Controller as MonsterController;
                PoolManager.Instance.PushObj(gameObject.name, POOL_TYPE.MONSTER, gameObject);
                PoolManager.Instance.PushObj(monsterCon.TagController.name, POOL_TYPE.TAG, monsterCon.TagController.gameObject);
                break;
        }

    }
    public bool IsPlayingAnimation(string _name)
    {
        return animator.GetCurrentAnimatorStateInfo(0).IsTag(_name);
    }

}
