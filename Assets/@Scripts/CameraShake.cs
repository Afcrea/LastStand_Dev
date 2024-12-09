using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    // 회전할 카메라의 트랜스폼
    private Transform shakeCamera;
    private bool shakeRotate = true;

    // 초기 좌표와 회전값 저장
    private Vector3 originPos;
    private Quaternion originRot;

    private void Awake()
    {
        shakeCamera = this.transform;
    }

    private void Start()
    {
        // 카메라의 초기 값 저장
        originPos = shakeCamera.localPosition;
        originRot = shakeCamera.localRotation;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            StartCoroutine(ShakeCamera());
        }
    }

    //                                                          진동 시간,                     position,                           rotation
    public IEnumerator ShakeCamera(float duration = 0.05f, float magnitudePos = 0.03f, float magnitudeRot = 0.1f)
    {
        // 지나간 시간을 누적할 변수
        float passTime = 0.0f;

        // 진동 시간동안 루프 순회
        while(passTime < duration)
        {
            //// 불규칙한 위치를 산출
            //Vector3 shakePos = Random.insideUnitSphere;
            //// 카메라의 위치를 변경
            //shakeCamera.localPosition = shakePos * magnitudePos;

            // 불규칙한 회전을 사용하는 경우
            if (shakeRotate)
            {
                shakeRotate = false;
                // 불규칙한 회전값을 펄린 노이즈 함수를 사용해 추출
                Vector3 shakeRot = new Vector3(0, 0, Mathf.PerlinNoise(Time.time * magnitudeRot, 0.0f));
                // 카메라의 회전값을 변경
                shakeCamera.localRotation = Quaternion.Euler(shakeRot);
            }

            // 진동 시간을 누적시킴
            passTime += Time.deltaTime;

            yield return null;
        }

        // 진동이 끝난 후 카메라의 초기 값으로 설정
        shakeCamera.localPosition = originPos;
        shakeCamera.localRotation = originRot;

        shakeRotate = true;
    }
}
