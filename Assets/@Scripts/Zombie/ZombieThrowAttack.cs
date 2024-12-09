using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class ZombieThrowAttack : Zombie
{
    public float initialVelocityY = 10f; // 초기 y축 속도
    public float gravity = -9.81f; // 중력 가속도
    public float flightDuration = 2f; // 비행 시간

    private float elapsedTime = 0f; // 경과 시간
    private Vector3 startPosition; // 시작 위치

    public Transform _player;

    public Renderer objectRenderer;
    public Color glowColor = Color.green;
    public float intensity = 100.0f; // 세기를 최대한 높이기 위해 높은 값 설정

    // Start is called before the first frame update
    void Start()
    {
        GameObject playerObject = FindObjectWithLayer(LayerMask.NameToLayer("Player"));
        if (playerObject != null)
        {
            _player = playerObject.transform;
        }
        else
        {
            Debug.LogWarning("Player 레이어를 가진 오브젝트를 찾을 수 없습니다.");
        }
        startPosition = transform.position; // 시작 위치 저장
        this.hp = 1;
        this.damage = 1;
        //objectRenderer = GetComponent<Renderer>();

        //// Enable Emission and set the emission color
        //objectRenderer.material.EnableKeyword("_EMISSION");
        //objectRenderer.material.SetColor("_EmissionColor", glowColor * intensity);
    }

    // Update is called once per frame
    void Update()
    {
        elapsedTime += Time.deltaTime; // 경과 시간 업데이트

        // 포물선 방정식에 따라 y축 위치 계산
        float t = elapsedTime / flightDuration; // 0에서 1로 변화
        float y = Mathf.Lerp(startPosition.y, _player.position.y - 4, t) + (initialVelocityY * t + 0.5f * gravity * t * t); // y축 포물선

        // x, z 축은 타겟으로 직선 이동
        float x = Mathf.Lerp(startPosition.x, _player.position.x, t);
        float z = Mathf.Lerp(startPosition.z, _player.position.z, t);

        // 새로운 위치 설정
        transform.position = new Vector3(x, y, z);
        transform.LookAt(_player.position);
        if(CheckPlayer())
        {
            GameManager.Instance.HitPlayer(damage, isDead);
            Destroy(gameObject);
        }

        if(this.hp <= 0)
        {
            Destroy(gameObject);
        }
    }

    public new bool CheckPlayer() // 좀비 앞에 플레이어가 있는지 체크
    {
        Vector3 directionToPlayer = _player.position - transform.position;
        directionToPlayer.y = 0; // Y축은 무시 (높이 차이를 고려하지 않음)

        // 오브젝트의 앞 방향과 플레이어 방향 간의 각도 계산
        float angle = Vector3.Angle(transform.forward, directionToPlayer);

        // 일정 각도와 거리 내에 플레이어가 있으면 공격
        if (angle < attackAngle && directionToPlayer.magnitude < 0.01f)
        {
            return true;
        }

        return false;
    }


}
