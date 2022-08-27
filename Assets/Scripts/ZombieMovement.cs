
using UnityEngine;
using UnityEngine.AI;

public class ZombieMovement : MonoBehaviour
{
    public bool hasArrived;
    //this will control the zombie movement speed.
    [SerializeField] private float MoveSpeed = 5f;

    //our rigidbody for movement
    private NavMeshAgent navMeshAgent;

    
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.magnitude >= navMeshAgent.destination.magnitude)
            hasArrived = true;
    }

    public void SetDestination(Vector3 destination)
    {
        navMeshAgent.SetDestination(destination);
        hasArrived = false;
    }
}
