using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KatanaSkillDamage : MonoBehaviour
{
    BoxCollider col;
    private int katanaSkillDamage = 30;
    public AudioClip skillSound;
    public float speed = 10f;
    int _level;
    public float duration = 5f;  // 크기가 증가할 시간 (초)
    private Vector3 initialScale;  // 원래 크기
    private Vector3 initialScaleCol;  // 원래 크기
    private float targetScaleFactor = 3f;  // 목표 크기 배율
    private float startTime;  // 애니메이션 시작 시간
    private void Start()
    {
        speed = 15f;
        Destroy(this.gameObject, 5f);
        BoxCollider col = GetComponent<BoxCollider>();
        _level = GameManager.Instance.augmentLevels[5];
        duration = 5f;
        targetScaleFactor = 3f;
        initialScale = transform.localScale;  // 시작 크기 저장
        initialScaleCol = col.size;
        startTime = Time.time;  // 시작 시간 기록

        if(_level >= 5)
        {
            // 부모 오브젝트 및 자식 오브젝트에 있는 모든 ParticleSystem 컴포넌트를 가져옵니다.
            ParticleSystem[] particleSystems = GetComponentsInChildren<ParticleSystem>();

            Color customColor = new Color(190f / 255f, 0f / 255f, 18f / 255f, 1f);  // RGBA -> 0.831, 0.173, 0, 1

            // 모든 자식 파티클 시스템에 대해 스타트 컬러 변경
            foreach (var ps in particleSystems)
            {
                var main = ps.main;

                // 원하는 색상으로 스타트 컬러 설정 (예: 빨간색)
                main.startColor = customColor;
            }
        }
    }

    private void Update()
    {
        // 로컬 좌표계에서 이동하려면 TransformDirection을 사용하여 로컬 방향 벡터를 월드 좌표로 변환
        transform.position += transform.TransformDirection(Vector3.forward) * Time.deltaTime * speed;

        if (_level >= 5)
        {
            float elapsedTime = Time.time - startTime;  // 경과 시간 계산
            float scaleFactor = Mathf.Lerp(1f, targetScaleFactor, elapsedTime / duration);  // 크기 비율 계산

            // 크기 증가
            transform.localScale = initialScale * scaleFactor;

            if (col is BoxCollider boxCol)
            {
                boxCol.size = initialScaleCol * scaleFactor;
            }

            // 5초가 지나면 스케일을 더 이상 변경하지 않음
            if (elapsedTime >= duration)
            {
                transform.localScale = initialScale * targetScaleFactor;  // 최종 크기로 설정
                col.
                enabled = false;  // Update를 더 이상 실행하지 않음
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("Enemy 체크");
            Zombie zombie = other.GetComponent<Zombie>();

            if(!zombie)
            {
                return; 
            }

            zombie.HitByGun(katanaSkillDamage + GameManager.Instance.swordUpgradeDamage);
            // 스킬 사운드 재생 추가
            SoundManager.Instance.PlayOneShotNonRepeat(skillSound);
        }
    }
}
