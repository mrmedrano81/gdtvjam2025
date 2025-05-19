using UnityEngine;

public class TowerAim : MonoBehaviour
{
    [Header("References")]
    public Transform pivotPoint;
    public Transform firePoint;
    private Transform currentTarget;

    [Header("Settings")]
    public float radius = 5f;
    public float rotationSpeed = 10f;
    public LayerMask targetLayer;
    public int maxTargetsDetected;
    public bool lockVerticalRotation = true;
    public bool invertDirection = false;

    public Vector3 AimDirection { get; private set; }

    public bool UpdateAimDirection = true;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
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
        if (currentTarget != null) return;

        int maxColliders = maxTargetsDetected;
        Collider[] hitColliders = new Collider[maxColliders];

        int numColliders = Physics.OverlapSphereNonAlloc(pivotPoint.position, radius, hitColliders, targetLayer);

        float closestDistance = Mathf.Infinity;

        Transform closestTarget = null;

        for (int i = 0; i < numColliders; i++)
        {
            if (Vector3.Distance(hitColliders[i].transform.position, pivotPoint.position) < closestDistance)
            {
                closestTarget = hitColliders[i].transform;
                closestDistance = Vector3.Distance(hitColliders[i].transform.position, pivotPoint.position);
            }
        }

        currentTarget = closestTarget;
    }

    public bool CanShoot()
    {
        return currentTarget != null;
    }

    public Vector3 GetAimDirection(Transform firePointTransform)
    {
        return currentTarget.position - firePointTransform.position;
    }

    private void AimAtTarget()
    {
        if (currentTarget != null)
        {
            Vector3 direction = currentTarget.position - firePoint.position;

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
        if (currentTarget == null)
        {
            return Vector3.zero;
        }

        return currentTarget.position;
    }

    private void CheckTargetDistance()
    {
        if (currentTarget != null)
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

        if (currentTarget != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(firePoint.position, currentTarget.position);
        }
    }
}
