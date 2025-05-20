using System.Collections;
using UnityEngine;

[RequireComponent(typeof(TowerAim))]
public class HeavyTowerScript : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private bool addBulletSpread = true;
    public float laserLockonDuration = 0.7f;
    [SerializeField] private LayerMask mask;

    [Header("VFX Settings")]
    [SerializeField] private float maxWidthMult;

    [Header("References")]
    [SerializeField] private Transform EffectfirePoint;
    [SerializeField] private Transform bulletSpawnPosition;
    [SerializeField] private LineRenderer laserTrail;
    [SerializeField] private ParticleSystem shootingParticleSystem;
    [SerializeField] private ParticleSystem impactParticleSystem;
    [SerializeField] private GameObject explosionPrefab;

    //private Animator animator;
    private float lastShootTime;
    private TowerAim towerAim;
    private TowerStats towerStats;

    private void Awake()
    {
        //animator = GetComponent<Animator>();
        towerAim = GetComponent<TowerAim>();
    }

    // Update is called once per frame
    void Update()
    {
        if (towerAim.CurrentTargetExists())
        {
            LaserExecutionSequence();
        }
    }

    public void SetHeavyTowerStats(TowerStats towerStats)
    {
        this.towerStats = towerStats;

        towerAim.radius = towerStats.range;
        towerAim.rotationSpeed = towerStats.rotationSpeed;
    }

    private void LaserExecutionSequence()
    {
        if (Time.time - lastShootTime > 1 / towerStats.fireRate && towerAim.CurrentTargetExists())
        {
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

        Gradient originalGradient = laserRenderer.colorGradient;

        GradientColorKey[] colorKeys = originalGradient.colorKeys;
        GradientAlphaKey[] alphaKeys = originalGradient.alphaKeys;

        float[] originalAlphas = new float[alphaKeys.Length];
        for (int i = 0; i < alphaKeys.Length; i++)
        {
            originalAlphas[i] = alphaKeys[i].alpha;
            alphaKeys[i].alpha = 0f;
        }

        Gradient fadeGradient = new Gradient();
        fadeGradient.SetKeys(colorKeys, alphaKeys);
        laserRenderer.colorGradient = fadeGradient;

        Vector3 lastAimDirection = towerAim.GetAimDirection(bulletSpawnPosition);

        while (time < 1f)
        {
            float t = time;

            for (int i = 0; i < alphaKeys.Length; i++)
            {
                alphaKeys[i].alpha = Mathf.Lerp(0f, originalAlphas[i], t);
            }

            fadeGradient.SetKeys(colorKeys, alphaKeys);
            laserRenderer.colorGradient = fadeGradient;

            laserRenderer.widthMultiplier = Mathf.Lerp(maxWidthMult, 0, t);

            midPoint = (EffectfirePoint.position + towerAim.GetTargetPosition()) / 2;
            laserRenderer.SetPosition(0, EffectfirePoint.position);
            laserRenderer.SetPosition(1, midPoint);
            laserRenderer.SetPosition(2, towerAim.GetTargetPosition());

            lastAimDirection = towerAim.AimDirection;

            time += Time.deltaTime / laserLockonDuration;

            if (!towerAim.CurrentTargetExists())
            {
                time = 2;
            }

            yield return null;
        }

        if (towerAim.CurrentTargetExists())
        {
            ShootAtTarget();
        }
        else
        {
            ShootAtLastDirection(lastAimDirection);
        }

        Destroy(laserRenderer.gameObject);
    }



    private void ShootAtTarget()
    {
        shootingParticleSystem.Play();

        if (Physics.Raycast(bulletSpawnPosition.position, towerAim.GetAimDirection(bulletSpawnPosition), out RaycastHit hit, Mathf.Infinity, mask.value))
        {
            Instantiate(impactParticleSystem, towerAim.GetTargetPosition(), Quaternion.LookRotation(-towerAim.AimDirection), hit.collider.gameObject.transform);
        }
        else
        {
            Debug.LogError("[HEAVY] Has target but no hit detected");
        }
    }

    private void ShootAtLastDirection(Vector3 lastAimDirection)
    {
        shootingParticleSystem.Play();

        if (Physics.Raycast(bulletSpawnPosition.position, lastAimDirection, out RaycastHit hit, Mathf.Infinity))
        {
            Instantiate(impactParticleSystem, hit.point, Quaternion.LookRotation(-towerAim.AimDirection));
        }
        else
        {
            Debug.LogError("[HEAVY] No target and no hit detected");
        }
    }
}
