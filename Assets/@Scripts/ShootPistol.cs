using UnityEngine;
using UnityEngine.InputSystem;

public class ShootPistol : MonoBehaviour
{
    public InputActionReference leftShootPistol; // 이 스크립트 인스턴스에 할당된 손의 입력 매핑
    public InputActionReference rightShootPistol; // 이 스크립트 인스턴스에 할당된 손의 입력 매핑
    public AudioClip pistolSound;
    private Transform firePos; // 레이가 나가는 위치
    private ParticleSystem pistolMuzzleFlash; // 권총 머즐 플래쉬
    private float pistolDistance = 500f; // 레이 사정거리

    private void Awake()
    {
        leftShootPistol.action.performed += context => Fire();
        rightShootPistol.action.performed += context => Fire();

        firePos = transform.Find("FirePos");
        pistolMuzzleFlash = GetComponentInChildren<ParticleSystem>();
    }

    private void Fire()
    {
        Debug.Log($"{gameObject.name} 피스톨 사격");
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
