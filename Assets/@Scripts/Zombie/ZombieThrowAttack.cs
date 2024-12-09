using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class ZombieThrowAttack : Zombie
{
    public float initialVelocityY = 10f; // �ʱ� y�� �ӵ�
    public float gravity = -9.81f; // �߷� ���ӵ�
    public float flightDuration = 2f; // ���� �ð�

    private float elapsedTime = 0f; // ��� �ð�
    private Vector3 startPosition; // ���� ��ġ

    public Transform _player;

    public Renderer objectRenderer;
    public Color glowColor = Color.green;
    public float intensity = 100.0f; // ���⸦ �ִ��� ���̱� ���� ���� �� ����

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
            Debug.LogWarning("Player ���̾ ���� ������Ʈ�� ã�� �� �����ϴ�.");
        }
        startPosition = transform.position; // ���� ��ġ ����
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
        elapsedTime += Time.deltaTime; // ��� �ð� ������Ʈ

        // ������ �����Ŀ� ���� y�� ��ġ ���
        float t = elapsedTime / flightDuration; // 0���� 1�� ��ȭ
        float y = Mathf.Lerp(startPosition.y, _player.position.y - 4, t) + (initialVelocityY * t + 0.5f * gravity * t * t); // y�� ������

        // x, z ���� Ÿ������ ���� �̵�
        float x = Mathf.Lerp(startPosition.x, _player.position.x, t);
        float z = Mathf.Lerp(startPosition.z, _player.position.z, t);

        // ���ο� ��ġ ����
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

    public new bool CheckPlayer() // ���� �տ� �÷��̾ �ִ��� üũ
    {
        Vector3 directionToPlayer = _player.position - transform.position;
        directionToPlayer.y = 0; // Y���� ���� (���� ���̸� ������� ����)

        // ������Ʈ�� �� ����� �÷��̾� ���� ���� ���� ���
        float angle = Vector3.Angle(transform.forward, directionToPlayer);

        // ���� ������ �Ÿ� ���� �÷��̾ ������ ����
        if (angle < attackAngle && directionToPlayer.magnitude < 0.01f)
        {
            return true;
        }

        return false;
    }


}
