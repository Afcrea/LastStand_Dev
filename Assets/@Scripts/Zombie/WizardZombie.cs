using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class WizardZombie : Zombie
{
    private Animation animationComponent;
    // Update is called once per frame
    public float AttackDelay = 10f;

    public GameObject[] SpawnPos;
    public GameObject[] SpawnBoss;

    public AudioClip zombieSound;
    AudioSource audioSource;

    public AudioMixerGroup mixerGroup; // 연결할 AudioMixerGroup

    private void Start()
    {
        InitTarget();

        AttackDelay = 7f;

        animationComponent = GetComponent<Animation>();

        this.hp = 10000;
        this.EXP = this.EXP + 50000;
        this.maxHp = this.hp;

        transform.LookAt(player);

        StartCoroutine(Attack());

        audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.loop = false;

        audioSource.outputAudioMixerGroup = mixerGroup;

        audioSource.volume = SoundManager.Instance.zombieSoundVolum;

        audioSource.clip = zombieSound;

        audioSource.Play();

        HPBarImage = HPBar.GetComponent<Image>();
    }
    void Update()
    {
        ZombieAction();

        if (HPBarImage != null)
        {
            HPBarImage.fillAmount = (float)this.hp / (float)this.maxHp;
        }
    }

    void ZombieAction()
    {
        if (isDead) return;

        if (hp <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;

        GameManager.Instance.zombieBossCount--;

        GameManager.Instance.GetZombieEXP(EXP, 100);

        CapsuleCollider capsuleCollider = transform.GetComponent<CapsuleCollider>();
        capsuleCollider.height = 0.1f;
        capsuleCollider.center = new Vector3(0, 0.1f, 0);
        capsuleCollider.radius = 0.1f;

        Head.GetComponent<SphereCollider>().enabled = false;

        if (HPBar != null)
        {
            HPBar.SetActive(false);
            HPBarUI.SetActive(false);
        }

        animationComponent.Play("dead");

        audioSource.clip = null;

        StartCoroutine(DeadWizardZombieDestroy());
    }

    public IEnumerator DeadWizardZombieDestroy() // 죽은 시체 5초 뒤 삭제
    {
        yield return new WaitForSeconds(2f);

        //Destroy(gameObject);

        GameManager.DestroyWithCleanup(this.gameObject);
    }

    new IEnumerator Attack()
    {
        animationComponent.Play("idle_normal");

        yield return null;

        while(!isDead)
        {
            animationComponent.CrossFade("idle_combat", 1f);

            StartCoroutine( SpawnZombie());

            yield return new WaitForSeconds(animationComponent["idle_combat"].length);

            animationComponent.CrossFade("idle_normal", 1f);

            yield return new WaitForSeconds(AttackDelay);
        }
    }

    IEnumerator SpawnZombie()
    {
        GameObject[] Boss = SpawnBoss;
        GameObject enemyPar = GameManager.Instance.enemySpawn.enemyParent;

        foreach (GameObject Spawn in SpawnPos)
        {
            GameObject spawnedObject = Instantiate(Boss[Random.Range(1, Boss.Length)], Spawn.transform.position, Quaternion.identity); // 좀비 생성

            Zombie zombie = spawnedObject.GetComponentInChildren<Zombie>(); // 좀비 스크립트 가져옴

            GameManager.Instance.SetZombieBossStat(zombie);

            if (enemyPar != null)
            {
                spawnedObject.transform.parent = enemyPar.transform;
            }

            yield return new WaitForSeconds(1f);
        }
    }
}
