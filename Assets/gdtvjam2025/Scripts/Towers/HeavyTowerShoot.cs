using System.Collections;
using UnityEngine;

public class HeavyTowerShoot : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private bool addBulletSpread = true;
    [SerializeField] private float fireRate = 2f;
    [SerializeField] private float shootDelay = 0.5f;
    [SerializeField] private Vector3 bulletSpreadVariance = Vector3.zero;
    [SerializeField] private LayerMask mask;

    [Header("References")]
    [SerializeField] private Transform EffectfirePoint;
    [SerializeField] private Transform bulletSpawnPosition;
    [SerializeField] private TrailRenderer bulletTrail;
    [SerializeField] private ParticleSystem shootingParticleSystem;
    [SerializeField] private ParticleSystem impactParticleSystem;

    //private Animator animator;
    private float lastShootTime;
    private TowerAim towerAim;


    private void Awake()
    {
        //animator = GetComponent<Animator>();
        towerAim = GetComponent<TowerAim>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (towerAim.CanShoot())
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        if (Time.time - lastShootTime > 1 / fireRate)
        {
            //animator.SetBool("IsShooting", true);

            Vector3 direction = towerAim.AimDirection;

            if (Physics.Raycast(bulletSpawnPosition.position, direction, out RaycastHit hit, Mathf.Infinity, mask.value))
            {
                if (addBulletSpread)
                {
                    Vector3 randomSpread = new Vector3(
                        Random.Range(-bulletSpreadVariance.x, bulletSpreadVariance.x),
                        Random.Range(-bulletSpreadVariance.y, bulletSpreadVariance.y),
                        Random.Range(-bulletSpreadVariance.z, bulletSpreadVariance.z)
                    );

                    direction += randomSpread;
                }

                shootingParticleSystem.Play();
                TrailRenderer trail = Instantiate(bulletTrail, EffectfirePoint.position, Quaternion.identity);
                StartCoroutine(SpawnTrail(trail, hit));
                trail.transform.forward = towerAim.GetAimDirection(EffectfirePoint);

                Instantiate(impactParticleSystem, hit.point, Quaternion.LookRotation(-direction), hit.collider.gameObject.transform);

                lastShootTime = Time.time;
            }
        }
    }

    private IEnumerator SpawnTrail(TrailRenderer trail, RaycastHit hit)
    {
        float time = 0;
        Vector3 startPos = trail.transform.position;

        while (time < 1)
        {
            trail.transform.position = Vector3.Lerp(startPos, hit.point, time);
            time += Time.deltaTime / trail.time;

            yield return null;
        }

        //animator.SetBool("IsShooting", false);
        trail.transform.position = hit.point;

        Destroy(trail.gameObject, trail.time);
    }
}
