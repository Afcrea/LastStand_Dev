using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RfAttack : UpgradeCard
{
    Button upgradeButton;
    // Start is called before the first frame update
    void Awake()
    {
        upgradeButton = GetComponent<Button>();
    }

    private void OnEnable()
    {
        SetUIText(1);
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
        GameManager.Instance.augmentLevels[1]++;

        if (GameManager.Instance.augmentLevels[1] == 1)
        {
            this.uiGameScene.UnlockWeapon(1);
        }
        else if (GameManager.Instance.augmentLevels[1] == 3)
        {
            AssignMaterialToChildren<LeftRifle>(1);
            AssignMaterialToChildren<RightRifle>(1);
        }
        else if (GameManager.Instance.augmentLevels[1] == 5)
        {
            AssignMaterialToChildren<LeftRifle>(2);
            AssignMaterialToChildren<RightRifle>(2);
        }

        GameManager.Instance.rifleTotal = GameManager.Instance.rifleTotal + 200;
        GameManager.Instance.rifleUpgradeDamage = GameManager.Instance.rifleUpgradeDamage + 70;

        GameManager.Instance.isUpgrading = false;
    }
}
