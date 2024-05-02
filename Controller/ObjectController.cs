using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ObjectController : MonoBehaviour
{
    [Header("ObjController")]
    public OBJ_TYPE objType;
    
    protected AnimationController aniCtrl;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public abstract void ObjectGetComponent();
    public virtual void Init() { }
 }
