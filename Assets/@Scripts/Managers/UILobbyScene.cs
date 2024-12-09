using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UILobbyScene : MonoBehaviour
{
    private GameObject firstMenu;
    public GameObject tutoPanel;
    public GameObject Option;
    public AudioClip buttonSound;
    public AudioClip selectMapSound;
    public AudioClip gameExitSound;
    //����� ����
    [SerializeField] private Slider m_MusicMasterSlider;
    [SerializeField] private Slider m_MusicBGMSlider;
    [SerializeField] private Slider m_MusicSFXSlider;
    // Start is called before the first frame update
    void Start()
    {
        firstMenu = transform.Find("Screen-first").gameObject;

        m_MusicMasterSlider.value = SoundManager.Instance.GetMasterVol(); // ���� ���� ���� �����̴��� ����
        m_MusicBGMSlider.value = SoundManager.Instance.GetBgm();
        m_MusicSFXSlider.value = SoundManager.Instance.GetSfx();
        m_MusicMasterSlider.onValueChanged.AddListener(SetMasterVolume);
        m_MusicBGMSlider.onValueChanged.AddListener(SetBgmVolume);
        m_MusicSFXSlider.onValueChanged.AddListener(SetSFXVolume);
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

    #region �޴� ����

    public void GameStart()
    {
        SoundManager.Instance.PlayOneShot(selectMapSound);
        SceneManager.LoadScene("LoadingScene", LoadSceneMode.Single);
    }


    #endregion

    #region ���丮��  ����

    public void SkipTuto()
    {
        SoundManager.Instance.PlayOneShot(buttonSound);
        tutoPanel.SetActive(false);
        firstMenu.SetActive(true);
    }

    #endregion

    #region �ɼ� ����
    public void OpenOption()
    {
        SoundManager.Instance.PlayOneShot(buttonSound);
        Option.SetActive(true);
    }

    public void CloseOption()
    {
        SoundManager.Instance.PlayOneShot(buttonSound);
        Option.SetActive(false);
    }


    #endregion

    #region ��������
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
    #endregion
}
