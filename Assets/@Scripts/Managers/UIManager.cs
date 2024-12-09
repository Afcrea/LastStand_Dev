using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public GameObject firstMenu;
    public GameObject SelectMap;
    public GameObject Option;
    public GameObject Ranking;
    public GameObject pauseMenu; // �Ͻ����� ȭ��.
    public GameObject LeftChange; //���� �ѱ� ��ü ȭ��
    public ChangeGun resetWeapon; // pauseMenu�� �������� �� �ѱ� ��� ��Ȱ��ȭ

    public InputActionReference leftChangeButton;
    public InputActionReference menuButtonAction; //��ǲ�׼��� �����ϱ� ���� ����

    private string currentWeapon; // ���� ����ִ� ���⸦ �����

    private void OnEnable()
    {
        // InputAction Ȱ��ȭ �� �̺�Ʈ ����
        menuButtonAction.action.Enable();
        menuButtonAction.action.performed += ToggleObject;
        leftChangeButton.action.Enable();
        leftChangeButton.action.performed += LeftChanger;
    }

    private void OnDisable()
    {
        // �̺�Ʈ ���� �� InputAction ��Ȱ��ȭ
        menuButtonAction.action.performed -= ToggleObject;
        menuButtonAction.action.Disable();
        leftChangeButton.action.performed -= LeftChanger;
        leftChangeButton.action.Disable();
    }
    // InputAction�� ���� ȣ��Ǵ� �Լ�
    private void ToggleObject(InputAction.CallbackContext context)
    {
        // ���� �����ڸ� ���� ������Ʈ�� Ȱ�� ���¸� ���
        pauseMenu.SetActive(!pauseMenu.activeSelf);

        OnPauseMenu();
    }

    // ���� �޴��� ȣ�� �� �ؾ� �� �ϵ�
    private void OnPauseMenu()
    {
        //���� �޴� Ȱ��ȭ�� ���� �Ͻ� ����, ��� ���� ��Ȱ��ȭ
        if (pauseMenu.activeSelf /* ||  LeftChange.activeSelf */) // ���� �ٲٴ� UI�� ���ܵ� ���� ���߱�?
        {
            resetWeapon.ResetlWeapons();
            resetWeapon.ToggleController();
            Time.timeScale = 0;
        }
        else  //��Ȱ��ȭ �� �ٽ� ���� ����, ���� ���� �ٽ� ���
        {
            resetWeapon.ActiveWeapon();
            Time.timeScale = 1;
        }
    }

    private void LeftChanger(InputAction.CallbackContext context)
    {
        // ���� �����ڸ� ���� ������Ʈ�� Ȱ�� ���¸� ���
        LeftChange.SetActive(!LeftChange.activeSelf);
        // RightChange �־ �����ʵ� ��Ȱ��ȭ �ؾ��� -------------------------------
    }
}