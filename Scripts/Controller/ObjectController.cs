using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ObjectController : MonoBehaviour
{
    public EObjType objType;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public abstract void ObjectGetComponent();
    public virtual void Init() { }
 }
