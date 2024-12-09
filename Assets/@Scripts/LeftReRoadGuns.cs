using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftReRoadGuns : MonoBehaviour
{
    public LeftRifle L_rifle;
    public LeftShotGun L_shotgun;
    public LeftMachineGun L_machinegun;
    public RightRifle R_rifle;
    public RightShotGun R_shotgun;
    public RightMachineGun R_machinegun;

    public AudioClip reroadShotgun;
    public AudioClip reroadRifle;
    public AudioClip reroadMashinegun;

    private void OnTriggerEnter(Collider other)
    {
        // 라이플
        if (other.CompareTag("L_ShotGun"))
        {
            SoundManager.Instance.PlayOneShot(reroadShotgun);
            L_shotgun.Reroad();
        }
        if (other.CompareTag("R_ShotGun"))
        {
            SoundManager.Instance.PlayOneShot(reroadShotgun);
            R_shotgun.Reroad();
        }

        // 샷건
        if (other.CompareTag("L_Rifle"))
        {
            SoundManager.Instance.PlayOneShot(reroadRifle);
            L_rifle.Reroad();
        }
        if (other.CompareTag("R_Rifle"))
        {
            SoundManager.Instance.PlayOneShot(reroadRifle);
            R_rifle.Reroad();
        }

        // 머신건
        if (other.CompareTag("L_MachineGun"))
        {
            SoundManager.Instance.PlayOneShot(reroadMashinegun);
            L_machinegun.Reroad();
        }
        if (other.CompareTag("R_MachineGun"))
        {
            SoundManager.Instance.PlayOneShot(reroadMashinegun);
            R_machinegun.Reroad();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A)) 
        {
            L_shotgun.Reroad();
            R_shotgun.Reroad();
            L_rifle.Reroad();
            R_rifle.Reroad();
            L_machinegun.Reroad();
            R_machinegun.Reroad();
        }
    }
}
