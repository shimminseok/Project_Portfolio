using Tables;
using Unity.VisualScripting;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    OBJ_ANIMATION_STATE currentState;

    OBJ_ANIMATION_STATE prevState;
    ObjectController m_Controller;
    Animator animator;


    public OBJ_ANIMATION_STATE CurrentState { set { currentState = value; } }

    void Start()
    {
        m_Controller = GetComponentInParent<ObjectController>();
        animator = GetComponent<Animator>();
        EnterAnimationState(OBJ_ANIMATION_STATE.IDLE);
    }
    void Update()
    {
        if (prevState != currentState)
            ChangeAnimation(currentState);
    }
    void ChangeAnimation(OBJ_ANIMATION_STATE _state)
    {
        ExitAnimationState(prevState);
        EnterAnimationState(_state);
    }
    void EnterAnimationState(OBJ_ANIMATION_STATE _state)
    {
        animator.SetInteger("State",(int)_state);
        //switch (_state)
        //{
        //    case OBJ_ANIMATION_STATE.IDLE:
        //        animator.SetBool("IsIdle", true);
        //        break;
        //    case OBJ_ANIMATION_STATE.DIE:
        //        animator.SetBool("IsDead", true);
        //        break;
        //    case OBJ_ANIMATION_STATE.MOVE:
        //        animator.SetBool("IsMove", true);
        //        break;
        //    case OBJ_ANIMATION_STATE.ATTACK:
        //        animator.SetBool("IsAttack", true);
        //        break;
        //    case OBJ_ANIMATION_STATE.LESS_HP:
        //        animator.SetBool("LessHp", true);
        //        break;
        //    case OBJ_ANIMATION_STATE.SKILL_1:
        //        animator.SetInteger("SkillNum", 1);
        //        break;
        //    case OBJ_ANIMATION_STATE.SKILL_2:
        //        animator.SetInteger("SkillNum", 2);
        //        break;
        //}
        currentState = _state;
    }
    void ExitAnimationState(OBJ_ANIMATION_STATE _state)
    {
        animator.SetInteger("State",(int)_state);
        //switch (_state)
        //{
        //    case OBJ_ANIMATION_STATE.IDLE:
        //        animator.SetBool("IsIdle", false);
        //        break;
        //    case OBJ_ANIMATION_STATE.DIE:
        //        animator.SetBool("IsDead", false);
        //        break;
        //    case OBJ_ANIMATION_STATE.MOVE:
        //        animator.SetBool("IsMove", false);
        //        break;
        //    case OBJ_ANIMATION_STATE.ATTACK:
        //        animator.SetBool("IsAttack", false);
        //        break;
        //    case OBJ_ANIMATION_STATE.LESS_HP:
        //        animator.SetBool("LessHp", false);
        //        break;
        //    case OBJ_ANIMATION_STATE.SKILL_1:
        //    case OBJ_ANIMATION_STATE.SKILL_2:
        //        animator.SetInteger("SkillNum", 0);
        //        break;
        //}
        prevState = currentState;
    }
    public void AttackEvent()
    {
        switch(m_Controller.objType)
        {
            case OBJ_TYPE.PLAYER:
                PlayerController characterCon = m_Controller as PlayerController;
                MonsterController targetMon =  characterCon.Target as MonsterController;
                targetMon.SetDamage(characterCon);
                break;

            case OBJ_TYPE.MONSTER:
                MonsterController monsterCon = m_Controller as MonsterController;
                PlayerController.Instance.SetDamage(monsterCon);
                break;
        }
    }
    public void EndAttackAni()
    {

    }

    public void MoveEvent()
    {

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
                PoolManager.Instance.PushObj(monsterCon.name, POOL_TYPE.MONSTER, monsterCon.gameObject);
                monsterCon.Init();
                break;
        }
    }

}
