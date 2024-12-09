using UnityEngine;
using UnityEngine.InputSystem;

public class Katana : MonoBehaviour
{
    int katanaDamage = 1;

    public AudioClip cutZombieSound;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Zombie zombie = other.GetComponent<Zombie>();

            if (!zombie)
            {
                return;
            }

            zombie.HitByGun(katanaDamage + GameManager.Instance.swordUpgradeDamage);
            SoundManager.Instance.PlayOneShot(cutZombieSound);
        }
    }
}
