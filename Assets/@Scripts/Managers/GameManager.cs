using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public EnemySpawn enemySpawn;
    GameObject enemy;

    public int playerMaxHP = 100;
    public int playerMaxEXP = 100;
    public int gameMaxLevel = 100;

    [Header("�÷��̾�")]
    public int playerHP;
    public int playerEXP;
    public int playerLevel;
    public bool isDead;
    public AudioClip[] playerHitSounds;

    [Header("���� ����")]
    public float breakTime; // ���� �ð�
    public int gameWave; // ���̺� �ܰ�
    public int gameLevel;
    public int score;
    public float baseValue; // �ʱ� ���� �����ϱ� ���� ��
    public float count; // ���� ��ȯ ��
    public bool isAllZombieDead; // ���� ���̺꿡 ���� ���Ҵ� �� üũ
    public bool startWave; // ���� ���̺� ���� Ʈ����
    public bool isGameClear; // ���� Ŭ���� ���� üũ
    public float breakStartTime; // ���� �ð� ������ �ð� ����
    public float passTime; // ���� �ð� ������ �ð� ����
    public int zombieCount; // ���� ��
    public int zombieBossCount; // ���� ���� ��
    bool updateScore = false; // ���� ������Ʈ ����
    public AudioClip[] BGM; // bgm
    public GameObject BGMObject;
    AudioSource BGMSource;

    [Header("���׷��̵�")]
    public int pistolUpgradeDamage = 0;
    public int shotgunUpgradeDamage = 0;
    public int rifleUpgradeDamage = 0;
    public int machinegunUpgradeDamage = 0;
    public int turretUpgradeDamage = 0;
    public int swordUpgradeDamage = 0;
    public int bowUpgradeDamage = 0;
    public int shotgunTotal = 0;
    public int rifleTotal = 0;
    public int machinegunTotal = 0;
    public bool isUpgrading = false;
    public bool[] isGunUnlock;

    [Header("���� ����")]
    int zombieStat;
    int zombieBossStat;
    public float zombieSpawnTime;

    [Header("UI")]
    public Image bloodScreen;
    public GameObject gameOverUI;
    public TextMeshProUGUI scoreText;
    public GameObject playerStateUI;
    public GameObject cameraVR;

    [Header("���׷��̵� UI")]
    public GameObject upgradeUI;
    public GameObject[] cardSlots; // ī�尡 ǥ�õ� ���Ե�

    [Header("���׷��̵� ī�� ������")]
    public GameObject healCardPrefab;
    public GameObject[] upgradeCardPrefabs;

    public Dictionary<int, int> augmentLevels = new Dictionary<int, int>();
    // 0 = ����
    // 1 = ������
    // 2 = ����
    // 3 = �ӽŰ�
    // 4 = �ͷ�
    // 5 = ��
    // 6 = ����
    private float[] augmentProbabilities;

    UIGameScene gameScene;

    public int layerMask; // ���׵� ����

    private void Awake()
    {
        InitInstance();
        InitPlayer();
        InitZombieStat();
        InitWeapon();
        InitAugments();
    }

    private void Start()
    {
        Application.targetFrameRate = 60;

        InitComponent();

        StartCoroutine(ZombieGame());

        layerMask = ~LayerMask.GetMask("Ragdoll");

        BGMSource = BGMObject.GetComponent<AudioSource>();
        BGMSource.loop = true;
    }

    void InitInstance()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        Instance = this;
    }

    void InitPlayer()
    {
        playerHP = playerMaxHP;
        playerEXP = 0;
        playerLevel = 0;
        score = 0;
        playerMaxHP = 100;
        playerMaxEXP = 10;
        gameMaxLevel = 50;
        breakTime = 2f;
        gameWave = 1;
        isDead = false;
        baseValue = 1.05f;
        count = 10;
        zombieCount = 0;
        zombieBossCount = 0;
        gameLevel = 1;
    }

    void InitWeapon()
    {
        shotgunTotal = 60;
        rifleTotal = 300;
        machinegunTotal = 450;
    }

    void InitComponent()
    {
        enemySpawn = GameObject.FindAnyObjectByType<EnemySpawn>();
        enemy = GameObject.Find("Enemy").gameObject;
        gameScene = FindAnyObjectByType<UIGameScene>().GetComponent<UIGameScene>();
    }

    void InitZombieStat()
    {
        zombieStat = 1;
        zombieBossStat = 1000;
        zombieSpawnTime = 0.5f;
    }

    void InitAugments()
    {
        augmentLevels.Clear();
        augmentProbabilities = new float[upgradeCardPrefabs.Length];
        for (int i = 0; i < augmentProbabilities.Length; i++)
        {
            augmentLevels[i] = 0;
            augmentProbabilities[i] = 1.0f;
        }
        augmentLevels[0] = 1;
    }

    private void Update()
    {
        PlayerDieCheck();
    }

    #region �÷��̾� ����

    public void HitPlayer(int damage, bool isZombieDead)
    {
        if(isZombieDead)
        {
            return;
        }

        playerHP -= damage;

        SoundManager.Instance.PlayOneShotNonRepeat(playerHitSounds[Random.Range(0, playerHitSounds.Length)]);

        StartCoroutine(ShowBloodScreen());
    }

    void PlayerDieCheck()
    {
        if (playerHP <= 0)
        {
            gameOverUI.SetActive(true);
            scoreText.text = "Score : " + score.ToString();
            if(!updateScore)
            {
                GetComponent<SaveRanking>().UploadScore(score);
                updateScore = true;
            }
            enemySpawn.gameObject.SetActive(false);
            enemy.SetActive(false);
            playerStateUI.SetActive(false);
            isDead = true;
        }
    }

    IEnumerator ShowBloodScreen()
    {
        bloodScreen.color = new Color(1, 0, 0, 0.5f);
        yield return new WaitForSecondsRealtime(0.1f);
        bloodScreen.color = Color.clear;
    }

    public void GetZombieEXP(int EXP, int _score)
    {
        score += _score;
        playerEXP += EXP;
        PlayerLevelUp();
        GameLevelUp();
    }

    void PlayerLevelUp()
    {
        if (playerEXP >= playerMaxEXP)
        {
            playerEXP = 0;
            playerMaxEXP *= 2;

            //if(playerMaxEXP >= 500)
            //{
            //    playerMaxEXP = 10000;
            //}

            if (playerMaxEXP >= 200000)
            {
                playerMaxEXP = 200000;
            }
            playerLevel++;
            StartCoroutine(UpgradePlayer());
        }
    }

    void GameLevelUp()
    {
        if (score % gameMaxLevel == gameMaxLevel / 2)
        {
            enemySpawn.SpawnBoss();
        }
        if (score % gameMaxLevel == 0)
        {
            gameLevel++;
        }
    }

    IEnumerator UpgradePlayer()
    {
        if(isUpgrading)
        {
            yield break;
        }

        SoundManager.Instance.PlayOneShot(SoundManager.Instance.upgradeCardSound);

        isUpgrading = true;

        gameScene.ChangeObj();

        GameObject healCard = Instantiate(healCardPrefab, cardSlots[0].transform);

        // ���� ī�� ����
        int firstIndex = ChooseAugmentCardIndex();

        int secondIndex = 0;

        do
        {
            secondIndex = ChooseAugmentCardIndex();
        } while (firstIndex == secondIndex);

        float startTimeScale = Time.timeScale; // ���� Ÿ�ӽ�����
        float elapsedTime = 0f;

        while (elapsedTime < 1.0f)
        {
            elapsedTime += Time.unscaledDeltaTime; // Time.unscaledDeltaTime�� Ÿ�ӽ����Ͽ� ������ ���� ����
            Time.timeScale = Mathf.Lerp(startTimeScale, 0f, elapsedTime / 1.0f); // Lerp�� ������ ����
            yield return null; // ���� �����ӱ��� ���
        }

        upgradeUI.SetActive(true);



        GameObject augmentfirstCard = Instantiate(upgradeCardPrefabs[firstIndex], cardSlots[1].transform);
        GameObject augmentsecondCard = Instantiate(upgradeCardPrefabs[secondIndex], cardSlots[2].transform);

        // ���ñ��� ���
        while (isUpgrading)
        {
            Time.timeScale = 0;
            yield return null;
        }

        // ���� �� ī�� ���� ����ֱ�
        //foreach (GameObject slot in cardSlots)
        //{
        //    foreach(Transform child in slot.transform.GetComponentsInChildren<Transform>())
        //    {
        //        if(child.gameObject == slot)
        //        { 
        //            continue; 
        //        }
        //        Destroy(child.gameObject);
        //    }
        //}

        GameManager.DestroyWithCleanup(augmentfirstCard);
        GameManager.DestroyWithCleanup(healCard);
        GameManager.DestroyWithCleanup(augmentsecondCard);

        upgradeUI.SetActive(false);

        Time.timeScale = 1;
    }

    // ����ġ
    int ChooseAugmentCardIndex()
    {
        float totalWeight = 0;
        for (int i = 0; i < augmentProbabilities.Length; i++)
        {
            if (augmentLevels[i] < 5)
            {
                totalWeight += augmentProbabilities[i];
            }
        }

        float randomValue = Random.Range(0, totalWeight);
        float cumulativeWeight = 0;
        for (int i = 0; i < augmentProbabilities.Length; i++)
        {
            if (augmentLevels[i] >= 5) continue;

            cumulativeWeight += augmentProbabilities[i];
            if (randomValue <= cumulativeWeight)
            {
                augmentProbabilities[i] += 0.1f;
                return i;
            }
        }
        return 0;
    }

    #endregion

    #region ���� ���� ����

    IEnumerator ZombieGame()
    {
        #region ���� �ð�

        startWave = false;

        breakStartTime = Time.time;
        passTime = 0f;

        while (!startWave) // ��ŸƮ ���̺갡 Ʈ�� �Ǹ� Ż�� ��ŸƮ ���̺갡 Ʈ��Ǵ� ������ 30�ʰ� �����ų� Ư�� ������Ʈ�� ���缭 ����
        {
            passTime = Time.time - breakStartTime;
            yield return new WaitForSecondsRealtime(0.1f);

            if (passTime > breakTime)
            {
                startWave = true;
            }
        }

        #endregion

        while (!isGameClear)
        {
            // ���� ��ȯ �� ���̺� ����
            isAllZombieDead = false;

            Resources.UnloadUnusedAssets();
            System.GC.Collect();


            int calCount = CalZombieSpawnCount();

            ZombieWave(calCount);

            int deadLock = 0;

            while(startWave) // ��ȯ�� ���� ���
            {
                yield return new WaitForSecondsRealtime(5f);
            }

            while (!isAllZombieDead) // ���� ���� ���
            {
                if (zombieBossCount <= 0 && zombieCount <= 0)
                {
                    isAllZombieDead = true;
                }

                deadLock++;

                if(deadLock >= 30)
                {
                    isAllZombieDead = true;
                }

                yield return new WaitForSecondsRealtime(0.5f);
            }
            gameWave++; // ���̺� Ŭ����

            #region ���� �ð�

            startWave = false;

            breakStartTime = Time.time;
            passTime = 0f;

            while (!startWave) // ��ŸƮ ���̺갡 Ʈ�� �Ǹ� Ż�� ��ŸƮ ���̺갡 Ʈ��Ǵ� ������ 30�ʰ� �����ų� Ư�� ������Ʈ�� ���缭 ����
            {
                passTime = Time.time - breakStartTime;
                yield return new WaitForSecondsRealtime(0.1f);

                if (passTime > breakTime)
                {
                    startWave = true;
                }
            }

            #endregion
        }
    }

    void ZombieWave(int calCount)
    {
        //if(gameWave == 1)
        //{
        //    enemySpawn.StartZombieSpawn();
        //}

        if(gameWave % 2 == 0)
        {
            StartCoroutine(BossSpawn(gameWave));
        }

        if (gameWave / 3 >= 1)
        {
            if (gameWave % 3 == 0)
            {
                passTime = 1f;

                ChangeBGM(1);
                enemySpawn.ZombieThrowSpawn();
            }

            else if (gameWave % 5 == 0)
            {
                ChangeBGM(2);
                enemySpawn.WizardZombieSpawn();
            }

            if (gameWave % 7 == 0)
            {
                ChangeBGM(0);

                if(gameWave == 7)
                {
                    breakTime = 0.1f;
                    enemySpawn.StartZombieSpawn();
                }
                
                zombieBossStat *= (gameWave / 3) + 2;
            }
        }

        //if(gameWave <= 6)
        //{
            enemySpawn.StartWave(calCount); // ���� ��ȯ
        //}
        //else
        //{
        //    startWave = false;
        //}

    }

    IEnumerator BossSpawn(int count)
    {
        for(int i = 0; i < count; i++)
        {
            enemySpawn.SpawnBoss();
            yield return new WaitForSecondsRealtime(10f);
        }
    }

    // ��� ���� 1 : �⺻, 2 : throw ����, 3 : ������ ����
    void ChangeBGM(int i)
    {
        BGMSource.clip = BGM[i];
        BGMSource.Play();
    }

    int CalZombieSpawnCount() // ��ȯ�� ���� �� ���
    { 
        return Mathf.RoundToInt(count * Mathf.Pow(baseValue, gameWave));
    } 


    #endregion

    #region ���� ���� �� ���̵��� ���� ���� ����

    public void SetZombieStat(Zombie zombie)
    {
        zombie.hp = zombieStat + gameLevel;
        zombie.damage = gameLevel;
        zombie.moveSpeed = zombieStat + gameLevel + 10;

        if(zombie.moveSpeed >= 15)
        {
            zombie.moveSpeed = 15;
        }

        zombie.EXP = zombieStat + gameLevel * gameLevel;
        zombie.score = 1;
    }

    public void SetZombieBossStat(Zombie zombie)
    {
        zombie.hp = zombieBossStat / 2 + gameLevel * 5;
        zombie.damage = 5 + gameLevel;
        zombie.moveSpeed = 8;
        zombie.EXP = zombieBossStat * gameLevel;
        zombie.score = 10;
    }

    #endregion

    #region Utility

    public Zombie getZombieInParent(GameObject curr)
    {
        Zombie zombie = curr.GetComponent<Zombie>();

        while (zombie == null)
        {
            GameObject parent = curr.transform.parent.gameObject;

            zombie = parent.GetComponent<Zombie>();

            curr = parent;
        }

        return zombie;
    }

    public static void DestroyWithCleanup(GameObject obj)
    {
        if (obj == null) return;

        // ��� �ڽ� ������Ʈ���� ����
        foreach (Transform child in obj.transform)
        {
            DestroyWithCleanup(child.gameObject);
        }

        // ������Ʈ ��ü�� ���ҽ� ����
        CleanupComponents(obj);

        // ���������� GameObject ����
        Destroy(obj);
    }

    private static void CleanupComponents(GameObject obj)
    {
        // Renderer�� Material ����
        Renderer renderer = obj.GetComponent<Renderer>();
        if (renderer != null)
        {
            if (renderer.material != null)
            {
                Destroy(renderer.material);
            }
            if (renderer.sharedMaterial != null)
            {
                Destroy(renderer.sharedMaterial);
            }
        }

        // AudioSource ����
        AudioSource audioSource = obj.GetComponent<AudioSource>();
        if (audioSource != null)
        {
            audioSource.Stop();
            audioSource.clip = null; // ���� ����
        }

        // Texture ���� (��: RawImage���� ���Ǵ� Texture)
        var rawImage = obj.GetComponent<UnityEngine.UI.RawImage>();
        if (rawImage != null && rawImage.texture != null)
        {
            Destroy(rawImage.texture);
        }

        // ParticleSystem ���� ���ҽ� ����
        var particleSystem = obj.GetComponent<ParticleSystem>();
        if (particleSystem != null)
        {
            Destroy(particleSystem);
        }

        // ��Ÿ �������� �Ҵ�� ������Ʈ ����
        var meshFilter = obj.GetComponent<MeshFilter>();
        if (meshFilter != null && meshFilter.mesh != null)
        {
            Destroy(meshFilter.mesh);
        }

        var collider = obj.GetComponent<Collider>();
        if (collider != null)
        {
            Destroy(collider);
        }

        // ���� ������Ʈ�� �߰����� ���� ���� (�ʿ�� �߰�)
    }

    #endregion
}
