using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class LeftRifle : MonoBehaviour
{
    public RightRifle r_Rifle;
    public TextMeshProUGUI text;
    public InputActionReference leftShootRifle; // �� ��ũ��Ʈ �ν��Ͻ��� �Ҵ�� ���� �Է� ����
    public AudioClip rifleSound; // ������ ��� ����
    public XRBaseController haptic; // ��� �� ��Ʈ�ѷ� ����
    public GameObject hitBloodEffect; // Enemy���� �Ѿ��� ������ �ǰ� ������ ȿ��
    public GameObject hitConcreteEffect; // ���� źȯ�� �ε����� ����� ȿ��
    private Transform firePos; // ���̰� ������ ��ġ
    private Transform cartridgeEffectPos; // ������ ź�� ����Ʈ�� ������ ��ġ
    private ParticleSystem rifleMuzzleFlash; // ������ ���� �÷���
    private ParticleSystem rifleCartridgeEffect; // ������ ź�� ����Ʈ
    private Coroutine firingCoroutine;
    private WaitForSeconds rateOfFire = new WaitForSeconds(0.125f);
    private CameraShake cameraShake;
    public LineRenderer renderer;

    private float rifleDistance = 500f; // ������ �����Ÿ�
    private int rifleDamage = 1;
    public int bulletsPerMag = 30;
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

    public void Reroad()
    {
        if (GameManager.Instance.rifleTotal <= 0)
        {
            GameManager.Instance.rifleTotal = 0;
            return;
        }

        // �ʿ��� źȯ �� ���
        int bulletsNeeded = bulletsPerMag - currentBullets;

        // ������ �� �ִ� �ִ� źȯ �� ����
        int bulletsToReload = Mathf.Min(bulletsNeeded, GameManager.Instance.rifleTotal);
        currentBullets += bulletsToReload;
        GameManager.Instance.rifleTotal -= bulletsToReload;

        // UI �ؽ�Ʈ ������Ʈ
        text.text = currentBullets.ToString() + "/" + GameManager.Instance.rifleTotal.ToString();
        r_Rifle.text.text = r_Rifle.currentBullets.ToString() + "/" + GameManager.Instance.rifleTotal.ToString();
    }

    private void Fire()
    {
        if (currentBullets > 0)
        {
            StartCoroutine(cameraShake.ShakeCamera(0.01f, 0, 0.001f));
            currentBullets--; // 1�߾� ����
            haptic.SendHapticImpulse(1f, 0.3f);
            text.text = currentBullets.ToString() + "/";
            text.text += GameManager.Instance.rifleTotal.ToString();

            SoundManager.Instance.PlayOneShot(rifleSound);
            rifleMuzzleFlash.Play();
            rifleCartridgeEffect.Play();

            RaycastHit hit;

            // ���̰� �߻��
            if (Physics.Raycast(firePos.position, firePos.forward, out hit, rifleDistance, GameManager.Instance.layerMask))
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

    private IEnumerator ShotRenderer()
    {
        renderer.startWidth = 0.003f; // ���� ���� �ʺ�
        renderer.endWidth = 0.003f;   // ���� �� �ʺ�

        // LineRenderer�� positionCount ����
        renderer.positionCount = 2;

        // ���� �������� FirePos
        renderer.SetPosition(0, firePos.position);
        // ���� �� ���� �Է����� ���� �浹 ��ġ
        renderer.SetPosition(1, firePos.position + transform.forward * rifleDistance);
        // ���� �������� Ȱ��ȭ �Ͽ� ź�� ������ �׸�
        renderer.enabled = true;

        // ��� ���
        yield return new WaitForSecondsRealtime(0.03f);

        // ���� ������ ��Ȱ��ȭ
        renderer.enabled = false;
    }

    private void Init()
    {
        cameraShake = GameObject.Find("Camera Offset").GetComponent<CameraShake>();
        currentBullets = bulletsPerMag;
        text.text = currentBullets.ToString() + "/";
        text.text += GameManager.Instance.rifleTotal.ToString();

        firePos = transform.Find("FirePos");
        cartridgeEffectPos = transform.Find("CartridgeEffect");
        rifleMuzzleFlash = firePos.GetComponentInChildren<ParticleSystem>();
        rifleCartridgeEffect = cartridgeEffectPos.GetComponentInChildren<ParticleSystem>();
    }

    void Attack(RaycastHit hit)
    {
        int rifleLevel = GameManager.Instance.augmentLevels[1];

        Zombie zombie = GameManager.Instance.getZombieInParent(hit.collider.gameObject);

        if (rifleLevel < 3) // ���� 1, 2
        {
            zombie.HitByGun(rifleDamage + GameManager.Instance.rifleUpgradeDamage + rifleLevel);
        }
        else if (rifleLevel < 5) // ���� 3, 4
        {
            zombie.HitByGun(rifleDamage + GameManager.Instance.rifleUpgradeDamage + rifleLevel + 10);
        }
        else // ���� 5
        {
            zombie.HitByGun(rifleDamage + GameManager.Instance.rifleUpgradeDamage + rifleLevel);
            GameManager.Instance.playerHP = GameManager.Instance.playerHP + 2;

            if(GameManager.Instance.playerHP >= 100)
            {
                GameManager.Instance.playerHP = 100;
            }
        }
    }

    void HeadShot(RaycastHit hit)
    {
        int rifleLevel = GameManager.Instance.augmentLevels[1];

        Zombie zombie = GameManager.Instance.getZombieInParent(hit.collider.gameObject);

        if (rifleLevel < 3) // ���� 1, 2
        {
            zombie.HitByGunAtHead(rifleDamage + GameManager.Instance.rifleUpgradeDamage + rifleLevel);
        }
        else if (rifleLevel < 5) // ���� 3, 4
        {
            zombie.HitByGunAtHead(rifleDamage + GameManager.Instance.rifleUpgradeDamage + rifleLevel + 10);
        }
        else // ���� 5
        {
            zombie.HitByGunAtHead(rifleDamage + GameManager.Instance.rifleUpgradeDamage + rifleLevel);
            GameManager.Instance.playerHP = GameManager.Instance.playerHP + 2;

            if (GameManager.Instance.playerHP >= 100)
            {
                GameManager.Instance.playerHP = 100;
            }
        }
    }
}
