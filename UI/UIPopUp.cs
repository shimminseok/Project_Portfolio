using UnityEngine;
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
            SoundManager.Instance.MuteEffectSount(false);
            SoundManager.Instance.PlayBGMSound(SOUND_BGM.NO_0);
        }
    }
    public virtual void OpenPopUp()
    {
        ChildSetActive(true);
        SoundManager.Instance.PlayEffectSound(SOUND_EFFECT.NO_11);
        if (UIManager.Instance.popupType != FULL_POPUP_TYPE.NONE)
            SoundManager.Instance.PlayBGMSound(ChangeBGMSound(UIManager.Instance.popupType));
    }

    SOUND_BGM ChangeBGMSound(FULL_POPUP_TYPE _type)
    {
        return _type switch
        {
            FULL_POPUP_TYPE.SUMMON => SOUND_BGM.NO_1,
            _ => throw new System.ArgumentException("Invalid SOUND_BGM")
        };
    }




}
