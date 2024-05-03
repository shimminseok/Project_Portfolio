using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Temp : MonoBehaviour
{
    public Vector3 worldTransform;
    int i;
    int j;
    private void Awake()
    {

    }
    void Start()
    {
        worldTransform = transform.position;
    }

    public void Set(int i , int j)
    {
        this.i = i;
        this.j = j;
    }
}
