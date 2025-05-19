using System.Collections;
using UnityEngine;

[RequireComponent(typeof(TowerAim))]
public class HeavyTowerScript : MonoBehaviour
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
            shootingParticleSystem.Play();

            LineRenderer laserTrail = Instantiate(this.laserTrail, EffectfirePoint.position, Quaternion.identity);

            StartCoroutine(ExecuteLaser(laserTrail));
            lastShootTime = Time.time;
        }
    }

    private IEnumerator ExecuteLaser(LineRenderer laserRenderer)
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
}
