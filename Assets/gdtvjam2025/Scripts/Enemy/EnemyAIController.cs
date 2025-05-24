using System;
using System.IO;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

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
    public int maxNavmeshSamples = 5;
    public LayerMask targetLayer;

    [Header("Targeting Settings")]
    public float sampleRangeMultiplier = 3f; // Multiplier for the sample range when searching for targets

    [Header("Attack Settings")]

    [SerializeField] private float effectiveAttackRange;
    [SerializeField] private float baseAttackRange;
    [SerializeField] private float attackRangeMultiplier;

    [Header("Transform References")]
    public Transform generalTargetPoint;
    public Transform agentBody;
    public Transform currentTarget;

    [Header("Script References")]
    public NavMeshAgent agent;
    public ObjectPool enemyPool;

    private Collider[] hitColliders;
    private Collider[] validTargets;

    private Vector3 currentDestination;

    private Health health;
    private UIHealthBar healthBar;
    private bool isAttacking;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        health = GetComponent<Health>();
        healthBar = GetComponent<UIHealthBar>();

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

        //if (currentTarget == null)
        //{
        //    MoveTowardsTarget(generalTargetPoint.position);
        //}
        //else
        //{
        //    MoveTowardsTarget(currentTarget.position);
        //}

        if (currentTarget == null)
        {
            // If no specific target, move towards the general objective (e.g., HQ)
            MoveTowardsTarget(generalTargetPoint.position);
        }
        else
        {
            // Calculate effective attack range dynamically
            CalculateEffectiveAttackRange();

            //float distanceToTarget = Vector3.Distance(transform.position, currentTarget.position);
            float distanceToTarget = Vector3.Distance(transform.position, currentDestination);

            if (distanceToTarget <= effectiveAttackRange)
            {
                // We are within attack range and have stopped moving
                if (!isAttacking)
                {
                    StartAttack();
                }
                RotateTowardsTarget(currentTarget.position); // Ensure agent faces the target
            }
            else
            {
                // We need to move closer to the target's attack position
                StopAttack(); // Stop any ongoing attack
                MoveToAttackPosition(currentTarget); // Move towards a reachable attack point
            }
        }
    }

    #region Attacking
     /// <summary>
    /// Calculates a valid attack position on the NavMesh near the target and moves the agent there.
    /// </summary>
    private void MoveToAttackPosition(Transform target)
    {
        if (target == null)
        {
            agent.isStopped = true;
            return;
        }

        // Calculate a point 'effectiveAttackRange - offset' away from the target, in the direction from agent to target
        // The offset ensures the agent tries to stop slightly inside the attack range, accounting for stoppingDistance.
        Vector3 directionToTarget = (target.position - transform.position).normalized;
        // Ensure effectiveAttackRange is always positive and greater than stoppingDistance
        float distanceToStop = Mathf.Max(0.1f, effectiveAttackRange - (agent.stoppingDistance / 2f));
        Vector3 desiredAttackPosition = target.position - directionToTarget * distanceToStop;

        NavMeshHit hit;
        // Sample a position on the NavMesh near the desired attack point
        // The maxDistance for SamplePosition should be large enough to find a point around the obstacle
        if (NavMesh.SamplePosition(desiredAttackPosition, out hit, effectiveAttackRange * 2f, NavMesh.AllAreas))
        {
            // Only set destination if it's new or the current path is invalid
            if (currentDestination != hit.position || agent.pathStatus == NavMeshPathStatus.PathInvalid)
            {
                agent.SetDestination(hit.position);
                currentDestination = hit.position;
            }
            agent.isStopped = false; // Ensure agent can move
            RotateTowardsVelocity(); // Rotate while moving
        }
        else
        {
            // Could not find a reachable point on the NavMesh near the tower.
            Debug.LogWarning($"Could not find a reachable attack point on NavMesh near {target.name}. Agent cannot reach attack position.");
            agent.isStopped = true; // Stop the agent if it can't find a path
        }
    }

    private void StartAttack()
    {
        Debug.Log("Attacking");
    }

    /// <summary>
    /// Stops the attack sequence.
    /// </summary>
    void StopAttack()
    {
        if (isAttacking)
        {
            isAttacking = false;
            // Only re-enable movement if there's a target to move towards
            if (currentTarget != null)
            {
                agent.isStopped = false;
            }
            Debug.Log($"Agent {gameObject.name} stopped attacking {currentTarget.name}.");
            // TODO: Stop any ongoing attack effects
            // Example: CancelInvoke("DealDamage");
        }
    }

    /// <summary>
    /// Calculates the effective attack range based on the target's NavMeshObstacle size.
    /// </summary>
    private void CalculateEffectiveAttackRange()
    {
        NavMeshObstacle obstacle = currentTarget.GetComponent<NavMeshObstacle>();
        if (obstacle != null)
        {
            float obstacleSize = 0f;
            if (obstacle.shape == NavMeshObstacleShape.Box)
            {
                // For a box, take the largest dimension (XZ plane) as a proxy for its "width"
                obstacleSize = Mathf.Max(obstacle.size.x, obstacle.size.z);
            }
            else if (obstacle.shape == NavMeshObstacleShape.Capsule)
            {
                // For a capsule, use its radius
                obstacleSize = obstacle.radius * 2; // Diameter
            }

            // Combine base attack range with obstacle size
            effectiveAttackRange = baseAttackRange + (obstacleSize * attackRangeMultiplier);
        }
        else
        {
            // If the target doesn't have a NavMeshObstacle, use the base attack range
            effectiveAttackRange = baseAttackRange;
        }
    }

    private float PathLength(NavMeshPath path)
    {
        if (path.corners.Length < 2)
            return 0;

        float lengthSoFar = 0.0F;
        for (int i = 1; i < path.corners.Length; i++)
        {
            lengthSoFar += Vector3.Distance(path.corners[i - 1], path.corners[i]);
        }
        return lengthSoFar;
    }

    #endregion

    #region pool related
    public void OnObjectGet()
    {
        health.ResetHealth();
        healthBar.UpdateHealthbar();
        generalTargetPoint = GameObject.FindGameObjectWithTag("HQ").transform;
    }

    public void OnObjectReturned()
    {
        health.ResetHealth();
        healthBar.UpdateHealthbar();
        generalTargetPoint = GameObject.FindGameObjectWithTag("HQ").transform;
    }
    #endregion

    // To be called by health script when the enemy dies
    public void EnemyDie()
    {
        enemyPool.ReturnObject(gameObject);
    }


    public void SetTarget(Transform target)
    {
        Vector3 targetPoint;


        if (NavMesh.SamplePosition(target.position, out NavMeshHit hit, agent.height*2f, NavMesh.AllAreas))
        {
            targetPoint = hit.position;

            agent.SetDestination(targetPoint);
        }
    }

    //public void ScanForTargets()
    //{
    //    if (currentTarget != null) return;

    //    Array.Clear(validTargets, 0, validTargets.Length);
    //    Array.Clear(hitColliders, 0, hitColliders.Length);


    //    int numColliders = Physics.OverlapSphereNonAlloc(agentBody.position, scanRadius, hitColliders, targetLayer);

    //    float closestDistance = Mathf.Infinity;

    //    Transform closestTarget = null;

    //    for (int i = 0; i < numColliders; i++)
    //    {
    //        if (HasValidPathToTarget(hitColliders[i].transform.position))
    //        {
    //            validTargets[i] = hitColliders[i];
    //        }
    //        else
    //        {
    //            Debug.Log("No valid path to target. Path status: " + hitColliders[i].name + ", " + agent.pathStatus);
    //        }
    //    }

    //    if (validTargets.Length == 0)
    //    {
    //        Debug.Log("No valid targets found.");
    //        return;
    //    }

    //    for (int i = 0; i < validTargets.Length; i++)
    //    {
    //        if (validTargets[i] == null) continue;

    //        float distance = Vector3.Distance(validTargets[i].transform.position, agentBody.position);

    //        if (distance < closestDistance)
    //        {
    //            // TODO: Prioritize structures over walls. Gatherer > Tower > Wall
    //            closestTarget = validTargets[i].transform;
    //            closestDistance = distance;
    //        }
    //    }

    //    currentTarget = closestTarget;
    //}

    public void ScanForTargets()
    {
        if (currentTarget != null) return;

        Array.Clear(validTargets, 0, validTargets.Length);
        Array.Clear(hitColliders, 0, hitColliders.Length);

        int numColliders = Physics.OverlapSphereNonAlloc(agentBody.position, scanRadius, hitColliders, targetLayer);

        Transform closestTarget = null;
        float closestDistance = Mathf.Infinity;

        for (int i = 0; i < numColliders; i++)
        {
            if (hitColliders[i] == null) continue; // Skip null entries

            float distance = Vector3.Distance(hitColliders[i].transform.position, agentBody.position);

            // TODO: Implement your prioritization logic (Gatherer > Tower > Wall) here if needed.
            // For now, it simply finds the closest one.
            if (distance < closestDistance)
            {
                closestTarget = hitColliders[i].transform;
                closestDistance = distance;
            }
        }

        if (closestTarget != null)
        {
            currentTarget = closestTarget;
            Debug.Log($"New target found: {currentTarget.name}");
        }
        else
        {
            Debug.Log("No specific targets found. Moving towards general objective.");
        }
    }

    private bool HasValidPathToTarget(Vector3 targetPosition)
    {

        //NavMesh.Raycast(agentBody.position, targetPosition, out NavMeshHit rayHit, NavMesh.AllAreas);

        //if (NavMesh.SamplePosition(targetPosition, out NavMeshHit hit, agent.radius * 2f, NavMesh.AllAreas))
        //{
        //    targetPoint = hit.position;
        //}

        NavMeshPath path = new NavMeshPath();

        agent.CalculatePath(targetPosition, path);

        if (path.status == NavMeshPathStatus.PathInvalid)
        {
            Debug.Log("No valid path to target. Path status: " + path.status, this);
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

    //private void MoveTowardsTarget(Vector3 position)
    //{
    //    if (!HasValidPathToTarget(position))
    //    {
    //        StopMoving();
    //        return;
    //    }

    //    RotateTowardsVelocity();

    //    if (currentDestination == position) return;

    //    agent.SetDestination(position);
    //}

    /// <summary>
    /// Attempts to set the agent's destination, trying to find a valid NavMesh point.
    /// This is a general helper for movement.
    /// </summary>
    private void MoveTowardsTarget(Vector3 targetWorldPosition)
    {
        NavMeshHit hit;
        // Try to sample a valid point on the NavMesh near the target world position
        // A radius of agent.height * 2f is a reasonable default for general movement
        if (NavMesh.SamplePosition(targetWorldPosition, out hit, agent.height * sampleRangeMultiplier, NavMesh.AllAreas))
        {
            if (currentDestination != hit.position) // Only set destination if it's different
            {
                agent.SetDestination(hit.position);
                currentDestination = hit.position; // Store the actual NavMesh destination
            }
            agent.isStopped = false; // Ensure agent can move
            RotateTowardsVelocity(); // Rotate while moving
        }
        else
        {
            // Could not find a valid point on the NavMesh near the target position
            Debug.LogWarning($"Could not find a valid point on NavMesh near {targetWorldPosition}. Agent cannot move.");
        }
    }

    private void RotateTowardsVelocity()
    {
        if (agent.velocity.magnitude > 0.1f)
        {
            // If the agent is not moving, do not rotate
            return;
        }

        agentBody.LookAt(agent.velocity.normalized, Vector3.up);
    }

    /// <summary>
    /// Rotates the agent's body directly towards the target's position.
    /// Used when the agent is stopped and attacking.
    /// </summary>
    private void RotateTowardsTarget(Vector3 targetPosition)
    {
        Vector3 lookDirection = targetPosition - agentBody.position;
        lookDirection.y = 0; // Keep rotation horizontal
        if (lookDirection == Vector3.zero) return; // Avoid NaN if directly on target

        Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
        agentBody.rotation = Quaternion.Slerp(agentBody.rotation, targetRotation, 5f * Time.deltaTime);
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
        //if (showDetectionRadius)
        //{
        //    Gizmos.color = Color.orangeRed;
        //    Gizmos.DrawWireSphere(agentBody.position, scanRadius);
        //}
        //if (showTargetDirection && currentTarget != null)
        //{
        //    Gizmos.color = Color.greenYellow;
        //    Gizmos.DrawLine(agentBody.position, currentTarget.position);
        //}

        if (showDetectionRadius && agentBody != null)
        {
            Gizmos.color = Color.red; // Changed to red for clarity
            Gizmos.DrawWireSphere(agentBody.position, scanRadius);
        }
        if (showTargetDirection && currentTarget != null && agentBody != null)
        {
            Gizmos.color = Color.cyan; // Changed to cyan for clarity
            Gizmos.DrawLine(agentBody.position, currentTarget.position);

            // Draw the effective attack range as a circle around the target
            if (Application.isPlaying) // Only draw this in play mode as effectiveAttackRange is calculated then
            {
                Gizmos.color = Color.magenta;
                Gizmos.DrawWireSphere(currentTarget.position, effectiveAttackRange);
            }
        }

        // Draw the current NavMeshAgent path
        if (agent != null && agent.hasPath)
        {
            Gizmos.color = Color.blue;
            Vector3[] corners = agent.path.corners;
            for (int i = 0; i < corners.Length - 1; i++)
            {
                Gizmos.DrawLine(corners[i], corners[i + 1]);
            }
        }

        // Draw the calculated destination point (useful for debugging where MoveToAttackPosition aims)
        if (Application.isPlaying && currentDestination != Vector3.zero)
        {
            Gizmos.color = Color.yellow; // Changed to yellow for visibility
            Gizmos.DrawSphere(currentDestination, 0.3f);
        }
    }

}
