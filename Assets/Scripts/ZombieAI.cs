using System.Collections;
using UnityEngine;

public class ZombieAI : MonoBehaviour
{
    [Header ("Zombie Wait Settings")]
    [SerializeField] private float minWaitTime = 2.5f;
    [SerializeField] private float maxWaitTime = 4.0f;

    [Header("Zombie Wander Settings")]
    [SerializeField] private float minDistance = -5f;
    [SerializeField] private float maxDistance = 5f ;

    [Header("Zombie Chase Settings")]
    [SerializeField] private float PlayerStartChaseDistance = 10f;
    [SerializeField] private float PlayerStopChaseDistance= 15f;

    [Header("Zombie Attack Settings")]
    [SerializeField] private float StartAttackDistance = 3f;
    [SerializeField] private float StopAttackDistance = 5f;
    [SerializeField] private float attackCooldown = 1.5f;

    // Zombies will exhaust themselves to death after several attacks
    [Header("Zombie Attack Settings")]
    [SerializeField] private int numberOfZombieAttacksUntilDeath = 5;

    public enum ZombieState
    {
        Wait,
        Wander,
        Chase,
        Attack,
        Death,
    }

    private ZombieState zombieState = ZombieState.Wait;

    private bool isWaiting = false;
    private Coroutine waitingcoroutine;

    private bool isWandering = false;
    
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
                    zombieState = ZombieState.Attack;
                }
                ZombieChase();
                break;
            case ZombieState.Attack:
                ZombieAttack();
                break;
            case ZombieState.Death:
                ZombieDeath();
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
        StopCoroutine(waitingcoroutine);
        isWaiting = false;
        isWandering = false;
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
            float currentX = transform.position.x;
            float currentZ = transform.position.z;
            float targetX = currentX + Random.Range(minDistance, maxDistance);
            float targetZ = currentZ + Random.Range(minDistance, maxDistance);
            float targetY = Terrain.activeTerrain.SampleHeight(new Vector3(targetX, 0f, targetZ));
            zombieMovement.SetDestination(new Vector3(targetX, targetY, targetZ));
        }

        if (zombieMovement.hasArrived && isWandering)
        {
            isWandering = false;
            zombieState = ZombieState.Wait;
        }
    }

    private void ZombieChase()
    {
        if (ChasePlayer())
        {
            transform.LookAt(playerTransform);
            zombieMovement.SetDestination(PlayerPosition);
        } else
        {
            zombieMovement.SetDestination(transform.position);
            zombieState = ZombieState.Wait;
        }
    }


    private void ZombieAttack()
    {
        if (canAttack)
        {
            //attack code
            Debug.Log($"Attacked the Player!, {numberOfZombieAttacksUntilDeath} attacks until zombie exhausts itself to death.");
            numberOfZombieAttacksUntilDeath--;
            if(numberOfZombieAttacksUntilDeath <= 0)
                zombieState = ZombieState.Death;
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
    
    private void ZombieDeath()
    {
        Destroy(gameObject);
    }
}

