using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    private static SoundManager instance = null;
    private AudioSource audioSource;

    public AudioClip upgradeCardSound;

    public float zombieSoundVolum;


    private float masterVolumeLevel = 0.5f;  // 기본값 설정
    private float bgmVolumeLevel = 0.5f;     // 기본값 설정
    private float SfxvolumeLevel = 0.5f;     // 기본값 설정


    private bool isSoundPlaying = false;

    [SerializeField] private AudioMixer m_AudioMixer;
  


    public void SetMasterVolume(float masterVolume)
    {
        masterVolumeLevel= masterVolume;
        m_AudioMixer.SetFloat("MyExposedParam", Mathf.Log10(masterVolume) * 20);
    }

    public void SetMusicVolume(float BgmVol)
    {
        bgmVolumeLevel= BgmVol;
        m_AudioMixer.SetFloat("BGM", Mathf.Log10(BgmVol) * 20);
    }

    public void SetSFXVolume(float SfxVol)
    {
        SfxvolumeLevel= SfxVol;
        m_AudioMixer.SetFloat("SFX", Mathf.Log10(SfxVol) * 20);
    }
    public float GetMasterVol()
    {
        return masterVolumeLevel;
    }
    public float GetBgm()
    {
        return bgmVolumeLevel;
    }
    public float GetSfx()
    {
        return SfxvolumeLevel;
    }

    
    void Awake()
    {
        if (null == instance)
        {
            instance = this;

            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }

        audioSource = GetComponent<AudioSource>();

        zombieSoundVolum = 0.5f;
       
    }

    public static SoundManager Instance
    {
        get
        {
            if (null == instance)
            {
                return null;
            }
            return instance;
        }
    }

    public void PlayOneShot(AudioClip clip)
    {
        if (clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogError("오디오 클립이 할당되지 않았습니다");
        }
    }

    // 사운드 실행하는데 실행 중이면 안함 중복 실행안함
    public void PlayOneShotNonRepeat(AudioClip clip)
    {
        if (isSoundPlaying)
        {
            return;
        }

        if (clip != null)
        {
            isSoundPlaying = true;
            audioSource.PlayOneShot(clip);
            StartCoroutine(PlayingCheck(clip));
        }
        else
        {
            Debug.LogError("오디오 클립이 할당되지 않았습니다");
        }
    }

    IEnumerator PlayingCheck(AudioClip clip)
    {
        yield return new WaitForSeconds(clip.length);

        isSoundPlaying = false;
    }
}
