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

    // UI 설명 스크립트
    public void SetUIText(int augmentState)
    {
        TextMeshProUGUI[] textComponents = GetComponentsInChildren<TextMeshProUGUI>();
        //2번째 TMP에 레벨을 표시하기 위해서 TMP배열을 받아오고 2번째에 있는 TMP에 표시함.
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
    /// 특정 스크립트를 가진 오브젝트를 찾아 자식의 MeshRenderer에 Material 추가 
    /// 파라미터는 업그레이드 단계 현재 2단계 까지 구현
    /// </summary>
    /// <typeparam name="T">찾을 스크립트 타입</typeparam>
    public void AssignMaterialToChildren<T>(int upgradeCount) where T : MonoBehaviour
    {
        // 현재 씬의 모든 루트 오브젝트 가져오기
        GameObject[] rootObjects = SceneManager.GetActiveScene().GetRootGameObjects();
        List<T> objectsWithScript = new List<T>();

        // 루트 오브젝트를 시작으로 하이어라키 내 모든 T 스크립트 찾기
        foreach (GameObject rootObject in rootObjects)
        {
            T[] scripts = rootObject.GetComponentsInChildren<T>(true); // 비활성화된 자식 포함
            objectsWithScript.AddRange(scripts);
        }

        // 찾은 오브젝트에 대해 Material 처리
        foreach (T script in objectsWithScript)
        {
            GameObject parentObject = script.gameObject;

            // 자식 중 MeshRenderer를 가진 오브젝트를 찾음
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
        // Material 추가 또는 교체
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
