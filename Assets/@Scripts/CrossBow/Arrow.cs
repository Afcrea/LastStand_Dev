using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public GameObject hitBloodEffect;      // �� ������ ȿ�� instanciate
    // public ParticleSystem _hitBloodEffect; // �� ������ ȿ�� ��ƼŬ �ý���
    private ParticleSystem _explosionEffect; // ���� ȿ�� ��ƼŬ �ý���
    public AudioClip explosionSound;         // ���� ����
    private float explosionRadius = 5f;     // ���� ����
    private int explosionDamage = 10;       // ���� ���ط�
    bool attackDone = false;
    bool isOn = false;

    private void Awake()
    {
        // Arrow�� �ڽ��� BigExplosionEffect
        _explosionEffect = GetComponentInChildren<ParticleSystem>();
        explosionRadius = 5f;
        attackDone = false;
        isOn = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(isOn)
        {
            return;
        }

        isOn = true;

        Explode();

        StartCoroutine(waitAttack(2f));
    }

    private void Explode()
    {
        SoundManager.Instance.PlayOneShot(explosionSound);
        // ���� ȿ���� ����
        //GameObject explosionInstance = Instantiate(explosionEffect, transform.position, Quaternion.identity);
        //Destroy(explosionInstance, 2f); // ���� ȿ���� 2�� �Ŀ� �ı��ǵ��� ����
        _explosionEffect?.Play();

        // ���� ���� ���� ������Ʈ�� ���ظ� ������
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius, GameManager.Instance.layerMask);

        Zombie prevZombie = null;
        foreach (Collider hit in colliders)
        {
            if (hit.CompareTag("Enemy"))
            {
                Zombie zombie = GameManager.Instance.getZombieInParent(hit.gameObject);

                if(zombie == prevZombie)
                {
                    continue;
                }

                if(zombie.isDead)
                {
                    continue;
                }

                prevZombie = zombie;

                Attack(hit, zombie);

                // ���߷� ���� (���� �ָ� ƨ���� ������ �ٽ� Player�� ���ͼ� ������ �ȵ�;
                //Rigidbody enemyRb = hit.GetComponent<Rigidbody>();
                //if (enemyRb != null)
                //{
                //    // �׾��� �� Constraints üũ ����
                //    enemyRb.constraints = RigidbodyConstraints.None;
                //    // ���� ���� ���� ���
                //    Vector3 explosionDirection = hit.transform.position - transform.position; // ���� �߽ɰ� �� ��ġ�� ����
                //    float explosionDistance = explosionDirection.magnitude; // �� �� ������ �Ÿ�
                //    explosionDirection.Normalize(); // ���� ���ͷ� ����ȭ

                //    // ���� ����ϰ� ���� (���߷°� �Ÿ� ���)
                //    float forceMagnitude = explosionDamage / Mathf.Max(explosionDistance, 1f) * 2;
                //    enemyRb.AddForce(explosionDirection * forceMagnitude, ForceMode.Impulse);
                //}
            }
        }

        attackDone = true;
    }

    private void Attack(Collider hit, Zombie zombie)
    {
        zombie.HitByGun(explosionDamage + GameManager.Instance.bowUpgradeDamage + 500);
        GameObject bloodInstance = Instantiate(hitBloodEffect, transform.position, Quaternion.identity);
        bloodInstance.transform.position = hit.transform.position;
        Destroy(bloodInstance, 1f);
    }

    IEnumerator waitAttack(float delay)
    {
        while(!attackDone)
        {
            yield return new WaitForSeconds(delay);
        }

        Destroy(gameObject);
    }
}
