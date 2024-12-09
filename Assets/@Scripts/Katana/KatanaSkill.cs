using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine.XR;

public class KatanaSkill : MonoBehaviour
{
    public InputActionReference leftTriggerAction;  // InputReference 사용
    // public InputActionReference rightTriggerAction;

    public Transform spawnPoint; // 참격 오브젝트 생성 위치
    public GameObject objectToSpawn;   // 생성할 오브젝트
    private GameObject objectToSpawnInstance; // 생성한 오브젝트의 인스턴스
    private ParticleSystem bloodEffect; // 벨 때 나오는 피
    public AudioClip slashSound;
    public AudioClip slashSkillSound;
    public AudioClip swordSwing;
    private int katanaDamage = 10;
    public float speedThreshold = 10f;  // 속도 임계값

    private Vector3 leftPrevPosition;  // 왼쪽 컨트롤러의 이전 위치
    private float leftSpeed;             // 왼쪽 컨트롤러의 속도

    private void Awake()
    {
        // KatanaSkill 자식의 Blood_Headshot
        bloodEffect = transform.Find("Blood_Headshot").GetComponentInChildren<ParticleSystem>();
    }

    void OnEnable()
    {
        // 트리거 액션에 이벤트 핸들러 추가
        if (leftTriggerAction != null)
        {
            leftTriggerAction.action.Enable();

            leftTriggerAction.action.performed += OnTriggerPressed;
        }

        // 초기 위치 설정
        leftPrevPosition = transform.position;
    }

    private void OnDisable()
    {
        // 트리거 액션에서 이벤트 핸들러 제거
        if (leftTriggerAction != null)
        {
            leftTriggerAction.action.performed -= OnTriggerPressed;

            leftTriggerAction.action.Disable();
        }
    }

    private void Update()
    {
        // 컨트롤러의 속도 계산 (위치 차이 / deltaTime)
        CalculateControllerSpeed();

    }

    private void OnTriggerPressed(InputAction.CallbackContext context)
    {
        Attack();
    }

    private void CalculateControllerSpeed()
    {
        // 왼쪽 컨트롤러의 위치 변화량 계산
        Vector3 leftCurrentPosition = transform.position; // 예시로 transform 사용, 실제로는 좌측 컨트롤러의 위치 사용
        float leftDistance = Vector3.Distance(leftPrevPosition, leftCurrentPosition);
        leftSpeed = leftDistance / Time.deltaTime;
        // 이전 위치 업데이트
        leftPrevPosition = leftCurrentPosition;

        // 허공에 휘둘러도 일정 속도가 넘는다면 소리 재생
        if (leftSpeed > speedThreshold)
        {
            SoundManager.Instance.PlayOneShot(swordSwing);
        }
    }

    // 검기 생성
    private void SpawnObjectInDirection()
    {
        if (objectToSpawn != null)
        {
            Vector3 dir = GameManager.Instance.cameraVR.transform.forward;

            dir.y = 0f ;
            
            dir.Normalize();

            Quaternion rotation = Quaternion.LookRotation(dir);

            SoundManager.Instance.PlayOneShotNonRepeat(slashSkillSound);
            // 오브젝트를 생성하고, 진행 방향은 firePos.forward로 설정 new Quaternion(90, 0, Quaternion.identity.z, 0)
            objectToSpawnInstance = Instantiate(objectToSpawn, spawnPoint.position, rotation);
            //StartCoroutine(ForwardSlash(objectToSpawnInstance));

            //Rigidbody rb = objectToSpawnInstance.GetComponent<Rigidbody>();
            //rb.AddForce(spawnPoint.forward * 20, ForceMode.Impulse);
            //Destroy(objectToSpawnInstance, 2.0f);
        }
    }

    private IEnumerator ForwardSlash(GameObject obj)
    {
        float speed = 10f;
        float duration = 4f;  // 전진할 시간 설정
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            // 점진적 전진
            obj.transform.position += obj.transform.forward * Time.deltaTime * speed;
            // 점진적 크기 증가
            //obj.transform.localScale *= 1.05f;
            BoxCollider col = GetComponent<BoxCollider>();
            //col.size *= 1.05f;
            elapsedTime += Time.unscaledDeltaTime; ;  // 경과 시간 업데이트
            yield return new WaitForSecondsRealtime(1 / 60);
        }

        Destroy(obj);
    }

    void Attack() // 업그레이드 상태에 따라 공격 변경
    {
        if (GameManager.Instance.augmentLevels[5] < 3) // 3 레벨부터 검기 발사
        {
            return;
        }

        // 왼쪽 트리거와 오른쪽 트리거의 속도가 임계값을 초과한 경우
        if (leftSpeed > speedThreshold)
        {
            SpawnObjectInDirection();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // 1. 왼쪽 컨트롤러의 속도값이 임계값을 넘는다면
        // 2. 카타나와 몬스터의 콜라이더 충돌이 일어난다
        if (leftSpeed > speedThreshold)
        {
            if (other.CompareTag("Enemy"))
            {
                Zombie zombie = other.GetComponent<Zombie>();

                if (!zombie)
                {
                    return;
                }

                zombie.HitByGun(katanaDamage + GameManager.Instance.swordUpgradeDamage);

                // 콜라이더의 경계에서 충돌 지점까지의 최단 거리를 제공함
                // 한마디로 콜라이더의 중심점이 아니라 닿은 부분을 리턴시켜준다는 의미인듯?
                bloodEffect.transform.position = other.ClosestPoint(transform.position);
                bloodEffect?.transform.SetParent(null);
                bloodEffect?.Play();
                SoundManager.Instance.PlayOneShotNonRepeat(slashSound);
            }
        }
    }
}
