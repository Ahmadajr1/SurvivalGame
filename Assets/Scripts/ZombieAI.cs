using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieAI : MonoBehaviour
{
    [Header ("Zombie Wait Settings")]
    [SerializeField] private float minWaitTime = 2.5f;
    [SerializeField] private float maxWaitTime = 4.0f;

    [Header("Zombie Wander Settings")]
    [SerializeField] private float minWanderTime = 5f;
    [SerializeField] private float maxWanderTime = 10f;
    [SerializeField] private float minRotationDistance = 30f;
    [SerializeField] private float maxRotationDistance = 330f ;

    [Header("Zombie Chase Settings")]
    [SerializeField] private float PlayerStartChaseDistance = 10f;
    [SerializeField] private float PlayerStopChaseDistance= 15f;

    [Header("Zombie Attack Settings")]
    [SerializeField] private float StartAttackDistance = 3f;
    [SerializeField] private float StopAttackDistance = 5f;
    [SerializeField] private float attackCooldown = 1.5f;

    public enum ZombieState
    {
        Wait,
        Wander,
        Chase,
        Attack
    }

    private ZombieState zombieState = ZombieState.Wait;

    private bool isWaiting = false;
    private Coroutine waitingcoroutine;

    private bool isWandering = false;
    private Coroutine wanderingcoroutine;

    private ZombieMovement zombieMovement;

    private Transform playerTransform;
    private bool isChasing = false;

    private bool canAttack = true;

    // Start is called before the first frame update
    void Start()
    {
        zombieMovement=GetComponent<ZombieMovement>();
        playerTransform = GameObject.FindWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(zombieState);
        switch (zombieState)
        {
            case ZombieState.Wait:
                ZombieWait();
                if (ChasePlayer())
                {
                    InitChase();
                }
                break;
            case ZombieState.Wander:
                ZombieWander();
                if (ChasePlayer())
                {
                    InitChase();
                }
                break;
            case ZombieState.Chase:
                if (playerDistance <= StartAttackDistance)
                {
                    zombieMovement.IsMoving = false;
                    zombieState = ZombieState.Attack;
                }
                ZombieChase();
                break;
            case ZombieState.Attack:
                ZombieAttack();
                break;
            default:
                break;
        }
    }

    private Vector3 PlayerPosition
    {
        get { return playerTransform.position; }
    }

    private float playerDistance
    {
        get { return Vector3.Distance(transform.position, PlayerPosition); }
    }

    private bool ChasePlayer()
    {
        if (isChasing)
        {
            if (playerDistance <= PlayerStopChaseDistance)
            {
                return true;
            } else
            {
                isChasing = false;
                return false;
            }
        } else
        {
            if (playerDistance <= PlayerStartChaseDistance)
            {
                isChasing = true;
                return true;
            } else
            {
                return false;
            }
        }
    }

    private void InitChase()
    {
        StopCoroutine(wanderingcoroutine);
        StopCoroutine(waitingcoroutine);
        isWaiting = false;
        isWandering = false;
        zombieMovement.IsMoving = true;
        zombieState = ZombieState.Chase;
    }

    private void ZombieWait()
    {
        if (!isWaiting)
        {
            waitingcoroutine = StartCoroutine(GoWandering());
        }
    }

    IEnumerator GoWandering()
    {
        isWaiting = true;
        yield return new WaitForSeconds(Random.Range(minWaitTime, maxWaitTime));
        zombieState = ZombieState.Wander;
        isWaiting = false;
    }

  
    private void ZombieWander()
    {
        if (!isWandering)
        {
            isWandering = true;
            transform.Rotate(0f, Random.Range(minRotationDistance, maxRotationDistance), 0f);
            wanderingcoroutine = StartCoroutine(WalkTime());
        }
    }

    IEnumerator WalkTime()
    {
        zombieMovement.IsMoving = true;
        yield return new WaitForSeconds(Random.Range(minWanderTime, maxWanderTime));
        zombieMovement.IsMoving = false;
        isWaiting = false;
        zombieState = ZombieState.Wait;
        isWandering = false;
    }

    private void ZombieChase()
    {
        if (ChasePlayer())
        {
            transform.LookAt(playerTransform);
        } else
        {
            zombieMovement.IsMoving = false;
            zombieState = ZombieState.Wait;
        }
    }


    private void ZombieAttack()
    {
        if ( canAttack)
        {
            //attack code
            Debug.Log("Attacked the Player!");
            StartCoroutine(AttackCooldown());
        } else
        {
            if ( playerDistance > StopAttackDistance)
            {
                InitChase();
            }
        }
    }

    IEnumerator AttackCooldown()
    {
        canAttack = false;
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }
}

