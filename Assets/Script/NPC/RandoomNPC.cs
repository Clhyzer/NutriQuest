using UnityEngine;
using UnityEngine.AI;

public class RandoomNPC : MonoBehaviour
{
    private NavMeshAgent agent;

    [Header("Waypoints (urutan penting)")]
    public WaypointNode[] waypoint;

    private int currentWaypoint = 0;
    [Header("Avoidance")]
    public float waypointClearRadius = 0.8f; // radius to search for free spot at waypoint
    public int waypointSampleAttempts = 8; // sampling attempts to find free spot
    public LayerMask npcLayer; // which layers count as NPC blockers (leave empty to check all)
    public float agentClearRadiusMultiplier = 1.0f; // multiplier for overlap check (1 = agent.radius)

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
        {
            // randomize avoidance priority so agents avoid each other better
            agent.avoidancePriority = Random.Range(30, 70);
            agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;

            Vector3 dest = FindClearNavmeshPositionNear(waypoint[currentWaypoint].transform.position);
            agent.SetDestination(dest);
        }
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

        Vector3 dest = FindClearNavmeshPositionNear(waypoint[currentWaypoint].transform.position);
        agent.SetDestination(dest);
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

    // cari posisi di NavMesh dekat center yang bebas dari NPC lain
    Vector3 FindClearNavmeshPositionNear(Vector3 center)
    {
        NavMeshHit hit;
        // coba posisi center dulu
        if (NavMesh.SamplePosition(center, out hit, waypointClearRadius, NavMesh.AllAreas))
        {
            if (!IsPositionBlocked(hit.position))
                return hit.position;
        }

        for (int i = 0; i < waypointSampleAttempts; i++)
        {
            Vector3 sample = center + Random.insideUnitSphere * waypointClearRadius;
            sample.y = center.y;
            if (NavMesh.SamplePosition(sample, out hit, 1.0f, NavMesh.AllAreas))
            {
                if (!IsPositionBlocked(hit.position))
                    return hit.position;
            }
        }

        // fallback: kembalikan posisi terdekat di navmesh atau center
        if (NavMesh.SamplePosition(center, out hit, waypointClearRadius, NavMesh.AllAreas))
            return hit.position;
        return center;
    }

    bool IsPositionBlocked(Vector3 pos)
    {
        int mask = npcLayer.value == 0 ? ~0 : npcLayer.value;
        float checkRadius = Mathf.Max(0.1f, agent.radius * agentClearRadiusMultiplier);
        Collider[] cols = Physics.OverlapSphere(pos, checkRadius, mask, QueryTriggerInteraction.Ignore);
        foreach (var c in cols)
        {
            if (c.GetComponentInParent<RandoomNPC>() != null || c.GetComponentInParent<NavMeshAgent>() != null)
                return true;
        }
        return false;
    }
}
