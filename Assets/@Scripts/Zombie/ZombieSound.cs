using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

public class ZombieSound : MonoBehaviour
{
    public AudioClip[] zombieClips;

    bool isPlaying = false;

    int currIndex;
    int prevIndex;

    Zombie zombie;

    AudioSource audioSource;

    public AudioMixerGroup mixerGroup; // 연결할 AudioMixerGroup

    private void Start()
    {
        zombie = GetComponent<Zombie>();
        
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.loop = false;
        audioSource.outputAudioMixerGroup = mixerGroup;
        audioSource.volume = SoundManager.Instance.zombieSoundVolum;
        audioSource.spatialBlend = 1.0f; // 3D 사운드 활성화
        audioSource.minDistance = 1f; // 가까울 때 최대 볼륨
        audioSource.maxDistance = 10.0f; // 최대 거리에서 소리가 사라짐
        audioSource.rolloffMode = AudioRolloffMode.Logarithmic; // 거리 감소 방식

        StartCoroutine(PlaySound());
    }

    void Update()
    {
        if(GameManager.Instance.isUpgrading)
        {
            audioSource.Stop();
        }
        if(zombie.isDead)
        {
            audioSource.Stop();
        }
    }

    IEnumerator PlaySound()
    {
        while(!zombie.isDead)
        {
            prevIndex = currIndex;

            currIndex = Random.Range(0, zombieClips.Length);

            do
            {
                currIndex = Random.Range(0, zombieClips.Length);
            } while(currIndex == prevIndex);

            //SoundManager.Instance.PlayOneShotNonRepeat(zombieClips[currIndex]);
            audioSource.clip = zombieClips[currIndex];

            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            } 

            yield return new WaitForSeconds(zombieClips[currIndex].length);
        }

        audioSource.Stop();
        audioSource.clip = null;
    }
}
