using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIPopUp : MonoBehaviour
{
    // Start is called before the first frame update
    protected virtual void Start()
    {
        ChildSetActive(false);
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
        ChildSetActive(false);
        if (UIManager.Instance.popupType != FULL_POPUP_TYPE.NONE)
        {   
            UIManager.Instance.popupType = FULL_POPUP_TYPE.NONE;
            UIManager.Instance.fullPopUp.ChildSetActive(false);
        }
    }
    public virtual void OpenPopUp()
    {
        ChildSetActive(true);
        SoundManager.Instance.PlayEffectSound(SOUND_EFFECT.NO_11);
    }

}
