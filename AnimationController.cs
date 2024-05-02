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
        switch (_state)
        {
            case OBJ_ANIMATION_STATE.IDLE:
                animator.SetBool("IsIdle", true);
                break;
            case OBJ_ANIMATION_STATE.DIE:
                animator.SetBool("IsDead", true);
                break;
            case OBJ_ANIMATION_STATE.MOVE:
                animator.SetBool("IsMove", true);
                break;
            case OBJ_ANIMATION_STATE.ATTACK:
                animator.SetBool("IsAttack", true);
                break;
            case OBJ_ANIMATION_STATE.LESS_HP:
                animator.SetBool("LessHp", true);
                break;
        }
        currentState = _state;
    }
    void ExitAnimationState(OBJ_ANIMATION_STATE _state)
    {
        switch (_state)
        {
            case OBJ_ANIMATION_STATE.IDLE:
                animator.SetBool("IsIdle", false);
                break;
            case OBJ_ANIMATION_STATE.DIE:
                animator.SetBool("IsDead", false);
                break;
            case OBJ_ANIMATION_STATE.MOVE:
                animator.SetBool("IsMove", false);
                break;
            case OBJ_ANIMATION_STATE.ATTACK:
                animator.SetBool("IsAttack", false);
                break;
            case OBJ_ANIMATION_STATE.LESS_HP:
                animator.SetBool("LessHp", false);
                break;
        }
        prevState = currentState;
    }
    public void AttackEvent()
    {

    }
    public void EndAttackAni()
    {
        //animator.SetInteger("AttackCount", m_Attackable.AttackCount);
    }

    public void MoveEvent()
    {
    }

}
