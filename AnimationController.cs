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
                targetMon.GetDamage(characterCon.CalculateAttackDamage());
                break;

            case OBJ_TYPE.MONSTER:
                MonsterController monsterCon = m_Controller as MonsterController;
                switch(monsterCon.Target.objType)
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
                PoolManager.Instance.PushObj(monsterCon.TagController.name, POOL_TYPE.TAG, monsterCon.TagController.gameObject);
                monsterCon.Init();
                break;
        }
    }
    public bool IsPlayingAnimation(string _name)
    {
        return animator.GetCurrentAnimatorStateInfo(0).IsTag(_name);
    }

}
