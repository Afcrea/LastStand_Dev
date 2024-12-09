using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public string gameSceneName = "GameScene"; // 게임 씬 이름
    private bool isLoadingComplete = false;
  
  
    void Start()
    {
        isLoadingComplete = false;
        // 비동기 로딩 시작
        StartCoroutine(LoadGameSceneAsync());
        Debug.Log("qwe");
     
    }
  

    private IEnumerator LoadGameSceneAsync()
    {
        // 게임 씬을 비동기 로드하고 로딩이 완료될 때까지 대기
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(gameSceneName);
        asyncLoad.allowSceneActivation = false; // 씬 자동 전환을 막기 위해 false로 설정

        // 로딩 완료 체크
        while (!asyncLoad.isDone)
        {
            if (asyncLoad.progress >= 0.9f)
            {
                Debug.Log("wait");
                isLoadingComplete = true; // 로딩 완료
                break;
            }
            yield return null;
        }

        // 로딩이 완료된 후 3초 대기
        yield return new WaitForSecondsRealtime(3f);

        // 두 조건을 만족했을 때 게임 씬으로 전환
        if (isLoadingComplete)
        {
            Debug.Log("완");
            asyncLoad.allowSceneActivation = true;
        }
    }
}
