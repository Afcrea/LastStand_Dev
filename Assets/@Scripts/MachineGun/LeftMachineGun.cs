using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;

public class LeftMachineGun : MonoBehaviour
{
    public RightMachineGun r_mashinegun;
    public TextMeshProUGUI text;
    public InputActionReference leftShootRifle; // �� ��ũ��Ʈ �ν��Ͻ��� �Ҵ�� ���� �Է� ����
    public AudioClip machineGunSound; // �ӽŰ� ��� ����
    public XRBaseController haptic; // ��� �� ��Ʈ�ѷ� ����
    public GameObject hitBloodEffect; // Enemy���� �Ѿ��� ������ �ǰ� ������ ȿ��
    public GameObject hitConcreteEffect; // ���� źȯ�� �ε����� ����� ȿ��
    private Transform firePos; // ���̰� ������ ��ġ
    private Transform cartridgeEffectPos; // �ӽŰ� ź�� ����Ʈ�� ������ ��ġ
    private ParticleSystem machineGunMuzzleFlash; // �ӽŰ� ���� �÷���
    private ParticleSystem machineGunCartridgeEffect; // �ӽŰ� ź�� ����Ʈ
    private Coroutine firingCoroutine;
    private WaitForSeconds rateOfFire = new WaitForSeconds(0.1f); // �ӽŰ� ����ӵ�
    private CameraShake cameraShake;
    public LineRenderer renderer;

    private float machineGunDistance = 500f; // �ӽŰ� �����Ÿ�
    private int machineGunDamage = 1;
    public int bulletsPerMag = 150;
    public int currentBullets;

    private void Awake()
    {
        Init();
    }

    private void OnEnable()
    {
        leftShootRifle.action.Enable();
        leftShootRifle.action.started += OnTriggerPressed;
        leftShootRifle.action.canceled += OnTriggerReleased;
    }

    private void OnDisable()
    {
        leftShootRifle.action.started -= OnTriggerPressed;
        leftShootRifle.action.canceled -= OnTriggerReleased;
        leftShootRifle.action.Disable();
    }

    private void OnTriggerPressed(InputAction.CallbackContext context)
    {
        if (firingCoroutine == null)
        {
            firingCoroutine = StartCoroutine(FireRoutine());
        }
    }

    private void OnTriggerReleased(InputAction.CallbackContext context)
    {
        if (firingCoroutine != null)
        {
            StopCoroutine(firingCoroutine);
            firingCoroutine = null;
        }
    }

    private IEnumerator FireRoutine()
    {
        while (true)
        {
            Fire();
            yield return rateOfFire; // �߻� ����
        }
    }

    private IEnumerator ShotRenderer()
    {
        renderer.startWidth = 0.003f; // ���� ���� �ʺ�
        renderer.endWidth = 0.003f;   // ���� �� �ʺ�

        // LineRenderer�� positionCount ����
        renderer.positionCount = 2;

        // ���� �������� FirePos
        renderer.SetPosition(0, firePos.position);
        // ���� �� ���� �Է����� ���� �浹 ��ġ
        renderer.SetPosition(1, firePos.position + transform.forward * machineGunDistance);
        // ���� �������� Ȱ��ȭ �Ͽ� ź�� ������ �׸�
        renderer.enabled = true;

        // ��� ���
        yield return new WaitForSecondsRealtime(0.03f);

        // ���� ������ ��Ȱ��ȭ
        renderer.enabled = false;
    }

    public void Reroad()
    {
        if (GameManager.Instance.augmentLevels[3] >= 3) // ���� 3���� ��������
        {
            return; 
        }

        if (GameManager.Instance.machinegunTotal <= 0)
        {
            GameManager.Instance.machinegunTotal = 0;
            return;
        }

        // �ʿ��� źȯ �� ���
        int bulletsNeeded = bulletsPerMag - currentBullets;

        // ������ �� �ִ� �ִ� źȯ �� ����
        int bulletsToReload = Mathf.Min(bulletsNeeded, GameManager.Instance.machinegunTotal);
        currentBullets += bulletsToReload;
        GameManager.Instance.machinegunTotal -= bulletsToReload;

        // UI �ؽ�Ʈ ������Ʈ
        text.text = currentBullets.ToString() + "/" + GameManager.Instance.machinegunTotal.ToString();
        r_mashinegun.text.text = r_mashinegun.currentBullets.ToString() + "/" + GameManager.Instance.machinegunTotal.ToString();
    }

