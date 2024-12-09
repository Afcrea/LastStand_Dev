using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;
using System.Text.RegularExpressions;
using TMPro;

public class Ranking : MonoBehaviour
{
    private string getScoreUrl = "http://211.213.138.122:8080/getScore.php";

    public GameObject[] rankingSection;

    [System.Serializable]
    public class PlayerData
    {
        //public string name;
        public int score;
    }

    [System.Serializable]
    public class PlayerDataList
    {
        public List<PlayerData> players;
    }

    public List<PlayerData> topPlayers = new List<PlayerData>();

    private string jsonResponse;

    private void OnEnable()
    {
        StartCoroutine(FetchData());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(FetchData());
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            TMP_Text childText = rankingSection[0].GetComponentInChildren<TMP_Text>();
            Debug.Log(childText.text + " / " + childText.gameObject.name);
        }
    }

    IEnumerator FetchData()
    {
        int count = 1;

        UnityWebRequest request = UnityWebRequest.Get(getScoreUrl);

        // 인증서 검증 무시 (테스트 환경에서만 사용!)
        request.certificateHandler = new BypassCertificateHandler();

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
        }
        else
        {
            jsonResponse = request.downloadHandler.text;

            List<PlayerData> players = ParseJson(jsonResponse);

            // 결과 출력
            foreach (var player in players)
            {
                rankingSection[count - 1].GetComponentInChildren<TMP_Text>().text = ($" {player.score} ");

                count++;
            }
        }
    }
    List<PlayerData> ParseJson(string json)
    {
        // 배열 형식으로 JSON을 변환
        string wrappedJson = "{\"players\":" + json + "}";
        PlayerDataList playerList = JsonUtility.FromJson<PlayerDataList>(wrappedJson);
        return playerList.players;
    }
}



public static class UnicodeDecoder
{
    public static string DecodeUnicode(string unicodeString)
    {
        return Regex.Unescape(unicodeString);
    }
}

public class BypassCertificateHandler : CertificateHandler
{
    protected override bool ValidateCertificate(byte[] certificateData)
    {
        // 모든 인증서를 신뢰 (테스트 환경에서만 사용!)
        return true;
    }
}
