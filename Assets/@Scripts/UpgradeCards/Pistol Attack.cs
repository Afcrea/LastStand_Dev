using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PistolAttack : UpgradeCard
{
    Button upgradeButton;
    // Start is called before the first frame update
    void Awake()
    {
        upgradeButton = GetComponent<Button>();
    }

    private void OnEnable()
    {
        SetUIText(0);
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
        GameManager.Instance.augmentLevels[0]++;

        if (GameManager.Instance.augmentLevels[0] == 1)
        {
            this.uiGameScene.UnlockWeapon(0);
        }

        else if (GameManager.Instance.augmentLevels[0] == 3)
        {
            AssignMaterialToChildren<LeftPistol>(1);
            AssignMaterialToChildren<RightPistol>(1);
        }

        else if (GameManager.Instance.augmentLevels[0] == 5)
        {
            AssignMaterialToChildren<LeftPistol>(2);
            AssignMaterialToChildren<RightPistol>(2);
        }

        GameManager.Instance.isUpgrading = false;
    }
}
