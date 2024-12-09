using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public GameObject hitBloodEffect;      // 피 터지는 효과 instanciate
    // public ParticleSystem _hitBloodEffect; // 피 터지는 효과 파티클 시스템
    private ParticleSystem _explosionEffect; // 폭발 효과 파티클 시스템
    public AudioClip explosionSound;         // 폭발 사운드
    private float explosionRadius = 5f;     // 폭발 범위
    private int explosionDamage = 10;       // 폭발 피해량
    bool attackDone = false;
    bool isOn = false;

    private void Awake()
    {
        // Arrow의 자식인 BigExplosionEffect
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
        // 폭발 효과를 생성
        //GameObject explosionInstance = Instantiate(explosionEffect, transform.position, Quaternion.identity);
        //Destroy(explosionInstance, 2f); // 폭발 효과가 2초 후에 파괴되도록 설정
        _explosionEffect?.Play();

        // 폭발 범위 내의 오브젝트에 피해를 입힌다
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

                // 폭발력 전달 (몹이 멀리 튕겨져 나갈때 다시 Player로 못와서 진행이 안됨;
                //Rigidbody enemyRb = hit.GetComponent<Rigidbody>();
                //if (enemyRb != null)
                //{
                //    // 죽었을 때 Constraints 체크 해제
                //    enemyRb.constraints = RigidbodyConstraints.None;
                //    // 폭발 방향 벡터 계산
                //    Vector3 explosionDirection = hit.transform.position - transform.position; // 폭발 중심과 적 위치의 벡터
                //    float explosionDistance = explosionDirection.magnitude; // 두 점 사이의 거리
                //    explosionDirection.Normalize(); // 방향 벡터로 정규화

                //    // 힘을 계산하고 적용 (폭발력과 거리 비례)
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
