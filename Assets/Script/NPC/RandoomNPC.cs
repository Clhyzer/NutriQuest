using UnityEngine;
using UnityEngine.AI;

public class RandoomNPC : MonoBehaviour
{
    private NavMeshAgent agent;

    [Header("Waypoints (urutan penting)")]
    public WaypointNode[] waypoint;

    private int currentWaypoint = 0;

    [Header("Animation")]
    public Animator animator;

    private bool isBusy = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        // Pastikan NPC ada di NavMesh
        NavMeshHit hit;
        if (NavMesh.SamplePosition(transform.position, out hit, 2f, NavMesh.AllAreas))
            agent.Warp(hit.position);

        if (waypoint.Length > 0)
            agent.SetDestination(waypoint[currentWaypoint].transform.position);
    }

    void Update()
    {
        if (!agent.isOnNavMesh || isBusy) return;

        animator.SetFloat("Speed", agent.velocity.magnitude);

        if (!agent.pathPending && agent.remainingDistance <= 0.15f)
        {
            HandleWaypoint(waypoint[currentWaypoint]);
        }
    }

    void HandleWaypoint(WaypointNode node)
    {
        switch (node.type)
        {
            case WaypointType.Shop:
                StartCoroutine(ShopRoutine(node.stopDuration));
                break;

            case WaypointType.Cashier:
                StartCoroutine(CashierRoutine(node.stopDuration));
                break;

            case WaypointType.Exit:
                ExitStore();
                break;

            default:
                GoToNextWaypoint();
                break;
        }
    }

    void GoToNextWaypoint()
    {
        currentWaypoint++;

        if (currentWaypoint >= waypoint.Length)
            currentWaypoint = waypoint.Length - 1;

        agent.SetDestination(waypoint[currentWaypoint].transform.position);
    }

    System.Collections.IEnumerator ShopRoutine(float time)
    {
        isBusy = true;
        agent.isStopped = true;
        animator.SetFloat("Speed", 0);
        animator.SetTrigger("PickItem");

        yield return new WaitForSeconds(time);

        agent.isStopped = false;
        isBusy = false;
        GoToNextWaypoint();
    }

    System.Collections.IEnumerator CashierRoutine(float time)
    {
        isBusy = true;
        agent.isStopped = true;
        animator.SetFloat("Speed", 0);
        animator.SetTrigger("Pay");

        yield return new WaitForSeconds(time);

        agent.isStopped = false;
        isBusy = false;
        GoToNextWaypoint();
    }

    void ExitStore()
    {
        animator.SetFloat("Speed", 0);
        Destroy(gameObject, 1f);
    }
}
