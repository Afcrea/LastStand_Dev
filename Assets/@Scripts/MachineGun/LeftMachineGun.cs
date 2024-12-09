using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;

public class LeftMachineGun : MonoBehaviour
{
    public RightMachineGun r_mashinegun;
    public TextMeshProUGUI text;
    public InputActionReference leftShootRifle; // 이 스크립트 인스턴스에 할당된 손의 입력 매핑
    public AudioClip machineGunSound; // 머신건 사격 사운드
    public XRBaseController haptic; // 사격 시 컨트롤러 진동
    public GameObject hitBloodEffect; // Enemy에게 총알이 닿으면 피가 터지는 효과
    public GameObject hitConcreteEffect; // 벽에 탄환이 부딪히면 생기는 효과
    private Transform firePos; // 레이가 나가는 위치
    private Transform cartridgeEffectPos; // 머신건 탄피 이펙트가 나가는 위치
    private ParticleSystem machineGunMuzzleFlash; // 머신건 머즐 플래쉬
    private ParticleSystem machineGunCartridgeEffect; // 머신건 탄피 이펙트
    private Coroutine firingCoroutine;
    private WaitForSeconds rateOfFire = new WaitForSeconds(0.1f); // 머신건 연사속도
    private CameraShake cameraShake;
    public LineRenderer renderer;

    private float machineGunDistance = 500f; // 머신건 사정거리
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
            yield return rateOfFire; // 발사 간격
        }
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
        renderer.SetPosition(1, firePos.position + transform.forward * machineGunDistance);
        // 라인 렌더러를 활성화 하여 탄알 궤적을 그림
        renderer.enabled = true;

        // 잠시 대기
        yield return new WaitForSecondsRealtime(0.03f);

        // 라인 렌더러 비활성화
        renderer.enabled = false;
    }

    public void Reroad()
    {
        if (GameManager.Instance.augmentLevels[3] >= 3) // 레벨 3부터 장전안함
        {
            return; 
        }

        if (GameManager.Instance.machinegunTotal <= 0)
        {
            GameManager.Instance.machinegunTotal = 0;
            return;
        }

        // 필요한 탄환 수 계산
        int bulletsNeeded = bulletsPerMag - currentBullets;

        // 장전할 수 있는 최대 탄환 수 결정
        int bulletsToReload = Mathf.Min(bulletsNeeded, GameManager.Instance.machinegunTotal);
        currentBullets += bulletsToReload;
        GameManager.Instance.machinegunTotal -= bulletsToReload;

        // UI 텍스트 업데이트
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

            if (machineGunLevel < 3) // 레벨 1, 2
            {
                currentBullets--; // 1발씩 감소
                text.text = currentBullets.ToString() + "/";
                text.text += GameManager.Instance.machinegunTotal.ToString();
            }
            else if (machineGunLevel < 5) // 레벨 3, 4
            {
                GameManager.Instance.machinegunTotal--;
                currentBullets = GameManager.Instance.machinegunTotal;
                
                if (currentBullets <= 0)
                {
                    currentBullets = 0;
                }

                text.text = currentBullets.ToString();
            }
            else // 레벨 5
            {
                currentBullets = 10;
                // 무한 표기
                text.text = "9999";
            }

            SoundManager.Instance.PlayOneShot(machineGunSound);
            machineGunMuzzleFlash.Play();
            machineGunCartridgeEffect.Play();

            RaycastHit hit;

            // 레이가 발사됨
            if (Physics.Raycast(firePos.position, firePos.forward, out hit, machineGunDistance, GameManager.Instance.layerMask))
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

        if (machineGunLevel < 3) // 레벨 1, 2
        {
            zombie.HitByGun(machineGunDamage + GameManager.Instance.machinegunUpgradeDamage + machineGunLevel);
        }
        else if (machineGunLevel < 5) // 레벨 3, 4
        {
            zombie.HitByGun(machineGunDamage + GameManager.Instance.machinegunUpgradeDamage + machineGunLevel);
        }
        else // 레벨 5
        {
            zombie.HitByGun(machineGunDamage + GameManager.Instance.machinegunUpgradeDamage + machineGunLevel);
            currentBullets = 999;
            // 여기에 무한 이미지 띄우기
        }
    }

    void HeadShot(RaycastHit hit)
    {
        int machineGunLevel = GameManager.Instance.augmentLevels[3];

        Zombie zombie = GameManager.Instance.getZombieInParent(hit.collider.gameObject);

        if (machineGunLevel < 3) // 레벨 1, 2
        {
            zombie.HitByGunAtHead(machineGunDamage + GameManager.Instance.machinegunUpgradeDamage + machineGunLevel);
        }
        else if (machineGunLevel < 5) // 레벨 3, 4
        {
            zombie.HitByGunAtHead(machineGunDamage + GameManager.Instance.machinegunUpgradeDamage + machineGunLevel);
        }
        else // 레벨 5
        {
            zombie.HitByGunAtHead(machineGunDamage + GameManager.Instance.machinegunUpgradeDamage + machineGunLevel);
            currentBullets = 999;
        }
    }

}
