using UnityEngine;
using UnityEngine.AI;

public class RandoomNPC : MonoBehaviour
{
    private NavMeshAgent agent;
    public WaypointNode[] waypoint;

    private int currentWaypoint = 0;
    private float waitCounter = 0f;
    private bool isWaiting = false;
    private bool movingForward = true;

    public Animator animator;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        // Pastikan berada di navmesh
        NavMeshHit hit;
        if (NavMesh.SamplePosition(transform.position, out hit, 2f, NavMesh.AllAreas))
            agent.Warp(hit.position);

        if (waypoint.Length > 0)
            agent.SetDestination(waypoint[currentWaypoint].transform.position);
    }

    void Update()
    {
        if (!agent.isOnNavMesh) return;

        // Kalau sedang menunggu, jangan pindah
        if (isWaiting)
        {
            waitCounter += Time.deltaTime;
            if (waitCounter >= waypoint[currentWaypoint].stopDuration)
            {
                isWaiting = false;
                GoToNextWaypoint();
            }
            animator.SetFloat("Speed", 0);
            return;
        }

        // Sudah tiba di titik
        if (!agent.pathPending && agent.remainingDistance <= 0.15f)
        {
            WaypointNode node = waypoint[currentWaypoint];

            // Hanya berhenti kalau stopHere = true
            if (node.stopHere)
            {
                isWaiting = true;
                waitCounter = 0;
            }
            else
            {
                GoToNextWaypoint();
            }
        }

        animator.SetFloat("Speed", agent.velocity.magnitude);
    }

    void GoToNextWaypoint()
    {
        if (movingForward)
        {
            currentWaypoint++;
            if (currentWaypoint >= waypoint.Length)
            {
                currentWaypoint = waypoint.Length - 2;
                movingForward = false;
            }
        }
        else
        {
            currentWaypoint--;
            if (currentWaypoint < 0)
            {
                currentWaypoint = 1;
                movingForward = true;
            }
        }

        agent.SetDestination(waypoint[currentWaypoint].transform.position);
    }
}
