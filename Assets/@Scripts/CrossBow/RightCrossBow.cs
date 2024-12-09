using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Haptics;
using UnityEngine.InputSystem.HID;
using UnityEngine.XR.Interaction.Toolkit;

public class RightCrossBow : MonoBehaviour
{
    public InputActionReference rightShoot; // ��Ʈ�ѷ�(Ʈ����)Ű ���� üũ
    public XRBaseController haptic; // ��� �� ��Ʈ�ѷ� ����
    public GameObject rightArrow;              // ȭ�� ������
    public AudioClip bowSound;
    private GameObject ArrowInstance;        // ȭ�� �������� �ν��Ͻ�
    private Transform firePos;
    public GameObject hitBloodEffect; // Enemy���� �Ѿ��� ������ �ǰ� ������ ȿ��
    public GameObject hitConcreteEffect; // ���� źȯ�� �ε����� ����� ȿ��
    public LineRenderer renderer;

    private float crossBowDistance = 100f;
    private int crossBowDamage = 1;            // ȭ�� ������

    bool isFire = false;
    private void Awake()
    {
        firePos = transform.Find("FirePos");
    }

    private void OnEnable()
    {
        if (rightShoot != null)
        {
            rightShoot.action.Enable();
            rightShoot.action.performed += OnTriggerPressed;
        }
        isFire = false;
    }

    private void OnDisable()
    {
        if (rightShoot != null)
        {
            rightShoot.action.performed -= OnTriggerPressed;
            rightShoot.action.Disable();
        }
    }

    private void OnTriggerPressed(InputAction.CallbackContext context)
    {
        Attack();
    }

    private void Fire()
    {
        if(isFire)
        {
            return;
        }

        haptic.SendHapticImpulse(1f, 0.3f);
        // ȭ�� ����
        ArrowInstance = Instantiate(rightArrow, firePos.position, firePos.rotation);
        SoundManager.Instance.PlayOneShot(bowSound);
        // ������ ȭ���� Rigidbody�� �����´�
        Rigidbody arrowRb = ArrowInstance.GetComponent<Rigidbody>();

        // ������ ȭ�쿡 ���� �߰��Ѵ�
        arrowRb.AddForce(transform.forward * 50f, ForceMode.Impulse);
        // ���� ������ ����
        StartCoroutine(wait1s());
    }

    IEnumerator wait1s()
    {
        isFire = true;

        yield return new WaitForSecondsRealtime(1f);

        isFire = false;
    }

    private IEnumerator ShotRenderer(float crossBowDistance)
    {
        renderer.startWidth = 0.003f; // ���� ���� �ʺ�
        renderer.endWidth = 0.003f;   // ���� �� �ʺ�

        // LineRenderer�� positionCount ����
        renderer.positionCount = 2;

        // ���� �������� FirePos
        renderer.SetPosition(0, firePos.position);
        // ���� �� ���� �Է����� ���� �浹 ��ġ
        renderer.SetPosition(1, firePos.position + transform.forward * crossBowDistance);
        // ���� �������� Ȱ��ȭ �Ͽ� ź�� ������ �׸�
        renderer.enabled = true;

        yield return new WaitForSecondsRealtime(0.03f);

        // ���� ������ ��Ȱ��ȭ
        renderer.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        Destroy(ArrowInstance);
    }

    void Attack()
    {
        haptic.SendHapticImpulse(1f, 0.3f);

        int CrossBowLevel = GameManager.Instance.augmentLevels[6];

        if (CrossBowLevel < 3) // ���� 1, 2
        {
            NormalFire(CrossBowLevel);
        }
        else if (CrossBowLevel < 5) // ���� 3, 4
        {
            NormalFire(CrossBowLevel);
        }
        else // ���� 5
        {
            Fire();
        }
    }

    // ���� ���� �߻� ����
    void NormalFire(int CrossBowLevel)
    {
        SoundManager.Instance.PlayOneShot(bowSound);

        RaycastHit hit;

        // ���̰� �߻��
        if (Physics.Raycast(firePos.position, firePos.forward, out hit, crossBowDistance, GameManager.Instance.layerMask))
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                Zombie zombie = GameManager.Instance.getZombieInParent(hit.collider.gameObject);

                GameObject bloodHitInstance = Instantiate(hitBloodEffect, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(bloodHitInstance, 1f);
                // ���� �༮�� HP�� 1 ��´�
                zombie.HitByGun(crossBowDamage + GameManager.Instance.bowUpgradeDamage + CrossBowLevel);
            }
            else if (hit.collider.CompareTag("Head"))
            {
                Zombie zombie = GameManager.Instance.getZombieInParent(hit.collider.gameObject);

                GameObject bloodHitInstance = Instantiate(hitBloodEffect, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(bloodHitInstance, 1f);
                // ���� �༮�� HP�� 1 ��´�
                zombie.HitByGunAtHead(crossBowDamage + GameManager.Instance.bowUpgradeDamage + CrossBowLevel);
            }
            else if (hit.collider.CompareTag("Ground"))
            {
                GameObject concreteHitInstance = Instantiate(hitConcreteEffect, hit.point, Quaternion.LookRotation(hit.normal));
            }

            StartCoroutine(ShotRenderer(crossBowDistance));
        }
    }
}
