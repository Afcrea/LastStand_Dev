using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public string gameSceneName = "GameScene"; // ���� �� �̸�
    private bool isLoadingComplete = false;
  
  
    void Start()
    {
        isLoadingComplete = false;
        // �񵿱� �ε� ����
        StartCoroutine(LoadGameSceneAsync());
        Debug.Log("qwe");
     
    }
  

    private IEnumerator LoadGameSceneAsync()
    {
        // ���� ���� �񵿱� �ε��ϰ� �ε��� �Ϸ�� ������ ���
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(gameSceneName);
        asyncLoad.allowSceneActivation = false; // �� �ڵ� ��ȯ�� ���� ���� false�� ����

        // �ε� �Ϸ� üũ
        while (!asyncLoad.isDone)
        {
            if (asyncLoad.progress >= 0.9f)
            {
                Debug.Log("wait");
                isLoadingComplete = true; // �ε� �Ϸ�
                break;
            }
            yield return null;
        }

        // �ε��� �Ϸ�� �� 3�� ���
        yield return new WaitForSecondsRealtime(3f);

        // �� ������ �������� �� ���� ������ ��ȯ
        if (isLoadingComplete)
        {
            Debug.Log("��");
            asyncLoad.allowSceneActivation = true;
        }
    }
}
