using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
    public GameObject[] zombiePrefabToSpawn; // ��ȯ�� ������Ʈ ������
    public GameObject[] bossPrefabToSpawn; // ��ȯ�� ������Ʈ ������
    public GameObject[] bossSpecialPrefabToSpawn; // ��ȯ�� ������Ʈ ������

    //public string resourcePath = "Prefabs/YourPrefabName"; // ���ҽ� ���
    public float spawnRadius = 3f; // ��ȯ �ݰ�
    public int numberOfObjectsToSpawn = 1; // ��ȯ�� ������Ʈ ����

    Transform[] enemySpawnPos; // ��ȯ�� ��ġ, ���� ������Ʈ�� �ڽĵ�

    public GameObject enemyParent; // ��ȯ �� �θ�� ����

    bool isClear = false; // ���� ����Ŭ���� ����, ���ӸŴ������� �޾ƿ���


    private void OnEnable()
    {
        enemySpawnPos = GetAllChildren(transform).ToArray();
        enemyParent = GameObject.Find("Enemy");
        //StartZombieSpawn();
    }

    private void OnDisable()
    {
        //StopZombieSpawn();
    }

    // Update is called once per frame
    public IEnumerator SpawnObject()
    {
        while (!isClear)
        {
            foreach (Transform spawnPos in enemySpawnPos)
            {
                if (spawnPos == transform)
                {
                    yield return null;
                    continue;
                }
                int childCount = enemyParent.transform.childCount;

                if (childCount >= 30)
                {
                    yield return null;
                    continue;
                }
                List<Transform> wayPoints = GetAllChildren(spawnPos); // ���� �������� ���� ����Ʈ ������

                yield return null;

                if (wayPoints.Count <= 0) // ����� ���� ��ȯ pos ��� �ڵ�
                {
                    yield return null;
                    continue;
                }

                Vector3 spawnPosition = spawnPos.position;
                spawnPosition.y = spawnPos.position.y; // ������ ���̿��� ��ȯ

                GameObject spawnedObject = Instantiate(zombiePrefabToSpawn[Random.Range(0, zombiePrefabToSpawn.Length)], spawnPosition, Quaternion.identity); // ���� ����

                yield return null;

                Zombie zombie = spawnedObject.GetComponent<Zombie>(); // ���� ��ũ��Ʈ ������

                zombie.targets = GetAllChildren(wayPoints[Random.Range(0, wayPoints.Count)]).ToArray(); // ���� ����Ʈ ����

                yield return null;

                GameManager.Instance.SetZombieStat(zombie);

                if (enemyParent != null)
                {
                    spawnedObject.transform.parent = enemyParent.transform;
                }

                yield return new WaitForSeconds(GameManager.Instance.zombieSpawnTime);

                GameManager.Instance.zombieCount++;
            }
        }
    }

    public IEnumerator SpawnObjectInWave(int count)
    {
        for (int i = 0; i < count; i++)
        {
            Transform spawnPos = enemySpawnPos[Random.Range(0, enemySpawnPos.Length - 2)];

            if (spawnPos == transform)
            {
                yield return null;
                continue;
            }
            List<Transform> wayPoints = GetAllChildren(spawnPos); // ���� �������� ���� ����Ʈ ������

            if (wayPoints.Count <= 0) // ����� ���� ��ȯ pos ��� �ڵ�
            {
                yield return null;
                continue;
            }

            yield return null;

            Vector3 spawnPosition = spawnPos.position;
            spawnPosition.y = spawnPos.position.y; // ������ ���̿��� ��ȯ

            yield return null;

            GameObject spawnedObject = Instantiate(zombiePrefabToSpawn[Random.Range(0, zombiePrefabToSpawn.Length)], spawnPosition, Quaternion.identity); // ���� ����

            yield return null;

            Zombie zombie = spawnedObject.GetComponent<Zombie>(); // ���� ��ũ��Ʈ ������

            zombie.targets = GetAllChildren(wayPoints[Random.Range(0, wayPoints.Count)]).ToArray(); // ���� ����Ʈ ����

            GameManager.Instance.SetZombieStat(zombie);

            if (enemyParent != null)
            {
                spawnedObject.transform.parent = enemyParent.transform;
            }

            GameManager.Instance.zombieCount++;

            yield return new WaitForSeconds(GameManager.Instance.zombieSpawnTime);
        }

        GameManager.Instance.startWave = false;
    }

    public void StartWave(int count)
    {
        StartCoroutine(SpawnObjectInWave(count));
    }
    public void StopWave(int count)
    {
        StopCoroutine(SpawnObjectInWave(count));
    }

    public void StopZombieSpawn()
    {
        StopCoroutine(SpawnObject());
    }

    public void StartZombieSpawn()
    {
        StartCoroutine(SpawnObject());
    }

    public void SpawnBoss()
    {
        Transform spawnPos = enemySpawnPos[Random.Range(0, enemySpawnPos.Length - 2)];

        Vector3 spawnPosition = spawnPos.position;
        spawnPosition.y = spawnPos.position.y + 1; // ������ ���̿��� ��ȯ
        GameObject spawnedObject = Instantiate(bossPrefabToSpawn[Random.Range(0, bossPrefabToSpawn.Length)], spawnPosition, Quaternion.identity);

        GameManager.Instance.zombieCount++;

        Zombie zombie = spawnedObject.GetComponentInChildren<Zombie>(); // ���� ��ũ��Ʈ ������

        List<Transform> wayPoints = GetAllChildren(spawnPos); // ���� �������� ���� ����Ʈ ������

        zombie.targets = GetAllChildren(wayPoints[Random.Range(0, wayPoints.Count)]).ToArray(); // ���� ����Ʈ ����

        GameManager.Instance.SetZombieBossStat(zombie);

        if (enemyParent != null)
        {
            spawnedObject.transform.parent = enemyParent.transform;
        }
    }

    List<Transform> GetAllChildren(Transform parentObject)
    {
        List<Transform> children = new List<Transform>();

        foreach (Transform child in parentObject.transform)
        {
            children.Add(child);
            // �ڽ��� �ڽĵ鵵 �����ϱ� ���� ��������� ȣ���մϴ�.
            //children.AddRange(GetAllChildren(child.gameObject));
        }

        return children;
    }

    public void WizardZombieSpawn()
    {
        GameObject spawnedObject = Instantiate(bossSpecialPrefabToSpawn[0], enemySpawnPos[enemySpawnPos.Length-1].position, Quaternion.identity);

        GameManager.Instance.zombieBossCount++;
    }

    public void ZombieThrowSpawn()
    {
        GameObject spawnedObject = Instantiate(bossSpecialPrefabToSpawn[1], enemySpawnPos[enemySpawnPos.Length - 2].position, Quaternion.identity);

        GameManager.Instance.zombieBossCount++;
    }

}