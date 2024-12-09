using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CreepZombie : Zombie
{
    public GameObject parent;
    private void OnEnable()
    {
        this.attackAnimationDelay = 0.74f;
        this.attackDelay = 0.63f;
        this.moveSpeed = 10f;
        this.EXP = this.EXP + 1000;
        this.maxHp = this.hp;
    }

    // Update is called once per frame
    void Update()
    {
        ZombieAction();
        if (HPBarImage != null)
        {
            HPBarImage.fillAmount = (float)this.hp / (float)this.maxHp;
        }
    }

    void ZombieAction()
    {
        if (isDead) return;

        if (hp <= 0)
        {
            Die();
        }
        else
        {
            if (player != null)
            {
                // 현재 위치에서 타겟 위치로 이동합니다.
                //if(CheckPlayer())
                if (CheckPlayer())
                {
                    Attack();
                }
                else
                {
                    Move();
                }
            }
        }
    }

    void Die()
    {
        this.isDead = true;

        GameManager.Instance.GetZombieEXP(EXP, score);

        animator.SetTrigger("Dead");

        CapsuleCollider capsuleCollider = transform.GetComponent<CapsuleCollider>();
        capsuleCollider.height = 0.1f;
        capsuleCollider.center = new Vector3(0, 0.1f, 0);
        capsuleCollider.radius = 0.1f;
        Head.GetComponent<SphereCollider>().enabled = false;

        HPBarUI.SetActive(false);
        HPBar.SetActive(false);

        StartCoroutine(DeadDestroy());
    }

    IEnumerator DeadDestroy()
    {
        GameManager.Instance.zombieCount--;

        yield return new WaitForSeconds(5f);

        //Destroy(parent);
        GameManager.DestroyWithCleanup(parent);
    }
}
