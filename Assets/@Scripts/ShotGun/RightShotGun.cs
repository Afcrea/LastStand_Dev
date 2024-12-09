using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;

public class RightShotGun : MonoBehaviour
{
    public LeftShotGun l_shotgun;
    public TextMeshProUGUI text;
    public InputActionReference rightShootShotGun; // 이 스크립트 인스턴스에 할당된 손의 입력 매핑
    public AudioClip ShotGunSound; // 샷건 사격 사운드
    public XRBaseController haptic; // 사격 시 컨트롤러 진동
    public GameObject hitConcreteEffect; // 벽에 탄환이 부딪히면 생기는 효과
    private Transform firePos; // 레이가 나가는 위치
    private Transform cartridgeEffectPos; // 샷건 탄피 이펙트가 나가는 위치
    private ParticleSystem hitBloodEffect; // Enemy에게 총알이 닿으면 피가 터지는 효과
    private ParticleSystem ShotGunMuzzleFlash; // 샷건 머즐 플래쉬
    private ParticleSystem ShotGunCartridgeEffect; // 샷건 탄피 이펙트
    private Animator ShotGunAnim;
    private CameraShake cameraShake;

    private int pelletCount = 10; // 발사되는 탄환 수
    private float spreadAngle = 0.05f; // 퍼지는 각도
    private float ShotGunDistance = 20f; // 샷건 사정거리
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

    // 키 입력이 들어오면(Trigger 버튼) Fire 메서드 실행
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

        // 필요한 탄환 수 계산
        int bulletsNeeded = bulletsPerMag - currentBullets;

        // 장전할 수 있는 최대 탄환 수 결정
        int bulletsToReload = Mathf.Min(bulletsNeeded, GameManager.Instance.shotgunTotal);
        currentBullets += bulletsToReload;
        GameManager.Instance.shotgunTotal -= bulletsToReload;

        // UI 텍스트 업데이트
        text.text = currentBullets.ToString() + "/" + GameManager.Instance.shotgunTotal.ToString();
        l_shotgun.text.text = l_shotgun.currentBullets.ToString() + "/" + GameManager.Instance.shotgunTotal.ToString();
    }

    private void Fire()
    {
        StartCoroutine(cameraShake.ShakeCamera(0.03f, 0, 0.3f));
        currentBullets--; // 1발씩 감소
        haptic.SendHapticImpulse(1f, 0.6f);
        text.text = currentBullets.ToString() + "/";
        text.text += GameManager.Instance.shotgunTotal.ToString();

        // 탄환의 수만큼 for문 진행
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

        // 탄환이 퍼지는 각도를 받음
        Vector3 direction = GetSpreadDirection();

        // firePos에서 출발한 탄환이 나아 가야 할 방향은 direction 으로 나아간다
        if (Physics.Raycast(firePos.position, direction, out hit, ShotGunDistance, GameManager.Instance.layerMask))
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                // 맞은 지점에 이펙트 위치 설정
                hitBloodEffect.transform.position = hit.point;
                // 맞은 지점의 법선 벡터를 사용해 이펙트 방향 설정
                hitBloodEffect.transform.rotation = Quaternion.LookRotation(hit.normal);

                // 파티클 이펙트의 부모를 null로 설정하여 총의 움직임과 독립적으로 만들기
                hitBloodEffect.transform.SetParent(null);
                hitBloodEffect?.Play();
                // 맞은 녀석의 HP를 깎는다
                Zombie zombie = GameManager.Instance.getZombieInParent(hit.collider.gameObject);
                zombie.HitByGun(shotgunDamage + GameManager.Instance.shotgunUpgradeDamage);
            }
            else if (hit.collider.CompareTag("Head"))
            {
                // 맞은 지점에 이펙트 위치 설정
                hitBloodEffect.transform.position = hit.point;
                // 맞은 지점의 법선 벡터를 사용해 이펙트 방향 설정
                hitBloodEffect.transform.rotation = Quaternion.LookRotation(hit.normal);

                // 파티클 이펙트의 부모를 null로 설정하여 총의 움직임과 독립적으로 만들기
                hitBloodEffect.transform.SetParent(null);
                hitBloodEffect?.Play();
                // 맞은 녀석의 HP를 깎는다
                Zombie zombie = GameManager.Instance.getZombieInParent(hit.collider.gameObject);
                zombie.HitByGunAtHead(shotgunDamage + GameManager.Instance.shotgunUpgradeDamage);
            }
            else if (hit.collider.CompareTag("Ground"))
            {
                GameObject concreteHitInstance = Instantiate(hitConcreteEffect, hit.point, Quaternion.LookRotation(hit.normal));
            }
        }
    }

    // 관통 로직
    private void FirePelletUpgrade()
    {
        // 탄환이 퍼지는 각도를 받음
        Vector3 direction = GetSpreadDirection();

        Ray ray = new Ray(transform.position, direction);

        RaycastHit[] hits = Physics.RaycastAll(ray, ShotGunDistance, GameManager.Instance.layerMask);

        // firePos에서 출발한 탄환이 나아 가야 할 방향은 direction 으로 나아간다
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                // 맞은 지점에 이펙트 위치 설정
                hitBloodEffect.transform.position = hit.point;
                // 맞은 지점의 법선 벡터를 사용해 이펙트 방향 설정
                hitBloodEffect.transform.rotation = Quaternion.LookRotation(hit.normal);

                // 파티클 이펙트의 부모를 null로 설정하여 총의 움직임과 독립적으로 만들기
                hitBloodEffect.transform.SetParent(null);
                hitBloodEffect?.Play();
                // 맞은 녀석의 HP를 깎는다
                Zombie zombie = GameManager.Instance.getZombieInParent(hit.collider.gameObject);
                zombie.HitByGun(shotgunDamage + GameManager.Instance.shotgunUpgradeDamage);
            }
            else if (hit.collider.CompareTag("Head"))
            {
                // 맞은 지점에 이펙트 위치 설정
                hitBloodEffect.transform.position = hit.point;
                // 맞은 지점의 법선 벡터를 사용해 이펙트 방향 설정
                hitBloodEffect.transform.rotation = Quaternion.LookRotation(hit.normal);

                // 파티클 이펙트의 부모를 null로 설정하여 총의 움직임과 독립적으로 만들기
                hitBloodEffect.transform.SetParent(null);
                hitBloodEffect?.Play();
                // 맞은 녀석의 HP를 깎는다
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

        if (shotgunLevel < 3) // 레벨 1, 2
        {
            FirePellet();
        }
        else if (shotgunLevel < 5) // 레벨 3, 4
        {
            FirePellet();
        }
        else // 레벨 5
        {
            ShotGunDistance = 50f;
            FirePelletUpgrade();
        }
    }
}
