using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwordAttack : UpgradeCard
{
    Button upgradeButton;
    // Start is called before the first frame update
    void Awake()
    {
        upgradeButton = GetComponent<Button>();
    }

    private void OnEnable()
    {
        SetUIText(5);
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
        GameManager.Instance.augmentLevels[5]++;

        if (GameManager.Instance.augmentLevels[5] == 1)
        {
            this.uiGameScene.UnlockWeapon(5);
        }
        else if (GameManager.Instance.augmentLevels[5] == 3)
        {
            AssignMaterialToChildren<KatanaMash>(1);
        }
        else if (GameManager.Instance.augmentLevels[5] == 5)
        {
            AssignMaterialToChildren<KatanaMash>(2);
        }

        GameManager.Instance.swordUpgradeDamage = GameManager.Instance.swordUpgradeDamage + 40;

        GameManager.Instance.isUpgrading = false;
    }
}
