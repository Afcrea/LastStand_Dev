using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class UIGameScene : MonoBehaviour
{
    private GameObject Ranking;
    private GameObject pauseMenu; // 일시정지 화면.
    private GameObject LeftChange; // 총기 교체 화면
    private GameObject gameOver; // 게임 오버 UI
    private GameObject upgradeCard; // 업그레이드 UI
    private ChangeGun resetWeapon;
    public Button[] weaponButtons; // 총기 교체 버튼 배열
    public GameObject Option;

    public InputActionReference leftChangeButton;
    public InputActionReference menuButtonAction; //인풋액션을 감지하기 위한 변수

    public AudioClip buttonSound;    //기본 버튼음
    public AudioClip gameExitSound;  //종료 버튼음
    public Sprite[] levelImage;   //업그레이드 현황 이미지 배열
    public GameObject []targetObj; //바뀔 대상 배열

    public LineRenderer leftRenderer;
    public LineRenderer rightRenderer;

    //오디오 관련
    [SerializeField] private Slider m_MusicMasterSlider;
    [SerializeField] private Slider m_MusicBGMSlider;
    [SerializeField] private Slider m_MusicSFXSlider;

    int[] prevAugmentLevels = { 0, 0, 0, 0, 0, 0, 0 };
    int[] upgradeState = { 0, 0, 0, 0, 0, 0, 0 };
    // Start is called before the first frame update
    void Start()
    {
        pauseMenu = transform.Find("PauseMenu").gameObject;
        Ranking = pauseMenu.transform.Find("Ranking").gameObject;
        LeftChange = transform.Find("GunChange").gameObject;
        gameOver = transform.Find("GameOver").gameObject;
        upgradeCard = transform.Find("UpgradeCard").gameObject;
        resetWeapon = LeftChange.GetComponent<ChangeGun>();
        UpdateButtonStates();

        for(int i = 0; i < GameManager.Instance.augmentLevels.Count; i++)
        {
            prevAugmentLevels[i] = GameManager.Instance.augmentLevels[i];
        }

        prevAugmentLevels[0] = 0;

        m_MusicMasterSlider.value = SoundManager.Instance.GetMasterVol(); // 기존 볼륨 값을 슬라이더에 적용
        m_MusicBGMSlider.value = SoundManager.Instance.GetBgm();
        m_MusicSFXSlider.value = SoundManager.Instance.GetSfx();
        m_MusicMasterSlider.onValueChanged.AddListener(SetMasterVolume);
        m_MusicBGMSlider.onValueChanged.AddListener(SetBgmVolume);
        m_MusicSFXSlider.onValueChanged.AddListener(SetSFXVolume);
    }

    // Update is called once per frame
    void Update()
    {
        //UI 메뉴중 어느 하나라도 활성화시 현재 총기 off, 게임 일시 정지, 다시 UI가 꺼지면 현재 총기 재 활성화
        if (pauseMenu.activeSelf || LeftChange.activeSelf || gameOver.activeSelf || upgradeCard.activeSelf)
        {
            resetWeapon.ResetlWeapons();

            if (upgradeCard.activeSelf)
            {
                resetWeapon.ToggleRightController();
            }
            else
            {
                resetWeapon.ToggleController();
            }

            rightRenderer.enabled = false;
            leftRenderer.enabled = false;

            Time.timeScale = 0;
        }
        else
        {
            resetWeapon.ActiveWeapon();
            Time.timeScale = 1;
        }
    }

    IEnumerator UICheck()
    {
        while(GameManager.Instance)
        {
            //UI 메뉴중 어느 하나라도 활성화시 현재 총기 off, 게임 일시 정지, 다시 UI가 꺼지면 현재 총기 재 활성화
            if (pauseMenu.activeSelf || LeftChange.activeSelf || gameOver.activeSelf || upgradeCard.activeSelf)
            {
                resetWeapon.ResetlWeapons();

                if (upgradeCard.activeSelf)
                {
                    resetWeapon.ToggleRightController();
                }
                else
                {
                    resetWeapon.ToggleController();
                }

                rightRenderer.enabled = false;
                leftRenderer.enabled = false;

                Time.timeScale = 0;
            }
            else
            {
                resetWeapon.ActiveWeapon();
                Time.timeScale = 1;
            }

            yield return new WaitForSecondsRealtime(0.1f);
        }
    }

    private void OnEnable()
    {
        // InputAction 활성화 및 이벤트 연결
        menuButtonAction.action.Enable();
        menuButtonAction.action.performed += ToggleObject;
        leftChangeButton.action.Enable();
        leftChangeButton.action.performed += OnButtonPress;
        //leftChangeButton.action.canceled += OnButtonRelease;
    }

    private void OnDisable()
    {
        // 이벤트 해제 및 InputAction 비활성화
        menuButtonAction.action.performed -= ToggleObject;
        menuButtonAction.action.Disable();
        leftChangeButton.action.Disable();
        leftChangeButton.action.performed -= OnButtonPress;
        //leftChangeButton.action.canceled -= OnButtonRelease;
    }
    public void CloseOption()
    {
        SoundManager.Instance.PlayOneShot(buttonSound);
        Option.SetActive(false);
    }
    public void OpenOption()
    {
        SoundManager.Instance.PlayOneShot(buttonSound);
        Option.SetActive(true);
    }

    private void SetMasterVolume(float value)
    {
        SoundManager.Instance.SetMasterVolume(value);
    }
    private void SetBgmVolume(float value)
    {
        SoundManager.Instance.SetMusicVolume(value);
    }
    private void SetSFXVolume(float value)
    {
        SoundManager.Instance.SetSFXVolume(value);
    }
    #region 인풋 시스템 관련

    private void ToggleObject(InputAction.CallbackContext context)
    {
        if (GameManager.Instance.isDead)
        {
            return; 
        }

        // 삼항 연산자를 통해 오브젝트의 활성 상태를 토글
        pauseMenu.SetActive(!pauseMenu.activeSelf);
    }

    public void OnButtonPress(InputAction.CallbackContext context)
    {
        if (GameManager.Instance.isDead)
        {
            return;
        }

        if (context.performed)
        {
            UnlockWeapon(0); //기본적으로 권총만 세팅해두고 나머지는 해금되면 켜기.
            LeftChange.SetActive(!LeftChange.activeSelf);
        }
    }

    public void OnButtonRelease(InputAction.CallbackContext context)
    {
        //if (context.canceled)
        //{
        //    LeftChange.SetActive(false);
        //}
    }

    #endregion

    #region 퍼즈 관련
    public void GameExit()
    {
        SoundManager.Instance.PlayOneShot(gameExitSound);
        StartCoroutine(Exit());
    }
    IEnumerator Exit()
    {
        yield return new WaitForSecondsRealtime(1f);
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void ClosePauseMenu()
    {
        SoundManager.Instance.PlayOneShot(buttonSound);
        pauseMenu.SetActive(false);
    }


    #endregion

    #region 게임 오버 관련
    public void RestartScene()
    {
        SoundManager.Instance.PlayOneShot(buttonSound);
        // 현재 씬의 이름을 얻고 다시 로드
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    public void MoveHome()
    {
        SoundManager.Instance.PlayOneShot(buttonSound);
        SceneManager.LoadScene("LobbyScene", LoadSceneMode.Single);
    }


    #endregion

    #region 랭킹 관련

    public void OpenRank()
    {
        SoundManager.Instance.PlayOneShot(buttonSound);
        Ranking.SetActive(true);
    }

    public void CloseRank()
    {
        SoundManager.Instance.PlayOneShot(buttonSound);
        Ranking.SetActive(false);
    }


    #endregion

    #region 총기 교체 버튼 On Off
    public void UpdateButtonStates()
    {
        for (int i = 0; i < weaponButtons.Length; i++)
        {
            bool isUnlocked = GameManager.Instance.isGunUnlock[i];
            weaponButtons[i].gameObject.SetActive(isUnlocked);
            weaponButtons[i].interactable = isUnlocked;
        }
    }
  public void UnlockWeapon(int index)
    {
        if (index < GameManager.Instance.isGunUnlock.Length)
        {
            GameManager.Instance.isGunUnlock[index] = true;
            // UI 갱신
            UpdateButtonStates();
        }
    }

    public void ChangeObj() 
    {
        // 현재 해금된 총 수 체크 = 레벨 1 이상 무기

        // 반복문 해금된 총 수 만큼의 타겟obj를 순회하며 이미지 주고 텍스트 주고

        //int count = 0;

        //for(int level = 0; level < GameManager.Instance.augmentLevels.Count; level++)
        //{
        //    if (GameManager.Instance.augmentLevels[level] >= 1)
        //    {
        //        count++;
        //    }
        //}

        int changeGunLevel = 0;
        // 달라진 레벨 찾기

        for(int i = 0; i < GameManager.Instance.augmentLevels.Count; i++)
        {
            if (prevAugmentLevels[i] != GameManager.Instance.augmentLevels[i])
            {
                changeGunLevel = i;
                break;
            }
        }

        //for (int i = 0; i < count; i++)
        for (int i = 0; i < GameManager.Instance.augmentLevels.Count; i++)
        {
            //if(targetObj[i].activeSelf)
            //{
            //    continue;
            //}

            //targetObj[i].SetActive(true);

            //targetObj[i].GetComponentInChildren<Image>().sprite = levelImage[changeGunLevel];


            targetObj[i].GetComponentInChildren<TextMeshProUGUI>().text = "Level : "
            + GameManager.Instance.augmentLevels[i].ToString();
        }

        for (int i = 0; i < GameManager.Instance.augmentLevels.Count; i++)
        {
            prevAugmentLevels[i] = GameManager.Instance.augmentLevels[i];
        }

    }

    public void ChangeObjj()
    {
        // 현재 해금된 총 수 체크 = 레벨 1 이상 무기

        // 반복문 해금된 총 수 만큼의 타겟obj를 순회하며 이미지 주고 텍스트 주고

        int count = 0;

        for (int level = 0; level < GameManager.Instance.augmentLevels.Count; level++)
        {
            if (GameManager.Instance.augmentLevels[level] >= 1)
            {
                count++;
            }
        }

        int changeGunLevel = 0;
        // 달라진 레벨 찾기

        for (int i = 0; i < GameManager.Instance.augmentLevels.Count; i++)
        {
            if (prevAugmentLevels[i] != GameManager.Instance.augmentLevels[i])
            {
                changeGunLevel = i;
                break;
            }
        }

        for (int i = 0; i < count; i++)
        {
            if (targetObj[i].activeSelf)
            {
                targetObj[i].GetComponentInChildren<TextMeshProUGUI>().text = "Level : "
            + GameManager.Instance.augmentLevels[upgradeState[i]].ToString();
                continue;
            }

            targetObj[i].SetActive(true);
            upgradeState[i] = changeGunLevel;
            targetObj[i].GetComponentInChildren<Image>().sprite = levelImage[changeGunLevel];
        }

        for (int i = 0; i < GameManager.Instance.augmentLevels.Count; i++)
        {
            int levels = GameManager.Instance.augmentLevels[i];
            prevAugmentLevels[i] = levels;
        }
    }

    #endregion
}
