using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UpgradeCard : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public UIGameScene uiGameScene;
    public float buttonDelay = 2f;

    Material[] upgradeMaterial = { null, null};

    bool isHover = false;

    private void Awake()
    {
        buttonDelay = 2f;
    }

    private void Start()
    {
        uiGameScene = FindAnyObjectByType<UIGameScene>().GetComponent<UIGameScene>();

        upgradeMaterial[0] = Resources.Load<Material>("Holograph_mat");

        upgradeMaterial[1] = Resources.Load<Material>("Plasma (2)");
    }

    void UpgradeEffect() {
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(!isHover)
        {   
            transform.localScale = transform.localScale * 1.5f;
        }

        isHover = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isHover)
        {
            transform.localScale = transform.localScale / 1.5f;
        }

        isHover = false;
    }

    // UI ���� ��ũ��Ʈ
    public void SetUIText(int augmentState)
    {
        TextMeshProUGUI[] textComponents = GetComponentsInChildren<TextMeshProUGUI>();
        //2��° TMP�� ������ ǥ���ϱ� ���ؼ� TMP�迭�� �޾ƿ��� 2��°�� �ִ� TMP�� ǥ����.
        if(textComponents.Length > 1)
        {
            if (GameManager.Instance.augmentLevels[augmentState] >= 1)
            {
                textComponents[1].text = $"Level : {GameManager.Instance.augmentLevels[augmentState]} > {GameManager.Instance.augmentLevels[augmentState] + 1}";
            }
            else
            {
                textComponents[1].text = $"Un Lock";
            }
        }
        
    }

    /// <summary>
    /// Ư�� ��ũ��Ʈ�� ���� ������Ʈ�� ã�� �ڽ��� MeshRenderer�� Material �߰� 
    /// �Ķ���ʹ� ���׷��̵� �ܰ� ���� 2�ܰ� ���� ����
    /// </summary>
    /// <typeparam name="T">ã�� ��ũ��Ʈ Ÿ��</typeparam>
    public void AssignMaterialToChildren<T>(int upgradeCount) where T : MonoBehaviour
    {
        // ���� ���� ��� ��Ʈ ������Ʈ ��������
        GameObject[] rootObjects = SceneManager.GetActiveScene().GetRootGameObjects();
        List<T> objectsWithScript = new List<T>();

        // ��Ʈ ������Ʈ�� �������� ���̾��Ű �� ��� T ��ũ��Ʈ ã��
        foreach (GameObject rootObject in rootObjects)
        {
            T[] scripts = rootObject.GetComponentsInChildren<T>(true); // ��Ȱ��ȭ�� �ڽ� ����
            objectsWithScript.AddRange(scripts);
        }

        // ã�� ������Ʈ�� ���� Material ó��
        foreach (T script in objectsWithScript)
        {
            GameObject parentObject = script.gameObject;

            // �ڽ� �� MeshRenderer�� ���� ������Ʈ�� ã��
            MeshRenderer[] childRenderers = parentObject.GetComponentsInChildren<MeshRenderer>();
            SkinnedMeshRenderer[] childSkinnedRenderers = parentObject.GetComponentsInChildren<SkinnedMeshRenderer>();

            foreach (MeshRenderer renderer in childRenderers)
            {
                AssignMaterial(renderer, upgradeCount);
            }

            foreach (SkinnedMeshRenderer renderer in childSkinnedRenderers)
            {
                AssignMaterial(renderer, upgradeCount);
            }
        }
    }

    private void AssignMaterial(Renderer renderer, int upgradeCount)
    {
        // Material �߰� �Ǵ� ��ü
        List<Material> materials = new List<Material>(renderer.sharedMaterials);

        if (upgradeCount == 1)
        {
            materials.Add(upgradeMaterial[upgradeCount - 1]);
        }
        else if (upgradeCount == 2)
        {
            materials[materials.Count - 1] = upgradeMaterial[upgradeCount - 1];
        }

        renderer.materials = materials.ToArray();
    }
}
