using UnityEngine;
using UnityEngine.AI;

public class RandoomNPC : MonoBehaviour
{
    private NavMeshAgent agent;
    public Transform[] waypoint;
    public float waitTime = 2f;

    private int currentWaypoint = 0;
    private float waitCounter = 0f;
    private bool movingForward = true;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        // Pastikan agent berada di NavMesh
        NavMeshHit hit;
        if (NavMesh.SamplePosition(transform.position, out hit, 2f, NavMesh.AllAreas))
        {
            agent.Warp(hit.position);
        }

        if (waypoint.Length > 0)
            agent.SetDestination(waypoint[currentWaypoint].position);
    }

    void Update()
    {
        // Cek apakah agent benar2 berada di navmesh
        if (!agent.isOnNavMesh) return;

        // Cek sampai tujuan
        if (!agent.pathPending && agent.remainingDistance <= 0.2f)
        {
            waitCounter += Time.deltaTime;

            if (waitCounter >= waitTime)
            {
                waitCounter = 0;
                GoToNextWaypoint();
            }
        }
    }

    void GoToNextWaypoint()
    {
        if (movingForward)
        {
            currentWaypoint++;
            if (currentWaypoint >= waypoint.Length)
            {
                currentWaypoint = waypoint.Length - 2;
                movingForward = false; // mulai mundur
            }
        }
        else
        {
            currentWaypoint--;
            if (currentWaypoint < 0)
            {
                currentWaypoint = 1;
                movingForward = true; // mulai maju lagi
            }
        }

        agent.SetDestination(waypoint[currentWaypoint].position);
    }
}
