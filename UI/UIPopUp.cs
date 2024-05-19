using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIPopUp : MonoBehaviour
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
    public virtual void ClosePopUp()
    {
        if (UIManager.Instance.openedPopupList.TryPop(out UIPopUp popup))
            popup.OpenPopUp();

        ChildSetActive(false );
    }
    public virtual void OpenPopUp()
    {
        ChildSetActive(true);
    }

}
