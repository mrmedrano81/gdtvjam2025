using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.LowLevelPhysics;
using static UnityEngine.GraphicsBuffer;

public class EnemyAIController : MonoBehaviour
{
    [Header("DEBUGGING")]
    public bool showDetectionRadius = true;
    public bool showTargetDirection = true;
    public bool isStopped = false;
    public bool isAttacking;
    public NavMeshPathStatus pathStatus;

    [Header("Settings")]
    public float speed = 5f;
    public float attackSpeed = 1f; // Attack speed in seconds

    [Header("Targeting Settings")]
    public float sampleRangeMultiplier = 3f; // Multiplier for the sample range when searching for targets
    public float scanRadius = 10f;
    public float scanInterval = 0.5f;
    private float scanTimer;
    public int maxTargets = 10;
    public int maxNavmeshSamples = 5;
    public LayerMask targetLayer;
    public LayerMask wallLayer;

    [Header("Attack Settings")]

    [SerializeField] private float effectiveAttackRangeOuterBound;
    [SerializeField] private float effectiveAttackRangeInnerBound;
    [SerializeField] private float baseAttackRange;
    [SerializeField] private float attackRangeMultiplier;

    [Header("Transform References")]
    public Transform generalTargetPoint;
    public Transform agentBody;
    public Transform currentTarget;

    [Header("Script References")]
    public NavMeshAgent agent;
    public ObjectPool enemyGruntPool;
    public ObjectPool enemyGruntDeadPool;

    private Collider[] hitColliders;
    private Collider[] validTargets;

    private Vector3 currentDestination;

    private Health health;
    private UIHealthBar healthBar;

    private float lastAttackTime;

    private Animator animator;
    private bool isDead = false;

    private readonly string ATTACK_ANIM = "Attack";
    private readonly string MOVE_ANIM = "Move";
    private readonly string IDLE_ANIM = "Idle";
    private readonly string DIE_ANIM = "Die";

    private bool includeWallsInTargets = false;
    private float lastMoveTime = 0f; // Track the last time the agent moved

    [Header("Safety Measures")]
    public float mobTimeOutDuration = 10f; // Time after which the mob will reset if no target is found

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        health = GetComponent<Health>();
        healthBar = GetComponent<UIHealthBar>();
        animator = GetComponentInChildren<Animator>();

