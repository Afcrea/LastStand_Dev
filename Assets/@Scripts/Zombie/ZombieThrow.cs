using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SocialPlatforms.Impl;

public class ZombieThrow : MonoBehaviour
{
    public Animator animator;
    Zombie zombie;

    public AudioClip zombieSound;
    AudioSource audioSource;

    public string animationStateName;
    public int animationLayer = 0;

    public GameObject throwAttack;
    public GameObject throwAttackPos;

    public float attackDelay = 10f;

    public AudioMixerGroup mixerGroup; // ø¨∞·«“ AudioMixerGroup

    private void Start()
    {
        attackDelay = 5f;

        animationStateName = "Throw";

        animator = GetComponent<Animator>();
        zombie = GetComponent<Zombie>();

        zombie.hp = zombie.hp + 5000;
        zombie.EXP = zombie.EXP + 40000;
        zombie.score = zombie.score + 1000;

        zombie.maxHp = zombie.hp;

        StartCoroutine(Attack());

        audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.loop = false;

        audioSource.outputAudioMixerGroup = mixerGroup;

        audioSource.volume = SoundManager.Instance.zombieSoundVolum;

        audioSource.clip = zombieSound;

        audioSource.Play();
    }

    IEnumerator Attack()
    {
        while(!zombie.isDead)
        {
            animator.SetBool("Attack", true);

            yield return new WaitForSeconds(0.73f);

            // Instantiate the throw attack at the specified position and rotation
            Instantiate(throwAttack, throwAttackPos.transform.position, throwAttackPos.transform.rotation);

            animator.SetBool("Attack", false);

            yield return new WaitForSeconds(attackDelay);
        }
    }

    private void OnDisable()
    {
        GameManager.Instance.zombieBossCount--;
        audioSource.clip = null;
    }

   
}
