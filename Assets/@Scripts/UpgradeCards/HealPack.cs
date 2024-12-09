using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HealPack : UpgradeCard
{
    Button upgradeButton;
    int healValue;
    // Start is called before the first frame update
    void Awake()
    {
        upgradeButton = GetComponent<Button>();
    }

    private void OnEnable()
    {
        // 설명 UI
        healValue = Random.Range(1, 11) * 10;
        GetComponentInChildren<TextMeshProUGUI>().text = $"Heal: {healValue}";

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

    // 카드 효과
    void UpgradeEffect()
    {
        GameManager.Instance.playerHP = GameManager.Instance.playerHP + healValue;

        if(GameManager.Instance.playerHP > GameManager.Instance.playerMaxHP)
        {
            GameManager.Instance.playerHP = GameManager.Instance.playerMaxHP;
        }

        GameManager.Instance.isUpgrading = false;
    }
}

