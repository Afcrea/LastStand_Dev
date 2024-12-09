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
    private GameObject pauseMenu; // �Ͻ����� ȭ��.
    private GameObject LeftChange; // �ѱ� ��ü ȭ��
    private GameObject gameOver; // ���� ���� UI
    private GameObject upgradeCard; // ���׷��̵� UI
    private ChangeGun resetWeapon;
    public Button[] weaponButtons; // �ѱ� ��ü ��ư �迭
    public GameObject Option;

    public InputActionReference leftChangeButton;
    public InputActionReference menuButtonAction; //��ǲ�׼��� �����ϱ� ���� ����

    public AudioClip buttonSound;    //�⺻ ��ư��
    public AudioClip gameExitSound;  //���� ��ư��
    public Sprite[] levelImage;   //���׷��̵� ��Ȳ �̹��� �迭
    public GameObject []targetObj; //�ٲ� ��� �迭

    public LineRenderer leftRenderer;
    public LineRenderer rightRenderer;

    //����� ����
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

        m_MusicMasterSlider.value = SoundManager.Instance.GetMasterVol(); // ���� ���� ���� �����̴��� ����
        m_MusicBGMSlider.value = SoundManager.Instance.GetBgm();
        m_MusicSFXSlider.value = SoundManager.Instance.GetSfx();
        m_MusicMasterSlider.onValueChanged.AddListener(SetMasterVolume);
        m_MusicBGMSlider.onValueChanged.AddListener(SetBgmVolume);
        m_MusicSFXSlider.onValueChanged.AddListener(SetSFXVolume);
    }

    // Update is called once per frame
    void Update()
    {
        //UI �޴��� ��� �ϳ��� Ȱ��ȭ�� ���� �ѱ� off, ���� �Ͻ� ����, �ٽ� UI�� ������ ���� �ѱ� �� Ȱ��ȭ
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
            //UI �޴��� ��� �ϳ��� Ȱ��ȭ�� ���� �ѱ� off, ���� �Ͻ� ����, �ٽ� UI�� ������ ���� �ѱ� �� Ȱ��ȭ
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
        // InputAction Ȱ��ȭ �� �̺�Ʈ ����
        menuButtonAction.action.Enable();
        menuButtonAction.action.performed += ToggleObject;
        leftChangeButton.action.Enable();
        leftChangeButton.action.performed += OnButtonPress;
        //leftChangeButton.action.canceled += OnButtonRelease;
    }

    private void OnDisable()
    {
        // �̺�Ʈ ���� �� InputAction ��Ȱ��ȭ
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
    #region ��ǲ �ý��� ����

    private void ToggleObject(InputAction.CallbackContext context)
    {
        if (GameManager.Instance.isDead)
        {
            return; 
        }

        // ���� �����ڸ� ���� ������Ʈ�� Ȱ�� ���¸� ���
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
            UnlockWeapon(0); //�⺻������ ���Ѹ� �����صΰ� �������� �رݵǸ� �ѱ�.
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

    #region ���� ����
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

    #region ���� ���� ����
    public void RestartScene()
    {
        SoundManager.Instance.PlayOneShot(buttonSound);
        // ���� ���� �̸��� ��� �ٽ� �ε�
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    public void MoveHome()
    {
        SoundManager.Instance.PlayOneShot(buttonSound);
        SceneManager.LoadScene("LobbyScene", LoadSceneMode.Single);
    }


    #endregion

    #region ��ŷ ����

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

    #region �ѱ� ��ü ��ư On Off
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
            // UI ����
            UpdateButtonStates();
        }
    }

    public void ChangeObj() 
    {
        // ���� �رݵ� �� �� üũ = ���� 1 �̻� ����

        // �ݺ��� �رݵ� �� �� ��ŭ�� Ÿ��obj�� ��ȸ�ϸ� �̹��� �ְ� �ؽ�Ʈ �ְ�

        //int count = 0;

        //for(int level = 0; level < GameManager.Instance.augmentLevels.Count; level++)
        //{
        //    if (GameManager.Instance.augmentLevels[level] >= 1)
        //    {
        //        count++;
        //    }
        //}

        int changeGunLevel = 0;
        // �޶��� ���� ã��

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
        // ���� �رݵ� �� �� üũ = ���� 1 �̻� ����

        // �ݺ��� �رݵ� �� �� ��ŭ�� Ÿ��obj�� ��ȸ�ϸ� �̹��� �ְ� �ؽ�Ʈ �ְ�

        int count = 0;

        for (int level = 0; level < GameManager.Instance.augmentLevels.Count; level++)
        {
            if (GameManager.Instance.augmentLevels[level] >= 1)
            {
                count++;
            }
        }

        int changeGunLevel = 0;
        // �޶��� ���� ã��

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
