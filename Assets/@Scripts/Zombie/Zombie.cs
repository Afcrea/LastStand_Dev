using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows;
using static UnityEngine.GraphicsBuffer;

public class Zombie : MonoBehaviour
{
    public Transform player; // 플레이어

    public Transform[] targets; // 경로의 지점 배열
    private int currentWaypoint = 0;

    public Animator animator; // 캐릭터의 Animator 컴포넌트
    private Rigidbody[] ragdollBodies; // 좀비의 래그돌 리지드바디
    public GameObject Head; // 헤드샷
    public GameObject HPBar; // hp바
    protected Image HPBarImage; // hp바
    public GameObject HPBarUI; // hp바 canvas

    Rigidbody currRigidbody; // 좀비의 리지드바디

    public bool isDead = false; // 좀비 죽었는 지 체크
    public bool isAttack = false; // 좀비 공격 중인지 체크

    [Header("좀비 스탯")]
    public int hp = 1; // 좀비 체력 
    public float attackDelay = 1f; // 좀비 공격 딜레이
    public float attackAnimationDelay = 0.93f; // 좀비 애니메이션 공격 딜레이
    public int damage = 1; // Zombie Damage
    public float moveSpeed; // 이동 속도
    public int EXP = 1;
    public int score = 1;
    public int maxHp;
                            // 
    public float attackAngle = 90f; // 오브젝트의 앞 방향과의 각도 기준
    public float attackDistance = 5f; // 공격 거리

    void Start()
    {
        InitRagdoll();

        InitTarget();

        InitRigidbody();

        animator.speed = moveSpeed / 2;

        if(animator.speed < 1)
        {
            animator.speed = 1;
        }

        if(HPBar != null)
        {
            HPBarImage = HPBar.GetComponent<Image>();
        }

        maxHp = hp;
    }

    void Update()
    {
        ZombieAction();

        if (HPBarImage != null)
        {
            HPBarImage.fillAmount = (float)hp / (float)maxHp;
        }
    }

    #region Init 

    void InitRagdoll() // 랙돌 초기화
    {
        animator = GetComponent<Animator>();
        ragdollBodies = GetComponentsInChildren<Rigidbody>();
        SetRagdollState(false);
    }

    public void InitTarget() // 플레이어 찾기
    {
        // "Player" 레이어를 가진 오브젝트를 찾습니다.
        GameObject playerObject = FindObjectWithLayer(LayerMask.NameToLayer("Player"));
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogWarning("Player 레이어를 가진 오브젝트를 찾을 수 없습니다.");
        }
    }

    void InitRigidbody()
    {
        currRigidbody = GetComponent<Rigidbody>();
        currRigidbody.isKinematic = false;
        currRigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    public GameObject FindObjectWithLayer(int layer) // 특정 레이어 오브젝트 찾기 함수
    {
        // 모든 게임 오브젝트를 순회하며 특정 레이어를 가진 오브젝트를 찾습니다.
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            if (obj.layer == layer)
            {
                return obj;
            }
        }
        return null;
    }

    #endregion

    #region 랙돌 관련

    public void ActivateRagdoll()
    {
        // Ragdoll 활성화
        SetRagdollState(true);
    }

    private void SetRagdollState(bool state)
    {
        foreach (Rigidbody rb in ragdollBodies)
        {
            rb.isKinematic = !state;
        }
    }

    #endregion

    #region 좀비 행동 관련

    void ZombieAction()
    {
        if (isDead) return;

        if (hp <= 0)
        {
            Die();
        }
        else
        {
            if (player != null)
            {
                // 현재 위치에서 타겟 위치로 이동합니다.
                //if(CheckPlayer())
                if (CheckPlayer())
                {
                    Attack();
                }
                else
                {
                    Move();
                }
            }
        }
    }

    #region 공격 관련 #플레이어 데미지 주는거 추가 필요

    public void Attack() // 공격 
    {
        if (isAttack) return;

        isAttack = true;

        StartCoroutine(ZombieDamageToPlayer());

        animator.speed = 1f;
        
    }

    public bool CheckPlayer() // 좀비 앞에 플레이어가 있는지 체크
    {
        Vector3 directionToPlayer = player.position - transform.position;
        directionToPlayer.y = 0; // Y축은 무시 (높이 차이를 고려하지 않음)

        // 오브젝트의 앞 방향과 플레이어 방향 간의 각도 계산
        float angle = Vector3.Angle(transform.forward, directionToPlayer);

        // 일정 각도와 거리 내에 플레이어가 있으면 공격
        if (angle < attackAngle && directionToPlayer.magnitude < attackDistance)
        {
            return true;
        }

        return false;
    }

    IEnumerator ZombieDamageToPlayer()
    {
        while(!isDead)
        {
            animator.SetBool("Attack", true);

            yield return new WaitForSeconds(attackAnimationDelay); // 좀비가 공격 하는 모션까지 대기 

            GameManager.Instance.HitPlayer(damage, isDead);

            animator.SetBool("Attack", false);

            animator.applyRootMotion = false;

            yield return new WaitForSeconds(attackDelay);
        }
    }

    #endregion

    public void Move() // 타겟 방향으로 움직이기
    {
        Vector3 targetPos;

        if (currentWaypoint < targets.Length)
        {
            targetPos = targets[currentWaypoint].position;

            if (Vector3.Distance(transform.position, targets[currentWaypoint].position) < 0.5f)
            {
                currentWaypoint++;
            }
        }
        else
        {
            targetPos = player.position;
        }

        Vector3 direction = (targetPos - transform.position);
        direction.y = 0; // y축 이동을 제외하고 수평 방향만 사용

        // 이동할 수평 방향 벡터
        Vector3 horizontalMovement = direction.normalized * moveSpeed * Time.deltaTime;

        if (!animator.applyRootMotion)
        {
            currRigidbody.MovePosition(transform.position + horizontalMovement);
        }

        transform.LookAt(targetPos);
    }

    public void HitByGun(int damage)
    {
        hp = hp - damage;
    }

    public void HitByGunAtHead(int damage)
    {
        hp = hp - damage * 2;
        GameManager.Instance.score = GameManager.Instance.score + 10;
    }

    void Die() // HP 0 이하일 때 죽음 행동
    {
        isDead = true;
        animator.enabled = false;

        GameManager.Instance.GetZombieEXP(EXP, score);

        ActivateRagdoll();

        CapsuleCollider capsuleCollider = transform.GetComponent<CapsuleCollider>();
        capsuleCollider.height = 0.1f;
        capsuleCollider.center = new Vector3(0,0.1f,0);
        capsuleCollider.radius = 0.1f;
        Head.GetComponent<SphereCollider>().enabled = false;

        if(HPBar != null)
        {
            HPBar.SetActive(false);
            HPBarUI.SetActive(false);
        }

        StartCoroutine(DeadZombieDestroy());
    }

    public IEnumerator DeadZombieDestroy() // 죽은 시체 5초 뒤 삭제
    {
        GameManager.Instance.zombieCount--;

        yield return new WaitForSeconds(2f);

        //Destroy(gameObject);

        GameManager.DestroyWithCleanup(this.gameObject);
    }

    #endregion
}
