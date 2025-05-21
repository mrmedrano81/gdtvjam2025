using System.Collections;
using UnityEngine;

[RequireComponent(typeof(TowerAim))]
public class MissileTowerScript : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private bool addBulletSpread = true;
    [SerializeField] private Vector3 bulletSpreadVariance = Vector3.zero;
    [SerializeField] private float maxWidthMult;
    [SerializeField] private float missileLockOnDuration;
    [SerializeField] private LayerMask mask;

    [Header("References")]
    [SerializeField] private Transform EffectStartPosition;
    [SerializeField] private Transform EffectEndPosition;
    [SerializeField] private Transform bulletSpawnPosition;
    [SerializeField] private LineRenderer laserTrail;
    [SerializeField] private ParticleSystem shootingParticleSystem;
    [SerializeField] private ParticleSystem impactParticleSystem;

    private float lastShootTime;
    private TowerAim towerAim;
    private TowerStats towerStats;


    private void Awake()
    {
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

    public void SetNormalTowerStats(TowerStats towerStats)
    {
        this.towerStats = towerStats;

        towerAim.radius = towerStats.range;
        towerAim.rotationSpeed = towerStats.rotationSpeed;
    }

    private void LaserExecutionSequence()
    {
        if (Time.time - lastShootTime > 1 / towerStats.fireRate && towerAim.CurrentTargetExists())
        {
            LineRenderer laserTrail = Instantiate(this.laserTrail, EffectStartPosition.position, Quaternion.identity);

            StartCoroutine(ExecuteLaser(laserTrail));
            lastShootTime = Time.time;
        }
    }

    private IEnumerator ExecuteLaser(LineRenderer laserRenderer)
    {
        float time = 0;

        Vector3 midPoint = (EffectStartPosition.position + EffectEndPosition.position) / 2;

        laserRenderer.SetPosition(0, EffectStartPosition.position);
        laserRenderer.SetPosition(1, midPoint);
        laserRenderer.SetPosition(2, EffectEndPosition.position);

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

        Vector3 lastAimPosition = towerAim.AimPosition;

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

            midPoint = (EffectStartPosition.position + towerAim.AimPosition) / 2;
            laserRenderer.SetPosition(0, EffectStartPosition.position);
            laserRenderer.SetPosition(1, midPoint);
            laserRenderer.SetPosition(2, towerAim.AimPosition);

            lastAimPosition = towerAim.AimPosition;

            time += Time.deltaTime / missileLockOnDuration;

            if (!towerAim.CurrentTargetExists())
            {
                time = 2;
            }

            yield return null;
        }

        Destroy(laserRenderer.gameObject);
    }
}
