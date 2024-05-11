using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ObjectController : MonoBehaviour
{
    [Header("ObjController")]
    public OBJ_TYPE objType;
    public AnimationController aniCtrl;

    void Start()
    {
        
    }

    public abstract void ObjectGetComponent();
    public virtual void Init() { }
    public virtual void FindEnemy() { }
    public  void ChangeState(OBJ_ANIMATION_STATE _state) 
    {
        aniCtrl.ChangeAnimation(_state);
    }
}
