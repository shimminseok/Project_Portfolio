using UnityEngine;

public class AnimationContrller : MonoBehaviour
{
    [SerializeField] ObjectController m_Controller;
    IAttackable m_Attackable;
    IMoveable m_Moveable;
    IHittable m_Hittable;
    IControlable m_Controlable;
    int AttackCount;
    Animator animator;
    void Start()
    {
        m_Controller = GetComponentInParent<ObjectController>();
        if(m_Controller != null )
        {
            //if(m_Controller.objType == EObjType.COLLEAGUE)
            //{
            //    m_Controller = m_Controller as PlayerController;
            //}
        }
        m_Attackable = m_Controller.GetComponent<IAttackable>();
        m_Moveable = m_Controller.GetComponent<IMoveable>();
        m_Hittable = m_Controller.GetComponent<IHittable>();
        m_Controlable = m_Controller.GetComponent<IControlable>();
        animator = GetComponent<Animator>();
    }
    void Update()
    {
        //if (m_Attackable != null)
        //{
        //    if(Input.GetMouseButtonDown(0))
        //        animator.SetBool("IsAttack", true);
        //    else if(Input.GetMouseButtonUp(0))
        //        animator.SetBool("IsAttack", false);
        //}


        if (m_Hittable != null)
        {
            if (m_Hittable.CurHP < m_Hittable.MaxHP)
            {
                if (animator.GetBool("LessHp") && m_Hittable.CurHP / m_Hittable.MaxHP >= 0.3)
                    animator.SetBool("LessHp", false);
            }

            if (m_Hittable.CurHP / m_Hittable.MaxHP < 0.3)
                animator.SetBool("LessHp", true);

            if (m_Hittable.IsDead)
            {
                animator.SetBool("IsDead", true);
            }
        }

        if (m_Controlable != null)
        {
            animator.SetBool("IsMove", m_Controlable.IsMove);
        }
    }

    public void AttackEvent()
    {
        m_Attackable?.SetAttackEvent();
    }
    public void EndAttackAni()
    {
        m_Attackable.AttackCount = 0;
        animator.SetInteger("AttackCount", m_Attackable.AttackCount);
    }

    public void MoveEvent()
    {
        m_Moveable?.SetMoveEvent();
    }
}
