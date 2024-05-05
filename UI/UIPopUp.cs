using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UIPopUp : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        ChildSetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ChildSetActive(bool _isActive)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(_isActive);
        }
    }
    public abstract void ClosePopUp();
    public abstract void OpenPopUp();

}
