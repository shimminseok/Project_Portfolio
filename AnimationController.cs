using Tables;
using Unity.VisualScripting;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    OBJ_ANIMATION_STATE currentState;

    OBJ_ANIMATION_STATE prevState;
    ObjectController m_Controller;
    Animator animator;


    public OBJ_ANIMATION_STATE GetAniState { get => currentState; }
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
        animator.SetInteger("State",(int)_state);
    }
    void ExitAnimationState(OBJ_ANIMATION_STATE _state)
    {
        animator.SetInteger("State",(int)_state);
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
        ChangeAnimation(OBJ_ANIMATION_STATE.IDLE);
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
