using System.Collections;
using UnityEngine;

[RequireComponent(typeof(TowerAim))]
public class HeavyTowerShoot : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private bool addBulletSpread = true;
    [SerializeField] private float fireRate = 2f;
    [SerializeField] private float laserShotDuration = 0.7f;
    [SerializeField] private Vector3 bulletSpreadVariance = Vector3.zero;
    [SerializeField] private LayerMask mask;

    [Header("VFX Settings")]
    [SerializeField] private float maxWidthMult;

    [Header("References")]
    [SerializeField] private Transform EffectfirePoint;
    [SerializeField] private Transform bulletSpawnPosition;
    [SerializeField] private LineRenderer laserTrail;
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
        if (Time.time - lastShootTime > 1 / fireRate && towerAim.CanShoot())
        {
            //animator.SetBool("IsShooting", true);

            //Vector3 direction = towerAim.AimDirection;

            //if (Physics.Raycast(bulletSpawnPosition.position, direction, out RaycastHit hit, Mathf.Infinity, mask.value))
            //{
            //    if (addBulletSpread)
            //    {
            //        Vector3 randomSpread = new Vector3(
            //            Random.Range(-bulletSpreadVariance.x, bulletSpreadVariance.x),
            //            Random.Range(-bulletSpreadVariance.y, bulletSpreadVariance.y),
            //            Random.Range(-bulletSpreadVariance.z, bulletSpreadVariance.z)
            //        );

            //        direction += randomSpread;
            //    }

            //    shootingParticleSystem.Play();
            //    TrailRenderer trail = Instantiate(projectileTrail, EffectfirePoint.position, Quaternion.identity);
            //    LineRenderer laserTrail = Instantiate(this.laserTrail, EffectfirePoint.position, Quaternion.identity);
            //    StartCoroutine(SpawnTrail(trail, laserTrail, hit));
            //    trail.transform.forward = towerAim.GetAimDirection(EffectfirePoint);
            //    lastShootTime = Time.time;
            //}


            shootingParticleSystem.Play();

            LineRenderer laserTrail = Instantiate(this.laserTrail, EffectfirePoint.position, Quaternion.identity);

            StartCoroutine(SpawnTrail(laserTrail));
            lastShootTime = Time.time;

        }
    }

    private IEnumerator SpawnTrail(LineRenderer laserRenderer)
    {
        float time = 0;

        Vector3 midPoint = (EffectfirePoint.position + towerAim.GetTargetPosition()) /2;

        laserRenderer.SetPosition(0, EffectfirePoint.position);
        laserRenderer.SetPosition(1, midPoint);
        laserRenderer.SetPosition(2, towerAim.GetTargetPosition());

        laserRenderer.enabled = true;

        while (time < 1)
        {
            laserRenderer.widthMultiplier = Mathf.Lerp(maxWidthMult, 0, time);

            time += Time.deltaTime / laserShotDuration;

            midPoint = (EffectfirePoint.position + towerAim.GetTargetPosition()) / 2;

            laserRenderer.SetPosition(0, EffectfirePoint.position);
            laserRenderer.SetPosition(1, midPoint);
            laserRenderer.SetPosition(2, towerAim.GetTargetPosition());

            yield return null;
        }

        if (Physics.Raycast(bulletSpawnPosition.position, towerAim.GetAimDirection(bulletSpawnPosition), out RaycastHit hit, Mathf.Infinity, mask.value))
        {
            Instantiate(impactParticleSystem, towerAim.GetTargetPosition(), Quaternion.LookRotation(-towerAim.AimDirection), hit.collider.gameObject.transform);
        }
        else
        {
            Debug.LogError("[HEAVY] No hit detected");
        }

        Destroy(laserRenderer.gameObject);
    }

    //private IEnumerator SpawnTrail(TrailRenderer trail, RaycastHit hit)
    //{
    //    float time = 0;
    //    Vector3 startPos = trail.transform.position;

    //    while (time < 1)
    //    {
    //        //trail.transform.position = Vector3.Lerp(startPos, hit.point, time);
    //        trail.transform.position = Vector3.Lerp(startPos, hit.point, time);
    //        time += Time.deltaTime / trail.time;

    //        yield return null;
    //    }

    //    //animator.SetBool("IsShooting", false);
    //    trail.transform.position = hit.point;

    //    Instantiate(impactParticleSystem, hit.point, Quaternion.LookRotation(-towerAim.AimDirection), hit.collider.gameObject.transform);

    //    Destroy(trail.gameObject, trail.time);
    //}
}
