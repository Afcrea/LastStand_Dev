using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;

public class RightShotGun : MonoBehaviour
{
    public LeftShotGun l_shotgun;
    public TextMeshProUGUI text;
    public InputActionReference rightShootShotGun; // �� ��ũ��Ʈ �ν��Ͻ��� �Ҵ�� ���� �Է� ����
    public AudioClip ShotGunSound; // ���� ��� ����
    public XRBaseController haptic; // ��� �� ��Ʈ�ѷ� ����
    public GameObject hitConcreteEffect; // ���� źȯ�� �ε����� ����� ȿ��
    private Transform firePos; // ���̰� ������ ��ġ
    private Transform cartridgeEffectPos; // ���� ź�� ����Ʈ�� ������ ��ġ
    private ParticleSystem hitBloodEffect; // Enemy���� �Ѿ��� ������ �ǰ� ������ ȿ��
    private ParticleSystem ShotGunMuzzleFlash; // ���� ���� �÷���
    private ParticleSystem ShotGunCartridgeEffect; // ���� ź�� ����Ʈ
    private Animator ShotGunAnim;
    private CameraShake cameraShake;

    private int pelletCount = 10; // �߻�Ǵ� źȯ ��
    private float spreadAngle = 0.05f; // ������ ����
    private float ShotGunDistance = 20f; // ���� �����Ÿ�
    private int shotgunDamage = 1;
    public int bulletsPerMag = 10;
    public int currentBullets;

    private void Awake()
    {
        Init();
    }

    private void OnEnable()
    {
        rightShootShotGun.action.Enable();
        rightShootShotGun.action.performed += OnFirePerformed;
    }

    private void OnDisable()
    {
        rightShootShotGun.action.performed -= OnFirePerformed;
        rightShootShotGun.action.Disable();
    }

    // Ű �Է��� ������(Trigger ��ư) Fire �޼��� ����
    private void OnFirePerformed(InputAction.CallbackContext context)
    {
        if (currentBullets > 0)
        {
            Fire();
        }
    }

    public void Reroad()
    {
        if (GameManager.Instance.shotgunTotal == 0)
        {
            return;
        }

        // �ʿ��� źȯ �� ���
        int bulletsNeeded = bulletsPerMag - currentBullets;

        // ������ �� �ִ� �ִ� źȯ �� ����
        int bulletsToReload = Mathf.Min(bulletsNeeded, GameManager.Instance.shotgunTotal);
        currentBullets += bulletsToReload;
        GameManager.Instance.shotgunTotal -= bulletsToReload;

        // UI �ؽ�Ʈ ������Ʈ
        text.text = currentBullets.ToString() + "/" + GameManager.Instance.shotgunTotal.ToString();
        l_shotgun.text.text = l_shotgun.currentBullets.ToString() + "/" + GameManager.Instance.shotgunTotal.ToString();
    }

    private void Fire()
    {
        StartCoroutine(cameraShake.ShakeCamera(0.03f, 0, 0.3f));
        currentBullets--; // 1�߾� ����
        haptic.SendHapticImpulse(1f, 0.6f);
        text.text = currentBullets.ToString() + "/";
        text.text += GameManager.Instance.shotgunTotal.ToString();

        // źȯ�� ����ŭ for�� ����
        for (int i = 0; i < pelletCount; i++)
        {
            Attack();
        }

        ShotGunMuzzleFlash.Play();
        ShotGunCartridgeEffect.Play();
        ShotGunAnim?.SetTrigger("ShotGun");
        SoundManager.Instance.PlayOneShot(ShotGunSound);
    }

