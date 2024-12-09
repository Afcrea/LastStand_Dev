using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStartTrigger : Zombie
{
    // Update is called once per frame
    void Update()
    {
        ZombieAction();
    }

    private void OnEnable()
    {
        this.hp = 1;
        this.isDead = false;
    }

    void ZombieAction()
    {
        if (isDead) return;

        if (hp <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        this.isDead = true;

        gameObject.SetActive(false);

        GameManager.Instance.startWave = true;
    }


}
