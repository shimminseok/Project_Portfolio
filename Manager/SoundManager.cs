using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{
    [SerializeField] AudioSource bgm_Audio;
    [SerializeField] AudioSource effect_Audio;
    [SerializeField] List<AudioClip> bgm_Clip;
    [SerializeField] List<AudioClip> effect_Clip;




    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            PlayEffectSound(SOUND_EFFECT.NO_5);
        }
    }

    public void PlayEffectSound(SOUND_EFFECT _type)
    {
        effect_Audio.PlayOneShot(effect_Clip[(int)_type]);
    }
    public void PlayBGMSound(SOUND_BGM _type)
    {
        bgm_Audio.clip = bgm_Clip[(int)_type];
        bgm_Audio.Play();
    }

    public void MuteEffectSount(bool _isMute)
    {
        effect_Audio.mute = _isMute;
    }
}
