using System;
using UnityEngine;

public class TowerAim : MonoBehaviour
{
    [Header("References")]
    public Transform pivotPoint;
    public Transform firePoint;
    public Transform currentTarget;

    [Header("Targetting Settings")]
    public bool targetNearest = true;
    public float radius = 5f;
    public bool hasInnerRadius = false;
    public float Innerradius = 2f;
    public float rotationSpeed = 10f;
    public bool offsetFirePoint = false;
    [SerializeField] private float firePointOffset = 1f;
    public LayerMask targetLayer;
    public int maxTargetsDetected;
    public float scanInterval = 1f;
    public bool useAngleAlignmentConstraint = false;
    [Range(0f, 1f)]
    public float minimumAlignment = 0.1f;
    public bool lockVerticalRotation = true;
    public bool invertDirection = false;

    [Header("Debugging")]
    public bool UpdateAimDirection = true;
    public Collider[] hitColliders;

    public Vector3 AimDirection { get; private set; }
    public Vector3 AimPosition { get; private set; }


    private float lastScanTime = 0f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        hitColliders = new Collider[maxTargetsDetected];
        currentTarget = null;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentTarget != null && TargetOutOfRange(currentTarget))
        {
            currentTarget = null;
        }

        GetTarget();

        if (UpdateAimDirection)
        {
            AimAtTarget();
        }
    }

    public bool WithinFiringAngle()
    {
        Vector3 pivotForward = Vector3.ProjectOnPlane(pivotPoint.forward, Vector3.up);

        if (invertDirection)
        {
            pivotForward = -pivotForward;
        }

        if (Vector3.Dot(pivotForward, AimDirection.normalized) > minimumAlignment)
        {
            return true;
        }

        return false;
    }

    private void GetTarget()
    {
        if (CurrentTargetExists()) return;

        Transform closestTarget = null;

        float closestDistance = Mathf.Infinity;

        int numTargets = hitColliders.Length;

        Vector3 pivotXZPosition = new Vector3(pivotPoint.position.x, 0, pivotPoint.position.z);

        if (Time.time - lastScanTime > scanInterval)
        {
            Array.Clear(hitColliders, 0, hitColliders.Length);

            Physics.OverlapSphereNonAlloc(pivotPoint.position, radius, hitColliders, targetLayer);

            lastScanTime = Time.time;
        }

        for (int i = 0; i < numTargets; i++)
        {
            if (hitColliders[i] == null)
            {
                //Debug.Log("[HEAVY] Null targets");
            }
            else
            {
                Vector3 targetXZPosition = new Vector3(hitColliders[i].transform.position.x, 0, hitColliders[i].transform.position.z);

                float dist = Vector3.Distance(targetXZPosition, pivotXZPosition);

                if (TargetOutOfRange(hitColliders[i].transform))
                {
                    //Debug.Log("[HEAVY] Targets out of range");
                    continue;
                }

                if (!targetNearest)
                {
                    closestTarget = hitColliders[i].transform;
                    return;
                }
                else if (dist < closestDistance)
                {
                    closestTarget = hitColliders[i].transform;
                    closestDistance = dist;

                    //Debug.DrawLine(pivotPoint.position, hitColliders[i].transform.position, Color.red, 1f);
                    //Debug.Log($"[HEAVY] Closest target updated {i}");
                }
            }
        }

        if (closestTarget == null)
        {
            //Debug.Log("[HEAVY] No targets found");
        }
        currentTarget = closestTarget;
    }

    public bool CurrentTargetExists()
    {
        if (currentTarget == null)
        {
            return false;
        }

        if (TargetOutOfRange(currentTarget))
        {
            return false;
        }

        return currentTarget.gameObject.activeInHierarchy;
    }

    public Vector3 GetAimDirection(Transform firePointTransform)
    {
        //if (offsetFirePoint)
        //{
        //    return currentTarget.position - (firePointTransform.position - firePoint.forward*firePointOffset);
        //}
        //else
        //{
        //    return currentTarget.position - firePointTransform.position;
        //}

        return currentTarget.position - firePointTransform.position;
    }

    private void AimAtTarget()
    {
        if (CurrentTargetExists())
        {
            if (TargetOutOfRange(currentTarget))
            {
                return;
            }

            Vector3 direction = currentTarget.position - firePoint.position;

            if (offsetFirePoint)
            {
                direction = currentTarget.position - (firePoint.position + firePoint.forward * firePointOffset);
            }

            AimDirection = direction.normalized;
            AimPosition = currentTarget.position;

            if (lockVerticalRotation)
            {
                direction = Vector3.ProjectOnPlane(direction, Vector3.up);
            }

            if (invertDirection)
            {
                direction = -direction;
            }

            Quaternion lookRotation = Quaternion.LookRotation(direction);

            pivotPoint.rotation = Quaternion.Slerp(pivotPoint.rotation, lookRotation, Time.deltaTime * rotationSpeed);
        }
    }


    //public Vector3 GetTargetPosition()
    //{
    //    if (!CurrentTargetExists())
    //    {
    //        return Vector3.zero;
    //    }

    //    return currentTarget.position;
    //}

    private bool TargetOutOfRange(Transform targetTransform)
    {
        if (targetTransform == null)
        {
            return true;
        }

        Vector3 targetXZPosition = new Vector3(targetTransform.position.x, 0, targetTransform.position.z);
        Vector3 pivotXZPosition = new Vector3(pivotPoint.position.x, 0, pivotPoint.position.z);

        float distance = Vector3.Distance(targetXZPosition, pivotXZPosition);

        if (hasInnerRadius && distance < Innerradius)
        {
            return true;
        }
        if (distance > radius)
        {
            return true;
        }

        return false;
    }

    //private void CheckTargetDistance()
    //{
    //    if (CurrentTargetExists())
    //    {
    //        if (Vector3.Distance(pivotPoint.position, currentTarget.position) > radius)
    //        {
    //            currentTarget = null;
    //        }
    //    }
    //}

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.orangeRed;
        Gizmos.DrawWireSphere(pivotPoint.position, radius);

        if (CurrentTargetExists())
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(firePoint.position, currentTarget.position);
        }

        if (hasInnerRadius)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(pivotPoint.position, Innerradius);
        }
    }
}
