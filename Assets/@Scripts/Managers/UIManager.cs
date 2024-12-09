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
    public GameObject pauseMenu; // 일시정지 화면.
    public GameObject LeftChange; //왼쪽 총기 교체 화면
    public ChangeGun resetWeapon; // pauseMenu가 켜져있을 때 총기 모두 비활성화

    public InputActionReference leftChangeButton;
    public InputActionReference menuButtonAction; //인풋액션을 감지하기 위한 변수

    private string currentWeapon; // 현재 들고있는 무기를 기억함

    private void OnEnable()
    {
        // InputAction 활성화 및 이벤트 연결
        menuButtonAction.action.Enable();
        menuButtonAction.action.performed += ToggleObject;
        leftChangeButton.action.Enable();
        leftChangeButton.action.performed += LeftChanger;
    }

    private void OnDisable()
    {
        // 이벤트 해제 및 InputAction 비활성화
        menuButtonAction.action.performed -= ToggleObject;
        menuButtonAction.action.Disable();
        leftChangeButton.action.performed -= LeftChanger;
        leftChangeButton.action.Disable();
    }
    // InputAction을 통해 호출되는 함수
    private void ToggleObject(InputAction.CallbackContext context)
    {
        // 삼항 연산자를 통해 오브젝트의 활성 상태를 토글
        pauseMenu.SetActive(!pauseMenu.activeSelf);

        OnPauseMenu();
    }

    // 퍼즈 메뉴를 호출 시 해야 될 일들
    private void OnPauseMenu()
    {
        //퍼즈 메뉴 활성화시 게임 일시 정지, 모든 무기 비활성화
        if (pauseMenu.activeSelf /* ||  LeftChange.activeSelf */) // 무기 바꾸는 UI가 생겨도 게임 멈추기?
        {
            resetWeapon.ResetlWeapons();
            resetWeapon.ToggleController();
            Time.timeScale = 0;
        }
        else  //비활성화 시 다시 게임 시작, 직전 무기 다시 들기
        {
            resetWeapon.ActiveWeapon();
            Time.timeScale = 1;
        }
    }

    private void LeftChanger(InputAction.CallbackContext context)
    {
        // 삼항 연산자를 통해 오브젝트의 활성 상태를 토글
        LeftChange.SetActive(!LeftChange.activeSelf);
        // RightChange 넣어서 오른쪽도 비활성화 해야함 -------------------------------
    }
}