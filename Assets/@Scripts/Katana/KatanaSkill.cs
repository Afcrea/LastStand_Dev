using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine.XR;

public class KatanaSkill : MonoBehaviour
{
    public InputActionReference leftTriggerAction;  // InputReference ���
    // public InputActionReference rightTriggerAction;

    public Transform spawnPoint; // ���� ������Ʈ ���� ��ġ
    public GameObject objectToSpawn;   // ������ ������Ʈ
    private GameObject objectToSpawnInstance; // ������ ������Ʈ�� �ν��Ͻ�
    private ParticleSystem bloodEffect; // �� �� ������ ��
    public AudioClip slashSound;
    public AudioClip slashSkillSound;
    public AudioClip swordSwing;
    private int katanaDamage = 10;
    public float speedThreshold = 10f;  // �ӵ� �Ӱ谪

    private Vector3 leftPrevPosition;  // ���� ��Ʈ�ѷ��� ���� ��ġ
    private float leftSpeed;             // ���� ��Ʈ�ѷ��� �ӵ�

    private void Awake()
    {
        // KatanaSkill �ڽ��� Blood_Headshot
        bloodEffect = transform.Find("Blood_Headshot").GetComponentInChildren<ParticleSystem>();
    }

    void OnEnable()
    {
        // Ʈ���� �׼ǿ� �̺�Ʈ �ڵ鷯 �߰�
        if (leftTriggerAction != null)
        {
            leftTriggerAction.action.Enable();

            leftTriggerAction.action.performed += OnTriggerPressed;
        }

        // �ʱ� ��ġ ����
        leftPrevPosition = transform.position;
    }

    private void OnDisable()
    {
        // Ʈ���� �׼ǿ��� �̺�Ʈ �ڵ鷯 ����
        if (leftTriggerAction != null)
        {
            leftTriggerAction.action.performed -= OnTriggerPressed;

            leftTriggerAction.action.Disable();
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
        Vector3 leftCurrentPosition = transform.position; // ���÷� transform ���, �����δ� ���� ��Ʈ�ѷ��� ��ġ ���
        float leftDistance = Vector3.Distance(leftPrevPosition, leftCurrentPosition);
        leftSpeed = leftDistance / Time.deltaTime;
        // ���� ��ġ ������Ʈ
        leftPrevPosition = leftCurrentPosition;

        // ����� �ֵѷ��� ���� �ӵ��� �Ѵ´ٸ� �Ҹ� ���
        if (leftSpeed > speedThreshold)
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

            dir.y = 0f ;
            
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
            elapsedTime += Time.unscaledDeltaTime; ;  // ��� �ð� ������Ʈ
            yield return new WaitForSecondsRealtime(1 / 60);
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
        if (leftSpeed > speedThreshold)
        {
            SpawnObjectInDirection();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // 1. ���� ��Ʈ�ѷ��� �ӵ����� �Ӱ谪�� �Ѵ´ٸ�
        // 2. īŸ���� ������ �ݶ��̴� �浹�� �Ͼ��
        if (leftSpeed > speedThreshold)
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
