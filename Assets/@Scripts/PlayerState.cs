using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class PlayerState : MonoBehaviour
{
    public UnityEngine.UI.Slider hpSlider; //HP 슬라이더 
    public UnityEngine.UI.Slider expSlider; //Exp슬라이더
    public TextMeshProUGUI scoreText; //Score



    private void Start()
    {
        StartCoroutine(SetSlider());
    }
    IEnumerator SetSlider()
    {
        while (GameManager.Instance.isDead == false)
        {
            hpSlider.value = ((float)GameManager.Instance.playerHP / GameManager.Instance.playerMaxHP);  //HP슬라이더
            expSlider.value = (float)GameManager.Instance.playerEXP / GameManager.Instance.playerMaxEXP; //Exp슬라이더
            scoreText.text = "Score : " + GameManager.Instance.score.ToString();
            yield return new WaitForSecondsRealtime(0.1f);
        }
    }
}
