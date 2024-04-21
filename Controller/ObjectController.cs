using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ObjectController : MonoBehaviour
{
    [Header("ObjController")]
    public EObjType objType;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public abstract void ObjectGetComponent();
    public virtual void Init() { }
 }
