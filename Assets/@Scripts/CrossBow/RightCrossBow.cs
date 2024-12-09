using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Haptics;
using UnityEngine.InputSystem.HID;
using UnityEngine.XR.Interaction.Toolkit;

public class RightCrossBow : MonoBehaviour
{
    public InputActionReference rightShoot; // 컨트롤러(트리거)키 눌림 체크
    public XRBaseController haptic; // 사격 시 컨트롤러 진동
    public GameObject rightArrow;              // 화살 프리팹
    public AudioClip bowSound;
    private GameObject ArrowInstance;        // 화살 프리팹의 인스턴스
    private Transform firePos;
    public GameObject hitBloodEffect; // Enemy에게 총알이 닿으면 피가 터지는 효과
    public GameObject hitConcreteEffect; // 벽에 탄환이 부딪히면 생기는 효과
    public LineRenderer renderer;

    private float crossBowDistance = 100f;
    private int crossBowDamage = 1;            // 화살 데미지

    bool isFire = false;
    private void Awake()
    {
        firePos = transform.Find("FirePos");
    }

    private void OnEnable()
    {
        if (rightShoot != null)
        {
            rightShoot.action.Enable();
            rightShoot.action.performed += OnTriggerPressed;
        }
        isFire = false;
    }

    private void OnDisable()
    {
        if (rightShoot != null)
        {
            rightShoot.action.performed -= OnTriggerPressed;
            rightShoot.action.Disable();
        }
    }

    private void OnTriggerPressed(InputAction.CallbackContext context)
    {
        Attack();
    }

    private void Fire()
    {
        if(isFire)
        {
            return;
        }

        haptic.SendHapticImpulse(1f, 0.3f);
        // 화살 생성
        ArrowInstance = Instantiate(rightArrow, firePos.position, firePos.rotation);
        SoundManager.Instance.PlayOneShot(bowSound);
        // 생성된 화살의 Rigidbody를 가져온다
        Rigidbody arrowRb = ArrowInstance.GetComponent<Rigidbody>();

        // 생성된 화살에 힘을 추가한다
        arrowRb.AddForce(transform.forward * 50f, ForceMode.Impulse);
        // 라인 렌더러 실행
        StartCoroutine(wait1s());
    }

    IEnumerator wait1s()
    {
        isFire = true;

        yield return new WaitForSecondsRealtime(1f);

        isFire = false;
    }

    private IEnumerator ShotRenderer(float crossBowDistance)
    {
        renderer.startWidth = 0.003f; // 선의 시작 너비
        renderer.endWidth = 0.003f;   // 선의 끝 너비

        // LineRenderer의 positionCount 설정
        renderer.positionCount = 2;

        // 선의 시작점은 FirePos
        renderer.SetPosition(0, firePos.position);
        // 선의 끝 점은 입력으로 들어온 충돌 위치
        renderer.SetPosition(1, firePos.position + transform.forward * crossBowDistance);
        // 라인 렌더러를 활성화 하여 탄알 궤적을 그림
        renderer.enabled = true;

        yield return new WaitForSecondsRealtime(0.03f);

        // 라인 렌더러 비활성화
        renderer.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        Destroy(ArrowInstance);
    }

    void Attack()
    {
        haptic.SendHapticImpulse(1f, 0.3f);

        int CrossBowLevel = GameManager.Instance.augmentLevels[6];

        if (CrossBowLevel < 3) // 레벨 1, 2
        {
            NormalFire(CrossBowLevel);
        }
        else if (CrossBowLevel < 5) // 레벨 3, 4
        {
            NormalFire(CrossBowLevel);
        }
        else // 레벨 5
        {
            Fire();
        }
    }

    // 저렙 석궁 발사 로직
    void NormalFire(int CrossBowLevel)
    {
        SoundManager.Instance.PlayOneShot(bowSound);

        RaycastHit hit;

        // 레이가 발사됨
        if (Physics.Raycast(firePos.position, firePos.forward, out hit, crossBowDistance, GameManager.Instance.layerMask))
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                Zombie zombie = GameManager.Instance.getZombieInParent(hit.collider.gameObject);

                GameObject bloodHitInstance = Instantiate(hitBloodEffect, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(bloodHitInstance, 1f);
                // 맞은 녀석의 HP를 1 깎는다
                zombie.HitByGun(crossBowDamage + GameManager.Instance.bowUpgradeDamage + CrossBowLevel);
            }
            else if (hit.collider.CompareTag("Head"))
            {
                Zombie zombie = GameManager.Instance.getZombieInParent(hit.collider.gameObject);

                GameObject bloodHitInstance = Instantiate(hitBloodEffect, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(bloodHitInstance, 1f);
                // 맞은 녀석의 HP를 1 깎는다
                zombie.HitByGunAtHead(crossBowDamage + GameManager.Instance.bowUpgradeDamage + CrossBowLevel);
            }
            else if (hit.collider.CompareTag("Ground"))
            {
                GameObject concreteHitInstance = Instantiate(hitConcreteEffect, hit.point, Quaternion.LookRotation(hit.normal));
            }

            StartCoroutine(ShotRenderer(crossBowDistance));
        }
    }
}
