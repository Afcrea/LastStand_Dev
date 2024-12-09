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
    public float duration = 5f;  // ũ�Ⱑ ������ �ð� (��)
    private Vector3 initialScale;  // ���� ũ��
    private Vector3 initialScaleCol;  // ���� ũ��
    private float targetScaleFactor = 3f;  // ��ǥ ũ�� ����
    private float startTime;  // �ִϸ��̼� ���� �ð�
    private void Start()
    {
        speed = 15f;
        Destroy(this.gameObject, 5f);
        BoxCollider col = GetComponent<BoxCollider>();
        _level = GameManager.Instance.augmentLevels[5];
        duration = 5f;
        targetScaleFactor = 3f;
        initialScale = transform.localScale;  // ���� ũ�� ����
        initialScaleCol = col.size;
        startTime = Time.time;  // ���� �ð� ���

        if(_level >= 5)
        {
            // �θ� ������Ʈ �� �ڽ� ������Ʈ�� �ִ� ��� ParticleSystem ������Ʈ�� �����ɴϴ�.
            ParticleSystem[] particleSystems = GetComponentsInChildren<ParticleSystem>();

            Color customColor = new Color(190f / 255f, 0f / 255f, 18f / 255f, 1f);  // RGBA -> 0.831, 0.173, 0, 1

            // ��� �ڽ� ��ƼŬ �ý��ۿ� ���� ��ŸƮ �÷� ����
            foreach (var ps in particleSystems)
            {
                var main = ps.main;

                // ���ϴ� �������� ��ŸƮ �÷� ���� (��: ������)
                main.startColor = customColor;
            }
        }
    }

    private void Update()
    {
        // ���� ��ǥ�迡�� �̵��Ϸ��� TransformDirection�� ����Ͽ� ���� ���� ���͸� ���� ��ǥ�� ��ȯ
        transform.position += transform.TransformDirection(Vector3.forward) * Time.deltaTime * speed;

        if (_level >= 5)
        {
            float elapsedTime = Time.time - startTime;  // ��� �ð� ���
            float scaleFactor = Mathf.Lerp(1f, targetScaleFactor, elapsedTime / duration);  // ũ�� ���� ���

            // ũ�� ����
            transform.localScale = initialScale * scaleFactor;

            if (col is BoxCollider boxCol)
            {
                boxCol.size = initialScaleCol * scaleFactor;
            }

            // 5�ʰ� ������ �������� �� �̻� �������� ����
            if (elapsedTime >= duration)
            {
                transform.localScale = initialScale * targetScaleFactor;  // ���� ũ��� ����
                col.
                enabled = false;  // Update�� �� �̻� �������� ����
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("Enemy üũ");
            Zombie zombie = other.GetComponent<Zombie>();

            if(!zombie)
            {
                return; 
            }

            zombie.HitByGun(katanaSkillDamage + GameManager.Instance.swordUpgradeDamage);
            // ��ų ���� ��� �߰�
            SoundManager.Instance.PlayOneShotNonRepeat(skillSound);
        }
    }
}
