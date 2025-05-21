using System;
using UnityEngine;

public class TowerAim : MonoBehaviour
{
    [Header("References")]
    public Transform pivotPoint;
    public Transform firePoint;
    public Transform currentTarget;

    [Header("Settings")]
    public float radius = 5f;
    public float rotationSpeed = 10f;
    [SerializeField] private float firePointOffset = 1f;
    public LayerMask targetLayer;
    public int maxTargetsDetected;
    public float scanInterval = 1f;
    public bool offsetFirePoint = false;
    public bool lockVerticalRotation = true;
    public bool invertDirection = false;

    public Collider[] hitColliders;

    public Vector3 AimDirection { get; private set; }

    public bool UpdateAimDirection = true;

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
        GetClosestTarget();
        CheckTargetDistance();

        if (UpdateAimDirection)
        {
            AimAtTarget();
        }
    }

    private void GetClosestTarget()
    {
        if (CurrentTargetExists()) return;

        Transform closestTarget = null;

        float closestDistance = Mathf.Infinity;

        int numTargets = hitColliders.Length;

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
                continue;
            }

            float dist = Vector3.Distance(hitColliders[i].transform.position, pivotPoint.position);

            if (dist < closestDistance)
            {
                closestTarget = hitColliders[i].transform;
                closestDistance = dist;
            }
        }

        currentTarget = closestTarget;
    }

    public bool CurrentTargetExists()
    {
        if (currentTarget == null)
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
            Vector3 direction = currentTarget.position - firePoint.position;

            if (offsetFirePoint)
            {
                direction = currentTarget.position - (firePoint.position + firePoint.forward * firePointOffset);
            }

            AimDirection = direction.normalized;

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


    public Vector3 GetTargetPosition()
    {
        if (!CurrentTargetExists())
        {
            return Vector3.zero;
        }

        return currentTarget.position;
    }

    private void CheckTargetDistance()
    {
        if (CurrentTargetExists())
        {
            if (Vector3.Distance(pivotPoint.position, currentTarget.position) > radius)
            {
                currentTarget = null;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(pivotPoint.position, radius);

        if (CurrentTargetExists())
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(firePoint.position, currentTarget.position);
        }
    }
}
