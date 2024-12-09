using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Turret : MonoBehaviour
{
    private Transform partToRotate; // 터렛이 중심으로 회전할 회전축
    private Transform target; // 터렛이 쳐다보게 될 타겟
    private Transform firePos;
    public AudioClip turretSound;
    private ParticleSystem muzzleFlash;
    private float range = 15f; // 터렛이 적을 인식할 수 있는 범위
    private string enemyTag = "Enemy"; // 터렛이 인식할 적의 태그
    private float turnSpeed = 10f; // 터렛의 회전할 때 속도
    private int turretDamage = 1;
    bool isFire = false;
    bool isOn = true; // 터렛 온오프 체크
    public float targetingDelay = 0.5f; // 타겟 재설정 딜레이
    public float attackDelay = 1f; // 타겟 재설정 딜레이
    public AudioClip attackSound;
    //public LineRenderer renderer;

    private void Awake()
    {
        partToRotate = transform.Find("MGMain");
        firePos = partToRotate.transform.Find("FirePos");
        muzzleFlash = partToRotate.GetComponentInChildren<ParticleSystem>();
    }

    void OnEnable()
    {
        isOn = true;
        isFire = false;
        StartCoroutine(UpdateTarget());
    }

    void OnDisable()
    {
        isOn = false;
        StopCoroutine(UpdateTarget());
    }

    //private IEnumerator ShotRenderer()
    //{
    //    renderer.startWidth = 0.007f; // 선의 시작 너비
    //    renderer.endWidth = 0.003f;   // 선의 끝 너비

    //    // LineRenderer의 positionCount 설정
    //    renderer.positionCount = 2;

    //    // 선의 시작점은 FirePos
    //    renderer.SetPosition(0, firePos.position);
    //    // 선의 끝 점은 입력으로 들어온 충돌 위치
    //    renderer.SetPosition(1, firePos.position + transform.forward * range);
    //    // 라인 렌더러를 활성화 하여 탄알 궤적을 그림
    //    renderer.enabled = true;

    //    // 잠시 대기
    //    yield return new WaitForSeconds(0.03f);

    //    // 라인 렌더러 비활성화
    //    renderer.enabled = false;
    //}

    IEnumerator UpdateTarget()
    {
        while(isOn)
        {
            GameObject nearestEnemy = null;
            Collider[] colliders = Physics.OverlapSphere(transform.position, range); // 구형 탐지

            foreach (Collider collider in colliders)
            {
                if (collider.CompareTag("Enemy")) // 태그 확인
                {
                    Zombie zombie = GameManager.Instance.getZombieInParent(collider.gameObject);
                    if (zombie.isDead) continue;
                    nearestEnemy = zombie.gameObject;
                    break;
                }
            }

            if (nearestEnemy != null && !isFire)
            {
                target = nearestEnemy.transform;
                StartCoroutine(FireCoroutine(nearestEnemy));
            }

            yield return new WaitForSeconds(targetingDelay);
        }
    }

    void Update()
    {
        if (target == null)
        {
            isFire = false;
            return; // 타겟 없으면 가만히 있음
        }

        Vector3 dir = target.position - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(dir);
        Vector3 rotation = Quaternion.Lerp(partToRotate.rotation, lookRotation, Time.deltaTime * turnSpeed).eulerAngles;
        partToRotate.rotation = Quaternion.Euler(0f, rotation.y, 0f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }

    private IEnumerator FireCoroutine(GameObject target)
    {
        while (!target.GetComponent<Zombie>().isDead)
        {
            Fire();
            isFire = true;
            yield return new WaitForSeconds(attackDelay); // fireRate
        }

        yield return null;

        isFire = false;
    }

    private void Fire()
    {
        RaycastHit hit;

        if (Physics.Raycast(firePos.position, firePos.forward, out hit, range, GameManager.Instance.layerMask))
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                Zombie zombie = GameManager.Instance.getZombieInParent(hit.collider.gameObject);

                SoundManager.Instance.PlayOneShot(turretSound);
                muzzleFlash.Play();
                zombie.HitByGun(turretDamage + GameManager.Instance.turretUpgradeDamage);
                
            }
        }
        //StartCoroutine(ShotRenderer());
    }
}
