using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class ChangeGun : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // �� ��Ʈ�ѷ��� ���� �ѱ� ������Ʈ
    public GameObject leftPistol, leftShotgun, leftRifle, leftMachine, leftBow,leftSword,leftTurret;
    public GameObject rightPistol, rightShotgun, rightRifle, rightMachine,rightBow,rightSword,rightTurret;

    private RectTransform currentHoveredObject; // ���� Hover�� UI ��ü
    private Vector3 originalScale;  // ���� ũ�� ����
    private bool isHover = false; 

    public GameObject leftChangeGun; //��ư ��ġ �� ���� ����.

    public XRInteractorLineVisual leftLineVisual;
    public XRInteractorLineVisual rightLineVisual;

    public GameObject leftOculusController;
    public GameObject rightOculusController;

    private string leftWeaponType;
    private string rightWeaponType;

    public AudioClip changeGunSound;

    public InputActionProperty leftHandAction;  //���� ��Ʈ�ѷ�
    public InputActionProperty rightHandAction; //������ ��Ʈ�ѷ�

    string controllerType; //��, �� ��Ʈ�ѷ� ������ ���� ���� 

    //public XRRayInteractor leftRay;
    //public XRRayInteractor rightRay;


    private void Awake()
    {
        // leftLineVisual = GameObject.Find("Left Controller").GetComponent<XRInteractorLineVisual>();
        // rightLineVisual = GameObject.Find("Right Controller").GetComponent<XRInteractorLineVisual>();

        // ������ ���۵Ǿ��� ��� �⺻ ���� Ÿ���� Pistol
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
        //�޼����� ��������.
        controllerType = "Left";
    }

    private void OnRightHandButtonPressed(InputAction.CallbackContext context)
    {
        //���������� ��ư��������
        controllerType = "Right";
    }

    // Ư�� ��Ʈ�ѷ��� �ѱ⸦ ����
    public void SwitchWeaponForController(string weaponName)
    {
        ResetlWeapons();
        ToggleController();

        //���� if() ���� ��Ʈ�ѷ��� ��ġ������ 
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

        //else() ���������� Ŭ������ �� ���������� �ٲ�� ���� �߰�.
        SoundManager.Instance.PlayOneShot(changeGunSound);
        leftChangeGun.SetActive(false); //��ư ���� 

    }

    public void ActiveWeapon()
    {
        // ���� �޴� �� �� ���� ����� ��Ȱ��ȭ
        leftLineVisual.enabled = false;
        rightLineVisual.enabled = false;
        // ��ŧ���� ��Ʈ�ѷ� ��Ȱ��ȭ
        leftOculusController.SetActive(false);
        rightOculusController.SetActive(false);

        leftLineVisual.gameObject.SetActive(true);

        // ���� ���� �ٽ� Ȱ��ȭ
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

        // ������ ���� �ٽ� Ȱ��ȭ
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

    // ��� �ѱ⸦ ��Ȱ��ȭ�ϴ� �Լ�
    public void ResetlWeapons()
    {
        // �Ʒ��� ���ǹ��� pause �޴��� Ȱ��ȭ�� �� ���� ���� ��� �ִ� ���Ⱑ �������� üũ��
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
        // �޴� ų �� ���� ����� Ȱ��ȭ
        leftLineVisual.enabled = true;
        rightLineVisual.enabled = true;
        // ��ŧ���� ��Ʈ�ѷ� Ȱ��ȭ
        leftOculusController.SetActive(true);
        rightOculusController.SetActive(true);
    }

    public void ToggleRightController()
    {
        leftLineVisual.gameObject.SetActive(false);
        // �޴� ų �� ���� ����� Ȱ��ȭ
        rightLineVisual.enabled = true;
        // ��ŧ���� ��Ʈ�ѷ� Ȱ��ȭ
        rightOculusController.SetActive(true);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isHover)
        {
            // UI�� Hover�� �� ũ�� ����
            currentHoveredObject = eventData.pointerEnter.GetComponent<RectTransform>();
            if (currentHoveredObject != null)
            {
                originalScale = currentHoveredObject.localScale;  // ���� ũ�� ����
                currentHoveredObject.localScale *= 1.1f;  // ũ�� ����
                isHover = true;
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isHover && currentHoveredObject != null)
        {
            // UI Hover ���� �� ũ�� ����
            currentHoveredObject.localScale = originalScale;  // ���� ũ�� ����
            isHover = false;
        }
    }
}