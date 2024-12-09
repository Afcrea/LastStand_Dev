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

    [Header("플레이어")]
    public int playerHP;
    public int playerEXP;
    public int playerLevel;
    public bool isDead;
    public AudioClip[] playerHitSounds;

    [Header("게임 진행")]
    public float breakTime; // 쉬는 시간
    public int gameWave; // 웨이브 단계
    public int gameLevel;
    public int score;
    public float baseValue; // 초기 값을 조절하기 위한 값
    public float count; // 몬스터 소환 수
    public bool isAllZombieDead; // 현재 웨이브에 좀비가 남았는 지 체크
    public bool startWave; // 다음 웨이브 시작 트리거
    public bool isGameClear; // 게임 클리어 여부 체크
    public float breakStartTime; // 쉬는 시간 시작한 시간 저장
    public float passTime; // 쉬는 시간 지나간 시간 저장
    public int zombieCount; // 좀비 수
    public int zombieBossCount; // 좀비 보스 수
    bool updateScore = false; // 점수 업데이트 여부
    public AudioClip[] BGM; // bgm
    public GameObject BGMObject;
    AudioSource BGMSource;

    [Header("업그레이드")]
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

    [Header("좀비 스탯")]
    int zombieStat;
    int zombieBossStat;
    public float zombieSpawnTime;

    [Header("UI")]
    public Image bloodScreen;
    public GameObject gameOverUI;
    public TextMeshProUGUI scoreText;
    public GameObject playerStateUI;
    public GameObject cameraVR;

    [Header("업그레이드 UI")]
    public GameObject upgradeUI;
    public GameObject[] cardSlots; // 카드가 표시될 슬롯들

    [Header("업그레이드 카드 프리펩")]
    public GameObject healCardPrefab;
    public GameObject[] upgradeCardPrefabs;

    public Dictionary<int, int> augmentLevels = new Dictionary<int, int>();
    // 0 = 권총
    // 1 = 라이플
    // 2 = 샷건
    // 3 = 머신건
    // 4 = 터렛
    // 5 = 검
    // 6 = 석궁
    private float[] augmentProbabilities;

    UIGameScene gameScene;

    public int layerMask; // 레그돌 제외

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

    #region 플레이어 관련

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

        // 공격 카드 생성
        int firstIndex = ChooseAugmentCardIndex();

        int secondIndex = 0;

        do
        {
            secondIndex = ChooseAugmentCardIndex();
        } while (firstIndex == secondIndex);

        float startTimeScale = Time.timeScale; // 현재 타임스케일
        float elapsedTime = 0f;

        while (elapsedTime < 1.0f)
        {
            elapsedTime += Time.unscaledDeltaTime; // Time.unscaledDeltaTime은 타임스케일에 영향을 받지 않음
            Time.timeScale = Mathf.Lerp(startTimeScale, 0f, elapsedTime / 1.0f); // Lerp로 점진적 감소
            yield return null; // 다음 프레임까지 대기
        }

        upgradeUI.SetActive(true);



        GameObject augmentfirstCard = Instantiate(upgradeCardPrefabs[firstIndex], cardSlots[1].transform);
        GameObject augmentsecondCard = Instantiate(upgradeCardPrefabs[secondIndex], cardSlots[2].transform);

        // 선택까지 대기
        while (isUpgrading)
        {
            Time.timeScale = 0;
            yield return null;
        }

        // 선택 후 카드 슬롯 비워주기
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

    // 가중치
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

    #region 게임 진행 관련

    IEnumerator ZombieGame()
    {
        #region 쉬는 시간

        startWave = false;

        breakStartTime = Time.time;
        passTime = 0f;

        while (!startWave) // 스타트 웨이브가 트루 되면 탈출 스타트 웨이브가 트루되는 조건은 30초가 지나거나 특정 오브젝트를 맞춰서 시작
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
            // 좀비 소환 및 웨이브 시작
            isAllZombieDead = false;

            Resources.UnloadUnusedAssets();
            System.GC.Collect();


            int calCount = CalZombieSpawnCount();

            ZombieWave(calCount);

            int deadLock = 0;

            while(startWave) // 소환할 동안 대기
            {
                yield return new WaitForSecondsRealtime(5f);
            }

            while (!isAllZombieDead) // 남은 좀비 계산
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
            gameWave++; // 웨이브 클리어

            #region 쉬는 시간

            startWave = false;

            breakStartTime = Time.time;
            passTime = 0f;

            while (!startWave) // 스타트 웨이브가 트루 되면 탈출 스타트 웨이브가 트루되는 조건은 30초가 지나거나 특정 오브젝트를 맞춰서 시작
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
            enemySpawn.StartWave(calCount); // 좀비 소환
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

    // 브금 변경 1 : 기본, 2 : throw 보스, 3 : 마지막 보스
    void ChangeBGM(int i)
    {
        BGMSource.clip = BGM[i];
        BGMSource.Play();
    }

    int CalZombieSpawnCount() // 소환할 좀비 수 계산
    { 
        return Mathf.RoundToInt(count * Mathf.Pow(baseValue, gameWave));
    } 


    #endregion

    #region 좀비 생성 시 난이도에 따라 스탯 설정

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

        // 모든 자식 오브젝트부터 삭제
        foreach (Transform child in obj.transform)
        {
            DestroyWithCleanup(child.gameObject);
        }

        // 오브젝트 자체의 리소스 정리
        CleanupComponents(obj);

        // 최종적으로 GameObject 삭제
        Destroy(obj);
    }

    private static void CleanupComponents(GameObject obj)
    {
        // Renderer와 Material 정리
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

        // AudioSource 정리
        AudioSource audioSource = obj.GetComponent<AudioSource>();
        if (audioSource != null)
        {
            audioSource.Stop();
            audioSource.clip = null; // 참조 해제
        }

        // Texture 정리 (예: RawImage에서 사용되는 Texture)
        var rawImage = obj.GetComponent<UnityEngine.UI.RawImage>();
        if (rawImage != null && rawImage.texture != null)
        {
            Destroy(rawImage.texture);
        }

        // ParticleSystem 관련 리소스 정리
        var particleSystem = obj.GetComponent<ParticleSystem>();
        if (particleSystem != null)
        {
            Destroy(particleSystem);
        }

        // 기타 동적으로 할당된 컴포넌트 정리
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

        // 동적 오브젝트의 추가적인 참조 해제 (필요시 추가)
    }

    #endregion
}
