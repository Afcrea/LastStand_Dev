using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    // ȸ���� ī�޶��� Ʈ������
    private Transform shakeCamera;
    private bool shakeRotate = true;

    // �ʱ� ��ǥ�� ȸ���� ����
    private Vector3 originPos;
    private Quaternion originRot;

    private void Awake()
    {
        shakeCamera = this.transform;
    }

    private void Start()
    {
        // ī�޶��� �ʱ� �� ����
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

    //                                                          ���� �ð�,                     position,                           rotation
    public IEnumerator ShakeCamera(float duration = 0.05f, float magnitudePos = 0.03f, float magnitudeRot = 0.1f)
    {
        // ������ �ð��� ������ ����
        float passTime = 0.0f;

        // ���� �ð����� ���� ��ȸ
        while(passTime < duration)
        {
            //// �ұ�Ģ�� ��ġ�� ����
            //Vector3 shakePos = Random.insideUnitSphere;
            //// ī�޶��� ��ġ�� ����
            //shakeCamera.localPosition = shakePos * magnitudePos;

            // �ұ�Ģ�� ȸ���� ����ϴ� ���
            if (shakeRotate)
            {
                shakeRotate = false;
                // �ұ�Ģ�� ȸ������ �޸� ������ �Լ��� ����� ����
                Vector3 shakeRot = new Vector3(0, 0, Mathf.PerlinNoise(Time.time * magnitudeRot, 0.0f));
                // ī�޶��� ȸ������ ����
                shakeCamera.localRotation = Quaternion.Euler(shakeRot);
            }

            // ���� �ð��� ������Ŵ
            passTime += Time.deltaTime;

            yield return null;
        }

        // ������ ���� �� ī�޶��� �ʱ� ������ ����
        shakeCamera.localPosition = originPos;
        shakeCamera.localRotation = originRot;

        shakeRotate = true;
    }
}
