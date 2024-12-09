using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MachineAttack : UpgradeCard
{
    Button upgradeButton;
    // Start is called before the first frame update
    void Awake()
    {
        upgradeButton = GetComponent<Button>();
    }

    private void OnEnable()
    {
        SetUIText(3);
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
        GameManager.Instance.augmentLevels[3]++;

        if (GameManager.Instance.augmentLevels[3] == 1)
        {
            this.uiGameScene.UnlockWeapon(3);
        }
        else if (GameManager.Instance.augmentLevels[3] == 3)
        {
            AssignMaterialToChildren<LeftMachineGun>(1);
            AssignMaterialToChildren<RightMachineGun>(1);
        }
        else if (GameManager.Instance.augmentLevels[3] == 5)
        {
            AssignMaterialToChildren<LeftMachineGun>(2);
            AssignMaterialToChildren<RightMachineGun>(2);
        }

        GameManager.Instance.machinegunTotal = GameManager.Instance.machinegunTotal + 300;
        GameManager.Instance.machinegunUpgradeDamage = GameManager.Instance.machinegunUpgradeDamage + 10;

        GameManager.Instance.isUpgrading = false;
    }
}
