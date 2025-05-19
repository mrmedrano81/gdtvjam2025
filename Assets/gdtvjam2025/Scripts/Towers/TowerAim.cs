using UnityEngine;

public class TowerAim : MonoBehaviour
{
    [Header("References")]
    public Transform towerBody;
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


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GetClosestTarget();
        CheckTargetDistance();
        AimAtTarget();
    }

    private void GetClosestTarget()
    {
        if (currentTarget != null) return;

        int maxColliders = maxTargetsDetected;
        Collider[] hitColliders = new Collider[maxColliders];

        int numColliders = Physics.OverlapSphereNonAlloc(towerBody.position, radius, hitColliders, targetLayer);

        float closestDistance = Mathf.Infinity;

        Transform closestTarget = null;

        for (int i = 0; i < numColliders; i++)
        {
            if (Vector3.Distance(hitColliders[i].transform.position, towerBody.position) < closestDistance)
            {
                closestTarget = hitColliders[i].transform;
                closestDistance = Vector3.Distance(hitColliders[i].transform.position, towerBody.position);
            }
        }

        currentTarget = closestTarget;
    }

    public bool CanShoot()
    {
        return currentTarget != null;
    }

    private void AimAtTarget()
    {
        if (currentTarget != null)
        {
            Vector3 direction = currentTarget.position - firePoint.position;

            if (lockVerticalRotation)
            {
                direction = Vector3.ProjectOnPlane(direction, Vector3.up);
            }

            if (invertDirection)
            {
                direction = -direction;
            }

            Quaternion lookRotation = Quaternion.LookRotation(direction);

            towerBody.rotation = Quaternion.Slerp(towerBody.rotation, lookRotation, Time.deltaTime * rotationSpeed);

            AimDirection = direction.normalized;
        }
    }

    private void CheckTargetDistance()
    {
        if (currentTarget != null)
        {
            if (Vector3.Distance(towerBody.position, currentTarget.position) > radius)
            {
                currentTarget = null;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(towerBody.position, radius);

        if (currentTarget != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(firePoint.position, currentTarget.position);
        }
    }
}
