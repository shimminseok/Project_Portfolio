using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    [SerializeField] List<AnimationClip> m_Clips;
    [SerializeField] List<GameObject> effectList;

    OBJ_ANIMATION_STATE currentState;

    ObjectController m_Controller;
    Animator animator;

    public OBJ_ANIMATION_STATE CurrentState { get => currentState; }

    public List<GameObject> EffectList { get { return effectList; } set { effectList = value; } }
    public Animator m_Animator => animator;
    void Awake()
    {
        m_Controller = GetComponentInParent<ObjectController>();
        animator = GetComponent<Animator>();
    }
    void Start()
    {
        animator.runtimeAnimatorController = ChangeClip();
        EnterAnimationState(OBJ_ANIMATION_STATE.IDLE);
    }
    AnimatorOverrideController ChangeClip()
    {
        AnimatorOverrideController overrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
        for (int i = 0; i < m_Clips.Count; i++)
        {
            overrideController[m_Clips[i].name] = m_Clips[i];

        }
        return overrideController;

    }
    public void ChangeAnimation(OBJ_ANIMATION_STATE _state)
    {
        EnterAnimationState(_state);
    }

    void EnterAnimationState(OBJ_ANIMATION_STATE _state)
    {
        float speed = 1;
        string clipName = GetClipNameAndSpeed(_state, ref speed);

        animator.speed = (GameManager.Instance.GameSpeed / 1.5f) * speed;
        currentState = _state;
        animator.CrossFade(clipName, 0.1f);


        switch (_state)
        {
            case OBJ_ANIMATION_STATE.IDLE:
                clipName = "idle";
                break;
            case OBJ_ANIMATION_STATE.DIE:
                clipName = "dead";
                break;
            case OBJ_ANIMATION_STATE.MOVE:
                {
                    if (m_Controller.TryGetComponent<IMoveable>(out var moveable))
                    {
                        speed = moveable.MoveSpd;
                    }
                    clipName = "move";
                }
                break;
            case OBJ_ANIMATION_STATE.ATTACK:
                {
                    if (m_Controller.TryGetComponent<IAttackable>(out var attackable))
                    {
                        speed = attackable.AttackSpd;
                    }
                    clipName = "atk";
                }
                break;
            case OBJ_ANIMATION_STATE.WIN:
                {
                }
                clipName = "win";
                break;
            case OBJ_ANIMATION_STATE.SKILL_1:
                clipName = "skill01";
                break;
            case OBJ_ANIMATION_STATE.SKILL_2:
                clipName = "skill02";
                break;
            case OBJ_ANIMATION_STATE.SKILL_3:
                clipName = "skill03";
                break; ;
            case OBJ_ANIMATION_STATE.SKILL_4:
                clipName = "skill04";
                break;
            case OBJ_ANIMATION_STATE.SKILL_5:
                clipName = "skill05";
                break;
            case OBJ_ANIMATION_STATE.SKILL_6:
                clipName = "skill06";
                break;
            case OBJ_ANIMATION_STATE.SKILL_7:
                clipName = "skill07";
                break;
            case OBJ_ANIMATION_STATE.SKILL_8:
                clipName = "skill08";
                break;
            case OBJ_ANIMATION_STATE.SKILL_9:
                clipName = "skill09";
                break;
            case OBJ_ANIMATION_STATE.SKILL_10:
                clipName = "skill10";
                break;
        }

        animator.speed = (GameManager.Instance.GameSpeed / 1.5f) * speed;
        currentState = _state;
        animator.CrossFade(clipName, 0.1f);
        HandleStateEvents(_state);
    }

    string GetClipNameAndSpeed(OBJ_ANIMATION_STATE _state, ref float _speed)
    {
        switch (_state)
        {
            case OBJ_ANIMATION_STATE.IDLE:
                return "idle";

            case OBJ_ANIMATION_STATE.DIE:
                return "dead";

            case OBJ_ANIMATION_STATE.MOVE:
                if (m_Controller.TryGetComponent<IMoveable>(out var moveable))
                {
                    _speed = moveable.MoveSpd;
                }
                return "move";

            case OBJ_ANIMATION_STATE.ATTACK:
                if (m_Controller.TryGetComponent<IAttackable>(out var attackable))
                {
                    _speed = attackable.AttackSpd;
                }
                return "atk";

            case OBJ_ANIMATION_STATE.WIN:
                return "win";

            case OBJ_ANIMATION_STATE.SKILL_1:
                return "skill01";

            case OBJ_ANIMATION_STATE.SKILL_2:
                return "skill02";

            case OBJ_ANIMATION_STATE.SKILL_3:
                return "skill03";

            case OBJ_ANIMATION_STATE.SKILL_4:
                return "skill04";

            case OBJ_ANIMATION_STATE.SKILL_5:
                return "skill05";

            case OBJ_ANIMATION_STATE.SKILL_6:
                return "skill06";

            case OBJ_ANIMATION_STATE.SKILL_7:
                return "skill07";

            case OBJ_ANIMATION_STATE.SKILL_8:
                return "skill08";

            case OBJ_ANIMATION_STATE.SKILL_9:
                return "skill09";

            case OBJ_ANIMATION_STATE.SKILL_10:
                return "skill10";

            default:
                return string.Empty;
        }
    }
    void HandleStateEvents(OBJ_ANIMATION_STATE _state)
    {
        switch (_state)
        {
            case OBJ_ANIMATION_STATE.DIE:
                if (m_Controller.TryGetComponent<IHittable>(out var hittable))
                    StartCoroutine(hittable.SetDeadEvent());
                break;
            case OBJ_ANIMATION_STATE.IDLE:
            case OBJ_ANIMATION_STATE.MOVE:
            case OBJ_ANIMATION_STATE.ATTACK:
            case OBJ_ANIMATION_STATE.WIN:
            case OBJ_ANIMATION_STATE.SKILL_1:
            case OBJ_ANIMATION_STATE.SKILL_2:
            case OBJ_ANIMATION_STATE.SKILL_3:
            case OBJ_ANIMATION_STATE.SKILL_4:
            case OBJ_ANIMATION_STATE.SKILL_5:
            case OBJ_ANIMATION_STATE.SKILL_6:
            case OBJ_ANIMATION_STATE.SKILL_7:
            case OBJ_ANIMATION_STATE.SKILL_8:
            case OBJ_ANIMATION_STATE.SKILL_9:
            case OBJ_ANIMATION_STATE.SKILL_10:
                break;
        }
    }
    public void AttackEvent()
    {
        if (m_Controller.TryGetComponent<IAttackable>(out var iAtk))
        {
            if (iAtk.Target != null)
            {
                if (iAtk.Target.TryGetComponent<IHittable>(out var ihit))
                    iAtk.AttackAniEvent(ihit);
            }
        }
    }
    public bool IsPlayingAnimation(string _tag)
    {
        var curAni = animator.GetCurrentAnimatorStateInfo(0);
        if (curAni.IsTag(_tag))
        {
            float time = curAni.normalizedTime;
            if (time >= 1)
                return false;
            else
                return true;
        }
        else
            return false;
    }
    public void EndSkillAni()
    {
        if (currentState != OBJ_ANIMATION_STATE.DIE)
            ChangeAnimation(OBJ_ANIMATION_STATE.IDLE);
    }
    public void MoveEvent()
    {
        if (m_Controller.TryGetComponent<IMoveable>(out var imov))
        {
            imov.SetMoveEvent();
        }
    }
    public void effect(int _type)
    {
        //if (effectList.Count > _type)
        //{
        //    GameObject eft = PoolManager.Instance.GetObj(effectList[_type].name, POOL_TYPE.EFFECT);
        //    eft.transform.SetParent(m_Controller.effectRoot);
        //    eft.transform.localPosition = transform.localPosition;
        //    eft.transform.eulerAngles = transform.eulerAngles;
        //    EffectManager.instance.PlayEffect(eft);
        //}
    }
    public void AttackEffect(int _type)
    {
        GameObject eft = PoolManager.Instance.GetObj(effectList[_type].name, POOL_TYPE.EFFECT);
        eft.transform.SetParent(m_Controller.effectRoot);
        eft.transform.localPosition = transform.localPosition;
        eft.transform.eulerAngles = transform.eulerAngles;

        if (m_Controller.TryGetComponent<IAttackable>(out var attackable))
        {
            EffectManager.instance.PlayEffect(eft, attackable.AttackSpd);
        }

    }
    public void PlaySound(int _index)
    {
        SoundManager.Instance.PlayEffectSound((SOUND_EFFECT)_index);
    }
    public void PlayerAttackSound()
    {
        if (currentState == OBJ_ANIMATION_STATE.ATTACK)
            SoundManager.Instance.PlayEffectSound((SOUND_EFFECT)Random.Range(0, 4));
    }
    public void SkillTierAura()
    {

    }
    public void SkillEffect(int _type)
    {
        if (m_Controller.gameObject.TryGetComponent<IUseSkill>(out var iUseSkill))
        {
            iUseSkill.SkillAniEvent(_type);
        }
    }
    public void SkillSound()
    {

    }
    public void Skill()
    {
        if (m_Controller.TryGetComponent<IUseSkill>(out var iUseSkill))
        {
            Tables.Skill skillTb = Tables.Skill.Get(iUseSkill.SkillInfoList[iUseSkill.UseSkillNum].skillKey);
            if (skillTb != null)
            {
                List<IHittable> gos = new List<IHittable>();
                switch ((SKILL_TYPE)skillTb.SkillType)
                {
                    case SKILL_TYPE.CIRCLE:
                        gos = iUseSkill.GetInCircleObjects(transform, skillTb.SkillRadius);
                        break;
                    case SKILL_TYPE.BAR:
                        gos = iUseSkill.GetInBarObjects(transform, skillTb.SkillWidth, skillTb.SkillRange);
                        break;
                    case SKILL_TYPE.ANGLE:
                        break;
                }
                foreach (var go in gos)
                {
                    go.GetDamage(iUseSkill.CalculateSkillDamage(iUseSkill.SkillInfoList[iUseSkill.UseSkillNum]));
                }
            }
        }
    }



}
