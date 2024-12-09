using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class ChangeGun : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // 각 컨트롤러에 대한 총기 오브젝트
    public GameObject leftPistol, leftShotgun, leftRifle, leftMachine, leftBow,leftSword,leftTurret;
    public GameObject rightPistol, rightShotgun, rightRifle, rightMachine,rightBow,rightSword,rightTurret;

    private RectTransform currentHoveredObject; // 현재 Hover된 UI 객체
    private Vector3 originalScale;  // 원래 크기 저장
    private bool isHover = false; 

    public GameObject leftChangeGun; //버튼 터치 후 끄기 위함.

    public XRInteractorLineVisual leftLineVisual;
    public XRInteractorLineVisual rightLineVisual;

    public GameObject leftOculusController;
    public GameObject rightOculusController;

    private string leftWeaponType;
    private string rightWeaponType;

    public AudioClip changeGunSound;

    public InputActionProperty leftHandAction;  //왼쪽 컨트롤러
    public InputActionProperty rightHandAction; //오른쪽 컨트롤러

    string controllerType; //왼, 오 컨트롤러 구분을 위한 변수 

    //public XRRayInteractor leftRay;
    //public XRRayInteractor rightRay;


    private void Awake()
    {
        // leftLineVisual = GameObject.Find("Left Controller").GetComponent<XRInteractorLineVisual>();
        // rightLineVisual = GameObject.Find("Right Controller").GetComponent<XRInteractorLineVisual>();

        // 게임이 시작되었을 경우 기본 무기 타입은 Pistol
        leftWeaponType = "Pistol";
        rightWeaponType = "Pistol";
    }

    private void OnEnable()
    {
        leftHandAction.action.performed += OnLeftHandButtonPressed;
        rightHandAction.action.performed += OnRightHandButtonPressed;
    }

    private void OnDisable()
    {
        leftHandAction.action.performed -= OnLeftHandButtonPressed;
        rightHandAction.action.performed -= OnRightHandButtonPressed;
    }

    private void OnLeftHandButtonPressed(InputAction.CallbackContext context)
    {
        //왼손으로 눌렀을때.
        controllerType = "Left";
    }

    private void OnRightHandButtonPressed(InputAction.CallbackContext context)
    {
        //오른손으로 버튼눌렀을때
        controllerType = "Right";
    }

    // 특정 컨트롤러의 총기를 변경
    public void SwitchWeaponForController(string weaponName)
    {
        ResetlWeapons();
        ToggleController();

        //만약 if() 왼쪽 컨트롤러로 터치했을때 
        if (controllerType != null)
        {
            if (controllerType == "Left")
            {
                switch (weaponName)
                {
                    case "Pistol":
                        leftPistol.SetActive(true);
                        leftWeaponType = "Pistol";
                        break;
                    case "Shotgun":
                        leftShotgun.SetActive(true);
                        leftWeaponType = "Shotgun";
                        break;
                    case "Rifle":
                        leftRifle.SetActive(true);
                        leftWeaponType = "Rifle";
                        break;
                    case "Machinegun":
                        leftMachine.SetActive(true);
                        leftWeaponType = "Machinegun";
                        break;
                    case "Bow":
                        leftBow.SetActive(true);
                        leftWeaponType = "Bow";
                        break;
                    case "Sword":
                        leftSword.SetActive(true);
                        leftWeaponType = "Sword";
                        break;
                    case "Turret":
                        leftTurret.SetActive(true);
                        leftWeaponType = "Turret";
                        break;
                }
            }
            if (controllerType == "Right")
            {
                {
                    switch (weaponName)
                    {
                        case "Pistol":
                            rightPistol.SetActive(true);
                            rightWeaponType = "Pistol";
                            break;
                        case "Shotgun":
                            rightShotgun.SetActive(true);
                            rightWeaponType = "Shotgun";
                            break;
                        case "Rifle":
                            rightRifle.SetActive(true);
                            rightWeaponType = "Rifle";
                            break;
                        case "Machinegun":
                            rightMachine.SetActive(true);
                            rightWeaponType = "Machinegun";
                            break;
                        case "Bow":
                            rightBow.SetActive(true);
                            rightWeaponType = "Bow";
                            break;
                        case "Sword":
                            rightSword.SetActive(true);
                            rightWeaponType = "Sword";
                            break;
                        case "Turret":
                            rightTurret.SetActive(true);
                            rightWeaponType = "Turret";
                            break;
                    }
                }
            }
        }

        //else() 오른쪽으로 클릭했을 때 오른쪽으로 바뀌는 로직 추가.
        SoundManager.Instance.PlayOneShot(changeGunSound);
        leftChangeGun.SetActive(false); //버튼 끄기 

    }

    public void ActiveWeapon()
    {
        // 퍼즈 메뉴 끌 때 라인 비쥬얼 비활성화
        leftLineVisual.enabled = false;
        rightLineVisual.enabled = false;
        // 오큘러스 컨트롤러 비활성화
        leftOculusController.SetActive(false);
        rightOculusController.SetActive(false);

        leftLineVisual.gameObject.SetActive(true);

        // 왼쪽 무기 다시 활성화
        switch (leftWeaponType)
        {
            case "Pistol":
                leftPistol.SetActive(true);
                break;
            case "Shotgun":
                leftShotgun.SetActive(true);
                break;
            case "Rifle":
                leftRifle.SetActive(true);
                break;
            case "Machinegun":
                leftMachine.SetActive(true);
                break;
            case "Bow":
                leftBow.SetActive(true);
                break;
            case "Turret":
                leftTurret.SetActive(true);
                break;
            case "Sword":
                leftSword.SetActive(true);
                break;
        }

        // 오른쪽 무기 다시 활성화
        switch (rightWeaponType)
        {
            case "Pistol":
                rightPistol.SetActive(true);
                break;
            case "Shotgun":
                rightShotgun.SetActive(true);
                break;
            case "Rifle":
                rightRifle.SetActive(true);
                break;
            case "Machinegun":
                rightMachine.SetActive(true);
                break;
            case "Bow":
                rightBow.SetActive(true);
                break;
            case "Turret":
                rightTurret.SetActive(true);
                break;
            case "Sword":
                rightSword.SetActive(true);
                break;
        }
    }

    // 모든 총기를 비활성화하는 함수
    public void ResetlWeapons()
    {
        // 아래의 조건문은 pause 메뉴가 활성화될 때 현재 내가 들고 있는 무기가 무엇인지 체크함
        if (leftPistol.activeSelf)
        {
            leftWeaponType = "Pistol";
        }
        if (leftShotgun.activeSelf)
        {
            leftWeaponType = "Shotgun";
        }
        if (leftRifle.activeSelf)
        {
            leftWeaponType = "Rifle";
        }
        if (leftMachine.activeSelf)
        {
            leftWeaponType = "Machinegun";
        }
        if (leftBow.activeSelf)
        {
            leftWeaponType = "Bow";
        }
        if (leftSword.activeSelf)
        {
            leftWeaponType = "Sword";
        }
        if (leftTurret.activeSelf)
        {
            leftWeaponType = "Turret";
        }


        if (rightPistol.activeSelf)
        {
            rightWeaponType = "Pistol";
        }
        if (rightShotgun.activeSelf)
        {
            rightWeaponType = "Shotgun";
        }
        if (rightRifle.activeSelf)
        {
            rightWeaponType = "Rifle";
        }
        if (rightMachine.activeSelf)
        {
            rightWeaponType = "Machinegun";
        }
        if (rightBow.activeSelf)
        {
            rightWeaponType = "Bow";
        }
        if (rightSword.activeSelf)
        {
            rightWeaponType = "Sword";
        }
        if (rightTurret.activeSelf)
        {
            rightWeaponType = "Turret";
        }


        leftPistol.SetActive(false);
        leftShotgun.SetActive(false);
        leftRifle.SetActive(false);
        leftMachine.SetActive(false);
        leftBow.SetActive(false);
        leftSword.SetActive(false);
        leftTurret.SetActive(false);
        rightPistol.SetActive(false);
        rightShotgun.SetActive(false);
        rightRifle.SetActive(false);
        rightMachine.SetActive(false);
        rightBow.SetActive(false);
        rightSword.SetActive(false);
        rightTurret.SetActive(false);
    }

    public void ToggleController()
    {
        // 메뉴 킬 때 라인 비쥬얼 활성화
        leftLineVisual.enabled = true;
        rightLineVisual.enabled = true;
        // 오큘러스 컨트롤러 활성화
        leftOculusController.SetActive(true);
        rightOculusController.SetActive(true);
    }

    public void ToggleRightController()
    {
        leftLineVisual.gameObject.SetActive(false);
        // 메뉴 킬 때 라인 비쥬얼 활성화
        rightLineVisual.enabled = true;
        // 오큘러스 컨트롤러 활성화
        rightOculusController.SetActive(true);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isHover)
        {
            // UI가 Hover될 때 크기 증가
            currentHoveredObject = eventData.pointerEnter.GetComponent<RectTransform>();
            if (currentHoveredObject != null)
            {
                originalScale = currentHoveredObject.localScale;  // 원래 크기 저장
                currentHoveredObject.localScale *= 1.1f;  // 크기 증가
                isHover = true;
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isHover && currentHoveredObject != null)
        {
            // UI Hover 종료 시 크기 복원
            currentHoveredObject.localScale = originalScale;  // 원래 크기 복원
            isHover = false;
        }
    }
}