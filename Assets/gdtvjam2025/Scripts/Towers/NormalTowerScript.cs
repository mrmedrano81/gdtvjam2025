using System.Collections;
using UnityEngine;

[RequireComponent(typeof(TowerAim))]
public class NormalTowerScript : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private bool addBulletSpread = true;
    [SerializeField] private Vector3 bulletSpreadVariance = Vector3.zero;
    [SerializeField] private LayerMask mask;

    [Header("References")]
    [SerializeField] private Transform[] EffectfirePoint; 
    [SerializeField] private Transform bulletSpawnPosition; 
    //[SerializeField] private TrailRenderer bulletTrail; 
    [SerializeField] private LineRenderer bulletLineRenderer; 
    [SerializeField] private ParticleSystem[] shootingParticleSystem;
    [SerializeField] private ParticleSystem impactParticleSystem;

    public NormalBulletLinePool bulletTrailPool;
    public EnemyHitEffectPool hitEffectPool;

    private float lastShootTime;
    private TowerAim towerAim;
    private TowerStats towerStats;


    private void Awake()
    {
        towerAim = GetComponent<TowerAim>();
        bulletTrailPool = FindFirstObjectByType<NormalBulletLinePool>();
        hitEffectPool = FindFirstObjectByType<EnemyHitEffectPool>();
    }

    // Update is called once per frame
    void Update()
    {
        if (towerAim.CurrentTargetExists())
        {
            Shoot();
        }
    }

    public void SetNormalTowerStats(TowerStats towerStats)
    {
        this.towerStats = towerStats;

        towerAim.radius = towerStats.range;
        towerAim.rotationSpeed = towerStats.rotationSpeed;
    }

    private void Shoot()
    {
        if (Time.time - lastShootTime > 1/towerStats.fireRate)
        {
            //animator.SetBool("IsShooting", true);

            Vector3 direction = towerAim.AimDirection;

            if (Physics.Raycast(bulletSpawnPosition.position, direction, out RaycastHit hit, Mathf.Infinity, mask.value, QueryTriggerInteraction.Collide))
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


                //int gunNumber = Random.Range(0, EffectfirePoint.Length);

                //GameObject trailObject = bulletTrailPool.GetObject();
                //TrailRenderer trail = trailObject.GetComponent<TrailRenderer>();


                //if (gunNumber < 2)
                //{
                //    for (int i = 0; i < 2; i++)
                //    {
                //        SpawnBulletLine(EffectfirePoint[i].position, hit.point);
                //        //trailObject.transform.position = EffectfirePoint[i].position;
                //        //StartCoroutine(SpawnTrail(trail, hit, i, trailObject));
                //        //trail.transform.forward = towerAim.GetAimDirection(EffectfirePoint[i]);
                //    }
                //}
                //else
                //{
                //    for (int i = 2; i < 4; i++)
                //    {
                //        SpawnBulletLine(EffectfirePoint[i].position, hit.point);
                //        //trailObject.transform.position = EffectfirePoint[i].position;
                //        //StartCoroutine(SpawnTrail(trail, hit, i, trailObject));
                //        //trail.transform.forward = towerAim.GetAimDirection(EffectfirePoint[i]);
                //    }
                //}

                // ------------- random firepoint ---------------- //

                int gunNumber = Random.Range(0, EffectfirePoint.Length);
                shootingParticleSystem[gunNumber].Play();
                SpawnBulletLine(EffectfirePoint[gunNumber].position, hit.point);

                // ------------- All firepoints ---------------- //
                //for (int i = 0; i < EffectfirePoint.Length; i++)
                //{
                //    shootingParticleSystem[i].Play();
                //    TrailRenderer trail = Instantiate(bulletTrail, EffectfirePoint[i].position, Quaternion.identity);
                //    StartCoroutine(SpawnTrail(trail, hit));
                //    trail.transform.forward = towerAim.GetAimDirection(EffectfirePoint[i]);
                //}


                Health health = hit.collider.GetComponentInParent<Health>();

                if (health != null)
                {
                    health.TakeDamage(towerStats.damage);
                }

                EnemyCollisionHandler enemyCollisionHandler = hit.collider.GetComponent<EnemyCollisionHandler>();

                if (enemyCollisionHandler != null)
                {
                    enemyCollisionHandler.PlayHitEffectAt(hit.point);
                }
                else
                {
                    Debug.LogError($"No EnemyCollisionHandler found on {hit.collider.name}");
                }

                //Instantiate(impactParticleSystem, hit.point, Quaternion.LookRotation(-direction), hit.collider.gameObject.transform);

                lastShootTime = Time.time;
            }
        }
    }

    private void SpawnBulletLine(Vector3 startPoint, Vector3 endPoint)
    {
        GameObject bulletTrail = bulletTrailPool.GetObject();

        LineRenderer lr = bulletTrail.GetComponent<LineRenderer>();

        lr.SetPosition(0, startPoint);
        lr.SetPosition(1, endPoint);
    }

    //private IEnumerator SpawnTrail(TrailRenderer trail, RaycastHit hit, int index, GameObject trailObject)
    //{
    //    float randomDelay = Random.Range(0, 0.1f);

    //    yield return new WaitForSeconds(randomDelay);

    //    shootingParticleSystem[index].Play();

    //    float time = 0;
    //    Vector3 startPos = trail.transform.position;

    //    while(time < 1)
    //    {
    //        trail.transform.position = Vector3.Lerp(startPos, hit.point, time);
    //        time += Time.deltaTime/trail.time;

    //        yield return null;
    //    }

    //    //animator.SetBool("IsShooting", false);
    //    trail.transform.position = hit.point;

    //    bulletTrailPool.ReturnObject(trailObject);
    //}
}
