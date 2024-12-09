using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurretAttack : UpgradeCard
{
    Button upgradeButton;
    // Start is called before the first frame update
    void Awake()
    {
        upgradeButton = GetComponent<Button>();
    }

    private void OnEnable()
    {
        SetUIText(4);
        StartCoroutine(Wait5sButton());
    }

    private void OnDisable()
    {
        upgradeButton.onClick.RemoveListener(UpgradeEffect);
    }

    IEnumerator Wait5sButton()
    {
        yield return new WaitForSecondsRealtime(1f);

        upgradeButton.onClick.AddListener(UpgradeEffect);
    }
    void UpgradeEffect()
    {
        GameManager.Instance.augmentLevels[4]++;
        
        if(GameManager.Instance.augmentLevels[4] == 1)
        {
            this.uiGameScene.UnlockWeapon(4);
        }
        else if(GameManager.Instance.augmentLevels[4] == 3)
        {
            ChangeTurretAttackDelay(0.1f);
            AssignMaterialToChildren<Turret>(1);
        }
        else if (GameManager.Instance.augmentLevels[4] == 5)
        {
            ChangeTurretTargetingDelay(0.01f);
            AssignMaterialToChildren<Turret>(2);
        }

        GameManager.Instance.turretUpgradeDamage = GameManager.Instance.turretUpgradeDamage + 33;

        GameManager.Instance.isUpgrading = false;
    }

    void ChangeTurretTargetingDelay(float delay)
    {
        Turret[] turrets = Resources.FindObjectsOfTypeAll<Turret>();
        foreach (Turret turret in turrets)
        {
            turret.targetingDelay = delay;
        }
    }

    void ChangeTurretAttackDelay(float delay)
    {
        Turret[] turrets = Resources.FindObjectsOfTypeAll<Turret>();
        foreach (Turret turret in turrets)
        {
            turret.attackDelay = delay;
        }
    }
}
