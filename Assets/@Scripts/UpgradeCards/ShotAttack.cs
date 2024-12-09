using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShotAttack : UpgradeCard
{
    Button upgradeButton;
    // Start is called before the first frame update
    void Awake()
    {
        upgradeButton = GetComponent<Button>();
    }

    private void OnEnable()
    {
        SetUIText(2);
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
        GameManager.Instance.augmentLevels[2]++;

        if (GameManager.Instance.augmentLevels[2] == 1)
        {
            this.uiGameScene.UnlockWeapon(2);
        }
        else if (GameManager.Instance.augmentLevels[2] == 3)
        {
            AssignMaterialToChildren<LeftShotGun>(1);
            AssignMaterialToChildren<RightShotGun>(1);
        }
        else if (GameManager.Instance.augmentLevels[2] == 5)
        {
            AssignMaterialToChildren<LeftShotGun>(2);
            AssignMaterialToChildren<RightShotGun>(2);
        }

        GameManager.Instance.shotgunTotal = GameManager.Instance.shotgunTotal + 50;
        GameManager.Instance.shotgunUpgradeDamage = GameManager.Instance.shotgunUpgradeDamage + 25;

        GameManager.Instance.isUpgrading = false;
    }
}
