using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows;
using static UnityEngine.GraphicsBuffer;

public class Zombie : MonoBehaviour
{
    public Transform player; // �÷��̾�

    public Transform[] targets; // ����� ���� �迭
    private int currentWaypoint = 0;

    public Animator animator; // ĳ������ Animator ������Ʈ
    private Rigidbody[] ragdollBodies; // ������ ���׵� ������ٵ�
    public GameObject Head; // ��弦
    public GameObject HPBar; // hp��
    protected Image HPBarImage; // hp��
    public GameObject HPBarUI; // hp�� canvas

    Rigidbody currRigidbody; // ������ ������ٵ�

    public bool isDead = false; // ���� �׾��� �� üũ
    public bool isAttack = false; // ���� ���� ������ üũ

    [Header("���� ����")]
    public int hp = 1; // ���� ü�� 
    public float attackDelay = 1f; // ���� ���� ������
    public float attackAnimationDelay = 0.93f; // ���� �ִϸ��̼� ���� ������
    public int damage = 1; // Zombie Damage
    public float moveSpeed; // �̵� �ӵ�
    public int EXP = 1;
    public int score = 1;
    public int maxHp;
                            // 
    public float attackAngle = 90f; // ������Ʈ�� �� ������� ���� ����
    public float attackDistance = 5f; // ���� �Ÿ�

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

    void InitRagdoll() // ���� �ʱ�ȭ
    {
        animator = GetComponent<Animator>();
        ragdollBodies = GetComponentsInChildren<Rigidbody>();
        SetRagdollState(false);
    }

    public void InitTarget() // �÷��̾� ã��
    {
        // "Player" ���̾ ���� ������Ʈ�� ã���ϴ�.
        GameObject playerObject = FindObjectWithLayer(LayerMask.NameToLayer("Player"));
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogWarning("Player ���̾ ���� ������Ʈ�� ã�� �� �����ϴ�.");
        }
    }

    void InitRigidbody()
    {
        currRigidbody = GetComponent<Rigidbody>();
        currRigidbody.isKinematic = false;
        currRigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    public GameObject FindObjectWithLayer(int layer) // Ư�� ���̾� ������Ʈ ã�� �Լ�
    {
        // ��� ���� ������Ʈ�� ��ȸ�ϸ� Ư�� ���̾ ���� ������Ʈ�� ã���ϴ�.
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

    #region ���� ����

    public void ActivateRagdoll()
    {
        // Ragdoll Ȱ��ȭ
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

    #region ���� �ൿ ����

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
                // ���� ��ġ���� Ÿ�� ��ġ�� �̵��մϴ�.
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

    #region ���� ���� #�÷��̾� ������ �ִ°� �߰� �ʿ�

    public void Attack() // ���� 
    {
        if (isAttack) return;

        isAttack = true;

        StartCoroutine(ZombieDamageToPlayer());

        animator.speed = 1f;
        
    }

    public bool CheckPlayer() // ���� �տ� �÷��̾ �ִ��� üũ
    {
        Vector3 directionToPlayer = player.position - transform.position;
        directionToPlayer.y = 0; // Y���� ���� (���� ���̸� ������� ����)

        // ������Ʈ�� �� ����� �÷��̾� ���� ���� ���� ���
        float angle = Vector3.Angle(transform.forward, directionToPlayer);

        // ���� ������ �Ÿ� ���� �÷��̾ ������ ����
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

            yield return new WaitForSeconds(attackAnimationDelay); // ���� ���� �ϴ� ��Ǳ��� ��� 

            GameManager.Instance.HitPlayer(damage, isDead);

            animator.SetBool("Attack", false);

            animator.applyRootMotion = false;

            yield return new WaitForSeconds(attackDelay);
        }
    }

    #endregion

    public void Move() // Ÿ�� �������� �����̱�
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
        direction.y = 0; // y�� �̵��� �����ϰ� ���� ���⸸ ���

        // �̵��� ���� ���� ����
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

    void Die() // HP 0 ������ �� ���� �ൿ
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

    public IEnumerator DeadZombieDestroy() // ���� ��ü 5�� �� ����
    {
        GameManager.Instance.zombieCount--;

        yield return new WaitForSeconds(2f);

        //Destroy(gameObject);

        GameManager.DestroyWithCleanup(this.gameObject);
    }

    #endregion
}
