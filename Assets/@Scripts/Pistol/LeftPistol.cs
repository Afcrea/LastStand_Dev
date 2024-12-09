using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.HID;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(Animator))]
public class LeftPistol : MonoBehaviour
{
    public InputActionReference leftShootPistol; // 이 스크립트 인스턴스에 할당된 손의 입력 매핑
    public XRBaseController haptic; // 사격 시 컨트롤러 진동
    public AudioClip pistolSound; // 권총 사격 사운드
    public GameObject hitBloodEffect; // Enemy에게 총알이 닿으면 피가 터지는 효과
    public GameObject hitConcreteEffect; // 벽에 탄환이 부딪히면 생기는 효과
    private Transform firePos; // 총알이 나가는 위치
    private Transform cartridgeEffectPos; // 권총 탄피 이펙트가 나가는 위치
    private ParticleSystem pistolMuzzleFlash; // 권총 머즐 플래쉬
    private ParticleSystem pistolCartridgeEffect; // 권총 탄피 이펙트
    private Animator pistolAnim;
    private CameraShake cameraShake;
    public LineRenderer renderer;

    private float pistolDistance = 500f; // 권총 사정거리
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

        // 레이가 발사됨
        if (Physics.Raycast(firePos.position, firePos.forward, out hit, pistolDistance, GameManager.Instance.layerMask))
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                GameObject bloodHitInstance = Instantiate(hitBloodEffect, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(bloodHitInstance, 1f);
                // 맞은 녀석의 HP를 1 깎는다
                Attack(hit);
            }
            else if (hit.collider.CompareTag("Head"))
            {
                GameObject bloodHitInstance = Instantiate(hitBloodEffect, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(bloodHitInstance, 1f);
                // 맞은 녀석의 HP를 1 깎는다
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
        renderer.startWidth = 0.003f; // 선의 시작 너비
        renderer.endWidth = 0.003f;   // 선의 끝 너비

        // LineRenderer의 positionCount 설정
        renderer.positionCount = 2;

        // 선의 시작점은 FirePos
        renderer.SetPosition(0, firePos.position);
        // 선의 끝 점은 입력으로 들어온 충돌 위치
        renderer.SetPosition(1, firePos.position + transform.forward * pistolDistance);
        // 라인 렌더러를 활성화 하여 탄알 궤적을 그림
        renderer.enabled = true;

        // 잠시 대기
        yield return new WaitForSecondsRealtime(0.03f);

        // 라인 렌더러 비활성화
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

        if (pistolLevel < 3) // 레벨 1, 2
        {
            zombie.HitByGun(pistolDamage + GameManager.Instance.pistolUpgradeDamage + (pistolLevel * 10));
        }
        else if (pistolLevel < 5) // 레벨 3, 4
        {
            zombie.HitByGun(pistolDamage + GameManager.Instance.pistolUpgradeDamage + (pistolLevel * 25));
        }
        else // 레벨 5
        {
            zombie.HitByGun(pistolDamage + GameManager.Instance.pistolUpgradeDamage + (pistolLevel * 50));
        }
    }
    void HeadShot(RaycastHit hit)
    {
        int pistolLevel = GameManager.Instance.augmentLevels[0];

        Zombie zombie = GameManager.Instance.getZombieInParent(hit.collider.gameObject);

        if (pistolLevel < 3) // 레벨 1, 2
        {
            zombie.HitByGunAtHead(pistolDamage + GameManager.Instance.pistolUpgradeDamage + (pistolLevel * 10));
        }
        else if (pistolLevel < 5) // 레벨 3, 4
        {
            zombie.HitByGunAtHead(pistolDamage + GameManager.Instance.pistolUpgradeDamage + (pistolLevel * 25));
        }
        else // 레벨 5
        {
            zombie.HitByGunAtHead(pistolDamage + GameManager.Instance.pistolUpgradeDamage + (pistolLevel * 50));
        }
    }
}
