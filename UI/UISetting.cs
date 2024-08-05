using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UISetting : UIPopUp
{

    [SerializeField] List<Toggle> settingToggleList = new List<Toggle>();
    [SerializeField] List<Slider> soundSlider = new List<Slider>();
    [SerializeField] List<TextMeshProUGUI> soundVolumeTxtList = new List<TextMeshProUGUI>();


    Dictionary<int, Action<float>> volumeChangeActions = new Dictionary<int, Action<float>>();

    bool isChangedSoundOption = false;

    protected override void Start()
    {
        base.Start();

        volumeChangeActions = new Dictionary<int, Action<float>>
        {
            { 0, SoundManager.Instance.ChangeBGMSoundVolume },
            { 1, SoundManager.Instance.ChangeEffectSoundVolume }
        };

        for (int i = 0; i < soundSlider.Count; i++)
        {
            float volume = PlayerPrefs.GetFloat($"SoundVolume{i}",1f);
            soundSlider[i].value = volume;
            ChangeSoundVolume(i);
        }
    }

    public override void OpenPopUp()
    {
        base.OpenPopUp();

        isChangedSoundOption = false;
    }
    public override void ClosePopUp()
    {
        SaveSettingData();

        base.ClosePopUp();
    }

    void SaveSettingData()
    {
        if (isChangedSoundOption)
        {
            for (int i = 0; i < soundSlider.Count; i++)
            {
                PlayerPrefs.SetFloat($"SoundVolume{i}", soundSlider[i].value);
            }
        }
    }
    #region[ButtonAction]

    public void OnClickSettingTab(GameObject _go)
    {
        int selectIndex = settingToggleList.FindIndex(x => x.gameObject.name == _go.name);
        settingToggleList[selectIndex].isOn = true;
    }

    public void OnClickSoundMax(int _audioIndex)
    {
        soundSlider[_audioIndex].value = 1;
    }
    public void OnClickSoundMin(int _audioIndex)
    {
        soundSlider[_audioIndex].value = 0;
    }

    public void ChangeSoundVolume(int _audioIndex)
    {
        if(volumeChangeActions.TryGetValue(_audioIndex, out var action))
        {
            float soundVolume = soundSlider[_audioIndex].value;
            action(soundVolume);
            soundVolumeTxtList[_audioIndex].text = ((int)(soundVolume * 100)).ToString();
        }
        else
        {
            Debug.LogWarning($"Unknown audio index: {_audioIndex}");
        }

        isChangedSoundOption = true;
    }
    #endregion

}
