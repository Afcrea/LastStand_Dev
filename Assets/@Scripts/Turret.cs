using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Turret : MonoBehaviour
{
    private Transform partToRotate; // �ͷ��� �߽����� ȸ���� ȸ����
    private Transform target; // �ͷ��� �Ĵٺ��� �� Ÿ��
    private Transform firePos;
    public AudioClip turretSound;
    private ParticleSystem muzzleFlash;
    private float range = 15f; // �ͷ��� ���� �ν��� �� �ִ� ����
    private string enemyTag = "Enemy"; // �ͷ��� �ν��� ���� �±�
    private float turnSpeed = 10f; // �ͷ��� ȸ���� �� �ӵ�
    private int turretDamage = 1;
    bool isFire = false;
    bool isOn = true; // �ͷ� �¿��� üũ
    public float targetingDelay = 0.5f; // Ÿ�� �缳�� ������
    public float attackDelay = 1f; // Ÿ�� �缳�� ������
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
    //    renderer.startWidth = 0.007f; // ���� ���� �ʺ�
    //    renderer.endWidth = 0.003f;   // ���� �� �ʺ�

    //    // LineRenderer�� positionCount ����
    //    renderer.positionCount = 2;

    //    // ���� �������� FirePos
    //    renderer.SetPosition(0, firePos.position);
    //    // ���� �� ���� �Է����� ���� �浹 ��ġ
    //    renderer.SetPosition(1, firePos.position + transform.forward * range);
    //    // ���� �������� Ȱ��ȭ �Ͽ� ź�� ������ �׸�
    //    renderer.enabled = true;

    //    // ��� ���
    //    yield return new WaitForSeconds(0.03f);

    //    // ���� ������ ��Ȱ��ȭ
    //    renderer.enabled = false;
    //}

    IEnumerator UpdateTarget()
    {
        while(isOn)
        {
            GameObject nearestEnemy = null;
            Collider[] colliders = Physics.OverlapSphere(transform.position, range); // ���� Ž��

            foreach (Collider collider in colliders)
            {
                if (collider.CompareTag("Enemy")) // �±� Ȯ��
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
            return; // Ÿ�� ������ ������ ����
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
