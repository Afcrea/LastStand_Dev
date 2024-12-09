using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CrossBowAttack : UpgradeCard
{
    Button upgradeButton;
    // Start is called before the first frame update
    void Awake()
    {
        upgradeButton = GetComponent<Button>();
    }

    private void OnEnable()
    {
        SetUIText(6);
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
        GameManager.Instance.augmentLevels[6]++;

        if (GameManager.Instance.augmentLevels[6] == 1)
        {
            this.uiGameScene.UnlockWeapon(6);
        }
        else if (GameManager.Instance.augmentLevels[6] == 3)
        {
            AssignMaterialToChildren<LeftCrossBow>(1);
            AssignMaterialToChildren<RightCrossBow>(1);
        }
        else if (GameManager.Instance.augmentLevels[6] == 5)
        {
            AssignMaterialToChildren<LeftCrossBow>(2);
            AssignMaterialToChildren<RightCrossBow>(2);
        }

        GameManager.Instance.bowUpgradeDamage = GameManager.Instance.bowUpgradeDamage + 30;

        GameManager.Instance.isUpgrading = false;
    }
}