        hitColliders = new Collider[maxTargets];
        validTargets = new Collider[maxTargets];
    }

    private void Start()
    {
        agent.speed = speed;
    }

    private void Update()
    {
        isStopped = agent.isStopped;
        pathStatus = agent.pathStatus;

        if (isDead) return; // If the enemy is dead, skip the update logic

        if (agent.isStopped && !isAttacking)
        {
            lastMoveTime += Time.deltaTime;
        }
        else
        {
            lastMoveTime = 0f; // Reset the timer if the agent is moving
        }

        if (lastMoveTime > mobTimeOutDuration)
        {
            EnemyDie();
        }

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
            //MoveTowardsTarget(generalTargetPoint.position);

            if (isAttacking)
            {
                StopAttack(); // Stop any ongoing attack if no target is available
            }

            MoveTowardsCenter(); // Move towards the center or general target point
        }
        else
        {
            // Calculate effective attack range dynamically
            CalculateEffectiveAttackRange();

            //float distanceToTarget = Vector3.Distance(transform.position, currentTarget.position);
            float distanceToTarget = Vector3.Distance(transform.position, currentDestination);

            if (distanceToTarget <= baseAttackRange)
            {
                RotateTowardsTarget(currentTarget.position); // Ensure agent faces the target

                // We are within attack range and have stopped moving

                agent.isStopped = true; // Stop the agent to prepare for attack

                StartAttack();

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
        if (isAttacking) return;

        if (target == null)
        {
            agent.isStopped = true;
            return;
        }

        // Calculate a point 'effectiveAttackRange - offset' away from the target, in the direction from agent to target
        // The offset ensures the agent tries to stop slightly inside the attack range, accounting for stoppingDistance.

        Vector3 directionToTarget = (target.position - transform.position).normalized;
        // Ensure effectiveAttackRange is always positive and greater than stoppingDistance
        float distanceToStop = Mathf.Max(agent.stoppingDistance, effectiveAttackRangeInnerBound - (agent.stoppingDistance / 2f));
        Vector3 desiredAttackPosition = target.position - directionToTarget * distanceToStop;

        NavMeshHit hit;
        // Sample a position on the NavMesh near the desired attack point
        // The maxDistance for SamplePosition should be large enough to find a point around the obstacle
        if (NavMesh.SamplePosition(desiredAttackPosition, out hit, effectiveAttackRangeOuterBound, NavMesh.AllAreas))
        {
            if (currentDestination != hit.position || agent.pathStatus == NavMeshPathStatus.PathInvalid)
            {
                Vector3 finalAttackPosition = hit.position;

                float hitPositionDistanceToTarget = Vector3.Distance(hit.position, target.position);

                if (hitPositionDistanceToTarget < effectiveAttackRangeInnerBound)
                {
                    finalAttackPosition = hit.position + (-directionToTarget) * (effectiveAttackRangeInnerBound - hitPositionDistanceToTarget) ;
                }

                agent.SetDestination(finalAttackPosition);
                currentDestination = finalAttackPosition;
            }

            agent.isStopped = false; // Ensure agent can move
            RotateTowardsVelocity(); // Rotate while moving

            animator.Play(MOVE_ANIM);
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
        if (Time.time - lastAttackTime > 1/attackSpeed)
        {
            isAttacking = true; // Set attacking state to true

            lastAttackTime = Time.time;

            Debug.Log($"Attacking at {Math.Round(lastAttackTime, 2)}");

            animator.Play(ATTACK_ANIM);
        }
    }

    /// <summary>
    /// Stops the attack sequence.
    /// </summary>
    void StopAttack()
    {
        if (isAttacking)
        {
            isAttacking = false;
            agent.isStopped = false;

            //Debug.Log($"Agent {gameObject.name} stopped attacking {currentTarget.name}.");
            // TODO: Stop any ongoing attack effects
            // Example: CancelInvoke("DealDamage");
        }
    }

    /// <summary>
    /// Calculates the effective attack range based on the target's NavMeshObstacle size.
    /// </summary>
    private void CalculateEffectiveAttackRange()
    {
        //NavMeshObstacle obstacle = currentTarget.GetComponent<NavMeshObstacle>();

        Collider collider = currentTarget.GetComponent<Collider>();

        if (collider != null)
        {
            float obstacleSize = 0f;

            //if (obstacle.shape == NavMeshObstacleShape.Box)
            if (collider.GeometryHolder.Type == GeometryType.Box)
            {
                float xOffset = currentTarget.GetComponent<BoxCollider>().size.x;
                float yOffset = currentTarget.GetComponent<BoxCollider>().size.y;

                // For a box, take the largest dimension (XZ plane) as a proxy for its "width"
                obstacleSize = Mathf.Max(xOffset, yOffset)/2f;
            }
            else if (collider.GeometryHolder.Type == GeometryType.Capsule)
            {
                float radius = currentTarget.GetComponent<CapsuleCollider>().radius;

                // For a capsule, use its radius
                obstacleSize = radius; // Diameter
            }
            else
            {
                Debug.LogWarning($"Unsupported collider shape for {currentTarget.name}. Using base attack range.");
            }

            effectiveAttackRangeInnerBound = obstacleSize;

            // Combine base attack range with obstacle size
            effectiveAttackRangeOuterBound = baseAttackRange + (obstacleSize * attackRangeMultiplier);
        }
        else
        {
            // If the target doesn't have a NavMeshObstacle, use the base attack range
            effectiveAttackRangeOuterBound = baseAttackRange;
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
        currentTarget = null; // Clear the current target when returning to the pool
        generalTargetPoint = GameObject.FindGameObjectWithTag("HQ").transform;
        isDead = false;
    }
    #endregion

    // Call this when the enemy should die
    public void EnemyDie()
    {
        isDead = true; // Set the enemy as dead to prevent further updates
        // Play the death animation (assuming "Die" is the state/clip name)
        agent.isStopped = true; // Stop the agent from moving

        enemyGruntDeadPool.SpawnObjectAt(transform.position, agentBody.rotation);

        enemyGruntPool.ReturnObject(gameObject);
    }

    public void ScanForTargets()
    {
        if (currentTarget != null) return;

        currentDestination = generalTargetPoint.position; // Reset current destination when scanning for new targets

        Array.Clear(validTargets, 0, validTargets.Length);
        Array.Clear(hitColliders, 0, hitColliders.Length);

        int numColliders = Physics.OverlapSphereNonAlloc(agentBody.position, scanRadius, hitColliders, targetLayer);

        Transform closestTarget = null;
        float closestDistance = Mathf.Infinity;

        for (int i = 0; i < numColliders; i++)
        {
            Debug.Log($"Checking collider {i}: {hitColliders[i]?.name}");

            if (hitColliders[i] == null || !HasValidPathToTarget(hitColliders[i].transform.position)) continue; // Skip null entries

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
            Array.Clear(hitColliders, 0, hitColliders.Length);

            numColliders = Physics.OverlapSphereNonAlloc(agentBody.position, scanRadius, hitColliders, wallLayer);

            closestDistance = Mathf.Infinity;

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
                Debug.Log($"Wall found: {currentTarget.name}");
            }
            else
            {
                Debug.Log("No valid targets found within scan radius.");
            }
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

        if (path.status == NavMeshPathStatus.PathComplete)
        {
            //Debug.Log("No valid path to target. Path status: " + path.status, this);
            return true;
        }

        return false;
    }

    private void MoveTowardsCenter()
    {
        NavMeshHit hit;

        if (NavMesh.SamplePosition(generalTargetPoint.position, out hit, agent.height * 2f, NavMesh.AllAreas))
        {
            if (!HasValidPathToTarget(hit.position))
            {
                Vector3 hqDirection = (generalTargetPoint.position - agentBody.position).normalized;
                agent.velocity = (hqDirection * agent.speed); // Stop the agent if no valid path
                Debug.Log("No valid path to general target. Moving towards center directly.");
            }
            else
            {
                if (currentDestination != hit.position) // Only set destination if it's different
                {
                    currentDestination = hit.position; // Store the actual NavMesh destination

                    //Debug.Log($"Agent {gameObject.name} set destination to {hit.position}. Path Status: {agent.pathStatus}");
                }

                agent.SetDestination(hit.position);
            }

            RotateTowardsVelocity(); // Rotate while moving

            animator.Play(MOVE_ANIM);
        }
    }

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

                //Debug.Log($"Agent {gameObject.name} set destination to {hit.position}. Path Status: {agent.pathStatus}");
            }
            else if (HasValidPathToTarget(currentDestination))
            {
                agent.SetDestination(currentDestination);
                //Debug.Log($"Agent {gameObject.name} already at destination {hit.position}. No need to set again.");
            }
            else
            {
                currentTarget = null; // If no valid path, clear the target
                return;
            }

            agent.isStopped = false; // Ensure agent can move

            RotateTowardsVelocity(); // Rotate while moving

            animator.Play(MOVE_ANIM);
        }
        else
        {
            // Could not find a valid point on the NavMesh near the target position
            //Debug.LogWarning($"Could not find a valid point on NavMesh near {targetWorldPosition}. Agent cannot move.");

            currentTarget = null; // Clear the target if no valid point is found
        }
    }
    private void RotateTowardsVelocity()
    {
        // If the agent is moving, rotate towards its velocity
        if (agent.velocity.magnitude > 0.1f)
        {
            Vector3 lookDirection = agent.velocity.normalized;
            lookDirection.y = 0; // Keep rotation horizontal
            if (lookDirection == Vector3.zero) return; // Avoid NaN if directly on target

            Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
            agentBody.rotation = Quaternion.Slerp(agentBody.rotation, targetRotation, 5f * Time.deltaTime);
        }
        // If not moving, and has a target, RotateTowardsTarget should handle it
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
                Gizmos.DrawWireSphere(currentTarget.position, effectiveAttackRangeOuterBound);

                Gizmos.color = Color.pink;
                Gizmos.DrawWireSphere(currentTarget.position, effectiveAttackRangeInnerBound);
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