    private void Fire()
    {
        if (currentBullets > 0)
        {
            StartCoroutine(cameraShake.ShakeCamera(0.01f, 0, 0.005f));
            haptic.SendHapticImpulse(1f, 0.3f);

            int machineGunLevel = GameManager.Instance.augmentLevels[3];

            if (machineGunLevel < 3) // ���� 1, 2
            {
                currentBullets--; // 1�߾� ����
                text.text = currentBullets.ToString() + "/";
                text.text += GameManager.Instance.machinegunTotal.ToString();
            }
            else if (machineGunLevel < 5) // ���� 3, 4
            {
                GameManager.Instance.machinegunTotal--;
                currentBullets = GameManager.Instance.machinegunTotal;
                
                if (currentBullets <= 0)
                {
                    currentBullets = 0;
                }

                text.text = currentBullets.ToString();
            }
            else // ���� 5
            {
                currentBullets = 10;
                // ���� ǥ��
                text.text = "9999";
            }

            SoundManager.Instance.PlayOneShot(machineGunSound);
            machineGunMuzzleFlash.Play();
            machineGunCartridgeEffect.Play();

            RaycastHit hit;

            // ���̰� �߻��
            if (Physics.Raycast(firePos.position, firePos.forward, out hit, machineGunDistance, GameManager.Instance.layerMask))
            {
                if (hit.collider.CompareTag("Enemy"))
                {
                    GameObject bloodHitInstance = Instantiate(hitBloodEffect, hit.point, Quaternion.LookRotation(hit.normal));
                    Destroy(bloodHitInstance, 1f);
                    // ���� �༮�� HP�� 1 ��´�
                    Attack(hit);
                }
                else if (hit.collider.CompareTag("Head"))
                {
                    GameObject bloodHitInstance = Instantiate(hitBloodEffect, hit.point, Quaternion.LookRotation(hit.normal));
                    Destroy(bloodHitInstance, 1f);
                    // ���� �༮�� HP�� 1 ��´�
                    HeadShot(hit);
                }
                else if (hit.collider.CompareTag("Ground"))
                {
                    GameObject concreteHitInstance = Instantiate(hitConcreteEffect, hit.point, Quaternion.LookRotation(hit.normal));
                }
            }

            StartCoroutine(ShotRenderer());
        }
    }

    private void Init()
    {
        cameraShake = GameObject.Find("Camera Offset").GetComponent<CameraShake>();
        currentBullets = bulletsPerMag;
        text.text = currentBullets.ToString() + "/";
        text.text += GameManager.Instance.machinegunTotal.ToString();

        firePos = transform.Find("FirePos");
        cartridgeEffectPos = transform.Find("CartridgeEffect");
        machineGunMuzzleFlash = firePos.GetComponentInChildren<ParticleSystem>();
        machineGunCartridgeEffect = cartridgeEffectPos.GetComponentInChildren<ParticleSystem>();
    }

    void Attack(RaycastHit hit)
    {
        int machineGunLevel = GameManager.Instance.augmentLevels[3];

        Zombie zombie = GameManager.Instance.getZombieInParent(hit.collider.gameObject);

        if (machineGunLevel < 3) // ���� 1, 2
        {
            zombie.HitByGun(machineGunDamage + GameManager.Instance.machinegunUpgradeDamage + machineGunLevel);
        }
        else if (machineGunLevel < 5) // ���� 3, 4
        {
            zombie.HitByGun(machineGunDamage + GameManager.Instance.machinegunUpgradeDamage + machineGunLevel);
        }
        else // ���� 5
        {
            zombie.HitByGun(machineGunDamage + GameManager.Instance.machinegunUpgradeDamage + machineGunLevel);
            currentBullets = 999;
            // ���⿡ ���� �̹��� ����
        }
    }

    void HeadShot(RaycastHit hit)
    {
        int machineGunLevel = GameManager.Instance.augmentLevels[3];

        Zombie zombie = GameManager.Instance.getZombieInParent(hit.collider.gameObject);

        if (machineGunLevel < 3) // ���� 1, 2
        {
            zombie.HitByGunAtHead(machineGunDamage + GameManager.Instance.machinegunUpgradeDamage + machineGunLevel);
        }
        else if (machineGunLevel < 5) // ���� 3, 4
        {
            zombie.HitByGunAtHead(machineGunDamage + GameManager.Instance.machinegunUpgradeDamage + machineGunLevel);
        }
        else // ���� 5
        {
            zombie.HitByGunAtHead(machineGunDamage + GameManager.Instance.machinegunUpgradeDamage + machineGunLevel);
            currentBullets = 999;
        }
    }

}