    private void FirePellet()
    {
        RaycastHit hit;

        // źȯ�� ������ ������ ����
        Vector3 direction = GetSpreadDirection();

        // firePos���� ����� źȯ�� ���� ���� �� ������ direction ���� ���ư���
        if (Physics.Raycast(firePos.position, direction, out hit, ShotGunDistance, GameManager.Instance.layerMask))
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                // ���� ������ ����Ʈ ��ġ ����
                hitBloodEffect.transform.position = hit.point;
                // ���� ������ ���� ���͸� ����� ����Ʈ ���� ����
                hitBloodEffect.transform.rotation = Quaternion.LookRotation(hit.normal);

                // ��ƼŬ ����Ʈ�� �θ� null�� �����Ͽ� ���� �����Ӱ� ���������� �����
                hitBloodEffect.transform.SetParent(null);
                hitBloodEffect?.Play();
                // ���� �༮�� HP�� ��´�
                Zombie zombie = GameManager.Instance.getZombieInParent(hit.collider.gameObject);
                zombie.HitByGun(shotgunDamage + GameManager.Instance.shotgunUpgradeDamage);
            }
            else if (hit.collider.CompareTag("Head"))
            {
                // ���� ������ ����Ʈ ��ġ ����
                hitBloodEffect.transform.position = hit.point;
                // ���� ������ ���� ���͸� ����� ����Ʈ ���� ����
                hitBloodEffect.transform.rotation = Quaternion.LookRotation(hit.normal);

                // ��ƼŬ ����Ʈ�� �θ� null�� �����Ͽ� ���� �����Ӱ� ���������� �����
                hitBloodEffect.transform.SetParent(null);
                hitBloodEffect?.Play();
                // ���� �༮�� HP�� ��´�
                Zombie zombie = GameManager.Instance.getZombieInParent(hit.collider.gameObject);
                zombie.HitByGunAtHead(shotgunDamage + GameManager.Instance.shotgunUpgradeDamage);
            }
            else if (hit.collider.CompareTag("Ground"))
            {
                GameObject concreteHitInstance = Instantiate(hitConcreteEffect, hit.point, Quaternion.LookRotation(hit.normal));
            }
        }
    }

    // ���� ����
    private void FirePelletUpgrade()
    {
        // źȯ�� ������ ������ ����
        Vector3 direction = GetSpreadDirection();

        Ray ray = new Ray(transform.position, direction);

        RaycastHit[] hits = Physics.RaycastAll(ray, ShotGunDistance, GameManager.Instance.layerMask);

        // firePos���� ����� źȯ�� ���� ���� �� ������ direction ���� ���ư���
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                // ���� ������ ����Ʈ ��ġ ����
                hitBloodEffect.transform.position = hit.point;
                // ���� ������ ���� ���͸� ����� ����Ʈ ���� ����
                hitBloodEffect.transform.rotation = Quaternion.LookRotation(hit.normal);

                // ��ƼŬ ����Ʈ�� �θ� null�� �����Ͽ� ���� �����Ӱ� ���������� �����
                hitBloodEffect.transform.SetParent(null);
                hitBloodEffect?.Play();
                // ���� �༮�� HP�� ��´�
                Zombie zombie = GameManager.Instance.getZombieInParent(hit.collider.gameObject);
                zombie.HitByGun(shotgunDamage + GameManager.Instance.shotgunUpgradeDamage);
            }
            else if (hit.collider.CompareTag("Head"))
            {
                // ���� ������ ����Ʈ ��ġ ����
                hitBloodEffect.transform.position = hit.point;
                // ���� ������ ���� ���͸� ����� ����Ʈ ���� ����
                hitBloodEffect.transform.rotation = Quaternion.LookRotation(hit.normal);

                // ��ƼŬ ����Ʈ�� �θ� null�� �����Ͽ� ���� �����Ӱ� ���������� �����
                hitBloodEffect.transform.SetParent(null);
                hitBloodEffect?.Play();
                // ���� �༮�� HP�� ��´�
                Zombie zombie = GameManager.Instance.getZombieInParent(hit.collider.gameObject);
                zombie.HitByGunAtHead(shotgunDamage + GameManager.Instance.shotgunUpgradeDamage);
            }
            else if (hit.collider.CompareTag("Ground"))
            {
                GameObject concreteHitInstance = Instantiate(hitConcreteEffect, hit.point, Quaternion.LookRotation(hit.normal));
            }
        }
    }

    private Vector3 GetSpreadDirection()
    {
        Vector3 forward = firePos.forward;
        Vector3 spread = Vector3.zero;

        spread += firePos.up * Random.Range(-spreadAngle, spreadAngle);
        spread += firePos.right * Random.Range(-spreadAngle, spreadAngle);

        return (forward + spread).normalized;
    }

    private void Init()
    {
        cameraShake = GameObject.Find("Camera Offset").GetComponent<CameraShake>();
        currentBullets = bulletsPerMag;
        text.text = currentBullets.ToString() + "/";
        text.text += GameManager.Instance.shotgunTotal.ToString();

        firePos = transform.Find("FirePos");
        hitBloodEffect = firePos.transform.Find("Blood_Headshot").GetComponentInChildren<ParticleSystem>();
        cartridgeEffectPos = transform.Find("CartridgeEffect");
        ShotGunMuzzleFlash = firePos.GetComponentInChildren<ParticleSystem>();
        ShotGunCartridgeEffect = cartridgeEffectPos.GetComponentInChildren<ParticleSystem>();
        ShotGunAnim = GetComponent<Animator>();
    }

    void Attack()
    {
        int shotgunLevel = GameManager.Instance.augmentLevels[0];

        if (shotgunLevel < 3) // ���� 1, 2
        {
            FirePellet();
        }
        else if (shotgunLevel < 5) // ���� 3, 4
        {
            FirePellet();
        }
        else // ���� 5
        {
            ShotGunDistance = 50f;
            FirePelletUpgrade();
        }
    }
}
