using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccountManager : Singleton<AccountManager>
{
    // Start is called before the first frame update
    int curStageKey = 101001;

    public int CurStageKey { get => curStageKey;    set => curStageKey = value; }

    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
