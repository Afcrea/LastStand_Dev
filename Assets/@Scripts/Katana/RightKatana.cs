using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RightKatana : MonoBehaviour
{
    //public InputActionReference leftTriggerAction;  // InputReference ���
    public InputActionReference rightTriggerAction;

    public Transform spawnPoint; // ���� ������Ʈ ���� ��ġ
    public GameObject objectToSpawn;   // ������ ������Ʈ
    private GameObject objectToSpawnInstance; // ������ ������Ʈ�� �ν��Ͻ�
    private ParticleSystem bloodEffect; // �� �� ������ ��
    public AudioClip slashSound;
    public AudioClip slashSkillSound;
    public AudioClip swordSwing;
    private int katanaDamage = 10;
    public float speedThreshold = 10f;  // �ӵ� �Ӱ谪

    private Vector3 rightPrevPosition;  // ���� ��Ʈ�ѷ��� ���� ��ġ
    private float rightSpeed;             // ���� ��Ʈ�ѷ��� �ӵ�

    private void Awake()
    {
        // KatanaSkill �ڽ��� Blood_Headshot
        bloodEffect = transform.Find("Blood_Headshot").GetComponentInChildren<ParticleSystem>();
    }


    void OnEnable()
    {
        // Ʈ���� �׼ǿ� �̺�Ʈ �ڵ鷯 �߰�
        if (rightTriggerAction != null)
        {
            rightTriggerAction.action.Enable();

            rightTriggerAction.action.performed += OnTriggerPressed;
        }

        // �ʱ� ��ġ ����
        rightPrevPosition = transform.position;
    }

    private void OnDisable()
    {
        // Ʈ���� �׼ǿ��� �̺�Ʈ �ڵ鷯 ����
        if (rightTriggerAction != null)
        {
            rightTriggerAction.action.performed -= OnTriggerPressed;

            rightTriggerAction.action.Disable();
        }
    }

    private void Update()
    {
        // ��Ʈ�ѷ��� �ӵ� ��� (��ġ ���� / deltaTime)
        CalculateControllerSpeed();
    }

    private void OnTriggerPressed(InputAction.CallbackContext context)
    {
        Attack();
    }

    private void CalculateControllerSpeed()
    {
        // ���� ��Ʈ�ѷ��� ��ġ ��ȭ�� ���
        Vector3 rightCurrentPosition = transform.position; // ���÷� transform ���, �����δ� ���� ��Ʈ�ѷ��� ��ġ ���
        float rightDistance = Vector3.Distance(rightPrevPosition, rightCurrentPosition);
        rightSpeed = rightDistance / Time.deltaTime;

        // ���� ��ġ ������Ʈ
        rightPrevPosition = rightCurrentPosition;

        // ����� �ֵѷ��� ���� �ӵ��� �Ѵ´ٸ� �Ҹ� ���
        if (rightSpeed > speedThreshold)
        {
            SoundManager.Instance.PlayOneShot(swordSwing);
        }
    }

    // �˱� ����
    private void SpawnObjectInDirection()
    {
        if (objectToSpawn != null)
        {
            Vector3 dir = GameManager.Instance.cameraVR.transform.forward;

            dir.y = 0f;

            dir.Normalize();

            Quaternion rotation = Quaternion.LookRotation(dir);

            SoundManager.Instance.PlayOneShotNonRepeat(slashSkillSound);
            // ������Ʈ�� �����ϰ�, ���� ������ firePos.forward�� ���� new Quaternion(90, 0, Quaternion.identity.z, 0)
            objectToSpawnInstance = Instantiate(objectToSpawn, spawnPoint.position, rotation);
            //StartCoroutine(ForwardSlash(objectToSpawnInstance));

            //Rigidbody rb = objectToSpawnInstance.GetComponent<Rigidbody>();
            //rb.AddForce(spawnPoint.forward * 20, ForceMode.Impulse);
            //Destroy(objectToSpawnInstance, 2.0f);
        }
    }

    private IEnumerator ForwardSlash(GameObject obj)
    {
        float speed = 10f;
        float duration = 4f;  // ������ �ð� ����
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            // ������ ����
            obj.transform.position += obj.transform.forward * Time.deltaTime * speed;
            // ������ ũ�� ����
            //obj.transform.localScale *= 1.05f;
            BoxCollider col = GetComponent<BoxCollider>();
            //col.size *= 1.05f;
            elapsedTime += Time.unscaledDeltaTime;  // ��� �ð� ������Ʈ
            yield return new WaitForSecondsRealtime(1/60);  // �� ������ ���
        }

        Destroy(obj);
    }

    void Attack() // ���׷��̵� ���¿� ���� ���� ����
    {
        if (GameManager.Instance.augmentLevels[5] < 3) // 3 �������� �˱� �߻�
        {
            return;
        }

        // ���� Ʈ���ſ� ������ Ʈ������ �ӵ��� �Ӱ谪�� �ʰ��� ���
        if (rightSpeed > speedThreshold)
        {
            SpawnObjectInDirection();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // 1. ���� ��Ʈ�ѷ��� �ӵ����� �Ӱ谪�� �Ѵ´ٸ�
        // 2. īŸ���� ������ �ݶ��̴� �浹�� �Ͼ��
        if (rightSpeed > speedThreshold)
        {
            if (other.CompareTag("Enemy"))
            {
                Zombie zombie = other.GetComponent<Zombie>();

                if (!zombie)
                {
                    return;
                }

                zombie.HitByGun(katanaDamage + GameManager.Instance.swordUpgradeDamage);

                // �ݶ��̴��� ��迡�� �浹 ���������� �ִ� �Ÿ��� ������
                // �Ѹ���� �ݶ��̴��� �߽����� �ƴ϶� ���� �κ��� ���Ͻ����شٴ� �ǹ��ε�?
                bloodEffect.transform.position = other.ClosestPoint(transform.position);
                bloodEffect?.transform.SetParent(null);
                bloodEffect?.Play();
                SoundManager.Instance.PlayOneShotNonRepeat(slashSound);
            }
        }
    }
}
