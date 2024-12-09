using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SaveRanking : MonoBehaviour
{
    private string setScoreUrl = "http://211.213.138.122:8080/setScore.php";
    public bool isUpdate = false;


    public void UploadScore(int score)
    {
        StartCoroutine(SendScore(score));
    }

    IEnumerator SendScore(int score)
    {
        WWWForm form = new WWWForm();
        form.AddField("score", score);

        using (UnityWebRequest request = UnityWebRequest.Post(setScoreUrl, form))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                isUpdate = true;
            }
            else
            {
                isUpdate = false;
            }
        }
    }
}
