using UnityEngine;
using UnityEngine.InputSystem;

public class ShootPistol : MonoBehaviour
{
    public InputActionReference leftShootPistol; // �� ��ũ��Ʈ �ν��Ͻ��� �Ҵ�� ���� �Է� ����
    public InputActionReference rightShootPistol; // �� ��ũ��Ʈ �ν��Ͻ��� �Ҵ�� ���� �Է� ����
    public AudioClip pistolSound;
    private Transform firePos; // ���̰� ������ ��ġ
    private ParticleSystem pistolMuzzleFlash; // ���� ���� �÷���
    private float pistolDistance = 500f; // ���� �����Ÿ�

    private void Awake()
    {
        leftShootPistol.action.performed += context => Fire();
        rightShootPistol.action.performed += context => Fire();

        firePos = transform.Find("FirePos");
        pistolMuzzleFlash = GetComponentInChildren<ParticleSystem>();
    }

    private void Fire()
    {
        Debug.Log($"{gameObject.name} �ǽ��� ���");
       // SoundManager.Instance.PlayOneShot(pistolSound);
        pistolMuzzleFlash.Play();

        RaycastHit hit;
        Debug.DrawRay(firePos.position, firePos.forward * pistolDistance, Color.yellow, 5f);

        if (Physics.Raycast(firePos.position, firePos.forward, out hit, pistolDistance))
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                hit.collider.transform.GetComponent<Zombie>().hp--;
                Debug.Log($"hit.transform.name: {hit.transform.name}");
            }
        }
    }
}
