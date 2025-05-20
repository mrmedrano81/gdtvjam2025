using System;
using System.IO;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAIController : MonoBehaviour
{
    [Header("DEBUGGING")]
    public bool showDetectionRadius = true;
    public bool showTargetDirection = true;

    [Header("Settings")]
    public float speed = 5f;
    public float scanRadius = 10f;
    public float scanInterval = 0.5f;
    private float scanTimer;
    public int maxTargets = 5;
    public LayerMask targetLayer;

    [Header("Transform References")]
    public Transform mapCenterPoint;
    public Transform agentBody;
    public Transform currentTarget;

    [Header("Script References")]
    public NavMeshAgent agent;
    public ObjectPool enemyPool;

    private Collider[] hitColliders;
    private Collider[] validTargets;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        hitColliders = new Collider[maxTargets];
        validTargets = new Collider[maxTargets];
    }

    private void Start()
    {
        agent.speed = speed;
    }

    private void Update()
    {
        RunScanTimer();

        if (currentTarget == null)
        {
            MoveTowardsTarget(mapCenterPoint.position);
        }
        else
        {
            MoveTowardsTarget(currentTarget.position);
        }
    }

    public void Initialize()
    {
        mapCenterPoint = GameObject.FindGameObjectWithTag("HQ").transform;
    }

    public void EnemyDie()
    {
        enemyPool.ReturnObject(gameObject);
    }

    public void ScanForTargets()
    {
        if (currentTarget != null) return;

        Array.Clear(validTargets, 0, validTargets.Length);
        Array.Clear(hitColliders, 0, hitColliders.Length);


        int numColliders = Physics.OverlapSphereNonAlloc(agentBody.position, scanRadius, hitColliders, targetLayer);

        float closestDistance = Mathf.Infinity;

        Transform closestTarget = null;

        for (int i = 0; i < numColliders; i++)
        {
            if (HasValidPathToTarget(hitColliders[i].transform.position))
            {
                validTargets[i] = hitColliders[i];
            }
        }

        if (validTargets.Length == 0)
        {
            //Debug.Log("No valid targets found.");
            return;
        }

        for (int i = 0; i < validTargets.Length; i++)
        {
            if (validTargets[i] == null) continue;

            float distance = Vector3.Distance(validTargets[i].transform.position, agentBody.position);

            if (distance < closestDistance)
            {
                // TODO: Prioritize structures over walls. Gatherer > Tower > Wall
                closestTarget = validTargets[i].transform;
                closestDistance = distance;
            }
        }

        currentTarget = closestTarget;
    }

    private bool HasValidPathToTarget(Vector3 targetPosition)
    {
        NavMeshPath path = new NavMeshPath();

        agent.CalculatePath(targetPosition, path);

        if (path.status != NavMeshPathStatus.PathComplete)
        {
            //Debug.Log("No valid path to target. Path status: " + agent.pathStatus, this);
            return false;
        }

        return true;
    }

    //private void MoveTowardsCenter()
    //{
    //    agent.SetDestination(mapCenterPoint.position);
    //    agentBody.LookAt(mapCenterPoint.position, Vector3.up);
    //}

    private void StopMoving()
    {
        agent.isStopped = true;
        agent.ResetPath();
    }

    private void MoveTowardsTarget(Vector3 position)
    {
        agent.SetDestination(position);
        agentBody.LookAt(position, Vector3.up);
    }


    private void RunScanTimer()
    {
        scanTimer += Time.deltaTime;

        if (scanTimer >= scanInterval)
        {
            scanTimer = 0f;
            ScanForTargets();
        }
    }

    private void OnDrawGizmos()
    {
        if (showDetectionRadius)
        {
            Gizmos.color = Color.orangeRed;
            Gizmos.DrawWireSphere(agentBody.position, scanRadius);
        }
        if (showTargetDirection && currentTarget != null)
        {
            Gizmos.color = Color.greenYellow;
            Gizmos.DrawLine(agentBody.position, currentTarget.position);
        }
    }
}
