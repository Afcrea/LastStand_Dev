using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
    public GameObject[] zombiePrefabToSpawn; // 소환할 오브젝트 프리팹
    public GameObject[] bossPrefabToSpawn; // 소환할 오브젝트 프리팹
    public GameObject[] bossSpecialPrefabToSpawn; // 소환할 오브젝트 프리팹

    //public string resourcePath = "Prefabs/YourPrefabName"; // 리소스 경로
    public float spawnRadius = 3f; // 소환 반경
    public int numberOfObjectsToSpawn = 1; // 소환할 오브젝트 개수

    Transform[] enemySpawnPos; // 소환할 위치, 현재 오브젝트의 자식들

    public GameObject enemyParent; // 소환 후 부모로 지정

    bool isClear = false; // 현재 게임클리어 여부, 게임매니저에서 받아오기


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
                List<Transform> wayPoints = GetAllChildren(spawnPos); // 스폰 포지션의 웨이 포인트 가져옴

                yield return null;

                if (wayPoints.Count <= 0) // 스페셜 보스 소환 pos 방어 코드
                {
                    yield return null;
                    continue;
                }

                Vector3 spawnPosition = spawnPos.position;
                spawnPosition.y = spawnPos.position.y; // 동일한 높이에서 소환

                GameObject spawnedObject = Instantiate(zombiePrefabToSpawn[Random.Range(0, zombiePrefabToSpawn.Length)], spawnPosition, Quaternion.identity); // 좀비 생성

                yield return null;

                Zombie zombie = spawnedObject.GetComponent<Zombie>(); // 좀비 스크립트 가져옴

                zombie.targets = GetAllChildren(wayPoints[Random.Range(0, wayPoints.Count)]).ToArray(); // 웨이 포인트 설정

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
            List<Transform> wayPoints = GetAllChildren(spawnPos); // 스폰 포지션의 웨이 포인트 가져옴

            if (wayPoints.Count <= 0) // 스페셜 보스 소환 pos 방어 코드
            {
                yield return null;
                continue;
            }

            yield return null;

            Vector3 spawnPosition = spawnPos.position;
            spawnPosition.y = spawnPos.position.y; // 동일한 높이에서 소환

            yield return null;

            GameObject spawnedObject = Instantiate(zombiePrefabToSpawn[Random.Range(0, zombiePrefabToSpawn.Length)], spawnPosition, Quaternion.identity); // 좀비 생성

            yield return null;

            Zombie zombie = spawnedObject.GetComponent<Zombie>(); // 좀비 스크립트 가져옴

            zombie.targets = GetAllChildren(wayPoints[Random.Range(0, wayPoints.Count)]).ToArray(); // 웨이 포인트 설정

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
        spawnPosition.y = spawnPos.position.y + 1; // 동일한 높이에서 소환
        GameObject spawnedObject = Instantiate(bossPrefabToSpawn[Random.Range(0, bossPrefabToSpawn.Length)], spawnPosition, Quaternion.identity);

        GameManager.Instance.zombieCount++;

        Zombie zombie = spawnedObject.GetComponentInChildren<Zombie>(); // 좀비 스크립트 가져옴

        List<Transform> wayPoints = GetAllChildren(spawnPos); // 스폰 포지션의 웨이 포인트 가져옴

        zombie.targets = GetAllChildren(wayPoints[Random.Range(0, wayPoints.Count)]).ToArray(); // 웨이 포인트 설정

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
            // 자식의 자식들도 포함하기 위해 재귀적으로 호출합니다.
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