using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TowerAim))]
public class MissileTowerScript : MonoBehaviour
{
    [Header("Settings")]
    public int missileCount = 3;
    public float chargeupDuration = 2f;
    public float chargeInterval = 0.3f;
    [SerializeField] private float chargeupHeight = 5f;

    [Header("VFX Settings")]
    [SerializeField] private float maxWidthMult;

    [Header("References")]
    [SerializeField] private Transform EffectfirePoint;
    [SerializeField] private Transform bulletSpawnPosition;
    [SerializeField] private LineRenderer chargeupTrail;
    [SerializeField] private ParticleSystem shootingParticleSystem;
    [SerializeField] private GameObject attackPrefab;

    //private Animator animator;
    private float lastShootTime;
    private TowerAim towerAim;
    private TowerStats towerStats;
    private Coroutine chargeUp;

    private List<LineRenderer> InstantiatedChargupLines = new();
    private List<Coroutine> chargeupCoroutines = new();

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
            AttackExecutionSequence();
        }
    }

    public void OnTowerRemoved()
    {
        if (chargeUp != null)
        {
            StopCoroutine(chargeUp);
            chargeUp = null;
        }

        InstantiatedChargupLines.ForEach(laser => Destroy(laser.gameObject));
        chargeupCoroutines.ForEach(routine => StopCoroutine(routine));
    }

    public void SetMissileTowerStats(TowerStats towerStats)
    {
        this.towerStats = towerStats;

        towerAim.radius = towerStats.range;
        towerAim.rotationSpeed = towerStats.rotationSpeed;
    }

    private void AttackExecutionSequence()
    {
        if (Time.time - lastShootTime > 1 / towerStats.fireRate
            && towerAim.CurrentTargetExists()
            && chargeUp == null)
        {
            chargeUp = StartCoroutine(ExecuteChargeup());
        }
    }

    private IEnumerator ExecuteLaunch(LineRenderer chargeupLineRenderer)
    {
        float time = 0;

        Vector3 lineEnd = new Vector3(EffectfirePoint.position.x, EffectfirePoint.position.y * chargeupHeight, EffectfirePoint.position.z);

        Vector3 midPoint = (EffectfirePoint.position + lineEnd) / 2;

        chargeupLineRenderer.SetPosition(0, EffectfirePoint.position);
        chargeupLineRenderer.SetPosition(1, midPoint);
        chargeupLineRenderer.SetPosition(2, lineEnd);

        chargeupLineRenderer.enabled = true;

        Gradient originalGradient = chargeupLineRenderer.colorGradient;

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
        chargeupLineRenderer.colorGradient = fadeGradient;

        Vector3 lastAimPosition = towerAim.AimPosition;

        while (time < 1f)
        {
            float t = time;

            for (int i = 0; i < alphaKeys.Length; i++)
            {
                alphaKeys[i].alpha = Mathf.Lerp(0f, originalAlphas[i], t);
            }

            fadeGradient.SetKeys(colorKeys, alphaKeys);
            chargeupLineRenderer.colorGradient = fadeGradient;

            chargeupLineRenderer.widthMultiplier = Mathf.Lerp(maxWidthMult, 0, t);

            if (towerAim.CurrentTargetExists())
            {
                lastAimPosition = towerAim.AimPosition;
            }

            midPoint = (EffectfirePoint.position + lineEnd) / 2;

            chargeupLineRenderer.SetPosition(0, EffectfirePoint.position);
            chargeupLineRenderer.SetPosition(1, midPoint);
            chargeupLineRenderer.SetPosition(2, lineEnd);

            time += Time.deltaTime / chargeupDuration;

            yield return null;
        }

        if (!towerAim.CurrentTargetExists())
        {
            ShootAtLastTargetPosition(lastAimPosition, towerAim.currentTarget);
        }
        else
        {
            ShootAtLastTargetPosition(lastAimPosition, null);
        }

        towerAim.UpdateAimDirection = true;

        lastShootTime = Time.time;

        InstantiatedChargupLines.Remove(chargeupLineRenderer);

        Destroy(chargeupLineRenderer.gameObject);
    }

    private IEnumerator ExecuteChargeup()
    {
        int count = 0;

        while (count < 3)
        {
            LineRenderer chargeupTrail = Instantiate(this.chargeupTrail, EffectfirePoint.position, Quaternion.identity);

            InstantiatedChargupLines.Add(chargeupTrail);

            Coroutine chargeupCoroutine = StartCoroutine(ExecuteLaunch(chargeupTrail));

            chargeupCoroutines.Add(chargeupCoroutine);

            yield return new WaitForSeconds(chargeInterval);

            count++;
        }

        chargeUp = null;
    }

    private void ShootAtLastTargetPosition(Vector3 lastAimposition, Transform followTransform = null)
    {
        shootingParticleSystem.Play();

        GameObject missileAttackObject = Instantiate(attackPrefab, transform.up * 15f, Quaternion.identity);

        MissileAttack missileAttack = missileAttackObject.GetComponent<MissileAttack>();

        missileAttack.damage = towerStats.damage;
        //missileAttack.duration = chargeupDuration;
        missileAttack.lineHeight = chargeupHeight;
        missileAttack.followTransform = followTransform;
        missileAttack.targetPosition = lastAimposition;

        missileAttack.StartTracking();
    }
}
