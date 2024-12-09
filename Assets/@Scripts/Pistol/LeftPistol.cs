using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.HID;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(Animator))]
public class LeftPistol : MonoBehaviour
{
    public InputActionReference leftShootPistol; // �� ��ũ��Ʈ �ν��Ͻ��� �Ҵ�� ���� �Է� ����
    public XRBaseController haptic; // ��� �� ��Ʈ�ѷ� ����
    public AudioClip pistolSound; // ���� ��� ����
    public GameObject hitBloodEffect; // Enemy���� �Ѿ��� ������ �ǰ� ������ ȿ��
    public GameObject hitConcreteEffect; // ���� źȯ�� �ε����� ����� ȿ��
    private Transform firePos; // �Ѿ��� ������ ��ġ
    private Transform cartridgeEffectPos; // ���� ź�� ����Ʈ�� ������ ��ġ
    private ParticleSystem pistolMuzzleFlash; // ���� ���� �÷���
    private ParticleSystem pistolCartridgeEffect; // ���� ź�� ����Ʈ
    private Animator pistolAnim;
    private CameraShake cameraShake;
    public LineRenderer renderer;

    private float pistolDistance = 500f; // ���� �����Ÿ�
    private int pistolDamage = 1;

    private void Awake()
    {
        Init();
    }

    private void OnEnable()
    {
        leftShootPistol.action.Enable();
        leftShootPistol.action.performed += OnFirePerformed;
    }

    private void OnDisable()
    {
        leftShootPistol.action.performed -= OnFirePerformed;
        leftShootPistol.action.Disable();
    }

    private void OnFirePerformed(InputAction.CallbackContext context)
    {
        Fire();
    }

    private void Fire()
    {
        StartCoroutine(cameraShake.ShakeCamera(0.005f, 0.03f, 0.01f));
        haptic.SendHapticImpulse(1f, 0.3f);
        SoundManager.Instance.PlayOneShot(pistolSound);
        pistolMuzzleFlash.Play();
        pistolCartridgeEffect.Play();
        pistolAnim?.SetTrigger("Pistol");

        RaycastHit hit;

        // ���̰� �߻��
        if (Physics.Raycast(firePos.position, firePos.forward, out hit, pistolDistance, GameManager.Instance.layerMask))
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

    private IEnumerator ShotRenderer()
    {
        renderer.startWidth = 0.003f; // ���� ���� �ʺ�
        renderer.endWidth = 0.003f;   // ���� �� �ʺ�

        // LineRenderer�� positionCount ����
        renderer.positionCount = 2;

        // ���� �������� FirePos
        renderer.SetPosition(0, firePos.position);
        // ���� �� ���� �Է����� ���� �浹 ��ġ
        renderer.SetPosition(1, firePos.position + transform.forward * pistolDistance);
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
        firePos = transform.Find("FirePos");
        cartridgeEffectPos = transform.Find("CartridgeEffect");
        pistolMuzzleFlash = firePos.GetComponentInChildren<ParticleSystem>();
        pistolCartridgeEffect = cartridgeEffectPos.GetComponentInChildren<ParticleSystem>();
        pistolAnim = GetComponent<Animator>();
    }

    void Attack(RaycastHit hit)
    {
        int pistolLevel = GameManager.Instance.augmentLevels[0];

        Zombie zombie = GameManager.Instance.getZombieInParent(hit.collider.gameObject);

        if (pistolLevel < 3) // ���� 1, 2
        {
            zombie.HitByGun(pistolDamage + GameManager.Instance.pistolUpgradeDamage + (pistolLevel * 10));
        }
        else if (pistolLevel < 5) // ���� 3, 4
        {
            zombie.HitByGun(pistolDamage + GameManager.Instance.pistolUpgradeDamage + (pistolLevel * 25));
        }
        else // ���� 5
        {
            zombie.HitByGun(pistolDamage + GameManager.Instance.pistolUpgradeDamage + (pistolLevel * 50));
        }
    }
    void HeadShot(RaycastHit hit)
    {
        int pistolLevel = GameManager.Instance.augmentLevels[0];

        Zombie zombie = GameManager.Instance.getZombieInParent(hit.collider.gameObject);

        if (pistolLevel < 3) // ���� 1, 2
        {
            zombie.HitByGunAtHead(pistolDamage + GameManager.Instance.pistolUpgradeDamage + (pistolLevel * 10));
        }
        else if (pistolLevel < 5) // ���� 3, 4
        {
            zombie.HitByGunAtHead(pistolDamage + GameManager.Instance.pistolUpgradeDamage + (pistolLevel * 25));
        }
        else // ���� 5
        {
            zombie.HitByGunAtHead(pistolDamage + GameManager.Instance.pistolUpgradeDamage + (pistolLevel * 50));
        }
    }
}
